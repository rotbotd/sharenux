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
    /// Represents a color palette for indexed color images.
    /// Compatible with System.Drawing.Imaging.ColorPalette.
    /// </summary>
    public class ColorPalette
    {
        /// <summary>
        /// Array of colors in the palette.
        /// </summary>
        public SKColor[] Entries { get; set; }

        /// <summary>
        /// Palette flags (unused in SkiaSharp, kept for compatibility).
        /// </summary>
        public int Flags { get; set; }

        public ColorPalette() : this(256)
        {
        }

        public ColorPalette(int count)
        {
            Entries = new SKColor[count];
            Flags = 0;
        }

        public ColorPalette(SKColor[] colors)
        {
            Entries = colors;
            Flags = 0;
        }
    }
}
