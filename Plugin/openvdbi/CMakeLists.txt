file(GLOB sources *.cpp *.h Importer/*.h Importer/*.cpp)

IF(BUILD_SHARED_LIBS)
    add_plugin(openvdbi SOURCES ${sources})
ELSE()
    INSTALL ( TARGETS openvdbi DESTINATION lib)
ENDIF()

target_include_directories(openvdbi PUBLIC .)
conan_target_link_libraries(openvdbi)