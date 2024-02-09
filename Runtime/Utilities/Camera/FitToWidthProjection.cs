using UnityEngine;

namespace SeganX
{
    public class FitToWidthProjection : MonoBehaviour
    {
        public Camera currCamera = null;


        private void OnPreRender()
        {
            //  recompute projection matrix to fix the width of screen
            UpdateProjectionMatrix();
        }

        public void UpdateProjectionMatrix()
        {
            if (currCamera == null)
            {
                currCamera = GetComponent<Camera>();
                enabled = currCamera != null;
            }

            float yscale = 1.0f / Mathf.Tan(currCamera.fieldOfView * Mathf.Deg2Rad);
            Matrix4x4 mat = currCamera.projectionMatrix;
            mat.m00 = yscale;
            mat.m11 = yscale * currCamera.aspect;
            currCamera.projectionMatrix = mat;
            currCamera.transparencySortMode = TransparencySortMode.Orthographic;
        }

        private void OnValidate()
        {
            if (currCamera == null) currCamera = transform.GetComponent<Camera>(true, true);
            if (currCamera == null) currCamera = Camera.main;
        }

        private void Reset()
        {
            OnValidate();
        }

        //public static void 
    }
}