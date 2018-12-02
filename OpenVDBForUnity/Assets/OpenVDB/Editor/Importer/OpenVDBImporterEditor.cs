#if UNITY_2017_1_OR_NEWER

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

namespace OpenVDB
{
    [CustomEditor(typeof(OpenVDBImporter)), CanEditMultipleObjects]
    public class OpenVDBImporterEditor : ScriptedImporterEditor
    {
        private int _toolbar = 0;

        private static void ShowSerializedPropertyArrayElement<T>(SerializedProperty property, int index) where T : UnityEngine.Object
        {
            var obj = property.GetArrayElementAtIndex(index).objectReferenceValue as T;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(obj.name);
            var newObj = EditorGUILayout.ObjectField(obj, typeof(T), false);
            if (newObj.GetHashCode() != obj.GetHashCode())
            {
                property.GetArrayElementAtIndex(index).objectReferenceValue = newObj;
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void SerializedPropertyUpdateArray<T>(SerializedProperty property, T[] objects) where T : UnityEngine.Object
        {
            property.ClearArray();
            foreach (var obj in objects)
            {
                var index = property.arraySize;
                property.InsertArrayElementAtIndex(index);
                property.GetArrayElementAtIndex(index).objectReferenceValue = obj;
            }
        }


        private static string GetRelativePathInProjectFolder(string path)
        {
            var projectPath = Path.GetDirectoryName(Application.dataPath) + "/";

            if(!path.StartsWith(projectPath))
                throw new InvalidOperationException(string.Format("Invalid path:{0}", path));
            return path.Substring(projectPath.Length);
        }

        public override void OnInspectorGUI()
        {
            const string pathSettings = "streamSettings.";
            var importer = serializedObject.targetObject as OpenVDBImporter;
            if(importer == null)
                throw new InvalidOperationException();

            _toolbar = GUILayout.Toolbar( _toolbar,new []{ "Model", "Material" } );

            switch (_toolbar)
            {
                case 0:
                {
                    EditorGUILayout.LabelField("Meshes", EditorStyles.boldLabel);
                    var property = serializedObject.FindProperty(pathSettings + "scaleFactor");
                    if (property != null)
                    {
                        EditorGUILayout.PropertyField(property,
                            new GUIContent("Scale Factor",
                                "How much to scale the models compared to what is in the source file."));
                    }
                    break;
                }
                case 1:
                {
                    // Import Materials
                    {
                        var property = serializedObject.FindProperty(pathSettings + "importMaterials");
                        if (property != null)
                        {
                            EditorGUILayout.PropertyField(property,
                                new GUIContent("Import Materials",
                                    "How much to scale the models compared to what is in the source file."));
                        }
                    }

                    // Extract Textures
                    {
                        var property = serializedObject.FindProperty(pathSettings + "extractTextures");
                        var disabled = property.boolValue;
                        EditorGUI.BeginDisabledGroup(disabled);
                        
                        // Max Texture Size
                        {
                            var property2 = serializedObject.FindProperty(pathSettings + "textureMaxSize");
                            var textureMaxSize = property2.intValue;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PrefixLabel("Texture Max Size");
                            property2.intValue = EditorGUILayout.IntField(textureMaxSize);
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel("Textures");
                        if (GUILayout.Button("Extract Textures"))
                        {
                            var folder = EditorUtility.OpenFolderPanel("Select Textures Folder", Path.GetDirectoryName(importer.assetPath), "");
                            if (!string.IsNullOrEmpty(folder))
                            {
                                folder = GetRelativePathInProjectFolder(folder);
                                var externalTextures = Extractor.ExtractSubassetsFromPath<Texture>(importer.assetPath, folder, "asset");
                                var texturesProperty = serializedObject.FindProperty(pathSettings + "textures");
                                SerializedPropertyUpdateArray(texturesProperty, externalTextures);
                                property.boolValue = true;
                                ApplyAndImport();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();
                    }

                    // Extract Materials
                    {
                        var property = serializedObject.FindProperty(pathSettings + "extractMaterials");
                        var disabled = property.boolValue;
                        EditorGUI.BeginDisabledGroup(disabled);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel("Materials");
                        if (GUILayout.Button("Extract Materials"))
                        {
                            var folder = EditorUtility.OpenFolderPanel("Select Materials Folder", Path.GetDirectoryName(importer.assetPath), "");
                            if (!string.IsNullOrEmpty(folder))
                            {
                                folder = GetRelativePathInProjectFolder(folder);
                                var externalMaterials = Extractor.ExtractSubassetsFromPath<Material>(importer.assetPath, folder, "mat");
                                var materialsProperty = serializedObject.FindProperty(pathSettings + "materials");
                                SerializedPropertyUpdateArray(materialsProperty, externalMaterials);
                                property.boolValue = true;
                                ApplyAndImport();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();
                    }

                    // Remapped Textures
                    {
                        var property = serializedObject.FindProperty(pathSettings + "textures");
                        if (property.arraySize > 0)
                        {
                            EditorGUILayout.HelpBox("Textures assignments can be remapped below.", MessageType.Info);
                            EditorGUILayout.LabelField("Remapped Textures", EditorStyles.boldLabel);

                            for (var index = 0; index < property.arraySize; index++)
                            {
                                ShowSerializedPropertyArrayElement<Texture>(property, index);
                            }
                        }
                    }

                    // Remapped Materials
                    {
                        var property = serializedObject.FindProperty(pathSettings + "materials");
                        if (property.arraySize > 0)
                        {
                            EditorGUILayout.HelpBox("Material assignments can be remapped below.", MessageType.Info);
                            EditorGUILayout.LabelField("Remapped Materials", EditorStyles.boldLabel);

                            for (var index = 0; index < property.arraySize; index++)
                            {
                                ShowSerializedPropertyArrayElement<Material>(property, index);
                            }
                        }
                    }
                    break;
                }
                default:
                    throw new NotImplementedException();
            }

            EditorGUILayout.Separator();

            ApplyRevertGUI();
        }
    }
}

#endif
