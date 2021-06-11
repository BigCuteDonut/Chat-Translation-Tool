using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Input;
using TranslateTool;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using WinKey = System.Windows.Forms.Keys;

namespace ChatTranslator
{
    public class HotkeyInputHandler
    {
        private  Hotkey selectingHotkey;
        private  bool hotkeyInput = false;
        private  Button button;
        private  string hotkeyMessageID;
        private  Setting<Hotkey> setting;
        private  bool oldSettingClickThrough;
        private  bool oldSettingOCR;

        public HotkeyInputHandler(Button button, string hotkeyMessageID, Setting<Hotkey> setting)
        {
            this.button = button;
            this.hotkeyMessageID = hotkeyMessageID;
            this.setting = setting;
            UpdateHotkeySelectMessage(setting);
        }

        private  bool IsModifierKey(WinKey key)
        {
            return key == WinKey.LShiftKey || key == WinKey.RShiftKey || key == WinKey.LControlKey || key == WinKey.RControlKey || key == WinKey.Alt || key == WinKey.ControlKey || key == WinKey.ShiftKey || key == WinKey.Shift || key == WinKey.Control;
        }

        private  void UpdateHotkeySelectMessage(Hotkey hotkey)
        {
            button.Content = string.Format(MainWindowLogic.Language[hotkeyMessageID], hotkey.ToString());
        }

        private  EventHandler<TranslateTool.KeyEventArgs> CreateKeyDownHandler()
        {
            return (sender, eventArgs) =>
            {
                if (hotkeyInput)
                {
                    if (IsModifierKey(eventArgs.Key))
                    {
                        selectingHotkey.Modifier = SettingsWindow.Keyboard.Modifier;
                        button.Content = string.Format(MainWindowLogic.Language[hotkeyMessageID], selectingHotkey.ToString());
                        SettingsWindow.Keyboard.KeyDownQueue.Add(CreateKeyDownHandler());
                    }
                    else
                    {
                        selectingHotkey.Modifier = SettingsWindow.Keyboard.Modifier;
                        selectingHotkey.Key = eventArgs.Key;

                        if (selectingHotkey.IsValid())
                        {
                            FinalizeHotkeyInput();
                        }
                        else
                        {
                            button.Content = string.Format(MainWindowLogic.Language[hotkeyMessageID], selectingHotkey.ToString());
                            SettingsWindow.Keyboard.KeyDownQueue.Add(CreateKeyDownHandler());
                        }
                    }
                }
            };
        }

        private  void FinalizeHotkeyInput()
        {
            SettingsWindow.Keyboard.KeyDownQueue.Clear();
            hotkeyInput = false;

            if (selectingHotkey.IsValid())
            {
                setting.Value = selectingHotkey;
                UpdateHotkeySelectMessage(selectingHotkey);
            }
            Settings.ClickthroughKeyEnabled.Value = oldSettingClickThrough;
            Settings.OCRKeyEnabled.Value = oldSettingOCR;
        }

        public  RoutedEventHandler CreateHotkeySelectBinding()
        {

            return (sender, eventArgs) =>
            {
                if (!hotkeyInput && SettingsWindow.windowConfigured)
                {
                    oldSettingClickThrough = Settings.ClickthroughKeyEnabled;
                    oldSettingOCR = Settings.OCRKeyEnabled;
                    Settings.ClickthroughKeyEnabled.Value = false;
                    Settings.OCRKeyEnabled.Value = false;
                    hotkeyInput = true;
                    selectingHotkey = new Hotkey();
                    UpdateHotkeySelectMessage(selectingHotkey);
                    SettingsWindow.Keyboard.KeyDownQueue.Add(CreateKeyDownHandler());
                }
            };
        }
    }
}
