#if UNITY_2017_1_OR_NEWER

using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

namespace OpenVDB
{
    [ScriptedImporter(2, "abc")]
    public class AlembicImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var streamDescriptor = ScriptableObject.CreateInstance<OpenVDBStreamDescriptor>();
            
            using (var vdbStream = new OpenVDBStream(streamDescriptor))
            {
                
            }
        }
    }
}

#endif