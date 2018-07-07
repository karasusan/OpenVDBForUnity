using UnityEngine;
using UnityEditor;

namespace OpenVDB
{
    public static class PackageUploader
    {
        const string PackageUploadURL = "https://api.bintray.com/content/kazuki/Unity/OpenVDBForUnity:kazuki/";
        const string PackageVersion = "0.0.1";

        public static bool Upload(string path)
        {
            return true;
        }
    }
}