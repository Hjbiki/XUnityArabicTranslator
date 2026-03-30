using System;
using System.Collections.Generic;
using System.Text;

namespace ArabicGoogleTranslate
{
    internal static class ArabicReshaper
    {
        private struct LetterForms
        {
            public readonly char Isolated;
            public readonly char Final;
            public readonly char Initial;
            public readonly char Medial;

            public LetterForms(int isolated, int final_, int initial, int medial)
            {
                Isolated = (char)isolated;
                Final = (char)final_;
                Initial = initial == 0 ? '\0' : (char)initial;
                Medial = medial == 0 ? '\0' : (char)medial;
            }

            public bool IsDualJoining { get { return Initial != '\0'; } }
        }

        private struct LigatureForms
        {
            public readonly char Isolated;
            public readonly char Final;

            public LigatureForms(int isolated, int final_)
            {
                Isolated = (char)isolated;
                Final = (char)final_;
            }
        }

        private static readonly Dictionary<char, LetterForms> Letters;
        private static readonly Dictionary<char, LigatureForms> LamAlefMap;
        private static readonly Dictionary<char, char> MirrorMap;
        private const char LAM = '\u0644';

        static ArabicReshaper()
        {
            Letters = new Dictionary<char, LetterForms>
            {
                // Standard Arabic (28 letters + Hamza + Tatweel)
                { '\u0621', new LetterForms(0xFE80, 0xFE80, 0, 0) },
                { '\u0622', new LetterForms(0xFE81, 0xFE82, 0, 0) },
                { '\u0623', new LetterForms(0xFE83, 0xFE84, 0, 0) },
                { '\u0624', new LetterForms(0xFE85, 0xFE86, 0, 0) },
                { '\u0625', new LetterForms(0xFE87, 0xFE88, 0, 0) },
                { '\u0626', new LetterForms(0xFE89, 0xFE8A, 0xFE8B, 0xFE8C) },
                { '\u0627', new LetterForms(0xFE8D, 0xFE8E, 0, 0) },
                { '\u0628', new LetterForms(0xFE8F, 0xFE90, 0xFE91, 0xFE92) },
                { '\u0629', new LetterForms(0xFE93, 0xFE94, 0, 0) },
                { '\u062A', new LetterForms(0xFE95, 0xFE96, 0xFE97, 0xFE98) },
                { '\u062B', new LetterForms(0xFE99, 0xFE9A, 0xFE9B, 0xFE9C) },
                { '\u062C', new LetterForms(0xFE9D, 0xFE9E, 0xFE9F, 0xFEA0) },
                { '\u062D', new LetterForms(0xFEA1, 0xFEA2, 0xFEA3, 0xFEA4) },
                { '\u062E', new LetterForms(0xFEA5, 0xFEA6, 0xFEA7, 0xFEA8) },
                { '\u062F', new LetterForms(0xFEA9, 0xFEAA, 0, 0) },
                { '\u0630', new LetterForms(0xFEAB, 0xFEAC, 0, 0) },
                { '\u0631', new LetterForms(0xFEAD, 0xFEAE, 0, 0) },
                { '\u0632', new LetterForms(0xFEAF, 0xFEB0, 0, 0) },
                { '\u0633', new LetterForms(0xFEB1, 0xFEB2, 0xFEB3, 0xFEB4) },
                { '\u0634', new LetterForms(0xFEB5, 0xFEB6, 0xFEB7, 0xFEB8) },
                { '\u0635', new LetterForms(0xFEB9, 0xFEBA, 0xFEBB, 0xFEBC) },
                { '\u0636', new LetterForms(0xFEBD, 0xFEBE, 0xFEBF, 0xFEC0) },
                { '\u0637', new LetterForms(0xFEC1, 0xFEC2, 0xFEC3, 0xFEC4) },
                { '\u0638', new LetterForms(0xFEC5, 0xFEC6, 0xFEC7, 0xFEC8) },
                { '\u0639', new LetterForms(0xFEC9, 0xFECA, 0xFECB, 0xFECC) },
                { '\u063A', new LetterForms(0xFECD, 0xFECE, 0xFECF, 0xFED0) },
                { '\u0640', new LetterForms(0x0640, 0x0640, 0x0640, 0x0640) },
                { '\u0641', new LetterForms(0xFED1, 0xFED2, 0xFED3, 0xFED4) },
                { '\u0642', new LetterForms(0xFED5, 0xFED6, 0xFED7, 0xFED8) },
                { '\u0643', new LetterForms(0xFED9, 0xFEDA, 0xFEDB, 0xFEDC) },
                { '\u0644', new LetterForms(0xFEDD, 0xFEDE, 0xFEDF, 0xFEE0) },
                { '\u0645', new LetterForms(0xFEE1, 0xFEE2, 0xFEE3, 0xFEE4) },
                { '\u0646', new LetterForms(0xFEE5, 0xFEE6, 0xFEE7, 0xFEE8) },
                { '\u0647', new LetterForms(0xFEE9, 0xFEEA, 0xFEEB, 0xFEEC) },
                { '\u0648', new LetterForms(0xFEED, 0xFEEE, 0, 0) },
                { '\u0649', new LetterForms(0xFEEF, 0xFEF0, 0, 0) },
                { '\u064A', new LetterForms(0xFEF1, 0xFEF2, 0xFEF3, 0xFEF4) },

                // Persian / Urdu
                { '\u067E', new LetterForms(0xFB56, 0xFB57, 0xFB58, 0xFB59) },
                { '\u0686', new LetterForms(0xFB7A, 0xFB7B, 0xFB7C, 0xFB7D) },
                { '\u0698', new LetterForms(0xFB8A, 0xFB8B, 0, 0) },
                { '\u06A4', new LetterForms(0xFB6A, 0xFB6B, 0xFB6C, 0xFB6D) },
                { '\u06A9', new LetterForms(0xFB8E, 0xFB8F, 0xFB90, 0xFB91) },
                { '\u06AF', new LetterForms(0xFB92, 0xFB93, 0xFB94, 0xFB95) },
                { '\u06CC', new LetterForms(0xFBFC, 0xFBFD, 0xFBFE, 0xFBFF) },
                { '\u06BE', new LetterForms(0xFBAA, 0xFBAB, 0xFBAC, 0xFBAD) },
                { '\u06C1', new LetterForms(0xFBA6, 0xFBA7, 0xFBA8, 0xFBA9) },
                { '\u06D2', new LetterForms(0xFBAE, 0xFBAF, 0, 0) },
            };

            LamAlefMap = new Dictionary<char, LigatureForms>
            {
                { '\u0622', new LigatureForms(0xFEF5, 0xFEF6) },
                { '\u0623', new LigatureForms(0xFEF7, 0xFEF8) },
                { '\u0625', new LigatureForms(0xFEF9, 0xFEFA) },
                { '\u0627', new LigatureForms(0xFEFB, 0xFEFC) },
            };

            MirrorMap = new Dictionary<char, char>
            {
                { '(', ')' }, { ')', '(' },
                { '[', ']' }, { ']', '[' },
                { '{', '}' }, { '}', '{' },
                { '<', '>' }, { '>', '<' },
                { '\u00AB', '\u00BB' }, { '\u00BB', '\u00AB' },
            };
        }

