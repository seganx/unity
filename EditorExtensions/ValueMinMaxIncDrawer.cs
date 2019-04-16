#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SeganX
{
    [CustomPropertyDrawer(typeof(ValueMinMaxInc))]
    public class ValueMinMaxIncDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            var cur = prop.FindPropertyRelative("current");
            var max = prop.FindPropertyRelative("max");
            var min = prop.FindPropertyRelative("min");
            var inc = prop.FindPropertyRelative("perSecond");
            var str = label.text + ": [Current:" + cur.floatValue.ToString("0.0") + ", Max:" + max.floatValue + ", Min:" + min.floatValue + ", PerSecond:" + inc.floatValue + "]";
            EditorGUI.PropertyField(pos, prop, new GUIContent(str), true);
        }
    }
}
#endif