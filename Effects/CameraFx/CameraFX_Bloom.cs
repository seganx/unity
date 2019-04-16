using UnityEngine;

namespace SeganX.Effects
{
    [System.Serializable]
    public class CameraFX_Bloom
    {
        public bool activate = true;
        public int scaleFactor = 16;
        public Material downScaleMaterial = null;
        public Material blurMaterial = null;
        public Material postMaterial = null;

        private RenderTexture bloomBuffer = null;
        private RenderTexture tmpBuffer = null;
        private Vector4 offset = Vector4.zero;
        private Vector4 offsetX = Vector4.zero;
        private Vector4 offsetY = Vector4.zero;

        public void OnPostRender(RenderTexture frameBuffer)
        {
            if (activate == false)
            {
                Clear();
                return;
            }

            if (bloomBuffer == null)
                Initialize(frameBuffer);

            bloomBuffer.DiscardContents();
            downScaleMaterial.SetVector("_Offset", offset);
            Graphics.Blit(frameBuffer, bloomBuffer, downScaleMaterial);

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

            frameBuffer.MarkRestoreExpected();
            Graphics.Blit(bloomBuffer, frameBuffer, postMaterial);
        }

        private void Initialize(RenderTexture frameBuffer)
        {
            Clear();
            bloomBuffer = CameraFX.CreateBuffer(frameBuffer.width / scaleFactor, frameBuffer.height / scaleFactor, false);
            tmpBuffer = CameraFX.CreateBuffer(frameBuffer.width / scaleFactor, frameBuffer.height / scaleFactor, false);

            offset.z = offset.x = frameBuffer.texelSize.x * scaleFactor * 0.5f;
            offset.w -= offset.y = frameBuffer.texelSize.y * scaleFactor * 0.5f;

            offsetX.x = bloomBuffer.texelSize.x;
            offsetY.y = bloomBuffer.texelSize.y;
            offsetX.z = bloomBuffer.texelSize.x * 2;
            offsetY.w = bloomBuffer.texelSize.y * 2;
        }

        public void Clear()
        {
            if (bloomBuffer == null) return;
            CameraFX.ReleaseBuffer(ref bloomBuffer);
            CameraFX.ReleaseBuffer(ref tmpBuffer);
        }
    }
}