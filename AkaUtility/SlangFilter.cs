using System;
using System.Collections.Generic;
using System.Linq;

namespace AkaUtility
{
    public class SlangFilter
    {
        private static RecurNode root = null;
        public static void SetSlang(List<string> slangs)
        {
            root = new RecurNode();
            root.Initialize();

            foreach (var slang in slangs)
            {
                root.Add(System.Text.RegularExpressions.Regex.Replace(slang, @"\s", ""), null);
            }
        }

        public bool IsInvalidWord(string word)
        {
            return String.IsNullOrWhiteSpace(word) || word.Contains(' ') || word.Contains("'") || word.Contains('"') || word.Contains(';');
        }

        public bool IsInvalidWordChattingMessage(string word)
        {
            return String.IsNullOrWhiteSpace(word) || word.Contains("'") || word.Contains('"') || word.Contains(';');
        }

        public bool IsFiltered(string word)
        {
            if (IsInvalidWord(word))
                return true;

            if (root.IsLastLeaf)
                return false;

            List<RecurNode> matchedOld = new List<RecurNode>();
            List<RecurNode> matchedNew = new List<RecurNode>();
            RecurNode nextNode = null;
            foreach (char letter in word)
            {
                if (root.Letters.TryGetValue(letter.ToString(), out nextNode))
                {
                    matchedNew.Add(nextNode);
                }

                foreach (RecurNode node in matchedOld)
                {
                    if (node.IsLastLeaf)
                    {
                        return true;
                    }
                    if (node.Letters.TryGetValue(letter.ToString(), out nextNode))
                    {
                        matchedNew.Add(nextNode);
                    }
                }

                matchedOld.Clear();
                // swap
                List<RecurNode> temp = matchedOld;
                matchedOld = matchedNew;
                matchedNew = temp;
            }

            foreach (RecurNode node in matchedOld)
            {
                if (node.IsLastLeaf)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsFilteredText(string word)
        {
            if (IsInvalidWordChattingMessage(word))
                return true;

            if (root.IsLastLeaf)
                return false;

            List<RecurNode> matchedOld = new List<RecurNode>();
            List<RecurNode> matchedNew = new List<RecurNode>();
            RecurNode nextNode = null;
            foreach (char letter in word)
            {
                if (root.Letters.TryGetValue(letter.ToString(), out nextNode))
                {
                    matchedNew.Add(nextNode);
                }

                foreach (RecurNode node in matchedOld)
                {
                    if (node.IsLastLeaf)
                    {
                        return true;
                    }
                    if (node.Letters.TryGetValue(letter.ToString(), out nextNode))
                    {
                        matchedNew.Add(nextNode);
                    }
                }

                matchedOld.Clear();
                // swap
                List<RecurNode> temp = matchedOld;
                matchedOld = matchedNew;
                matchedNew = temp;
            }

            foreach (RecurNode node in matchedOld)
            {
                if (node.IsLastLeaf)
                {
                    return true;
                }
            }

            return false;
        }

        public class MatchedHistory
        {
            public RecurNode node;
            public int ignoreLength;
        }

        public string ReplaceFiltered(string word, char replaceChar)
        {
            if (root.IsLastLeaf)
            {
                return word;
            }

            List<MatchedHistory> matchedOld = new List<MatchedHistory>();
            List<MatchedHistory> matchedNew = new List<MatchedHistory>();
            RecurNode nextNode = null;
            Char[] ignoreChar = " \t".ToArray(); // 공백들
            var input = word.ToArray();
            int inputLength = input.Length;
            for (int cursor = 0; cursor < inputLength; ++cursor)
            {
                bool ignored = false;
                foreach (char ignore in ignoreChar)
                {
                    if (input[cursor] == ignore)
                    {
                        ignored = true;
                        break;
                    }
                }

                if (ignored)
                {
                    foreach (MatchedHistory history in matchedOld)
                    {
                        if (history.node.IsLastLeaf)
                        {
                            replace(input, cursor - history.node.Depth - history.ignoreLength, cursor - 1, replaceChar, ignoreChar);
                            continue;
                        }
                        history.ignoreLength++;
                        matchedNew.Add(new MatchedHistory() { node = history.node, ignoreLength = history.ignoreLength });
                    }
                }
                else
                {
                    if (root.Letters.TryGetValue(input[cursor].ToString(), out nextNode))
                    {
                        matchedNew.Add(new MatchedHistory() { node = nextNode, ignoreLength = 0 });
                    }

                    foreach (MatchedHistory history in matchedOld)
                    {
                        if (history.node.IsLastLeaf)
                        {
                            replace(input, cursor - history.node.Depth - history.ignoreLength, cursor - 1, replaceChar, ignoreChar);
                            continue;
                        }
                        if (history.node.Letters.TryGetValue(input[cursor].ToString(), out nextNode))
                        {
                            matchedNew.Add(new MatchedHistory() { node = nextNode, ignoreLength = history.ignoreLength });
                        }
                    }
                }
                matchedOld.Clear();
                // swap
                List<MatchedHistory> temp = matchedOld;
                matchedOld = matchedNew;
                matchedNew = temp;
            }

            foreach (MatchedHistory history in matchedOld)
            {
                if (history.node.IsLastLeaf)
                {
                    replace(input, inputLength - history.node.Depth - history.ignoreLength, inputLength - 1, replaceChar, ignoreChar);
                }
            }

            return new string(input);
        }

        private void replace(char[] input, int from, int to, char replaceLetter, char[] ignoreChar)
        {
            for (int cursor = from; cursor <= to; ++cursor)
            {
                if (ignoreChar != null)
                {
                    bool ignored = false;
                    foreach (char ignore in ignoreChar)
                    {
                        if (input[cursor] == ignore)
                        {
                            ignored = true;
                            break;
                        }
                    }
                    if (ignored)
                    {
                        continue;
                    }
                }
                input[cursor] = replaceLetter;
            }
        }
    }
}
