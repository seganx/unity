using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeganX
{
    public class EnumFlagAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
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
            int buttonsIntValue = 0;
            int enumLength = property.enumNames.Length;
            bool[] buttonPressed = new bool[enumLength];

            EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height), label);

            EditorGUI.BeginChangeCheck();

            rowsCount = 1;
            float top = rect.y;
            float left = rect.x + EditorGUIUtility.labelWidth;
            for (int i = 0; i < enumLength; i++)
            {
                // Check if the button is/was pressed 
                if ((property.intValue & (1 << i)) == 1 << i)
                {
                    buttonPressed[i] = true;
                }


                float buttonWidth = ComputeButtonWidth(property.enumNames[i]);
                if (left + buttonWidth > rect.width + rect.x)
                {
                    left = rect.x + EditorGUIUtility.labelWidth;
                    top += buttonHeight;
                    rowsCount++;
                }

                Rect buttonPos = new Rect(left, top, buttonWidth, buttonHeight);
                buttonPressed[i] = GUI.Toggle(buttonPos, buttonPressed[i], property.enumNames[i], "Button");
                if (buttonPressed[i])
                    buttonsIntValue |= 1 << i;

                left += buttonWidth;
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = buttonsIntValue;
            }
        }

        private float ComputeButtonWidth(string label)
        {
            return GUI.skin.label.CalcSize(new GUIContent(label)).x + 10;
        }
    }
#endif
}
