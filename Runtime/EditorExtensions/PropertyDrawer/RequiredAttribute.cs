using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeganX
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredAttribute : PropertyAttribute
    {
        public string Message { get; private set; }

        public RequiredAttribute(string message = null)
        {
            Message = message;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredAttributeDrawer : PropertyDrawer
    {
        private float height = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (IsAlramed(property))
            {
                var attrib = attribute as RequiredAttribute;
                height = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, new GUIContent(attrib.Message)) * 2;
                return height + base.GetPropertyHeight(property, new GUIContent(attrib.Message));
            }
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attrib = attribute as RequiredAttribute;

            if (IsAlramed(property))
            {
                var message = string.IsNullOrEmpty(attrib.Message) ? label.text + " Is Required!" : attrib.Message;
                var rect = position;
                rect.height = height;
                EditorGUI.HelpBox(rect, message, MessageType.Error);
            }

            position.y += height;
            position.height -= height;
            EditorGUI.PropertyField(position, property, label);
        }

        private bool IsAlramed(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null;
        }
    }
#endif
}