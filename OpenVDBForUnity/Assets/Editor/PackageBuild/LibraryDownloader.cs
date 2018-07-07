using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace OpenVDB
{
    class LibraryPackageInfo
    {
        public string os;
        public string arch;
        public string packageId;
    }

    public static class LibraryDownloader
    {
        const string LibraryDownloadURL = "https://dl.bintray.com/kazuki/conan/kazuki/OpenVDBForUnity/{0}/{1}/package/{2}/{3}";
        const string LibraryVersion = "0.0.1";
        const string LibraryChannel = "stable";
        const string LibraryTgzFileName = "conan_package.tgz";

        const string LibraryDestFolder = "OpenVDB/Plugins/{0}/";

        static LibraryPackageInfo[] Infos = 
        {
            new LibraryPackageInfo { os = "Windows", arch = "x86", packageId = "07a258b63529b1a6b9517b05bd8057994689b8eb" },
            new LibraryPackageInfo { os = "Windows", arch = "x86_64", packageId = "07a258b63529b1a6b9517b05bd8057994689b8eb" },
            new LibraryPackageInfo { os = "Macos", arch = "x86_64", packageId = "07a258b63529b1a6b9517b05bd8057994689b8eb" },
            new LibraryPackageInfo { os = "Linux", arch = "x86_64", packageId = "07a258b63529b1a6b9517b05bd8057994689b8eb" },
        };


        public static bool Run()
        {
            foreach(var info in Infos)
            {
                var url= string.Format(LibraryDownloadURL, LibraryVersion, LibraryChannel, info.packageId, LibraryTgzFileName);
                var tempPath = FileUtil.GetUniqueTempPathInProject();

                // Download file
                Debug.LogFormat("Downloading from {0}", url);
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

                // Copy library file to project folder
                var src = Path.Combine(extractPath, "lib");
                var dest = Path.Combine(Application.dataPath, string.Format(LibraryDestFolder, info.arch));
                Debug.LogFormat("Copy file {0}, Destination Directory {1}", src, dest);
                CopyFiles(src, dest);
            }
            return true;
        }

        public static byte[] Download(string url)
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

        public static void CopyFiles(string src, string dst)
        {
            var files = Directory.GetFiles(src);
            foreach(var file in files)
            {
                File.Copy(file, Path.Combine(dst, Path.GetFileName(file)));
            }
        }
    }
}
