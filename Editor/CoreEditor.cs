using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeganX
{
    [CustomEditor(typeof(Core))]
    public class CoreEditor : Editor
    {
        private class Item
        {
            public string name = string.Empty;
            public string symbol = string.Empty;
            public bool value = false;

            public Item(string name, string symbol) { this.name = name; this.symbol = symbol; }
            public override string ToString() { return name + " " + symbol + " " + value; }
        }

        private readonly List<Item> items = new List<Item>() {
            new Item("Camera FX", "SX_CAMFX"),
            new Item("Online System", "SX_ONLINE"),
            new Item("Purchase System", "SX_IAP"),
            new Item("Zip Compression", "SX_ZIP"),
        };

        private readonly List<BuildTargetGroup> platforms = new List<BuildTargetGroup>() {
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS,
            BuildTargetGroup.WebGL,
            BuildTargetGroup.Standalone
        };

        public override void OnInspectorGUI()
        {
            var core = target as Core;

            base.OnInspectorGUI();
            EditorGUILayout.Space(30);
            
            //var coreSymbols = Core.GetCoreSymbols();
            //if (coreSymbols.HasContent())
            //    foreach (var platform in platforms)
            //        PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, coreSymbols);
        }

        private bool HasSymbol(string current, string symbol)
        {
            var cursymbols = new List<string>(current.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
            return cursymbols.Contains(symbol);
        }

        private string AddRemoveSymbol(string current, string symbol, bool addsymbols)
        {
            if (string.IsNullOrEmpty(symbol)) return current;

            var cursymbols = new List<string>(current.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
            var newsymbols = new List<string>(symbol.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));

            //  remove duplicated symbols
            foreach (var item in newsymbols)
                cursymbols.RemoveAll(x => x == item);

            //  add new symbols
            if (addsymbols)
                foreach (var item in newsymbols)
                    cursymbols.Add(item);

            cursymbols.Sort();
            return string.Join(";", cursymbols.ToArray());
        }

        [MenuItem("SeganX/Settings", priority = 100)]
        private static void Settings()
        {
            Selection.activeObject = Core.Instance;
        }

        static class SeganXSettings
        {
            private static Editor editor = null;

            [SettingsProvider]
            public static SettingsProvider CreateSettings()
            {
                // First parameter is the path in the Settings window.
                // Second parameter is the scope of this setting: it only appears in the Project Settings window.
                var provider = new SettingsProvider("Project/SeganX", SettingsScope.Project)
                {
                    // By default the last token of the path is used as display name if no label is provided.
                    label = "SeganX",

                    // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                    guiHandler = (searchContext) =>
                    {
                        if (editor == null)
                            editor = CreateEditor(Core.Instance, typeof(CoreEditor));
                        editor.OnInspectorGUI();
                    },

                    // Populate the search keywords to enable smart search filtering and label highlighting:
                    keywords = new HashSet<string>(new[] { "SeganX", "Settings", "Core" })
                };

                return provider;
            }
        }
    }
}