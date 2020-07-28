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
using System.Text;

namespace TranslateTool
{
    class SimpleDictionary
    {
        public List<char> Keys = new List<char>();
        public List<SortedItem> Values = new List<SortedItem>();

        public SortedItem this[char chr]
        {
            get
            {
                var index = 0;

                foreach (var key in Keys)
                {
                    if (key == chr)
                    {
                        return Values[index];
                    }
                    index++;
                }

                return null;
            }
            set
            {
                var index = Keys.IndexOf(chr);

                if (index >= 0)
                {
                    Values[index] = value;
                }
                else
                {
                    Keys.Add(chr);
                    Values.Add(value);
                }
            }
        }

        public bool TryGetValue(char key, out SortedItem item)
        {
            item = this[key];

            if (item != null)
            {
                return true;
            }

            return false;
        }
    }
    class Entry
    {
        public string WithKanji;
        public string Reading;
        public string Meaning;
    }
    class SortedItem
    {
        public Entry Result;
        public SimpleDictionary Next;
    }
    class Hinter
    {
        private SimpleDictionary _database;
        private Dictionary<string, string> kanaConvert;
        private byte[] fileBytes;

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
            kanaConvert["\u3063"] = "tsu";
            kanaConvert["\u3061"] = "chi";
            kanaConvert["\u3060"] = "chi";
            kanaConvert["\u3057"] = "shi";
            kanaConvert["\u3056"] = "shi";
            kanaConvert["\u3058"] = "ji";
            kanaConvert["\u3057"] = "ji";
            kanaConvert["\u30C4"] = "tsu";
            kanaConvert["\u30C3"] = "tsu";
            kanaConvert["\u30c1"] = "chi";
            kanaConvert["\u30c0"] = "chi";
            kanaConvert["\u30b7"] = "shi";
            kanaConvert["\u30b6"] = "shi";
            kanaConvert["\u30b8"] = "ji";
            kanaConvert["\u30b7"] = "ji";
            kanaConvert["\u3084"] = "ya";
            kanaConvert["\u3083"] = "ya";
            kanaConvert["\u3086"] = "yu";
            kanaConvert["\u3085"] = "yu";
            kanaConvert["\u3088"] = "yo";
            kanaConvert["\u3087"] = "yo";
            kanaConvert["\u30e4"] = "ya";
            kanaConvert["\u30e3"] = "ya";
            kanaConvert["\u30e6"] = "yu";
            kanaConvert["\u30e5"] = "yu";
            kanaConvert["\u30e8"] = "yo";
            kanaConvert["\u30e7"] = "yo";
            kanaConvert["\u3042"] = "a";
            kanaConvert["\u3041"] = "a";
            kanaConvert["\u3048"] = "e";
            kanaConvert["\u3047"] = "e";
            kanaConvert["\u3044"] = "i";
            kanaConvert["\u3043"] = "i";
            kanaConvert["\u304A"] = "o";
            kanaConvert["\u3049"] = "o";
            kanaConvert["\u3046"] = "u";
            kanaConvert["\u3045"] = "u";
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
            kanaConvert["\u30a1"] = "a";
            kanaConvert["\u30a8"] = "e";
            kanaConvert["\u30a7"] = "e";
            kanaConvert["\u30a6"] = "u";
            kanaConvert["\u30a5"] = "u";
            kanaConvert["\u30a4"] = "i";
            kanaConvert["\u30a3"] = "i";
            kanaConvert["\u30aa"] = "o";
            kanaConvert["\u30a9"] = "o";
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
            kanaConvert["\u30f3"] = "n";

            fileBytes = File.ReadAllBytes("WordDatabase.bin");
            var index = 0;

            _database = ReverseProcess(ref index);
            fileBytes = null;
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
        private List<Entry> SubSearchMeaning(SortedItem current, List<string> input)
        {
            var results = new List<Entry>();
            var matched = true;

            if (current.Result != null)
            {
                if (current.Result.Meaning.Length < 60 && current.Result.Meaning.Length > 0)
                {
                    var check = " " + current.Result.Meaning.ToLower() + " ";

                    for (int i = 0, l = input.Count; i < l; i++)
                    {
                        if (!check.Contains(" " + input[i] + " "))
                        {
                            matched = false;
                        }
                    }
                    if (matched)
                    {
                        results.Add(current.Result);
                    }
                }
            }
            foreach (var value in current.Next.Values)
            {
                results.AddRange(SubSearchMeaning(value, input));
            }
            return results;
        }

