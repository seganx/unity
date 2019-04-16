using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SeganX.Effects
{
    public class CameraFX_Reflection : MonoBehaviour
    {
        public CameraFX camerafX = null;
        public Camera reflectionCamera = null;
        public Transform reflectionAnchor = null;
        public int screenScaleFactor = 8;

        private RenderTexture reflectionBuffer = null;

        private void Start()
        {
            if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Default) == false) return;

            var frameBufferDesc = new RenderTextureDescriptor(camerafX.Width / screenScaleFactor, camerafX.Height / screenScaleFactor, RenderTextureFormat.ARGB32, 32);
            frameBufferDesc.autoGenerateMips = false;
            reflectionBuffer = new RenderTexture(frameBufferDesc);
        }

        private void OnDestroy()
        {         
            if (reflectionBuffer == null) return;

            reflectionCamera.targetTexture = null;
            reflectionBuffer.Release();
            reflectionBuffer = null;
        }

        private void OnValidate()
        {
            if (reflectionCamera == null)
                reflectionCamera = GetComponent<Camera>();
        }

        private void OnPreRender()
        {
            if (reflectionBuffer == null) return;
            reflectionCamera.targetTexture = reflectionBuffer;
            ComputeCameraReflection();
        }

        private void OnPostRender()
        {
            if (reflectionBuffer == null) return;
            Shader.SetGlobalTexture("_RflctTex", reflectionBuffer);
        }

        private void ComputeCameraReflection()
        {
            var v = reflectionAnchor.position - Camera.main.transform.position;
            var n = reflectionAnchor.up.normalized;
            var r = v - n * Vector3.Dot(v, n) * 2;
            transform.position = reflectionAnchor.position - r;
            transform.LookAt(reflectionAnchor.position);            
        }
    }
}