using UnityEngine;

namespace SeganX.Widgets
{
    public class AutoScaleBroadcaster : MonoBehaviour
    {
        [SerializeField, TagSelector] private string targetTag;

        public void PlayAll(bool reset = false)
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                AutoScaleAlways.PlayAll(reset);
                AutoScaleOnView.PlayAll(reset);
            }
            else
            {
                AutoScaleAlways.PlayAllByTag(targetTag, reset);
                AutoScaleOnView.PlayAllByTag(targetTag, reset);
            }
        }

        public void StopAll()
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                AutoScaleAlways.StopAll();
                AutoScaleOnView.StopAll();
            }
            else
            {
                AutoScaleAlways.StopAllByTag(targetTag);
                AutoScaleOnView.StopAllByTag(targetTag);
            }
        }
    }
}