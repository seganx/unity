using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SeganX.Builder
{
    [CustomEditor(typeof(Builder))]
    public class BuilderEditor : Editor
    {
        private void OnEnable()
        {
            versionProp = serializedObject.FindProperty("version");
            versionCodeProp = serializedObject.FindProperty("bundleVersionCode");
            buildAndRunIndexProp = serializedObject.FindProperty("buildAndRunIndex");
            stopQueueOnErrorProp = serializedObject.FindProperty("stopQueueOnError");
            symbolsProp = serializedObject.FindProperty("symbols");
            buildsProp = serializedObject.FindProperty("builds");
        }

        public override void OnInspectorGUI()
        {
            var builder = target as Builder;
            serializedObject.Update();

            EditorGUILayout.LabelField("Versions:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(versionProp, new GUIContent("Config Version"));
            EditorGUILayout.PropertyField(versionCodeProp);
            EditorGUILayout.PropertyField(symbolsProp);
            if (symbolsProp.isExpanded && GUILayout.Button("Update Symbols"))
            {
                PerformSymbols(builder);
            }
            EditorGUILayout.Space(30);


            var buttonHeight = GUILayout.Height(EditorGUIUtility.singleLineHeight * 2);

            // verify files
            if (isBuilding)
            {
                EditorGUILayout.HelpBox("Do not use unlock button!\nUse it just in emergency!", MessageType.Warning);
                if (GUILayout.Button("Unlock", buttonHeight))
                {
                    isBuilding = false;
                    EditorApplication.UnlockReloadAssemblies();
                }
            }
            else
            {
                string error = GetError(builder);
                if (string.IsNullOrEmpty(error))
                {
                    if (GUILayout.Button("Build", buttonHeight))
                    {
                        StartBuilds(builder);
                    }

                    // prepare build configs
                    for (int i = 0; i < builder.builds.Count; i++)
                    {
                        var build = builder.builds[i];
                        build.title = i + ":" + (build.IsActive ? "✓ " : "x ") + build.Config.FileName;

                        if (build.IsActive)
                            build.Config.OnInspectorGUI();
                    }
                }
                else EditorGUILayout.HelpBox(error, MessageType.Error);
            }

            EditorGUILayout.PropertyField(buildAndRunIndexProp);
            EditorGUILayout.PropertyField(stopQueueOnErrorProp);
            EditorGUILayout.Space(30);
            EditorGUILayout.PropertyField(buildsProp);

            serializedObject.ApplyModifiedProperties();
        }


        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        public static bool isBuilding = false;
        static private SerializedProperty versionProp = null;
        static private SerializedProperty versionCodeProp = null;
        static private SerializedProperty buildAndRunIndexProp = null;
        static private SerializedProperty stopQueueOnErrorProp = null;
        static private SerializedProperty symbolsProp = null;
        static private SerializedProperty buildsProp = null;

        static private readonly List<BuildTargetGroup> platforms = new List<BuildTargetGroup>() {
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS,
            BuildTargetGroup.WebGL,
            BuildTargetGroup.Standalone
        };

        private static void PerformSymbols(Builder builder)
        {
            var currentSymbols = builder.symbols.Get();
            foreach (var platform in platforms)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, currentSymbols);
        }

        private static string GetError(Builder builder)
        {
            if (isBuilding) return null;

            foreach (var build in builder.builds)
            {
                if (build.Config == null) return "File refrence(s) missing! Please check file refrences!";
                if (build.IsActive == false) continue;

                var error = build.Config.GetError();
                if (string.IsNullOrEmpty(error) == false) return error;
            }

            return null;
        }

        private static async void StartBuilds(Builder builder)
        {
            if (isBuilding || string.IsNullOrEmpty(GetError(builder)) == false) return;

            var path = Directory.GetParent(Application.dataPath).Parent.FullName + "/Bin/";
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            var folder = EditorUtility.SaveFolderPanel("Select Destination Folder", path, null);
            if (string.IsNullOrEmpty(folder)) return;

            PlayerSettings.SplashScreen.showUnityLogo = false;

            isBuilding = true;
            EditorApplication.LockReloadAssemblies();
            PerformSymbols(builder);

            for (int index = 0; index < builder.builds.Count; index++)
            {
                Builder.CurrentBuilding = builder.builds[index];
                if (Builder.CurrentBuilding.IsActive == false) continue;

                Debug.Log("Build " + (index + 1) + " of " + builder.builds.Count + " started...");
                await Task.Delay(10);

                bool buildResult = await Builder.CurrentBuilding.Config.Build(
                    Builder.CurrentBuilding.Config.PerformSymbols(builder.symbols.Get()),
                    Path.Combine(folder, Builder.CurrentBuilding.Config.FileName),
                    index == builder.buildAndRunIndex);

                await Task.Delay(10);
                Debug.Log("Build " + (index + 1) + " of " + builder.builds.Count + " finished.");

                if (builder.stopQueueOnError && buildResult == false)
                    break;
                else
                    await BuildConfigBase.WaitForEditor();
            }

            Debug.Log("Finished building process.");
            PerformSymbols(builder);
            EditorApplication.UnlockReloadAssemblies();
            isBuilding = false;
        }
    }
}