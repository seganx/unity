using UnityEngine;
using UnityEngine.UI;

namespace SeganX
{
    public static class ButtonEx
    {
        public static Button SetInteractable(this Button self, bool interactable)
        {
            if (self == null) return self;
            var group = self.GetComponent<CanvasGroup>();
            if (group == null) group = self.gameObject.AddComponent<CanvasGroup>();
            group.alpha = interactable ? self.colors.normalColor.a : self.colors.disabledColor.a;
            self.interactable = interactable;
            return self;
        }

        public static Button SetText(this Button self, string text)
        {
            if (self == null) return self;

#if SX_PARSI
            var localtext = self.GetComponent<LocalText>(true, false);
            if (localtext != null)
            {
                localtext.SetText(text);
                return self;
            }
#endif

            var view = self.GetComponent<Text>(true, false);
            if (view != null)
            {
                view.text = text;
                return self;
            }

            return self;
        }
    }
}