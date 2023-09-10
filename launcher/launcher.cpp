// launcher.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "launcher.h"
#include <string>
#pragma comment(lib, "user32.lib")

char coPath[MAX_PATH] = "Conquer.exe blacknull";

void CreateShellcode(int ret, int str, unsigned char** shellcode, int* shellcodeSize)
{
	unsigned char* retChar = (unsigned char*)&ret;
	unsigned char* strChar = (unsigned char*)&str;
	int api = (int)GetProcAddress(LoadLibraryA("kernel32.dll"), "LoadLibraryA");
	unsigned char* apiChar = (unsigned char*)&api;
	unsigned char sc[] = {
		// Push ret
		0x68, retChar[0], retChar[1], retChar[2], retChar[3],
		// Push all flags
		0x9C,
		// Push all register
		0x60,
		// Push 0x66666666 (later we convert it to the string of "C:\DLLInjectionTest.dll")
		0x68, strChar[0], strChar[1], strChar[2], strChar[3],
		// Mov eax, 0x66666666 (later we convert it to LoadLibrary adress)
		0xB8, apiChar[0], apiChar[1], apiChar[2], apiChar[3],
		// Call eax
		0xFF, 0xD0,
		// Pop all register
		0x61,
		// Pop all flags
		0x9D,
		// Ret
		0xC3
	};

	*shellcodeSize = 22;
	*shellcode = (unsigned char*)malloc(22);
	memcpy(*shellcode, sc, 22);
}

void mb(char* c)
{
	//MessageBoxA(0, c, 0, MB_OK);
}

void InjectDLL2(char* dllPath, char* processStr)
{
	unsigned char* shellcode;
	int shellcodeLen;

	LPVOID remote_dllStringPtr;
	LPVOID remote_shellcodePtr;

	CONTEXT ctx;

	// Create Process SUSPENDED
	PROCESS_INFORMATION pi;
	STARTUPINFOA Startup;
	ZeroMemory(&Startup, sizeof(Startup));
	ZeroMemory(&pi, sizeof(pi));
	CreateProcessA(NULL, coPath, NULL, NULL, NULL, CREATE_SUSPENDED, NULL, NULL, &Startup, &pi);

	ResumeThread(pi.hThread);
	Sleep(1000);
	SuspendThread(pi.hThread);

	mb("Allocating Remote Memory For DLL Path\n");
	remote_dllStringPtr = VirtualAllocEx(pi.hProcess, NULL, strlen(dllPath) + 1, MEM_COMMIT, PAGE_READWRITE);
	// mb("DLL Adress: %X\n", remote_dllStringPtr);

	mb("Get EIP\n");
	ctx.ContextFlags = CONTEXT_CONTROL;
	GetThreadContext(pi.hThread, &ctx);
	//mb("EIP: %X\n", ctx.Eip);

	mb("Build Shellcode\n");
	CreateShellcode(ctx.Eip, (int)remote_dllStringPtr, &shellcode, &shellcodeLen);

	mb("Created Shellcode: \n");
	//for(int i=0; i < shellcodeLen; i++)
   //	mb ("%X ", shellcode[i]);
	mb("\n");

	mb("Allocating Remote Memory For Shellcode\n");
	remote_shellcodePtr = VirtualAllocEx(pi.hProcess, NULL, shellcodeLen, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
	//mb("Shellcode Adress: %X\n", remote_shellcodePtr);

	mb("Write DLL Path To Remote Process\n");
	WriteProcessMemory(pi.hProcess, remote_dllStringPtr, dllPath, strlen(dllPath) + 1, NULL);

	mb("Write Shellcode To Remote Process\n");
	WriteProcessMemory(pi.hProcess, remote_shellcodePtr, shellcode, shellcodeLen, NULL);

	mb("Set EIP\n");
	ctx.Eip = (DWORD)remote_shellcodePtr;
	ctx.ContextFlags = CONTEXT_CONTROL;
	SetThreadContext(pi.hThread, &ctx);

	mb("Run The Shellcode\n");
	ResumeThread(pi.hThread);

	mb("Wait Till Code Was Executed\n");
	Sleep(8000);

	mb("Free Remote Resources\n");
	VirtualFreeEx(pi.hProcess, remote_dllStringPtr, strlen(dllPath) + 1, MEM_DECOMMIT);
	VirtualFreeEx(pi.hProcess, remote_shellcodePtr, shellcodeLen, MEM_DECOMMIT);
}

int APIENTRY _tWinMain(HINSTANCE hInstance,
	HINSTANCE hPrevInstance,
	LPTSTR    lpCmdLine,
	int       nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

	InjectDLL2("Hook.dll", coPath);
	return 0;
}