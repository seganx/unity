using UnityEngine;
using UnityEngine.UI;

public static class ImageEx
{
    public static Image SetFillAmount(this Image self, double value, double max)
    {
        self.fillAmount = Mathf.Clamp01((float)(value / max));
        return self;
    }
}
