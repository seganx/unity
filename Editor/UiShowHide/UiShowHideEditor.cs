using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeganX
{
    [CustomEditor(typeof(UiShowHide)), CanEditMultipleObjects]
    public class UiShowHideEditor : Editor
    {
        private List<string> popupLabels = new List<string>();
        private List<int> popupIds = new List<int>();

        private void OnEnable()
        {
            popupLabels.Add("None");
            popupIds.Add(0);
            foreach (var item in UiShowHideConfig.Instance.configs)
            {
                popupLabels.Add(item.name);
                popupIds.Add(item.id);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var autoShow = serializedObject.FindProperty("autoShow");
            var startConfigId = serializedObject.FindProperty("startConfigId");
            var hideConfigId = serializedObject.FindProperty("hideConfigId");
            var startConfig = serializedObject.FindProperty("startConfig");
            var hideConfig = serializedObject.FindProperty("hideConfig");

            EditorGUILayout.PropertyField(autoShow);

            InspectorConfigPopup("Start Reference", startConfigId, startConfig);
            if (startConfigId.intValue > 0)
                EditorGUILayout.PropertyField(startConfig.FindPropertyRelative("delay"), new GUIContent("Start Delay"));

            InspectorConfigPopup("Hide Reference", hideConfigId, hideConfig);
            if (hideConfigId.intValue > 0)
                EditorGUILayout.PropertyField(hideConfig.FindPropertyRelative("delay"), new GUIContent("Hide Delay"));

            if (startConfigId.intValue == 0)
                InspectorConfig(startConfig);
            if (hideConfigId.intValue == 0)
                InspectorConfig(hideConfig);

            serializedObject.ApplyModifiedProperties();

            if (EditorApplication.isPlaying == false)
            {
                foreach (Component item in targets)
                {
                    var animator = item.GetComponent<Animator>();
                    if (animator) animator.enabled = false;
                }
            }
        }

        private void InspectorConfig(SerializedProperty config)
        {
            var rect = EditorGUILayout.GetControlRect(true, 25);
            EditorGUI.HelpBox(rect, string.Empty, MessageType.None);
            config.isExpanded = EditorGUI.Foldout(rect, config.isExpanded, " " + config.displayName, true);
            if (config.isExpanded)
            {
                EditorGUI.indentLevel++;
                config.NextVisible(true);
                EditorGUILayout.PropertyField(config);
                config.NextVisible(false);
                EditorGUILayout.PropertyField(config);
                config.NextVisible(false);
                EditorGUILayout.PropertyField(config);
                config.NextVisible(false);
                EditorGUILayout.PropertyField(config);
                config.NextVisible(false);
                EditorGUILayout.PropertyField(config);
                config.NextVisible(false);
                EditorGUI.indentLevel--;
            }
        }

        private void InspectorConfigPopup(string label, SerializedProperty configId, SerializedProperty currentState)
        {
            var newvalue = EditorGUILayout.IntPopup(label, configId.hasMultipleDifferentValues ? -1 : configId.intValue, popupLabels.ToArray(), popupIds.ToArray());
            if (newvalue == -1) return;
            configId.intValue = newvalue;

            var state = FindConfigProperty(configId.intValue);
            if (state != null)
            {
                currentState.FindPropertyRelative("curve").animationCurveValue = state.FindPropertyRelative("curve").animationCurveValue;
                currentState.FindPropertyRelative("direction").vector3Value = state.FindPropertyRelative("direction").vector3Value;
                currentState.FindPropertyRelative("scale").vector3Value = state.FindPropertyRelative("scale").vector3Value;
                currentState.FindPropertyRelative("alpha").floatValue = state.FindPropertyRelative("alpha").floatValue;
            }
            else configId.intValue = 0;
        }

        private SerializedProperty FindConfigProperty(int id)
        {
            var config = new SerializedObject(UiShowHideConfig.Instance);
            var list = config.FindProperty("configs");
            foreach (SerializedProperty item in list)
            {
                var itemid = item.FindPropertyRelative("id");
                if (itemid.intValue == id)
                    return item;
            }
            return null;
        }
    }
}
