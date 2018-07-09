project(PackageTest)
cmake_minimum_required(VERSION 2.8)

include(${CMAKE_BINARY_DIR}/conanbuildinfo.cmake)
conan_basic_setup()

add_executable(testPackage main.cpp)
target_link_libraries(testPackage ${CONAN_LIBS})
