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
using Avalonia.Media.Imaging;
using SkiaSharp;
using System;
using System.IO;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// A window that displays a semi-transparent bitmap overlay.
    /// In Avalonia, this is achieved using transparent windows with image content.
    /// </summary>
    public class LayeredForm : Window
    {
        private Avalonia.Controls.Image imageControl;

        public LayeredForm()
        {
            Width = 300;
            Height = 300;
            SystemDecorations = SystemDecorations.None;
            ShowInTaskbar = false;
            Background = Brushes.Transparent;
            TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent };
            
            imageControl = new Avalonia.Controls.Image
            {
                Stretch = Stretch.None
            };
            Content = imageControl;
        }

        public void SelectBitmap(SKBitmap bitmap, int opacity = 255)
        {
            if (bitmap == null)
                return;

            // Apply opacity to bitmap if needed
            SKBitmap displayBitmap = bitmap;
            if (opacity < 255)
            {
                displayBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
                using (var canvas = new SKCanvas(displayBitmap))
                using (var paint = new SKPaint())
                {
                    paint.Color = paint.Color.WithAlpha((byte)opacity);
                    canvas.DrawBitmap(bitmap, 0, 0, paint);
                }
            }

            // Convert to Avalonia bitmap
            using var data = displayBitmap.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream();
            data.SaveTo(stream);
            stream.Position = 0;
            
            var avaloniaBitmap = new Avalonia.Media.Imaging.Bitmap(stream);
            imageControl.Source = avaloniaBitmap;

            Width = bitmap.Width;
            Height = bitmap.Height;

            if (opacity < 255 && displayBitmap != bitmap)
            {
                displayBitmap.Dispose();
            }
        }
    }
}