        public List<Entry> SearchMeaning(List<string> input)
        {
            var results = new List<Entry>();

            for (int i = 0, l = input.Count; i < l; i++)
            {
                input[i] = input[i].ToLower();
            }

            foreach (var value in _database.Values)
            {
                results.AddRange(SubSearchMeaning(value, input));
            }

            return results;
        }

        public string FindWordsByMeaning(List<string> input)
        {
            var result = "";
            var i = 0;
            var searchResult = SearchMeaning(input);

            searchResult.Sort((a, b) => { return a.Meaning.Length - b.Meaning.Length; });

            foreach (var entry in searchResult)
            {
                if ((entry.WithKanji ?? "").Length > 0)
                {
                    result += $"{entry.WithKanji} ({entry.Reading}|{ HintKana(entry.Reading)}):{entry.Meaning}\r\n";
                }
                else
                {
                    result += $"{entry.Reading}({HintKana(entry.Reading)}):{entry.Meaning}\r\n";
                }
                if (i++ == 10)
                {
                    break;
                }
            }

            return result;
        }

        public (Entry Result, int Missed) FindEntryInString(string source, int startIndex)
        {
            Entry lastValidResult = null;
            var current = _database;
            var startedFrom = startIndex - 1;
            var index = startIndex;
            var missed = -1;

            while (lastValidResult == null)
            {
                missed++;
                startedFrom++;
                index = startedFrom;

                if (index >= source.Length)
                {
                    return (null, missed); ;
                }
                while (index < source.Length)
                {
                    if (current.TryGetValue(source[index], out SortedItem value))
                    {
                        current = value.Next;

                        if (value.Result != null)
                        {
                            lastValidResult = value.Result;
                        }
                    }
                    else
                    {
                        break;
                    }
                    index++;
                }
            }
            return (lastValidResult, missed);
        }

        public List<Entry> FindEntriesInString(string source)
        {
            var index = 0;
            var results = new List<Entry>();

            while (index < source.Length)
            {
                var entry = FindEntryInString(source, index);

                if (entry.Result != null)
                {
                    results.Add(entry.Result);

                    if ((entry.Result.WithKanji ?? "").Length > 0)
                    {
                        index += entry.Result.WithKanji.Length + entry.Missed;
                    }
                    else
                    {
                        index += entry.Result.Reading.Length + entry.Missed;
                    }
                }
                else
                {
                    break;
                }
            }

            return results;
        }

        public string AddHinting(string input)
        {
            var entries = FindEntriesInString(input);
            var result = "";

            foreach (var entry in entries)
            {
                if ((entry.WithKanji ?? "").Length > 0)
                {
                    result += $"{entry.WithKanji} ({entry.Reading}|{HintKana(entry.Reading)}):{entry.Meaning}\r\n";
                }
                else
                {
                    result += $"{entry.Reading}({HintKana(entry.Reading)}):{entry.Meaning}\r\n";
                }
            }

            return result;
        }

        ushort GetShort(ref int indexToOffset)
        {
            var value = BitConverter.ToUInt16(fileBytes, indexToOffset);

            indexToOffset += 2;

            return value;
        }

        string GetString(ref int indexToOffset)
        {
            var length = GetShort(ref indexToOffset);
            var value = Encoding.UTF8.GetString(fileBytes, indexToOffset, length);

            indexToOffset += length;

            return value;
        }

        byte GetByte(ref int indexToOffset)
        {
            var value = fileBytes[indexToOffset];

            indexToOffset += 1;

            return value;
        }

        SortedItem ReverseProcessItem(ref int index)
        {
            var result = new SortedItem();

            if (GetByte(ref index) == 1)
            {
                var entry = new Entry();

                entry.WithKanji = GetString(ref index);
                entry.Reading = GetString(ref index);
                entry.Meaning = GetString(ref index);
                result.Result = entry;
            }
            else
            {
                result.Result = null;
            }
            result.Next = ReverseProcess(ref index);

            return result;
        }

        SimpleDictionary ReverseProcess(ref int index)
        {
            var result = new SimpleDictionary();
            var keyCount = GetShort(ref index);
            var keyIndex = 0;

            while (keyIndex < keyCount)
            {
                var key = (char)GetShort(ref index);

                result[key] = ReverseProcessItem(ref index);
                keyIndex++;
            }

            return result;
        }

    }
}
