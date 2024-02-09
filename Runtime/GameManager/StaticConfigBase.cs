using UnityEngine;

namespace SeganX
{
    public abstract class StaticConfigBase : ScriptableObject
    {
        public int version = 1;

        protected virtual void OnInitialize() { }



        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
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