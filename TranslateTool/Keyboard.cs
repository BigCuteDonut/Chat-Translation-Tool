using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinKey = System.Windows.Forms.Keys;
using System.Windows.Interop;

namespace TranslateTool
{
    public class KeyEventArgs : EventArgs
    {
        public readonly WinKey Key;

        public KeyEventArgs(WinKey key)
        {
            Key = key;
        }
    }
    public class Keyboard
    {
        private readonly Dictionary<WinKey, bool> keyStates;

        public readonly List<EventHandler<KeyEventArgs>> KeyDownQueue;
        public readonly List<EventHandler<KeyEventArgs>> KeyUpQueue;
        public KeyModifier Modifier = KeyModifier.None;

        public Keyboard(HwndSource hook)
        {
            keyStates = new Dictionary<WinKey, bool>();
            hook.AddHook(WndProc);
            KeyDownQueue = new List<EventHandler<KeyEventArgs>>();
            KeyUpQueue = new List<EventHandler<KeyEventArgs>>();
        }

        public bool this[WinKey key]
        {
            get
            {
                bool keyState;

                if (keyStates.TryGetValue(key, out keyState))
                {
                    return keyState;
                }
                else
                {
                    keyState = false;
                    keyStates[key] = keyState;

                    return keyState;
                }
            }
            private set
            {
                keyStates[key] = value;
            }
        }

        private void UpdateModifier(WinKey key, bool down)
        {
            if (down)
            {
                if (key == WinKey.LShiftKey || key == WinKey.RShiftKey || key == WinKey.ShiftKey || key == WinKey.Shift)
                {
                    Modifier |= KeyModifier.Shift;
                }
                else if (key == WinKey.LControlKey || key == WinKey.RControlKey || key == WinKey.ControlKey || key == WinKey.Control)
                {
                    Modifier |= KeyModifier.Control;
                }
                else if (key == WinKey.Alt)
                {
                    Modifier |= KeyModifier.Alt;
                }
            }
            else
            {
                if (key == WinKey.LShiftKey || key == WinKey.RShiftKey || key == WinKey.ShiftKey || key == WinKey.Shift)
                {
                    Modifier ^= KeyModifier.Shift;
                }
                else if (key == WinKey.LControlKey || key == WinKey.RControlKey || key == WinKey.ControlKey || key == WinKey.Control)
                {
                    Modifier ^= KeyModifier.Control;
                }
                else if (key == WinKey.Alt)
                {
                    Modifier ^= KeyModifier.Alt;
                }
            }
        }

        private IntPtr WndProc(IntPtr windowHandle, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message == 0x0100 || message == 0x0104)
            {
                var key = (WinKey)wParam;

                if (!this[key])
                {
                    var eventArgs = new KeyEventArgs(key);
                    var queueCopy = KeyDownQueue.ToArray();

                    KeyDownQueue.Clear();
                    this[key] = true;
                    UpdateModifier(key, true);

                    foreach (var keyDown in queueCopy)
                    {
                        keyDown(this, eventArgs);
                    }
                }
            }
            else if (message == 0x0101 || message == 0x0105)
            {
                var key = (WinKey)wParam;
                var eventArgs = new KeyEventArgs(key);
                var queueCopy = KeyUpQueue.ToArray();

                KeyUpQueue.Clear();
                this[key] = false;
                UpdateModifier(key, false);

                foreach (var keyUp in queueCopy)
                {
                    keyUp(this, eventArgs);
                }
            }
            return IntPtr.Zero;
        }
    }
}
