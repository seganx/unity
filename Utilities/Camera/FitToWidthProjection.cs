using UnityEngine;

public class FitToWidthProjection : MonoBehaviour
{
    public Camera currCamera = null;

    // Update is called once per frame
    void Update()
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

        float fov = currCamera.fieldOfView * Mathf.Deg2Rad;
        float yscale = 1.0f / Mathf.Tan(fov * 0.5f);
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
