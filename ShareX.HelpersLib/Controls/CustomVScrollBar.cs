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
using SkiaSharp;
using System;

namespace ShareX.HelpersLib
{
    public class CustomVScrollBar : Avalonia.Controls.Control
    {
        public event EventHandler ValueChanged;

        public static readonly StyledProperty<int> MinimumProperty =
            AvaloniaProperty.Register<CustomVScrollBar, int>(nameof(Minimum), 0);

        public static readonly StyledProperty<int> MaximumProperty =
            AvaloniaProperty.Register<CustomVScrollBar, int>(nameof(Maximum), 100);

        public static readonly StyledProperty<int> ValueProperty =
            AvaloniaProperty.Register<CustomVScrollBar, int>(nameof(Value), 0);

        public static readonly StyledProperty<int> PageSizeProperty =
            AvaloniaProperty.Register<CustomVScrollBar, int>(nameof(PageSize), 10);

        public int Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public int Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public int Value
        {
            get => GetValue(ValueProperty);
            set
            {
                int newValue = Math.Max(Minimum, Math.Min(value, Maximum));
                if (GetValue(ValueProperty) != newValue)
                {
                    SetValue(ValueProperty, newValue);
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public int PageSize
        {
            get => GetValue(PageSizeProperty);
            set => SetValue(PageSizeProperty, value);
        }

        public int SmallScrollStep { get; set; } = 20;
        public int LargeScrollStep { get; set; } = 100;

        private bool isDragging;
        private bool isThumbHovered;
        private double dragOffset = 0;

        public CustomVScrollBar()
        {
            ClipToBounds = true;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == MinimumProperty ||
                change.Property == MaximumProperty ||
                change.Property == ValueProperty ||
                change.Property == PageSizeProperty)
            {
                InvalidateVisual();
            }
        }

        private Rect GetThumbRectangle()
        {
            double trackHeight = Bounds.Height;
            int effectivePageSize = Math.Max(PageSize, 1);
            int effectiveMaximum = Math.Max(Maximum, 0);
            int sum = Math.Max(effectiveMaximum + effectivePageSize, 1);

            double thumbHeight = trackHeight * effectivePageSize / sum;
            thumbHeight = Math.Max(thumbHeight, 20);

            double movementRange = Math.Max(trackHeight - thumbHeight, 0);
            double thumbTop = effectiveMaximum > 0 ? movementRange * Value / effectiveMaximum : 0;

            return new Rect(0, thumbTop, Bounds.Width, thumbHeight);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            // Track background
            var trackColor = ToAvaloniaColor(ShareXResources.Theme.DarkBackgroundColor);
            context.FillRectangle(new SolidColorBrush(trackColor), new Rect(Bounds.Size));

            // Thumb
            var thumbRect = GetThumbRectangle();
            var thumbBaseColor = ShareXResources.Theme.LightBackgroundColor;
            var thumbColor = isThumbHovered 
                ? ToAvaloniaColor(ColorHelpers.LighterColor(thumbBaseColor, 0.1f))
                : ToAvaloniaColor(thumbBaseColor);
            context.FillRectangle(new SolidColorBrush(thumbColor), thumbRect);

            // Borders
            var borderColor = ToAvaloniaColor(ColorHelpers.DarkerColor(ShareXResources.Theme.DarkBackgroundColor, 0.03f));
            var borderPen = new Pen(new SolidColorBrush(borderColor), 1);
            
            context.DrawLine(borderPen, new Point(thumbRect.Left, thumbRect.Top), new Point(thumbRect.Right, thumbRect.Top));
            context.DrawLine(borderPen, new Point(thumbRect.Left, thumbRect.Bottom), new Point(thumbRect.Right, thumbRect.Bottom));
            context.DrawLine(borderPen, new Point(0, 0), new Point(0, Bounds.Height));
            context.DrawLine(borderPen, new Point(Bounds.Width - 1, 0), new Point(Bounds.Width - 1, Bounds.Height));
        }

        private static Avalonia.Media.Color ToAvaloniaColor(SKColor c) =>
            Avalonia.Media.Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var point = e.GetPosition(this);
            var thumbRect = GetThumbRectangle();

            if (thumbRect.Contains(point))
            {
                isDragging = true;
                dragOffset = point.Y - thumbRect.Top;
                e.Pointer.Capture(this);
            }
            else
            {
                if (point.Y < thumbRect.Top)
                {
                    Value = Math.Max(Minimum, Value - PageSize);
                }
                else if (point.Y > thumbRect.Bottom)
                {
                    Value = Math.Min(Maximum, Value + PageSize);
                }
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            var point = e.GetPosition(this);

            if (isDragging)
            {
                var thumbRect = GetThumbRectangle();
                double movementRange = Math.Max(Bounds.Height - thumbRect.Height, 1);
                double newThumbTop = point.Y - dragOffset;
                newThumbTop = Math.Max(0, Math.Min(newThumbTop, movementRange));

                Value = Maximum > 0 ? (int)(newThumbTop * Maximum / movementRange) : 0;
            }
            else
            {
                var thumbRect = GetThumbRectangle();
                bool hovered = thumbRect.Contains(point);

                if (isThumbHovered != hovered)
                {
                    isThumbHovered = hovered;
                    InvalidateVisual();
                }
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            isDragging = false;
            e.Pointer.Capture(null);
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);

            if (isThumbHovered)
            {
                isThumbHovered = false;
                InvalidateVisual();
            }
        }
    }
}
