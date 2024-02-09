using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeganX.Spline
{
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : Editor
    {
        private Spline spline = null;

        private void OnEnable()
        {
            spline = target as Spline;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var auto = serializedObject.FindProperty("auto");
            var parts = serializedObject.FindProperty("parts");
            var power = serializedObject.FindProperty("power");

            int partsValue = parts.intValue;
            float powerValue = power.floatValue;

            EditorGUI.BeginChangeCheck();
            bool autoValue = EditorGUILayout.Toggle("Automatic bezeir", auto.boolValue);
            if (autoValue == false)
            {
                partsValue = EditorGUILayout.IntField("Points count", parts.intValue);
                powerValue = EditorGUILayout.FloatField("Bezier power", power.floatValue);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, target.name + " params");
                auto.boolValue = autoValue;
                parts.intValue = partsValue;
                power.floatValue = powerValue;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                spline.Create();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Total Length: " + spline.Length, EditorStyles.label);
        }

        private void OnSceneGUI()
        {
            var section = target as Spline;
            section.GetComponentsInChildren(temp);

            if (Tools.current == Tool.Move)
                HandlePositions(section);
            else if (Tools.current == Tool.Rotate)
                HandleRotation(section);
            else if (Tools.current == Tool.Transform)
                HandleTransform(section);
        }

        private void HandlePositions(Spline section)
        {
            for (int i = 0; i < temp.Count; i++)
            {
                EditorGUI.BeginChangeCheck();
                var position = Handles.PositionHandle(temp[i].transform.position, Tools.pivotRotation == PivotRotation.Local ? temp[i].transform.rotation : Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(section, "Change child transform");
                    temp[i].transform.position = position;
                    EditorUtility.SetDirty(target);
                    Repaint();
                    spline.Create();
                }
            }
        }

        private void HandleRotation(Spline section)
        {
            for (int i = 0; i < temp.Count; i++)
            {
                EditorGUI.BeginChangeCheck();
                var rotation = Handles.RotationHandle(Tools.pivotRotation == PivotRotation.Local ? temp[i].transform.rotation : Quaternion.identity, temp[i].transform.position);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(section, "Change child transform");
                    temp[i].transform.rotation = rotation;
                    EditorUtility.SetDirty(target);
                    Repaint();
                    spline.Create();
                }
            }
        }

        private void HandleTransform(Spline section)
        {
            for (int i = 0; i < temp.Count; i++)
            {
                var position = temp[i].transform.position;
                var rotation = Tools.pivotRotation == PivotRotation.Local ? temp[i].transform.rotation : Quaternion.identity;

                EditorGUI.BeginChangeCheck();
                Handles.TransformHandle(ref position, ref rotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(section, "Change child transform");
                    temp[i].transform.position = position;
                    temp[i].transform.rotation = rotation;
                    EditorUtility.SetDirty(target);
                    Repaint();
                    spline.Create();
                }
            }
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static readonly List<SplinePoint> temp = new List<SplinePoint>(16);
    }
}
