using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeganX
{
    public class PersianPreviewAttribute : PropertyAttribute
    {
        public float height = 0;
        public bool force = true;
        public PersianPreviewAttribute() { }
        public PersianPreviewAttribute(bool forcePersian)
        {
            force = forcePersian;
        }
        public PersianPreviewAttribute(int previewHeight, bool forcePersian = true)
        {
            height = previewHeight;
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
        private float previewHeight;
        private bool forcePersian = true;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            baseHeight = base.GetPropertyHeight(property, label);
            previewHeight = attribute.As<PersianPreviewAttribute>().height;
            forcePersian = attribute.As<PersianPreviewAttribute>().force;
            if (previewHeight == 0) previewHeight = baseHeight;
            return baseHeight + previewHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = baseHeight;
            EditorGUI.PropertyField(position, property);
            position.y += baseHeight;
            position.height = previewHeight;
            EditorGUI.LabelField(position, property.stringValue.CleanFromCode().CleanForPersian().Persian(forcePersian), style);
        }
    }
#endif
}