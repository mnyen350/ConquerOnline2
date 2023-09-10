// CSV3Hook.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "detours.h"
#include <iostream>
#include <stdarg.h>
#include "LegacyCipher.h"
#include "Memory.h"

#define HOOK_VER	SVN_REVISION

const int VIEW_DISTANCE = 32;
const int JUMP_DISTANCE = VIEW_DISTANCE * 8 / 9;

//
// hooks.h is from a library/project (MID) that I (InfamousNoone) wrote back in 2010.
// It was a /FAILED/ project, and I decided to discontinue it.
// If anyone is interested in it, they can see the following link below:
// http://subversion.assembla.com/svn/mid_project/trunk/MID_LIBRARY/
//

#define lib_func(lib, func) (GetProcAddress(GetModuleHandleA(lib), func))

char szPassword[32];
char szServer[32];
char szConfig[MAX_PATH]; 

extern HANDLE g_pCurrentProcess = GetCurrentProcess();

typedef int (WINAPI *LPFCONNECT)(SOCKET s, sockaddr *name, int namelen);
typedef int (WINAPI *LPFRECV)(SOCKET s, char *buf, int len, int flags);
typedef int (WINAPI* LPFSEND)(SOCKET s, char* buf, int len, int flags);
typedef HINSTANCE (WINAPI *LPFSHELLEXEC)(HWND hwnd, char* lpOperation, char* lpFile, char* lpParameters, char* lpDirectory, INT nShowCmd);
typedef int (__cdecl *LPFSNPRINTF)(char* buffer, size_t count, const char* format, ...);


LPFCONNECT pConnectOriginal = (LPFCONNECT)DetourFindFunction("ws2_32.dll", "connect");
LPFRECV pRecvOriginal = (LPFRECV)DetourFindFunction("ws2_32.dll", "recv");
LPFRECV pSendOriginal = (LPFRECV)DetourFindFunction("ws2_32.dll", "send");
LPFSHELLEXEC pShellExecuteOriginal = (LPFSHELLEXEC)DetourFindFunction("shell32.dll", "ShellExecuteA");
LPFSNPRINTF psnprintfOriginal = (LPFSNPRINTF)DetourFindFunction("msvcr90.dll", "_snprintf");

PBYTE pMsgBuf=NULL;
PBYTE pPostHandlerTrampoline;
PBYTE pNetworkObj=NULL;

CLegacyCipher* pcs_legacy=NULL;

unsigned long parse_saddr(char* str)
{
	unsigned long saddr = 0;
	unsigned char *paddr = (unsigned char*)&saddr;
	hostent *pent = gethostbyname(str);

	if (pent && pent->h_addr_list[0])
		return ((in_addr*)pent->h_addr_list[0])->s_addr;
	else if (sscanf(str, "%d.%d.%d.%d", &paddr[0], &paddr[1], &paddr[2], &paddr[3]) == 4)
		return saddr;
	else if (sscanf(str, "%d", &saddr) == 1)
		return saddr;
	return -1;
}

int WINAPI csv3_connect(SOCKET s, sockaddr *name, int namelen)
{
	sockaddr_in *name_in = (sockaddr_in *)name;

	int rPort = ntohs(name_in->sin_port);
	if (rPort >= 9950 && rPort <= 9970)
	{
		char szIPAddress[32];
		GetPrivateProfileStringA("auth", "host", "", szIPAddress, 32, szConfig);
		int nPort = GetPrivateProfileIntA("auth", "port", 9959, szConfig);
		if (strcmp(szIPAddress, "") != 0)
			name_in->sin_addr.s_addr = parse_saddr(szIPAddress);
		name_in->sin_port = htons(nPort);

		dbg_printf("Auth Server %s:%d\n", inet_ntoa(name_in->sin_addr), nPort);
	}
	
	return pConnectOriginal(s, name, namelen);
}

int WINAPI csv3_send(SOCKET s, char* buf, int len, int flags)
{
	//dbg_printf("Send %d\n", len);
	return pSendOriginal(s, buf, len, flags);
}

int WINAPI csv3_recv(SOCKET s, char *buf, int len, int flags)
{
	return pRecvOriginal(s, buf, len, flags);
}

