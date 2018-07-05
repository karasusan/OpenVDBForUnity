#include <openvdb/openvdb.h>

#define openvdbiImpl

#ifdef _WIN32
    #define NOMINMAX
    #include <windows.h>
    #include <ppl.h>
    #pragma warning(disable: 4996)
    #pragma warning(disable: 4190)
#endif // _WIN32