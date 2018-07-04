using UnityEngine;
using UnityEditor;
using System.IO;

namespace OpenVDB
{
    public static class UnityCloudBuildTrigger
    {
        /// <summary>
        /// Call from Unity Cloud Build to build OpenVdb.unitypackage automatically
        /// </summary>
        public static void PostProcess()
        {
            Debug.LogFormat("PostExport Start");

            var exportPath = Path.GetDirectoryName(EditorApplication.applicationPath);
            var info = PackageBuilder.Build(exportPath);

            Debug.LogFormat("PostExport End. Build {0}. FileSize:{1} ExportPath:{2}", 
                            info.succeed ? "Succeed" : "Failed", 
                            info.fileSize, 
                            info.exportPath);
        }
    }
}