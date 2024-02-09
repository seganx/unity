using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Profiling;

namespace SeganX.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        public enum Channel
        {
            Master = 0,
            Sound = 1,
            VFX = 2,
            UI = 3,
            Music = 4,
            Ambient = 5,
            Narrator = 6,
            Weapons = 7,
            AS_Attack = 8,
            AS_Assist = 9,
            AS_Shield = 10,
            GP_Probs = 11,
            Count = 12
        }

        [SerializeField] private AudioMixer mixer;

        private readonly WaitForEndOfFrame waitForFrame = new WaitForEndOfFrame();
        private readonly Dictionary<Channel, AudioSource> audioSources = new Dictionary<Channel, AudioSource>(8);

        private void Start()
        {
            for (int i = 0; i < (int)Channel.Count; i++)
            {
                var channel = (Channel)i;
                try
                {
                    audioSources.Add(channel, CreateAudioSource("AudioSource2D", channel, 0));
                }
                catch (System.Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private AudioSource CreateAudioSource(string audioType, Channel channel, float spatialBlend)
        {
            var audioSource = new GameObject($"{audioType}_{channel}").AddComponent<AudioSource>();
            audioSource.transform.SetParent(transform, false);
            audioSource.outputAudioMixerGroup = GetMixer(channel);
            audioSource.spatialBlend = spatialBlend;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            return audioSource;
        }

        private IEnumerator DoPlayMusic(AudioPlayerProfile musicProfile, float fadeInTime, float fadeOutTime)
        {
            var currentSource = instance.audioSources[Channel.Music];

            if (currentSource != null)
                StartCoroutine(DoFadeOut(currentSource, fadeOutTime));

            var source = instance.audioSources[Channel.Music].gameObject.AddComponent<AudioSource>();
            source.ignoreListenerVolume = true;
            source.outputAudioMixerGroup = mixer.FindMatchingGroups(Channel.Music.ToString())[0];
            source.clip = musicProfile.clip;
            source.pitch = CalculateMinMaxValue(musicProfile.pitch);
            source.loop = musicProfile.isLoop;
            source.volume = 0;
            source.Play();

            instance.audioSources[Channel.Music] = source;

            var targetVolume = CalculateMinMaxValue(musicProfile.volume);

            while (source.volume < targetVolume)
            {
                source.volume = Mathf.MoveTowards(source.volume, targetVolume, Time.deltaTime / fadeInTime);
                yield return waitForFrame;
            }
        }

        private IEnumerator DoFadeOut(AudioSource source, float fadeOutTime)
        {
            Destroy(source, fadeOutTime + .2f);
            while (source.volume > 0)
            {
                source.volume = Mathf.MoveTowards(source.volume, 0, Time.deltaTime / fadeOutTime);
                yield return waitForFrame;
            }
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static AudioPlayer instance = null;

        public static bool IsMute(Channel channel)
        {
            return PlayerPrefs.GetInt("AudioPlayer_Mute" + channel, 0) == 1;
        }

        public static void SetMute(Channel channel, bool mute)
        {
            if (mute)
                SetVolume(channel, 0);
            else
                SetVolume(channel, 1);

            PlayerPrefs.SetInt("AudioPlayer_Mute" + channel, mute ? 1 : 0);
        }

        /// <param name="value">between 0 and 1</param>
        public static void SetVolume(Channel channel, float value)
        {
            var key = $"{channel}Volume";
            PlayerPrefs.SetFloat("AudioPlayer_" + key, value);
            instance.mixer.SetFloat(key, Mathf.LerpUnclamped(-80, 0, value));
        }

        public static float GetVolume(Channel channel)
        {
            var key = $"{channel}Volume";
            return PlayerPrefs.GetFloat("AudioPlayer_" + key, 0);
        }

        public static AudioMixerGroup GetMixer(Channel channel)
        {
            return instance.mixer.FindMatchingGroups(channel.ToString())[0];
        }

        public static void TransitionTo(string state, float timeToReach)
        {
            var snapSh = instance.mixer.FindSnapshot(state);

            if (snapSh == null)
            {
                Debug.LogError($"{instance.name}: {nameof(PlayMusic)}: AudioMixerSnapshot is null!");
                return;
            }

            snapSh.TransitionTo(timeToReach);
        }

        public static void Play(AudioPlayerProfile audioProfile, float? pitch = null, float? volume = null)
        {
            if (audioProfile == null) return;
            if (audioProfile.clip == null) return;

            PlaySound(audioProfile, pitch, volume);
        }

        public static void Stop(AudioPlayerProfile profile)
        {
            if (profile == null) return;
            if (profile.clip == null) return;
            StopSound(profile.channel);
        }

        public static void PlayMusic(AudioPlayerProfile musicProfile, float fadeInTime = 0, float fadeOutTime = 0)
        {
            if (musicProfile == null) return;
            if (musicProfile.clip == null) return;
            instance.StartCoroutine(instance.DoPlayMusic(musicProfile, fadeInTime, fadeOutTime));
        }

        public static void PlayWithRandomPitch(AudioPlayerProfile audioProfile, float overridePitchFrom = 0.8f,
            float overridePitchTo = 1.2f)
        {
            PlaySound(audioProfile, Random.Range(overridePitchFrom, overridePitchTo));
        }

        public static void PlayRandom(List<AudioPlayerProfile> audioProfiles)
        {
            if (audioProfiles == null) return;
            if (audioProfiles.Count == 0) return;

            var tmpProfile = audioProfiles[Random.Range(0, audioProfiles.Count)];

            Play(tmpProfile);
        }

        private static void PlaySound(AudioPlayerProfile audioProfile, float? pitch = null, float? volume = null)
        {
            var key = audioProfile.channel;

            if (pitch.HasValue == false)
                pitch = CalculateMinMaxValue(audioProfile.pitch);

            if (volume.HasValue == false)
                volume = CalculateMinMaxValue(audioProfile.volume);

            // play 
            instance.audioSources[key].pitch = pitch.Value;
            instance.audioSources[key].PlayOneShot(audioProfile.clip, volume.Value);
        }

        private static void StopSound(Channel channel)
        {
            instance.audioSources[channel].Stop();
        }

        internal static float CalculateMinMaxValue(ParticleSystem.MinMaxCurve minMaxVolume)
        {
            return minMaxVolume.mode == ParticleSystemCurveMode.TwoConstants
                ? Random.Range(minMaxVolume.constantMin, minMaxVolume.constantMax)
                : minMaxVolume.constant;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnInitialize()
        {
            instance = new GameObject(nameof(AudioPlayer)).AddComponent<AudioPlayer>();
            instance.mixer = Resources.Load<AudioMixer>("Configs/AudioMixer");
            DontDestroyOnLoad(instance);
        }

        //////////////////////////////////////////////////////
        /// EDITOR MEMBERS
        //////////////////////////////////////////////////////
#if UNITY_EDITOR
        [UnityEditor.MenuItem("SeganX/Audio Mixer", priority = 75)]
        protected static void SelectMe()
        {
            UnityEditor.Selection.activeObject = Resources.Load<AudioMixer>("Configs/AudioMixer");
        }

        static AudioPlayer()
        {
            var currentMixer = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/Resources/Configs/AudioMixer.mixer");
            if (currentMixer == null)
            {
                UnityEditor.AssetDatabase.CopyAsset("Packages/com.seganx.unity/Editor/Templates/AudioMixerTemplate.mixer", "Assets/Resources/Configs/AudioMixer.mixer");
            }
        }
#endif
    }
}