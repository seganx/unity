using UnityEngine;

namespace SeganX
{
    public abstract class StaticConfigBase<T, FILENAME> : ScriptableObject where T : StaticConfigBase<T, FILENAME>
    {
        protected virtual void OnInitialize() { }


        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static T instance = default;

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null) CreateMe(CreateInstance<T>(), typeof(FILENAME).Name);
#endif
                if (instance == null)
                {
                    instance = Resources.Load<T>("Configs/" + typeof(FILENAME).Name);
                    instance.OnInitialize();
                }
                return instance;
            }
        }

#if UNITY_EDITOR
        protected static void CreateMe(Object instance, string name)
        {
            var path = "/Resources/Configs/";
            var appath = Application.dataPath + path;
            var fileName = path + name + ".asset";
            if (System.IO.File.Exists(Application.dataPath + fileName)) return;
            if (!System.IO.Directory.Exists(appath)) System.IO.Directory.CreateDirectory(appath);
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets" + fileName);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}