using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeganX
{
    [CustomEditor(typeof(LocalizationKit))]
    public class LocalizationKitEditor : Editor
    {
        static string filter = string.Empty;

        static GUIStyle style = null;

        public static GUIStyle labelStyle
        {
            get
            {
                if (style == null)
                    style = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.UpperRight,
                        wordWrap = true,
                        border = new RectOffset(10, 10, 0, 0),
                        padding = new RectOffset(10, 15, 0, 0)
                    };
                return style;
            }
        }

        public override void OnInspectorGUI()
        {
            var local = target as LocalizationKit;
            DrawKitItems(local);
            EditorUtility.SetDirty(local);
        }

        public static void DrawKitItems(LocalizationKit localKit)
        {
            localKit.kit.language = EditorGUILayout.TextField("Language:", localKit.kit.language);
            EditorGUILayout.Separator();

            if (GUILayout.Button("Add new string"))
                filter = localKit.AddString(null).ToString();
            if (GUILayout.Button("Export"))
            {
                var filename = EditorUtility.SaveFilePanel("Save exported data", System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(localKit)), localKit.name, "json");
                if (filename.HasContent(4))
                {
                    System.IO.File.WriteAllText(filename, JsonUtility.ToJson(localKit.kit, true), System.Text.Encoding.UTF8);
                }
            }
            if (GUILayout.Button("Import"))
            {
                var filename = EditorUtility.OpenFilePanelWithFilters("Open json to import", System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(localKit)), new string[] { "Json File", "json" });
                if (filename.HasContent(4))
                {
                    localKit.kit = JsonUtility.FromJson<LocalizationKit.LocalKitData>(System.IO.File.ReadAllText(filename));
                }
            }

            EditorGUIUtility.labelWidth = 60;
            EditorGUILayout.LabelField("Number of strings: " + localKit.kit.strings.Count);

            filter = EditorGUILayout.TextField("Search:", filter);
            int filterid = filter.ToInt();
            int filterRange = (filterid > 0 ? Mathf.Pow(10, 6 - filter.Length).ToInt() : 0);
            filterid *= Mathf.Pow(10, 6 - filter.Length).ToInt();


            EditorGUILayout.Separator();
            if (filterid > 0 && filterRange < 2)
            {
                var item = localKit.kit.strings.Find(x => x.i == filterid);
                if (item != null)
                {
                    item.s = EditorGUILayout.TextArea(item.s, GUILayout.MinHeight(60)).CleanFromCode().CleanForPersian();
                    var persianStr = PersianTextShaper.PersianTextShaper.ShapeText(item.s.CleanFromCode().CleanForPersian());
                    EditorGUILayout.LabelField(persianStr, labelStyle);
                }
            }
            else
            {
                foreach (var item in localKit.kit.strings)
                {
                    if (filterid > 0)
                    {
                        if (filterRange > 1)
                        {
                            if (!item.i.Between(filterid - filterRange, filterid + filterRange)) continue;
                        }
                        else
                        {
                            if (item.i != filterid) continue;
                        }
                    }
                    else if (filter.HasContent())
                    {
                        if (!item.s.Contains(filter)) continue;
                    }

                    if (item.s.IsRtl())
                    {
                        var persianStr = PersianTextShaper.PersianTextShaper.ShapeText(item.s.CleanFromCode().CleanForPersian());
                        EditorGUILayout.LabelField(item.i + ":", persianStr);
                    }
                    else EditorGUILayout.LabelField(item.i + ":", item.s);
                    EditorGUILayout.Separator();
                }
            }
        }
    }
}

