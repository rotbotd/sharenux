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
using System.Collections;

namespace ShareX.HelpersLib
{
    // Avalonia replacement for WinForms ToolStripControlHost<LabeledComboBox>
    // A StackPanel containing a TextBlock label and ComboBox
    public class ToolStripLabeledComboBox : StackPanel
    {
        private readonly TextBlock label;
        private readonly ComboBox comboBox;

        public LabeledComboBox Content => new LabeledComboBox { Text = Text };

        public string Text
        {
            get => label.Text;
            set => label.Text = value;
        }

        public IEnumerable ItemsSource
        {
            get => comboBox.ItemsSource;
            set => comboBox.ItemsSource = value;
        }

        public object SelectedItem
        {
            get => comboBox.SelectedItem;
            set => comboBox.SelectedItem = value;
        }

        public int SelectedIndex
        {
            get => comboBox.SelectedIndex;
            set => comboBox.SelectedIndex = value;
        }

        public ToolStripLabeledComboBox() : this(string.Empty) { }

        public ToolStripLabeledComboBox(string text)
        {
            Orientation = Orientation.Horizontal;
            Spacing = 4;
            VerticalAlignment = VerticalAlignment.Center;

            label = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center
            };

            comboBox = new ComboBox
            {
                MinWidth = 80
            };

            Children.Add(label);
            Children.Add(comboBox);
        }
    }
}
