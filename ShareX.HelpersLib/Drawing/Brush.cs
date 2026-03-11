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

using SkiaSharp;

namespace ShareX.HelpersLib
{
    public abstract class Brush : IDisposable
    {
        public abstract SKPaint Paint { get; }
        public abstract void Dispose();
    }

    public class SolidBrush : Brush
    {
        private readonly SKPaint _paint;
        public override SKPaint Paint => _paint;
        public SKColor Color => _paint.Color;

        public SolidBrush(SKColor color)
        {
            _paint = new SKPaint
            {
                Color = color,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
        }

        public override void Dispose()
        {
            _paint.Dispose();
        }
    }

    public class LinearGradientBrush : Brush
    {
        private readonly SKPaint _paint;
        private readonly SKPoint _start;
        private readonly SKPoint _end;
        public override SKPaint Paint => _paint;

        private ColorBlend? _interpolationColors;
        public ColorBlend? InterpolationColors
        {
            get => _interpolationColors;
            set
            {
                _interpolationColors = value;
                if (value != null && value.Colors.Length > 0)
                {
                    _paint.Shader?.Dispose();
                    _paint.Shader = SKShader.CreateLinearGradient(
                        _start, _end,
                        value.Colors,
                        value.Positions,
                        SKShaderTileMode.Clamp);
                }
            }
        }

        public LinearGradientBrush(SKPoint start, SKPoint end, SKColor startColor, SKColor endColor)
        {
            _start = start;
            _end = end;
            _paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Shader = SKShader.CreateLinearGradient(
                    start, end,
                    new[] { startColor, endColor },
                    SKShaderTileMode.Clamp)
            };
        }

        public LinearGradientBrush(SKRectI rect, SKColor startColor, SKColor endColor, LinearGradientMode mode)
        {
            switch (mode)
            {
                case LinearGradientMode.Horizontal:
                    _start = new SKPoint(rect.Left, rect.Top);
                    _end = new SKPoint(rect.Right, rect.Top);
                    break;
                case LinearGradientMode.Vertical:
                    _start = new SKPoint(rect.Left, rect.Top);
                    _end = new SKPoint(rect.Left, rect.Bottom);
                    break;
                case LinearGradientMode.ForwardDiagonal:
                    _start = new SKPoint(rect.Left, rect.Top);
                    _end = new SKPoint(rect.Right, rect.Bottom);
                    break;
                case LinearGradientMode.BackwardDiagonal:
                    _start = new SKPoint(rect.Right, rect.Top);
                    _end = new SKPoint(rect.Left, rect.Bottom);
                    break;
                default:
                    _start = new SKPoint(rect.Left, rect.Top);
                    _end = new SKPoint(rect.Right, rect.Top);
                    break;
            }

            _paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Shader = SKShader.CreateLinearGradient(
                    _start, _end,
                    new[] { startColor, endColor },
                    SKShaderTileMode.Clamp)
            };
        }

        public override void Dispose()
        {
            _paint.Shader?.Dispose();
            _paint.Dispose();
        }
    }

    public enum LinearGradientMode
    {
        Horizontal = 0,
        Vertical = 1,
        ForwardDiagonal = 2,
        BackwardDiagonal = 3
    }

    // Static brush instances matching System.Drawing.Brushes
    public static class Brushes
    {
        public static SolidBrush Black => new SolidBrush(SKColors.Black);
        public static SolidBrush White => new SolidBrush(SKColors.White);
        public static SolidBrush Red => new SolidBrush(SKColors.Red);
        public static SolidBrush Green => new SolidBrush(SKColors.Green);
        public static SolidBrush Blue => new SolidBrush(SKColors.Blue);
        public static SolidBrush Gray => new SolidBrush(SKColors.Gray);
        public static SolidBrush LightGray => new SolidBrush(SKColors.LightGray);
        public static SolidBrush DarkGray => new SolidBrush(SKColors.DarkGray);
        public static SolidBrush Transparent => new SolidBrush(SKColors.Transparent);
    }
}
