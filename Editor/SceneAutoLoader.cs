#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SeganX
{

    /// <summary>
    /// Scene auto loader.
    /// </summary>
    /// <description>
    /// This class adds a Tools > Scene Autoload menu containing options to select
    /// a "master scene" enable it to be auto-loaded when the user presses play
    /// in the editor. When enabled, the selected scene will be loaded on play,
    /// then the original scene will be reloaded on stop.
    ///
    /// Based on an idea on this thread:
    /// http://forum.unity3d.com/threads/157502-Executing-first-scene-in-build-settings-when-pressing-play-button-in-editor
    /// </description>
    [InitializeOnLoad]
    static class SceneAutoLoader
    {
        // Static constructor binds a play-mode-changed callback.
        // [InitializeOnLoad] above makes sure this gets executed.
        static SceneAutoLoader()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        // Menu items to select the "master" scene and control whether or not to load it.
        [MenuItem("SeganX/Scene Autoload/Select Master Scene...")]
        private static void SelectMasterScene()
        {
            string masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");
            if (!string.IsNullOrEmpty(masterScene))
            {
                MasterScene = masterScene;
                LoadMasterOnPlay = true;
            }
        }

        [MenuItem("SeganX/Scene Autoload/Load Master On Play", true)]
        private static bool ShowLoadMasterOnPlay()
        {
            return !LoadMasterOnPlay;
        }

        [MenuItem("SeganX/Scene Autoload/Load Master On Play")]
        private static void EnableLoadMasterOnPlay()
        {
            LoadMasterOnPlay = true;
        }

        [MenuItem("SeganX/Scene Autoload/Don't Load Master On Play", true)]
        private static bool ShowDontLoadMasterOnPlay()
        {
            return LoadMasterOnPlay;
        }

        [MenuItem("SeganX/Scene Autoload/Don't Load Master On Play")]
        private static void DisableLoadMasterOnPlay()
        {
            LoadMasterOnPlay = false;
        }

        // Play mode change callback handles the scene load/reload.
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!LoadMasterOnPlay)
            {
                return;
            }

            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // User pressed play -- auto load master scene.
                PreviousScene = EditorSceneManager.GetActiveScene().path;
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    if (!EditorSceneManager.OpenScene(MasterScene, OpenSceneMode.Single).IsValid())
                    {
                        Debug.LogError(string.Format("error: scene not found: {0}", MasterScene));
                        EditorApplication.isPlaying = false;
                    }
                }
                else
                {
                    // User canceled the save operation -- cancel play as well.
                    EditorApplication.isPlaying = false;
                }
            }

            // isPlaying check required because cannot OpenScene while playing
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // User pressed stop -- reload previous scene.
                if (!EditorSceneManager.OpenScene(PreviousScene, OpenSceneMode.Single).IsValid())
                {
                    Debug.LogError(string.Format("error: scene not found: {0}", PreviousScene));
                }
            }
        }

        private static string ProjectName
        {
            get
            {
                string[] s = Application.dataPath.Split('/');
                return s[s.Length - 2];
            }
        }

        private static string EditorPrefLoadMasterOnPlay { get { return ProjectName + ".SceneAutoLoader.LoadMasterOnPlay"; } }
        private static string EditorPrefMasterScene { get { return ProjectName + ".SceneAutoLoader.MasterScene"; } }
        private static string EditorPrefPreviousScene { get { return ProjectName + ".SceneAutoLoader.PreviousScene"; } }

        private static bool LoadMasterOnPlay
        {
            get { return EditorPrefs.GetBool(EditorPrefLoadMasterOnPlay, false); }
            set { EditorPrefs.SetBool(EditorPrefLoadMasterOnPlay, value); }
        }

        private static string MasterScene
        {
            get { return EditorPrefs.GetString(EditorPrefMasterScene, "Assets/Seganx/Core.unity"); }
            set { EditorPrefs.SetString(EditorPrefMasterScene, value); }
        }

        private static string PreviousScene
        {
            get { return EditorPrefs.GetString(EditorPrefPreviousScene, EditorSceneManager.GetActiveScene().name); }
            set { EditorPrefs.SetString(EditorPrefPreviousScene, value); }
        }
    }
}

#endif