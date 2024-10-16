using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SeganX
{
    public class DropdownAttribute : PropertyAttribute
    {
        private List<string> itemList = new List<string>();

        public List<string> Items => itemList;

        public DropdownAttribute(params string[] items)
        {
            itemList.AddRange(items);
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attrib = attribute as DropdownAttribute;

            EditorGUI.BeginChangeCheck();

            var newIndex = EditorGUI.Popup(position, label.text, attrib.Items.FindIndex(x => x.Equals(property.stringValue)), attrib.Items.ToArray());

            if (EditorGUI.EndChangeCheck())
                property.stringValue = attrib.Items[newIndex];
        }
    }
#endif
}