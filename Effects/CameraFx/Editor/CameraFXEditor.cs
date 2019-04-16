using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SeganX.Effects
{
#if OFF
    [CustomEditor(typeof(CameraFX))]
    public class CameraFXEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorApplication.isPlaying == false) return;
            var camerafx = target as CameraFX;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Realtime Configuration:");
            EditorGUILayout.Space();
            camerafx.Resolution = EditorGUILayout.Slider("Resolution:", camerafx.Resolution, 20, 100, null).ToInt();
            EditorGUILayout.LabelField("Width x Height:", camerafx.Width + "x" + camerafx.Height);
        }
    }
#endif
}
