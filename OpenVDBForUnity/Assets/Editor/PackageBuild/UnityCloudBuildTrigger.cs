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

            Debug.LogFormat("PlayerSettings.GetScriptingDefineSymbolsForGroup = {0}", PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
            foreach(var def in EditorUserBuildSettings.activeScriptCompilationDefines)
                Debug.LogFormat("EditorUserBuildSettings.activeScriptCompilationDefines = {0}", def);

            var exportDir = FileUtil.GetUniqueTempPathInProject();
            Directory.CreateDirectory(exportDir);

            var info = PackageBuilder.Build(exportDir);

            Debug.LogFormat("PostExport End. Build {0}. FileSize:{1} ExportPath:{2}", 
                            info.succeed ? "Succeed" : "Failed", 
                            info.fileSize,
                            info.exportPath);

            PackageUploader.Upload(info.exportPath);
        }
    }
}