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
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ShareX.HelpersLib
{
    public class MyListView : UserControl
    {
        public delegate void ListViewItemMovedEventHandler(object sender, int oldIndex, int newIndex);

        public event ListViewItemMovedEventHandler ItemMoving;
        public event ListViewItemMovedEventHandler ItemMoved;

        public bool AutoFillColumn { get; set; }
        public int AutoFillColumnIndex { get; set; } = -1;
        public bool AllowColumnSort { get; set; }
        public bool AllowItemDrag { get; set; }
        public bool AllowSelectAll { get; set; } = true;
        public bool DisableDeselect { get; set; }
        public bool FullRowSelect { get; set; } = true;

        private readonly ListBox listBox;
        private readonly ObservableCollection<MyListViewItem> items;

        public IList<MyListViewItem> Items => items;
        public IList<MyListViewItem> SelectedItems => listBox.SelectedItems.Cast<MyListViewItem>().ToList();
        public IList<int> SelectedIndices => SelectedItems.Select(i => items.IndexOf(i)).ToList();

        public int SelectedIndex
        {
            get => listBox.SelectedIndex;
            set
            {
                UnselectAll();
                if (value >= 0 && value < items.Count)
                {
                    listBox.SelectedIndex = value;
                    listBox.ScrollIntoView(items[value]);
                }
            }
        }

        public MyListView()
        {
            items = new ObservableCollection<MyListViewItem>();
            
            listBox = new ListBox
            {
                SelectionMode = SelectionMode.Multiple,
                ItemsSource = items
            };

            listBox.SelectionChanged += ListBox_SelectionChanged;
            listBox.KeyDown += ListBox_KeyDown;

            Content = listBox;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection changes
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A && e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                SelectAll();
                e.Handled = true;
            }
        }

        public void Select(int index)
        {
            if (items.Count > 0 && index >= 0 && index < items.Count)
            {
                SelectedIndex = index;
            }
        }

        public void SelectLast()
        {
            if (items.Count > 0)
            {
                SelectedIndex = items.Count - 1;
            }
        }

        public void SelectSingle(MyListViewItem item)
        {
            UnselectAll();
            if (item != null)
            {
                listBox.SelectedItem = item;
            }
        }

        public void SelectAll()
        {
            if (AllowSelectAll)
            {
                listBox.SelectAll();
            }
        }

        public void UnselectAll()
        {
            listBox.SelectedItems.Clear();
        }

        public void EnsureSelectedVisible()
        {
            if (SelectedItems.Count > 0)
            {
                listBox.ScrollIntoView(SelectedItems[0]);
            }
        }

        public void AddItem(MyListViewItem item)
        {
            items.Add(item);
        }

        public void RemoveItem(MyListViewItem item)
        {
            items.Remove(item);
        }

        public void RemoveItemAt(int index)
        {
            if (index >= 0 && index < items.Count)
            {
                items.RemoveAt(index);
            }
        }

        public void InsertItem(int index, MyListViewItem item)
        {
            items.Insert(index, item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public void MoveItem(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= items.Count || newIndex < 0 || newIndex >= items.Count)
                return;

            ItemMoving?.Invoke(this, oldIndex, newIndex);
            
            var item = items[oldIndex];
            items.RemoveAt(oldIndex);
            items.Insert(newIndex, item);

            ItemMoved?.Invoke(this, oldIndex, newIndex);
        }
    }

    public class MyListViewItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string text;
        public string Text
        {
            get => text;
            set
            {
                text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }

        public object Tag { get; set; }

        private List<string> subItems = new List<string>();
        public List<string> SubItems => subItems;

        public MyListViewItem() { }

        public MyListViewItem(string text)
        {
            Text = text;
        }

        public MyListViewItem(string[] items)
        {
            if (items.Length > 0)
            {
                Text = items[0];
                for (int i = 1; i < items.Length; i++)
                {
                    subItems.Add(items[i]);
                }
            }
        }

        public override string ToString() => Text;
    }
}
