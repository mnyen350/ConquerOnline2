#include "Scanner.h"

Scanner::Scanner()
{
	this->m_pPattern = NULL;
	this->m_pMask = NULL;
	this->m_pHasMask = NULL;

	this->Reset();
}

Scanner::Scanner(std::string pattern)
{
	this->m_pPattern = NULL;
	this->m_pMask = NULL;
	this->m_pHasMask = NULL;

	this->Compile(pattern);
}

Scanner::~Scanner()
{
	this->Reset();
}

void Scanner::Reset()
{
	this->m_nLength = 0;

	if (this->m_pPattern)
	{
		delete[] this->m_pPattern;
		this->m_pPattern = NULL;
	}

	if (m_pMask)
	{
		delete[] this->m_pMask;
		this->m_pMask = NULL;
	}

	if (this->m_pHasMask)
	{
		delete[] this->m_pHasMask;
		this->m_pHasMask = NULL;
	}
}

bool Scanner::Compile(std::string pattern)
{
	this->Reset();

	pattern.erase(std::remove_if(pattern.begin(), pattern.end(), ::isspace), pattern.end());

	int patternLength = pattern.length();
	if (patternLength % 2 != 0)
	{
		dbg_printf("invalid pattern length\n");
		return false;
	}
	else if (pattern.length() == 0)
	{
		dbg_printf("invalid pattern length\n");
		return false;
	}

	m_nLength = pattern.length() / 2;
	m_pPattern = new uint8_t[m_nLength];
	m_pMask = new uint8_t[m_nLength];
	m_pHasMask = new uint8_t[m_nLength];

	int i = 0;
	char nibble[2] = { 0, 0 };
	for (std::string::iterator it = pattern.begin(); it != pattern.end(); i++)
	{
		nibble[0] = *it++;
		nibble[1] = *it++;

		uint8_t value = 0;
		uint8_t mask = 0;
		uint8_t hasMask = (nibble[0] == '?' || nibble[1] == '?');
		if (nibble[0] != '?')
		{
			try
			{
				value |= this->toByte(nibble[0]) << 4;
				mask |= 0xf0;
			}
			catch (std::exception e)
			{
				dbg_printf("%s\n", e.what());
				this->Reset();
				return false;
			}
		}

		if (nibble[1] != '?')
		{
			try
			{
				value |= this->toByte(nibble[1]);
				mask |= 0x0f;
			}
			catch (std::exception e)
			{
				dbg_printf("%s\n", e.what());
				this->Reset();
				return false;
			}
		}

		this->m_pPattern[i] = value;
		this->m_pMask[i] = mask;
		this->m_pHasMask[i] = hasMask;
	}

	return true;
}

bool Scanner::Search(uint8_t* buffer, size_t size, uint32_t skip, size_t& offset)
{
	uint8_t* processBuffer = buffer;

	for (size_t i = 0; i < size; i++)
	{
		bool match = true;
		for (size_t j = 0; j < this->m_nLength; j++)
		{
			uint8_t value = processBuffer[i + j];
			uint8_t pattern = this->m_pPattern[j];
			uint8_t mask = this->m_pMask[j];
			uint8_t hasMask = this->m_pHasMask[j];

			if (value != pattern && (!hasMask || ((value & mask) != (pattern & mask))))
			{
				match = false;
				break;
			}
		}
		if (match)
		{
			if (skip-- <= 0)
			{
				offset = i;
				return true;
			}
		}
	}

	return false;
}
