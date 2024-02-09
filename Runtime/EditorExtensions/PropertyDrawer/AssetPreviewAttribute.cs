using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeganX
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AssetPreviewAttribute : PropertyAttribute
    {
        public float Height { get; private set; }

        public AssetPreviewAttribute() { }
        public AssetPreviewAttribute(int previewHeight)
        {
            Height = previewHeight;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AssetPreviewAttribute))]
    public class AssetPreviewAttributeDrawer : PropertyDrawer
    {
        private float baseHeight;
        private float previewHeight;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            baseHeight = base.GetPropertyHeight(property, label);
            previewHeight = attribute.As<AssetPreviewAttribute>().Height;
            if (previewHeight == 0)
            {
                var texture = GetAssetTexture(property);
                if (texture != null)
                    previewHeight = texture.height;
            }
            return baseHeight + previewHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = baseHeight;
            EditorGUI.PropertyField(position, property);

            if (property.objectReferenceValue != null)
            {
                var texture = GetAssetTexture(property);
                if (texture != null)
                {
                    position.y += baseHeight;
                    position.x += EditorGUIUtility.labelWidth;
                    position.width -= EditorGUIUtility.labelWidth;
                    position.height = previewHeight;
                    GUI.Label(position, texture);
                }
            }
        }

        private Texture2D GetAssetTexture(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
                return AssetPreview.GetAssetPreview(property.objectReferenceValue);
            return null;
        }
    }
#endif

}