HINSTANCE WINAPI csv3_shellexec(HWND hwnd, char* lpOperation, char* lpFile, char* lpParameters, char* lpDirectory, INT nShowCmd)
{
	return (HINSTANCE)SE_ERR_FNF;
}

int csv3_snprintf(char* str, int len, const char* format, ...)
{
	static const char TQ_FORMAT[] = { 0x25, 0x73, 0xA3, 0xAC, 0xA1, 0xA3, 0x66, 0x64, 0x6A, 0x66, 0x2C, 0x6A, 0x6B, 0x67, 0x66, 0x6B, 0x6C, 0x00 };
	va_list args;
	va_start(args, format);
	if (strcmp(format, TQ_FORMAT) == 0)
	{
		char* password = va_arg(args, PCHAR);
		strcpy(szPassword, password);
		str[0] = NULL;
		strcat(str, password);
		strcat(str, &TQ_FORMAT[2]);

		//MessageBoxA(NULL, szPassword, "Password", MB_OK);

		return strlen(str);
	}
	else
	{
		return vsnprintf(str, len, format, args);
	}
}

LONG Detour(PVOID *ppPointer, PVOID pDetour)
{
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach(ppPointer, pDetour);
	return DetourTransactionCommit();
}

void csv3_init(HMODULE hModule)
{
	float f = 0; // load support for floating operations -- thx ntl3fty!

#ifdef CONSOLE_ENABLED
	AllocConsole();
	SetConsoleTitleA("Hook Debug Console");
	freopen("CONOUT$", "w", stdout);
#endif
	
	//
	// obtain csv3config.ini's path
	//
	strcat(GetLocalPath(szConfig), "hook.ini");
	memset(szPassword, 0, 32);

	//
	// install standard hooks
	//

	if (NO_ERROR != Detour(&(PVOID&)pConnectOriginal, csv3_connect))
		dbg_printf("failed to detour connect\n");

	if (NO_ERROR != Detour(&(PVOID&)pRecvOriginal, csv3_recv))
		dbg_printf("failed to detour recv\n");

	if (NO_ERROR != Detour(&(PVOID&)pSendOriginal, csv3_send))
		dbg_printf("failed to detour send\n");

	if (NO_ERROR != Detour(&(PVOID&)pShellExecuteOriginal, csv3_shellexec))
		dbg_printf("failed to detour ShellExecuteA\n");

	if (NO_ERROR != Detour(&(PVOID&)psnprintfOriginal, csv3_snprintf))
		dbg_printf("failed to detour _snprintf\n");


	Memory memory;
	LPBYTE pMatch;
	if (!memory.FindMemoryPattern("8B 44 24 04 2B 44 24 0C 50", pMatch))
	{
		dbg_printf("failed to locate CGameMap::Outof9Block function\n");
	}
	else
	{
		uint8_t patch = VIEW_DISTANCE;
		GetMemory()->WriteMemory(&pMatch[16], &patch, sizeof(patch));
		GetMemory()->WriteMemory(&pMatch[36], &patch, sizeof(patch));

		dbg_printf("CGameMap::Outof9Block function patched: %p\n", pMatch);
	}

	if (!GetMemory()->FindMemoryPattern("0F 8E ?? ?? ?? ?? 83 F8 10 7E ??", 0, pMatch))
	{
		dbg_printf("failed to locate JumpDistanceCheck #1\n");
	}
	else
	{
		uint8_t patch = JUMP_DISTANCE;
		GetMemory()->WriteMemory(&pMatch[8], &patch, sizeof(patch));

		dbg_printf("JumpDistanceCheck #1 patched: %p\n", pMatch);
	}

	if (!memory.FindMemoryPattern("0F 8E ?? ?? ?? ?? 83 F8 10 7E ??", 1, pMatch))
	{
		dbg_printf("failed to locate JumpDistanceCheck #2\n");
	}
	else
	{
		uint8_t patch = JUMP_DISTANCE;
		GetMemory()->WriteMemory(&pMatch[8], &patch, sizeof(patch));

		dbg_printf("JumpDistanceCheck #2 patched: %p\n", pMatch);
	}
}