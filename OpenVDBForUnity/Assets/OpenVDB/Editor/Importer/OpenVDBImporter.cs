#if UNITY_2017_1_OR_NEWER

using System;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor.Experimental.AssetImporters;
using System.Text.RegularExpressions;
using Extensions;
using UnityEditor;
using Object = UnityEngine.Object;

namespace OpenVDB
{
    [Serializable]
    [ScriptedImporter(1, "vdb")]
    public class OpenVDBImporter : ScriptedImporter
    {
        [SerializeField] public OpenVDBStreamSettings streamSettings = new OpenVDBStreamSettings();

        private static string MakeShortAssetPath(string assetPath)
        {
            return Regex.Replace(assetPath, "^Assets/", "");
        }

        private static string SourcePath(string assetPath)
        {
            if (assetPath.StartsWith("Packages", System.StringComparison.Ordinal))
            {
                return Path.Combine(Path.GetDirectoryName(Application.dataPath), assetPath);
            }
            else
            {
                return Path.Combine(Application.dataPath, assetPath);
            }
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var shortAssetPath = MakeShortAssetPath(ctx.assetPath);
            var sourcePath = SourcePath(shortAssetPath);
            var destPath = Path.Combine(Application.streamingAssetsPath, shortAssetPath);
            var directoryPath = Path.GetDirectoryName(destPath);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            if (File.Exists(destPath))
                File.SetAttributes(destPath, FileAttributes.Normal);
            File.Copy(sourcePath, destPath, true);

            var fileName = Path.GetFileNameWithoutExtension(destPath);
            var go = new GameObject(fileName);

            var streamDescriptor = ScriptableObject.CreateInstance<OpenVDBStreamDescriptor>();
            streamDescriptor.name = go.name + "_VDBDesc";
            streamDescriptor.pathToVDB = shortAssetPath;
            streamDescriptor.settings = streamSettings;

            using (var vdbStream = new OpenVDBStream(go, streamDescriptor))
            {
                if (!vdbStream.Load())
                    return;

                var subassets = new Subassets(ctx);
                subassets.Add(streamDescriptor.name, streamDescriptor);
                GenerateSubAssets(subassets, vdbStream, streamDescriptor);

#if UNITY_2017_3_OR_NEWER
                ctx.AddObjectToAsset(go.name, go);
                ctx.SetMainObject(go);
#else
                ctx.SetMainAsset(go.name, go);
#endif
            }
        }

        class Subassets
        {
            AssetImportContext m_ctx;
            Material m_defaultMaterial;

            public Subassets(AssetImportContext ctx)
            {
                m_ctx = ctx;
            }

            public Material defaultMaterial
            {
                get
                {
                    if (m_defaultMaterial != null) return m_defaultMaterial;
                    m_defaultMaterial = new Material(Shader.Find("OpenVDB/Standard"))
                    {
                        name = "Default Material",
                        hideFlags = HideFlags.NotEditable,
                    };
                    Add("Default Material", m_defaultMaterial);
                    return m_defaultMaterial;
                }
            }
            public void Add(string identifier, Object asset)
            {
#if UNITY_2017_3_OR_NEWER
                m_ctx.AddObjectToAsset(identifier, asset);
#else
                m_ctx.AddSubAsset(identifier, asset);
#endif
            }
        }

        private void GenerateSubAssets(Subassets subassets, OpenVDBStream stream, OpenVDBStreamDescriptor descriptor)
        {
            CollectSubAssets(subassets, stream, descriptor);
        }

        private void CollectSubAssets(Subassets subassets, OpenVDBStream stream, OpenVDBStreamDescriptor descriptor)
        {
            var go = stream.gameObject;
            Texture texture = null;
            
            if (descriptor.settings.extractTextures)
            {
                texture = descriptor.settings.textures.First();
                AddRemap(new SourceAssetIdentifier(typeof(Texture), texture.name), texture);
            }
            else
            {
                if (stream.texture3D != null)
                {
                    texture = stream.texture3D;
                    subassets.Add(stream.texture3D.name, stream.texture3D);
                }
            }
            
            var meshFilter = go.GetOrAddComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.sharedMesh = stream.mesh;
                meshFilter.sharedMesh.name = go.name;
                subassets.Add(meshFilter.sharedMesh.name, meshFilter.sharedMesh);
            }
            var renderer = go.GetOrAddComponent<MeshRenderer>();
            if (renderer == null) return;
            if (!descriptor.settings.importMaterials) return;
            if (descriptor.settings.extractMaterials)
            {
                var material = descriptor.settings.materials.First();
                AddRemap(new SourceAssetIdentifier(typeof(Material), material.name), material);
                renderer.sharedMaterial = material;
            }
            else
            {
                renderer.sharedMaterial = subassets.defaultMaterial;
            }

            if (texture == null) return;
            renderer.sharedMaterial.SetTexture("_Volume", texture);
            renderer.sharedMaterial.name = texture.name;

        }
    }
}

#endif
