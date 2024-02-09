using UnityEngine;
using UnityEngine.Audio;

namespace SeganX.Audio
{
    public class AudioTransition : MonoBehaviour
    {
        public float timeToReach = 1f;

        public void TransitionTo(AudioMixerSnapshot snapshot)
        {
            AudioPlayer.TransitionTo(snapshot.ToString(), timeToReach);
        }

        public void TransitionToImmediate(AudioMixerSnapshot snapshot)
        {
            AudioPlayer.TransitionTo(snapshot.ToString(), 0);
        }
    }
}