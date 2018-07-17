import os
import shutil
from conans import ConanFile, CMake, tools


class OpenVDBNativePluginConan(ConanFile):
    name = "OpenVDBNativePlugin"
    version = "0.0.1"
    license = "MIT"
    description = "OpenVDBNativePlugin is an open source C++ library for Unity plugin of OpenVDB"
    url = "https://github.com/karasusan/OpenVDBForUnity"
    requires = ( "OpenVDB/4.0.2@kazuki/stable"
               )
    generators = "cmake"
    settings = "os", "arch", "compiler", "build_type"
    options = { "shared": [True, False]
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
            # Set fPIC=True to all dependencies that have the option.
            for _, pkg_opts in self.options.deps_package_values.items():
                if "fPIC" in pkg_opts.fields:
                    pkg_opts.fPIC = True
                    
        if self.settings.os != "Windows":
            self.options["TBB"].shared = False

    def source(self):
        self.run("git clone https://github.com/karasusan/OpenVDBForUnity src")
        # self.run("cd src && git checkout v%s" % self.version)

    def build(self):
        cmake = CMake(self)
        cmake.configure(source_dir="src/Plugin")
        cmake.build()

    def package(self):
        self.copy("LICENSE", dst="license", src="src")
        self.copy("*", dst="lib", src="lib")
        self.copy("openvdbi.h", dst="include", src="src/Plugin/openvdbi")
        self.copy("OpenVDBImporter.h", dst="include/Importer", src="src/Plugin/openvdbi/Importer")