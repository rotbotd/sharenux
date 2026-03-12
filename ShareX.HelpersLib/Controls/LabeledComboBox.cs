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
using Avalonia.Layout;
using System;

namespace ShareX.HelpersLib
{
    public class LabeledComboBox : UserControl
    {
        private readonly TextBlock lblText;
        private readonly ComboBox cbList;

        public new string Text
        {
            get => lblText.Text;
            set => lblText.Text = value;
        }

        public int SelectedIndex
        {
            get => cbList.SelectedIndex;
            set => cbList.SelectedIndex = value;
        }

        public object SelectedItem
        {
            get => cbList.SelectedItem;
            set => cbList.SelectedItem = value;
        }

        public event EventHandler<SelectionChangedEventArgs> SelectedIndexChanged;

        public LabeledComboBox()
        {
            lblText = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            cbList = new ComboBox
            {
                MinWidth = 80
            };

            cbList.SelectionChanged += (s, e) => SelectedIndexChanged?.Invoke(s, e);

            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 4
            };

            panel.Children.Add(lblText);
            panel.Children.Add(cbList);

            Content = panel;
        }

        public void Add(object item)
        {
            cbList.Items.Add(item);
        }

        public void AddRange(object[] items)
        {
            foreach (var item in items)
            {
                cbList.Items.Add(item);
            }
        }

        public void Clear()
        {
            cbList.Items.Clear();
        }
    }
}
