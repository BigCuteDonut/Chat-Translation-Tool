/*
    Chat Translation Tool
    Copyright (C) 2020 Johanna Sierak

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TranslateTool
{
    class Hinter
    {
        public List<Word> _database;
        private Dictionary<string, string> kanaConvert;

        public Hinter()
        {
            kanaConvert = new Dictionary<string, string>();
            kanaConvert["\u3058\u3083"] = "jya";
            kanaConvert["\u3058\u3085"] = "jyu";
            kanaConvert["\u3058\u3087"] = "jyo";
            kanaConvert["\u3057\u3083"] = "sha";
            kanaConvert["\u3057\u3085"] = "shu";
            kanaConvert["\u3057\u3087"] = "sho";
            kanaConvert["\u3061\u3083"] = "cha";
            kanaConvert["\u3061\u3085"] = "chu";
            kanaConvert["\u3061\u30e7"] = "cho";
            kanaConvert["\u30b8\u30e3"] = "jya";
            kanaConvert["\u30b8\u30e5"] = "jyu";
            kanaConvert["\u30b8\u3087"] = "jyo";
            kanaConvert["\u30b7\u30e3"] = "sha";
            kanaConvert["\u30b7\u30e5"] = "shu";
            kanaConvert["\u30b7\u30e7"] = "sho";
            kanaConvert["\u30c1\u30e3"] = "cha";
            kanaConvert["\u30c1\u30e5"] = "chu";
            kanaConvert["\u30c1\u30e7"] = "cho";
            kanaConvert["\u3064"] = "tsu";
            kanaConvert["\u3061"] = "chi";
            kanaConvert["\u3057"] = "shi";
            kanaConvert["\u3058"] = "ji";
            kanaConvert["\u30C4"] = "tsu";
            kanaConvert["\u30c1"] = "chi";
            kanaConvert["\u30b7"] = "shi";
            kanaConvert["\u30b8"] = "ji";
            kanaConvert["\u3084"] = "ya";
            kanaConvert["\u3086"] = "yu";
            kanaConvert["\u3088"] = "yo";
            kanaConvert["\u30e4"] = "ya";
            kanaConvert["\u30e6"] = "yu";
            kanaConvert["\u30e8"] = "yo";
            kanaConvert["\u3042"] = "a";
            kanaConvert["\u3048"] = "e";
            kanaConvert["\u3044"] = "i";
            kanaConvert["\u304A"] = "o";
            kanaConvert["\u3046"] = "u";
            kanaConvert["\u304B"] = "ka";
            kanaConvert["\u3051"] = "ke";
            kanaConvert["\u304F"] = "ku";
            kanaConvert["\u304D"] = "ki";
            kanaConvert["\u3053"] = "ko";
            kanaConvert["\u307E"] = "ma";
            kanaConvert["\u3081"] = "me";
            kanaConvert["\u3080"] = "mu";
            kanaConvert["\u307F"] = "mi";
            kanaConvert["\u3082"] = "mo";
            kanaConvert["\u3071"] = "pa";
            kanaConvert["\u307A"] = "pe";
            kanaConvert["\u3077"] = "pu";
            kanaConvert["\u3074"] = "pi";
            kanaConvert["\u307D"] = "po";
            kanaConvert["\u3056"] = "za";
            kanaConvert["\u305C"] = "ze";
            kanaConvert["\u305A"] = "zu";
            kanaConvert["\u306A"] = "na";
            kanaConvert["\u306D"] = "ne";
            kanaConvert["\u306C"] = "nu";
            kanaConvert["\u306B"] = "ni";
            kanaConvert["\u306E"] = "no";
            kanaConvert["\u3070"] = "ba";
            kanaConvert["\u3079"] = "be";
            kanaConvert["\u3076"] = "bu";
            kanaConvert["\u3073"] = "bi";
            kanaConvert["\u307C"] = "bo";
            kanaConvert["\u308F"] = "wa";
            kanaConvert["\u3092"] = "wo";
            kanaConvert["\u3089"] = "ra";
            kanaConvert["\u308C"] = "re";
            kanaConvert["\u308B"] = "ru";
            kanaConvert["\u308A"] = "ri";
            kanaConvert["\u308D"] = "ro";
            kanaConvert["\u3093"] = "n";
            kanaConvert["\u3075"] = "fu";
            kanaConvert["\u3055"] = "sa";
            kanaConvert["\u305B"] = "se";
            kanaConvert["\u3059"] = "su";
            kanaConvert["\u305D"] = "so";
            kanaConvert["\u305F"] = "ta";
            kanaConvert["\u3066"] = "te";
            kanaConvert["\u3068"] = "to";
            kanaConvert["\u306F"] = "ha";
            kanaConvert["\u3078"] = "he";
            kanaConvert["\u3072"] = "hi";
            kanaConvert["\u307B"] = "ho";
            kanaConvert["\u304C"] = "ga";
            kanaConvert["\u3052"] = "ge";
            kanaConvert["\u3050"] = "gu";
            kanaConvert["\u304E"] = "gi";
            kanaConvert["\u3054"] = "go";
            kanaConvert["\u3060"] = "da";
            kanaConvert["\u3067"] = "de";
            kanaConvert["\u3065"] = "du";
            kanaConvert["\u3069"] = "do";
            kanaConvert["\u30ab"] = "ka";
            kanaConvert["\u30b1"] = "ke";
            kanaConvert["\u30af"] = "ku";
            kanaConvert["\u30ad"] = "ki";
            kanaConvert["\u30b3"] = "ko";
            kanaConvert["\u30ac"] = "ga";
            kanaConvert["\u30b2"] = "ge";
            kanaConvert["\u30b0"] = "gu";
            kanaConvert["\u30ae"] = "gi";
            kanaConvert["\u30b4"] = "go";
            kanaConvert["\u30d1"] = "pa";
            kanaConvert["\u30da"] = "pe";
            kanaConvert["\u30d7"] = "pu";
            kanaConvert["\u30d4"] = "pi";
            kanaConvert["\u30dd"] = "po";
            kanaConvert["\u30e9"] = "ra";
            kanaConvert["\u30ec"] = "re";
            kanaConvert["\u30eb"] = "ru";
            kanaConvert["\u30ea"] = "ri";
            kanaConvert["\u30ed"] = "ro";
            kanaConvert["\u30d0"] = "ba";
            kanaConvert["\u30d9"] = "be";
            kanaConvert["\u30d6"] = "bu";
            kanaConvert["\u30d3"] = "bi";
            kanaConvert["\u30dc"] = "bo";
            kanaConvert["\u30a2"] = "a";
            kanaConvert["\u30a8"] = "e";
            kanaConvert["\u30a6"] = "u";
            kanaConvert["\u30a4"] = "i";
            kanaConvert["\u30aa"] = "o";
            kanaConvert["\u30de"] = "ma";
            kanaConvert["\u30e1"] = "me";
            kanaConvert["\u30e0"] = "mu";
            kanaConvert["\u30df"] = "mi";
            kanaConvert["\u30e2"] = "mo";
            kanaConvert["\u30ca"] = "na";
            kanaConvert["\u30cd"] = "ne";
            kanaConvert["\u30cc"] = "nu";
            kanaConvert["\u30cb"] = "ni";
            kanaConvert["\u30ce"] = "no";
            kanaConvert["\u30ef"] = "wa";
            kanaConvert["\u30f2"] = "wo";
            kanaConvert["\u30cf"] = "ha";
            kanaConvert["\u30d8"] = "he";
            kanaConvert["\u30db"] = "ho";
            kanaConvert["\u30b6"] = "za";
            kanaConvert["\u30bc"] = "ze";
            kanaConvert["\u30ba"] = "zu";
            kanaConvert["\u30c0"] = "da";
            kanaConvert["\u30c7"] = "de";
            kanaConvert["\u30c5"] = "du";
            kanaConvert["\u30c9"] = "do";
            kanaConvert["\u30d5"] = "fu";
            kanaConvert["\u30bf"] = "ta";
            kanaConvert["\u30c6"] = "te";
            kanaConvert["\u30c8"] = "to";
            kanaConvert["サ"] = "sa";
            kanaConvert["セ"] = "se";
            kanaConvert["ス"] = "su";
            kanaConvert["ソ"] = "so";
            kanaConvert["ぞ"] = "zo";
            kanaConvert["ゾ"] = "zo";
            kanaConvert["\u30f3"] = "n";

            _database = WordDatabase.Deserialise();
        }
        private string HintKana(string kana)
        {
            var result = kana.Replace("\u3058\u3083", "jya")
                .Replace("\u3058\u3085", "jyu")
                .Replace("\u3058\u3087", "jyo")
                .Replace("\u3057\u3083", "sha")
                .Replace("\u3057\u3085", "shu")
                .Replace("\u3057\u3087", "sho")
                .Replace("\u3061\u3083", "cha")
                .Replace("\u3061\u3085", "chu")
                .Replace("\u3061\u3087", "cho")
                .Replace("\u30b8\u30e3", "jya")
                .Replace("\u30b8\u30e5", "jyu")
                .Replace("\u30b8\u3087", "jyo")
                .Replace("\u30b7\u30e3", "sha")
                .Replace("\u30b7\u30e5", "shu")
                .Replace("\u30b7\u30e7", "sho")
                .Replace("\u30c1\u30e3", "cha")
                .Replace("\u30c1\u30e5", "chu")
                .Replace("\u30c1\u30e7", "cho");

            var finalResult = "";

            for (int i = 0, l = result.Length; i < l; i++)
            {
                var chr = result[i].ToString();

                if (kanaConvert.TryGetValue(chr, out string romaji))
                {
                    finalResult += romaji;
                }
                else
                {
                    finalResult += chr;
                }

            }
            return finalResult;
        }

        public string FindWords(string input)
        {
            var foundWords = new List<Word>();
            var result = "";

            foreach (var word in _database)
            {
                if (word.ToString().Contains(input))
                {
                    foundWords.Add(word);
                }
            }
            foundWords.OrderBy((a) => { return a.ToString().Length; });

            for (var i = 0; i < 15; i++)
            {
                if (i >= foundWords.Count)
                {
                    break;
                }
                result += $"{HintWord(foundWords[i])}\r\n";
            }

            return result.TrimEnd('\n').TrimEnd('\r');
        }

        public string FindWordsByMeaning(string input)
        {
            var foundWords = new List<(Word, string)>();
            var result = "";

            foreach (var word in _database)
            {
                foreach (var meaning in word.Meanings)
                {
                    if ($" {meaning.ToString().TrimEnd('.')} ".ToLower().Contains($" {input.ToLower()} "))
                    {
                        foundWords.Add((word, meaning.ToString()));
                        break;
                    }
                }
            }
            foundWords.OrderBy((a) => { return a.Item2.Length; });

            for (var i = 0; i < 15; i++)
            {
                if(i >= foundWords.Count)
                {
                    break;
                }
                result += $"{HintWordWithMeaning(foundWords[i].Item1,foundWords[i].Item2)}\r\n";
            }

            return result.TrimEnd('\n').TrimEnd('\r');
        }

        //This still has issues due to some words overlapping. For example it will detect "今日は" as "konnichiha" (hello), instead of the more likely 今日 は, kyou ha (today is). Possible solution is to check for such overlaps (perhaps include them in the database) and rerun the detection with the alternate interpretation. This would help preserve the integrity of the rest of the sentence, but could potentially slowdown the process exponentially, and also won't fix the above scenario. 
        public List<Word> FindWordsInString(string source)
        {
            var results = new List<Word>();
            var searchResults = new List<Word>();
            Word lastLastPossibleWord = null;
            Word lastPossibleWord = null;
            var lastReturnIndex = 0;
            var returnIndex = 0;
            var searchIndex = 0;
            var endSearch = false;
            var index = 0;

            searchResults.AddRange(_database);

            while (index < source.Length)
            {
                var currentChar = source[index];

                if (Language.IsJapaneseChar(currentChar))
                {
                    searchResults.RemoveAll((word) =>
                    {
                        var wordText = word.ToString();

                        if (wordText.Length > searchIndex)
                        {
                            return wordText[searchIndex] != source[index];
                        }
                        return true;
                    });

                    if (searchResults.Count > 0)
                    {
                        foreach (var searchResult in searchResults)
                        {
                            if (searchResult.ToString().Length == searchIndex + 1)
                            {
                                lastLastPossibleWord = lastPossibleWord;
                                lastPossibleWord = searchResult;
                                lastReturnIndex = returnIndex;
                                returnIndex = index;
                            }
                        }
                        searchIndex++;
                    }
                    else
                    {
                        endSearch = true;
                    }
                }
                else
                {
                    endSearch = true;
                }
                if (endSearch)
                {
                    if (lastPossibleWord != null)
                    {
                        var last = lastPossibleWord.Value.Text.Last();

                        if ((last == 'は' || last == 'に' || last == 'が' || last == 'を' || last == 'の') && lastPossibleWord.ToString().Length != 1)
                        {
                            results.Add(lastLastPossibleWord);
                            lastLastPossibleWord = null;
                            lastPossibleWord = null;
                            index = lastReturnIndex;
                        }
                        else
                        {
                            results.Add(lastPossibleWord);
                            lastLastPossibleWord = null;
                            lastPossibleWord = null;
                            index = returnIndex;
                        }
                    }
                    else
                    {
                        returnIndex = index + 1;
                    }
                    if (searchResults.Count < _database.Count)
                    {
                        searchResults.Clear();
                        searchResults.AddRange(_database);
                    }
                    searchIndex = 0;
                    endSearch = false;
                }
                index++;
            }
            if (lastPossibleWord != null)
            {
                results.Add(lastPossibleWord);
            }

            return results;
        }

        public string HintWordWithMeaning(Word input, string meaning)
        {
            if (input is Kanji kanji)
            {
                return $"{kanji} ({HintKana(kanji.ReadingList[0].ToString())}):{meaning}";
            }
            else if (input is Kana kana)
            {
                return $"{kana} ({HintKana(kana.ToString())}):{meaning}";
            }
            return "";
        }

        public string HintWord(Word input)
        {
            if (input is Kanji kanji)
            {
                if (kanji.ReadingList.Count > 0 && kanji.Meanings.Count > 0)
                {
                    return $"{kanji} ({HintKana(kanji.ReadingList[0].ToString())}):{kanji.Meanings[0]}";
                }
            }
            else if (input is Kana kana)
            {
                if (kana.Meanings.Count > 0)
                {
                    return $"{kana} ({HintKana(kana.ToString())}):{kana.Meanings[0]}";
                }
            }
            return "";
        }

        public string AddHinting(string input)
        {
            var entries = FindWordsInString(input);
            var result = "";

            foreach (var entry in entries)
            {
                if (entry is Kanji kanji)
                {
                    if (kanji.ReadingList.Count > 0 && kanji.Meanings.Count > 0)
                    {
                        result += $"{kanji} ({HintKana(kanji.ReadingList[0].ToString())}):{kanji.Meanings[0]}\r\n";
                    }
                }
                else if (entry is Kana kana)
                {
                    if (kana.Meanings.Count > 0)
                    {
                        result += $"{kana}({HintKana(kana.ToString())}):{kana.Meanings[0]}\r\n";
                    }
                }
            }

            return result;
        }

    }
}
