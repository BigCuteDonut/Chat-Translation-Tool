/*
    ARKS Translator
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

namespace ChatTranslator
{
    public static class AppColours
    {
        private static System.Windows.Media.Color frame;
        private static System.Windows.Media.Color frameHighlight;
        private static System.Windows.Media.Color textBackground;
        private static System.Windows.Media.Color text;
        private static System.Windows.Media.Color selectedTab;
        private static System.Windows.Media.Color unselectedTab;
        private static System.Windows.Media.Color groupBackground;
        private static System.Windows.Media.Color highlight;
        private static System.Windows.Media.Color checkmarkGlyph;
        private static System.Windows.Media.Color checkmarkGlyphHighlight;
        private static System.Windows.Media.Color checkmarkBackground;
        private static System.Windows.Media.Color checkmarkBackgroundHighlight;
        private static System.Windows.Media.Color checkmarkBorderHighlight;
        private static System.Windows.Media.Color checkmarkBackgroundDisabled;
        private static System.Windows.Media.Color scrollBorder;
        private static System.Windows.Media.Color scrollHandle;
        private static System.Windows.Media.Color scrollHighlight;
        private static System.Windows.Media.Color scrollBackground;
        private static System.Windows.Media.Color scrollGlyph;
        private static System.Windows.Media.Color decoration;
        private static System.Windows.Media.Color decorationShine;

        public static System.Windows.Media.Color Frame
        {
            get
            {
                return frame;
            }
            set
            {
                App.AppResources["FrameColor"] = value;
                frame = value;
            }
        }
        public static System.Windows.Media.Color FrameHighlight
        {
            get
            {
                return frameHighlight;
            }
            set
            {
                App.AppResources["FrameHighlightColor"] = value;
                frameHighlight = value;
            }
        }
        public static System.Windows.Media.Color TextBackground
        {
            get
            {
                return textBackground;
            }
            set
            {
                App.AppResources["TextBackgroundColor"] = value;
                textBackground = value;
            }
        }
        public static System.Windows.Media.Color Text
        {
            get
            {
                return text;
            }
            set
            {
                App.AppResources["TextColor"] = value;
                text = value;
            }
        }
        public static System.Windows.Media.Color SelectedTab
        {
            get
            {
                return selectedTab;
            }
            set
            {
                App.AppResources["SelectedTabColor"] = value;
                selectedTab = value;
            }
        }
        public static System.Windows.Media.Color UnselectedTab
        {
            get
            {
                return unselectedTab;
            }
            set
            {
                App.AppResources["UnselectedTabColor"] = value;
                unselectedTab = value;
            }
        }
        public static System.Windows.Media.Color GroupBackground
        {
            get
            {
                return groupBackground;
            }
            set
            {
                App.AppResources["GroupBackgroundColor"] = value;
                groupBackground = value;
            }
        }
        public static System.Windows.Media.Color Highlight
        {
            get
            {
                return highlight;
            }
            set
            {
                App.AppResources["HighlightColor"] = value;
                highlight = value;
            }
        }
        public static System.Windows.Media.Color CheckmarkGlyph
        {
            get
            {
                return checkmarkGlyph;
            }
            set
            {
                App.AppResources["CheckmarkGlyphColor"] = value;
                checkmarkGlyph = value;
            }
        }
        public static System.Windows.Media.Color CheckmarkGlyphHighlight
        {
            get
            {
                return checkmarkGlyphHighlight;
            }
            set
            {
                App.AppResources["CheckmarkGlyphHighlightColor"] = value;
                checkmarkGlyphHighlight = value;
            }
        }
        public static System.Windows.Media.Color CheckmarkBackground
        {
            get
            {
                return checkmarkBackground;
            }
            set
            {
                App.AppResources["CheckmarkBackgroundColor"] = value;
                checkmarkBackground = value;
            }
        }
        public static System.Windows.Media.Color CheckmarkBackgroundHighlight
        {
            get
            {
                return checkmarkBackgroundHighlight;
            }
            set
            {
                App.AppResources["CheckmarkBackgroundHighlightColor"] = value;
                checkmarkBackgroundHighlight = value;
            }
        }
        public static System.Windows.Media.Color CheckmarkBorderHighlight
        {
            get
            {
                return checkmarkBorderHighlight;
            }
            set
            {
                App.AppResources["CheckmarkBorderHighlightColor"] = value;
                checkmarkBorderHighlight = value;
            }
        }
        public static System.Windows.Media.Color CheckmarkBackgroundDisabled
        {
            get
            {
                return checkmarkBackgroundDisabled;
            }
            set
            {
                App.AppResources["CheckmarkBackgroundDisabledColor"] = value;
                checkmarkBackgroundDisabled = value;
            }
        }
        public static System.Windows.Media.Color ScrollBorder
        {
            get
            {
                return scrollBorder;
            }
            set
            {
                App.AppResources["ScrollBorderColor"] = value;
                scrollBorder = value;
            }
        }
        public static System.Windows.Media.Color ScrollHandle
        {
            get
            {
                return scrollHandle;
            }
            set
            {
                App.AppResources["ScrollHandleColor"] = value;
                scrollHandle = value;
            }
        }
        public static System.Windows.Media.Color ScrollHighlight
        {
            get
            {
                return scrollHighlight;
            }
            set
            {
                App.AppResources["ScrollHighlightColor"] = value;
                scrollHighlight = value;
            }
        }
        public static System.Windows.Media.Color ScrollBackground
        {
            get
            {
                return scrollBackground;
            }
            set
            {
                App.AppResources["ScrollBackgroundColor"] = value;
                scrollBackground = value;
            }
        }
        public static System.Windows.Media.Color ScrollGlyph
        {
            get
            {
                return scrollGlyph;
            }
            set
            {
                App.AppResources["ScrollGlyphColor"] = value;
                scrollGlyph = value;
            }
        }
        public static System.Windows.Media.Color Decoration
        {
            get
            {
                return decoration;
            }
            set
            {
                App.AppResources["DecorationColor"] = value;
                decoration = value;
            }
        }
        public static System.Windows.Media.Color DecorationShine
        {
            get
            {
                return decorationShine;
            }
            set
            {
                App.AppResources["DecorationShineColor"] = value;
                decorationShine = value;
            }
        }
    }
}
