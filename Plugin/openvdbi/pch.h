#include <algorithm>
#include <numeric>
#include <map>
#include <set>
#include <vector>
#include <deque>
#include <memory>
#include <thread>
#include <mutex>
#include <condition_variable>
#include <future>
#include <functional>
#include <limits>
#include <sstream>
#include <fstream>
#include <type_traits>
#include <locale>
#include <codecvt>
#include <openvdb/openvdb.h>
#include <OpenEXR/ImathVec.h>

#define openvdbiImpl

#define aiDebug

#if defined(aiDebug)
void LogPrint(const char* fmt, ...);
    void LogPrint(const wchar_t* fmt, ...);
    #define DebugLog(...)       LogPrint("openvdbi Log: " __VA_ARGS__)
    #define DebugWarning(...)   LogPrint("openvdbi Warning: " __VA_ARGS__)
    #define DebugError(...)     LogPrint("openvdbi Error: "  __VA_ARGS__)
    #define DebugLogW(...)      LogPrint(L"openvdbi Log: " __VA_ARGS__)
    #define DebugWarningW(...)  LogPrint(L"openvdbi Warning: " __VA_ARGS__)
    #define DebugErrorW(...)    LogPrint(L"openvdbi Error: "  __VA_ARGS__)
#else
#define DebugLog(...)
#define DebugWarning(...)
#define DebugError(...)
#define DebugLogW(...)
#define DebugWarningW(...)
#define DebugErrorW(...)
#endif

#ifdef _WIN32
    #define NOMINMAX
    #include <windows.h>
    #include <ppl.h>
    #pragma warning(disable: 4996)
    #pragma warning(disable: 4190)
#endif // _WIN32

using openvdbV4 = Imath::V4f;