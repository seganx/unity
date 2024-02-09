using UnityEngine;

namespace SeganX.Widgets
{
    public class ColorAnimationBroadcaster : MonoBehaviour
    {
        [SerializeField, TagSelector] private string targetTag;

        public void PlayAll(bool reset = false)
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                ColorAnimation3D.PlayAll(reset);
                UiColorAnimation.PlayAll(reset);
            }
            else
            {
                ColorAnimation3D.PlayAllByTag(targetTag, reset);
                UiColorAnimation.PlayAllByTag(targetTag, reset);
            }
        }

        public void StopAll()
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                ColorAnimation3D.StopAll();
                UiColorAnimation.StopAll();
            }
            else
            {
                ColorAnimation3D.StopAllByTag(targetTag);
                UiColorAnimation.StopAllByTag(targetTag);
            }
        }
    }
}