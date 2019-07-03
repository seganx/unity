using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Effects
{
    public class CameraFX_Luts : ScriptableObject
    {
        [SerializeField] public Texture[] lutTextures = null;

        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        private static CameraFX_Luts instance = null;

        private static CameraFX_Luts Instance
        {
            get
            {
#if UNITY_EDITOR
                //if (instance == null) CreateMe();
#endif
                if (instance == null)
                    instance = Resources.Load<CameraFX_Luts>(typeof(CameraFX_Luts).Name);
                return instance;
            }
        }

#if UNITY_EDITOR
        private static void CreateMe()
        {
            var path = "/Resources/";
            var appath = Application.dataPath + path;
            var fileName = path + typeof(CameraFX_Luts).Name + ".asset";
            if (System.IO.File.Exists(Application.dataPath + fileName)) return;
            if (!System.IO.Directory.Exists(appath)) System.IO.Directory.CreateDirectory(appath);
            UnityEditor.AssetDatabase.CreateAsset(CreateInstance<CameraFX_Luts>(), "Assets" + fileName);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif

        public static int Count { get { return Instance.lutTextures.Length; } }

        public static Texture Get(int index)
        {
            return Count > 0 ? Instance.lutTextures[index % Count] : null;
        }
    }
}
