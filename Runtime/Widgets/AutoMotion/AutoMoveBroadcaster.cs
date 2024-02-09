using UnityEngine;

namespace SeganX.Widgets
{
    public class AutoMoveBroadcaster : MonoBehaviour
    {
        [SerializeField, TagSelector] private string targetTag;

        public void PlayAll(bool reset = false)
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                AutoMoveAlways.PlayAll(reset);
                AutoMoveOnView.PlayAll(reset);
                UiAutoMove.PlayAll(reset);
            }
            else
            {
                AutoMoveAlways.PlayAllByTag(targetTag, reset);
                AutoMoveOnView.PlayAllByTag(targetTag, reset);
                UiAutoMove.PlayAllByTag(targetTag, reset);
            }
        }

        public void StopAll()
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                AutoMoveAlways.StopAll();
                AutoMoveOnView.StopAll();
                UiAutoMove.StopAll();
            }
            else
            {
                AutoMoveAlways.StopAllByTag(targetTag);
                AutoMoveOnView.StopAllByTag(targetTag);
                UiAutoMove.StopAllByTag(targetTag);
            }
        }
    }
}