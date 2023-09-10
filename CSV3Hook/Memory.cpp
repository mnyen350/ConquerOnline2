#include "Memory.h"

extern LPMEMORY g_pMemory = NULL;

BOOL Memory::ReadMemory(void* lpBaseAddress, void* lpBuffer, SIZE_T nSize, SIZE_T* lpNumberOfBytesRead)
{
	return ReadProcessMemory(g_pCurrentProcess, lpBaseAddress, lpBuffer, nSize, lpNumberOfBytesRead);
}

BOOL Memory::WriteMemory(void* lpBaseAddress, void* lpBuffer, SIZE_T nSize, SIZE_T* lpNumberOfBytesWritten)
{
	return WriteProcessMemory(g_pCurrentProcess, lpBaseAddress, lpBuffer, nSize, lpNumberOfBytesWritten);
}

bool Memory::FindMemoryPattern(char* lpPattern, char* lpModuleName, DWORD dwSkip, LPBYTE& lpAddress)
{
	char* lpModuleNameSafe = lpModuleName;
	if (lpModuleNameSafe == NULL)
		lpModuleNameSafe = "NULL";

	LPMEMORYMAP pMemoryMap;
	if (m_mapMemory->count(lpModuleNameSafe))
	{
		pMemoryMap = (*m_mapMemory)[lpModuleNameSafe];
	}
	else
	{
		DWORD dwSize;
		HMODULE hModule = GetModuleHandleA(lpModuleName);
		if (!GetMemory()->GetModuleSize(g_pCurrentProcess, hModule, dwSize))
			return NULL;

		PBYTE pBuf = new BYTE[dwSize];

		SIZE_T read;
		if (!this->ReadMemory(hModule, pBuf, dwSize, &read))
		{
			delete[] pBuf;
			return NULL;
		}

		pMemoryMap = new memorymap_t();
		pMemoryMap->hModule = hModule;
		pMemoryMap->pBuffer = pBuf;
		pMemoryMap->dwSize = read;

		(*m_mapMemory)[lpModuleNameSafe] = pMemoryMap;
		dbg_printf("created new memory map for: %s[%p][%p][%d]\n", lpModuleNameSafe, pMemoryMap->hModule, pMemoryMap->pBuffer, pMemoryMap->dwSize);
	}


	m_pScanner->Compile(lpPattern);

	size_t dwOffset = 0;
	if (!m_pScanner->Search(pMemoryMap->pBuffer, pMemoryMap->dwSize, dwSkip, dwOffset))
	{
		lpAddress = NULL;
		return false;
	}

	lpAddress = (PBYTE)pMemoryMap->hModule + dwOffset;
	return true;
}

int Memory::ReleaseMemoryMap()
{
	int nCount = 0;

	typedef std::map<char*, LPMEMORYMAP>::iterator iter;
	for (iter it = m_mapMemory->begin(); it != m_mapMemory->end(); it++)
	{
		delete[] it->second->pBuffer;
		nCount++;
	}

	m_mapMemory->clear();
	return nCount;
}

bool Memory::GetModuleSize(HANDLE hProcess, void* lpImageBase, DWORD& dwSize)
{
	MEMORY_BASIC_INFORMATION mbi;
	PBYTE pQueryAddress = (PBYTE)lpImageBase;
	bool bFound = false;
	while (!bFound)
	{
		if (VirtualQueryEx(hProcess, pQueryAddress, &mbi, sizeof(mbi)) != sizeof(mbi))
			break;

		if (mbi.AllocationBase != lpImageBase)
		{
			dwSize = pQueryAddress - (PBYTE)lpImageBase;
			bFound = true;
			break;
		}

		pQueryAddress += mbi.RegionSize;
	}

	return bFound;
}
