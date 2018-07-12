#include "pch.h"
#include <cstdarg>

void LogPrint(const char* fmt, ...)
{
    va_list vl;
    va_start(vl, fmt);
#ifdef _WIN32
    char buf[2048];
    vsprintf(buf, fmt, vl);
    ::OutputDebugStringA(buf);
    ::OutputDebugStringA("\n");
#else
    vprintf(fmt, vl);
    printf("\n");
#endif
    va_end(vl);
}

void LogPrint(const wchar_t* fmt, ...)
{
    va_list vl;
    va_start(vl, fmt);
#ifdef _WIN32
    wchar_t buf[2048];
    vswprintf(buf, fmt, vl);
    ::OutputDebugStringW(buf);
    ::OutputDebugStringW(L"\n");
#else
    vwprintf(fmt, vl);
    printf("\n");
#endif
    va_end(vl);
}
