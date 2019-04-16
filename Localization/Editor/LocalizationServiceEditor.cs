using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeganX
{
    [CustomEditor(typeof(LocalizationService))]
    public class LocalizationServiceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var local = target as LocalizationService;

            if (local.currentKit == null)
            {
                EditorGUILayout.HelpBox("Please select a Kit file for localization service!", MessageType.Warning);
                local.currentKit = (LocalizationKit)EditorGUILayout.ObjectField("Currnet Kit:", local.currentKit, typeof(LocalizationKit), false);
            }
            else base.OnInspectorGUI();
        }
    }

    static class LocalizationMenu
    {
        [MenuItem("SeganX/Localization/Current Kit")]
        private static void CurrentKit()
        {
            Selection.activeObject = LocalizationService.Instance.currentKit;
        }

        [MenuItem("SeganX/Localization/Settings")]
        private static void Settings()
        {
            Selection.activeObject = LocalizationService.Instance;
        }
    }
}
