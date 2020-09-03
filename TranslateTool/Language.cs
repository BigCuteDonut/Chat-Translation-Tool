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
using System.Globalization;
using System.IO;
using System.Text;
using TranslateTool.Properties;

namespace TranslateTool
{
    public class Language
    {
        public readonly bool IsJapanese;
        public string[] Text { get; private set; } = new string[48];

        //Probably should redesign the layout of the language file, it's kinda dumb.
        public Language(string languageCode, string version)
        {
            var fileName = "";
            var fileText = "";
            var reading = false;
            var indexMatching = false;
            var endMatching = false;
            var messageIndex = 0;
            var lastChar = '\0';
            var messageBuilder = new StringBuilder();
            var settingValue = Settings.Default.Language;

            if (languageCode == string.Empty)
            {
                if (settingValue != "None")
                {
                    if (settingValue == "Japanese")
                    {
                        languageCode = "ja";
                    }
                    else if (settingValue == "English")
                    {
                        languageCode = "en";
                    }
                }
                else
                {
                    languageCode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                }
            }
           
            if (languageCode == "ja")
            {
                fileName = "Japanese.txt";
                IsJapanese = true;
                Text[11] = $"バグや問題点の報告は、Twitterの@CuteDonut3か、PSO2のBreadButterflyまでご連絡ください。このプログラムを開発している人は日本語を書くのが苦手なのでご注意ください。また、言語の修正を報告していただいても構いません。\n\nVer. {version}.";
                Settings.Default.Language = "Japanese";
                Settings.Default.Save();
            }
            else
            {
                fileName = "English.txt";
                IsJapanese = false;
                Text[11] = $"Please contact @CuteDonut on Twitter or BreadButterfly in-game(PSO2) to report bugs/issues. \n\nVersion {version}.";
                Settings.Default.Language = "English";
                Settings.Default.Save();
            }
            fileText = File.ReadAllText(Environment.CurrentDirectory + $@"\Language\{fileName}");

            foreach (var chr in fileText)
            {
                if (reading)
                {
                    if (chr == ';')
                    {
                        if (endMatching)
                        {
                            if (messageIndex != 11)
                            {
                                Text[messageIndex] = messageBuilder.ToString();
                            }
                            messageBuilder.Clear();
                            messageIndex++;
                            reading = false;
                            endMatching = false;
                        }
                        else
                        {
                            endMatching = true;
                        }
                    }
                    else
                    {
                        if (endMatching)
                        {
                            messageBuilder.Append(';');
                            endMatching = false;
                        }
                        messageBuilder.Append(chr);
                    }
                }
                else if (indexMatching)
                {
                    if (lastChar == ':')
                    {
                        if (chr != ' ')
                        {
                            messageBuilder.Append(chr);
                        }
                        lastChar = '\0';
                        reading = true;
                    }
                    else if (chr != ':' && chr != ' ' && !char.IsDigit(chr))
                    {
                        indexMatching = false;
                    }
                    else
                    {
                        lastChar = chr;
                    }
                }
                else if (char.IsDigit(chr))
                {
                    indexMatching = true;
                }
            }
        }

        public Language(string version) : this("", version)
        {
        }

        private static bool IsJapaneseChar(char chr)
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

        private static bool IsEnglishChar(char chr)
        {
            return
                ((chr >= 97 && chr <= 122) ||
                (chr >= 65 && chr <= 90)) &&
                chr != 'w';
        }

        public bool IsTargetLanguageChar(char chr)
        {
            if (!IsJapanese)
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
