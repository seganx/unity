using UnityEngine;

namespace SeganX
{
    public abstract class StaticConfig<T> : StaticConfig<T, T> where T : StaticConfig<T, T> { }

    public abstract class StaticConfig<T, FIELNAME> : StaticConfigBase where T : StaticConfig<T, FIELNAME>
    {
        private static T instance = default;

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null) CreateMe(CreateInstance<T>(), typeof(FIELNAME).Name);
#endif
                if (instance == null)
                {
                    instance = Resources.Load<T>("Configs/" + typeof(FIELNAME).Name);
                    instance.OnInitialize();
                }
                return instance;
            }
        }


        protected static void SaveData<D>(D data)
        {
            PlayerPrefsEx.SetObject(Instance.name + instance.version, data);
        }

        protected static D LoadData<D>(D defauleObj)
        {
            return PlayerPrefsEx.GetObject(Instance.name + instance.version, defauleObj);
        }

#if UNITY_EDITOR
        protected static string ConfigFilePath
        {
            get
            {
                var res = System.IO.Directory.GetParent(Application.dataPath).Parent.FullName + "/Configs/" + Instance.version;
                if (System.IO.Directory.Exists(res) == false) System.IO.Directory.CreateDirectory(res);
                return res;
            }
        }

        protected static void ExportAsJson(object obj, string postfix, bool prettyPrint = false)
        {
            var filename = UnityEditor.EditorUtility.SaveFilePanel("Save exported data", ConfigFilePath, "foundation" + postfix, "txt");
            if (filename.HasContent(4))
                System.IO.File.WriteAllText(filename, JsonUtility.ToJson(obj, prettyPrint), System.Text.Encoding.UTF8);
        }

        protected static string ImportFromJson()
        {
            var filename = UnityEditor.EditorUtility.OpenFilePanel("Import data from file", ConfigFilePath, "txt");
            if (filename.HasContent(4) == false) return default;
            return System.IO.File.ReadAllText(filename);
        }

        public static C ImportFromJson<C>()
        {
            var json = ImportFromJson();
            if (json.HasContent(5) == false) return default;
            return JsonUtility.FromJson<C>(json);
        }
#endif
    }
}
