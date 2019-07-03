using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Confirm : GameState
{
    [SerializeField] private float minHeight = 200;
    [SerializeField] private float maxHeight = 450;
    [SerializeField] private float buttonSize = 55;
    [SerializeField] private RectTransform panel = null;
    [SerializeField] private Text descLabel = null;
    [SerializeField] private Button okButton = null;
    [SerializeField] private Button cancelButton = null;

    private System.Action<bool> callbackFunc = null;

    public Popup_Confirm Setup(int stringId, bool displayOkButton, System.Action<bool> callback)
    {
        return Setup(LocalizationService.Get(stringId), displayOkButton, callback);
    }

    public Popup_Confirm Setup(string desc, bool displayOkButton, System.Action<bool> callback)
    {
        if (displayOkButton)
        {
            var botEdge = descLabel.rectTransform.GetEdge(RectTransform.Edge.Bottom);
            descLabel.rectTransform.SetEdge(RectTransform.Edge.Bottom, botEdge + buttonSize);
        }

        callbackFunc = callback;
        okButton.gameObject.SetActive(displayOkButton);
        okButton.onClick.AddListener(() => Close(true));
        cancelButton.onClick.AddListener(() => Close(false));
        descLabel.SetText(desc, false, LocalizationService.IsPersian);
        return this;
    }

    private void Start()
    {
        var topEdge = descLabel.rectTransform.GetEdge(RectTransform.Edge.Top);
        var botEdge = descLabel.rectTransform.GetEdge(RectTransform.Edge.Bottom);
        var panelheight = Mathf.Clamp(topEdge + descLabel.preferredHeight + botEdge, minHeight, maxHeight);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelheight);
        UiShowHide.ShowAll(transform);
    }

    public override void Back()
    {
        Close(false);
    }

    private void Close(bool isok)
    {
        base.Back();
        if (callbackFunc != null) callbackFunc(isok);
    }
}
