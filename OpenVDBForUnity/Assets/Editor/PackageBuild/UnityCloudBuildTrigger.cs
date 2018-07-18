using UnityEngine;
using UnityEditor;
using System.IO;

namespace OpenVDB.PackageBuild
{
    public static class UnityCloudBuildTrigger
    {
        /// <summary>
        /// Call from Unity Cloud Build to build OpenVdb.unitypackage automatically
        /// </summary>
        public static void PreProcess()
        {
            Debug.LogFormat("PostExport Start");

            var exportDir = FileUtil.GetUniqueTempPathInProject();
            Directory.CreateDirectory(exportDir);

            var info = PackageBuilder.Build(exportDir);

            Debug.LogFormat("PostExport End. Build {0}. FileSize:{1} ExportPath:{2}", 
                            info.succeed ? "Succeed" : "Failed", 
                            info.fileSize,
                            info.exportPath);

            PackageBuilder.Copy(info.exportPath);
        }
    }
}