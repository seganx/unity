using UnityEngine;
using UnityEngine.UI;

namespace SeganX
{
    public static class ImageEx
    {
        public static int ToInt(this Color32 color)
        {
            return (color.r) | (color.g) << 8 | (color.b) << 16 | (color.a) << 24;
        }

        public static uint ToUint(this Color32 color)
        {
            return (uint)ToInt(color);
        }

        public static Color32 ToColor32(this int color)
        {
            Color32 res = new Color32();
            res.r = (byte)((color));
            res.g = (byte)((color) >> 8);
            res.b = (byte)((color) >> 16);
            res.a = (byte)((color) >> 24);
            return res;
        }

        public static Color32 ToColor32(this uint color)
        {
            return ToColor32((int)color);
        }

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
}