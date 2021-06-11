using System;
using System.Runtime.InteropServices;
using WinKey = System.Windows.Forms.Keys;

namespace TranslateTool
{
    public struct Hotkey
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static bool operator ==(Hotkey left, Hotkey right)
        {
            return left.Key == right.Key && left.Modifier == right.Modifier;
        }
        public static bool operator !=(Hotkey left, Hotkey right)
        {
            return left.Key != right.Key && left.Modifier != right.Modifier;
        }
        public KeyModifier Modifier;
        public WinKey Key;

        public Hotkey(KeyModifier modifier, WinKey key)
        {
            Modifier = modifier;
            Key = key;
        }

        public void Register(IntPtr hWnd, int id)
        {
            RegisterHotKey(hWnd, id, (int)Modifier, (int)Key);
        }

        public void Unregister(IntPtr hWnd, int id)
        {
            UnregisterHotKey(hWnd, id);
        }

        public override bool Equals(object obj)
        {
            return obj is Hotkey hotkey &&
                   Modifier == hotkey.Modifier &&
                   Key == hotkey.Key;
        }

        public override int GetHashCode()
        {
            var hashCode = 503722236;
            hashCode = hashCode * -1521134295 + Modifier.GetHashCode();
            hashCode = hashCode * -1521134295 + Key.GetHashCode();
            return hashCode;
        }

        public bool IsValid()
        {
            if(Modifier == KeyModifier.None)
            {
                return false;
            }
            if(Key == WinKey.None)
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            var result = string.Empty;

            if(Modifier.HasFlag(KeyModifier.Control))
            {
                result += "Ctrl + ";
            }
            if (Modifier.HasFlag(KeyModifier.Alt))
            {
                result += "Alt + ";
            }
            if (Modifier.HasFlag(KeyModifier.Shift))
            {
                result += "Shift + ";
            }
            if (Key != WinKey.None)
            {
                result += Key.ToString();
            }
            if(result == string.Empty)
            {
                result = "...";
            }
            return result;
        }
    }
}
