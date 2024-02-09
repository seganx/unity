using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SeganX
{
    [ExecuteAlways, RequireComponent(typeof(CanvasRenderer))]
    public class UiParticleRenderer : MaskableGraphic
    {
        private enum MovementMode { Free, GotoEnd }

        private struct Particle
        {
            public float lifetime;
            public float age;           // is always between 0 and 1
            public float size;
            public float roation;       // start rotation angle in radian
            public Color color;
            public Vector3 startPoint;
            public Vector3 endPoint;
            public Vector3 startSpeed;
            public Vector3 position;
            public Vector3 velocity;
            public Vector4 noise;       // size, rotation, color, position
            public MovementMode movementMode;
        }

        [SerializeField] private Sprite sprite = null;
        [SerializeField, VectorLabels("Width ", "Height ")] private Vector2 spriteSize = Vector2.zero;
        [SerializeField] private ParticleSystem.MinMaxGradient colorOverLife = new ParticleSystem.MinMaxGradient(Color.white);
        [SerializeField] private ParticleSystem.MinMaxCurve sizeOverLife = new ParticleSystem.MinMaxCurve(1);
        [SerializeField] private ParticleSystem.MinMaxCurve rotateOverLife = new ParticleSystem.MinMaxCurve(0);
        [SerializeField] private ParticleSystem.MinMaxCurve velocityOverLife = new ParticleSystem.MinMaxCurve(1);
        [SerializeField] private ParticleSystem.MinMaxCurve windOverLife = new ParticleSystem.MinMaxCurve(0);
        [SerializeField] private ParticleSystem.MinMaxCurve gravityOverLife = new ParticleSystem.MinMaxCurve(0);
        [SerializeField] private ParticleSystem.MinMaxCurve noiseAmplitude = new ParticleSystem.MinMaxCurve(1, new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0)));
        [SerializeField] private ParticleSystem.MinMaxCurve noiseFrequency = new ParticleSystem.MinMaxCurve(0);
        [SerializeField] private ParticleSystem.MinMaxCurve destinationFactor = new ParticleSystem.MinMaxCurve(1, new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0)));

        private readonly List<Particle> particles = new List<Particle>(128);

        public UnityEvent OnParticleDead = new UnityEvent();
        public UnityEvent OnFinished = new UnityEvent();

        public override Texture mainTexture => sprite ? sprite.texture : base.mainTexture;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var uv = sprite ? UnityEngine.Sprites.DataUtility.GetOuterUV(sprite) : Vector4.zero;
            var sw = spriteSize.x > Mathf.Epsilon ? spriteSize.x : (rectTransform.rect.width * 0.5f);
            var sh = spriteSize.y > Mathf.Epsilon ? spriteSize.y : (rectTransform.rect.height * 0.5f);

            var vertex = new UIVertex[4];

            for (int i = 0; i < particles.Count; i++)
            {
                var particle = particles[i];
                if (particle.age < 0 || particle.age > 1) continue;

                var position = ComputeQuadPosition(ref particle);

                var qsize = sizeOverLife.Evaluate(particle.age, particle.noise.x) * particle.size;
                var qrotate = rotateOverLife.Evaluate(particle.age, particle.noise.y) * Mathf.Deg2Rad + particle.roation;
                var qcolor = colorOverLife.Evaluate(particle.age, particle.noise.z) * particle.color * color;

                var qw = sw * qsize;
                var qh = sh * qsize;
                var sinr = Mathf.Sin(qrotate);
                var cosr = Mathf.Cos(qrotate);
                var xsin = qw * sinr;
                var xcos = qw * cosr;
                var ysin = qh * sinr;
                var ycos = qh * cosr;

                vertex[0] = UIVertex.simpleVert;
                vertex[0].color = qcolor;
                vertex[0].position = new Vector3(-xcos + ysin, -xsin - ycos, 0) + position;
                vertex[0].uv0 = new Vector2(uv.x, uv.y);

                vertex[1] = UIVertex.simpleVert;
                vertex[1].color = qcolor;
                vertex[1].position = new Vector3(-xcos - ysin, -xsin + ycos, 0) + position;
                vertex[1].uv0 = new Vector2(uv.x, uv.w);

                vertex[2] = UIVertex.simpleVert;
                vertex[2].color = qcolor;
                vertex[2].position = new Vector3(xcos - ysin, xsin + ycos, 0) + position;
                vertex[2].uv0 = new Vector2(uv.z, uv.w);

                vertex[3] = UIVertex.simpleVert;
                vertex[3].color = qcolor;
                vertex[3].position = new Vector3(xcos + ysin, xsin - ycos, 0) + position;
                vertex[3].uv0 = new Vector2(uv.z, uv.y);

                vh.AddUIVertexQuad(vertex);
                particles[i] = particle;
            }

