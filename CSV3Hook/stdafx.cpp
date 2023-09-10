#include "stdafx.h"
#include "Memory.h"

char* GetLocalPath(char* buffer)
{
	GetModuleFileNameA(NULL, buffer, MAX_PATH);
	for (int i = strlen(buffer) - 1; i >= 0; i--)
	{
		if (buffer[i] == '\\')
		{
			buffer[i+1] = NULL;
			break;
		}
	}
	return buffer;
}

void msgdump(const char* szHeader, void* pBuf, int nSize, bool bPrintHeaderEx)
{
	const int BYTES_PER_LINE = 16;
	if (nSize <= 0)
		return;

	// print header
	dbg_printf("      MsgDump [%s] size=%d\n", szHeader, nSize);

	if (bPrintHeaderEx)
	{
		dbg_printf("      00 01 02 03 04 05 06 07 08 09 0a 0b 0c 0d 0e 0f    0123456789abcdef\n");
		dbg_printf("    +------------------------------------------------    ----------------\n");
	}

	uint8_t* buf = (uint8_t*)pBuf;

	int lines = (nSize + BYTES_PER_LINE - 1) / BYTES_PER_LINE;
	for (int i = 0; i < lines; i++)
	{
		char szBytes[2 * BYTES_PER_LINE + (BYTES_PER_LINE - 1) + 1] = { 0 };
		char szChars[BYTES_PER_LINE + 1] = { 0 };
		char szByte[2 + 1] = { 0 }; // used for formatting bytes

		int index = i * BYTES_PER_LINE;

		// process first byte separately
		{
			sprintf(szByte, "%02x", buf[index]);
			strcat(szBytes, szByte);

			szChars[0] = isprint(buf[index]) ? (char)buf[index] : '.';
		}

		for (int j = index + 1; j < index + BYTES_PER_LINE; j++)
		{
			char c = ' ';
			if (j < nSize)
			{
				sprintf(szByte, "%02x", buf[j]);
				c = isprint(buf[j]) ? (char)buf[j] : '.';

				strcat(szBytes, " ");
				strcat(szBytes, szByte);
			}
			else
			{
				strcat(szBytes, "   ");
			}
			szChars[j - index] = c;
		}

		dbg_printf("%03x | %s    %s\n", index, szBytes, szChars);
	}
}