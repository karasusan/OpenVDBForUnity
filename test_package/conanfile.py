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
        self.options["TBB"].shared = [False]

    def build(self):
        cmake = CMake(self)
        cmake.configure()
        cmake.build()

    def imports(self):
        self.copy("*.dll", dst="bin", src="bin")
        self.copy("*.dylib", dst="bin", src="lib")

    def test(self):
        self.run("cd bin && .%stestPackage" % os.sep)
