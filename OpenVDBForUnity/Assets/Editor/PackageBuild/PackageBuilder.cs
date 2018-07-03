using System.IO;
using UnityEditor;

namespace Assets.Editor.PackageBuild
{
    class PackageBuildInfo
    {
        public bool succeed;
        public string exportPath;
        public long fileSize;
    }

    static class PackageBuilder
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

            if(succeed)
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
    }
}
