#pragma once

#ifdef openvdbiImpl
    #ifndef openvdbiStaticLink
        #ifdef _WIN32
            #define openvdbiAPI extern "C" __declspec(dllexport)
        #else
            #define openvdbiAPI extern "C"
        #endif
    #else
        #define openvdbiAPI 
    #endif
#else
    #ifdef _MSC_VER
        #ifndef openvdbiStaticLink
            #define openvdbiAPI extern "C" __declspec(dllimport)
            #pragma comment(lib, "openvdbi.lib")
        #else
            #define openvdbiAPI extern "C"
            #pragma comment(lib, "openvdbi_s.lib")
        #endif
    #else
        #define openvdbiAPI extern "C"
    #endif
#endif // openvdbiImpl

#include "Importer/OpenVDBImporter.h"