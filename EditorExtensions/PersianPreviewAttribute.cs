using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeganX
{
    public class PersianPreviewAttribute : PropertyAttribute
    {
        public float height = 0;
        public PersianPreviewAttribute() { }
        public PersianPreviewAttribute(int previewHeight)
        {
            height = previewHeight;
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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            baseHeight = base.GetPropertyHeight(property, label);
            previewHeight = attribute.As<PersianPreviewAttribute>().height;
            if (previewHeight == 0) previewHeight = baseHeight;
            return baseHeight + previewHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = baseHeight;
            EditorGUI.PropertyField(position, property);

            var persianStr = PersianTextShaper.PersianTextShaper.ShapeText(property.stringValue.CleanFromCode().CleanForPersian());
            position.y += baseHeight;
            position.height = previewHeight;
            EditorGUI.LabelField(position, persianStr, style);
        }
    }
#endif
}