        private static bool IsArabicLetter(char c)
        {
            return Letters.ContainsKey(c);
        }

        private static bool IsTransparent(char c)
        {
            return (c >= '\u064B' && c <= '\u065F') ||
                   c == '\u0670' ||
                   (c >= '\u0610' && c <= '\u061A') ||
                   (c >= '\u06D6' && c <= '\u06ED');
        }

        private static bool IsRtlChar(char c)
        {
            return (c >= '\u0600' && c <= '\u06FF') ||
                   (c >= '\u0750' && c <= '\u077F') ||
                   (c >= '\uFB50' && c <= '\uFDFF') ||
                   (c >= '\uFE70' && c <= '\uFEFF') ||
                   c == '\u200F';
        }

        private static int FindNextLetter(string text, int start)
        {
            for (int i = start; i < text.Length; i++)
            {
                if (IsTransparent(text[i])) continue;
                if (IsArabicLetter(text[i])) return i;
                return -1;
            }
            return -1;
        }

        private static bool PrevLetterIsDualJoining(string text, int pos, HashSet<int> skipIndices)
        {
            for (int i = pos - 1; i >= 0; i--)
            {
                if (skipIndices != null && skipIndices.Contains(i)) continue;
                char c = text[i];
                if (IsTransparent(c)) continue;
                if (!IsArabicLetter(c)) return false;
                LetterForms forms;
                return Letters.TryGetValue(c, out forms) && forms.IsDualJoining;
            }
            return false;
        }

        private static bool NextIsArabicLetter(string text, int pos, HashSet<int> skipIndices)
        {
            for (int i = pos + 1; i < text.Length; i++)
            {
                if (skipIndices != null && skipIndices.Contains(i)) continue;
                char c = text[i];
                if (IsTransparent(c)) continue;
                return IsArabicLetter(c);
            }
            return false;
        }

