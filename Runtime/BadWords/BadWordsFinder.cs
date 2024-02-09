using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SeganX
{
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
        private static readonly string patternStart = @"\b(";
        private static readonly string patternEnd = @"+)\b";
        private static readonly string patternBody = @"+)([\W|\d|_]*)(";
        private static List<Regex> badWordMatchers = new List<Regex>();

        public static void Initialize(string[] badWords)
        {
            badWordMatchers.Clear();
            foreach (var item in badWords)
#if SX_PARSI
                AddBadWord(item.Trim().CleanForPersian());
#else
            AddBadWord(item.Trim());
#endif
        }

        public static void AddBadWord(string badWord)
        {
            if (badWord == null || badWord.IsNullOrEmpty()) return;

            var str = patternStart;
            for (int i = 0; i < (badWord.Length - 1); i++)
                str += badWord[i] + patternBody;
            str += badWord[badWord.Length - 1] + patternEnd;

            badWordMatchers.Add(new Regex(str, RegexOptions.IgnoreCase | RegexOptions.Multiline));
        }

        public static string Censore(string text, string replaceWith)
        {
            // replace . , ~ ! @ # $ 
#if SX_PARSI
            return badWordMatchers.Aggregate(text.CleanForPersian(), (current, matcher) => matcher.Replace(current, replaceWith));
#else
        return badWordMatchers.Aggregate(text, (current, matcher) => matcher.Replace(current, replaceWith));
#endif
        }

        public static bool HasBadWord(string text)
        {
#if SX_PARSI
            return badWordMatchers.Any(x => x.Match(text.CleanForPersian()).Success);
#else
        return badWordMatchers.Any(x => x.Match(text).Success);
#endif
        }
    }
}
