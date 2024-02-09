#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SeganX
{
    public class EditorCacheItems
    {
        [MenuItem("SeganX/Cache/Clear All", priority = 151)]
        private static void ClearAll()
        {
            ClearPlayerPrefs();
            ClearFiles();
        }

        [MenuItem("SeganX/Cache/Clear PlayerPrefs", priority = 152)]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefsEx.ClearData();
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("SeganX/Cache/Clear Files", priority = 153)]
        private static void ClearFiles()
        {
            Caching.ClearCache();

            var files = Directory.GetFiles(Application.persistentDataPath);
            foreach (var item in files)
                try { File.Delete(item); }
                catch { };

            var dirs = Directory.GetDirectories(Application.persistentDataPath);
            foreach (var item in dirs)
                try { Directory.Delete(item, true); }
                catch { };

            files = Directory.GetFiles(Application.temporaryCachePath);
            foreach (var item in files)
                try { File.Delete(item); }
                catch { };

            dirs = Directory.GetDirectories(Application.temporaryCachePath);
            foreach (var item in dirs)
                try { Directory.Delete(item, true); }
                catch { };
        }
    }
}
#endif