        /// <summary>
        /// Reshape Arabic text: convert logical characters to their correct
        /// positional presentation forms and handle Lam-Alef ligatures.
        /// </summary>
        public static string Reshape(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Pass 1: find Lam-Alef pairs and record which indices to skip (the Alef)
            var lamAlefPairs = new Dictionary<int, char>();
            var skipIndices = new HashSet<int>();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != LAM || skipIndices.Contains(i)) continue;

                int nextIdx = FindNextLetter(text, i + 1);
                if (nextIdx < 0) continue;

                if (LamAlefMap.ContainsKey(text[nextIdx]))
                {
                    lamAlefPairs[i] = text[nextIdx];
                    skipIndices.Add(nextIdx);
                }
            }

            // Pass 2: reshape
            var result = new List<char>(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                if (skipIndices.Contains(i))
                {
                    if (!lamAlefPairs.ContainsKey(i))
                    {
                        if (IsTransparent(text[i]))
                            result.Add(text[i]);
                        continue;
                    }
                }

                char c = text[i];

                if (IsTransparent(c))
                {
                    result.Add(c);
                    continue;
                }

                if (!IsArabicLetter(c))
                {
                    result.Add(c);
                    continue;
                }

                // Lam-Alef ligature
                if (lamAlefPairs.ContainsKey(i))
                {
                    char alef = lamAlefPairs[i];
                    LigatureForms lig = LamAlefMap[alef];
                    bool prevConn = PrevLetterIsDualJoining(text, i, skipIndices);
                    result.Add(prevConn ? lig.Final : lig.Isolated);
                    continue;
                }

                // Regular Arabic letter
                LetterForms forms = Letters[c];
                bool prev = PrevLetterIsDualJoining(text, i, skipIndices);
                bool next = forms.IsDualJoining && NextIsArabicLetter(text, i, skipIndices);

                char shaped;
                if (!prev && !next)
                    shaped = forms.Isolated;
                else if (!prev && next)
                    shaped = forms.Initial;
                else if (prev && !next)
                    shaped = forms.Final;
                else
                    shaped = forms.Medial;

                result.Add(shaped);
            }

