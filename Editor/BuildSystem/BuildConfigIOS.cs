#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;

namespace SeganX.Builder
{
    [CreateAssetMenu(menuName = "Builder/iOS/Offline")]
    public class BuildConfigIOS : BuildConfigBase
    {
        public iOSTargetDevice targetDevice = iOSTargetDevice.iPhoneAndiPad;
        public iOSSdkVersion targetSDK = iOSSdkVersion.DeviceSDK;

        [Space]
        public bool automaticallySign = true;
        public string signingTeamID = "8769CRVZHQ";

        [Space(50), InspectorButton(100, "Copy", "CopyFiles", "Paste", "PasteFiles", true)]
        public bool buttons = false;
        public List<FileInfo> files = new List<FileInfo>();



        public override string FileName => $"{Core.GameName}_{Builder.Instance.version}.{BundleVersion}_iOS{postfix}";

        public override string GetError()
        {
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
            var initialPackageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);

            PlayerSettings.iOS.appleEnableAutomaticSigning = automaticallySign;
            PlayerSettings.iOS.appleDeveloperTeamID = signingTeamID.Trim();
            PlayerSettings.iOS.targetOSVersionString = "12.0";
            PlayerSettings.iOS.targetDevice = targetDevice;
            PlayerSettings.iOS.sdkVersion = targetSDK;
            PlayerSettings.iOS.appInBackgroundBehavior = iOSAppInBackgroundBehavior.Suspend;

            PlayerSettings.iOS.buildNumber = BundleVersion.ToString();
            PlayerSettings.bundleVersion = Builder.Instance.version + "." + PlayerSettings.iOS.buildNumber;
            PlayerSettings.productName = productName.HasContent(3) ? productName : initialProductName;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, packageName.HasContent(3) ? packageName : initialPackageName);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, symbols);
            await WaitForEditor();

            if (BackupFiles(files) && PerformReplaces(files) && BringExternals(externalAssets))
            {
                await WaitForEditor();

                try
                {
                    result = BuildPlayer(BuildTarget.iOS, locationPathName, autoRun);
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

            PlayerSettings.productName = initialProductName;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, initialPackageName);

            return result;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateFileInfos(files);
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
