using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public static class BadWordsFinder
{
    private static readonly string patternTemplate = @"+([\d]|[^\w]|_)*";
    private static List<Regex> badWordMatchers = new List<Regex>();

    public static void Initialize(string[] badWords)
    {
        badWordMatchers.Clear();
        foreach (var item in badWords)
            AddBadWord(item.Trim().CleanForPersian());
    }

    public static void AddBadWord(string badWord)
    {
        if (badWord == null || badWord.IsNullOrEmpty()) return;

        var s = badWord;
        for (int i = 0; i < badWord.Length - 1; i++)
            s = s.Insert(i * patternTemplate.Length + i + 1, patternTemplate);
        badWordMatchers.Add(new Regex(s, RegexOptions.IgnoreCase | RegexOptions.Multiline));
    }

    public static string Censore(string text, string replaceWith)
    {
        return badWordMatchers.Aggregate(text.CleanForPersian(), (current, matcher) => matcher.Replace(current, replaceWith));
    }

    public static bool HasBadWord(string text)
    {
        return badWordMatchers.Any(x => x.Match(text.CleanForPersian()).Success);
    }
}
