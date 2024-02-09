#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;

namespace SeganX.Builder
{
    [CreateAssetMenu(menuName = "Builder/Android/Standard Offline")]
    public class BuildConfigAndroid : BuildConfigBase
    {
        public bool buildAppBundle = false;
        public bool proguardMinification = false;
        public Market market = Market.Bazaar;
        public Architecture architecture = Architecture.ARMV7;
        [Space(50), InspectorButton("Open KeyStore File", "OpenKeyStoreFile", false)]
        [Delayed] public string storeFilename = "../user.keystore";
        [Delayed] public string storePassword = string.Empty;
        [Delayed] public string storeAliseName = "release";
        [Delayed] public string storeAlisePassword = string.Empty;
        [Space(50), InspectorButton(100, "Copy", "CopyFiles", "Paste", "PasteFiles", true)]
        public bool buttons = false;
        public List<FileInfo> files = new List<FileInfo>();

        public override string FileName
        {
            get
            {
                var currentGameName = Core.GetGameName(packageName.HasContent(3) ? packageName : Application.identifier, productName.HasContent(3) ? productName : Application.productName);
                var filename = $"{currentGameName}_{Builder.Instance.version}.{BundleVersion}_{market.ToString().ToLower()}";
                switch (architecture)
                {
                    case Architecture.ARMV7: filename += "_v7"; break;
                    case Architecture.ARM64: filename += "_v8"; break;
                    case Architecture.ARMV7_ARM64: filename += "_v7v8"; break;
                }
                filename += postfix;
                filename += (buildAppBundle ? ".aab" : ".apk");
                return filename;
            }
        }

        public override string GetError()
        {
            if (System.IO.File.Exists(storeFilename) == false)
            {
                return $"{name}: Can't find Keystore file! Please check the Store Filename field!";
            }

            if (storeAliseName.HasContent(3) == false ||
                storeAlisePassword.HasContent(6) == false ||
                storePassword.HasContent(6) == false)
                return $"{name}: Keystore is not valid! Please check the Keystore fields";

            int targetSdkVersion = (int)PlayerSettings.Android.targetSdkVersion;
            if (targetSdkVersion < 30)
                return $"{name}: Target SDK version can't be less that 30 but current is {targetSdkVersion}";

            int minSdkVersion = (int)PlayerSettings.Android.minSdkVersion;
            int sdkDelta = targetSdkVersion - minSdkVersion;
            if (sdkDelta < 5)
                return $"{name}: Target[{targetSdkVersion}] & Minimum[{minSdkVersion}] API versions are too close[{sdkDelta}] and cause rediuce target devices!";

            var err = base.GetError();
            if (err.HasContent()) return err;

            err = GetFilesError(files);
            if (err.HasContent()) return $"File Check Rrror!\n\nConfig name: {name}\n{err}!";

            return null;
        }

        public override async Task<bool> Build(string symbols, string locationPathName, bool autoRun)
        {
            bool result = false;

            var initialProductName = PlayerSettings.productName;
            var initialPackageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            var initialArchitectures = PlayerSettings.Android.targetArchitectures;

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = string.IsNullOrEmpty(storeFilename) ? PlayerSettings.Android.keystoreName : storeFilename;
            PlayerSettings.Android.keyaliasName = string.IsNullOrEmpty(storeAliseName) ? PlayerSettings.Android.keyaliasName : storeAliseName;
            PlayerSettings.Android.keystorePass = storePassword;
            PlayerSettings.Android.keyaliasPass = storeAlisePassword;

            PlayerSettings.Android.bundleVersionCode = BundleVersion;
            PlayerSettings.bundleVersion = Builder.Instance.version + "." + BundleVersion;

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, packageName.HasContent(3) ? packageName : initialPackageName);
            PlayerSettings.productName = productName.HasContent(3) ? productName : initialProductName;
            PlayerSettings.Android.targetArchitectures = (AndroidArchitecture)architecture;

#if UNITY_2020_1_OR_NEWER
            PlayerSettings.Android.minifyRelease = proguardMinification;
#else
            EditorUserBuildSettings.androidReleaseMinification = proguardMinification ? AndroidMinification.Proguard : AndroidMinification.None;
#endif
            EditorUserBuildSettings.buildAppBundle = buildAppBundle;

            switch (market)
            {
                case Market.Bazaar: symbols = AddSymbols(symbols, "BAZAAR"); break;
                case Market.Myket: symbols = AddSymbols(symbols, "MYKET"); break;
                case Market.GooglePlay: symbols = AddSymbols(symbols, "GOOGLE"); break;
                case Market.Huawei: symbols = AddSymbols(symbols, "HUAWEI"); break;
                case Market.Galaxy: symbols = AddSymbols(symbols, "GALAXY"); break;
                case Market.Tutuapp: symbols = AddSymbols(symbols, "TUTUAPP"); break;
                case Market.Aptoide: symbols = AddSymbols(symbols, "APTOIDE"); break;
                case Market.Emay: symbols = AddSymbols(symbols, "EMAY"); break;
                case Market.OneStore: symbols = AddSymbols(symbols, "ONESTORE"); break;
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbols);
            await WaitForEditor();

            if (BackupFiles(files) && PerformReplaces(files) && BringExternals(externalAssets))
            {
                await WaitForEditor();

                try
                {
                    result = BuildPlayer(BuildTarget.Android, locationPathName, autoRun);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }

                await WaitForEditor();
            }
            else Debug.LogError("Can't perform some file operations!");
            BuilderFileHandler.Externals.Cleanup();
            BuilderFileHandler.Bakcup.RestoreAllFiles();
            BuilderFileHandler.RemoveEmptyDirectoriesFromAssets();
            await WaitForEditor();

            EditorUserBuildSettings.buildAppBundle = false;
            PlayerSettings.productName = initialProductName;
            PlayerSettings.Android.targetArchitectures = initialArchitectures;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, initialPackageName);

            return result;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateFileInfos(files);
        }

        public void OpenKeyStoreFile(object sender)
        {
            var keypath = EditorUtility.OpenFilePanel("Open KeyStore File", null, "keystore");
            storeFilename = keypath.MakeRelative(System.IO.Directory.GetCurrentDirectory());
        }

        protected void CopyFiles(object sender)
        {
            if (files.Count < 1) return;
            EditorGUIUtility.systemCopyBuffer = JsonUtilityEx.ListToJson(files);
        }

        protected void PasteFiles(object sender)
        {
            try
            {
                var list = JsonUtilityEx.ListFromJson<FileInfo>(EditorGUIUtility.systemCopyBuffer);
                if (list.Count > 0)
                    files.AddRange(list);
            }
            catch { }
        }
    }
}
#endif
