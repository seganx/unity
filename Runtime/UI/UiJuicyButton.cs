using UnityEngine;
using UnityEngine.UI;

namespace SeganX
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UiJuicyButton : MonoBehaviour
    {
        [SerializeField] protected Button button = null;
        [SerializeField] protected Text caption = null;
        [SerializeField] protected UiParticleEmitter[] particles = null;

        private CanvasGroup canvasGroup = null;
        private float disabledColorAlpha = 1;

        public event System.Action OnClick = null;

        protected virtual void Reset()
        {
            button = transform.GetComponent<Button>(true, true);
            caption = transform.GetComponent<Text>(true, true);
            particles = transform.GetComponentsInChildren<UiParticleEmitter>(true);
        }

        protected virtual void Awake()
        {
            canvasGroup = transform.GetComponent<CanvasGroup>(true, true);
            button.onClick.AddListener(() => OnClick?.Invoke());

            // TODO : make an extension to handle this lines of codes
            var colors = button.colors;
            var dcolor = colors.disabledColor;
            disabledColorAlpha = dcolor.a;
            dcolor.a = 1;
            colors.disabledColor = dcolor;
            button.colors = colors;
        }

        protected virtual void Start()
        {
            foreach (var particle in particles)
                particle.Play();
        }

        public virtual void SetText(string text)
        {
            caption.text = text;
        }

        public virtual void SetInteractable(bool value)
        {
            if (value)
                foreach (var particle in particles)
                    particle.Play();
            else
                foreach (var particle in particles)
                    particle.Stop(false);

            canvasGroup.blocksRaycasts = value;
            canvasGroup.alpha = value ? 1 : disabledColorAlpha;
        }

        public virtual void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}