using UnityEngine;
using UnityEngine.UI;

public static class ImageEx
{
    public static Image SetColorAlpha(this Image self, float value)
    {
        var color = self.color;
        color.a = value;
        self.color = color;
        return self;
    }

    public static Image SetFillAmount(this Image self, double value, double max)
    {
        self.fillAmount = Mathf.Clamp01((float)(value / max));
        return self;
    }
}
