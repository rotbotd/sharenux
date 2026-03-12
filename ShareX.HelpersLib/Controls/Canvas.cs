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
using Avalonia.Threading;
using SkiaSharp;
using System;

namespace ShareX.HelpersLib
{
    public class Canvas : Avalonia.Controls.Control
    {
        public delegate void DrawEventHandler(SKCanvas canvas, SKImageInfo info);

        public event DrawEventHandler Draw;

        public int Interval { get; set; } = 100;

        private DispatcherTimer timer;
        private bool needPaint;

        public Canvas()
        {
            ClipToBounds = true;
        }

        public void Start()
        {
            if (timer == null || !timer.IsEnabled)
            {
                Stop();

                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(Interval)
                };
                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }

        public void Start(int interval)
        {
            Interval = interval;
            Start();
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            needPaint = true;
            InvalidateVisual();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (!needPaint)
                return;

            int width = (int)Bounds.Width;
            int height = (int)Bounds.Height;

            if (width <= 0 || height <= 0)
                return;

            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);

            if (surface != null)
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.Transparent);

                Draw?.Invoke(canvas, info);

                using var image = surface.Snapshot();
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = new System.IO.MemoryStream();
                data.SaveTo(stream);
                stream.Position = 0;

                var bitmap = new Avalonia.Media.Imaging.Bitmap(stream);
                context.DrawImage(bitmap, new Rect(0, 0, width, height));
            }

            needPaint = false;
        }
    }
}
