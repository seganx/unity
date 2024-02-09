using UnityEngine;

namespace SeganX.Audio
{
    public class PlaySound : MonoBehaviour
    {
        [SerializeField] protected bool is3D;
        [SerializeField] protected bool playOnStart;
        [SerializeField] protected AudioPlayerProfile audioPlayerProfile;

        private AudioSource audioSource;

        protected AudioSource AudioSource
        {
            get
            {
                if (audioSource == null)
                {
                    audioSource = GetComponent<AudioSource>();
                    if (audioSource == null)
                    {
                        audioSource = gameObject.AddComponent<AudioSource>();
                    }
                }

                return audioSource;
            }
            set => audioSource = value;
        }

        private void Start()
        {
            if (playOnStart && audioPlayerProfile != null)
            {
                Play();
            }
        }

        public void Play()
        {
            PlayCustom(audioPlayerProfile);
        }

        public virtual void PlayCustom(AudioPlayerProfile audioPlayerProfile)
        {
            if (audioPlayerProfile == null)
            {
                Debug.LogError($"{name}: {nameof(PlayMusic)}: Audio Player Profile is null!");
                return;
            }

            AudioSource.spatialBlend = is3D ? 1 : 0;
            AudioSource.rolloffMode = AudioRolloffMode.Linear;
            AudioSource.outputAudioMixerGroup = AudioPlayer.GetMixer(audioPlayerProfile.channel);
            AudioSource.pitch = AudioPlayer.CalculateMinMaxValue(audioPlayerProfile.pitch);
            AudioSource.volume = AudioPlayer.CalculateMinMaxValue(audioPlayerProfile.volume);
            AudioSource.loop = audioPlayerProfile.isLoop;

            if (AudioSource.isPlaying)
            {
                AudioSource.PlayOneShot(audioPlayerProfile.clip);
            }
            else
            {
                AudioSource.clip = audioPlayerProfile.clip;
                AudioSource.Play();
            }
        }

        public void Stop()
        {
            AudioSource.Stop();
        }
    }
}