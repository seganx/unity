using UnityEngine;

namespace SeganX.Audio
{
    //[CreateAssetMenu(menuName = "Audio Player Profile")]
    public class AudioPlayerProfile : ScriptableObject
    {
        public AudioClip clip = null;
        public AudioPlayer.Channel channel = AudioPlayer.Channel.Master;
        [Space]
        public ParticleSystem.MinMaxCurve volume = 1f;
        public ParticleSystem.MinMaxCurve pitch = 1f;
        public bool isLoop = false;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/AudioPlayerProfile", false, 1)]
        private static void CreateAudioPlayerProfile()
        {
            if (UnityEditor.Selection.objects.Length > 0)
            {
                foreach (var item in UnityEditor.Selection.objects)
                {
                    CreateNewOne(item);
                }
            }
            else CreateNewOne(null);
        }

        private static void CreateNewOne(Object selectedObject)
        {
            string path = "Assets";
            if (selectedObject != null)
            {
                path = UnityEditor.AssetDatabase.GetAssetPath(selectedObject);
                path = System.IO.Path.GetDirectoryName(path);
            }

            var source = selectedObject as AudioClip;
            var name = source ? source.name : nameof(AudioPlayerProfile);
            path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"{path}/{name}.asset");

            var asset = CreateInstance(typeof(AudioPlayerProfile)) as AudioPlayerProfile;
            asset.name = name;
            asset.clip = source;
            UnityEditor.AssetDatabase.CreateAsset(asset, path);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.Selection.activeObject = asset;
        }
#endif
    }
}