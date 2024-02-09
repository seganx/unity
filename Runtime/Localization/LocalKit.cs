using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Localization
{
    public static class LocalKit
    {
        private static readonly Dictionary<string, List<string>> strings = new Dictionary<string, List<string>>(64);

        public static bool IsRtl { get; private set; }
        public static int LanguageIndex { get; private set; }

        private static int SavedLanguageIndex
        {
            get => PlayerPrefs.GetInt("LocalKit.Index", 0);
            set => PlayerPrefs.SetInt("LocalKit.Index", value);
        }

        private static bool SavedLanguageRtl
        {
            get => PlayerPrefs.GetInt("LocalKit.Rtl", 0) > 0;
            set => PlayerPrefs.SetInt("LocalKit.Rtl", value ? 1 : 0);
        }

        public static bool Exist(string key)
        {
            return strings.ContainsKey(key);
        }

        public static string Get(string key)
        {
            if (strings.TryGetValue(key, out List<string> list))
            {
                if (LanguageIndex < list.Count)
                    return list[LanguageIndex];
            }
            return key;
        }

        public static void SetLanguageIndex(int languageIndex, bool rtl)
        {
            SavedLanguageRtl = IsRtl = rtl;
            SavedLanguageIndex = LanguageIndex = languageIndex;
            LocalText.LanguageChanged();
        }

        public static void LoadFromJson(string json)
        {
            var temp = TinyJson.JsonParser.FromJson<Dictionary<string, List<string>>>(json);
            foreach (var item in temp)
                strings[item.Key] = item.Value;
        }

#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void OnRuntimeInitialize()
        {
            strings.Clear();
            IsRtl = SavedLanguageRtl;
            LanguageIndex = SavedLanguageIndex;
            var files = ResourceFiles.FindAll("LocalKit", true);
            foreach (var file in files)
            {
                var asset = Resources.Load<TextAsset>(file.path);
                if (asset != null)
                    LoadFromJson(asset.text);
            }
        }
#else
        static LocalKit()
        {
            IsRtl = SavedLanguageRtl;
            LanguageIndex = SavedLanguageIndex;
            ImportAllTexts();
        }

        public static void ImportAllTexts()
        {
            const string localdir = "Assets/Resources/LocalKit/";
            strings.Clear();

            if (System.IO.Directory.Exists(localdir) == false)
            {
                System.IO.Directory.CreateDirectory(localdir);
                System.IO.File.WriteAllText($"{localdir}0_common.json", "{\n    \"exit_confirm\": [\n        \"Are you sure ?\"\n    ]\n}");
            }
            var files = System.IO.Directory.GetFiles(localdir);
            for (int i = 0; i < files.Length; i++)
            {
                var item = files[i].PreparePath();
                var extension = System.IO.Path.GetExtension(item);
                if (extension != ".json" && extension != ".txt") continue;
                var filename = System.IO.Path.GetFileNameWithoutExtension(item);
                var parts = filename.Split('_');
                if (parts[0].ToInt(-1) == -1) continue;
                var path = item.Remove(0, item.LastIndexOf("/Resources/") + 11).ExcludeFileExtention().PreparePath(false);
                var asset = Resources.Load<TextAsset>(path);
                if (asset != null)
                    LoadFromJson(asset.text);
            }
        }

        private class OnLocalAssetsChanged : UnityEditor.AssetPostprocessor
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
                {
                    ImportAllTexts();
                    LocalText.LanguageChanged();
                }
            }

            private static bool IsAssetsChanged(string[] paths)
            {
                foreach (var path in paths)
                    if (path.Contains("Assets/Resources/LocalKit/"))
                        return true;
                return false;
            }
        }
#endif
    }
}