using System.IO;
using UnityEngine;
using UnityEditor;

namespace SeganX
{
    [CustomEditor(typeof(Core))]
    public class CoreEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Encrypt File"))
            {
                var path = EditorUtility.OpenFilePanel("Encrypt File And Save", "", "");
                if (path.Length > 3)
                {
                    var src = File.ReadAllBytes(path);
                    var data = CryptoService.EncryptWithMac(src, Core.CryptoKey, Core.Salt);
                    File.WriteAllBytes(path + ".seganx", data);
                }
            }

            if (GUILayout.Button("Decrypt File"))
            {
                var path = EditorUtility.OpenFilePanel("Decrypt File And Save", "", "");
                if (path.Length > 3)
                {
                    var src = File.ReadAllBytes(path);
                    var data = CryptoService.DecryptWithMac(src, Core.CryptoKey, Core.Salt);
                    File.WriteAllBytes(path.Replace(".seganx", ""), data);
                }
            }
        }
    }

    static class CoreMenu
    {
        [MenuItem("SeganX/Settings")]
        private static void Settings()
        {
            Selection.activeObject = Core.Instance;
        }
    }
}
