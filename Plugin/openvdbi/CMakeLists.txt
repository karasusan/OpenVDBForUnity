file(GLOB sources *.cpp *.h Importer/*.h Importer/*.cpp)
set(plugins_dir ".")
set(ENABLE_DEPLOY OFF)
add_plugin(openvdbi SOURCES ${sources} PLUGINS_DIR ${plugins_dir})

target_include_directories(openvdbi PUBLIC .)
conan_target_link_libraries(openvdbi)