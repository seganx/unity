using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeganX
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class LineAttribute : PropertyAttribute
    {
        public Color color;
        public int thickness;

        public LineAttribute(float r, float g, float b, int thickness)
        {
            color = new Color(r, g, b);
            this.thickness = thickness;
        }
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LineAttribute))]
    public class LineAttributePropertyDrawer : PropertyDrawer
    {
        private float height;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attrib = attribute as LineAttribute;
            height = base.GetPropertyHeight(property, label);
            return height + attrib.thickness + EditorGUIUtility.standardVerticalSpacing * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attrib = attribute as LineAttribute;

            position.y += EditorGUIUtility.standardVerticalSpacing;
            position.height = attrib.thickness;
            EditorGUI.DrawRect(position, attrib.color);
            position.y += attrib.thickness + EditorGUIUtility.standardVerticalSpacing;
            position.height = height;
            EditorGUI.PropertyField(position, property, label);
        }
    }
#endif
}