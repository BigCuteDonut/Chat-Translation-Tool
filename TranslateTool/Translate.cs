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
using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslateTool
{
    public struct PackageTypes
    {
        public const short Finished = -1;
        public const short NoType = 0;
        public const short TranslateInputTo = 1;
        public const short TranslateInputFrom = 2;
        public const short TranslateChat = 3;
        public const short MoveBrowser = 11;
        public const short TranslatedChat = 100;
        public const short TranslatedInput = 101;
        public const short TranslatedInputConfirmation = 102;
        public const short HinterTranslation = 104;
        public const short HinterMeaning = 105;
        public const short TranslatedInputHint = 106;
    }

    public unsafe class DataPackage
    {
        public const int TextSize = 1400;
        public const int NameSize = 60;
        public const int SizeOf = 20 + TextSize + NameSize;
        public const int TypeOffset = 0;
        public const int AgeOffset = 2;
        public const int TextByteLengthOffset = 4;
        public const int NameByteLengthOffset = 8;
        public const int NameOffset = 12;
        public const int TextOffset = NameOffset + NameSize;

        public IntPtr Pointer;

        public DataPackage(IntPtr pointer)
        {
            Pointer = pointer;
            Type = -1;
            Age = 0;
            TextByteLength = 0;
        }

        public short Type
        {
            get => (sbyte)Marshal.ReadInt16(Pointer, TypeOffset);
            set => Marshal.WriteInt16(Pointer, TypeOffset, value);
        }
        public short Age
        {
            get => Marshal.ReadInt16(Pointer, AgeOffset);
            set => Marshal.WriteInt16(Pointer, AgeOffset, value);
        }
        public int TextByteLength
        {
            get => Marshal.ReadInt32(Pointer, TextByteLengthOffset);
            set => Marshal.WriteInt32(Pointer, TextByteLengthOffset, value);
        }
        public int NameByteLength
        {
            get => Marshal.ReadInt32(Pointer, NameByteLengthOffset);
            set => Marshal.WriteInt32(Pointer, NameByteLengthOffset, value);
        }
        public string Name
        {
            get => Encoding.UTF8.GetString((byte*)Pointer + NameOffset, NameByteLength);
            set
            {
                var bytes = Encoding.UTF8.GetBytes(value);

                if (bytes.Length > NameSize)
                {
                    bytes = (byte[])bytes.Take(NameSize);
                }
                NameByteLength = bytes.Length;
                Marshal.Copy(bytes, 0, Pointer + NameOffset, bytes.Length);
            }
        }
        public string Text
        {
            get => Encoding.UTF8.GetString((byte*)Pointer + TextOffset, TextByteLength);
            set
            {
                var bytes = Encoding.UTF8.GetBytes(value);

                if (bytes.Length > TextSize)
                {
                    bytes = (byte[])bytes.Take(TextSize);
                }
                TextByteLength = bytes.Length;
                Marshal.Copy(bytes, 0, Pointer + TextOffset, bytes.Length);
            }
        }
        public void Clear()
        {
            Type = PackageTypes.Finished;
        }
    }
    public class Data
    {
        public const int PackageMax = 50;

        public IntPtr View;
        public DataPackage[] Packages;

        public Data(IntPtr view)
        {
            View = view;
            Packages = new DataPackage[Data.PackageMax];
            for (var i = 0; i < PackageMax; i++)
            {
                Packages[i] = new DataPackage(View + (i * DataPackage.SizeOf));
            }
        }
        public DataPackage this[int index]
        {
            get => Packages[index];
        }
        public void Foreach(Action<DataPackage> action)
        {
            for (int i = 0, l = PackageMax; i < l; i++)
            {
                action(Packages[i]);
            }
        }
        public DataPackage Add()
        {
            DataPackage result = null;
            DataPackage oldestPackage = null;
            var oldestAge = 0;

            Foreach((package) =>
            {
                if (package.Type == PackageTypes.Finished)
                {
                    if (result == null)
                    {
                        package.Age = 0;
                        package.Type = 0;
                        result = package;
                    }
                }
                package.Age++;

                if (package.Age > oldestAge)
                {
                    oldestAge = package.Age;
                    oldestPackage = package;
                }
            });
            if (result == null)
            {
                oldestPackage.Age = 0;
                oldestPackage.Type = 0;

                return oldestPackage;
            }

            return result;
        }
        public DataPackage FindType(short type)
        {
            for (var i = 0; i < Data.PackageMax; i++)
            {
                var package = Packages[i];

                if (package.Type == type)
                {
                    return package;
                }
            }
            return null;
        }
    }
    public static class Translate
    {
        private static string fileName;
        private static FileStream file;
        private static long lastFileLength;
        private static bool closeWatchStop = false;
        private static Thread thread;
        private static Thread browserThread;
        //TODO Redesign the data class to use safe code. Leftover from interop platform.
        private unsafe static byte* view;
        private static Data data;
        private static int cycles;
        private static Stack<Browser> translateFromBrowsers;
        private static Browser translateToBrowser;
        private static bool translateToBusy = false;
        private static bool inputInfoEnabled = false;
        private static Hinter hinter;
        private static int instanceCount = 4;
        private static Regex commandRegex;
        private static int retryCount = 0;
        public static Language Language;
        public static string Input;

        public class PhraseInfo
        {
            public int UseCount;
            public string Result;
            public string Text;
        }

        unsafe static Translate()
        {

            commandRegex = new Regex("/(?:(?:a|p|t)|(?:(?:cmf|camouflage) \\S+)|(?:(?:la|cla|mla|fla) \\S+)|(?:ce\\d)|(?:ceall)|(?:ceall (?:(?:on)|(?:off)))|(?:ci\\d \\d)|(?:ci\\d+)|(?:face\\d (?:(?:on)|(?:off)))|(?:face\\d)|(?:fc\\d (?:(?:on)|(?:off)))|(?:fc\\d)|(?:mn\\d+)|(?:mpal\\d)|(?:moya)|(?:spal\\d+)|(?:toge)|(?:symbol\\d+)|(?:vo\\d+)|(?:mf\\d+)|(?:sr\\d+))");
            view = (byte*)Marshal.AllocHGlobal(DataPackage.SizeOf * (Data.PackageMax + 1));
            data = new Data((IntPtr)view);
            fileName = "";
        }

        public static bool LoadFile()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var directory = new DirectoryInfo(Path.Combine(documentsPath, @"SEGA\PHANTASYSTARONLINE2\log"));
            FileInfo latestFile = null;
            DateTime mostRecentTime = new DateTime(0);

            foreach (var file in directory.GetFiles("ChatLog*.txt"))
            {
                if (file.CreationTime > mostRecentTime)
                {
                    mostRecentTime = file.CreationTime;
                    latestFile = file;
                }
            }

            if (latestFile != null)
            {
                if (fileName != latestFile.FullName)
                {
                    lastFileLength = new FileInfo(latestFile.FullName).Length;
                    fileName = latestFile.FullName;

                    if (file != null)
                    {
                        file.Dispose();
                        file = null;
                    }
                    file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    Form1.Form.WriteLine(string.Format(Language.Text[2], fileName));
                    file.Position = lastFileLength;

                    return true;
                }
            }

            return false;
        }

        public static void Start()
        {
            var browsers = new Browser[instanceCount];

            Language = Form1.Language;
            translateFromBrowsers = new Stack<Browser>(4);
            translateToBrowser = new Browser();
            translateToBrowser.AsyncOpenUrl("https://www.deepl.com/translator");
            for (var i = 0; i < instanceCount; i++)
            {
                browsers[i] = new Browser();
                translateFromBrowsers.Push(browsers[i]);
                browsers[i].AsyncOpenUrl("https://www.deepl.com/translator");
            }
            thread = new Thread(new ThreadStart(_CloseWatch));
            browserThread = new Thread(new ThreadStart(BrowserThread));
            thread.Start();
            browserThread.Start();
        }

        public static void StopBrowsers()
        {
            while (translateFromBrowsers.Count < instanceCount)
            {
                Thread.Sleep(50);
            }

            while (translateFromBrowsers.Count > 0)
            {
                var browser = translateFromBrowsers.Pop();

                browser.Page.Dispose();
            }
            translateToBrowser.Page.Dispose();
            closeWatchStop = true;
        }

        private static string RemoveCommand(string message)
        {
            if (message.Length > 1)
            {
                if (message[0] == '/')
                {
                    return commandRegex.Replace(message, "").Trim(' ', '"');
                }
                else
                {
                    if (message[0] == '"')
                    {
                        if (message[1] == '/')
                        {
                            return commandRegex.Replace(message, "").Trim(' ', '"');
                        }
                    }
                }
            }
            return message;
        }

        private static void HandleChatMessage(string name, string message)
        {
            var translated = false;

            message = RemoveCommand(message);

            if (message.Trim(' ') == string.Empty)
            {
                return;
            }

            foreach (var chr in message)
            {
                if (Language.IsTargetLanguageChar(chr))
                {
                    TranslateChat(name, message);
                    translated = true;
                    break;
                }
            }
            if (!translated)
            {
                Form1.Form.WriteLine($"{name}:\r\n  {message}");

            }
        }

        private static void HandleInputCommand()
        {
            if (Input != null)
            {
                if (Input.Length > 0)
                {
                    if (Input[0] != '/')
                    {
                        TranslateInputTo(Input);
                        Form1.Form.SetInputValue("");
                        Input = null;
                    }
                    else
                    {
                        if (Input.Trim(' ').ToLower() == "/jp")
                        {
                            if (!Language.IsJapanese)
                            {
                                var language = new Language("ja", Form1.VersionNumber);

                                Language = language;
                                Form1.Form.ApplyTranslation(language);
                                Form1.Form.SetInputValue($"");
                            }
                        }
                        else if (Input.Trim(' ').ToLower() == "/en")
                        {
                            if (Language.IsJapanese)
                            {
                                var language = new Language("en", Form1.VersionNumber);

                                Language = language;
                                Form1.Form.ApplyTranslation(language);
                                Form1.Form.SetInputValue($"");
                            }
                        }
                        else if (Input.Trim(' ').ToLower() == "/re")
                        {
                            RetryTranslateTo();
                            Form1.Form.SetInputValue("");
                        }
                        else if (Input.Trim(' ').ToLower() == "/help")
                        {
                            Form1.Form.WriteLine(Language.Text[8]);
                            Form1.Form.SetInputValue("");
                        }
                        else
                        {
                            var command = "";
                            if (Input.Length > 6)
                            {
                                command = Input.Substring(0, 6).ToLower();

                                if (command == "/from ")
                                {
                                    TranslateInputFrom(Input.TrimStart(' ').Substring(6));
                                    Form1.Form.SetInputValue($"");
                                }
                                else if (Input.Length > 8)
                                {
                                    command = Input.Substring(0, 8).ToLower();

                                    if (command == "/search ")
                                    {
                                        Form1.Form.WriteLine(HinterSearch(Input.TrimStart(' ').Substring(8)));
                                        Form1.Form.SetInputValue($"");
                                    }
                                    else if (command == "/engine ")
                                    {
                                        Form1.Form.WriteLine(Language.Text[9]);
                                        Form1.Form.SetInputValue($"");
                                    }
                                }
                            }
                            else
                            {
                                Form1.Form.WriteLine(string.Format(Language.Text[3], Input));
                                Form1.Form.SetInputValue($"");
                            }
                        }
                        Input = null;
                    }
                }
            }
        }

        private static void _CloseWatch()
        {
            var previousName = "";

            while (!LoadFile())
            {
                Thread.Sleep(100);
            }
            while (!closeWatchStop)
            {
                cycles++;

                if (cycles >= 1000)
                {
                    cycles = 0;
                    LoadFile();
                }
                var sizeDifference = new FileInfo(fileName).Length - lastFileLength;

                if (sizeDifference > 0 && sizeDifference < 1000)
                {
                    try
                    {
                        var bytes = new byte[sizeDifference];

                        file.Read(bytes, 0, (int)sizeDifference);
                        lastFileLength = lastFileLength + sizeDifference;

                        var newLines = Encoding.Unicode.GetString(bytes).Split('\n');

                        foreach (var newLine in newLines.Take(newLines.Length - 1))
                        {
                            var line = newLine.TrimEnd('\r');
                            var parts = line.Split('\t');
                            string time;
                            string unknown;
                            string type;
                            string userID;
                            string name;
                            string message;

                            if (parts.Length == 6)
                            {
                                time = parts[0];
                                unknown = parts[2];
                                type = parts[2];
                                userID = parts[3];
                                name = parts[4];
                                message = parts[5];
                            }
                            else
                            {
                                name = previousName;
                                message = newLine;
                            }
                            HandleChatMessage(name, message);
                            previousName = name;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (sizeDifference >= 1000)
                {
                    lastFileLength += sizeDifference;
                }
                HandleInputCommand();

                for (var i = 0; i < Data.PackageMax; i++)
                {
                    var package = data.Packages[i];

                    HandlePackage(package);
                }
                Thread.Sleep(50);
            }
        }

        private static void BrowserThread()
        {
            while (!closeWatchStop)
            {
                for (var i = 0; i < Data.PackageMax; i++)
                {
                    var package = data.Packages[i];

                    HandleBrowserPackage(package);
                }
                Thread.Sleep(50);
            }
        }

        private static void HandlePackage(DataPackage package)
        {
            if (package.Type > 99)
            {
                if (package.Type == PackageTypes.TranslatedChat)
                {
                    ProcessTranslatedChat(package);
                }
                else if (package.Type == PackageTypes.TranslatedInput)
                {
                    ProcessTranslatedInput(package);
                }
                else if (package.Type == PackageTypes.TranslatedInputConfirmation)
                {
                    ProcessTranslatedInputConfirmation(package);
                }
                else if (package.Type == PackageTypes.HinterMeaning)
                {
                    ProcessHinterMeaning(package);
                }
                else if (package.Type == PackageTypes.TranslatedInputHint)
                {
                    ProcessTranslatedInputHint(package);
                }
            }

        }

        private static async Task<string> TranslateAny(Browser browser, string url, string text)
        {
            if (text == "")
            {
                return text;
            }
            var taskCompletion = new TaskCompletionSource<string>();

            ThreadPool.QueueUserWorkItem((state) =>
            {
                var (completionSource, targetBrowser, textValue) = ((TaskCompletionSource<string>, Browser, string))state;

                if (targetBrowser.AsyncOpenUrl(url + Uri.EscapeDataString(textValue.Replace("/", "\u2215"))).GetAwaiter().GetResult())
                {
                    targetBrowser.PageInitialize();
                    while (true)
                    {
                        var result = targetBrowser.Page.EvaluateScriptAsync("document.querySelector(\"#dl_translator > div.lmt__sides_container > div.lmt__side_container.lmt__side_container--target > div.lmt__textarea_container > div.lmt__inner_textarea_container > textarea\").value").GetAwaiter().GetResult().Result.ToString();
                        var timeout = 8000;

                        if (result == "")
                        {
                            Thread.Sleep(50);
                            timeout -= 50;

                            if (timeout <= 0)
                            {
                                taskCompletion.SetResult(result);
                                break;
                            }
                        }
                        else
                        {
                            completionSource.SetResult(result);
                            break;
                        }
                    }
                }
            }, (taskCompletion, browser, text));

            return await taskCompletion.Task;
        }

        private static async Task<string> TranslateFrom(string text)
        {
            string link;

            if (Language.IsJapanese)
            {
                link = "https://www.deepl.com/translator#en/ja/";
            }
            else
            {
                link = "https://www.deepl.com/translator#ja/en/";
            }

            while (translateFromBrowsers.Count == 0)
            {
                Thread.Sleep(50);
            }
            var browser = translateFromBrowsers.Pop();
            var result = await TranslateAny(browser, link, text);

            translateFromBrowsers.Push(browser);

            return result;
        }

        private static async Task<string> TranslateTo(string text)
        {
            string link;

            if (Language.IsJapanese)
            {
                link = "https://www.deepl.com/translator#ja/en/";
            }
            else
            {
                link = "https://www.deepl.com/translator#en/ja/";
            }

            while (translateToBusy)
            {
                Thread.Sleep(50);
            }
            translateToBusy = true;

            var result = await TranslateAny(translateToBrowser, link, text);

            translateToBusy = false;

            return result;
        }

        private static async void ProcessTranslateChat(DataPackage package)
        {
            package.Type = PackageTypes.NoType;

            var result = await TranslateFrom(package.Text);

            package.Text = result;
            package.Type = PackageTypes.TranslatedChat;
        }

        private static async void ProcessTranslateInputFrom(DataPackage package)
        {
            package.Type = PackageTypes.NoType;

            var result = await TranslateFrom(package.Text);

            package.Text = result;
            package.Type = PackageTypes.TranslatedInput;
        }

        private static async void ProcessTranslateInputTo(DataPackage package)
        {
            package.Type = PackageTypes.NoType;

            var result = await TranslateTo(package.Text);

            package.Text = result;
            package.Type = PackageTypes.TranslatedInput;
            retryCount = 0;

            var confirmResult = await TranslateFrom(result);
            var confirmPackage = data.Add();

            confirmPackage.Text = confirmResult;
            confirmPackage.Type = PackageTypes.TranslatedInputConfirmation;

            if (inputInfoEnabled)
            {
                var hintPackage = data.Add();

                hintPackage.Text = hinter.AddHinting(result);
                hintPackage.Type = PackageTypes.TranslatedInputHint;
            }
        }

        public static void HandleBrowserPackage(DataPackage package)
        {
            if (package.Type < 100 && package.Type > 0)
            {
                while (translateFromBrowsers.Count == 0)
                {
                    Thread.Sleep(50);
                }
                if (package.Type == PackageTypes.TranslateChat)
                {
                    ProcessTranslateChat(package);
                }
                else if (package.Type == PackageTypes.TranslateInputFrom)
                {
                    ProcessTranslateInputFrom(package);
                }
                else if (package.Type == PackageTypes.TranslateInputTo && !translateToBusy)
                {
                    ProcessTranslateInputTo(package);
                }
            }
        }

        public static void TranslateChat(string name, string text)
        {
            var data = Translate.data.Add();

            data.Text = text;
            data.Name = name;
            data.Type = PackageTypes.TranslateChat;
        }

        public static void ProcessTranslatedChat(DataPackage package)
        {
            Form1.Form.WriteLine($"{package.Name}:\r\n{package.Text}");
            package.Clear();
        }

        public static void TranslateInputTo(string text)
        {
            var data = Translate.data.Add();

            data.Type = PackageTypes.TranslateInputTo;
            data.Text = text;
        }

        public static void ProcessTranslatedInput(DataPackage package)
        {
            var text = package.Text;
            var action = new Action(() => Clipboard.SetText(text));

            Form1.Form.Invoke(action);
            Form1.Form.WriteLine(string.Format(Language.Text[7], text));
            package.Clear();
        }

        public static void ProcessHinterMeaning(DataPackage package)
        {
            Form1.Form.WriteLine($"({package.Text})");
            package.Clear();
        }

        public static void ProcessTranslatedInputHint(DataPackage package)
        {
            Form1.Form.WriteLine($"Info:{package.Text}");
            package.Clear();
        }

        public static void ProcessTranslatedInputConfirmation(DataPackage package)
        {
            Form1.Form.WriteLine($"({package.Text})");
            package.Clear();
        }

        public static void TranslateInputFrom(string text)
        {
            var data = Translate.data.Add();

            data.Type = PackageTypes.TranslateInputFrom;
            data.Text = text;
        }

        public static string HinterSearch(string text)
        {
            var words = text.Split(' ').ToList();

            for (var i = 0; i < words.Count; i++)
            {
                if (words[i] == "")
                {
                    words.RemoveAt(i);
                    i--;
                }
            }
            if (hinter == null)
            {
                hinter = new Hinter();
            }
            return hinter.FindWordsByMeaning(words);
        }

        public static string HinterTranslate(string text)
        {
            try
            {
                return hinter.AddHinting(text);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static async void RetryTranslateTo()
        {
            object alternative = null;

            if (retryCount == 0)
            {
                alternative = translateToBrowser.Page.EvaluateScriptAsync("document.querySelector(\"#dl_translator > div.lmt__sides_container > div.lmt__side_container.lmt__side_container--target > div.lmt__textarea_container > div.lmt__translations_as_text > p:nth-child(3) > button.lmt__translations_as_text__text_btn\").innerText").GetAwaiter().GetResult().Result;
            }
            else if (retryCount == 1)
            {
                alternative = translateToBrowser.Page.EvaluateScriptAsync("document.querySelector(\"#dl_translator > div.lmt__sides_container > div.lmt__side_container.lmt__side_container--target > div.lmt__textarea_container > div.lmt__translations_as_text > p:nth-child(4) > button.lmt__translations_as_text__text_btn\").innerText").GetAwaiter().GetResult().Result;
            }
            else if (retryCount == 2)
            {
                alternative = translateToBrowser.Page.EvaluateScriptAsync("document.querySelector(\"#dl_translator > div.lmt__sides_container > div.lmt__side_container.lmt__side_container--target > div.lmt__textarea_container > div.lmt__translations_as_text > p:nth-child(5) > button.lmt__translations_as_text__text_btn\").innerText").GetAwaiter().GetResult().Result;
            }
            if (alternative == null)
            {
                Form1.Form.WriteLine(Language.Text[18]);
            }
            else
            {
                var result = alternative.ToString();
                var translationPackage = data.Add();

                translationPackage.Text = result;
                translationPackage.Type = PackageTypes.TranslatedInput;

                var confirmResult = await TranslateFrom(result);
                var confirmPackage = data.Add();

                confirmPackage.Text = confirmResult;
                confirmPackage.Type = PackageTypes.TranslatedInputConfirmation;
                retryCount++;

                if (inputInfoEnabled)
                {
                    var hintPackage = data.Add();

                    hintPackage.Text = hinter.AddHinting(result);
                    hintPackage.Type = PackageTypes.TranslatedInputHint;
                }
            }
        }

        public static void DisableAdditionalInputInfo()
        {
            inputInfoEnabled = false;
        }

        public static void EnableAdditionalInputInfo()
        {
            inputInfoEnabled = true;

            if (hinter == null)
            {
                hinter = new Hinter();
            }
        }

    }
}
