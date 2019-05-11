using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public abstract class StaticConfig<T> : ScriptableObject where T : StaticConfig<T>
    {
        protected abstract void OnInitialize();

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
    }
}
