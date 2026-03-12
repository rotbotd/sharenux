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
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.HelpersLib
{
    public class TabToTreeView : UserControl
    {
        public delegate void TabChangedEventHandler(TabItem tabItem);
        public event TabChangedEventHandler TabChanged;

        private TabControl mainTabControl;
        private readonly TreeView treeView;
        private readonly TabControl displayTabControl;
        private readonly SplitView splitView;
        private readonly Border separator;

        public TabControl MainTabControl
        {
            get => mainTabControl;
            set
            {
                if (mainTabControl != value)
                {
                    mainTabControl = value;
                    FillTreeView();
                }
            }
        }

        private double treeViewSize = 150;

        public double TreeViewSize
        {
            get => treeViewSize;
            set
            {
                treeViewSize = value;
                splitView.OpenPaneLength = value;
            }
        }

        public IBrush LeftPanelBackColor
        {
            get => treeView.Background;
            set => treeView.Background = value;
        }

        public IBrush SeparatorColor
        {
            get => separator.Background;
            set => separator.Background = value;
        }

        public bool AutoSelectChild { get; set; }

        public TabToTreeView()
        {
            treeView = new TreeView
            {
                SelectionMode = SelectionMode.Single
            };
            treeView.SelectionChanged += TreeView_SelectionChanged;

            displayTabControl = new TabControl
            {
                IsVisible = false
            };

            separator = new Border
            {
                Width = 1,
                Background = Brushes.Gray
            };

            var rightPanel = new DockPanel();
            DockPanel.SetDock(separator, Dock.Left);
            rightPanel.Children.Add(separator);
            rightPanel.Children.Add(displayTabControl);

            splitView = new SplitView
            {
                IsPaneOpen = true,
                DisplayMode = SplitViewDisplayMode.Inline,
                OpenPaneLength = treeViewSize,
                PanePlacement = SplitViewPanePlacement.Left,
                Pane = treeView,
                Content = rightPanel
            };

            Content = splitView;
        }

        private void FillTreeView()
        {
            treeView.Items.Clear();

            if (mainTabControl == null)
                return;

            foreach (var tabItem in mainTabControl.Items.OfType<TabItem>())
            {
                var node = CreateTreeNode(tabItem);
                if (node != null)
                {
                    treeView.Items.Add(node);
                }
            }

            ExpandAll(treeView.Items.OfType<TreeViewItem>());
        }

        private TreeViewItem CreateTreeNode(TabItem tabItem, TreeViewItem parent = null)
        {
            if (parent != null && string.IsNullOrEmpty(tabItem.Header?.ToString()))
            {
                parent.Tag = tabItem;
                return null;
            }

            var node = new TreeViewItem
            {
                Header = tabItem.Header,
                Tag = tabItem,
                IsExpanded = true
            };

            // Check if tab contains a nested TabControl
            if (tabItem.Content is TabControl nestedTabControl)
            {
                foreach (var nestedTabItem in nestedTabControl.Items.OfType<TabItem>())
                {
                    var childNode = CreateTreeNode(nestedTabItem, node);
                    if (childNode != null)
                    {
                        node.Items.Add(childNode);
                    }
                }
            }
            else if (tabItem.Content is Avalonia.Controls.Control control)
            {
                // Look for TabControl in content
                var nestedTab = FindChild<TabControl>(control);
                if (nestedTab != null)
                {
                    foreach (var nestedTabItem in nestedTab.Items.OfType<TabItem>())
                    {
                        var childNode = CreateTreeNode(nestedTabItem, node);
                        if (childNode != null)
                        {
                            node.Items.Add(childNode);
                        }
                    }
                }
            }

            return node;
        }

        private static T FindChild<T>(Avalonia.Controls.Control parent) where T : Avalonia.Controls.Control
        {
            if (parent is T typed)
                return typed;

            if (parent is ContentControl cc && cc.Content is Avalonia.Controls.Control content)
            {
                var result = FindChild<T>(content);
                if (result != null) return result;
            }

            if (parent is Panel panel)
            {
                foreach (var child in panel.Children.OfType<Avalonia.Controls.Control>())
                {
                    var result = FindChild<T>(child);
                    if (result != null) return result;
                }
            }

            return null;
        }

        private void ExpandAll(IEnumerable<TreeViewItem> items)
        {
            foreach (var item in items)
            {
                item.IsExpanded = true;
                ExpandAll(item.Items.OfType<TreeViewItem>());
            }
        }

        private void TreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (treeView.SelectedItem is TreeViewItem node && node.Tag is TabItem tabItem)
            {
                if (AutoSelectChild && tabItem.Content is TabControl)
                {
                    SelectChildNode();
                }
                else
                {
                    SelectTabItem(tabItem);
                }
            }
        }

        private void SelectTabItem(TabItem tabItem)
        {
            if (tabItem != null)
            {
                displayTabControl.IsVisible = true;
                displayTabControl.Items.Clear();
                
                // Clone the tab item for display
                var displayTab = new TabItem
                {
                    Header = tabItem.Header,
                    Content = tabItem.Content
                };
                displayTabControl.Items.Add(displayTab);
                displayTabControl.SelectedIndex = 0;

                treeView.Focus();
                TabChanged?.Invoke(tabItem);
            }
        }

        public void NavigateToTabItem(TabItem tabItem)
        {
            if (tabItem == null) return;

            foreach (var node in GetAllNodes(treeView.Items.OfType<TreeViewItem>()))
            {
                if (node.Tag == tabItem)
                {
                    treeView.SelectedItem = node;
                    return;
                }
            }
        }

        private IEnumerable<TreeViewItem> GetAllNodes(IEnumerable<TreeViewItem> items)
        {
            foreach (var item in items)
            {
                yield return item;
                foreach (var child in GetAllNodes(item.Items.OfType<TreeViewItem>()))
                {
                    yield return child;
                }
            }
        }

        public void SelectChildNode()
        {
            if (treeView.SelectedItem is TreeViewItem node && node.Items.Count > 0)
            {
                treeView.SelectedItem = node.Items.OfType<TreeViewItem>().FirstOrDefault();
            }
        }
    }
}
