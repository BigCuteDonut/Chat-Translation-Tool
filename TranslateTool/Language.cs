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
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace TranslateTool
{
    public class Language
    {
        private static Dictionary<string, string> ReadLanguageConfig(string file)
        {
            var text = File.ReadAllText(file);
            var result = new Dictionary<string, string>();
            var key = new StringBuilder();
            var value = new StringBuilder();
            var getKey = true;
            var ignore = false;
            var previousChar = '\0';
            var index = 0;

            while (index < text.Length)
            {
                var current = text[index];

                if (current == '`' && previousChar != '\\')
                {
                    ignore = !ignore;
                    previousChar = current;
                    index++;
                    continue;
                }
                if (!ignore)
                {
                    if (!(current == '\\' && previousChar != '\\'))
                    {
                        if (getKey)
                        {
                            if (current == '=')
                            {
                                getKey = false;
                            }
                            else if (!char.IsWhiteSpace(current))
                            {
                                key.Append(current);
                            }
                        }
                        else
                        {
                            if (current == ';' && previousChar != '\\')
                            {
                                getKey = true;
                                result[key.ToString()] = value.ToString();
                                key.Clear();
                                value.Clear();
                            }
                            else
                            {
                                if (!char.IsWhiteSpace(current) || value.Length > 0)
                                {
                                    value.Append(current);
                                }
                            }
                        }
                    }
                }
                previousChar = current;
                index++;
            }

            return result;
        }
        public readonly UserLanguage Current;
        public Dictionary<string, string> Text;

        public Language(string version)
        {
            Current = Settings.Language.Value;
            Text = Language.ReadLanguageConfig(Path.GetFullPath(Path.Combine(Settings.LanguageFileDirectory,$"{Settings.Language.Value}.txt")));

            if (Settings.Language.Value == UserLanguage.English)
            {
                Text["Introduction"] = $"Please contact @CuteDonut3 on Twitter or BreadButterfly in-game(PSO2) to report bugs/issues. \n\nVersion {version}.";
            }
            else if (Settings.Language.Value == UserLanguage.Japanese)
            {
                Text["Introduction"] = $"バグや問題点の報告は、Twitterの@CuteDonut3か、PSO2のBreadButterflyまでご連絡ください。このプログラムを開発している人は日本語を書くのが苦手なのでご注意ください。また、言語の修正を報告していただいても構いません。\n\nVer. {version}.";
            }
        }

        public string GetTranslateFromLink()
        {
            if(Current == UserLanguage.English)
            {
                return "https://www.deepl.com/translator#ja/en/";
            }
            if(Current == UserLanguage.Japanese)
            {
                return "https://www.deepl.com/translator#en/ja/";
            }
            return "";
        }

        public string GetTranslateToLink()
        {
            if (Current == UserLanguage.English)
            {
                return "https://www.deepl.com/translator#en/ja/";
            }
            if (Current == UserLanguage.Japanese)
            {
                return "https://www.deepl.com/translator#ja/en/";
            }
            return "";
        }

        public static bool IsJapaneseChar(char chr)
        {
            return
                (chr >= 0x3041 && chr <= 0x3096) ||
                (chr >= 0x30A0 && chr <= 0x30FF) ||
                (chr >= 0x3400 && chr <= 0x4DB5) ||
                (chr >= 0x4E00 && chr <= 0x9FCB) ||
                (chr >= 0xF900 && chr <= 0xFA6A) ||
                (chr >= 0x2E80 && chr <= 0x2FD5) ||
                (chr >= 0xFF5F && chr <= 0xFF9F);
        }

        public static bool IsEnglishChar(char chr)
        {
            return
                ((chr >= 97 && chr <= 122) ||
                (chr >= 65 && chr <= 90)) &&
                chr != 'w';
        }

        public bool IsTargetLanguageChar(char chr)
        {
            if (Current != UserLanguage.Japanese)
            {
                return IsJapaneseChar(chr);
            }
            else
            {
                return IsEnglishChar(chr);
            }
        }

    }
}
