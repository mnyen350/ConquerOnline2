#pragma once

#include "stdafx.h"
#include "Scanner.h"
#include <map>
#include <cstring>

typedef class Memory
{
private:
	typedef struct
	{
		HMODULE hModule;
		PBYTE pBuffer;
		SIZE_T dwSize;
	} memorymap_t, * LPMEMORYMAP;

	typedef std::map<char*, LPMEMORYMAP> MAP_MEMORY;

public:
	Memory()
	{
		this->m_pScanner = new Scanner();
		this->m_mapMemory = new MAP_MEMORY();
	}

	BOOL ReadMemory(void* lpBaseAddress, void* lpBuffer, SIZE_T nSize, SIZE_T* lpNumberOfBytesRead);

	BOOL ReadMemory(void* lpBaseAddress, void* lpBuffer, SIZE_T nSize)
	{
		return this->ReadMemory(lpBaseAddress, lpBuffer, nSize, NULL);
	}

	BOOL WriteMemory(void* lpBaseAddress, void* lpBuffer, SIZE_T nSize, SIZE_T* lpNumberOfBytesWritten);

	BOOL WriteMemory(void* lpBaseAddress, void* lpBuffer, SIZE_T nSize)
	{
		return this->WriteMemory(lpBaseAddress, lpBuffer, nSize, NULL);
	}

	bool FindMemoryPattern(char* lpPattern, char* lpModuleName, DWORD dwSkip, LPBYTE& lpAddress);

	bool FindMemoryPattern(char* lpPattern, DWORD dwSkip, LPBYTE& lpAddress)
	{
		return FindMemoryPattern(lpPattern, NULL, dwSkip, lpAddress);
	}

	bool FindMemoryPattern(char* lpPattern, LPBYTE& lpAddress)
	{
		return FindMemoryPattern(lpPattern, NULL, 0, lpAddress);
	}

	int ReleaseMemoryMap();

private:
	bool GetModuleSize(HANDLE hProcess, void* lpImageBase, DWORD& dwSize);

private:
	Scanner* m_pScanner;
	MAP_MEMORY* m_mapMemory;

} *LPMEMORY;

extern LPMEMORY g_pMemory;

extern LPMEMORY GetMemory();
inline LPMEMORY GetMemory()
{
	if (!g_pMemory)
		g_pMemory = new Memory();

	return g_pMemory;
}
