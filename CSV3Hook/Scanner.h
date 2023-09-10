#pragma once

#include "stdafx.h"
#include <algorithm>
#include <string>

class Scanner
{
public:
	Scanner();
	Scanner(std::string pattern);
	~Scanner();

	size_t GetLength() { return this->m_nLength; }

	void Reset();

	bool Compile(std::string pattern);

	bool Search(uint8_t* buffer, size_t size, uint32_t skip, size_t& offset);

private:
	uint8_t* m_pPattern;
	uint8_t* m_pMask;
	uint8_t* m_pHasMask;
	size_t m_nLength;

	uint8_t toByte(char digit)
	{
		if (digit >= '0' && digit <= '9')
		{
			return digit - '0';
		}
		else if (digit >= 'a' && digit <= 'f')
		{
			return digit - 'a' + 10;
		}
		else if (digit >= 'A' && digit <= 'F')
		{
			return digit - 'A' + 10;
		}

		char szBuffer[128];
		sprintf(szBuffer, "invalid hexdecimal digit %c(%x)", digit, digit);
		throw std::exception(szBuffer);
	}
};
