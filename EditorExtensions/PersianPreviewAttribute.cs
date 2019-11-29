using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeganX
{
    public class PersianPreviewAttribute : PropertyAttribute
    {
        public int lines = 0;
        public bool force = true;
        public PersianPreviewAttribute() { }
        public PersianPreviewAttribute(bool forcePersian)
        {
            force = forcePersian;
        }
        public PersianPreviewAttribute(int lines, bool forcePersian = true)
        {
            this.lines = lines;
            force = forcePersian;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PersianPreviewAttribute))]
    public class PersianPreviewAttributeDrawer : PropertyDrawer
    {
        private static GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.UpperRight,
            wordWrap = true,
            border = new RectOffset(10, 10, 0, 0),
            padding = new RectOffset(10, 15, 0, 0)
        };

        private float baseHeight;
        private int lines;
        private bool forcePersian = true;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            baseHeight = base.GetPropertyHeight(property, label);
            lines = attribute.As<PersianPreviewAttribute>().lines;
            forcePersian = attribute.As<PersianPreviewAttribute>().force;
            if (lines < 1) lines = 1;
            return baseHeight * lines * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height /= 2;
            EditorGUI.LabelField(position, label);
            position.x += EditorGUIUtility.labelWidth - EditorGUIUtility.fieldWidth;
            position.width -= EditorGUIUtility.labelWidth - EditorGUIUtility.fieldWidth;
            property.stringValue = EditorGUI.TextArea(position, property.stringValue);
            position.y += position.height;
            EditorGUI.LabelField(position, property.stringValue.CleanFromCode().CleanForPersian().Persian(forcePersian), style);
        }
    }
#endif
}