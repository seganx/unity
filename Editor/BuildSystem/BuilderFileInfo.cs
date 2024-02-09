#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace SeganX.Builder
{
    public enum FileType : int { ReplaceText = 0, ReplaceFile = 1, DisableFile = 2 }

    [System.Serializable]
    public class FileInfo
    {
        [HideInInspector] public string title = string.Empty;
        public FileType type = FileType.ReplaceText;
        public Object destFile = null;
        public Object sourceFile = null;
        public string what = string.Empty;
        [TextArea(1, 20)] public string with = string.Empty;
        public string Filename { set; get; }
        public string Sourcename { set; get; }
    }

#if OFF || UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FileInfo), true)]
    public class FileInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            var height = rect.height;
            rect.height = EditorGUIUtility.singleLineHeight;
            prop.isExpanded = EditorGUI.Foldout(rect, prop.isExpanded, label, true);
            if (prop.isExpanded == false) return;

            var type = prop.FindPropertyRelative("type");
            var destFile = prop.FindPropertyRelative("destFile");
            var sourceFile = prop.FindPropertyRelative("sourceFile");
            var what = prop.FindPropertyRelative("what");
            var with = prop.FindPropertyRelative("with");


            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing;
            GUI.Box(rect, string.Empty, EditorStyles.helpBox);
            rect.width -= EditorGUIUtility.standardVerticalSpacing * 2;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += EditorGUIUtility.standardVerticalSpacing * 2;

            switch (type.enumValueIndex)
            {
                case 0:
                    {
                        if (DrawTypeField(rect, prop, type)) return;
                        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(rect, destFile);
                        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(rect, what);
                        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                        rect.height = height - EditorGUIUtility.singleLineHeight * 5 - EditorGUIUtility.standardVerticalSpacing * 2;
                        EditorGUI.PropertyField(rect, with);
                    }
                    break;
                case 1:
                    {
                        if (DrawTypeField(rect, prop, type)) return;
                        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(rect, sourceFile);
                        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(rect, destFile);
                    }
                    break;
                case 2:
                    {
                        if (DrawTypeField(rect, prop, type)) return;
                        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(rect, destFile);
                    }
                    break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            if (prop.isExpanded)
            {
                var type = prop.FindPropertyRelative("type");
                var with = prop.FindPropertyRelative("with");
                switch (type.enumValueIndex)
                {
                    case 2: return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 4;
                    case 1: return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 5;
                    case 0: return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 5 + EditorGUI.GetPropertyHeight(with);
                    default: return base.GetPropertyHeight(prop, label);
                }
            }
            else
            {
                return base.GetPropertyHeight(prop, label);
            }
        }

        private bool DrawTypeField(Rect rect, SerializedProperty parent, SerializedProperty type)
        {

            rect.width -= 50;
            EditorGUI.PropertyField(rect, type);
            rect.x = rect.max.x;
            rect.width = 25;
            if (GUI.Button(rect, "D"))
                parent.DuplicateCommand();
            rect.x += 25;
            if (GUI.Button(rect, "X"))
            {
                parent.DeleteCommand();
                return true;
            }
            return false;
        }
    }
#endif
}
#endif