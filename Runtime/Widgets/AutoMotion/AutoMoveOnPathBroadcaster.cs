using UnityEngine;

namespace SeganX.Widgets
{
    public class AutoMoveOnPathBroadcaster : MonoBehaviour
    {
        [SerializeField, TagSelector] private string targetTag;

        public void PlayAll(bool reset = false)
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                AutoMoveOnPath.PlayAll(reset);
            }
            else
            {
                AutoMoveOnPath.PlayAllByTag(targetTag, reset);
            }
        }

        public void StopAll()
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                AutoMoveOnPath.StopAll();
            }
            else
            {
                AutoMoveOnPath.StopAllByTag(targetTag);
            }
        }
    }
}