using UnityEngine;
using UnityEngine.UI;

public static class ButtonEx
{
    public static Button SetInteractable(this Button self, bool interactable)
    {
        var group = self.GetComponent<CanvasGroup>();
        if (group) group.alpha = interactable ? self.colors.normalColor.a : self.colors.disabledColor.a;
        self.interactable = interactable;
        return self;
    }

}
