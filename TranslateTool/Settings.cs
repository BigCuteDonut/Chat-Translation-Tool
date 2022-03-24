using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Globalization;
using WinKey = System.Windows.Forms.Keys;

namespace TranslateTool
{
    public enum KeyModifier
    {
        None = 0,
        Alt = 0x4001,
        Control = 0x4002,
        Shift = 0x4004,
        Windows = 0x4008,
    }
    public enum UserLanguage
    {
        English,
        Japanese
    }
    public class Settings
    {
        public const string LanguageFileDirectory = "../Language";
        public const string JSFileDirectory = "../JS";

        public static UserLanguage GetDefaultLanguage()
        {
            var languageCode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            if(languageCode == "ja")
            {
                return UserLanguage.Japanese;
            }
            else
            {
                return UserLanguage.English;
            }
        }

        public static event EventHandler<SettingChangedEventArgs<UserLanguage>> OnLanguageChanged;
        public static Language Language;
        public static Setting<bool> FirstTimeStartup = SettingHandler.LoadSetting("FirstTimeStartup", true);
        public static Setting<bool> PartialTransparency = SettingHandler.LoadSetting("PartialOpacityEnabled", true);
        public static Setting<bool> Clickthrough = SettingHandler.LoadSetting("ClickthroughEnabled", false);
        public static Setting<bool> ClickthroughKeyEnabled= SettingHandler.LoadSetting("ClickthroughKeyEnabled", false);
        public static Setting<Hotkey> ClickthroughKey= SettingHandler.LoadSetting("ClickthroughKey", new Hotkey(KeyModifier.Control,WinKey.T));
        public static Setting<bool> OCRKeyEnabled = SettingHandler.LoadSetting("OCRKeyEnabled", false);
        public static Setting<Hotkey> OCRKey = SettingHandler.LoadSetting("OCRKey", new Hotkey(KeyModifier.Control, WinKey.G));
        public static Setting<FixedString> OCRDirectory = SettingHandler.LoadSetting<FixedString>("OCRDirectory", "Click to set directory.");
        public static Setting<Vector2> WindowSize = SettingHandler.LoadSetting("WindowSize", new Vector2(355,400));
        public static Setting<Vector2> WindowPosition = SettingHandler.LoadSetting("WindowPosition", new Vector2(355,400));
        public static Setting<bool> ColourChat = SettingHandler.LoadSetting("ColourChat", true);
        public static Setting<bool> AdditionalInfo = SettingHandler.LoadSetting("AdditionalInfo", false);
        private static Setting<UserLanguage> language = SettingHandler.LoadSetting("UserLanguage", GetDefaultLanguage());
        public static Setting<bool> AutoShow = SettingHandler.LoadSetting("AutoShow", false);
        public static Setting<bool> AutoHide = SettingHandler.LoadSetting("AutoHide", true);

        static Settings()
        {
            Language = new Language(MainWindowLogic.VersionNumber, language);
        }
        public static void ChangeLanguage(UserLanguage newLanguage)
        {
            OnLanguageChanged?.Invoke(language, new SettingChangedEventArgs<UserLanguage>(language.Value, newLanguage));
            language.Value = newLanguage;
        }

    }
}
