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
    /// <summary>
    /// Represents an icon image. Wraps SKBitmap for cross-platform compatibility.
    /// </summary>
    public class Icon : IDisposable
    {
        private readonly SKBitmap _bitmap;
        
        public int Width => _bitmap.Width;
        public int Height => _bitmap.Height;
        public SKSizeI Size => new SKSizeI(Width, Height);

        public Icon(string filename)
        {
            _bitmap = SKBitmap.Decode(filename) 
                ?? throw new ArgumentException($"Could not load icon from {filename}");
        }

        public Icon(Stream stream)
        {
            _bitmap = SKBitmap.Decode(stream)
                ?? throw new ArgumentException("Could not load icon from stream");
        }

        public Icon(SKBitmap bitmap)
        {
            _bitmap = bitmap;
        }

        public Icon(Icon original, int width, int height)
        {
            _bitmap = original._bitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High)
                ?? throw new ArgumentException("Could not resize icon");
        }

        public Icon(Icon original, SKSizeI size) : this(original, size.Width, size.Height)
        {
        }

        public SKBitmap ToBitmap()
        {
            return _bitmap.Copy();
        }

        public static Icon FromHandle(IntPtr handle)
        {
            // Platform-specific handle conversion not supported in cross-platform
            // Return a placeholder 1x1 transparent icon
            var bmp = new SKBitmap(1, 1);
            bmp.Erase(SKColors.Transparent);
            return new Icon(bmp);
        }

        public static Icon ExtractAssociatedIcon(string filePath)
        {
            // Would need platform-specific implementation
            // Return a placeholder
            var bmp = new SKBitmap(32, 32);
            bmp.Erase(SKColors.Gray);
            return new Icon(bmp);
        }

        public void Dispose()
        {
            _bitmap.Dispose();
        }
    }
}
