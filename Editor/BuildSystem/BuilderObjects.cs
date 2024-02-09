#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Linq;
using UnityEditor.Build.Reporting;

namespace SeganX.Builder
{
    public enum Market : int { Bazaar = 1, Myket = 2, GooglePlay = 3, Huawei = 4, Galaxy = 5, Tutuapp = 6, Aptoide = 7, Emay = 8, OneStore = 9 }
    public enum Architecture : int { ARMV7 = 1, ARM64 = 2, ARMV7_ARM64 = 3 }

    [System.Serializable]
    public class Build
    {
        [HideInInspector] public string title = string.Empty;
        [SerializeField] private bool activated = false;
        [SerializeField] private BuildConfigBase config = null;

        public bool IsActive => activated;
        public BuildConfigBase Config => config;
    }

    public abstract class BuildConfigBase : ScriptableObject
    {
        public int versionOffset = 0;
        public string postfix = string.Empty;
        public string packageName = string.Empty;
        public string productName = string.Empty;
        public Builder.Symbols addSymbols = new Builder.Symbols();
        public List<string> removeSymbols = new List<string>();
        public List<string> externalAssets = new List<string>();
        public abstract string FileName { get; }
        public virtual int BundleVersion => Builder.Instance.bundleVersionCode + versionOffset;
        public abstract Task<bool> Build(string symbols, string locationPathName, bool autoRun);
        public virtual void OnInspectorGUI() { }

        public virtual string GetError()
        {
            if (BundleVersion < 1)
                return $"The current bundle version for {name} is {BundleVersion}!\n\nBundle version must be greater that 0. Please check bundle version code and/or version offset in build configs!";
            return null;
        }

        public virtual string PerformSymbols(string initialSymbols)
        {
            var symbols = RemoveSymbols(initialSymbols, "GOOGLE;BAZAAR;MYKET;PLAY_INSTANT;HUAWEI;GALAXY;TUTUAPP;APTOIDE;EMAY;ONESTORE");
            foreach (var symbol in removeSymbols)
                symbols = RemoveSymbols(symbols, symbol);
            symbols = AddSymbols(symbols, addSymbols.Get());
            return symbols;
        }

        protected virtual void OnValidate()
        {
            for (int i = 0; i < externalAssets.Count; i++)
            {
                var path = externalAssets[i];
                if (path.IsNullOrEmpty()) continue;
                var fullPath = System.IO.Path.GetFullPath(externalAssets[i]);
                var relativePath = fullPath.MakeRelative(System.IO.Path.Combine(Application.dataPath, ".."));
                externalAssets[i] = relativePath;
            }
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        public static async Task WaitForEditor()
        {
            await Task.Delay(100);
            AssetDatabase.Refresh();
            await Task.Delay(100);
            while (EditorApplication.isUpdating)
                await Task.Delay(100);
        }

        public static string RemoveSymbols(string current, string symbols)
        {
            if (string.IsNullOrEmpty(symbols)) return current;

            var cursymbols = new List<string>(current.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
            var newsymbols = new List<string>(symbols.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));

            //  remove duplicated symbols
            foreach (var item in newsymbols)
                cursymbols.RemoveAll(x => x == item);

            cursymbols.Sort();
            return string.Join(";", cursymbols.ToArray());
        }

        public static string AddSymbols(string current, string symbols)
        {
            if (string.IsNullOrEmpty(symbols)) return current;

            var cursymbols = new List<string>(current.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
            var newsymbols = new List<string>(symbols.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));

            //  remove duplicated symbols
            foreach (var item in newsymbols)
                cursymbols.RemoveAll(x => x == item);

            //  add new symbols
            foreach (var item in newsymbols)
                cursymbols.Add(item);

            cursymbols.Sort();
            return string.Join(";", cursymbols.ToArray());
        }

        public static void UpdateFileInfos(List<FileInfo> files)
        {
            foreach (var item in files)
            {
                switch (item.type)
                {
                    case FileType.DisableFile: item.title = (item.destFile == null) ? "WARNING: No file!" : "Disbale : " + item.destFile.name; break;
                    case FileType.ReplaceText: item.title = item.destFile == null ? "WARNING: No file!" : item.destFile.name + " : " + item.what + " = " + item.with.Replace("\n", string.Empty); break;
                    case FileType.ReplaceFile:
                        {
                            item.title = item.destFile == null ? "WARNING: No file!" : "Replace File : " + (item.sourceFile == null ? "WARNING: No source file!" : item.destFile.name + " = " + item.sourceFile.name);
                            if (item.sourceFile != null) item.Sourcename = AssetDatabase.GetAssetPath(item.sourceFile);
                        }
                        break;
                }
                if (item.destFile != null) item.Filename = AssetDatabase.GetAssetPath(item.destFile);
            }
        }

        public static string GetFilesError(List<FileInfo> files)
        {
            for (int i = 0; i < files.Count; i++)
            {
                var item = files[i];
                switch (item.type)
                {
                    case FileType.DisableFile:
                    case FileType.ReplaceText:
                        {
                            if (item.destFile == null)
                                return $"File [{i}]: Missing File reference! Please check file refrences!";
                        }
                        break;

                    case FileType.ReplaceFile:
                        {
                            if (item.destFile == null || item.destFile == null)
                                return $"File [{i}]: Missing File reference! Please check file refrences!";
                        }
                        break;
                }
            }
            return null;
        }

        public static bool BackupFiles(List<FileInfo> files)
        {
            foreach (var item in files)
            {
                if (item.type == FileType.DisableFile)
                {
                    if (BuilderFileHandler.Bakcup.DisableFileOrDir(item.Filename) == false)
                        return false;
                }
                else
                {
                    if (BuilderFileHandler.Bakcup.BackupFile(item.Filename) == false)
                        return false;
                }
            }
            return true;
        }

        public static bool PerformReplaces(List<FileInfo> files)
        {
            foreach (var item in files)
            {
                switch (item.type)
                {
                    case FileType.ReplaceText:
                        if (BuilderFileHandler.ReplaceFileText(item.Filename, item.what, item.with) == false)
                            return false;
                        break;
                    case FileType.ReplaceFile:
                        if (BuilderFileHandler.ReplaceWholeFile(item.Sourcename, item.Filename) == false)
                            return false;
                        break;
                    case FileType.DisableFile: break;
                }
            }
            return true;
        }

        public static bool BringExternals(List<string> externalAssets)
        {
            foreach (var item in externalAssets)
            {
                if (BuilderFileHandler.Externals.Bring(item) == false)
                    return false;
            }
            return true;
        }

        public static bool BuildPlayer(BuildTarget target, string locationPathName, bool autoRun)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray(),
                target = target,
                options = BuildOptions.CompressWithLz4HC | BuildOptions.ShowBuiltPlayer,
                locationPathName = locationPathName
            };

            if (autoRun)
                buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;

            try
            {
                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                BuildSummary summary = report.summary;

                switch (summary.result)
                {
                    case BuildResult.Succeeded:
                        Debug.Log("Build succeeded in " + (int)summary.totalTime.TotalSeconds + " seconds with " + summary.totalSize + " bytes");
                        return true;
                    case BuildResult.Failed:
                        Debug.Log("Build failed!");
                        return false;
                    case BuildResult.Cancelled:
                        Debug.Log("Build cancelled!");
                        return false;
                    default:
                        Debug.Log("Build result is unknown!");
                        return false;
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("Build failed: " + e.Message);
                return false;
            }
        }
    }
}
#endif