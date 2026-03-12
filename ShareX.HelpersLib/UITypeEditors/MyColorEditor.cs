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
using System;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// Color editor for property editing. In Avalonia, this is used as a marker
    /// and the actual editing is handled by custom property grid implementations.
    /// </summary>
    public class MyColorEditor
    {
        public static SKColor? ShowColorPicker(SKColor currentColor)
        {
            var form = new ColorPickerForm(currentColor);
            if (form.ShowDialog() == DialogResult.OK)
            {
                return form.NewColor;
            }
            return null;
        }

        public static async Task<SKColor?> ShowColorPickerAsync(SKColor currentColor)
        {
            var form = new ColorPickerForm(currentColor);
            var result = await form.ShowDialogAsync();
            if (result == DialogResult.OK)
            {
                return form.NewColor;
            }
            return null;
        }
    }
}
