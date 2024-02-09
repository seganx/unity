using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeganX
{
    public static class EditorUtils
    {
        public static void BeginBoxWithHeader(string title)
        {
            var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight - 4);
            rect.height += EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f));
            rect.x += 2;
            EditorGUI.LabelField(rect, title);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        }

        public static void EndBoxWithHeader()
        {
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }
    }
}