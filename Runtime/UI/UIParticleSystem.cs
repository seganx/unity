using UnityEngine;
using UnityEngine.UI;

namespace SeganX
{
    [System.Obsolete("Use UiParticleRenderer & UiParticleEmitter instead!")]
    public class UIParticleSystem : MaskableGraphic
    {        
        [SerializeField] private Shader hiddenShader = null;
        [SerializeField] private Texture particleTexture;
        [SerializeField] private Sprite particleSprite;

        private ParticleSystem currentParticleSystem;
        private ParticleSystem.Particle[] particles;
        private UIVertex[] quad = new UIVertex[4];
        private Vector4 uv = Vector4.zero;
        private ParticleSystem.TextureSheetAnimationModule textureSheetAnimation;
        private int textureSheetAnimationFrames;
        private Vector2 textureSheedAnimationFrameSize;

        public override Texture mainTexture
        {
            get
            {
                if (particleTexture)
                {
                    return particleTexture;
                }

                if (particleSprite)
                {
                    return particleSprite.texture;
                }

                return null;
            }
        }

        protected bool Initialize()
        {
            // prepare particle system
            ParticleSystemRenderer renderer = GetComponent<ParticleSystemRenderer>();
            bool setParticleSystemMaterial = false;

            if (currentParticleSystem == null)
            {
                currentParticleSystem = GetComponent<ParticleSystem>();

                if (currentParticleSystem == null)
                {
                    return false;
                }

                // get current particle texture
                if (renderer == null)
                {
                    renderer = currentParticleSystem.gameObject.AddComponent<ParticleSystemRenderer>();
                }
                Material currentMaterial = renderer.sharedMaterial;
                if (currentMaterial && currentMaterial.HasProperty("_MainTex"))
                {
                    particleTexture = currentMaterial.mainTexture;
                }

                particles = null;
                setParticleSystemMaterial = true;
            }
            else
            {
                if (Application.isPlaying)
                {
                    setParticleSystemMaterial = (renderer.material == null);
                }
#if UNITY_EDITOR
                else
                {
                    setParticleSystemMaterial = (renderer.sharedMaterial == null);
                }
#endif
            }

            // automatically set material to UI/Particles/Hidden shader, and get previous texture
            if (setParticleSystemMaterial)
            {
                Material material = new Material(hiddenShader);
                if (Application.isPlaying)
                {
                    renderer.material = material;
                    gameObject.RefreshMaterials();
                }
#if UNITY_EDITOR
                else
                {
                    material.hideFlags = HideFlags.DontSave;
                    renderer.sharedMaterial = material;
                }
#endif
            }

            // prepare particles array
            if (particles == null)
            {
                var mainModul = currentParticleSystem.main;
                particles = new ParticleSystem.Particle[mainModul.maxParticles];
            }

            // prepare uvs
            if (particleTexture)
            {
                uv = new Vector4(0, 0, 1, 1);
            }
            else if (particleSprite)
            {
                uv = UnityEngine.Sprites.DataUtility.GetOuterUV(particleSprite);
            }

            // prepare texture sheet animation
            textureSheetAnimation = currentParticleSystem.textureSheetAnimation;
            textureSheetAnimationFrames = 0;
            textureSheedAnimationFrameSize = Vector2.zero;
            if (textureSheetAnimation.enabled)
            {
                textureSheetAnimationFrames = textureSheetAnimation.numTilesX * textureSheetAnimation.numTilesY;
                textureSheedAnimationFrameSize = new Vector2(1f / textureSheetAnimation.numTilesX, 1f / textureSheetAnimation.numTilesY);
            }

            return true;
        }

