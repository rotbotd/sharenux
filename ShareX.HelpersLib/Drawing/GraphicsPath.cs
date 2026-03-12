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

using System;
using SkiaSharp;

namespace ShareX.HelpersLib
{
    public class GraphicsPath : IDisposable
    {
        private readonly SKPath _path = new SKPath();

        public SKPath Path => _path;

        public void AddLine(float x1, float y1, float x2, float y2)
        {
            if (_path.IsEmpty)
            {
                _path.MoveTo(x1, y1);
            }
            _path.LineTo(x2, y2);
        }

        public void AddLine(SKPoint p1, SKPoint p2)
        {
            AddLine(p1.X, p1.Y, p2.X, p2.Y);
        }

        public void AddRectangle(SKRectI rect)
        {
            _path.AddRect(SKRect.Create(rect.Left, rect.Top, rect.Width, rect.Height));
        }

        public void AddRectangle(SKRect rect)
        {
            _path.AddRect(rect);
        }

        public void AddEllipse(float x, float y, float width, float height)
        {
            _path.AddOval(SKRect.Create(x, y, width, height));
        }

        public void AddEllipse(SKRectI rect)
        {
            AddEllipse(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            _path.AddArc(SKRect.Create(x, y, width, height), startAngle, sweepAngle);
        }

        public void AddArc(SKRectI rect, float startAngle, float sweepAngle)
        {
            AddArc(rect.Left, rect.Top, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void AddPolygon(SKPoint[] points)
        {
            if (points.Length == 0) return;
            _path.MoveTo(points[0]);
            for (int i = 1; i < points.Length; i++)
            {
                _path.LineTo(points[i]);
            }
            _path.Close();
        }

        public void AddPolygon(SKPointI[] points)
        {
            if (points.Length == 0) return;
            _path.MoveTo(points[0].X, points[0].Y);
            for (int i = 1; i < points.Length; i++)
            {
                _path.LineTo(points[i].X, points[i].Y);
            }
            _path.Close();
        }

        public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            if (_path.IsEmpty)
            {
                _path.MoveTo(x1, y1);
            }
            _path.CubicTo(x2, y2, x3, y3, x4, y4);
        }

        public void AddCurve(SKPoint[] points)
        {
            if (points.Length < 2) return;
            _path.MoveTo(points[0]);
            for (int i = 1; i < points.Length; i++)
            {
                _path.LineTo(points[i]);
            }
        }

        public void AddString(string text, Font font, float x, float y)
        {
            using var paint = new SKPaint
            {
                TextSize = font.Size,
                Typeface = font.Typeface,
                IsAntialias = true
            };
            var textPath = paint.GetTextPath(text, x, y + font.Size);
            _path.AddPath(textPath);
        }

        public void CloseFigure()
        {
            _path.Close();
        }

        public void StartFigure()
        {
            // SkiaSharp automatically handles separate figures
        }

        public void Reset()
        {
            _path.Reset();
        }

        public SKRect GetBounds()
        {
            return _path.Bounds;
        }

        public bool IsVisible(float x, float y)
        {
            return _path.Contains(x, y);
        }

        public bool IsVisible(SKPoint point)
        {
            return _path.Contains(point.X, point.Y);
        }

        public void Dispose()
        {
            _path.Dispose();
        }
    }
}
