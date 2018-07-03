using UnityEngine;

namespace Assets.Editor.PackageBuild
{
    public static class UnityCloudBuildTrigger
    {
        public static void PostExport(string exportPath)
        {
            Debug.LogFormat("PostExport Start");
            var info = PackageBuilder.Build(exportPath);

            Debug.LogFormat("PostExport End. Build {0}. FileSize:{1} ExportPath:{2}", 
                            info.succeed ? "Succeed" : "Failed", 
                            info.fileSize, 
                            info.exportPath);
        }
    }
}