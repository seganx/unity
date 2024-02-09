using UnityEngine;

namespace SeganX.Widgets
{
    public class AutoRotateBroadcaster : MonoBehaviour
    {
        [SerializeField, TagSelector] private string targetTag;

        public void PlayAll(bool reset = false)
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                AutoRotateAlways.PlayAll(reset);
                AutoRotateOnView.PlayAll(reset);
            }
            else
            {
                AutoRotateAlways.PlayAllByTag(targetTag, reset);
                AutoRotateOnView.PlayAllByTag(targetTag, reset);
            }
        }

        public void StopAll()
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                AutoRotateAlways.StopAll();
                AutoRotateOnView.StopAll();
            }
            else
            {
                AutoRotateAlways.StopAllByTag(targetTag);
                AutoRotateOnView.StopAllByTag(targetTag);
            }
        }
    }
}