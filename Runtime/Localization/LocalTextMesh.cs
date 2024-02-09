using UnityEngine;

namespace SeganX.Localization
{
    [DefaultExecutionOrder(-2)]
    [AddComponentMenu("UI/SeganX/LocalTextMesh")]
    public class LocalTextMesh : LocalText
    {
        [SerializeField] private TextMesh displayText = null;

        protected override void DisplayText()
        {
            if (displayText == null) return;

            if (autoRtl)
                displayText.FitAlignment(LocalKit.IsRtl);

            if (localargs == null)
                displayText.text = LocalKit.IsRtl ? PersianTextUI.ShapeText(textbase) : textbase;
            else
                displayText.text = LocalKit.IsRtl ? PersianTextUI.ShapeText(string.Format(textbase, localargs)) : string.Format(textbase, localargs);
        }

        public override void UpdateRectSize()
        {

        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (displayText == null)
                displayText = transform.GetComponent<TextMesh>(true, true);
        }
#endif
    }
}