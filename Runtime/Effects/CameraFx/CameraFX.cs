#if SX_CAMFX
using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Effects
{
    public class CameraFX : MonoBehaviour
    {
        public Material postMaterial = null;
        public Material blitMaterial = null;
        public CameraFX_Bloom bloom = new CameraFX_Bloom();

        private Camera currentCamera = null;
        private RenderTexture sceneBuffer = null;
        private RenderTexture frameBuffer = null;

        private void Initialize()
        {
            if (!IsSupported) return;
            Clear();
            postMaterial = postMaterial.Clone();
            blitMaterial = blitMaterial.Clone();
            sceneBuffer = CreateBuffer(Width, Height, true);
            frameBuffer = CreateBuffer(Width, Height, false);
        }

        private void Clear()
        {
            if (frameBuffer == null) return;
            currentCamera.targetTexture = null;
            bloom.Clear();
            ReleaseBuffer(ref frameBuffer);
            ReleaseBuffer(ref sceneBuffer);
        }

        private void OnEnable()
        {
            currentCamera = GetComponent<Camera>();
            if (currentCamera == Camera.main) instance = this;
        }

        private void Start()
        {
            Activated = Activated;
        }

        private void OnPreRender()
        {
            if (Activated == false || sceneBuffer == null)
            {
                currentCamera.targetTexture = null;
                RenderTexture.active = null;
            }
            else currentCamera.targetTexture = sceneBuffer;

            if (Quality == 0)
            {
                Shader.DisableKeyword("SX_SIMPLE");
            }
            else
            {
                Shader.EnableKeyword("SX_SIMPLE");
            }
        }

        private void OnPostRender()
        {
            if (Activated == false || sceneBuffer == null) return;
            currentCamera.targetTexture = null;

            bloom.OnPostRender(sceneBuffer);

            frameBuffer.DiscardContents();
            Graphics.Blit(sceneBuffer, frameBuffer, postMaterial);

            Graphics.Blit(frameBuffer, null as RenderTexture, blitMaterial);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            bloom.Clear();
        }
#endif


        /////////////////////////////////////////////////////////////////////////////////////
        /// STATIC MEMBER
        /////////////////////////////////////////////////////////////////////////////////////
        private static CameraFX instance = null;
        private static List<RenderTexture> buffers = new List<RenderTexture>(5);

        public static bool IsSupported
        {
            get
            {
                if (instance == null) return false;
                return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Default) && instance.postMaterial.shader.isSupported && instance.blitMaterial.shader.isSupported;
            }
        }

        public static int Width
        {
            get
            {
                return Screen.width * Resolution / 100;
            }
        }

        public static int Height
        {
            get
            {
                return Screen.height * Resolution / 100;
            }
        }

        public static int Resolution
        {
            set
            {
                value = Mathf.Clamp(value, 20, 100);
                if (Resolution == value) return;
                PlayerPrefs.SetInt("CameraFx.Resolution", value);
                if (Activated) instance.Initialize();
            }

            get
            {
                return PlayerPrefs.GetInt("CameraFx.Resolution", 100);
            }
        }

        public static bool Activated
        {
            set
            {
                if (instance == null) return;
                PlayerPrefs.SetInt("CameraFx.Activated", value ? 1 : 0);
                if (value && !IsSupported)
                {
                    var str = "Can't support cameraFX shaders:";
                    if (instance.blitMaterial.shader.isSupported == false) str += "\n" + instance.blitMaterial.shader.name;
                    if (instance.postMaterial.shader.isSupported == false) str += "\n" + instance.postMaterial.shader.name;
                    if (instance.blitMaterial.shader.isSupported == false) str += "\n" + instance.blitMaterial.shader.name;
                    Debug.Log(str);
                    return;
                }
                if (value)
                    instance.Initialize();
                else
                    instance.Clear();
            }
            get
            {
                return IsSupported && PlayerPrefs.GetInt("CameraFx.Activated", 1) > 0;
            }
        }


        public static float MotionBlurValue
        {
            get { return (instance && instance.postMaterial) ? (1 - instance.postMaterial.color.a) : 0; }
            set
            {
                if (instance && instance.postMaterial)
                {
                    var c = instance.postMaterial.color;
                    c.a = 1 - value;
                    instance.postMaterial.color = c;
                }
            }
        }

        public static bool Bloom
        {
            get { return PlayerPrefs.GetInt("CameraFx.Bloom", 1) > 0; }
            set { PlayerPrefs.SetInt("CameraFx.Bloom", value ? 1 : 0); }
        }

        public static int Quality
        {
            get { return PlayerPrefs.GetInt("CameraFx.Quality", 0); }
            set { PlayerPrefs.SetInt("CameraFx.Quality", value); }
        }


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
                Destroy(buffer);
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
                    Destroy(buffers[i]);
                }
                buffers[i] = null;
            }
            buffers.Clear();
        }

        [Console("screen", "size")]
        public static void SetScreenSize(int resolution)
        {
            Resolution = resolution;
        }
    }
}
#endif