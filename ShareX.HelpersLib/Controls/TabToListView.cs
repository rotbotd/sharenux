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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// A control that displays tabs as a list view on the left side and content on the right.
    /// Similar to settings pages in many applications.
    /// </summary>
    public class TabToListView : UserControl
    {
        private TabControl mainTabControl;
        private ListBox listView;
        private ContentControl contentArea;
        private Grid splitContainer;
        private ObservableCollection<TabListItem> listItems = new ObservableCollection<TabListItem>();

        public TabControl MainTabControl
        {
            get => mainTabControl;
            set
            {
                mainTabControl = value;
                FillListView(mainTabControl);
            }
        }

        private int listViewSize = 150;
        public int ListViewSize
        {
            get => listViewSize;
            set
            {
                listViewSize = value;
                if (splitContainer != null)
                {
                    splitContainer.ColumnDefinitions[0].Width = new GridLength(listViewSize);
                }
            }
        }

        public TabToListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            splitContainer = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions($"{listViewSize},*")
            };

            listView = new ListBox
            {
                ItemsSource = listItems,
                Margin = new Thickness(0, 0, 5, 0)
            };
            listView.SelectionChanged += ListView_SelectionChanged;
            Grid.SetColumn(listView, 0);
            splitContainer.Children.Add(listView);

            contentArea = new ContentControl
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            Grid.SetColumn(contentArea, 1);
            splitContainer.Children.Add(contentArea);

            Content = splitContainer;
        }

        private void FillListView(TabControl tab)
        {
            if (tab == null) return;

            listItems.Clear();

            foreach (var item in tab.Items)
            {
                if (item is TabItem tabItem)
                {
                    // Check if this tab contains nested tabs
                    if (tabItem.Content is TabControl nestedTab)
                    {
                        string groupName = tabItem.Header?.ToString() ?? "";

                        foreach (var nestedItem in nestedTab.Items)
                        {
                            if (nestedItem is TabItem nestedTabItem)
                            {
                                listItems.Add(new TabListItem
                                {
                                    Text = nestedTabItem.Header?.ToString() ?? "",
                                    Group = groupName,
                                    Content = nestedTabItem.Content
                                });
                            }
                        }
                    }
                    else
                    {
                        listItems.Add(new TabListItem
                        {
                            Text = tabItem.Header?.ToString() ?? "",
                            Content = tabItem.Content
                        });
                    }
                }
            }

            if (listItems.Count > 0)
            {
                listView.SelectedIndex = 0;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedItem is TabListItem item && item.Content != null)
            {
                contentArea.Content = item.Content;
            }
        }

        public void NavigateToTabPage(object content)
        {
            for (int i = 0; i < listItems.Count; i++)
            {
                if (listItems[i].Content == content)
                {
                    listView.SelectedIndex = i;
                    return;
                }
            }
        }

        public void FocusListView()
        {
            listView.Focus();
        }
    }

    public class TabListItem
    {
        public string Text { get; set; }
        public string Group { get; set; }
        public object Content { get; set; }

        public override string ToString() => Text;
    }
}
