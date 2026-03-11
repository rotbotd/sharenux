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
    /// <summary>
    /// Defines arrays of colors and positions for multi-color gradients.
    /// Compatible with System.Drawing.Drawing2D.ColorBlend.
    /// </summary>
    public class ColorBlend
    {
        /// <summary>
        /// Array of colors to use in the gradient.
        /// </summary>
        public SKColor[] Colors { get; set; } = Array.Empty<SKColor>();

        /// <summary>
        /// Array of positions along the gradient line (0.0 to 1.0).
        /// Must have same length as Colors array.
        /// </summary>
        public float[] Positions { get; set; } = Array.Empty<float>();

        public ColorBlend()
        {
        }

        public ColorBlend(int count)
        {
            Colors = new SKColor[count];
            Positions = new float[count];
        }
    }
}
