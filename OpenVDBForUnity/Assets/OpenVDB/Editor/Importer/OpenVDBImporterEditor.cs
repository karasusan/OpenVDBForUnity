#if UNITY_2017_1_OR_NEWER

using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

namespace OpenVDB
{
    [CustomEditor(typeof(OpenVDBImporter)), CanEditMultipleObjects]
    public class OpenVDBImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            var pathSettings = "streamSettings.";

            EditorGUILayout.LabelField("Scene", EditorStyles.boldLabel);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(pathSettings + "scaleFactor"),
                    new GUIContent("Scale Factor", "How much to scale the models compared to what is in the source file."));
            }
            EditorGUILayout.Separator();

            ApplyRevertGUI();
        }
    }
}

#endif
