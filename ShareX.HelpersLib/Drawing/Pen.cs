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
    public class Pen : IDisposable
    {
        public SKPaint Paint { get; }
        public SKColor Color => Paint.Color;
        
        public float Width
        {
            get => Paint.StrokeWidth;
            set => Paint.StrokeWidth = value;
        }

        public Pen(SKColor color, float width = 1f)
        {
            Paint = new SKPaint
            {
                Color = color,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = width,
                IsAntialias = true
            };
        }

        public Pen(Brush brush, float width = 1f)
        {
            Paint = brush.Paint.Clone();
            Paint.Style = SKPaintStyle.Stroke;
            Paint.StrokeWidth = width;
        }

        public void Dispose()
        {
            Paint.Dispose();
        }
    }

    // Static pen instances matching System.Drawing.Pens
    public static class Pens
    {
        public static Pen Black => new Pen(SKColors.Black);
        public static Pen White => new Pen(SKColors.White);
        public static Pen Red => new Pen(SKColors.Red);
        public static Pen Green => new Pen(SKColors.Green);
        public static Pen Blue => new Pen(SKColors.Blue);
        public static Pen Gray => new Pen(SKColors.Gray);
        public static Pen LightGray => new Pen(SKColors.LightGray);
        public static Pen DarkGray => new Pen(SKColors.DarkGray);
        public static Pen Transparent => new Pen(SKColors.Transparent);
    }
}
