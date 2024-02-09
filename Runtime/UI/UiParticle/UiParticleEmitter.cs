using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    [ExecuteAlways]
    public class UiParticleEmitter : MonoBehaviour
    {
        public enum EmitShape { Rectangle, Circle }
        public enum EmitFrom { Volume, Eadge }

        [System.Serializable]
        public class OverrideDirection
        {
            public bool active = false;
            public ParticleSystem.MinMaxCurve x = new ParticleSystem.MinMaxCurve(-1, 1);
            public ParticleSystem.MinMaxCurve y = new ParticleSystem.MinMaxCurve(-1, 1);
        }

        public UiParticleRenderer particleRenderer = null;
        public bool playOnAwake = true;
        public float duration = 5;
        public bool loop = true;
        public ParticleSystem.MinMaxCurve startLifetime = new ParticleSystem.MinMaxCurve(2);
        public ParticleSystem.MinMaxGradient startColor = new ParticleSystem.MinMaxGradient(Color.white);
        public ParticleSystem.MinMaxCurve startSize = new ParticleSystem.MinMaxCurve(1);
        public ParticleSystem.MinMaxCurve startRotation = new ParticleSystem.MinMaxCurve(0);
        public ParticleSystem.MinMaxCurve startSpeed = new ParticleSystem.MinMaxCurve(50);
        public float rateOverTime = 10;
        [VectorLabels("Time", "Min", "Max")] public List<Vector3> bursts = new List<Vector3>();
        public EmitShape emitShape = EmitShape.Rectangle;
        public Rect emitShapeRect = new Rect(0, 0, 1, 1);
        [VectorLabels("X", "Y", "Radius")] public Vector3 emitShapeCircle = new Vector3(0, 0, 1);
        public EmitFrom emitFrom = EmitFrom.Volume;
        public OverrideDirection overrideDirection = new OverrideDirection();


        private RectTransform rectTransform = null;
        private bool playing = false;
        private Utils.StopWatch playTimer;
        private Utils.StopWatch rateTimer;
        private Utils.StopWatch burstTimer;
        private int burstIndex = 0;
        private Vector4 endPoint = Vector4.zero;

        public bool IsPlaying
        {
            get => playing;
            set
            {
                if (value)
                    ResetAndPlay();
                else
                    Stop();
            }
        }

        protected virtual void Start()
        {
            rectTransform = transform as RectTransform;

            if (particleRenderer == null)
                particleRenderer = transform.GetComponent<UiParticleRenderer>(true, true);

            if (playOnAwake && Application.isPlaying)
                Play();
        }

        protected virtual void Update()
        {
            if (playing == false) return;

            if (playTimer.UnscaledTimer > duration)
            {
                if (loop)
                    ResetAndPlay();
                else
                    Stop();
            }

            var rectsize = rectTransform.rect.size;
            var delayTime = rateOverTime > 0 ? 1 / rateOverTime : 0;
            if (delayTime > Mathf.Epsilon && rateTimer.UnscaledTimer > delayTime)
            {
                Emit(rectsize);
                rateTimer.Reset();
            }

            if (burstIndex < bursts.Count)
            {
                if (burstTimer.UnscaledTimer >= bursts[burstIndex].x)
                {
                    var count = Random.Range(bursts[burstIndex].y, bursts[burstIndex].z);
                    for (int i = 0; i < count; i++)
                        Emit(rectsize);
                    burstIndex++;
                }
            }
        }

        private void Emit(Vector2 rectsize)
        {
            Vector2 center = Vector2.zero, point = Vector2.zero, direction;
            var speed = startSpeed.Evaluate(playTimer.UnscaledTimer, Random.value);

            switch (emitShape)
            {
                case EmitShape.Rectangle:
                    center = emitShapeRect.position;
                    switch (emitFrom)
                    {
                        case EmitFrom.Volume: point = (0.5F * rectsize * emitShapeRect.size).Randomize(); break;
                        case EmitFrom.Eadge: point = (rectsize * emitShapeRect.size).RandomOnRectEdge(); break;
                    }
                    break;
                case EmitShape.Circle:
                    center = emitShapeCircle;
                    switch (emitFrom)
                    {
                        case EmitFrom.Volume: point = 0.5f * emitShapeCircle.z * rectsize * Random.insideUnitCircle; break;
                        case EmitFrom.Eadge: point = 0.5f * emitShapeCircle.z * rectsize * Random.insideUnitCircle.normalized; break;
                    }
                    break;
            }

            if (overrideDirection.active)
            {
                direction.x = overrideDirection.x.Evaluate(playTimer.UnscaledTimer, Random.value);
                direction.y = overrideDirection.y.Evaluate(playTimer.UnscaledTimer, Random.value);
            }
            else direction = point - center;

            if (endPoint.w < 0.5f)
                particleRenderer.Add(
                    startLifetime.Evaluate(playTimer.UnscaledTimer, Random.value),
                    startSize.Evaluate(playTimer.UnscaledTimer, Random.value),
                    startRotation.Evaluate(playTimer.UnscaledTimer, Random.value),
                    startColor.Evaluate(Random.value, Random.value),
                    center + point,
                    speed * direction.normalized);
            else
                particleRenderer.Add(
                    startLifetime.Evaluate(playTimer.UnscaledTimer, Random.value),
                    startSize.Evaluate(playTimer.UnscaledTimer, Random.value),
                    startRotation.Evaluate(playTimer.UnscaledTimer, Random.value),
                    startColor.Evaluate(Random.value, Random.value),
                    center + point,
                    speed * direction.normalized,
                    endPoint);
        }

        private void ResetAndPlay()
        {
            playTimer = Utils.StopWatch.Create();
            rateTimer = Utils.StopWatch.Create(-99999);
            burstTimer = Utils.StopWatch.Create();
            burstIndex = 0;
            playing = true;
        }

        public void Play()
        {
            endPoint = Vector4.zero;
            ResetAndPlay();
        }

        public void Play(Vector3 endPoint)
        {
            this.endPoint = new Vector4(endPoint.x, endPoint.y, endPoint.z, 1);
            ResetAndPlay();
        }

        public void Stop(bool clearAll = false)
        {
            playing = false;
            if (clearAll && particleRenderer)
                particleRenderer.Clear();
        }

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            if (particleRenderer == null)
                particleRenderer = transform.GetComponent<UiParticleRenderer>(true, true);
        }


        protected virtual void OnValidate()
        {
            if (particleRenderer == null)
                particleRenderer = transform.GetComponent<UiParticleRenderer>(true, true);

            for (int b = 0; b < bursts.Count; b++)
            {
                var burst = bursts[b];
                burst.y = Mathf.RoundToInt(burst.y);
                burst.z = Mathf.RoundToInt(burst.z);
                bursts[b] = burst;
            }
        }
#endif
    }
}