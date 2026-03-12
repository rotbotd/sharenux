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
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace ShareX.HelpersLib
{
    public class BlackStyleButton : Avalonia.Controls.Button
    {
        private bool isHover;

        private static readonly IBrush BackgroundNormal = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromRgb(105, 105, 105), 0),
                new GradientStop(Color.FromRgb(65, 65, 65), 1)
            }
        };

        private static readonly IBrush BackgroundHover = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromRgb(115, 115, 115), 0),
                new GradientStop(Color.FromRgb(75, 75, 75), 1)
            }
        };

        private static readonly IBrush BorderBrush = new SolidColorBrush(Color.FromRgb(30, 30, 30));

        public BlackStyleButton()
        {
            Foreground = Brushes.White;
            FontFamily = new FontFamily("Arial");
            FontSize = 12;
            BorderBrush = BlackStyleButton.BorderBrush;
            BorderThickness = new Thickness(1);
            Padding = new Thickness(8, 4);
            Background = BackgroundNormal;
        }

        protected override void OnPointerEntered(PointerEventArgs e)
        {
            base.OnPointerEntered(e);
            isHover = true;
            Background = BackgroundHover;
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);
            isHover = false;
            Background = BackgroundNormal;
        }
    }
}
