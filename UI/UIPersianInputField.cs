using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class UIPersianInputField : Base
{
    public bool autoAlignment = true;

    public InputField inputField { get; private set; }
    public Text inputText { get; private set; }

    private void Awake()
    {
        inputField = GetComponent<InputField>();
        inputText = inputField.textComponent.gameObject.Clone<Text>();
        inputField.textComponent.color = new Color(0, 0, 0, 0);
        inputText.raycastTarget = false;
        if (inputField == null || inputText == null)
        {
            Debug.LogWarning("Can't find objects in " + name);
            enabled = false;
            return;
        }

        inputField.onValueChanged.AddListener(OnInputTextChanged);
    }

    private void OnEnable()
    {
        OnInputTextChanged(inputField.text);
    }

    public void OnInputTextChanged(string value)
    {
        var res = inputField.text.Trim().CleanForPersian();
        inputText.SetTextAndWrap(res, autoAlignment, LocalizationService.IsPersian);
    }
}
