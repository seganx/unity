#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SeganX
{
    [CustomPropertyDrawer(typeof(Value))]
    public class ValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            var curr = prop.FindPropertyRelative("current");
            EditorGUI.BeginChangeCheck();
            var val = EditorGUI.FloatField(pos, label, curr.floatValue);
            if (EditorGUI.EndChangeCheck())
                curr.floatValue = val;
        }
    }
}
#endif