using UnityEditor;

namespace OpenVDB.PackageBuild
{
    static class PackageMenu
    {
        [MenuItem("Packages/OpenVDB/Download Library")]
        static void DownloadLibrary()
        {
            LibraryDownloader.Run();
        }
    }
}