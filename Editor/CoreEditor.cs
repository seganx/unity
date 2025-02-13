using UnityEditor;

namespace SeganX
{
    [CustomEditor(typeof(Core))]
    public class CoreEditor : Editor
    {
        [MenuItem("SeganX/Settings", priority = 100)]
        private static void Settings()
        {
            Selection.activeObject = Core.Instance;
        }
    }
}