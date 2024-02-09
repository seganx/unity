using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    [ExecuteAlways, System.Obsolete("This class will be removed! Use UiParticleRenderer, UiParticleEmitter instead.")]
    public class UiParticle : UiSimpleParticle
    {
        public enum EmitFrom { Volume, Eadge }

        public bool playOnAwake = true;
        public Gradient startColor = new Gradient();
        public EmitFrom emitFrom = EmitFrom.Volume;
        [VectorLabels("Width", "Height")]
        public Vector2 insideScale = new Vector2(0.5f, 0.5f);
        [VectorLabels("Width", "Height")]
        public Vector2 outsideScale = new Vector2(1.5f, 1.5f);
        public float rateOverTime = 10;
        [VectorLabels("Time", "Min", "Max")]
        public List<Vector3> bursts = new List<Vector3>();

        private bool playing = false;
        private Utils.StopWatch rateTimer;
        private Utils.StopWatch burstTimer;
        private int burstIndex = 0;

        public bool IsPlaying
        {
            get => playing;
            set
            {
                if (value)
                    Play();
                else
                    Stop();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (playOnAwake && Application.isPlaying)
                Play();
        }

        protected override void Update()
        {
            if (playing)
            {
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

            base.Update();
        }

        private void Emit(Vector2 rectsize)
        {
            Vector2 dir = Vector2.one;
            switch (emitFrom)
            {
                case EmitFrom.Volume: dir = rectsize.Randomize() * 0.5f; break;
                case EmitFrom.Eadge: dir = MathEx.RandomOnRectEdge(rectsize.x, rectsize.y); break;
            }
            var startPoint = new Vector2(dir.x * insideScale.x, dir.y * insideScale.y);
            var endPoint = new Vector2(dir.x * outsideScale.x, dir.y * outsideScale.y);
            Add(startPoint, endPoint, startColor.Evaluate(Random.value));
        }

        [System.Obsolete("This class will be removed! Use UiParticleRenderer, UiParticleEmitter instead.")]
        public void Play()
        {
            playing = true;
            rateTimer = Utils.StopWatch.Create();
            burstTimer = Utils.StopWatch.Create();
            burstIndex = 0;
        }

        [System.Obsolete("This class will be removed! Use UiParticleRenderer, UiParticleEmitter instead.")]
        public void Stop(bool clearAll = false)
        {
            playing = false;
            if (clearAll) Clear();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

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
