using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public abstract class StaticConfig<T> : ScriptableObject where T : StaticConfig<T>
    {
        protected abstract void OnInitialize();
       

        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        private static T instance = default(T);

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null) CreateMe();
#endif
                if (instance == null)
                {
                    instance = Resources.Load<T>("Configs/" + typeof(T).Name);
                    instance.OnInitialize();
                }
                return instance;
            }
        }

#if UNITY_EDITOR
        private static void CreateMe()
        {
            var path = "/Resources/Configs/";
            var appath = Application.dataPath + path;
            var fileName = path + typeof(T).Name + ".asset";
            if (System.IO.File.Exists(Application.dataPath + fileName)) return;
            if (!System.IO.Directory.Exists(appath)) System.IO.Directory.CreateDirectory(appath);
            UnityEditor.AssetDatabase.CreateAsset(CreateInstance<T>(), "Assets" + fileName);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif

        protected static void SaveData(object data)
        {
            var json = JsonUtility.ToJson(data);
            PlayerPrefsEx.SetString(Instance.name, json);
        }

        protected static D LoadData<D>(D defauleObj)
        {
            var json = PlayerPrefsEx.GetString(Instance.name, string.Empty);
            if (json.Length > 5)
                return JsonUtility.FromJson<D>(json);
            else
                return defauleObj;
        }
    }
}
