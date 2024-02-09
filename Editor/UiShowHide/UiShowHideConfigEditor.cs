using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeganX
{
    [CustomEditor(typeof(UiShowHideConfig))]
    public class UiShowHideConfigEditor : Editor
    {
        private UiShowHideConfig config = null;

        private int NextId
        {
            get
            {
                int res = 0;
                foreach (var item in config.configs)
                    if (item.id > res)
                        res = item.id;
                return res + 1;
            }
        }

        private void OnEnable()
        {
            config = target as UiShowHideConfig;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var activatedProp = serializedObject.FindProperty("activated");
            EditorGUILayout.PropertyField(activatedProp);
            EditorGUILayout.Space(20);

            if (GUILayout.Button("Add Config"))
            {
                config.configs.Add(new UiShowHideConfig.State()
                {
                    name = "Element " + NextId,
                    id = NextId
                });
            }
            EditorGUILayout.Space();

            var iterator = serializedObject.FindProperty("configs");

            int i = 0;
            foreach (SerializedProperty item in iterator)
            {
                var rect = EditorGUILayout.GetControlRect(true, 25);
                EditorGUI.HelpBox(rect, string.Empty, MessageType.None);

                if (GUI.Button(new Rect(rect.x + rect.width - 25, rect.y, 25, 25), "X"))
                    item.DeleteCommand();
                if (GUI.Button(new Rect(rect.x + rect.width - 75, rect.y, 25, 25), "▲"))
                    iterator.MoveArrayElement(i, i - 1);
                if (GUI.Button(new Rect(rect.x + rect.width - 100, rect.y, 25, 25), "▼"))
                    iterator.MoveArrayElement(i, i + 1);

                item.isExpanded = EditorGUI.Foldout(rect, item.isExpanded, " " + item.displayName, true);
                if (item.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    item.NextVisible(true);
                    EditorGUILayout.PropertyField(item);
                    item.NextVisible(false);
                    EditorGUILayout.PropertyField(item);
                    item.NextVisible(false);
                    EditorGUILayout.PropertyField(item);
                    item.NextVisible(false);
                    EditorGUILayout.PropertyField(item);
                    item.NextVisible(false);
                    EditorGUILayout.PropertyField(item);
                    item.NextVisible(false);
                    EditorGUILayout.PropertyField(item);
                    EditorGUI.indentLevel--;
                }

                i++;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}