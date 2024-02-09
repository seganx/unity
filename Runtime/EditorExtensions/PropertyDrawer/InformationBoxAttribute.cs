using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeganX
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class InformationBoxAttribute : PropertyAttribute
    {
        public enum Type
        {
            None = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }

        public readonly string message;
        public readonly Type messageType;

        public InformationBoxAttribute(string message, Type messageType)
        {
            this.message = message;
            this.messageType = messageType;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InformationBoxAttribute))]
    public class InformationBoxAttributePropertyDrawer : PropertyDrawer
    {
        private float height;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attrib = attribute as InformationBoxAttribute;
            height = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, new GUIContent(attrib.message)) * 2;
            return height + base.GetPropertyHeight(property, new GUIContent(attrib.message));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attrib = attribute as InformationBoxAttribute;
            var rect = position;

            rect.height = height;
            EditorGUI.HelpBox(rect, attrib.message, (MessageType)attrib.messageType);

            position.y += height;
            position.height -= height;
            EditorGUI.PropertyField(position, property, label);
        }
    }
#endif
}