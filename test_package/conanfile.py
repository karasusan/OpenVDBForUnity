from conans.model.conan_file import ConanFile
from conans import CMake
import os

class DefaultNameConan(ConanFile):
    name = "DefaultName"
    version = "0.1"
    settings = "os", "compiler", "arch", "build_type"
    generators = "cmake"

    def configure(self):
        # Fix me
        # This problem relate OpenVDB linker settings. 
        # TBB.shared is True when it is default.
        if self.options["TBB"].shared:
            self.options["TBB"].shared = False
        if self.options["OpenVDBNativePlugin"].shared:
            self.options["OpenVDBNativePlugin"].shared = False

    def build(self):
        cmake = CMake(self)
        cmake.configure()
        cmake.build()

    def imports(self):
        self.copy("*.dll", dst="bin", src="bin")
        self.copy("*.dylib", dst="bin", src="lib")

    def test(self):
        ld_lib_path_envvar = { "Linux": "LD_LIBRARY_PATH"
                             , "Macos": "DYLD_LIBRARY_PATH"
                             }.get(str(self.settings.os))
        if ld_lib_path_envvar:
            ld_library_paths = os.environ.get(ld_lib_path_envvar, "").split(":")
            ld_library_paths = [path for path in ld_library_paths if path]
            ld_library_paths.extend(self.deps_cpp_info.lib_paths)
            os.environ[ld_lib_path_envvar] = ":".join(ld_library_paths)
        self.run("cd bin && .%stestPackage" % os.sep)
