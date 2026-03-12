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
using Avalonia.Layout;
using Avalonia.Media;
using SkiaSharp;
using System;
using System.Globalization;

namespace ShareX.HelpersLib
{
    public class BlackStyleLabel : Avalonia.Controls.Control
    {
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<BlackStyleLabel, string>(nameof(Text), string.Empty);

        public static readonly StyledProperty<HorizontalAlignment> HorizontalTextAlignmentProperty =
            AvaloniaProperty.Register<BlackStyleLabel, HorizontalAlignment>(nameof(HorizontalTextAlignment), HorizontalAlignment.Left);

        public static readonly StyledProperty<VerticalAlignment> VerticalTextAlignmentProperty =
            AvaloniaProperty.Register<BlackStyleLabel, VerticalAlignment>(nameof(VerticalTextAlignment), VerticalAlignment.Top);

        public static readonly StyledProperty<SKColor> TextShadowColorProperty =
            AvaloniaProperty.Register<BlackStyleLabel, SKColor>(nameof(TextShadowColor), SKColors.Black);

        public static readonly StyledProperty<bool> DrawBorderProperty =
            AvaloniaProperty.Register<BlackStyleLabel, bool>(nameof(DrawBorder), false);

        public static readonly StyledProperty<SKColor> BorderColorProperty =
            AvaloniaProperty.Register<BlackStyleLabel, SKColor>(nameof(BorderColor), SKColors.Black);

        public static readonly StyledProperty<bool> AutoEllipsisProperty =
            AvaloniaProperty.Register<BlackStyleLabel, bool>(nameof(AutoEllipsis), false);

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public HorizontalAlignment HorizontalTextAlignment
        {
            get => GetValue(HorizontalTextAlignmentProperty);
            set => SetValue(HorizontalTextAlignmentProperty, value);
        }

        public VerticalAlignment VerticalTextAlignment
        {
            get => GetValue(VerticalTextAlignmentProperty);
            set => SetValue(VerticalTextAlignmentProperty, value);
        }

        public SKColor TextShadowColor
        {
            get => GetValue(TextShadowColorProperty);
            set => SetValue(TextShadowColorProperty, value);
        }

        public bool DrawBorder
        {
            get => GetValue(DrawBorderProperty);
            set => SetValue(DrawBorderProperty, value);
        }

        public SKColor BorderColor
        {
            get => GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public bool AutoEllipsis
        {
            get => GetValue(AutoEllipsisProperty);
            set => SetValue(AutoEllipsisProperty, value);
        }

        public BlackStyleLabel()
        {
            ClipToBounds = true;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            InvalidateVisual();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (string.IsNullOrEmpty(Text))
                return;

            var typeface = new Typeface(FontFamily.Default);
            var foreground = Foreground ?? Brushes.White;

            string displayText = Text;
            if (AutoEllipsis && Bounds.Width > 0)
            {
                var testText = new FormattedText(Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, FontSize, foreground);
                if (testText.Width > Bounds.Width)
                {
                    for (int i = Text.Length - 1; i > 0; i--)
                    {
                        displayText = Text.Substring(0, i) + "...";
                        testText = new FormattedText(displayText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, FontSize, foreground);
                        if (testText.Width <= Bounds.Width)
                            break;
                    }
                }
            }

            var formattedText = new FormattedText(displayText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, FontSize, foreground);

            double x = 0;
            double y = 0;

            switch (HorizontalTextAlignment)
            {
                case HorizontalAlignment.Center:
                    x = (Bounds.Width - formattedText.Width) / 2;
                    break;
                case HorizontalAlignment.Right:
                    x = Bounds.Width - formattedText.Width;
                    break;
            }

            switch (VerticalTextAlignment)
            {
                case VerticalAlignment.Center:
                    y = (Bounds.Height - formattedText.Height) / 2;
                    break;
                case VerticalAlignment.Bottom:
                    y = Bounds.Height - formattedText.Height;
                    break;
            }

            // Draw shadow
            if (TextShadowColor.Alpha > 0)
            {
                var shadowBrush = new SolidColorBrush(Color.FromArgb(TextShadowColor.Alpha, TextShadowColor.Red, TextShadowColor.Green, TextShadowColor.Blue));
                var shadowText = new FormattedText(displayText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, FontSize, shadowBrush);
                context.DrawText(shadowText, new Point(x, y + 1));
            }

            // Draw text
            context.DrawText(formattedText, new Point(x, y));

            // Draw border
            if (DrawBorder)
            {
                var borderBrush = new SolidColorBrush(Color.FromArgb(BorderColor.Alpha, BorderColor.Red, BorderColor.Green, BorderColor.Blue));
                var pen = new Pen(borderBrush, 1);
                context.DrawRectangle(pen, new Rect(0, 0, Bounds.Width - 1, Bounds.Height - 1));
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (string.IsNullOrEmpty(Text))
                return new Size(0, FontSize);

            var typeface = new Typeface(FontFamily.Default);
            var formattedText = new FormattedText(Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, FontSize, Brushes.White);
            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}
