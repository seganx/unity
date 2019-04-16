using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SeganX.Effects
{
    public class CameraFX : Base
    {
        public Vector2Int displaySize = new Vector2Int(1024, 500);
        public Material SceneMaterial = null; 
        public Material frameMaterial = null;
        public Material screenMaterial = null;
        public CameraFX_Bloom bloom = new CameraFX_Bloom();

        private Camera currentCamera = null;
        private RenderTexture sceneBuffer = null;
        private RenderTexture frameBuffer = null;
        private RenderTexture screenBuffer = null;
        private bool activate = false;
        private int resolationFactor = 100;

        public bool Supported
        {
            get
            {
                return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Default);//&& SystemInfo.supportsRenderToCubemap;
            }
        }

        public bool Activate
        {
            set
            {
                if (activate == value) return;
                if (value && !Supported) return;
                activate = value;
                if (value)
                    Initialize();
                else
                    Clear();
            }
            get { return activate && Supported; }
        }

        public int Resolution
        {
            set
            {
                value = Mathf.Clamp(value, 20, 100);
                if (resolationFactor == value) return;
                resolationFactor = value;
                if (activate) Initialize();
            }

            get { return resolationFactor; }
        }

        public int Width
        {
            get
            {
                return DisplaySize.x * resolationFactor / 100;
            }
        }

        public int Height
        {
            get
            {
                return DisplaySize.y * resolationFactor / 100;
            }
        }

        private Vector2Int DisplaySize
        {
            get
            {
                if (displaySize.x > Screen.width)
                {
                    displaySize.x = Screen.width;
                    displaySize.y = Screen.height;
                }
                else displaySize.y = displaySize.x * Screen.height / Screen.width;

                return displaySize;
            }
        }

        private void Initialize()
        {
            if (!Supported) return;
            Clear();
            sceneBuffer = CreateBuffer(Width, Height, true);
            frameBuffer = CreateBuffer(Width, Height, false);
            screenBuffer = CreateBuffer(Width, Height, false);
        }

        private void Clear()
        {
            if (frameBuffer == null) return;
            currentCamera.targetTexture = null;
            bloom.Clear();
            ReleaseBuffer(ref screenBuffer);
            ReleaseBuffer(ref frameBuffer);
            ReleaseBuffer(ref sceneBuffer);
        }

        private void Start()
        {
            currentCamera = GetComponent<Camera>();
            Activate = true;
        }

        private void OnPreRender()
        {
            if (sceneBuffer == null) return;
            currentCamera.targetTexture = sceneBuffer;
        }

        private void OnPostRender()
        {
            if (sceneBuffer == null) return;
            currentCamera.targetTexture = null;

            frameBuffer.DiscardContents();
            Graphics.Blit(sceneBuffer, frameBuffer, SceneMaterial);

            bloom.OnPostRender(frameBuffer);

            screenBuffer.MarkRestoreExpected();
            Graphics.Blit(frameBuffer, screenBuffer, frameMaterial);

            Graphics.Blit(screenBuffer, null as RenderTexture, screenMaterial);            
        }



        /////////////////////////////////////////////////////////////////////////////////////
        /// STATIC MEMBER
        /////////////////////////////////////////////////////////////////////////////////////
        private static List<RenderTexture> buffers = new List<RenderTexture>(5);

        public static RenderTexture CreateBuffer(int width, int height, bool depth)
        {
            var desc = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, depth ? 32 : 0);
            desc.autoGenerateMips = false;
            var res = new RenderTexture(desc);
            buffers.Add(res);
            return res;
        }

        public static RenderTexture ReleaseBuffer(ref RenderTexture buffer)
        {
            if (buffer != null)
            {
                buffers.Remove(buffer);
                buffer.Release();
                DestroyImmediate(buffer);
                buffer = null;
            }
            return null;
        }

        private static void ClearBuffers()
        {
            for (int i = 0; i < buffers.Count; i++)
            {
                if (buffers[i] != null)
                {
                    buffers[i].Release();
                    DestroyImmediate(buffers[i]);
                }
                buffers[i] = null;
            }
            buffers.Clear();
        }
    }
}