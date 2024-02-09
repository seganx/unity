using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class ResourceFiles : StaticConfig<ResourceFiles>
    {
        [System.Serializable]
        public class File
        {
            public string path = string.Empty;
            public string dire = string.Empty;
            public string name = string.Empty;
            public List<string> tags = new List<string>();
            public List<int> ids = new List<int>();
            public int Id => ids.Count > 0 ? ids[0] : -1;
        }

        public bool justFilesWithId = true;
        public List<File> files = new List<File>();


        ///////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ///////////////////////////////////////////////////////////////
        public static File Find(string dire, bool subfolders, System.Predicate<File> match)
        {
            if (dire.EndsWith("/") == false) dire += '/';
            return Instance.files.Find(x => IsSimilarPath(x.dire, dire, subfolders) && match(x));
        }

        public static List<File> FindAll(string dire, bool subfolders)
        {
            if (dire.EndsWith("/") == false) dire += '/';
            var res = Instance.files.FindAll(x => IsSimilarPath(x.dire, dire, subfolders));
            res.Sort((x, y) => x.Id - y.Id);
            return res;
        }

        public static List<File> FindAll(string dire, bool subfolders, System.Predicate<File> match)
        {
            if (dire.EndsWith("/") == false) dire += '/';
            var res = Instance.files.FindAll(x => IsSimilarPath(x.dire, dire, subfolders) && match(x));
            res.Sort((x, y) => x.Id - y.Id);
            return res;
        }

        private static bool IsSimilarPath(string path, string str, bool subfolders)
        {
            if (path == null || path.Length < 0) return false;

            if (subfolders)
            {
                var index = path.IndexOf(str, System.StringComparison.OrdinalIgnoreCase);
                return index == 0;
            }
            else
            {
                return path == str;
            }
        }




        //////////////////////////////////////////////////////
        /// EDITOR MEMBERS
        //////////////////////////////////////////////////////
#if UNITY_EDITOR
        private static void UpdateFiles()
        {
            Instance.files.Clear();
            AddFiles(Application.dataPath.PreparePath(true));
            Instance.files.Sort((x, y) => x.Id >= 0 && y.Id >= 0 ? x.Id - y.Id : string.Compare(x.path, y.path));
        }

        private static void AddFiles(string path)
        {
            if (System.IO.Directory.Exists(path) == false) return;

            var files = System.IO.Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                var item = files[i].PreparePath();
                if (System.IO.Path.GetExtension(item) == ".meta") continue;
                if (item.Contains("/Resources/") == false) continue;
                AddFile(item.Remove(0, item.LastIndexOf("/Resources/") + 11));
            }

            var dirs = System.IO.Directory.GetDirectories(path);
            foreach (var item in dirs)
                AddFiles(item);
        }

        private static void AddFile(string filepath)
        {
            var resname = System.IO.Path.GetFileNameWithoutExtension(filepath);
            if (resname.IsNullOrEmpty()) return;

            var item = new File
            {
                name = resname,
                dire = System.IO.Path.GetDirectoryName(filepath).PreparePath(true),
                path = filepath.ExcludeFileExtention().PreparePath(false),
            };

            var parts = item.name.Split(new char[] { '_' }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                var id = parts[i].ToInt(-1);
                if (id >= 0)
                    item.ids.Add(id);
                else if (parts.Length > 1)
                    item.tags.Add(parts[i]);
            }

            if (Instance.justFilesWithId)
            {
                if (item.Id == -1) return;
                if (Instance.files.Exists(x => x.name == item.name && x.dire == item.dire && x.path == item.path && x.Id == item.Id)) return;
            }
            else if (Instance.files.Exists(x => x.name == item.name && x.dire == item.dire && x.path == item.path)) return;

            Instance.files.Add(item);
        }

        class OnLocalAssetsChanged : UnityEditor.AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                bool assetCahanged = IsAssetsChanged(importedAssets);
                if (assetCahanged == false)
                    assetCahanged = IsAssetsChanged(deletedAssets);
                if (assetCahanged == false)
                    assetCahanged = IsAssetsChanged(movedAssets);
                if (assetCahanged == false)
                    assetCahanged = IsAssetsChanged(movedFromAssetPaths);
                if (assetCahanged)
                    UpdateFiles();
            }

            private static bool IsAssetsChanged(string[] paths)
            {
                foreach (var path in paths)
                    if (path.Contains("Resources"))
                        return true;
                return false;
            }
        }

        public class ResourceFilePreBuild : UnityEditor.Build.IPreprocessBuildWithReport
        {
            public int callbackOrder => -1000;

            public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
            {
                UpdateFiles();
                UnityEditor.EditorUtility.SetDirty(Instance);
                UnityEditor.AssetDatabase.SaveAssets();
                Debug.Log("Updated files list in ResourceFiles.\nIPreprocessBuildWithReport.callbackOrder: " + callbackOrder);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void EditorRuntimeInitialize()
        {
            UpdateFiles();
        }
#endif
    }
}
