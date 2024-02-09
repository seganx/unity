using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace SeganX.Localization
{
    public static class PersianTextUI
    {
        private class CharacterInfo
        {
            public char isolated;
            public char initial;
            public char medial;
            public char final;
            public bool joinsNext = true;
            public bool joinsPrevious = true;
            public bool rightToLeftOrdering = true;
        }

        private enum CharType
        {
            LTR,
            RTL,
            Neutral,
            Space
        }

        private static readonly Dictionary<char, CharacterInfo> charInfo = new Dictionary<char, CharacterInfo>();
        private static readonly HashSet<char> neutralCharacters = new HashSet<char>();

        private static readonly Dictionary<char, char> swappedRtlCharacters = new Dictionary<char, char>();

        static PersianTextUI()
        {
            charInfo.Add('ا', new CharacterInfo
            {
                isolated = 'ﺍ',
                final = 'ﺎ',
                initial = 'ﺍ',
                medial = 'ﺎ',
                joinsNext = false
            });
            charInfo.Add('آ', new CharacterInfo
            {
                isolated = 'ﺁ',
                final = 'ﺂ',
                initial = 'ﺁ',
                medial = 'ﺂ',
                joinsNext = false
            });
            charInfo.Add('ب', new CharacterInfo
            {
                isolated = 'ﺏ',
                final = 'ﺐ',
                initial = 'ﺑ',
                medial = 'ﺒ'
            });
            charInfo.Add('پ', new CharacterInfo
            {
                isolated = 'ﭖ',
                final = 'ﭗ',
                initial = 'ﭘ',
                medial = 'ﭙ'
            });
            charInfo.Add('ت', new CharacterInfo
            {
                isolated = 'ﺕ',
                final = 'ﺖ',
                initial = 'ﺗ',
                medial = 'ﺘ'
            });
            charInfo.Add('ث', new CharacterInfo
            {
                isolated = 'ﺙ',
                final = 'ﺚ',
                initial = 'ﺛ',
                medial = 'ﺜ'
            });
            charInfo.Add('ج', new CharacterInfo
            {
                isolated = 'ﺝ',
                final = 'ﺞ',
                initial = 'ﺟ',
                medial = 'ﺠ'
            });
            charInfo.Add('چ', new CharacterInfo
            {
                isolated = 'ﭺ',
                final = 'ﭻ',
                initial = 'ﭼ',
                medial = 'ﭽ'
            });
            charInfo.Add('ح', new CharacterInfo
            {
                isolated = 'ﺡ',
                final = 'ﺢ',
                initial = 'ﺣ',
                medial = 'ﺤ'
            });
            charInfo.Add('خ', new CharacterInfo
            {
                isolated = 'ﺥ',
                final = 'ﺦ',
                initial = 'ﺧ',
                medial = 'ﺨ'
            });
            charInfo.Add('د', new CharacterInfo
            {
                isolated = 'ﺩ',
                final = 'ﺪ',
                initial = 'ﺩ',
                medial = 'ﺪ',
                joinsNext = false
            });
            charInfo.Add('ذ', new CharacterInfo
            {
                isolated = 'ﺫ',
                final = 'ﺬ',
                initial = 'ﺫ',
                medial = 'ﺬ',
                joinsNext = false
            });
            charInfo.Add('ر', new CharacterInfo
            {
                isolated = 'ﺭ',
                final = 'ﺮ',
                initial = 'ﺭ',
                medial = 'ﺮ',
                joinsNext = false
            });
            charInfo.Add('ز', new CharacterInfo
            {
                isolated = 'ﺯ',
                final = 'ﺰ',
                initial = 'ﺯ',
                medial = 'ﺰ',
                joinsNext = false
            });
            charInfo.Add('ژ', new CharacterInfo
            {
                isolated = 'ﮊ',
                final = 'ﮋ',
                initial = 'ﮊ',
                medial = 'ﮋ',
                joinsNext = false
            });
            charInfo.Add('س', new CharacterInfo
            {
                isolated = 'ﺱ',
                final = 'ﺲ',
                initial = 'ﺳ',
                medial = 'ﺴ'
            });
            charInfo.Add('ش', new CharacterInfo
            {
                isolated = 'ﺵ',
                final = 'ﺶ',
                initial = 'ﺷ',
                medial = 'ﺸ'
            });
            charInfo.Add('ص', new CharacterInfo
            {
                isolated = 'ﺹ',
                final = 'ﺺ',
                initial = 'ﺻ',
                medial = 'ﺼ'
            });
            charInfo.Add('ض', new CharacterInfo
            {
                isolated = 'ﺽ',
                final = 'ﺾ',
                initial = 'ﺿ',
                medial = 'ﻀ'
            });
            charInfo.Add('ط', new CharacterInfo
            {
                isolated = 'ﻁ',
                final = 'ﻂ',
                initial = 'ﻃ',
                medial = 'ﻄ'
            });
            charInfo.Add('ظ', new CharacterInfo
            {
                isolated = 'ﻅ',
                final = 'ﻆ',
                initial = 'ﻇ',
                medial = 'ﻈ'
            });
            charInfo.Add('ع', new CharacterInfo
            {
                isolated = 'ﻉ',
                final = 'ﻊ',
                initial = 'ﻋ',
                medial = 'ﻌ'
            });
            charInfo.Add('غ', new CharacterInfo
            {
                isolated = 'ﻍ',
                final = 'ﻎ',
                initial = 'ﻏ',
                medial = 'ﻐ'
            });
            charInfo.Add('ف', new CharacterInfo
            {
                isolated = 'ﻑ',
                final = 'ﻒ',
                initial = 'ﻓ',
                medial = 'ﻔ'
            });
            charInfo.Add('ق', new CharacterInfo
            {
                isolated = 'ﻕ',
                final = 'ﻖ',
                initial = 'ﻗ',
                medial = 'ﻘ'
            });
            charInfo.Add('ک', new CharacterInfo
            {
                isolated = 'ﮎ',
                final = 'ﮏ',
                initial = 'ﮐ',
                medial = 'ﮑ'
            });
            charInfo.Add('گ', new CharacterInfo
            {
                isolated = 'ﮒ',
                final = 'ﮓ',
                initial = 'ﮔ',
                medial = 'ﮕ'
            });
            charInfo.Add('ل', new CharacterInfo
            {
                isolated = 'ﻝ',
                final = 'ﻞ',
                initial = 'ﻟ',
                medial = 'ﻠ'
            });
            charInfo.Add('م', new CharacterInfo
            {
                isolated = 'ﻡ',
                final = 'ﻢ',
                initial = 'ﻣ',
                medial = 'ﻤ'
            });
            charInfo.Add('ن', new CharacterInfo
            {
                isolated = 'ﻥ',
                final = 'ﻦ',
                initial = 'ﻧ',
                medial = 'ﻨ'
            });
            charInfo.Add('و', new CharacterInfo
            {
                isolated = 'ﻭ',
                final = 'ﻮ',
                initial = 'ﻭ',
                medial = 'ﻮ',
                joinsNext = false
            });
            charInfo.Add('ه', new CharacterInfo
            {
                isolated = 'ﻩ',
                final = 'ﻪ',
                initial = 'ﻫ',
                medial = 'ﻬ'
            });
            charInfo.Add('ی', new CharacterInfo
            {
                isolated = 'ﯼ',
                final = 'ﯽ',
                initial = 'ﯾ',
                medial = 'ﯿ'
            });
            charInfo.Add('ئ', new CharacterInfo
            {
                isolated = 'ﺉ',
                final = 'ﺊ',
                initial = 'ﺋ',
                medial = 'ﺌ'
            });
            charInfo.Add('ء', new CharacterInfo
            {
                isolated = 'ء',
                final = 'ء',
                initial = 'ء',
                medial = 'ء',
                joinsNext = false,
                joinsPrevious = false
            });
            charInfo.Add('ﻻ', new CharacterInfo
            {
                isolated = 'ﻻ',
                final = 'ﻼ',
                initial = 'ﻻ',
                medial = 'ﻼ',
                joinsNext = false
            });
            charInfo.Add('0', new CharacterInfo
            {
                isolated = '۰',
                final = '۰',
                initial = '۰',
                medial = '۰',
                joinsNext = false,
                joinsPrevious = false,
                rightToLeftOrdering = false
            });
            charInfo.Add('1', new CharacterInfo
            {
                isolated = '۱',
                final = '۱',
                initial = '۱',
                medial = '۱',
                joinsNext = false,
                joinsPrevious = false,
                rightToLeftOrdering = false
            });
            charInfo.Add('2', new CharacterInfo
            {
                isolated = '۲',
                final = '۲',
                initial = '۲',
                medial = '۲',
                joinsNext = false,
                joinsPrevious = false,
                rightToLeftOrdering = false
            });
            charInfo.Add('3', new CharacterInfo
            {
                isolated = '۳',
                final = '۳',
                initial = '۳',
                medial = '۳',
                joinsNext = false,
                joinsPrevious = false,
                rightToLeftOrdering = false
            });
            charInfo.Add('4', new CharacterInfo
            {
                isolated = '۴',
                final = '۴',
                initial = '۴',
                medial = '۴',
                joinsNext = false,
                joinsPrevious = false,
                rightToLeftOrdering = false
            });
            charInfo.Add('5', new CharacterInfo
            {
                isolated = '۵',
                final = '۵',
                initial = '۵',
                medial = '۵',
                joinsNext = false,
                joinsPrevious = false,
                rightToLeftOrdering = false
            });
            charInfo.Add('6', new CharacterInfo
            {
                isolated = '۶',
                final = '۶',
                initial = '۶',
                medial = '۶',
                joinsNext = false,
                joinsPrevious = false,
                rightToLeftOrdering = false
            });
            charInfo.Add('7', new CharacterInfo
            {
                isolated = '۷',
                final = '۷',
                initial = '۷',
                medial = '۷',
                joinsNext = false,
                joinsPrevious = false,
                rightToLeftOrdering = false
            });
            charInfo.Add('8', new CharacterInfo
            {
                isolated = '۸',
                final = '۸',
                initial = '۸',
                medial = '۸',
                joinsNext = false,
                joinsPrevious = false,
                rightToLeftOrdering = false
            });
            charInfo.Add('9', new CharacterInfo
            {
                isolated = '۹',
                final = '۹',
                initial = '۹',
                medial = '۹',
                joinsNext = false,
                joinsPrevious = false,
                rightToLeftOrdering = false
            });
            charInfo.Add('،', new CharacterInfo
            {
                isolated = '،',
                final = '،',
                initial = '،',
                medial = '،',
                joinsNext = false,
                joinsPrevious = false
            });
            charInfo.Add('؛', new CharacterInfo
            {
                isolated = '؛',
                final = '؛',
                initial = '؛',
                medial = '؛',
                joinsNext = false,
                joinsPrevious = false
            });
            neutralCharacters.Add(' ');
            neutralCharacters.Add('.');
            neutralCharacters.Add(':');
            neutralCharacters.Add(';');
            neutralCharacters.Add('\'');
            neutralCharacters.Add('"');
            neutralCharacters.Add('+');
            neutralCharacters.Add('-');
            neutralCharacters.Add('*');
            neutralCharacters.Add('/');
            neutralCharacters.Add('\\');
            neutralCharacters.Add('\n');
            swappedRtlCharacters['('] = ')';
            swappedRtlCharacters[')'] = '(';
            swappedRtlCharacters['['] = ']';
            swappedRtlCharacters[']'] = '[';
            swappedRtlCharacters['{'] = '}';
            swappedRtlCharacters['}'] = '{';
            swappedRtlCharacters['<'] = '>';
            swappedRtlCharacters['>'] = '<';
            swappedRtlCharacters['«'] = '»';
            swappedRtlCharacters['»'] = '«';
        }

        private static bool JoinsNext(char c)
        {
            if (charInfo.ContainsKey(c))
            {
                return charInfo[c].joinsNext;
            }
            return false;
        }

        private static bool JoinsPrevious(char c)
        {
            if (charInfo.ContainsKey(c))
            {
                return charInfo[c].joinsPrevious;
            }
            return false;
        }

        private static bool IsRightToLeftOrdered(char c, bool bTextRightToLeft)
        {
            if (swappedRtlCharacters.ContainsKey(c))
            {
                return bTextRightToLeft;
            }
            if (charInfo.ContainsKey(c))
            {
                return charInfo[c].rightToLeftOrdering;
            }
            return false;
        }

        private static bool IsNeutral(char c)
        {
            return neutralCharacters.Contains(c);
        }

        private static CharType GetCharType(char c, bool bTextRightToLeft)
        {
            if (c != ' ')
            {
                if (!IsNeutral(c))
                {
                    if (!IsRightToLeftOrdered(c, bTextRightToLeft))
                    {
                        return CharType.LTR;
                    }
                    return CharType.RTL;
                }
                return CharType.Neutral;
            }
            return CharType.Space;
        }

        private static bool CharTypesCompatibleForToken(CharType CT1, CharType CT2)
        {
            if (CT1 != CT2 && (CT1 != CharType.Neutral || CT2 == CharType.Space))
            {
                if (CT2 == CharType.Neutral)
                {
                    return CT1 != CharType.Space;
                }
                return false;
            }
            return true;
        }

        private static bool CharTypesCompatibleForRun(CharType CT1, CharType CT2)
        {
            if (CT1 == CharType.Space)
            {
                return CT2 == CharType.Space;
            }
            if (CT1 != CT2 && CT1 != CharType.Neutral && CT2 != CharType.Neutral)
            {
                return CT2 == CharType.Space;
            }
            return true;
        }

        private static char GetGlyph(char C, char Prev, char Next, bool bRightToLeft)
        {
            if (charInfo.ContainsKey(C))
            {
                bool flag = JoinsNext(Prev);
                bool flag2 = JoinsPrevious(Next);
                if (flag)
                {
                    if (flag2)
                    {
                        return charInfo[C].medial;
                    }
                    return charInfo[C].final;
                }
                if (flag2)
                {
                    return charInfo[C].initial;
                }
                return charInfo[C].isolated;
            }
            if (bRightToLeft && swappedRtlCharacters.ContainsKey(C))
            {
                return swappedRtlCharacters[C];
            }
            return C;
        }

        private static string ShapeTokenPersian(string token, bool rightToLeft)
        {
            StringBuilder stringBuilder = new StringBuilder();
            token = '\0' + token + '\0';
            for (int i = 1; i < token.Length - 1; i++)
            {
                if (token[i] == 'ل' && token[i + 1] == 'ا')
                {
                    if (rightToLeft)
                    {
                        stringBuilder.Insert(0, GetGlyph('ﻻ', token[i - 1], token[i + 2], rightToLeft));
                    }
                    else
                    {
                        stringBuilder.Append(GetGlyph('ﻻ', token[i - 1], token[i + 2], rightToLeft));
                    }
                    i++;
                }
                else if (rightToLeft)
                {
                    stringBuilder.Insert(0, GetGlyph(token[i], token[i - 1], token[i + 1], rightToLeft));
                }
                else
                {
                    stringBuilder.Append(GetGlyph(token[i], token[i - 1], token[i + 1], rightToLeft));
                }
            }
            return stringBuilder.ToString();
        }

        private static string GetNextToken(ref string Text, ref int scanIndex, bool textRightToLeft, out CharType result)
        {
            StringBuilder stringBuilder = new StringBuilder();
            result = GetCharType(Text[scanIndex], textRightToLeft);
            CharType charType;
            while (scanIndex < Text.Length && CharTypesCompatibleForToken(result, charType = GetCharType(Text[scanIndex], textRightToLeft)))
            {
                if (result == CharType.Neutral && charType != CharType.Neutral && charType != CharType.Space)
                {
                    result = charType;
                }
                stringBuilder.Append(Text[scanIndex]);
                scanIndex++;
            }
            if (result == CharType.Neutral)
            {
                result = (textRightToLeft ? CharType.RTL : CharType.LTR);
            }
            return stringBuilder.ToString();
        }

        private static string ShapeRun(string Text, bool bRightToLeft)
        {
            List<string> list = new List<string>();
            List<CharType> list2 = new List<CharType>();
            int ScanIdx = 0;
            while (ScanIdx < Text.Length)
            {
                CharType CharType;
                string nextToken = GetNextToken(ref Text, ref ScanIdx, bRightToLeft, out CharType);
                list.Add(nextToken);
                list2.Add(CharType);
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = ShapeTokenPersian(list[i], list2[i] == CharType.RTL);
            }
            StringBuilder stringBuilder = new StringBuilder();
            if (bRightToLeft)
            {
                for (int num = list.Count - 1; num >= 0; num--)
                {
                    stringBuilder.Append(list[num]);
                }
            }
            else
            {
                for (int j = 0; j < list.Count; j++)
                {
                    stringBuilder.Append(list[j]);
                }
            }
            return stringBuilder.ToString();
        }

        private static string GetNextRun(ref string Text, ref int ScanIdx, bool bTextRightToLeft, out bool bRunRightToLeft)
        {
            StringBuilder stringBuilder = new StringBuilder();
            CharType charType = GetCharType(Text[ScanIdx], bTextRightToLeft);
            CharType charType2;
            while (ScanIdx < Text.Length && CharTypesCompatibleForRun(charType, charType2 = GetCharType(Text[ScanIdx], bTextRightToLeft)))
            {
                if (charType == CharType.Neutral && charType2 != CharType.Neutral && charType2 != CharType.Space)
                {
                    charType = charType2;
                }
                stringBuilder.Append(Text[ScanIdx]);
                ScanIdx++;
            }
            switch (charType)
            {
                case CharType.Neutral:
                case CharType.Space:
                    bRunRightToLeft = bTextRightToLeft;
                    break;
                case CharType.RTL:
                    bRunRightToLeft = true;
                    break;
                default:
                    bRunRightToLeft = false;
                    break;
            }
            if (charType != CharType.Neutral)
            {
                while (GetCharType(stringBuilder[stringBuilder.Length - 1], bTextRightToLeft) == CharType.Neutral)
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    ScanIdx--;
                }
            }
            if (charType != CharType.Space)
            {
                while (GetCharType(stringBuilder[stringBuilder.Length - 1], bTextRightToLeft) == CharType.Space)
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    ScanIdx--;
                }
            }
            return stringBuilder.ToString();
        }

        private static string ShapeLine(string Text, bool bRightToLeft)
        {
            List<string> list = new List<string>();
            List<bool> list2 = new List<bool>();
            int ScanIdx = 0;
            while (ScanIdx < Text.Length)
            {
                bool bRunRightToLeft;
                string nextRun = GetNextRun(ref Text, ref ScanIdx, bRightToLeft, out bRunRightToLeft);
                list.Add(nextRun);
                list2.Add(bRunRightToLeft);
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = ShapeRun(list[i], list2[i]);
            }
            StringBuilder stringBuilder = new StringBuilder();
            if (bRightToLeft)
            {
                for (int num = list.Count - 1; num >= 0; num--)
                {
                    stringBuilder.Append(list[num]);
                }
            }
            else
            {
                for (int j = 0; j < list.Count; j++)
                {
                    stringBuilder.Append(list[j]);
                }
            }
            return stringBuilder.ToString();
        }

        public static string ShapeText(string text, bool rightToLeft = true)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] array = text.Replace('ي', 'ی').Split('\n');
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(ShapeLine(array[i], rightToLeft));
                if (i < array.Length - 1)
                {
                    stringBuilder.Append('\n');
                }
            }
            return stringBuilder.ToString().Replace("‌", "").Replace("‌", "");
        }

        private static int CovertCode(this char self)
        {
            switch (self)
            {
                case 'آ': return 10;
                case 'ا': return 11;
                case 'ب': return 12;
                case 'پ': return 13;
                case 'ت': return 14;
                case 'ث': return 15;
                case 'ج': return 16;
                case 'چ': return 17;
                case 'ح': return 18;
                case 'خ': return 19;
                case 'د': return 20;
                case 'ذ': return 21;
                case 'ر': return 22;
                case 'ز': return 23;
                case 'ژ': return 24;
                case 'س': return 25;
                case 'ش': return 26;
                case 'ص': return 27;
                case 'ض': return 28;
                case 'ط': return 29;
                case 'ظ': return 30;
                case 'ع': return 31;
                case 'غ': return 32;
                case 'ف': return 33;
                case 'ق': return 34;
                case 'ک': return 35;
                case 'گ': return 36;
                case 'ل': return 37;
                case 'م': return 38;
                case 'ن': return 39;
                case 'و': return 40;
                case 'ه': return 41;
                case 'ي':
                case 'ی': return 42;
                default: return 43 + self;
            }
        }

        public static int Compare(this string self, string with)
        {
            int n = System.Math.Max(self.Length, with.Length);
            int s = 0, w = 0;
            for (int i = 0; i < n; i++)
            {
                s = i < self.Length ? self[i].CovertCode() : 0;
                w = i < with.Length ? with[i].CovertCode() : 0;
                if (s == w) continue;
                return s - w;
            }
            return 0;
        }

        public static string GetPersianDateTime(this System.DateTime dateTime)
        {
            PersianCalendar pc = new PersianCalendar();
            try { return string.Format("{0}/{1}/{2} {3}:{4}", pc.GetYear(dateTime), pc.GetMonth(dateTime), pc.GetDayOfMonth(dateTime), pc.GetHour(dateTime), pc.GetMinute(dateTime)); }
            catch { }
            return "";
        }
    }
}