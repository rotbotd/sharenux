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
    public class Graphics : IDisposable
    {
        private readonly SKCanvas _canvas;
        private readonly SKBitmap? _bitmap;
        private bool _disposed;

        private Graphics(SKCanvas canvas, SKBitmap? bitmap = null)
        {
            _canvas = canvas;
            _bitmap = bitmap;
        }

        public static Graphics FromImage(SKBitmap bitmap)
        {
            var canvas = new SKCanvas(bitmap);
            return new Graphics(canvas, bitmap);
        }

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            _canvas.DrawLine(x1, y1, x2, y2, pen.Paint);
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            _canvas.DrawLine(x1, y1, x2, y2, pen.Paint);
        }

        public void DrawLine(Pen pen, SKPointI p1, SKPointI p2)
        {
            _canvas.DrawLine(p1.X, p1.Y, p2.X, p2.Y, pen.Paint);
        }

        public void DrawLine(Pen pen, SKPoint p1, SKPoint p2)
        {
            _canvas.DrawLine(p1, p2, pen.Paint);
        }

        public void DrawRectangle(Pen pen, int x, int y, int width, int height)
        {
            _canvas.DrawRect(x, y, width, height, pen.Paint);
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            _canvas.DrawRect(x, y, width, height, pen.Paint);
        }

        public void DrawRectangle(Pen pen, SKRectI rect)
        {
            _canvas.DrawRect(rect.Left, rect.Top, rect.Width, rect.Height, pen.Paint);
        }

        public void DrawRectangle(Pen pen, SKRect rect)
        {
            _canvas.DrawRect(rect, pen.Paint);
        }

        public void FillRectangle(Brush brush, int x, int y, int width, int height)
        {
            _canvas.DrawRect(x, y, width, height, brush.Paint);
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            _canvas.DrawRect(x, y, width, height, brush.Paint);
        }

        public void FillRectangle(Brush brush, SKRectI rect)
        {
            _canvas.DrawRect(rect.Left, rect.Top, rect.Width, rect.Height, brush.Paint);
        }

        public void FillRectangle(Brush brush, SKRect rect)
        {
            _canvas.DrawRect(rect, brush.Paint);
        }

        public void DrawEllipse(Pen pen, int x, int y, int width, int height)
        {
            _canvas.DrawOval(x + width / 2f, y + height / 2f, width / 2f, height / 2f, pen.Paint);
        }

        public void DrawEllipse(Pen pen, SKRectI rect)
        {
            DrawEllipse(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void FillEllipse(Brush brush, int x, int y, int width, int height)
        {
            _canvas.DrawOval(x + width / 2f, y + height / 2f, width / 2f, height / 2f, brush.Paint);
        }

        public void FillEllipse(Brush brush, SKRectI rect)
        {
            FillEllipse(brush, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void DrawImage(SKBitmap image, int x, int y)
        {
            _canvas.DrawBitmap(image, x, y);
        }

        public void DrawImage(SKBitmap image, SKRectI destRect)
        {
            _canvas.DrawBitmap(image, SKRect.Create(destRect.Left, destRect.Top, destRect.Width, destRect.Height));
        }

        public void DrawImage(SKBitmap image, SKRectI destRect, SKRectI srcRect)
        {
            var src = SKRect.Create(srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height);
            var dst = SKRect.Create(destRect.Left, destRect.Top, destRect.Width, destRect.Height);
            _canvas.DrawBitmap(image, src, dst);
        }

        public void DrawString(string text, Font font, Brush brush, float x, float y)
        {
            var paint = brush.Paint.Clone();
            paint.TextSize = font.Size;
            paint.Typeface = font.Typeface;
            _canvas.DrawText(text, x, y + font.Size, paint);
        }

        public void DrawString(string text, Font font, Brush brush, SKPoint point)
        {
            DrawString(text, font, brush, point.X, point.Y);
        }

        public void Clear(SKColor color)
        {
            _canvas.Clear(color);
        }

        public void Flush()
        {
            _canvas.Flush();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _canvas.Dispose();
                _disposed = true;
            }
        }
    }
}