#if UNITY_EDITOR
            if (Application.isPlaying == false && particles.Count < 1)
            {
                vertex[0] = UIVertex.simpleVert;
                vertex[0].color = color;
                vertex[0].position = new Vector3(-sw, -sh, 0);
                vertex[0].uv0 = new Vector2(uv.x, uv.y);

                vertex[1] = UIVertex.simpleVert;
                vertex[1].color = color;
                vertex[1].position = new Vector3(-sw, sh, 0);
                vertex[1].uv0 = new Vector2(uv.x, uv.w);

                vertex[2] = UIVertex.simpleVert;
                vertex[2].color = color;
                vertex[2].position = new Vector3(sw, sh, 0);
                vertex[2].uv0 = new Vector2(uv.z, uv.w);

                vertex[3] = UIVertex.simpleVert;
                vertex[3].color = color;
                vertex[3].position = new Vector3(sw, -sh, 0);
                vertex[3].uv0 = new Vector2(uv.z, uv.y);

                vh.AddUIVertexQuad(vertex);
            }
#endif
        }

        protected virtual void Update()
        {
            bool isderty = false;

            for (int i = 0; i < particles.Count; i++)
            {
                var particle = particles[i];
                particle.age += Time.unscaledDeltaTime / particle.lifetime;

                if (particle.age <= 1)
                {
                    particles[i] = particle;
                    isderty = true;
                }
                else
                {
                    isderty = true;
                    particles.RemoveAt(i--);
                    OnParticleDead?.Invoke();

                    if (particles.Count < 1)
                        OnFinished?.Invoke();
                }
            }

            if (isderty) SetAllDirty();
        }

        private Vector3 ComputeQuadPosition(ref Particle particle)
        {
            Vector3 finalPosition;
            const float canvaseScaler = 10;

            var quadPosition = particle.position;
            quadPosition += Time.unscaledDeltaTime * velocityOverLife.Evaluate(particle.age, particle.noise.w) * particle.startSpeed;
            quadPosition += Time.unscaledDeltaTime * windOverLife.Evaluate(particle.age, particle.noise.w) * canvaseScaler * Vector3.right;
            quadPosition -= Time.unscaledDeltaTime * gravityOverLife.Evaluate(particle.age, particle.noise.w) * canvaseScaler * Vector3.down;

            if (particle.movementMode == MovementMode.GotoEnd)
            {
                var endpointTime = destinationFactor.Evaluate(particle.age, particle.noise.w);
                var lifePosition = Vector2.Lerp(particle.startPoint, particle.endPoint, endpointTime);
                finalPosition = Vector3.Lerp(quadPosition, lifePosition, endpointTime);
            }
            else finalPosition = quadPosition;

            particle.velocity = Vector3.Lerp(particle.velocity, (finalPosition - particle.position), Time.deltaTime * 2);
            particle.position = quadPosition;

            // add perlin noise on movement
            var noiseAmp = noiseAmplitude.Evaluate(particle.age, particle.noise.w) * particle.velocity.magnitude;
            var noiseFre = noiseFrequency.Evaluate(particle.age, particle.noise.w);
            var perlin = Mathf.PerlinNoise(particle.age * noiseFre, particle.noise.w * 10) * 2 - 1;
            var right = Vector3.Cross(particle.velocity.normalized, Vector3.forward).normalized;
            var noiseOffset = noiseAmp * perlin * canvaseScaler * right;

            return finalPosition + noiseOffset;

        }

        public void Clear()
        {
            particles.Clear();
            SetAllDirty();
        }

        /// <summary>
        /// Add a quad to the particle renderer.
        /// </summary>
        /// <param name="lifetime">life time in seconds</param>
        /// <param name="size">size factor. default = 1</param>
        /// <param name="rotation">rotation arround Z axis in radian</param>
        /// <param name="color">initial color</param>
        /// <param name="startPoint">start position in the current rectangle transform</param>
        /// <param name="velocity">initial velocity</param>
        public void Add(float lifetime, float size, float rotation, Color color, Vector2 startPoint, Vector2 velocity)
        {
            particles.Add(new Particle()
            {
                lifetime = lifetime,
                age = 0,
                noise = new Vector4(Random.value, Random.value, Random.value, Random.value),
                size = size,
                roation = rotation,
                color = color,
                position = startPoint,
                startSpeed = velocity,
                movementMode = MovementMode.Free
            });
        }

        /// <summary>
        /// Add a quad to the particle and define a destination point for it.
        /// </summary>
        /// <param name="lifetime">life time in seconds</param>
        /// <param name="size">size factor. default = 1</param>
        /// <param name="rotation">rotation arround Z axis in radian</param>
        /// <param name="color">initial color</param>
        /// <param name="startPoint">start position in the current rectangle transform</param>
        /// <param name="velocity">initial velocity</param>
        /// <param name="endPoint">final destination of the quad to go to</param>
        public void Add(float lifetime, float size, float rotation, Color color, Vector2 startPoint, Vector2 velocity, Vector3 endPoint)
        {
            particles.Add(new Particle()
            {
                lifetime = lifetime,
                age = 0,
                noise = new Vector4(Random.value, Random.value, Random.value, Random.value),
                size = size,
                roation = rotation,
                color = color,
                startPoint = startPoint,
                endPoint = endPoint,
                position = startPoint,
                startSpeed = velocity,
                movementMode = MovementMode.GotoEnd
            });
        }

    }

}