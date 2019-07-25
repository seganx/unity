using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SeganX
{
    [ExecuteInEditMode]
    [AddComponentMenu("UI/SeganX/LocalText")]
    public class LocalText : Base
    {
        public Text target = null;
        public bool autoRtl = false;
        public bool autoWidth = false;
        public bool autoHeight = false;
        public int stringId = 0;
        public string currnetText = string.Empty;

        private object[] localargs = null;

        public void SetText(string text)
        {
            localargs = null;
            if (currnetText == text) return;
            currnetText = text;
            DisplayText();
            CheckSize();
        }

        public void SetFormatedText(params object[] args)
        {
            localargs = args;
            if (stringId > 0) currnetText = LocalizationService.Get(stringId);
            DisplayText();
            CheckSize();
        }

        private void DisplayText()
        {
            if (localargs == null)
                target.SetTextAndWrap(currnetText, autoRtl, LocalizationService.IsPersian);
            else
                target.SetTextAndWrap(string.Format(currnetText, localargs), autoRtl, LocalizationService.IsPersian);
        }

        private void CheckSize()
        {
            if (autoWidth)
            {
                var widthOffset = rectTransform.rect.width - target.rectTransform.rect.width;
                var widthAnchor = target.rectTransform.anchorMax.x - target.rectTransform.anchorMin.x;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, target.preferredWidth + widthOffset * widthAnchor);
            }

            if (autoHeight)
            {
                var heightOffset = rectTransform.rect.height - target.rectTransform.rect.height;
                var heightAnchor = target.rectTransform.anchorMax.y - target.rectTransform.anchorMin.y;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, target.preferredHeight + heightOffset * heightAnchor);
            }
        }

        private void OnLanguageChanged()
        {
            if (stringId == 0) return;
            if (localargs == null)
            {
                currnetText = LocalizationService.Get(stringId);
                DisplayText();
                CheckSize();
            }
            else SetFormatedText(localargs);
        }

        private void Awake()
        {
            all.Add(this);
            if (stringId > 0) currnetText = LocalizationService.Get(stringId);
        }

        private void OnDestroy()
        {
            all.Remove(this);
        }

        private void Start()
        {
            DisplayText();
            CheckSize();
        }

        private void OnRectTransformDimensionsChange()
        {
            DisplayText();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (UnityEditor.EditorApplication.isPlaying == false)
            {
                if (target == null) target = transform.GetComponent<Text>(true, true);
                DisplayText();
                //CheckSize();
            }
        }
#endif


        //////////////////////////////////////////////////////////
        //  STATIC MEMEBRS
        //////////////////////////////////////////////////////////
        public static List<LocalText> all = new List<LocalText>();

        public static void LanguageChanged()
        {
            foreach (var item in all)
                item.OnLanguageChanged();
        }

#if UNITY_EDITOR
        public static void SetStringId(LocalText local, int stringId)
        {
            local.stringId = stringId;
            if (local.stringId > 0)
                local.currnetText = LocalizationService.Get(stringId);
            local.DisplayText();
            local.CheckSize();
        }
#endif
    }
}
