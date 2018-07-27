using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace OpenVDB.PackageBuild
{
    class LibraryPackageInfo
    {
        public BuildTarget target;
        public string arch;
        public string packageId;
    }

    public static class LibraryDownloader
    {
        const string LibraryDownloadURL = "https://dl.bintray.com/kazuki/conan/kazuki/OpenVDBNativePlugin/{0}/{1}/package/{2}/{3}";
        const string LibraryVersion = "0.0.1";
        const string LibraryChannel = "stable";
        const string LibraryTgzFileName = "conan_package.tgz";

        const string LibraryDestFolder = "OpenVDB/Scripts/Plugins/{0}/";

        static LibraryPackageInfo[] Infos = 
        {
            new LibraryPackageInfo { target = BuildTarget.StandaloneWindows, arch = "x86", packageId = "07a258b63529b1a6b9517b05bd8057994689b8eb" },
            new LibraryPackageInfo { target = BuildTarget.StandaloneWindows64, arch = "x86_64", packageId = "2c0ede688cb6609cf77dafa57a7200b861971804" },
            new LibraryPackageInfo { target = BuildTarget.StandaloneOSX, arch = "x86_64", packageId = "267209270177540f16ec5a6a007b22ed0457c5b2" },
            new LibraryPackageInfo { target = BuildTarget.StandaloneLinuxUniversal, arch = "x86_64", packageId = "e8de85f12f9f4405cbaf1c4b62e665d6b58020a9" },
        };

        public static string MakeShortAssetPath(string fullpath)
        {
            return fullpath.Replace(Path.GetDirectoryName(Application.dataPath) + Path.DirectorySeparatorChar, string.Empty);
        }

        public static bool Run()
        {
            Clear();

            foreach(var info in Infos)
            {
                var url = string.Format(LibraryDownloadURL, LibraryVersion, LibraryChannel, info.packageId, LibraryTgzFileName);
                var tempPath = FileUtil.GetUniqueTempPathInProject();

                // Download file
                Debug.LogFormat("Downloading package for {0} {1} from {2}", info.target, info.arch, url);
                var bytes = Download(url);
                if(bytes == null)
                {
                    Debug.LogFormat("Download Failed");
                    return false;
                }
                Debug.LogFormat("Download Success");

                // Save downloaded file
                Directory.CreateDirectory(tempPath);
                var tgzFilePath = Path.Combine(tempPath, LibraryTgzFileName);
                File.WriteAllBytes(tgzFilePath, bytes);
                Debug.LogFormat("Saved package {0}", tgzFilePath);

                // Extract downloaded file
                var extractPath = Path.Combine(Path.GetDirectoryName(tgzFilePath), Path.GetFileNameWithoutExtension(tgzFilePath));
                Debug.LogFormat("Extract {0}", tgzFilePath);
                ExtractTGZ(tgzFilePath, extractPath);

                Debug.LogFormat("Extract Succeed {0}", extractPath);

                // Copy library files to project folder
                var folderNames = new string[] { "lib", "bin" };
                foreach(var folderName in folderNames)
                {
                    var src = Path.Combine(extractPath, folderName);
                    if (!Directory.Exists(src))
                        continue;
                    
                    var dest = Path.Combine(Application.dataPath, string.Format(LibraryDestFolder, info.arch));
                    if (!Directory.Exists(dest))
                    {
                        Directory.CreateDirectory(dest);
                    }
                    Debug.LogFormat("Copy file {0}, Destination Directory {1}", src, dest);
                    var paths = CopyFiles(src, dest);

                    UpdatePluginImporterSettings(paths.Select(_ => MakeShortAssetPath(_)).ToArray(), info.target);
                }
            }
            return true;
        }

        public static void Clear()
        {
            foreach(var info in Infos)
            {
                // Delete arch(x86 or x86_64) directory.
                var path = Path.Combine(Application.dataPath, string.Format(LibraryDestFolder, info.arch));
                if(Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }

                // Delete meta file.
                path = path + ".meta";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

            }
        }

        static byte[] Download(string url)
        {
            using(var request = UnityWebRequest.Get(url))
            {
                request.SendWebRequest();

                while (!request.isDone)
                {
                    System.Threading.Thread.Sleep(100);
                }
                if(request.isHttpError || request.isNetworkError)
                {
                    return null;
                }
                var handler = request.downloadHandler;
                return handler.data;
            }
        }


        public static void ExtractTGZ(string gzArchiveName, string destFolder) 
        {
            var inStream = File.OpenRead(gzArchiveName);
            var gzipStream = new GZipInputStream(inStream);

            var tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            tarArchive.ExtractContents(destFolder);
            tarArchive.Close();

            gzipStream.Close();
            inStream.Close();
        }

        public static List<string> CopyFiles(string src, string dst)
        {
            var paths = new List<string>();

            if (!Directory.Exists(dst))
            {
                Directory.CreateDirectory(dst);
                File.SetAttributes(dst, File.GetAttributes(src));
                paths.Add(dst);
            }

            var files = Directory.GetFiles(src);
            foreach (var file in files)
            {
                var path = Path.Combine(dst, Path.GetFileName(file));
                File.Copy(file, path, true);
                paths.Add(path);
            }

            var dirs = Directory.GetDirectories(src);
            foreach (string dir in dirs)
            {
                var _paths = CopyFiles(dir, Path.Combine(dst,Path.GetFileName(dir)));
                paths.AddRange(_paths);
            }
            return paths;
        }

        public static void UpdatePluginImporterSettings(string[] paths, BuildTarget target)
        {
            foreach(var path in paths)
            {
                AssetDatabase.ImportAsset(path);
            }

            var importers = paths.Select(AssetImporter.GetAtPath).Where (_importer => _importer is PluginImporter).Cast<PluginImporter>();

            foreach (var importer in importers)
            {
                importer.SetCompatibleWithAnyPlatform(false);
                importer.SetCompatibleWithPlatform(target, true);
                importer.SetCompatibleWithEditor(true);
                importer.SaveAndReimport();
            }
        }
    }
}
