using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SeganX.Localization
{
    [DefaultExecutionOrder(-2)]
    [AddComponentMenu("UI/SeganX/LocalTextUi")]
    public class LocalTextUi : LocalText
    {
        [SerializeField] protected Text displayText = null;

        private TextGenerationSettings settings;
        private readonly TextGenerator generator = new TextGenerator();
        private readonly List<string> linesCache = new List<string>(50);

        protected override void DisplayText()
        {
            if (displayText == null) return;
            if (settings.font == null)
                settings = displayText.GetGenerationSettings(displayText.rectTransform.rect.size);
            if (settings.font == null) return;

            SetTextAndWrap(localargs == null ? textbase : string.Format(textbase, localargs));
            base.DisplayText();
        }

        public override void UpdateRectSize()
        {
            if (displayText == null) return;

            if (autoWidth)
            {
                var widthOffset = rectTransform.rect.width - displayText.rectTransform.rect.width;
                var widthAnchor = displayText.rectTransform.anchorMax.x - displayText.rectTransform.anchorMin.x;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, displayText.preferredWidth + widthOffset * widthAnchor);
            }

            if (autoHeight)
            {
                var heightOffset = rectTransform.rect.height - displayText.rectTransform.rect.height;
                var heightAnchor = displayText.rectTransform.anchorMax.y - displayText.rectTransform.anchorMin.y;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, displayText.preferredHeight + heightOffset * heightAnchor);
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (displayText == null)
                displayText = transform.GetComponent<Text>(true, true);
        }
#endif

        private void SetTextAndWrap(string text)
        {
            if (text.IsNullOrEmpty())
            {
                displayText.text = text;
                return;
            }

            if (autoRtl)
                displayText.FitAlignment(LocalKit.IsRtl);

            if (LocalKit.IsRtl)
                text = PersianTextUI.ShapeText(text);

            if (displayText.horizontalOverflow == HorizontalWrapMode.Wrap && LocalKit.IsRtl)
            {
                if (displayText.rectTransform.rect.width > 0)
                {
                    var lines = text.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                        lines[i] = WrapLine(generator, lines[i], settings);
                    displayText.text = string.Join("\n", lines);
                }
                else displayText.text = text;
            }
            else displayText.text = text;
        }

        private string WrapLine(TextGenerator self, string line, TextGenerationSettings settings)
        {
            string str = string.Empty;
            float lineWidth = 0;
            float speceWidth = self.GetPreferredWidth(" ", settings);
            float maxWidth = (settings.generationExtents.x > 20 ? settings.generationExtents.x : 99999) * settings.scaleFactor;
            var words = line.Split(' ');
            linesCache.Clear();
            for (int i = words.Length - 1; i >= 0; --i)
            {
                lineWidth += self.GetPreferredWidth(words[i], settings) + speceWidth;
                if (lineWidth > maxWidth && str.Length > 0)
                {
                    lineWidth = 0;
                    linesCache.Add(str);
                    str = string.Empty;
                    i++;
                }
                else if (str.Length > 0)
                    str = words[i] + " " + str;
                else
                    str = words[i];
            }
            if (str.Length > 0) linesCache.Add(str);
            return string.Join("\n", linesCache.ToArray());
        }
    }
}
