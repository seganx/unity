using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeganX
{
    public class EnumToggleAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EnumToggleAttribute))]
    public class EnumToggleAttributeDrawer : PropertyDrawer
    {
        public float buttonHeight = 0;
        public int rowsCount = 1;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            buttonHeight = base.GetPropertyHeight(property, label);
            return buttonHeight * rowsCount;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height), label);

            int buttonsIntValue = property.enumValueIndex;

            EditorGUI.BeginChangeCheck();

            rowsCount = 1;
            float top = rect.y;
            float left = rect.x + EditorGUIUtility.labelWidth;
            for (int i = 0; i < property.enumNames.Length; i++)
            {
                float buttonWidth = ComputeButtonWidth(property.enumNames[i]);
                if (left + buttonWidth > rect.width + rect.x)
                {
                    left = rect.x + EditorGUIUtility.labelWidth;
                    top += buttonHeight;
                    rowsCount++;
                }

                Rect buttonPos = new Rect(left, top, buttonWidth, buttonHeight);
                if (GUI.Toggle(buttonPos, buttonsIntValue == i, property.enumNames[i], "Button"))
                    buttonsIntValue = i;

                left += buttonWidth;
            }

            if (EditorGUI.EndChangeCheck())
                property.enumValueIndex = buttonsIntValue;
        }

        private float ComputeButtonWidth(string label)
        {
            return GUI.skin.label.CalcSize(new GUIContent(label)).x + 10;
        }
    }
#endif
}

