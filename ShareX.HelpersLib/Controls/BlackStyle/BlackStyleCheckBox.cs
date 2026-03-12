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
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.ComponentModel;

namespace ShareX.HelpersLib
{
    public class BlackStyleCheckBox : ToggleButton
    {
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<BlackStyleCheckBox, string>(nameof(Text), string.Empty);

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool Checked
        {
            get => IsChecked ?? false;
            set => IsChecked = value;
        }

        public int SpaceAfterCheckBox { get; set; } = 3;
        public bool IgnoreClick { get; set; }

        public event EventHandler CheckedChanged;

        private const int CheckBoxSize = 13;

        private static readonly IBrush BackgroundBrush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromRgb(105, 105, 105), 0),
                new GradientStop(Color.FromRgb(55, 55, 55), 1)
            }
        };

        private static readonly IBrush BackgroundCheckedBrush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromRgb(102, 163, 226), 0),
                new GradientStop(Color.FromRgb(83, 135, 186), 0.49),
                new GradientStop(Color.FromRgb(75, 121, 175), 0.50),
                new GradientStop(Color.FromRgb(56, 93, 135), 1)
            }
        };

        private static readonly IPen InnerBorderPen = new Pen(new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromRgb(125, 125, 125), 0),
                new GradientStop(Color.FromRgb(65, 75, 75), 1)
            }
        }, 1);

        private static readonly IPen InnerBorderCheckedPen = new Pen(new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromRgb(133, 192, 241), 0),
                new GradientStop(Color.FromRgb(76, 119, 163), 1)
            }
        }, 1);

        private static readonly IPen BorderPen = new Pen(new SolidColorBrush(Color.FromRgb(30, 30, 30)), 1);

        private bool isHover;

        public BlackStyleCheckBox()
        {
            Foreground = Brushes.White;
            FontFamily = new FontFamily("Arial");
            FontSize = 8;
            Background = Brushes.Transparent;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsCheckedProperty)
            {
                CheckedChanged?.Invoke(this, EventArgs.Empty);
                InvalidateVisual();
            }
        }

        protected override void OnPointerEntered(PointerEventArgs e)
        {
            base.OnPointerEntered(e);
            isHover = true;
            InvalidateVisual();
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);
            isHover = false;
            InvalidateVisual();
        }

        protected override void OnClick()
        {
            if (!IgnoreClick)
            {
                base.OnClick();
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var checkBoxRect = new Rect(0, 0, CheckBoxSize, CheckBoxSize);
            var innerRect = new Rect(1, 1, CheckBoxSize - 2, CheckBoxSize - 2);
            var fillRect = new Rect(2, 2, CheckBoxSize - 4, CheckBoxSize - 4);

            // Draw checkbox background
            if (Checked)
            {
                context.FillRectangle(BackgroundCheckedBrush, fillRect);
                context.DrawRectangle(InnerBorderCheckedPen, innerRect);
            }
            else
            {
                context.FillRectangle(BackgroundBrush, fillRect);
                context.DrawRectangle(isHover ? InnerBorderCheckedPen : InnerBorderPen, innerRect);
            }

            // Draw border
            context.DrawRectangle(BorderPen, checkBoxRect);

            // Draw text
            if (!string.IsNullOrEmpty(Text))
            {
                var textX = CheckBoxSize + SpaceAfterCheckBox;
                var formattedText = new FormattedText(
                    Text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(FontFamily),
                    FontSize,
                    Foreground);

                // Draw shadow
                context.DrawText(
                    new FormattedText(Text, System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, new Typeface(FontFamily), FontSize, Brushes.Black),
                    new Avalonia.Point(textX, 1));

                // Draw text
                context.DrawText(formattedText, new Avalonia.Point(textX, 0));
            }
        }

        protected override Avalonia.Size MeasureOverride(Avalonia.Size availableSize)
        {
            var textWidth = 0.0;
            if (!string.IsNullOrEmpty(Text))
            {
                var formattedText = new FormattedText(
                    Text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(FontFamily),
                    FontSize,
                    Foreground);
                textWidth = formattedText.Width + SpaceAfterCheckBox;
            }

            return new Avalonia.Size(CheckBoxSize + textWidth, CheckBoxSize);
        }
    }
}
