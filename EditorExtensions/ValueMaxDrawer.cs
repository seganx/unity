#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SeganX
{
    [CustomPropertyDrawer(typeof(ValueMax))]
    public class ValueMaxDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            var cur = prop.FindPropertyRelative("current");
            var max = prop.FindPropertyRelative("max");
            var str = label.text + ": [Current:" + cur.floatValue + ", Max:" + max.floatValue + "]";
            EditorGUI.PropertyField(pos, prop, new GUIContent(str), true);
        }
    }
}
#endif