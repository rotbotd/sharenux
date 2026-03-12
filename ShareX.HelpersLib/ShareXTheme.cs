#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using Avalonia.Media;
using SkiaSharp;
using System.Collections.Generic;
using System.ComponentModel;

namespace ShareX.HelpersLib
{
    public class ShareXTheme
    {
        public string Name { get; set; }

        private SKColor backgroundColor;

        public SKColor BackgroundColor
        {
            get => backgroundColor;
            set
            {
                if (value.Alpha > 0) backgroundColor = value;
            }
        }

        private SKColor lightBackgroundColor;

        public SKColor LightBackgroundColor
        {
            get => lightBackgroundColor;
            set
            {
                if (value.Alpha > 0) lightBackgroundColor = value;
            }
        }

        private SKColor darkBackgroundColor;

        public SKColor DarkBackgroundColor
        {
            get => darkBackgroundColor;
            set
            {
                if (value.Alpha > 0) darkBackgroundColor = value;
            }
        }

        private SKColor textColor;

        public SKColor TextColor
        {
            get => textColor;
            set
            {
                if (value.Alpha > 0) textColor = value;
            }
        }

        private SKColor borderColor;

        public SKColor BorderColor
        {
            get => borderColor;
            set
            {
                if (value.Alpha > 0) borderColor = value;
            }
        }

        public SKColor CheckerColor { get; set; }

        public SKColor CheckerColor2 { get; set; }

        public int CheckerSize { get; set; } = 15;

        public SKColor LinkColor { get; set; }

        public SKColor MenuHighlightColor { get; set; }

        public SKColor MenuHighlightBorderColor { get; set; }

        public SKColor MenuBorderColor { get; set; }

        public SKColor MenuCheckBackgroundColor { get; set; }

        public string MenuFontFamily { get; set; } = "Segoe UI";
        public float MenuFontSize { get; set; } = 9.75f;

        public string ContextMenuFontFamily { get; set; } = "Segoe UI";
        public float ContextMenuFontSize { get; set; } = 9.75f;

        public int ContextMenuOpacity { get; set; } = 100;

        [Browsable(false)]
        public double ContextMenuOpacityDouble => System.Math.Clamp(ContextMenuOpacity, 10, 100) / 100d;

        public SKColor SeparatorLightColor { get; set; }

        public SKColor SeparatorDarkColor { get; set; }

        [Browsable(false)]
        public bool IsDarkTheme => ColorHelpers.IsDarkColor(BackgroundColor);

        // Avalonia color conversions
        public Avalonia.Media.Color GetAvaloniaBackgroundColor() => ToAvaloniaColor(BackgroundColor);
        public Avalonia.Media.Color GetAvaloniaTextColor() => ToAvaloniaColor(TextColor);
        public Avalonia.Media.Color GetAvaloniaBorderColor() => ToAvaloniaColor(BorderColor);
        public Avalonia.Media.Color GetAvaloniaLinkColor() => ToAvaloniaColor(LinkColor);

        private static Avalonia.Media.Color ToAvaloniaColor(SKColor c) => 
            Avalonia.Media.Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);

        private ShareXTheme()
        {
        }

        public static ShareXTheme DarkTheme => new ShareXTheme()
        {
            Name = "Dark",
            BackgroundColor = new SKColor(39, 39, 39),
            LightBackgroundColor = new SKColor(46, 46, 46),
            DarkBackgroundColor = new SKColor(34, 34, 34),
            TextColor = new SKColor(231, 233, 234),
            BorderColor = new SKColor(31, 31, 31),
            CheckerColor = new SKColor(46, 46, 46),
            CheckerColor2 = new SKColor(39, 39, 39),
            LinkColor = new SKColor(166, 212, 255),
            MenuHighlightColor = new SKColor(46, 46, 46),
            MenuHighlightBorderColor = new SKColor(63, 63, 63),
            MenuBorderColor = new SKColor(63, 63, 63),
            MenuCheckBackgroundColor = new SKColor(51, 51, 51),
            SeparatorLightColor = new SKColor(44, 44, 44),
            SeparatorDarkColor = new SKColor(31, 31, 31)
        };

        public static ShareXTheme LightTheme => new ShareXTheme()
        {
            Name = "Light",
            BackgroundColor = new SKColor(242, 242, 242),
            LightBackgroundColor = new SKColor(247, 247, 247),
            DarkBackgroundColor = new SKColor(235, 235, 235),
            TextColor = new SKColor(69, 69, 69),
            BorderColor = new SKColor(201, 201, 201),
            CheckerColor = new SKColor(247, 247, 247),
            CheckerColor2 = new SKColor(235, 235, 235),
            LinkColor = new SKColor(166, 212, 255),
            MenuHighlightColor = new SKColor(247, 247, 247),
            MenuHighlightBorderColor = new SKColor(96, 143, 226),
            MenuBorderColor = new SKColor(201, 201, 201),
            MenuCheckBackgroundColor = new SKColor(225, 233, 244),
            SeparatorLightColor = new SKColor(253, 253, 253),
            SeparatorDarkColor = new SKColor(189, 189, 189)
        };

