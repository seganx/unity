using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleReflection : MonoBehaviour
{
    public Camera currentCamera = null;
    public MeshRenderer meshRenderer = null;
    public Vector3 normalVector = Vector3.up;
    public Vector2Int reflectionBufferSize = Vector2Int.one * 64;

    private RenderTexture defaultBuffer = null;
    private RenderTexture reflectionBuffer = null;

    private void OnEnable()
    {
        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Default) == false) return;

        var reflectionBufferDesc = new RenderTextureDescriptor(reflectionBufferSize.x, reflectionBufferSize.y, RenderTextureFormat.Default, 24);
        reflectionBufferDesc.autoGenerateMips = false;
        reflectionBuffer = new RenderTexture(reflectionBufferDesc);

        if (reflectionBuffer != null)
            Debug.Log("Reflection buffer created: " + reflectionBufferDesc.GetStringDebug());
        else
            Debug.Log("Creating reflection buffer failed!");
    }

    private void OnDisable()
    {
        if (reflectionBuffer == null) return;
        RenderTexture.active = null;
        currentCamera.targetTexture = null;
        reflectionBuffer.Release();
        reflectionBuffer = null;
    }

    private void OnPreRender()
    {
        if (reflectionBuffer == null) return;
        defaultBuffer = RenderTexture.active;
        RenderTexture.active = reflectionBuffer;
        currentCamera.targetTexture = reflectionBuffer;
    }

    private void OnPostRender()
    {
        if (reflectionBuffer == null) return;
        RenderTexture.active = defaultBuffer;
        currentCamera.targetTexture = null;
        meshRenderer.material.mainTexture = reflectionBuffer;
    }
}
