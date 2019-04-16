#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace SeganX
{
    public class EditorCacheItems
    {
        [MenuItem("SeganX/Cache/Clear All")]
        private static void ClearAll()
        {
            ClearPlayerPrefs();
            ClearFiles();
        }

        [MenuItem("SeganX/Cache/Clear PlayerPrefs")]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("SeganX/Cache/Clear Files")]
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