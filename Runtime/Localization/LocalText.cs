using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Localization
{
    [DefaultExecutionOrder(-2)]
    public abstract class LocalText : Base
    {
        [SerializeField] protected bool autoRtl = false;
        [SerializeField] protected bool autoWidth = false;
        [SerializeField] protected bool autoHeight = false;
        [SerializeField] protected string stringKey = string.Empty;
        [SerializeField, TextArea] protected string textbase = string.Empty;

        protected object[] localargs = null;

        public abstract void UpdateRectSize();

        protected virtual void OnEnable()
        {
            if (stringKey.HasContent())
                textbase = LocalKit.Get(stringKey);
            DisplayText();
        }

        protected virtual void OnRectTransformDimensionsChange()
        {
            DisplayText();
        }

        protected virtual void LateUpdate()
        {
            UpdateRectSize();
            enabled = false;
        }

        protected virtual void DisplayText()
        {
            enabled = true;
        }

        public virtual void SetText(string text)
        {
            localargs = null;
            if (textbase == text) return;
            textbase = text;
            DisplayText();
        }

        public virtual void SetFormatedText(params object[] args)
        {
            localargs = args;
            if (stringKey.HasContent())
                textbase = LocalKit.Get(stringKey);
            DisplayText();
        }

        private void OnLanguageChanged()
        {
            if (stringKey.IsNullOrEmpty()) return;
            if (localargs == null)
            {
                textbase = LocalKit.Get(stringKey);
                DisplayText();
            }
            else SetFormatedText(localargs);
        }


        //////////////////////////////////////////////////////////
        //  STATIC MEMEBRS
        //////////////////////////////////////////////////////////
        public static void LanguageChanged()
        {
            var locals = FindObjectsOfType<LocalText>();
            foreach (var item in locals)
                item.OnLanguageChanged();
        }

#if UNITY_EDITOR
        //////////////////////////////////////////////////////
        /// EDITOR HELPER CLASS
        //////////////////////////////////////////////////////
        public static class Editor
        {
            public static void DisplayText(LocalText target)
            {
                target.DisplayText();
            }
        }
#endif
    }
}
