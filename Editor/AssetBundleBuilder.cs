using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SeganX
{
    public class AssetBundleBuilder : StaticConfig<AssetBundleBuilder>
    {
        public class BuildTextureData
        {
            public string path = string.Empty;
            public int maxSize = 256;
        }

        [System.Serializable]
        public class BuildOptions
        {
            public enum TextureResize : int { FullSize = 1, HalfSize = 2, QuarterSize = 4 }
            public bool active = true;
            public bool encrypt = true;
            public BuildTarget platform = BuildTarget.Android;
        }

        public string outputPath = "Excludes/Bundles";
        [EnumFlag]
        public BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.None;
        public List<BuildOptions> builds = new List<BuildOptions>();

        protected override void OnInitialize() { }


        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        [MenuItem("SeganX/AssetBundles/Build Options")]
        public static void SelectBuildConfig()
        {
            Selection.activeObject = Instance;
        }

        [MenuItem("SeganX/AssetBundles/Build from Selection")]
        public static void BuildBundlesFromSelection()
        {
            // get all selected *assets*
            var selectedAssets = Selection.objects.Where(o => !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(o))).ToArray();
            if (selectedAssets.Length < 1) return;

            List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
            HashSet<string> processedBundles = new HashSet<string>();

            // get asset bundle names from selection
            foreach (var serlectedOne in selectedAssets)
            {
                var assetPath = AssetDatabase.GetAssetPath(serlectedOne);
                var importer = AssetImporter.GetAtPath(assetPath);
                if (importer == null) continue;

                // get asset bundle name & variant
                var assetBundleName = importer.assetBundleName;
                var assetBundleVariant = importer.assetBundleVariant;
                var assetBundleFullName = string.IsNullOrEmpty(assetBundleVariant) ? assetBundleName : assetBundleName + "." + assetBundleVariant;

                // gnly process assetBundleFullName once. No need to add it again.
                if (processedBundles.Contains(assetBundleFullName)) continue;
                processedBundles.Add(assetBundleFullName);

                AssetBundleBuild build = new AssetBundleBuild();

                build.assetBundleName = assetBundleName;
                build.assetBundleVariant = assetBundleVariant;
                build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleFullName);

                assetBundleBuilds.Add(build);
            }

            BuildAssetBundlesAllTargets(assetBundleBuilds.ToArray());
        }

        [MenuItem("SeganX/AssetBundles/Build All")]
        public static void BuildAllAssetBundles()
        {
            BuildAssetBundlesAllTargets(null);
        }

        public static void BuildAssetBundlesAllTargets(AssetBundleBuild[] builds)
        {
            if (Directory.Exists(Instance.outputPath) == false)
                Directory.CreateDirectory(Instance.outputPath);

            foreach (var buildOption in Instance.builds)
            {
                if (buildOption.active == false) continue;
                
                string outputFolder = Instance.outputPath;
                switch (buildOption.platform)
                {
                    case BuildTarget.StandaloneWindows: outputFolder = Path.Combine(Instance.outputPath, "Windows"); break;
                    case BuildTarget.iOS: outputFolder = Path.Combine(Instance.outputPath, "iOS"); break;
                    case BuildTarget.Android: outputFolder = Path.Combine(Instance.outputPath, "Android"); break;
                }

                BuildAssetBundles(builds, buildOption.platform, outputFolder, buildOption.encrypt);
            }

            EditorUtility.RevealInFinder(Instance.outputPath);
        }

        public static void BuildAssetBundles(AssetBundleBuild[] builds, BuildTarget target, string outputPath, bool encrypt)
        {
            if (Directory.Exists(outputPath) == false)
                Directory.CreateDirectory(outputPath);

            AssetBundleManifest result;
            if (builds == null || builds.Length == 0)
                result = BuildPipeline.BuildAssetBundles(outputPath, Instance.buildOptions, target);
            else
                result = BuildPipeline.BuildAssetBundles(outputPath, builds, Instance.buildOptions, target);

            // list and encrypt assets if needed
            if (encrypt == false) return;
            var items = result.GetAllAssetBundles();
            foreach (var item in items)
            {
                var path = Path.Combine(outputPath, item);
                try
                {
                    var src = File.ReadAllBytes(path);
                    var data = Core.Encrypt(src);
                    File.WriteAllBytes(path + ".seganx", data);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        public static string VerifyDirectory(string outputPath, string folder)
        {
            string outputFolder = Path.Combine(outputPath, folder);
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);
            return outputFolder;
        }

#if OFF
        public static void VerifyTexturesInBuilds(AssetBundleBuild[] builds, int sizefactor, System.Action callback)
        {
            if (sizefactor == 1)
            {
                callback();
                return;
            }

            var textureList = new List<BuildTextureData>();

            //  collect all textures in dependencies
            foreach (var build in builds)
                foreach (var asset in build.assetNames)
                    foreach (var item in AssetDatabase.GetDependencies(asset))
                        if (AssetImporter.GetAtPath(item) is TextureImporter)
                            textureList.Add(new BuildTextureData() { path = item });

            VerifyTexturesSize(textureList, sizefactor, callback);
        }

        public static void VerifyTexturesInPath(string path, int sizefactor, System.Action callback)
        {
            if (sizefactor == 1)
            {
                callback();
                return;
            }

            var textureList = new List<BuildTextureData>();
            var guids = AssetDatabase.FindAssets("t:texture2D", new string[] { path });
            foreach (var guid in guids)
                textureList.Add(new BuildTextureData() { path = AssetDatabase.GUIDToAssetPath(guid) });

            VerifyTexturesSize(textureList, sizefactor, callback);
        }

        public static void VerifyTexturesSize(List<BuildTextureData> textureList, int sizefactor, System.Action callback)
        {
            //  verify texture size
            foreach (var texture in textureList)
            {
                var importer = TextureImporter.GetAtPath(texture.path) as TextureImporter;
                texture.maxSize = importer.maxTextureSize;
                importer.maxTextureSize /= sizefactor;
                AssetDatabase.ImportAsset(texture.path, ImportAssetOptions.ForceUpdate);
            }

            callback();

            foreach (var texture in textureList)
            {
                var importer = TextureImporter.GetAtPath(texture.path) as TextureImporter;
                importer.maxTextureSize = texture.maxSize;
                AssetDatabase.ImportAsset(texture.path, ImportAssetOptions.ForceUpdate);
            }
        }
#endif
    }
}
