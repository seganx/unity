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
        public Material blurMaterial = null;

        private RenderTexture reflectionBuffer = null;
        private RenderTexture reflectionBlurBuffer = null;
        private RenderTexture tmpBuffer = null;
        private Vector4 offset = Vector4.zero;
        private Vector4 offsetX = Vector4.zero;
        private Vector4 offsetY = Vector4.zero;

        private void Start()
        {
            if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32) == false) return;

            reflectionBuffer = CameraFX.CreateBuffer(camerafX.Width / screenScaleFactor, camerafX.Height / screenScaleFactor, true);
            screenScaleFactor *= 2;
            reflectionBlurBuffer = CameraFX.CreateBuffer(camerafX.Width / screenScaleFactor, camerafX.Height / screenScaleFactor, false);
            tmpBuffer = CameraFX.CreateBuffer(camerafX.Width / screenScaleFactor, camerafX.Height / screenScaleFactor, false);

            offsetX.x = reflectionBlurBuffer.texelSize.x;
            offsetY.y = reflectionBlurBuffer.texelSize.y;
            offsetX.z = reflectionBlurBuffer.texelSize.x * 2;
            offsetY.w = reflectionBlurBuffer.texelSize.y * 2;
        }

        private void OnDestroy()
        {
            if (reflectionBuffer == null) return;

            CameraFX.ReleaseBuffer(ref reflectionBuffer);
            CameraFX.ReleaseBuffer(ref reflectionBlurBuffer);
            CameraFX.ReleaseBuffer(ref tmpBuffer);
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
            reflectionCamera.fieldOfView = Camera.main.fieldOfView;
            ComputeCameraReflection();
        }

        private void OnPostRender()
        {
            if (reflectionBuffer == null) return;

            tmpBuffer.DiscardContents();
            blurMaterial.SetVector("_Offset", offsetX);
            Graphics.Blit(reflectionBuffer, tmpBuffer, blurMaterial);

            reflectionBlurBuffer.DiscardContents();
            blurMaterial.SetVector("_Offset", offsetY);
            Graphics.Blit(tmpBuffer, reflectionBlurBuffer, blurMaterial);

            Shader.SetGlobalTexture("_RflctTex", reflectionBuffer);
            Shader.SetGlobalTexture("_RflctBlurTex", reflectionBlurBuffer);
        }

        private void ComputeCameraReflection()
        {
            transform.position = PlanarMirror(camerafX.transform.position, reflectionAnchor.position, reflectionAnchor.up);
            var forward = PlanarMirror(camerafX.transform.forward, reflectionAnchor.position, reflectionAnchor.up);
            var up = PlanarMirror(camerafX.transform.up, reflectionAnchor.position, reflectionAnchor.up);
            transform.LookDirection(forward, up);
        }


        public static Vector3 PlanarMirror(Vector3 point, Vector3 planPosition, Vector3 planNormal)
        {
            var v = planPosition - point;
            var n = planNormal.normalized;
            var r = v - n * Vector3.Dot(v, n) * 2;
            return planPosition - r;
        }
    }
}