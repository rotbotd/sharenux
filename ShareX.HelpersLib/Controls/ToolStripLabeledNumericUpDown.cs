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
using Avalonia.Layout;

namespace ShareX.HelpersLib
{
    // Avalonia replacement for WinForms ToolStripControlHost<LabeledNumericUpDown>
    // A StackPanel containing a TextBlock label and NumericUpDown
    public class ToolStripLabeledNumericUpDown : StackPanel
    {
        private readonly TextBlock label;
        private readonly NumericUpDown numericUpDown;

        public LabeledNumericUpDown Content => new LabeledNumericUpDown { Text = Text, Value = Value };

        public string Text
        {
            get => label.Text;
            set => label.Text = value;
        }

        public decimal? Value
        {
            get => numericUpDown.Value;
            set => numericUpDown.Value = value;
        }

        public decimal? Minimum
        {
            get => numericUpDown.Minimum;
            set => numericUpDown.Minimum = value;
        }

        public decimal? Maximum
        {
            get => numericUpDown.Maximum;
            set => numericUpDown.Maximum = value;
        }

        public ToolStripLabeledNumericUpDown() : this(string.Empty) { }

        public ToolStripLabeledNumericUpDown(string text)
        {
            Orientation = Orientation.Horizontal;
            Spacing = 4;
            VerticalAlignment = VerticalAlignment.Center;

            label = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center
            };

            numericUpDown = new NumericUpDown
            {
                Width = 60,
                Minimum = 0,
                Maximum = 100,
                Increment = 1
            };

            Children.Add(label);
            Children.Add(numericUpDown);
        }
    }
}
