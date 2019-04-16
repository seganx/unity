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
            var key = prop.FindPropertyRelative("key");
            var value = prop.FindPropertyRelative("value");
            EditorGUI.BeginChangeCheck();
            var val = EditorGUI.IntField(pos, label, CryptoInt.Decrypt(value.intValue, key.intValue));
            if (EditorGUI.EndChangeCheck())
                value.intValue = CryptoInt.Encrypt(val, key.intValue);
        }
    }
}
#endif