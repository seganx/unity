#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SeganX.Builder
{
    [CreateAssetMenu(menuName = "Builder/Settings")]
    public class Builder : ScriptableObject
    {
        public int version = 1;
        public int bundleVersionCode = 1;
        public Symbols symbols = new Symbols();
        [Header("Options:"), Space()]
        public int buildAndRunIndex = -1;
        public bool stopQueueOnError = true;
        [Header("Build Configurations:"), Space()]
        public List<Build> builds = new List<Build>();


        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static Builder instance = default;
        public static Build CurrentBuilding = null;

        public static Builder Instance
        {
            get
            {
                if (instance == null)
                    CreateMe(CreateInstance<Builder>(), "Builder");
                if (instance == null)
                    instance = AssetDatabase.LoadAssetAtPath<Builder>("Assets/Editor/Builder.asset");
                return instance;
            }
        }

        [MenuItem("SeganX/Build", priority = 50)]
        private static void SelectMe()
        {
            Selection.activeObject = Instance;
        }

        protected static void CreateMe(Object instance, string name)
        {
            var path = "/Editor/";
            var appath = Application.dataPath + path;
            var fileName = path + name + ".asset";
            if (System.IO.File.Exists(Application.dataPath + fileName)) return;
            if (System.IO.Directory.Exists(appath) == false) System.IO.Directory.CreateDirectory(appath);
            AssetDatabase.CreateAsset(instance, "Assets" + fileName);
            AssetDatabase.SaveAssets();
        }

        //////////////////////////////////////////////////////
        /// HELPER CLASSES
        //////////////////////////////////////////////////////
        [System.Serializable]
        public class Symbols
        {
            public bool cameraFX = false;
            public bool onlineSystem = false;
            public bool purchaseSystem = false;
            public bool zipCompression = false;
            public bool playServices = false;
            public List<string> additionals = new List<string>();

            public string Get()
            {
                var result = new List<string>(8);
                if (cameraFX) result.Add("SX_CAMFX");
                if (onlineSystem) result.Add("SX_ONLINE");
                if (purchaseSystem) result.Add("SX_IAP");
                if (zipCompression) result.Add("SX_ZIP");
                if (playServices) result.Add("PLAY_SERVICES");
                result.AddRange(additionals);
                return string.Join(";", result);
            }
        }
    }
}
#endif
