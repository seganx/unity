using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class TypeValue
{
    public string type = string.Empty;
    public string value = string.Empty;
}

public class TypeValueTypesAttribute : PropertyAttribute
{
    private List<string> itemList = new List<string>();

    public List<string> Items => itemList;

    public TypeValueTypesAttribute(params string[] items)
    {
        itemList.AddRange(items);
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TypeValueTypesAttribute))]
public class TypeValuePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attrib = attribute as TypeValueTypesAttribute;

        position = EditorGUI.PrefixLabel(position, label);
        position.width /= 2;
        position.width -= 5;
        EditorGUI.indentLevel = 0;

        var typeProp = property.FindPropertyRelative("type");
        EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent("Type")).x;
        EditorGUI.BeginChangeCheck();
        var newIndex = EditorGUI.Popup(position, "Type", attrib.Items.FindIndex(x => x.Equals(typeProp.stringValue)), attrib.Items.ToArray());
        if (EditorGUI.EndChangeCheck())
            typeProp.stringValue = attrib.Items[newIndex];

        if (typeProp.stringValue != "Null" && typeProp.stringValue != "Free" && typeProp.stringValue != "Ad")
        {
            position.x += position.width + 10;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("value"));
        }
    }
}
#endif
