using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace SeganX
{
    public class InspectorButtonAttribute : PropertyAttribute
    {
        public readonly bool hideProperty = true;
        public readonly float buttonWidth = -1;
        public readonly string[] buttonNames;
        public readonly string[] methodNames;

        public InspectorButtonAttribute(string buttonName, string methodName, bool hideProperty = true)
        {
            buttonWidth = -1;
            methodNames = new string[1] { methodName };
            buttonNames = new string[1] { buttonName };
            this.hideProperty = hideProperty;
        }

        public InspectorButtonAttribute(float width, string buttonName, string methodName, bool hideProperty = true)
        {
            buttonWidth = width;
            methodNames = new string[1] { methodName };
            buttonNames = new string[1] { buttonName };
            this.hideProperty = hideProperty;
        }

        public InspectorButtonAttribute(float width, string buttonName1, string methodName1, string buttonName2, string methodName2, bool hideProperty = true)
        {
            buttonWidth = width;
            methodNames = new string[2] { methodName1, methodName2 };
            buttonNames = new string[2] { buttonName1, buttonName2 };
            this.hideProperty = hideProperty;
        }

        public InspectorButtonAttribute(float width, string buttonName1, string methodName1, string buttonName2, string methodName2, string buttonName3, string methodName3, bool hideProperty = true)
        {
            buttonWidth = width;
            methodNames = new string[3] { methodName1, methodName2, methodName3 };
            buttonNames = new string[3] { buttonName1, buttonName2, buttonName3 };
            this.hideProperty = hideProperty;
        }

        public InspectorButtonAttribute(float width, string buttonName1, string methodName1, string buttonName2, string methodName2, string buttonName3, string methodName3, string buttonName4, string methodName4, bool hideProperty = true)
        {
            buttonWidth = width;
            methodNames = new string[4] { methodName1, methodName2, methodName3, methodName4 };
            buttonNames = new string[4] { buttonName1, buttonName2, buttonName3, buttonName4 };
            this.hideProperty = hideProperty;
        }

    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    public class InspectorButtonPropertyDrawer : PropertyDrawer
    {
        void CallMethod(SerializedProperty prop, string eventName)
        {
            System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
            var eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (eventMethodInfo != null)
                eventMethodInfo.Invoke(prop.serializedObject.targetObject, new object[1] { prop.propertyPath });
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attrib = attribute as InspectorButtonAttribute;
            if (attrib.hideProperty)
                return EditorGUI.GetPropertyHeight(property, label, true);
            else
                return EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var attrib = attribute as InspectorButtonAttribute;

            var n = attrib.methodNames.Length - 1;
            float xOffset = n < 1 ? 0 : (position.width - attrib.buttonWidth) / n;
            float buttonWidth = attrib.buttonWidth < 1 ? position.width : attrib.buttonWidth;
            for (int i = 0; i < attrib.methodNames.Length; i++)
            {
                Rect buttonRect = new Rect(position.x + xOffset * i, position.y, buttonWidth, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(buttonRect, attrib.buttonNames[i]))
                    CallMethod(prop, attrib.methodNames[i]);
            }

            if (attrib.hideProperty == false)
            {
                position.y += EditorGUIUtility.singleLineHeight;
                position.height -= EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, prop, true);
            }
        }
    }
#endif
}
