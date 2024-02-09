using System.Collections;
using UnityEngine;

namespace SeganX.Audio
{
    public class PlayMusic : PlaySound
    {
        [SerializeField] private float fadeOutTime = 0.5f;
        [SerializeField] private float fadeInTime = 0.5f;

        public override void PlayCustom(AudioPlayerProfile audioPlayerProfile)
        {
            if (audioPlayerProfile == null)
            {
                Debug.LogError($"{name}: {nameof(PlayMusic)}: Audio Player Profile is null!");
                return;
            }

            StartCoroutine(DoPlayMusic(audioPlayerProfile, fadeInTime, fadeOutTime));
        }

        private IEnumerator DoPlayMusic(AudioPlayerProfile musicProfile, float fadeInTime, float fadeOutTime)
        {
            var currentSource = AudioSource;

            if (currentSource != null)
                StartCoroutine(DoFadeOut(currentSource, fadeOutTime));

            var source = gameObject.AddComponent<AudioSource>();

            source.ignoreListenerVolume = true;
            source.outputAudioMixerGroup = AudioPlayer.GetMixer(AudioPlayer.Channel.Music);
            source.spatialBlend = is3D ? 1 : 0;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.clip = musicProfile.clip;
            source.loop = musicProfile.isLoop;
            source.pitch = AudioPlayer.CalculateMinMaxValue(musicProfile.pitch);
            source.volume = 0;

            source.Play();

            AudioSource = source;

            var targetVolume = AudioPlayer.CalculateMinMaxValue(musicProfile.volume);

            while (source != null && source.volume < targetVolume)
            {
                source.volume = Mathf.MoveTowards(source.volume, targetVolume, Time.deltaTime / fadeInTime);
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator DoFadeOut(AudioSource source, float fadeOutTime)
        {
            while (source != null && source.volume > 0)
            {
                source.volume = Mathf.MoveTowards(source.volume, 0, Time.deltaTime / fadeOutTime);
                yield return new WaitForEndOfFrame();
            }

            Destroy(source);
        }
    }
}