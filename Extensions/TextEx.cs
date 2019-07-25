using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public static class TextEx
{
    public static TextMesh SetText(this TextMesh self, string text, bool autoRtl = false, bool forcePersian = false)
    {
        if (text.IsNullOrEmpty())
        {
            self.text = text;
            return self;
        }

        if (autoRtl)
            self.FitAlignment(text.IsRtl());

        if (!forcePersian && !text.HasRtl())
        {
            self.text = text;
            return self;
        }

        self.text = text.Persian();
        return self;
    }

    private static string WrapLine(this TextGenerator self, string line, TextGenerationSettings settings)
    {
        string str = string.Empty;
        float lineWidth = 0;
        float speceWidth = self.GetPreferredWidth(" ", settings);
        float maxWidth = (settings.generationExtents.x > 20 ? settings.generationExtents.x : 99999) * settings.scaleFactor;
        List<string> lines = new List<string>(50);
        var words = line.Split(' ');
        for (int i = words.Length - 1; i >= 0; --i)
        {
            lineWidth += self.GetPreferredWidth(words[i], settings) + speceWidth;
            if (lineWidth > maxWidth && str.Length > 0)
            {
                lineWidth = 0;
                lines.Add(str);
                str = string.Empty;
                i++;
            }
            else if (str.Length > 0)
                str = words[i] + " " + str;
            else
                str = words[i];
        }
        if (str.Length > 0) lines.Add(str);
        return string.Join("\n", lines.ToArray());
    }

    public static Text SetText(this Text self, string text, bool autoRtl = false, bool forcePersian = false)
    {
        if (text.IsNullOrEmpty())
        {
            self.text = text;
            return self;
        }

        if (autoRtl)
            self.FitAlignment(text.IsRtl());

        if (!forcePersian && !text.HasRtl())
        {
            self.text = text;
            return self;
        }

        self.text = text.Persian();

        return self;
    }

    public static Text SetTextAndWrap(this Text self, string text, bool autoRtl = false, bool forcePersian = false)
    {
        if (text.IsNullOrEmpty())
        {
            self.text = text;
            return self;
        }

        if (autoRtl)
            self.FitAlignment(text.IsRtl());

        if (!forcePersian && !text.HasRtl())
        {
            self.text = text;
            return self;
        }

        text = text.Persian();

        if (self.horizontalOverflow == HorizontalWrapMode.Wrap)
        {
            if (self.rectTransform.rect.width > 0)
            {
                TextGenerationSettings settings = self.GetGenerationSettings(self.rectTransform.rect.size);
                //if (settings.font.characterInfo.Length > 0)
                {
                    TextGenerator generator = new TextGenerator();
                    var lines = text.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                        lines[i] = generator.WrapLine(lines[i], settings);
                    self.text = string.Join("\n", lines);
                }
                // else do nothing
            }
            else self.text = text;
        }
        else self.text = text;

        return self;
    }

    public static Text FitAlignment(this Text self, bool rtl)
    {
        switch (self.alignment)
        {
            case TextAnchor.UpperLeft: if (rtl) self.alignment = TextAnchor.UpperRight; break;
            case TextAnchor.UpperRight: if (!rtl) self.alignment = TextAnchor.UpperLeft; break;
            case TextAnchor.MiddleLeft: if (rtl) self.alignment = TextAnchor.MiddleRight; break;
            case TextAnchor.MiddleRight: if (!rtl) self.alignment = TextAnchor.MiddleLeft; break;
            case TextAnchor.LowerLeft: if (rtl) self.alignment = TextAnchor.LowerRight; break;
            case TextAnchor.LowerRight: if (!rtl) self.alignment = TextAnchor.LowerLeft; break;
        }

        return self;
    }

    public static TextMesh FitAlignment(this TextMesh self, bool rtl)
    {
        switch (self.alignment)
        {
            case TextAlignment.Left: if (rtl) self.alignment = TextAlignment.Left; break;
            case TextAlignment.Center: if (!rtl) self.alignment = TextAlignment.Center; break;
            case TextAlignment.Right: if (rtl) self.alignment = TextAlignment.Right; break;
        }
        return self;
    }
}
