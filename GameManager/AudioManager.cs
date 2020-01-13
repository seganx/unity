using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class AudioManager : MonoBehaviour
    {
        private class MusicSource
        {
            public float initVolume = 1;
            public AudioSource source = null;
        }

        [SerializeField] private AudioClip[] musics = null;

        private WaitForEndOfFrame waitForFrame = new WaitForEndOfFrame();

        private void Awake()
        {
            instance = this;
        }

        public void Play(int index, float volume, float fadeInTime, float fadeOutTime)
        {
            StopAllCoroutines();
            index = Mathf.Clamp(index, 0, musics.Length - 1);
            StartCoroutine(DoPlay(index, volume, fadeInTime, fadeOutTime));
        }

        private IEnumerator DoPlay(int index, float volume, float fadeInTime, float fadeOutTime)
        {
            if (currentSource.source != null)
                StartCoroutine(DoFadeOut(currentSource.source, fadeOutTime));

            currentSource.source = gameObject.AddComponent<AudioSource>();
            currentSource.source.ignoreListenerVolume = true;
            currentSource.source.clip = musics[index];
            currentSource.source.loop = true;
            currentSource.source.volume = 0;
            currentSource.source.Play();
            currentSource.initVolume = volume;

            var targetVolume = volume * MusicVolume * 0.01f;
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

        //////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////////////
        private static AudioManager instance = null;
        private static MusicSource currentSource = new MusicSource();
        private static int lastRandomIndex = -1;

        public static int MusicVolume
        {
            get { return PlayerPrefs.GetInt("GameSettings.MusicVolume", 100); }
            set
            {
                if (currentSource != null)
                {
                    currentSource.source.volume = currentSource.initVolume * value * 0.01f;
                }
                PlayerPrefs.SetInt("GameSettings.MusicVolume", value);
            }
        }

        public static int SoundVolume
        {
            get { return PlayerPrefs.GetInt("GameSettings.SoundVolume", 100); }
            set
            {
                AudioListener.volume = value * 0.01f;
                PlayerPrefs.SetInt("GameSettings.SoundVolume", value);
            }
        }

        public static void PlayMusic(int index, float volume = 1, float fadeInTime = 1, float fadeOutTime = 1)
        {
            instance.Play(index, volume, fadeInTime, fadeOutTime);
        }

        public static void PlayRandom(int fromIndex, int toIndex, float volume = 0.2f, float fadeInTime = 1, float fadeOutTime = 1)
        {
            fromIndex = Mathf.Clamp(fromIndex, 0, instance.musics.Length - 1);
            toIndex = Mathf.Clamp(toIndex, 0, instance.musics.Length - 1);

            var index = lastRandomIndex;
            while (index == lastRandomIndex)
                index = Random.Range(fromIndex, toIndex + 1);
            lastRandomIndex = index;

            instance.Play(index, volume, fadeInTime, fadeOutTime);
        }
    }
}
