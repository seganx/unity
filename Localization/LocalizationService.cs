using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SeganX
{
    public class LocalizationService : ScriptableObject
    {
        public LocalizationKit currentKit = null;
        public List<LocalizationKit> kits = new List<LocalizationKit>();

        private void OnEnable()
        {
            instance = this;
        }

        private void Awake()
        {
            instance = this;
        }

        private string GetString(int id)
        {
            return currentKit == null ? id.ToString() : currentKit.Get(id);
        }

        private void SetLanguageKit(string language)
        {
            var kit = kits.Find(x => x.kit.language.Contains(language));
            if (kit == null) return;
            currentKit = kit;
            LocalText.LanguageChanged();
        }

        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        private static LocalizationService instance = null;

        public static LocalizationService Instance
        {
            get
            {
#if UNITY_EDITOR
                CheckService();
#endif
                if (instance == null)
                    instance = Resources.Load<LocalizationService>("Localization/LocalizationService");
                return instance;
            }
        }

        public static bool IsEnglish { get { return Instance.currentKit.kit.language.Contains("en"); } }
        public static bool IsPersian { get { return Instance.currentKit.kit.language.Contains("fa"); } }

        public static string Get(int id)
        {
            return Instance.GetString(id);
        }

        public static void SetLanguage(string language)
        {
            Instance.SetLanguageKit(language);
        }

#if UNITY_EDITOR
        public static int UpdateString(int id, string text)
        {
            if (Instance.currentKit)
                return Instance.currentKit.UpdateString(id, text);
            else
                return id;
        }

        public static void CheckService()
        {
            var path = "/Resources/Localization/";
            var fileName = path + "LocalizationService.asset";
            if (File.Exists(Application.dataPath + fileName)) return;

            var ioPath = Application.dataPath + path;
            if (!Directory.Exists(ioPath)) Directory.CreateDirectory(ioPath);

            instance = ScriptableObject.CreateInstance<LocalizationService>();
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets" + fileName);

            instance.currentKit = ScriptableObject.CreateInstance<LocalizationKit>();
            UnityEditor.AssetDatabase.CreateAsset(instance.currentKit, "Assets" + path + "LocKit_fa.asset");

            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}
