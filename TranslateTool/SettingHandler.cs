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
using System.Runtime.InteropServices;
using System.Text;

namespace TranslateTool
{
    public static unsafe class FileStreamExtensions
    {
        public static void WriteValue<T>(this FileStream fileStream, T value)
            where T : unmanaged
        {
            var bytes = new byte[sizeof(T)];

            Marshal.Copy((IntPtr)(&value), bytes, 0, bytes.Length);
            fileStream.Write(bytes, 0, bytes.Length);
        }

        public static T ReadValue<T>(this FileStream fileStream)
            where T : unmanaged
        {
            var tSize = sizeof(T);
            var bytes = new byte[tSize];
            var result = default(T);

            fileStream.Read(bytes, 0, tSize);
            Marshal.Copy(bytes, 0, (IntPtr)(&result), tSize);

            return result;
        }

        public static void WriteString(this FileStream fileStream, string value)
        {
            var bytes = Encoding.Unicode.GetBytes(value);

            fileStream.WriteValue(bytes.Length);
            fileStream.Write(bytes, 0, bytes.Length);
        }

        public static string ReadString(this FileStream fileStream)
        {
            var length = fileStream.ReadValue<int>();
            var bytes = new byte[length];

            fileStream.Read(bytes, 0, length);

            return Encoding.Unicode.GetString(bytes);
        }
    }
    public class SettingChangedEventArgs<T>
    {
        private readonly T oldValue;
        private readonly T newValue;

        public SettingChangedEventArgs(T oldValue, T newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public T OldValue => oldValue;
        public T NewValue => newValue;
    }
    public class Setting<T>
        where T : unmanaged
    {
        public static Setting<T> operator &(Setting<T> leftValue, T rightValue)
        {
            leftValue.Value = rightValue;

            return leftValue;
        }
        public static implicit operator T(Setting<T> value)
        {
            return value.Value;
        }

        private FileStream settingFile;
        private int pointer;
        private T value;
        public event EventHandler<SettingChangedEventArgs<T>> OnChanged;

        public Setting(int pointer, FileStream settingFile)
        {
            this.pointer = pointer;
            this.settingFile = settingFile;
            value = ReadValue();
        }

        public T Value
        {
            get => value;
            set
            {
                WriteValue(value);
                OnChanged?.Invoke(this, new SettingChangedEventArgs<T>(this.value, value));
                this.value = value;
            }
        }

        private T ReadValue()
        {
            settingFile.Position = pointer;

            return settingFile.ReadValue<T>();
        }

        private void WriteValue(T value)
        {
            settingFile.Position = pointer;
            settingFile.WriteValue(value);
            settingFile.Flush();
        }
    }

    public class KeyPointer
    {
        public string Key;
        public int Value;

        public KeyPointer(string key, int value)
        {
            Key = key;
            Value = value;
        }
    }

    public static class SettingHandler
    {
        private const string settingsPath = @"..\Data\Settings.bin";
        private const string pointersPath = @"..\Data\Pointers.bin";

        private static readonly Dictionary<string, int> pointers = new Dictionary<string, int>();
        private static FileStream pointerFile;
        private static FileStream settingFile;

        static SettingHandler()
        {
            Load();
        }

        private static void Load()
        {
            int count;
            var firstTime = false;

            if (!Directory.Exists(Path.GetDirectoryName(pointersPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(pointersPath));
                firstTime = true;
            }
            else if(!File.Exists(pointersPath) || !File.Exists(settingsPath))
            {
                firstTime = true;
            }
            if(firstTime)
            {
                File.WriteAllBytes(pointersPath,BitConverter.GetBytes(0));
                File.WriteAllBytes(settingsPath,new byte[0]);
            }

            pointerFile = new FileStream(pointersPath, FileMode.Open, FileAccess.ReadWrite);
            settingFile = new FileStream(settingsPath, FileMode.Open, FileAccess.ReadWrite);
            count = pointerFile.ReadValue<int>();

            for (var i = 0; i < count; i++)
            {
                var key = pointerFile.ReadString();
                var pointer = pointerFile.ReadValue<int>();

                pointers[key] = pointer;
            }
        }

        private static int AddPointer<T>(string name, T defaultValue)
            where T : unmanaged
        {
            int pointer;

            pointerFile.Position = 0;
            pointerFile.WriteValue(pointers.Count + 1); 
            pointerFile.Position = pointerFile.Length;
            pointerFile.WriteString(name);
            pointer = (int)settingFile.Length;
            pointerFile.WriteValue(pointer);
            settingFile.Position = pointer;
            settingFile.WriteValue(defaultValue);

            return pointer;
        }

        public static Setting<T> LoadSetting<T>(string name, T defaultValue)
            where T : unmanaged
        {
            int pointer;
    
            if (pointers.TryGetValue(name, out pointer))
            {
                return new Setting<T>(pointer, settingFile);
            }
            pointer = AddPointer(name, defaultValue);
            pointers[name] = pointer;

            return new Setting<T>(pointer, settingFile);
        }
    }
}
