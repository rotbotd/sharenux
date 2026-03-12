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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using SkiaSharp;
using System;
using System.IO;
using System.Reflection;

namespace ShareX.HelpersLib
{
    public static class ShareXResources
    {
        public static string Name { get; set; } = "ShareX";

        public static string UserAgent => $"{Name}/{Helpers.GetApplicationVersion()}";

        public static bool IsDarkTheme => Theme.IsDarkTheme;

        public static ShareXTheme Theme { get; set; } = ShareXTheme.DarkTheme;

        private static SKBitmap logo;
        
        public static SKBitmap Logo
        {
            get
            {
                if (logo == null)
                {
                    logo = LoadEmbeddedBitmap("ShareX_Logo.png");
                }
                return logo?.Copy();
            }
            set
            {
                logo?.Dispose();
                logo = value;
            }
        }

        private static SKBitmap icon;
        private static SKBitmap iconWhite;
        private static bool useWhiteIcon;

        public static bool UseWhiteIcon
        {
            get => useWhiteIcon;
            set
            {
                if (useWhiteIcon != value)
                {
                    useWhiteIcon = value;
                }
            }
        }

        public static SKBitmap Icon
        {
            get
            {
                if (useWhiteIcon)
                {
                    iconWhite ??= LoadEmbeddedBitmap("ShareX_Icon_White.png");
                    return iconWhite?.Copy();
                }
                else
                {
                    icon ??= LoadEmbeddedBitmap("ShareX_Icon.png");
                    return icon?.Copy();
                }
            }
        }

        private static SKBitmap LoadEmbeddedBitmap(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fullName = $"ShareX.HelpersLib.Resources.{resourceName}";
            
            using var stream = assembly.GetManifestResourceStream(fullName);
            if (stream != null)
            {
                return SKBitmap.Decode(stream);
            }
            
            // Try without namespace prefix
            foreach (var name in assembly.GetManifestResourceNames())
            {
                if (name.EndsWith(resourceName))
                {
                    using var s = assembly.GetManifestResourceStream(name);
                    if (s != null)
                    {
                        return SKBitmap.Decode(s);
                    }
                }
            }
            
            return null;
        }

        public static void ApplyTheme(Window window, bool closeOnEscape = false)
        {
            if (closeOnEscape)
            {
                window.CloseOnEscape();
            }

            // Apply dark/light theme variant
            if (Application.Current != null)
            {
                Application.Current.RequestedThemeVariant = 
                    Theme.IsDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
            }

            // Set window background
            window.Background = new SolidColorBrush(Theme.GetAvaloniaBackgroundColor());
        }

        public static void ApplyThemeToControl(Avalonia.Controls.Control control)
        {
            // Avalonia handles most theming through styles and theme variants
            // This method is here for compatibility but most work is done via XAML styles
            
            if (control is Window window)
            {
                window.Background = new SolidColorBrush(Theme.GetAvaloniaBackgroundColor());
            }
        }

        public static Avalonia.Media.Color GetThemeColor(string colorName)
        {
            return colorName switch
            {
                "Background" => Theme.GetAvaloniaBackgroundColor(),
                "Text" => Theme.GetAvaloniaTextColor(),
                "Border" => Theme.GetAvaloniaBorderColor(),
                "Link" => Theme.GetAvaloniaLinkColor(),
                _ => Theme.GetAvaloniaTextColor()
            };
        }

        public static IBrush GetThemeBrush(string colorName)
        {
            return new SolidColorBrush(GetThemeColor(colorName));
        }
    }
}
