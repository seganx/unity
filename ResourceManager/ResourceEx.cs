using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public interface IResource
    {
        int Id { get; set; }
    }

    public class ResourceEx : StaticConfig<ResourceEx>
    {
        [System.Serializable]
        public class File
        {
            public string path = string.Empty;
            public string dire = string.Empty;
            public string name = string.Empty;
            public int id = 0;
            public List<string> tags = new List<string>();
        }

        public bool justFilesWithId = true;
        public List<File> files = new List<File>();

        protected override void OnInitialize()
        {

        }

        ///////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ///////////////////////////////////////////////////////////////
        private static char[] splitIncludes = { ' ' };

        public static T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        public static T Load<T>(string dire, int id) where T : Object
        {
            var file = Instance.files.Find(x => x.dire.Contains(dire) && x.id == id);
            return file != null ? Resources.Load<T>(file.path) : default(T);
        }

        public static List<File> LoadAll(string dire, string includes)
        {
            var arr = includes.Split(splitIncludes, System.StringSplitOptions.RemoveEmptyEntries);
            return Instance.files.FindAll(x => x.dire.Contains(dire) && x.tags.Exists(y => arr.Contains(y)));
        }

        public static List<File> LoadAll(string dire, bool subfolders)
        {
            return subfolders ? Instance.files.FindAll(x => x.dire.Contains(dire)) : Instance.files.FindAll(x => x.dire == dire);
        }

        public static List<T> LoadAll<T>(string dire, bool subfolders) where T : Object
        {
            var res = new List<T>();
            var files = subfolders ? Instance.files.FindAll(x => x.dire.Contains(dire)) : Instance.files.FindAll(x => x.dire == dire);
            if (files.Count == 0) return res;

            files.Sort((x, y) => x.id - y.id);
            foreach (var item in files)
            {
                var loaded = Resources.Load<T>(item.path);
                if (loaded != null && res.Contains(loaded) == false)
                    res.Add(loaded);
            }

            return res;
        }

        public static List<T> LoadAll<T>(string dire, string includes) where T : Object
        {
            var arr = includes.Split(splitIncludes, System.StringSplitOptions.RemoveEmptyEntries);
            var res = new List<T>();
            var files = Instance.files.FindAll(x => x.dire.Contains(dire) && x.tags.Exists(y => arr.Contains(y)));
            if (files.Count == 0) return res;

            files.Sort((x, y) => x.id - y.id);
            foreach (var item in files)
            {
                var loaded = Resources.Load<T>(item.path);
                if (loaded != null) res.Add(loaded);
            }

            return res;
        }


        public static List<T> LoadAllWithId<T>(string dire, bool subfolders) where T : Object, IResource
        {
            var res = new List<T>();
            var files = subfolders ? Instance.files.FindAll(x => x.dire.Contains(dire)) : Instance.files.FindAll(x => x.dire == dire);
            if (files.Count == 0) return res;

            files.Sort((x, y) => x.id - y.id);
            foreach (var item in files)
            {
                var loaded = Resources.Load<T>(item.path);
                if (loaded != null && res.Contains(loaded) == false && loaded is IResource)
                {
                    loaded.As<IResource>().Id = item.id;
                    res.Add(loaded);
                }
            }

            return res;
        }

        public static List<T> LoadAllWithId<T>(string dire, string includes) where T : Object, IResource
        {
            var arr = includes.Split(splitIncludes, System.StringSplitOptions.RemoveEmptyEntries);
            var res = new List<T>();
            var files = Instance.files.FindAll(x => x.dire.Contains(dire) && x.tags.Exists(y => arr.Contains(y)));
            if (files.Count == 0) return res;

            files.Sort((x, y) => x.id - y.id);
            foreach (var item in files)
            {
                var loaded = Resources.Load<T>(item.path);
                if (loaded != null && loaded is IResource)
                {
                    loaded.As<IResource>().Id = item.id;
                    res.Add(loaded);
                }
            }

            return res;
        }


        ////////////////////////////////////////////////////////////
        /// STATIC HELPER FUNCTIONS
        ////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private static void AddFile(string filepath)
        {
            var resname = System.IO.Path.GetFileNameWithoutExtension(filepath);
            if (resname.IsNullOrEmpty()) return;
            char[] splitTags = { '_' };
            var item = new File();
            item.name = resname;
            item.dire = System.IO.Path.GetDirectoryName(filepath) + "/";
            item.path = filepath.ExcludeFileExtention();
            item.tags.AddRange(item.name.Split(splitTags, System.StringSplitOptions.RemoveEmptyEntries));
            item.id = item.tags.Count > 0 ? item.tags[0].ToInt(-1) : -1;
            if (Instance.justFilesWithId && item.id == -1) return;
            if (Instance.files.Exists(x => x.name == item.name && x.dire == item.dire && x.path == item.path && x.id == item.id)) return;
            Instance.files.Add(item);
        }

        private static void AddFilesToList(List<string> list, string path)
        {
            if (System.IO.Directory.Exists(path) == false) return;

            var files = System.IO.Directory.GetFiles(path);
            foreach (var item in files)
            {
                if (System.IO.Path.GetExtension(item) == ".meta") continue;
                list.Add(System.IO.Path.GetFullPath(item).MakeRelative(Application.dataPath + "/Resources/"));
            }

            var dirs = System.IO.Directory.GetDirectories(path);
            foreach (var item in dirs)
                AddFilesToList(list, item);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeMethodLoad()
        {
            Instance.files.Clear();

            var list = new List<string>(10);
            AddFilesToList(list, Application.dataPath + "/Resources/");
            foreach (var item in list)
                AddFile(item);

            UnityEditor.EditorUtility.SetDirty(Instance);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}