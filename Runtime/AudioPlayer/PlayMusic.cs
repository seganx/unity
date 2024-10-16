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
            // audioSource.isPlaying in the firs frame was true!!!!!! So I put it on the loop
            while (audioSource.isPlaying && audioSource.volume > 0)
            {
                audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0, Time.deltaTime / fadeOutTime);
                yield return new WaitForEndOfFrame();
            }

            base.PlayCustom(musicProfile);

            audioSource.volume = 0;
            var targetVolume = AudioPlayer.CalculateMinMaxValue(musicProfile.volume);

            while (audioSource.volume < targetVolume)
            {
                audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.deltaTime / fadeInTime);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}