            return new string(result.ToArray());
        }

        // ---- Simplified BiDi ----

        private static int GetCharDirection(char c)
        {
            if (IsRtlChar(c) || IsTransparent(c)) return -1;
            if ((c >= '0' && c <= '9') ||
                (c >= 'A' && c <= 'Z') ||
                (c >= 'a' && c <= 'z') ||
                (c >= '\u0660' && c <= '\u0669') ||
                (c >= '\u06F0' && c <= '\u06F9'))
                return 1;
            return 0;
        }

        /// <summary>
        /// Apply simplified BiDi reordering for display in LTR engines.
        /// Reverses the overall text (RTL base direction) then re-reverses
        /// embedded LTR runs (numbers, Latin) to restore their order.
        /// Also mirrors bracket characters.
        /// </summary>
        public static string ApplyBidi(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Build grapheme clusters (base char + trailing diacritics)
            var clusters = new List<string>();
            int idx = 0;
            while (idx < text.Length)
            {
                int start = idx;
                idx++;
                while (idx < text.Length && IsTransparent(text[idx]))
                    idx++;
                clusters.Add(text.Substring(start, idx - start));
            }

            // Reverse entire list (RTL base direction → visual LTR order)
            clusters.Reverse();

            // Re-reverse LTR runs (numbers, Latin)
            int i = 0;
            while (i < clusters.Count)
            {
                if (GetCharDirection(clusters[i][0]) == 1)
                {
                    int runStart = i;
                    while (i < clusters.Count)
                    {
                        int dir = GetCharDirection(clusters[i][0]);
                        if (dir == 1 || (dir == 0 && IsLtrContinuation(clusters, i)))
                            i++;
                        else
                            break;
                    }

                    int runEnd = i;
                    while (runEnd > runStart && GetCharDirection(clusters[runEnd - 1][0]) == 0)
                        runEnd--;

                    if (runEnd > runStart)
                        ReverseRange(clusters, runStart, runEnd - 1);
                }
                else
                {
                    i++;
                }
            }

            // Mirror brackets
            for (i = 0; i < clusters.Count; i++)
            {
                if (clusters[i].Length == 1)
                {
                    char mirror;
                    if (MirrorMap.TryGetValue(clusters[i][0], out mirror))
                        clusters[i] = mirror.ToString();
                }
            }

            var sb = new StringBuilder();
            for (int j = 0; j < clusters.Count; j++)
                sb.Append(clusters[j]);
            return sb.ToString();
        }

        private static bool IsLtrContinuation(List<string> clusters, int idx)
        {
            for (int j = idx + 1; j < clusters.Count; j++)
            {
                int dir = GetCharDirection(clusters[j][0]);
                if (dir == 1) return true;
                if (dir == -1) return false;
            }
            return false;
        }

        private static void ReverseRange(List<string> list, int left, int right)
        {
            while (left < right)
            {
                string tmp = list[left];
                list[left] = list[right];
                list[right] = tmp;
                left++;
                right--;
            }
        }

        internal static bool StripTashkeel = true;

        private static string RemoveTashkeel(string text)
        {
            var sb = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (!IsTransparent(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private static string ProcessLine(string line, int maxLineWidth)
        {
            if (string.IsNullOrEmpty(line))
                return line;

            if (StripTashkeel)
                line = RemoveTashkeel(line);

            string result = ApplyBidi(Reshape(line));

            if (maxLineWidth > 0 && result.Length > maxLineWidth)
                result = WrapAndReverse(result, maxLineWidth);

            return result;
        }

        /// <summary>
        /// Word-wrap text at maxWidth characters (at word boundaries),
        /// then reverse line order so Arabic reads top-to-bottom in LTR text boxes.
        /// </summary>
        internal static string WrapAndReverse(string text, int maxWidth)
        {
            string[] words = text.Split(' ');
            var lines = new List<string>();
            var currentLine = new StringBuilder();

            for (int w = 0; w < words.Length; w++)
            {
                string word = words[w];
                if (word.Length == 0) continue;

                if (currentLine.Length > 0 && currentLine.Length + 1 + word.Length > maxWidth)
                {
                    lines.Add(currentLine.ToString());
                    currentLine.Length = 0;
                }

                if (currentLine.Length > 0)
                    currentLine.Append(' ');
                currentLine.Append(word);
            }

            if (currentLine.Length > 0)
                lines.Add(currentLine.ToString());

            if (lines.Count <= 1)
                return text;

            lines.Reverse();
            return string.Join("\n", lines.ToArray());
        }

        private static bool IsCJK(char c)
        {
            return (c >= '\u3000' && c <= '\u9FFF') ||
                   (c >= '\uF900' && c <= '\uFAFF') ||
                   (c >= '\uFF00' && c <= '\uFFEF');
        }

        /// <summary>
        /// Estimate how many Arabic characters fit in the same visual width
        /// as the source text. CJK chars are ~2x wider than Arabic/Latin.
        /// </summary>
        internal static int EstimateArabicCapacity(string sourceText)
        {
            if (string.IsNullOrEmpty(sourceText))
                return 0;

            string[] parts;
            if (sourceText.IndexOf("\\n") >= 0)
                parts = sourceText.Split(new string[] { "\\n" }, StringSplitOptions.None);
            else if (sourceText.IndexOf('\n') >= 0)
                parts = sourceText.Split('\n');
            else
                parts = new string[] { sourceText };

            int maxCapacity = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                int widthUnits = 0;
                for (int j = 0; j < parts[i].Length; j++)
                {
                    widthUnits += IsCJK(parts[i][j]) ? 100 : 50;
                }
                int capacity = widthUnits / 55;
                if (capacity > maxCapacity)
                    maxCapacity = capacity;
            }

            return maxCapacity;
        }

        /// <summary>
        /// Full pipeline: reshape + bidi reorder + optional line wrapping.
        /// </summary>
        /// <param name="text">Translated Arabic text</param>
        /// <param name="maxLineWidth">Max chars per line (0 = no wrapping)</param>
        public static string Process(string text, int maxLineWidth = 0)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Literal \n (backslash + n) used by many game engines
            if (text.IndexOf("\\n") >= 0)
            {
                string[] parts = text.Split(new string[] { "\\n" }, StringSplitOptions.None);
                for (int i = 0; i < parts.Length; i++)
                    parts[i] = ProcessLine(parts[i], maxLineWidth);
                return string.Join("\\n", parts);
            }

            // Actual newline characters
            if (text.IndexOf('\n') >= 0)
            {
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                    lines[i] = ProcessLine(lines[i], maxLineWidth);
                return string.Join("\n", lines);
            }

            return ProcessLine(text, maxLineWidth);
        }
    }
}
