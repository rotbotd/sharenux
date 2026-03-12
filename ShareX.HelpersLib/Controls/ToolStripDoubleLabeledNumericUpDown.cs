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
    // Avalonia replacement for WinForms ToolStripControlHost<DoubleLabeledNumericUpDown>
    // A StackPanel containing two labels and two NumericUpDowns
    public class ToolStripDoubleLabeledNumericUpDown : StackPanel
    {
        private readonly TextBlock label1;
        private readonly NumericUpDown numericUpDown1;
        private readonly TextBlock label2;
        private readonly NumericUpDown numericUpDown2;

        public DoubleLabeledNumericUpDown Content => new DoubleLabeledNumericUpDown 
        { 
            Text = Text, 
            Text2 = Text2,
            Value = Value,
            Value2 = Value2
        };

        public string Text
        {
            get => label1.Text;
            set => label1.Text = value;
        }

        public string Text2
        {
            get => label2.Text;
            set => label2.Text = value;
        }

        public decimal? Value
        {
            get => numericUpDown1.Value;
            set => numericUpDown1.Value = value;
        }

        public decimal? Value2
        {
            get => numericUpDown2.Value;
            set => numericUpDown2.Value = value;
        }

        public ToolStripDoubleLabeledNumericUpDown() : this(string.Empty, string.Empty) { }

        public ToolStripDoubleLabeledNumericUpDown(string text, string text2)
        {
            Orientation = Orientation.Horizontal;
            Spacing = 4;
            VerticalAlignment = VerticalAlignment.Center;

            label1 = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center
            };

            numericUpDown1 = new NumericUpDown
            {
                Width = 60,
                Minimum = 0,
                Maximum = 100,
                Increment = 1
            };

            label2 = new TextBlock
            {
                Text = text2,
                VerticalAlignment = VerticalAlignment.Center
            };

            numericUpDown2 = new NumericUpDown
            {
                Width = 60,
                Minimum = 0,
                Maximum = 100,
                Increment = 1
            };

            Children.Add(label1);
            Children.Add(numericUpDown1);
            Children.Add(label2);
            Children.Add(numericUpDown2);
        }
    }
}
