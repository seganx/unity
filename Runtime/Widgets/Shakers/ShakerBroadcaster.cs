using UnityEngine;

namespace SeganX.Widgets
{
    public class ShakerBroadcaster : MonoBehaviour
    {
        [SerializeField, TagSelector] private string targetTag = string.Empty;

        public void ShakeAll(float seconds)
        {
            if (string.IsNullOrEmpty(targetTag))
                ShakerBase.ShakeAll(seconds);
            else
                ShakerBase.ShakeAll(seconds, targetTag);
        }
    }

}