        public static ShareXTheme NightTheme => new ShareXTheme()
        {
            Name = "Night",
            BackgroundColor = new SKColor(42, 47, 56),
            LightBackgroundColor = new SKColor(52, 57, 65),
            DarkBackgroundColor = new SKColor(28, 32, 38),
            TextColor = new SKColor(235, 235, 235),
            BorderColor = new SKColor(28, 32, 38),
            CheckerColor = new SKColor(60, 60, 60),
            CheckerColor2 = new SKColor(50, 50, 50),
            LinkColor = new SKColor(166, 212, 255),
            MenuHighlightColor = new SKColor(30, 34, 40),
            MenuHighlightBorderColor = new SKColor(116, 129, 152),
            MenuBorderColor = new SKColor(22, 26, 31),
            MenuCheckBackgroundColor = new SKColor(56, 64, 75),
            SeparatorLightColor = new SKColor(56, 64, 75),
            SeparatorDarkColor = new SKColor(22, 26, 31)
        };

        // https://www.nordtheme.com
        public static ShareXTheme NordDarkTheme => new ShareXTheme()
        {
            Name = "Nord Dark",
            BackgroundColor = new SKColor(46, 52, 64),
            LightBackgroundColor = new SKColor(59, 66, 82),
            DarkBackgroundColor = new SKColor(38, 44, 57),
            TextColor = new SKColor(229, 233, 240),
            BorderColor = new SKColor(30, 38, 54),
            CheckerColor = new SKColor(46, 52, 64),
            CheckerColor2 = new SKColor(36, 42, 54),
            LinkColor = new SKColor(136, 192, 208),
            MenuHighlightColor = new SKColor(36, 42, 54),
            MenuHighlightBorderColor = new SKColor(24, 30, 42),
            MenuBorderColor = new SKColor(24, 30, 42),
            MenuCheckBackgroundColor = new SKColor(59, 66, 82),
            SeparatorLightColor = new SKColor(59, 66, 82),
            SeparatorDarkColor = new SKColor(30, 38, 54)
        };

        // https://www.nordtheme.com
        public static ShareXTheme NordLightTheme => new ShareXTheme()
        {
            Name = "Nord Light",
            BackgroundColor = new SKColor(229, 233, 240),
            LightBackgroundColor = new SKColor(236, 239, 244),
            DarkBackgroundColor = new SKColor(216, 222, 233),
            TextColor = new SKColor(59, 66, 82),
            BorderColor = new SKColor(207, 216, 233),
            CheckerColor = new SKColor(229, 233, 240),
            CheckerColor2 = new SKColor(216, 222, 233),
            LinkColor = new SKColor(106, 162, 178),
            MenuHighlightColor = new SKColor(236, 239, 244),
            MenuHighlightBorderColor = new SKColor(207, 216, 233),
            MenuBorderColor = new SKColor(216, 222, 233),
            MenuCheckBackgroundColor = new SKColor(229, 233, 240),
            SeparatorLightColor = new SKColor(236, 239, 244),
            SeparatorDarkColor = new SKColor(207, 216, 233)
        };

        // https://draculatheme.com
        public static ShareXTheme DraculaTheme => new ShareXTheme()
        {
            Name = "Dracula",
            BackgroundColor = new SKColor(40, 42, 54),
            LightBackgroundColor = new SKColor(68, 71, 90),
            DarkBackgroundColor = new SKColor(36, 38, 48),
            TextColor = new SKColor(248, 248, 242),
            BorderColor = new SKColor(33, 35, 43),
            CheckerColor = new SKColor(40, 42, 54),
            CheckerColor2 = new SKColor(36, 38, 48),
            LinkColor = new SKColor(98, 114, 164),
            MenuHighlightColor = new SKColor(36, 38, 48),
            MenuHighlightBorderColor = new SKColor(255, 121, 198),
            MenuBorderColor = new SKColor(33, 35, 43),
            MenuCheckBackgroundColor = new SKColor(45, 47, 61),
            SeparatorLightColor = new SKColor(45, 47, 61),
            SeparatorDarkColor = new SKColor(33, 35, 43)
        };

        public static List<ShareXTheme> GetDefaultThemes()
        {
            return new List<ShareXTheme>() { DarkTheme, LightTheme, NightTheme, NordDarkTheme, NordLightTheme, DraculaTheme };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
