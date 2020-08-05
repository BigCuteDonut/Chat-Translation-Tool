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
using System.Threading;
using System.Threading.Tasks;

namespace TranslateTool
{
    public struct PackageTypes
    {
        public const short Finished = -1;
        public const short Operating = 0;
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
    public class DataPackage
    {
        public Data Parent;

        public DataPackage(Data parent)
        {
            Type = -1;
            Age = 0;
            Parent = parent;
        }

        public short Type;
        public short Age;
        public object Color;
        public string Name;
        public string Text;

        public void Clear()
        {
            Type = PackageTypes.Finished;
            Parent.RemovePackage(this);
        }

        public void SetTextLimited(string text)
        {
            if(text.Length > 700)
            {
                text = text.Substring(0, 700);
            }
            Text = text;
        }
    }

    public class Data
    {
        public const int PackageMax = 50;
        private readonly List<DataPackage> packages;
        private DataPackage[] packagesCopy = new DataPackage[0];
        private bool packagesCopied = true;

        public Data()
        {
            packages = new List<DataPackage>();
        }

        public DataPackage this[int index]
        {
            get => packages[index];
        }

        public DataPackage[] Packages
        {
            get
            {
                if(!packagesCopied)
                {
                    AwaitDuplicant().GetAwaiter().GetResult();
                }

                return packagesCopy;
            }
        }
        private async Task<bool> AwaitDuplicant()
        {
            var taskCompletion = new TaskCompletionSource<bool>();

            ThreadPool.QueueUserWorkItem((state) =>
            {
                while(!packagesCopied)
                {
                    Thread.Sleep(1);
                }
                taskCompletion.SetResult(true);
            });

            return await taskCompletion.Task;
        }
        public void RemovePackage(DataPackage package)
        {
            packages.Remove(package);
        }
        public DataPackage AddPackage()
        {
            foreach(var package in packages)
            {
                package.Age++;

                if (package.Age > PackageMax)
                {
                    package.Type = PackageTypes.Finished;
                    package.Clear();
                }
            }
            var result = new DataPackage(this);

            packages.Add(result);
            packagesCopied = false;
            packagesCopy = packages.ToArray();
            packagesCopied = true;

            return result;
        }
    }
}
