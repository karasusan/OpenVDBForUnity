import os
import shutil
from conans import ConanFile, CMake, tools


class OpenVDBForUnityConan(ConanFile):
    name = "OpenVDBForUnity"
    version = "0.0.1"
    license = "MIT"
    description = "OpenVDBForUnity is an open source C++ library for Unity plugin of OpenVDB"
    url = "https://github.com/karasusan/OpenVDBForUnity"
    requires = ( "OpenVDB/4.0.2@kazuki/stable"
               )
    generators = "cmake"
    settings = "os", "arch", "compiler", "build_type"
    options = { "shared": [True]
              , "fPIC": [True, False]
              }
    default_options = "shared=True", "fPIC=True"
    build_policy = "missing"

    def config_options(self):
        if self.settings.os == "Windows":
            self.options.remove("fPIC")

    def configure(self):
        if self.options.shared and "fPIC" in self.options.fields:
            self.options.fPIC = True

    def build(self):
        cmake = CMake(self)
        cmake.configure(source_dir="../Plugin")
        cmake.build()

    def package(self):
        self.copy("LICENSE", src=".", dst="license")