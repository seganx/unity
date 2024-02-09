using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeganX
{
    public class VectorLabelsAttribute : PropertyAttribute
    {
        public string captionX = "X";
        public string captionY = "Y";
        public string captionZ = "Z";
        public string captionW = "W";

        public VectorLabelsAttribute(string x, string y)
        {
            captionX = x;
            captionY = y;
        }

        public VectorLabelsAttribute(string x, string y, string z)
        {
            captionX = x;
            captionY = y;
            captionZ = z;
        }

        public VectorLabelsAttribute(string x, string y, string z, string w)
        {
            captionX = x;
            captionY = y;
            captionZ = z;
            captionW = w;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(VectorLabelsAttribute))]
    public class VectorLabelsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (position.width < 3) return;

            int lenght = 0;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2: lenght = 2; break;
                case SerializedPropertyType.Vector3: lenght = 3; break;
                case SerializedPropertyType.Vector4: lenght = 4; break;
                case SerializedPropertyType.Vector2Int: lenght = 2; break;
                case SerializedPropertyType.Vector3Int: lenght = 3; break;
            }
            if (lenght < 2)
            {
                EditorGUI.LabelField(position, "Invalid Attribute!");
                Debug.LogError("VectorLabels attribute must be used for vectors!");
                return;
            }
            position = EditorGUI.PrefixLabel(position, label);

            var attrib = attribute as VectorLabelsAttribute;
            var captionX = new GUIContent(attrib.captionX);
            var captionY = new GUIContent(attrib.captionY);
            var captionZ = new GUIContent(attrib.captionZ);
            var captionW = new GUIContent(attrib.captionW);
            var xSize = GUI.skin.label.CalcSize(captionX).x;
            var ySize = GUI.skin.label.CalcSize(captionY).x;
            var zSize = GUI.skin.label.CalcSize(captionZ).x;
            var wSize = GUI.skin.label.CalcSize(captionW).x;

            var space = lenght == 2 ? 25 : lenght == 3 ? 15 : 5;
            var totalTextWidth = xSize + ySize;
            if (lenght > 2) totalTextWidth += zSize;
            if (lenght > 3) totalTextWidth += wSize;
            var totalBoxWidth = position.width - totalTextWidth - space * (lenght - 1);
            var boxWidth = totalBoxWidth / lenght;

            var initLabelWidth = EditorGUIUtility.labelWidth;
            var initIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUIUtility.labelWidth = xSize;
            position.width = xSize + boxWidth;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("x"), captionX);
            position.x += position.width + space;

            EditorGUIUtility.labelWidth = ySize;
            position.width = ySize + boxWidth;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("y"), captionY);
            position.x += position.width + space;

            if (lenght > 2)
            {
                EditorGUIUtility.labelWidth = zSize;
                position.width = zSize + boxWidth;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("z"), captionZ);
                position.x += position.width + space;
            }

            if (lenght > 3)
            {
                EditorGUIUtility.labelWidth = wSize;
                position.width = wSize + boxWidth;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("w"), captionW);
            }

            EditorGUI.indentLevel = initIndentLevel;
            EditorGUIUtility.labelWidth = initLabelWidth;
        }
    }
#endif
}
