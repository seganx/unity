using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SeganX
{
    [ExecuteAlways, RequireComponent(typeof(CanvasRenderer)), System.Obsolete("This class will be removed! Use UiParticleRenderer, UiParticleEmitter instead.")]
    public class UiSimpleParticle : MaskableGraphic
    {
        private struct Quad
        {
            public float lifetime;
            public float age;
            public float noise;
            public float size;
            public Color color;
            public Vector3 start;
            public Vector3 end;
            public Vector3 right;
            public Vector3 position;
        }

        [SerializeField] private Sprite sprite = null;
        [SerializeField, VectorLabels("Width", "Height")] private Vector2 spriteSize = Vector2.zero;
        [SerializeField, VectorLabels("Min", "Max")] private Vector2 startLife = Vector2.one;
        [SerializeField, VectorLabels("Min", "Max")] private Vector2 startSize = Vector2.one;
        [SerializeField] private Gradient colorOverLife = new Gradient();
        [SerializeField] private AnimationCurve sizeOverLife = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0) });
        [SerializeField] private AnimationCurve rotateOverLife = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) });
        [SerializeField] private AnimationCurve velocityOverLife = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
        [SerializeField] private AnimationCurve windOverLife = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) });
        [SerializeField] private AnimationCurve gravityOverLife = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) });
        [SerializeField] private AnimationCurve noiseOverLife = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) }) { preWrapMode = WrapMode.PingPong, postWrapMode = WrapMode.PingPong };


        private List<Quad> quads = new List<Quad>(128);

        public event System.Action OnQuadStop = null;
        public event System.Action OnParticleStop = null;

        public override Texture mainTexture => sprite ? sprite.texture : base.mainTexture;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var uv = sprite ? UnityEngine.Sprites.DataUtility.GetOuterUV(sprite) : Vector4.zero;
            var sw = spriteSize.x > Mathf.Epsilon ? spriteSize.x : (rectTransform.rect.width * 0.5f);
            var sh = spriteSize.y > Mathf.Epsilon ? spriteSize.y : (rectTransform.rect.height * 0.5f);

            var quad = new UIVertex[4];

            for (int i = 0; i < quads.Count; i++)
            {
                var current = quads[i];
                if (current.age < 0 || current.age > 1) continue;

                var qrotate = rotateOverLife.Evaluate(current.age) * 360;
                var qcolor = colorOverLife.Evaluate(current.age) * current.color * color;
                var qsize = sizeOverLife.Evaluate(current.age) * current.size;
                var qw = sw * qsize;
                var qh = sh * qsize;
                var sinr = Mathf.Sin(qrotate);
                var cosr = Mathf.Cos(qrotate);
                var xsin = qw * sinr;
                var xcos = qw * cosr;
                var ysin = qh * sinr;
                var ycos = qh * cosr;

                quad[0] = UIVertex.simpleVert;
                quad[0].color = qcolor;
                quad[0].position = new Vector3(-xcos + ysin, -xsin - ycos, 0) + current.position;
                quad[0].uv0 = new Vector2(uv.x, uv.y);

                quad[1] = UIVertex.simpleVert;
                quad[1].color = qcolor;
                quad[1].position = new Vector3(-xcos - ysin, -xsin + ycos, 0) + current.position;
                quad[1].uv0 = new Vector2(uv.x, uv.w);

                quad[2] = UIVertex.simpleVert;
                quad[2].color = qcolor;
                quad[2].position = new Vector3(xcos - ysin, xsin + ycos, 0) + current.position;
                quad[2].uv0 = new Vector2(uv.z, uv.w);

                quad[3] = UIVertex.simpleVert;
                quad[3].color = qcolor;
                quad[3].position = new Vector3(xcos + ysin, xsin - ycos, 0) + current.position;
                quad[3].uv0 = new Vector2(uv.z, uv.y);

                vh.AddUIVertexQuad(quad);
            }

#if UNITY_EDITOR
            if (Application.isPlaying == false && quads.Count < 1)
            {
                quad[0] = UIVertex.simpleVert;
                quad[0].color = color;
                quad[0].position = new Vector3(-sw, -sh, 0);
                quad[0].uv0 = new Vector2(uv.x, uv.y);

                quad[1] = UIVertex.simpleVert;
                quad[1].color = color;
                quad[1].position = new Vector3(-sw, sh, 0);
                quad[1].uv0 = new Vector2(uv.x, uv.w);

                quad[2] = UIVertex.simpleVert;
                quad[2].color = color;
                quad[2].position = new Vector3(sw, sh, 0);
                quad[2].uv0 = new Vector2(uv.z, uv.w);

                quad[3] = UIVertex.simpleVert;
                quad[3].color = color;
                quad[3].position = new Vector3(sw, -sh, 0);
                quad[3].uv0 = new Vector2(uv.z, uv.y);

                vh.AddUIVertexQuad(quad);
            }
#endif
        }

        protected virtual void Update()
        {
            bool isderty = false;
            var canvaseScaler = 100 / canvas.scaleFactor;

            for (int i = 0; i < quads.Count; i++)
            {
                var quad = quads[i];
                quad.age += Time.unscaledDeltaTime / quad.lifetime;

                if (quad.age <= 1)
                {
                    quad.end += gravityOverLife.Evaluate(quad.age) * canvaseScaler * Time.deltaTime * Vector3.up;
                    quad.end += windOverLife.Evaluate(quad.age) * canvaseScaler * Time.deltaTime * Vector3.right;

                    var time = velocityOverLife.Evaluate(quad.age);
                    quad.position = Vector2.LerpUnclamped(quad.start, quad.end, time);
                    quad.position += noiseOverLife.Evaluate(time) * canvaseScaler * quad.noise * quad.right;

                    quads[i] = quad;
                    isderty = true;
                }
                else
                {
                    isderty = true;
                    quads.RemoveAt(i--);
                    OnQuadStop?.Invoke();

                    if (quads.Count < 1)
                        OnParticleStop?.Invoke();
                }
            }

            if (isderty) SetAllDirty();
        }

        [System.Obsolete("This class will be removed! Use UiParticleRenderer, UiParticleEmitter instead.")]
        public void Clear()
        {
            quads.Clear();
            SetAllDirty();
        }

        [System.Obsolete("This class will be removed! Use UiParticleRenderer, UiParticleEmitter instead.")]
        public void Add(Vector2 startPoint)
        {
            quads.Add(new Quad()
            {
                lifetime = Random.Range(startLife.x, startLife.y),
                age = 0,
                noise = Random.value * 2 - 1,
                size = Random.Range(startSize.x, startSize.y),
                color = Color.white,
                start = startPoint,
                end = Vector3.zero,
                right = Vector3.Cross(Vector3.forward, startPoint).normalized,
                position = startPoint
            }); ;
        }

        [System.Obsolete("This class will be removed! Use UiParticleRenderer, UiParticleEmitter instead.")]
        public void Add(Vector2 startPoint, Vector2 endPoint, Color quadColor, float quadSize = 1)
        {
            quads.Add(new Quad()
            {
                lifetime = Random.Range(startLife.x, startLife.y),
                age = 0,
                noise = Random.value * 2 - 1,
                size = Random.Range(startSize.x, startSize.y) * quadSize,
                color = quadColor,
                start = startPoint,
                end = endPoint,
                right = Vector3.Cross(Vector3.forward, endPoint - startPoint).normalized,
                position = startPoint
            });
        }
    }
}