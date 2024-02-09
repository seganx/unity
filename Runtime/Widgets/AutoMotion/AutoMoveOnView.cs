using UnityEngine;

namespace SeganX.Widgets
{
    [RequireComponent(typeof(Renderer))]
    public class AutoMoveOnView : AutoMoveBase
    {
        private void OnWillRenderObject()
        {
            if (IsReferenceAvailable)
                Action();
        }
    }
}