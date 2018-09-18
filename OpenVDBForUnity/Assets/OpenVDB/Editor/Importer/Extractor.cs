using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace OpenVDB
{
    public static class Extractor
    {
        public static IEnumerable<Object> FindSubassetsFromPath(string path)
        {
            return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAllAssetsAtPath(path).Where(AssetDatabase.IsSubAsset);
        }

        public static T[] ExtractSubassetsFromPath<T>(string path, string destinationPath, string extension) where T : Object
        {
            var enumerable = FindSubassetsFromPath(path).Select(subasset => subasset as T).Where(obj => obj != null);
            return enumerable.Select(subasset => ExtractFromAsset(subasset, Path.ChangeExtension(Path.Combine(destinationPath, subasset.name), extension))).ToArray();
        }
        
        public static T ExtractFromAsset<T>(T subAsset, string destinationPath) where T : Object
        {
            var clone = Object.Instantiate(subAsset);
            AssetDatabase.CreateAsset(clone, destinationPath);
            return AssetDatabase.LoadAssetAtPath<T>(destinationPath);
        }
    }    
}
