using UnityEditor;
using UnityEngine;

namespace SeganX.Localization
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LocalText), true)]
    public class LocalTextEditor : Editor
    {
        private enum ChangeState { Null, Key, Apply };
        private ChangeState changeState = ChangeState.Null;

        private SerializedProperty displayTextProp = null;
        private SerializedProperty autoRtlProp = null;
        private SerializedProperty autoWidthProp = null;
        private SerializedProperty autoHeightProp = null;
        private SerializedProperty textProp = null;
        private SerializedProperty stringKeyProp = null;

        private string newKey = string.Empty;

        private bool IsLinked => stringKeyProp.stringValue.Length > 0;

        private void OnEnable()
        {
            displayTextProp = serializedObject.FindProperty("displayText");
            autoRtlProp = serializedObject.FindProperty("autoRtl");
            autoWidthProp = serializedObject.FindProperty("autoWidth");
            autoHeightProp = serializedObject.FindProperty("autoHeight");
            textProp = serializedObject.FindProperty("textbase");
            stringKeyProp = serializedObject.FindProperty("stringKey");

            if (IsLinked)
            {
                serializedObject.Update();
                textProp.stringValue = LocalKit.Get(stringKeyProp.stringValue);
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(displayTextProp);

            if (IsLinked)
                EditorGUILayout.PropertyField(textProp, new GUIContent($"Text is linked to [{stringKeyProp.stringValue}]"));
            else
                EditorGUILayout.PropertyField(textProp, new GUIContent("Text"));

            var rect = EditorGUILayout.GetControlRect();
            var maxWidth = rect.width;

            switch (changeState)
            {
                case ChangeState.Null:
                    {
                        rect.width = 100;
                        if (IsLinked)
                        {
                            if (GUI.Button(rect, "Unlink"))
                                stringKeyProp.stringValue = string.Empty;
                        }
                        else
                        {
                            if (GUI.Button(rect, "Set Key"))
                                changeState = ChangeState.Key;
                        }

                        rect.width = 80;
                        rect.x += maxWidth - rect.width;
                        autoRtlProp.boolValue = GUI.Toggle(rect, autoRtlProp.boolValue, "Auto RTL", "Button");
                        rect.x -= rect.width + 5;
                        autoHeightProp.boolValue = GUI.Toggle(rect, autoHeightProp.boolValue, "Auto Height", "Button");
                        rect.x -= rect.width + 5;
                        autoWidthProp.boolValue = GUI.Toggle(rect, autoWidthProp.boolValue, "Auto Width", "Button");

                    }
                    break;
                case ChangeState.Key:
                    {
                        rect.width -= 120;
                        newKey = EditorGUI.TextField(rect, newKey);
                        rect.x += maxWidth - 100;
                        rect.width = 100;
                        if (GUI.Button(rect, "Apply"))
                        {
                            var exist = LocalKit.Exist(newKey);
                            if (exist)
                            {
                                stringKeyProp.stringValue = newKey;
                                textProp.stringValue = LocalKit.Get(newKey);
                            }
                            changeState = ChangeState.Null;
                        }
                    }
                    break;
            }

            serializedObject.ApplyModifiedProperties();

            foreach (LocalText item in targets)
            {
                LocalText.Editor.DisplayText(item);
                item.UpdateRectSize();
            }
        }
    }
}
