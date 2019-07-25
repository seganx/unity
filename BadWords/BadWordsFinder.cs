using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class BadWordsFinder : MonoBehaviour
{
    [SerializeField] private TextAsset badWords = null;

    private void Awake()
    {
        if (badWords == null) return;
        var bads = badWords.text.Split(new string[] { "\n", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        if (bads.IsNullOrEmpty()) return;
        Initialize(bads);
    }


    ///////////////////////////////////////////////////////////////////////////////////
    //  STATIC MEMBERS
    ///////////////////////////////////////////////////////////////////////////////////
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