        protected override void Awake()
        {
            base.Awake();

            if (hiddenShader == null)
                hiddenShader = Shader.Find("SeganX/UI/Hidden");

            if (!Initialize())
            {
                enabled = false;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (!Initialize())
                {
                    return;
                }
            }
#endif

            // prepare vertices
            vh.Clear();

            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            // iterate through current particles
            int count = currentParticleSystem.GetParticles(particles);
            var mainModul = currentParticleSystem.main;

            for (int i = 0; i < count; ++i)
            {
                ParticleSystem.Particle particle = particles[i];

                // get particle properties
                Vector2 position = (mainModul.simulationSpace == ParticleSystemSimulationSpace.Local ? particle.position : transform.InverseTransformPoint(particle.position));
                float rotation = -particle.rotation * Mathf.Deg2Rad;
                float rotation90 = rotation + Mathf.PI / 2;
                Color32 color = particle.GetCurrentColor(currentParticleSystem);
                float size = particle.GetCurrentSize(currentParticleSystem) * 0.5f;

                // apply scale
                if (mainModul.scalingMode == ParticleSystemScalingMode.Shape)
                {
                    position /= canvas.scaleFactor;
                }

                // apply texture sheet animation
                Vector4 particleUV = uv;
                if (textureSheetAnimation.enabled)
                {
                    float frameProgress = 1 - (particle.remainingLifetime / particle.startLifetime);
                    //                float frameProgress = textureSheetAnimation.frameOverTime.curveMin.Evaluate(1 - (particle.lifetime / particle.startLifetime)); // TODO - once Unity allows MinMaxCurve reading
                    frameProgress = Mathf.Repeat(frameProgress * textureSheetAnimation.cycleCount, 1);
                    int frame = 0;

                    switch (textureSheetAnimation.animation)
                    {

                        case ParticleSystemAnimationType.WholeSheet:
                            frame = Mathf.FloorToInt(frameProgress * textureSheetAnimationFrames);
                            break;

                        case ParticleSystemAnimationType.SingleRow:
                            frame = Mathf.FloorToInt(frameProgress * textureSheetAnimation.numTilesX);

                            int row = textureSheetAnimation.rowIndex;
                            //                    if (textureSheetAnimation.useRandomRow) { // FIXME - is this handled internally by rowIndex?
                            //                        row = Random.Range(0, textureSheetAnimation.numTilesY, using: particle.randomSeed);
                            //                    }
                            frame += row * textureSheetAnimation.numTilesX;
                            break;

                    }

                    frame %= textureSheetAnimationFrames;

                    particleUV.x = (frame % textureSheetAnimation.numTilesX) * textureSheedAnimationFrameSize.x;
                    particleUV.y = Mathf.FloorToInt(frame / textureSheetAnimation.numTilesX) * textureSheedAnimationFrameSize.y;
                    particleUV.z = particleUV.x + textureSheedAnimationFrameSize.x;
                    particleUV.w = particleUV.y + textureSheedAnimationFrameSize.y;
                }

                quad[0] = UIVertex.simpleVert;
                quad[0].color = color;
                quad[0].uv0 = new Vector2(particleUV.x, particleUV.y);

                quad[1] = UIVertex.simpleVert;
                quad[1].color = color;
                quad[1].uv0 = new Vector2(particleUV.x, particleUV.w);

                quad[2] = UIVertex.simpleVert;
                quad[2].color = color;
                quad[2].uv0 = new Vector2(particleUV.z, particleUV.w);

                quad[3] = UIVertex.simpleVert;
                quad[3].color = color;
                quad[3].uv0 = new Vector2(particleUV.z, particleUV.y);

                if (rotation == 0)
                {
                    // no rotation
                    Vector2 corner1 = new Vector2(position.x - size, position.y - size);
                    Vector2 corner2 = new Vector2(position.x + size, position.y + size);

                    quad[0].position = new Vector2(corner1.x, corner1.y);
                    quad[1].position = new Vector2(corner1.x, corner2.y);
                    quad[2].position = new Vector2(corner2.x, corner2.y);
                    quad[3].position = new Vector2(corner2.x, corner1.y);
                }
                else
                {
                    // apply rotation
                    Vector2 right = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)) * size;
                    Vector2 up = new Vector2(Mathf.Cos(rotation90), Mathf.Sin(rotation90)) * size;

                    quad[0].position = position - right - up;
                    quad[1].position = position - right + up;
                    quad[2].position = position + right + up;
                    quad[3].position = position + right - up;
                }

                vh.AddUIVertexQuad(quad);
            }
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                // unscaled animation within UI
                if (currentParticleSystem.main.playOnAwake)
                    currentParticleSystem.Simulate(Time.unscaledDeltaTime, false, false);

                SetAllDirty();
            }
        }

#if UNITY_EDITOR
        void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                SetAllDirty();
            }
        }


        protected override void OnValidate()
        {
            base.OnValidate();
            if (hiddenShader == null)
                hiddenShader = Shader.Find("SeganX/UI/Hidden");
        }
#endif
    }
}