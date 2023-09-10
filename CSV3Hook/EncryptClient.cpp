#include "stdafx.h"
#include "Memory.h"
#include "LegacyCipher.h"

extern CLegacyCipher* pcs_legacy;
extern char szPassword[32];
extern char szServer[32];

void __stdcall CryptData(PBYTE data, int len, BOOL isDecrypt)
{
	const uint16_t MSG_LOGINDATA = 1542;

	if (isDecrypt)
	{
		//dbg_msgdump("CryptData::Decrypt (before)", data, len);
		pcs_legacy->Decrypt(data, len);
		//dbg_msgdump("CryptData::Decrypt (after)", data, len);
	}
	else
	{
		dbg_printf("CryptData::Encrypt(%d)\n", *((uint16_t*)&data[2]));
		dbg_msgdump("CryptData::Encrypt", data, len);
		if (*((uint16_t*)&data[2]) == MSG_LOGINDATA)
		{
			*szServer = NULL;
			strcat((char*)&data[8 + 64], szPassword);
			strcat(szServer, (char*)&data[8 + 64 + 64]);
		}

		pcs_legacy->Encrypt(data, len);
	}
}

void InstallEncryptClientHooks()
{
	PBYTE pMatch = NULL;

	// found within CEncryptClient::Encrypt
	/*
		0073878B  /$  6A 10               PUSH 10                                  ; Conquer.0073878B(guessed Arg1,Arg2,Arg3)
		0073878D  |.  B8 24BD8B00         MOV EAX,008BBD24                         ; Entry point
		00738792  |.  E8 BF761100         CALL 0084FE56
		00738797  |.  33DB                XOR EBX,EBX
		00738799  |.  895D FC             MOV DWORD PTR SS:[EBP-4],EBX
		0073879C  |.  8B01                MOV EAX,DWORD PTR DS:[ECX]
	*/
	if (!GetMemory()->FindMemoryPattern("6A 10 B8 24 BD 8B 00 E8 BF 76 11 00 33 DB 89 5D FC 8B 01", pMatch))
	{
		dbg_printf("failed to locate CEncryptClient::Encrypt function\n");
	}
	else
	{
		BYTE patch[] = { 0xb8, 0, 0, 0, 0, 0xff, 0xe0 };
		*((void**)&patch[1]) = CryptData;
		GetMemory()->WriteMemory(pMatch, &patch[0], sizeof(patch));

		dbg_printf("CEncryptClient::Encrypt function patched: %p\n", pMatch);
	}

	// found in?
	/*
		0073BEEE  |> \6A 01               PUSH 1                                   ; /Arg3 = 1
		0073BEF0  |.  53                  PUSH EBX                                 ; |Arg2
		0073BEF1  |.  8B86 00200000       MOV EAX,DWORD PTR DS:[ESI+2000]          ; |
		0073BEF7  |.  03C6                ADD EAX,ESI                              ; |
		0073BEF9  |.  50                  PUSH EAX                                 ; |Arg1
		0073BEFA  |.  8D8E 14200100       LEA ECX,[ESI+12014]                      ; |
		0073BF00  |.  E8 86C8FFFF         CALL 0073878B                            ; \Conquer.0073878B
	*/
	if (!GetMemory()->FindMemoryPattern("6A 01 53 8B 86 00 20 00 00", pMatch))
	{
		dbg_printf("failed to locate EncryptClient hook\n");
	}
	else
	{
		BYTE patch = 0;
		GetMemory()->WriteMemory(&pMatch[1], &patch, sizeof(patch));

		dbg_printf("EncryptClient patched: %p\n", pMatch);
	}
}
