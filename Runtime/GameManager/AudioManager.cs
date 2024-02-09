using System.Collections;
using UnityEngine;

namespace SeganX
{
    public class AudioManager
    {
        private class MusicSource
        {
            public float targetVolume = 1;
            public AudioSource source = null;
        }

        private class Mono : MonoBehaviour
        {
            private readonly WaitForEndOfFrame waitForFrame = new WaitForEndOfFrame();

            private void Awake()
            {
                AudioListener.volume = SoundVolume;
                soundSource = gameObject.AddComponent<AudioSource>();
            }

            private IEnumerator Start()
            {
                var wait = new WaitForSecondsRealtime(5);
                while (true)
                {
                    yield return wait;
                    if (currentSource.source == null || currentSource.source.isPlaying == false)
                    {
                        mono.Play(currentTag, currentVolume, 1, 1);
                    }
                }
            }

            public void Play(string tag, float volume, float fadeInTime, float fadeOutTime)
            {
                var files = ResourceFiles.FindAll("Musics", false);
                if (files.Count < 1) return;
                files = files.FindAll(x => x.tags.Contains(tag));
                if (files.Count < 1) return;

                var index = PlayerPrefs.GetInt($"AudioManager.MusicIndex.{tag}", 0);
                PlayerPrefs.SetInt($"AudioManager.MusicIndex.{tag}", index + 1);

                var audioClip = Resources.Load<AudioClip>(files[index % files.Count].path);
                if (audioClip != null) StartCoroutine(DoPlay(audioClip, volume, fadeInTime, fadeOutTime));
            }

            private IEnumerator DoPlay(AudioClip audioClip, float volume, float fadeInTime, float fadeOutTime)
            {
                if (currentSource.source != null)
                    StartCoroutine(DoFadeOut(currentSource.source, fadeOutTime));

                currentSource.source = gameObject.AddComponent<AudioSource>();
                currentSource.source.ignoreListenerVolume = true;
                currentSource.source.clip = audioClip;
                currentSource.source.loop = false;
                currentSource.source.volume = 0;
                currentSource.source.Play();
                currentSource.targetVolume = volume;

                var targetVolume = volume * MusicVolume * MusicVolumeFactor;
                while (currentSource.source.volume < targetVolume)
                {
                    currentSource.source.volume = Mathf.MoveTowards(currentSource.source.volume, targetVolume, Time.deltaTime / fadeInTime);
                    yield return waitForFrame;
                }
            }

            private IEnumerator DoFadeOut(AudioSource source, float fadeOutTime)
            {
                while (source.volume > 0)
                {
                    source.volume = Mathf.MoveTowards(source.volume, 0, Time.deltaTime / fadeOutTime);
                    yield return waitForFrame;
                }
                Destroy(source);
            }
        }

        //////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////////////
        private static Mono mono = null;
        private static AudioSource soundSource = null;
        private static readonly MusicSource currentSource = new MusicSource();
        private static string currentTag = "menu";
        private static float currentVolume = 1;


        public static float MusicVolumeFactor
        {
            get { return PlayerPrefs.GetFloat("AudioManager.MusicVolumeFactor", 1); }
            set
            {
                PlayerPrefs.SetFloat("AudioManager.MusicVolumeFactor", value);
                if (currentSource != null && currentSource.source != null)
                    currentSource.source.volume = currentSource.targetVolume * MusicVolume * value;
            }
        }

        public static float MusicVolume
        {
            get { return PlayerPrefs.GetFloat("AudioManager.MusicVolume", 1); }
            set
            {
                PlayerPrefs.SetFloat("AudioManager.MusicVolume", value);
                if (currentSource != null && currentSource.source != null)
                    currentSource.source.volume = currentSource.targetVolume * MusicVolumeFactor * value;
            }
        }

        public static float SoundVolume
        {
            get { return PlayerPrefs.GetFloat("AudioManager.SoundVolume", 1); }
            set
            {
                PlayerPrefs.SetFloat("AudioManager.SoundVolume", value);
                AudioListener.volume = value;
            }
        }

        public static void PlayMusic(string tag, float volume)
        {
            if (tag != currentTag)
            {
                currentTag = tag;
                currentVolume = volume;
                mono.Play(currentTag, currentVolume, 1, 1);
            }
        }


        public static void PlaySound(AudioClip soundClip, float pitch = 1)
        {
            if (soundClip == null) return;
            soundSource.PlayOneShot(soundClip);
            soundSource.pitch = pitch;
        }

        public static void PlaySound(AudioClip[] soundClip, float pitch = 1)
        {
            if (soundClip == null || soundClip.Length < 1) return;
            soundSource.PlayOneShot(soundClip[Random.Range(0, 100) % soundClip.Length]);
            soundSource.pitch = pitch;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnInitialize()
        {
            mono = new GameObject("AudioManager").AddComponent<Mono>();
            //mono.gameObject.hideFlags = HideFlags.HideInHierarchy;
            Object.DontDestroyOnLoad(mono);
        }
    }
}
