using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeganX
{
    public class TagSelectorAttribute : PropertyAttribute
    {
        public bool UseDefaultTagFieldDrawer = false;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);

                var attrib = this.attribute as TagSelectorAttribute;

                if (attrib.UseDefaultTagFieldDrawer)
                {
                    property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                }
                else
                {
                    //generate the taglist + custom tags
                    List<string> tagList = new List<string>();
                    tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);
                    string propertyString = property.stringValue;
                    int index = 0;
                    for (int i = 1; i < tagList.Count; i++)
                    {
                        if (tagList[i] == propertyString)
                        {
                            index = i;
                            break;
                        }
                    }

                    //Draw the popup box with the current selected index
                    index = EditorGUI.Popup(position, label.text, index, tagList.ToArray());

                    //Adjust the actual string value of the property based on the selection
                    if (index == 0)
                        property.stringValue = string.Empty;
                    else if (index >= 1)
                        property.stringValue = tagList[index];
                    else
                        property.stringValue = string.Empty;
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
#endif
}