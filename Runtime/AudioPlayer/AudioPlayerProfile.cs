using UnityEngine;

namespace SeganX.Audio
{
    [CreateAssetMenu(menuName = "Audio Player Profile")]
    public class AudioPlayerProfile : ScriptableObject
    {
        public AudioClip clip;
        public AudioPlayer.Channel channel;
        [Space] public ParticleSystem.MinMaxCurve volume = 1f;
        public ParticleSystem.MinMaxCurve pitch = 1f;
        public bool isLoop;
    }
}