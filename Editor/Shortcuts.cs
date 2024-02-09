using UnityEditor;
using static UnityEditor.SceneView;

namespace SeganX
{
    public static class Shortcuts
    {
        [MenuItem("SeganX/Tools/Wireframe Mode &w", false)]
        public static void ChangeDrawCameraMode()
        {
            if (lastActiveSceneView == null) return;

            if (lastActiveSceneView.cameraMode.drawMode == DrawCameraMode.Wireframe)
            {
                lastActiveSceneView.cameraMode = GetBuiltinCameraMode(DrawCameraMode.Textured);
            }
            else
            {
                lastActiveSceneView.cameraMode = GetBuiltinCameraMode(DrawCameraMode.Wireframe);
            }
        }

        [MenuItem("SeganX/Tools/Lock Inspector &q", false)]
        public static void InspectorLock()
        {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        }
    }
}