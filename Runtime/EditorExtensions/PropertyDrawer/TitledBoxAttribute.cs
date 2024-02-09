using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeganX
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class TitledBoxAttribute : PropertyAttribute
    {
        public int count = 1;
        public string title = string.Empty;

        public TitledBoxAttribute(string title, int fieldsCount = 1)
        {
            this.title = title;
            count = fieldsCount;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TitledBoxAttribute))]
    public class TitledBoxAttributePropertyDrawer : PropertyDrawer
    {
        private float height;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            height = base.GetPropertyHeight(property, label);
            return height * 2 + EditorGUIUtility.standardVerticalSpacing * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attrib = attribute as TitledBoxAttribute;


            position.y += EditorGUIUtility.standardVerticalSpacing;
            position.height = height;
            EditorGUI.DrawRect(position, new Color(0.1f, 0.1f, 0.1f));
            position.x += 5;
            EditorGUI.LabelField(position, new GUIContent(attrib.title));
            position.x -= 5;

            position.y += height;
            position.height = (height + EditorGUIUtility.standardVerticalSpacing) * attrib.count + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.HelpBox(position, string.Empty, MessageType.None);

            position.y += EditorGUIUtility.standardVerticalSpacing;
            position.height = height;
            EditorGUI.PropertyField(position, property, label);
        }
    }
#endif
}