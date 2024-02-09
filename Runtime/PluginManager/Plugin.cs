#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    [CreateAssetMenu(fileName = "Plugin", menuName = "SeganX/Plugin")]
    public class Plugin : ScriptableObject
    {
        public bool activated = false;
        public string folder = string.Empty;
        public string symbols = string.Empty;
        public string description = string.Empty;
        public string productName = string.Empty;
        public string packageName = string.Empty;
        public List<string> files = new List<string>();
    }
}
#endif