using UnityEngine;

namespace SeganX.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class PlaySound : MonoBehaviour
    {
        [SerializeField] protected AudioSource audioSource = null;
        [SerializeField] protected AudioPlayerProfile audioPlayerProfile = null;

        protected virtual void Reset()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (audioSource.playOnAwake && audioPlayerProfile != null)
            {
                Play();
            }
        }

        public void Play()
        {
            PlayCustom(audioPlayerProfile);
        }

        public void Stop()
        {
            audioSource.Stop();
        }

        public virtual void PlayCustom(AudioPlayerProfile audioPlayerProfile)
        {
            if (audioPlayerProfile == null)
            {
                Debug.LogError($"{name}: {nameof(PlaySound)}: Audio Player Profile is null!");
                return;
            }
            audioSource.clip = audioPlayerProfile.clip;
            audioSource.outputAudioMixerGroup = AudioPlayer.GetMixer(audioPlayerProfile.channel);
            audioSource.pitch = AudioPlayer.CalculateMinMaxValue(audioPlayerProfile.pitch);
            audioSource.volume = AudioPlayer.CalculateMinMaxValue(audioPlayerProfile.volume);
            audioSource.loop = audioPlayerProfile.isLoop;
            audioSource.Play();
        }

        public virtual void PlayOneShot(AudioPlayerProfile audioPlayerProfile)
        {
            if (audioPlayerProfile == null)
            {
                Debug.LogError($"{name}: {nameof(PlaySound)}: Audio Player Profile is null!");
                return;
            }

            audioSource.outputAudioMixerGroup = AudioPlayer.GetMixer(audioPlayerProfile.channel);
            audioSource.pitch = AudioPlayer.CalculateMinMaxValue(audioPlayerProfile.pitch);
            audioSource.volume = AudioPlayer.CalculateMinMaxValue(audioPlayerProfile.volume);
            audioSource.loop = audioPlayerProfile.isLoop;
            audioSource.PlayOneShot(audioPlayerProfile.clip);
        }
    }
}