#pragma once

#include "targetver.h"
#include <windows.h>
#include <cstdio>
#include <cstdint>

#pragma comment(lib, "winmm.lib")
#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib, "user32.lib")
#pragma comment(lib, "detours.lib")

#ifdef _DEBUG
#define DEBUG_MODE
#else
#undef DEBUG_MODE
#endif

#define NETWORK_CRYPTO

#ifdef DEBUG_MODE
#define CONSOLE_ENABLED
#define dbg_printf(f, ...) do { printf(f, __VA_ARGS__); debuglog(f, __VA_ARGS__); } while(0)
#define dbg_msgdump msgdump
#else
#define dbg_printf(f, ...) do { } while(0)
#define dbg_msgdump(h,b,s) do { } while(0)
#endif

extern void debuglog(const char *format, ...);
extern char* GetLocalPath(char* buffer);
extern void msgdump(const char* szHeader, void* pBuf, int nSize, bool bPrintHeaderEx = true);
extern HANDLE g_pCurrentProcess;
