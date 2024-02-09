using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UiShowHide : Base
    {
        private enum AutoShow { Off, This, ThisAndChildren }

        private struct State
        {
            public float alpha;
            public Vector3 position;
            public Vector3 scale;
        }

        [System.Serializable]
        public class Config
        {
            public float delay = 0;
            public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
            public Vector3 direction = Vector3.zero;
            public Vector3 scale = Vector3.one;
            public float alpha = 0;
        }

        [SerializeField] private AutoShow autoShow = AutoShow.Off;
        [SerializeField] private int startConfigId = 0;
        [SerializeField] private Config startConfig = new Config();

        [SerializeField] private int hideConfigId = 0;
        [SerializeField] private Config hideConfig = new Config();

        private float timer = 1;
        private float delayTime = 0;
        private bool isShowing = false;
        private AnimationCurve curve = null;
        private State initiate = new State();
        private State current = new State();
        private State destination = new State();
        private System.Action showhideFunc = null;
        private System.Action endFunc = null;
        private CanvasGroup canvasGroup = null;
        private Animator animator = null;

        public override bool Visible
        {
            get
            {
                return canvasGroup.interactable;
            }

            set
            {
                if (value)
                    Show();
                else
                    Hide();
            }
        }

        private void OnEnable()
        {
            all.Add(this);
        }

        private void OnDisable()
        {
            all.Remove(this);
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            canvasGroup = GetComponent<CanvasGroup>();

            endFunc = () => { if (animator) animator.enabled = true; };

            GetConfigById(startConfig, startConfigId);

            current = initiate = CaptureState();

#if UNITY_EDITOR
            if (UiShowHideConfig.IsActive)
            {
                destination.position = initiate.position - startConfig.direction * 600;
                destination.scale = startConfig.scale;
                destination.alpha = startConfig.alpha;
                canvasGroup.blocksRaycasts = false;
            }
            else destination = current;
#else
            destination.position = initiate.position - startConfig.direction * 600;
            destination.scale = startConfig.scale;
            destination.alpha = startConfig.alpha;
            canvasGroup.blocksRaycasts = false;
#endif

            curve = startConfig.curve;
            PerformState();
        }

        private void Start()
        {
            switch (autoShow)
            {
                case AutoShow.This: Show(); break;
                case AutoShow.ThisAndChildren: ShowAll(transform); break;
            }
        }

        private void Update()
        {
            if (timer < 1)
            {
                timer += Time.unscaledDeltaTime;
                PerformState();
                if (timer >= 1 && endFunc != null)
                {
                    endFunc();
                    endFunc = null;
                }
            }

            if (showhideFunc != null)
            {
                delayTime -= Time.unscaledDeltaTime;
                if (delayTime <= 0)
                {
                    showhideFunc();
                    showhideFunc = null;
                }
            }
        }



        private State CaptureState()
        {
            State res;
            res.position = rectTransform.anchoredPosition;
            res.scale = rectTransform.localScale;
            res.alpha = canvasGroup.alpha;
            return res;
        }

        private void PerformState()
        {
            var t = curve.Evaluate(Mathf.Clamp01(timer));
#if UNITY_EDITOR
            if (UiShowHideConfig.IsActive)
            {
                rectTransform.anchoredPosition = Vector3.LerpUnclamped(current.position, destination.position, t);
                rectTransform.localScale = Vector3.LerpUnclamped(current.scale, destination.scale, t);
                canvasGroup.alpha = Mathf.Lerp(current.alpha, destination.alpha, t);
            }
#else
                rectTransform.anchoredPosition = Vector3.LerpUnclamped(current.position, destination.position, t);
                rectTransform.localScale = Vector3.LerpUnclamped(current.scale, destination.scale, t);
                canvasGroup.alpha = Mathf.Lerp(current.alpha, destination.alpha, t);
#endif
        }

        public void Show(float additionalDelay = 0)
        {
            if (isShowing) return;
            isShowing = true;

            delayTime = GetConfigById(startConfig, startConfigId) + additionalDelay;

            showhideFunc = () =>
            {
                canvasGroup.blocksRaycasts = true;
                current.position = initiate.position - startConfig.direction * 600;
                current.scale = startConfig.scale;
                current.alpha = startConfig.alpha;
                destination = initiate;
                curve = startConfig.curve;
                timer = 0;

                endFunc = () => { if (animator) animator.enabled = true; };
            };
        }

        public float Hide(float additionalDelay = 0)
        {
            if (isShowing == false) return 0;
            isShowing = false;

            delayTime = GetConfigById(hideConfig, hideConfigId) + additionalDelay;

            if (animator) animator.enabled = false;
            canvasGroup.blocksRaycasts = false;
            showhideFunc = () =>
            {
                current = CaptureState();
                destination.position = initiate.position + hideConfig.direction * 600;
                destination.scale = hideConfig.scale;
                destination.alpha = hideConfig.alpha;
                curve = hideConfig.curve;
                timer = 0;

                endFunc = () => { };
            };

            return delayTime + 0.5f;
        }

        ////////////////////////////////////////////////////////
        /// STATIC MEMBER
        ////////////////////////////////////////////////////////
        public static List<UiShowHide> all = new List<UiShowHide>();

        public static void SetVisible(Transform parent, bool visible, float additionalDelay = 0)
        {
            if (visible)
                ShowAll(parent, additionalDelay);
            else
                HideAll(parent, additionalDelay);
        }

        public static void ShowAll(Transform parent, float additionalDelay = 0)
        {
            foreach (var item in all)
                if (item.transform.IsChildOf(parent))
                    item.Show(additionalDelay);
        }

        public static float HideAll(Transform parent, float additionalDelay = 0)
        {
            float res = 0;
            foreach (var item in all)
                if (item.transform.IsChildOf(parent))
                {
                    var hidetime = item.Hide(additionalDelay);
                    if (hidetime > res)
                        res = hidetime;
                }
            return res;
        }

        // copy preset config to dest and return new delay time
        private static float GetConfigById(Config dest, int id)
        {
            var config = UiShowHideConfig.GetStateById(id);
            if (config == null) return dest.delay;
            dest.curve = config.curve;
            dest.direction = config.direction;
            dest.scale = config.scale;
            dest.alpha = config.alpha;
            return dest.delay + config.delay;
        }
    }
}