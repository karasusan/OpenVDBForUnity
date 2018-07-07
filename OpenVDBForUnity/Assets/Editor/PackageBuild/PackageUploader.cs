using UnityEngine;
using UnityEditor;
using System.IO;

namespace OpenVDB
{
    public static class PackageUploader
    {
        const string PackageUploadURL = "https://api.bintray.com/content/kazuki/Unity/OpenVDBForUnity:kazuki/";
        const string PackageVersion = "0.0.1";

        public static bool Upload(string path)
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            File.Copy(path, Path.Combine(Application.streamingAssetsPath, Path.GetFileName(path)));
            AssetDatabase.Refresh();
            return true;
        }
    }
}