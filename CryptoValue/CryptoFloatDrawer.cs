#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SeganX
{
    [CustomPropertyDrawer(typeof(CryptoFloat))]
    public class CryptoFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            var key = prop.FindPropertyRelative("key");
            var value = prop.FindPropertyRelative("value");
            EditorGUI.BeginChangeCheck();
            var val = EditorGUI.FloatField(pos, label, CryptoFloat.Decrypt(value.intValue, key.intValue));
            if (EditorGUI.EndChangeCheck())
                value.intValue = CryptoFloat.Encrypt(val, key.intValue);
        }
    }
}
#endif