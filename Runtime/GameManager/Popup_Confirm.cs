using SeganX.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SeganX
{
    public class Popup_Confirm : GameState
    {
        [SerializeField] private LocalText descLabel = null;
        [SerializeField] private Button okButton = null;
        [SerializeField] private Button cancelButton = null;
        [SerializeField] private Transform decoration = null;

        private System.Action<bool> callbackFunc = null;

        public Popup_Confirm Setup(string desc, string okTitle, string cancelTitle, System.Action<bool> callback)
        {
            return Setup(okTitle, cancelTitle, callback).SetText(desc);
        }

        public Popup_Confirm Setup(string desc, bool displayOkButton, bool displayCancelButton, System.Action<bool> callback)
        {
            return Setup(displayOkButton ? "OK" : null, displayCancelButton ? "Cancel" : null, callback).SetText(desc);
        }

        public Popup_Confirm SetupByKey(string stringKey, string okTitle, string cancelTitle, System.Action<bool> callback)
        {
            return Setup(LocalKit.Get(stringKey), okTitle, cancelTitle, callback);
        }

        public Popup_Confirm SetupByKey(string stringKey, bool displayOkButton, bool displayCancelButton, System.Action<bool> callback)
        {
            return Setup(LocalKit.Get(stringKey), displayOkButton, displayCancelButton, callback);
        }

        public Popup_Confirm Setup(string okTitle, string cancelTitle, System.Action<bool> callback)
        {
            callbackFunc = callback;
            okButton.onClick.AddListener(() => Close(true));
            cancelButton.onClick.AddListener(() => Close(false));

            if (okTitle.HasContent(1))
            {
                okButton.gameObject.SetActive(true);
                okButton.SetText(okTitle);
            }
            else okButton.gameObject.SetActive(false);

            if (cancelTitle.HasContent(1))
            {
                cancelButton.gameObject.SetActive(true);
                cancelButton.SetText(cancelTitle);
            }
            else cancelButton.gameObject.SetActive(false);

            return this;
        }

        public Popup_Confirm SetTextByKey(string key, params object[] args)
        {
            return SetText(LocalKit.Get(key), args);
        }

        public Popup_Confirm SetText(string desc, params object[] args)
        {
            var str = args == null ? desc : string.Format(desc, args);
            descLabel.SetText(str);
            return this;
        }

        public Popup_Confirm SetDecoration(int index)
        {
            if (decoration)
                decoration.SetActiveChild(index);
            return this;
        }

        private void Start()
        {
            UiShowHide.ShowAll(transform);
        }

        public override void Back()
        {
            Close(false);
        }

        public void Close(bool isok)
        {
            base.Back();
            callbackFunc?.Invoke(isok);
        }
    }
}