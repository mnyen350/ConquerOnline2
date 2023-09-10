#include "stdafx.h"
#include <cstdio>
#include <cstdarg>
#include <cassert>

#ifdef DEBUG_MODE
FILE *g_pDebugFile = fopen("csv3hook-debug.log", "w");

void debuglog(const char *format, ...)
{
	assert(g_pDebugFile != NULL);

	va_list v;
	va_start(v, format);

	char szBuf[256];
	vsprintf(szBuf, format, v);

	fprintf(g_pDebugFile, "%s", szBuf);
	fflush(g_pDebugFile);
}
#endif
