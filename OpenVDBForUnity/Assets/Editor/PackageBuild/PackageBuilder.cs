using System.IO;
using UnityEngine;
using UnityEditor;

namespace OpenVDB.PackageBuild
{
    public class PackageBuildInfo
    {
        public bool succeed;
        public string exportPath;
        public long fileSize;
    }

    public static class PackageBuilder
    {
        const string PackageFileName = "OpenVDB.unitypackage";
        static string[] AssetPathNames =
        {
            "Assets/OpenVDB"
        };

        public static PackageBuildInfo Build(string exportDir)
        {
            var flags = ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets;
            var exportPath = Path.Combine(exportDir, PackageFileName);
            AssetDatabase.ExportPackage(AssetPathNames, exportPath, flags);

            var succeed = File.Exists(exportPath);
            long fileSize = 0;

            if (succeed)
            {
                var info = new FileInfo(exportPath);
                fileSize = info.Length;
            }

            return new PackageBuildInfo
            {
                succeed = succeed,
                exportPath = exportPath,
                fileSize = fileSize
            };
        }

        public static void Copy(string path)
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            File.Copy(path, Path.Combine(Application.streamingAssetsPath, Path.GetFileName(path)));
            AssetDatabase.Refresh();
        }
    }
}
