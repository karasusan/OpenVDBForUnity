#if UNITY_2017_1_OR_NEWER

using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

namespace OpenVDB
{
    [CustomEditor(typeof(OpenVDBAssetImporter)), CanEditMultipleObjects]
    public class OpenVDBImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            var pathSettings = "streamSettings.";

            EditorGUILayout.LabelField("Scene", EditorStyles.boldLabel);
            {
                EditorGUI.indentLevel++;
                var property = serializedObject.FindProperty(pathSettings + "scaleFactor");
                if(property != null)
                {
                    EditorGUILayout.PropertyField(property,
                        new GUIContent("Scale Factor", "How much to scale the models compared to what is in the source file."));
                }
            }
            EditorGUILayout.Separator();

            ApplyRevertGUI();
        }
    }
}

#endif
