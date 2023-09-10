// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include <metahost.h>
#include <mscoree.h>
#pragma comment(lib, "user32.lib")
#pragma comment(lib, "mscoree.lib")

extern void csv3_init(HMODULE hModule);

ICLRRuntimeHost *pRuntimeHost = NULL;

void ThrowError(const char *message, HRESULT hr)
{
	LPSTR lpMsgBuf;
	FormatMessageA(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL,
		hr,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPSTR)&lpMsgBuf,
		0, NULL
		);

	if (FAILED(hr))
	{
		dbg_printf("%s failed with HR:%x (%s)\n", message, hr, lpMsgBuf);
	}
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		DisableThreadLibraryCalls(hModule);
		csv3_init(hModule);
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

