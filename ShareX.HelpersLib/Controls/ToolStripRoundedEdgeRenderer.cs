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

using Avalonia.Controls;
using Avalonia.Media;
using SkiaSharp;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// In Avalonia, toolbar styling is done via XAML styles rather than renderers.
    /// This class provides color configuration that can be applied to menus/toolbars.
    /// </summary>
    public class ToolStripRoundedEdgeRenderer
    {
        public bool RoundedEdges { get; set; } = false;
        public ToolStripColorTable ColorTable { get; }

        public ToolStripRoundedEdgeRenderer()
        {
            ColorTable = new ToolStripColorTable();
        }

        public ToolStripRoundedEdgeRenderer(ToolStripColorTable colorTable)
        {
            ColorTable = colorTable ?? new ToolStripColorTable();
        }

        public void ApplyToMenu(Menu menu)
        {
            if (menu != null)
            {
                menu.Background = new SolidColorBrush(ColorTable.MenuStripGradientBegin.ToAvaloniaColor());
            }
        }
    }

    /// <summary>
    /// Color configuration for toolbars and menus, replacing WinForms ProfessionalColorTable.
    /// </summary>
    public class ToolStripColorTable
    {
        public SKColor MenuStripGradientBegin { get; set; } = SKColors.White;
        public SKColor MenuStripGradientEnd { get; set; } = SKColors.LightGray;
        public SKColor MenuItemSelected { get; set; } = SKColor.Parse("#3399FF");
        public SKColor MenuItemSelectedGradientBegin { get; set; } = SKColor.Parse("#C1D2EE");
        public SKColor MenuItemSelectedGradientEnd { get; set; } = SKColor.Parse("#C1D2EE");
        public SKColor MenuItemBorder { get; set; } = SKColor.Parse("#3399FF");
        public SKColor MenuBorder { get; set; } = SKColor.Parse("#9BA7B7");
        public SKColor ImageMarginGradientBegin { get; set; } = SKColors.White;
        public SKColor ImageMarginGradientMiddle { get; set; } = SKColors.White;
        public SKColor ImageMarginGradientEnd { get; set; } = SKColors.White;
        public SKColor SeparatorDark { get; set; } = SKColor.Parse("#C5C5C5");
        public SKColor SeparatorLight { get; set; } = SKColors.White;
        public SKColor CheckBackground { get; set; } = SKColor.Parse("#E1E6E8");
        public SKColor CheckSelectedBackground { get; set; } = SKColor.Parse("#C1D2EE");
        public SKColor ButtonSelectedBorder { get; set; } = SKColor.Parse("#3399FF");
    }
}
