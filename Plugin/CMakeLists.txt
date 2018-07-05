PROJECT(OpenVDBForUnity)
CMAKE_MINIMUM_REQUIRED(VERSION 2.8.12)

include(${CMAKE_BINARY_DIR}/conanbuildinfo.cmake)
conan_basic_setup()

ADD_DEFINITIONS(-std=c++11)

LIST(APPEND CMAKE_MODULE_PATH "${CMAKE_SOURCE_DIR}/cmake")
include(AddPlugin)

ADD_SUBDIRECTORY(openvdbi)