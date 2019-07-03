#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SeganX
{
    [CustomPropertyDrawer(typeof(CryptoInt))]
    public class CryptoIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            var key = prop.FindPropertyRelative("k");
            var value = prop.FindPropertyRelative("v");

            CryptoInt tmp;
            tmp.k = key.intValue;
            tmp.v = value.intValue;

            EditorGUI.BeginChangeCheck();
            int val = EditorGUI.DelayedIntField(pos, label, tmp);
            if (EditorGUI.EndChangeCheck())
            {
                tmp = val;
                key.intValue = tmp.k;
                value.intValue = tmp.v;
            }
        }
    }
}
#endif