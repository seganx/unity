
using UnityEditor;

namespace SeganX.Spline
{
    [CustomEditor(typeof(SplinePoint))]
    public class SplinePointEditor : Editor
    {
        private SplinePoint point = null;
        private Spline spline = null;

        private void OnEnable()
        {
            point = target as SplinePoint;
            spline = point.transform.GetComponentInParent<Spline>(true);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var auto = serializedObject.FindProperty("autoPower");
            var power = serializedObject.FindProperty("power");

            float powerValue = power.floatValue;

            EditorGUI.BeginChangeCheck();
            bool autoValue = EditorGUILayout.Toggle("Automatic Power", auto.boolValue);
            if (autoValue == false)
                powerValue = EditorGUILayout.FloatField("Power", power.floatValue);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, target.name + " params");
                auto.boolValue = autoValue;
                power.floatValue = powerValue;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                if (spline != null) spline.Create();
            }
        }

        private void OnSceneGUI()
        {
            if (spline != null && point.transform.hasChanged)
            {
                point.transform.hasChanged = false;
                spline.Create();
            }
        }
    }
}

