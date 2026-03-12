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
    public class Font : IDisposable
    {
        public SKTypeface Typeface { get; }
        public float Size { get; }
        public FontStyle Style { get; }
        public string Name => Typeface.FamilyName;

        public Font(string familyName, float size, FontStyle style = FontStyle.Regular)
        {
            Size = size;
            Style = style;
            
            var weight = style.HasFlag(FontStyle.Bold) ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
            var slant = style.HasFlag(FontStyle.Italic) ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
            
            Typeface = SKTypeface.FromFamilyName(familyName, weight, SKFontStyleWidth.Normal, slant) 
                       ?? SKTypeface.Default;
        }

        public Font(Font prototype, FontStyle style)
            : this(prototype.Name, prototype.Size, style)
        {
        }

        public void Dispose()
        {
            Typeface.Dispose();
        }
    }

    [Flags]
    public enum FontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikeout = 8
    }
}
