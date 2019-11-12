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

    public static Color GetAverageColor(this Texture2D texture)
    {
        if (texture == null) return Color.white;
        var colors = texture.GetPixels(0, 0, texture.width, texture.height);
        Color total = new Color(0, 0, 0, 0);
        for (int i = 0; i < colors.Length; i++)
            total += colors[i];
        return total / colors.Length;
    }

    public static Color GetAverageColor(this Sprite sprite)
    {
        if (sprite == null || sprite.texture == null) return Color.white;
        var colors = sprite.texture.GetPixels(sprite.rect.x.ToInt(), sprite.rect.y.ToInt(), sprite.rect.width.ToInt(), sprite.rect.height.ToInt());
        int count = 0;
        Color total = new Color(0, 0, 0, 0);
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].a > 0.1f)
            {
                count++;
                total += colors[i];
            }
        }
        return total / count;
    }
}
