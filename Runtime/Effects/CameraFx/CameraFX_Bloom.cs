#if SX_CAMFX
using UnityEngine;

namespace SeganX.Effects
{
    [System.Serializable]
    public class CameraFX_Bloom
    {
        public int scaleFactor = 16;
        public float offsetFactor = 1;
        public Material downScaleMaterial = null;
        public Material blurMaterial = null;
        public Material postMaterial = null;

        private RenderTexture bloomBuffer = null;
        private RenderTexture tmpBuffer = null;
        private Vector4 offset = Vector4.zero;
        private Vector4 offsetX = Vector4.zero;
        private Vector4 offsetY = Vector4.zero;

#if UNITY_EDITOR
        [Header("Debug Params")]
        [SerializeField] private Material blitMaterial = null;
        [SerializeField] private bool displayDownScale = false;
        [SerializeField] private bool displayBlured = false;
#endif

        public bool IsSupported
        {
            get
            {
                return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Default) && downScaleMaterial.shader.isSupported && blurMaterial.shader.isSupported && postMaterial.shader.isSupported;
            }
        }

        public void OnPostRender(RenderTexture frameBuffer)
        {
            if (IsSupported == false || CameraFX.Bloom == false)
            {
                Clear();
                return;
            }

            if (bloomBuffer == null)
                Initialize(frameBuffer);

            bloomBuffer.DiscardContents();
            downScaleMaterial.SetVector("_Offset", offset);
            Graphics.Blit(frameBuffer, bloomBuffer, downScaleMaterial);

#if UNITY_EDITOR
            if (displayDownScale)
            {
                frameBuffer.MarkRestoreExpected();
                Graphics.Blit(bloomBuffer, frameBuffer, blitMaterial);
                return;
            }
#endif

            tmpBuffer.DiscardContents();
            blurMaterial.SetVector("_Offset", offsetX);
            Graphics.Blit(bloomBuffer, tmpBuffer, blurMaterial);

            bloomBuffer.DiscardContents();
            blurMaterial.SetVector("_Offset", offsetY);
            Graphics.Blit(tmpBuffer, bloomBuffer, blurMaterial);

            tmpBuffer.DiscardContents();
            blurMaterial.SetVector("_Offset", offsetX);
            Graphics.Blit(bloomBuffer, tmpBuffer, blurMaterial);

            bloomBuffer.DiscardContents();
            blurMaterial.SetVector("_Offset", offsetY);
            Graphics.Blit(tmpBuffer, bloomBuffer, blurMaterial);

#if UNITY_EDITOR
            if (displayBlured)
            {
                frameBuffer.MarkRestoreExpected();
                Graphics.Blit(bloomBuffer, frameBuffer, blitMaterial);
                return;
            }
#endif

            frameBuffer.MarkRestoreExpected();
            Graphics.Blit(bloomBuffer, frameBuffer, postMaterial);
        }

        private void Initialize(RenderTexture frameBuffer)
        {
            Clear();

            downScaleMaterial = downScaleMaterial.Clone();
            blurMaterial = blurMaterial.Clone();
            postMaterial = postMaterial.Clone();

            bloomBuffer = CameraFX.CreateBuffer(frameBuffer.width / scaleFactor, frameBuffer.height / scaleFactor, false);
            tmpBuffer = CameraFX.CreateBuffer(frameBuffer.width / scaleFactor, frameBuffer.height / scaleFactor, false);

            offset.z = offset.x = frameBuffer.texelSize.x * scaleFactor * 0.5f;
            offset.y = frameBuffer.texelSize.y * scaleFactor * 0.5f;
            offset.w = -offset.y;

            offsetX.x = bloomBuffer.texelSize.x * offsetFactor;
            offsetY.y = bloomBuffer.texelSize.y * offsetFactor;
            offsetX.z = bloomBuffer.texelSize.x * offsetFactor * 2;
            offsetY.w = bloomBuffer.texelSize.y * offsetFactor * 2;
        }

        public void Clear()
        {
            if (bloomBuffer == null) return;
            CameraFX.ReleaseBuffer(ref bloomBuffer);
            CameraFX.ReleaseBuffer(ref tmpBuffer);
        }
    }
}
#endif