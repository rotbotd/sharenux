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
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// A menu item that acts like a radio button - only one can be selected at a time within its parent.
    /// </summary>
    public class ToolStripRadioButtonMenuItem : MenuItem
    {
        public static readonly StyledProperty<bool> IsCheckedProperty =
            AvaloniaProperty.Register<ToolStripRadioButtonMenuItem, bool>(nameof(IsChecked), false);

        public static readonly StyledProperty<string> GroupNameProperty =
            AvaloniaProperty.Register<ToolStripRadioButtonMenuItem, string>(nameof(GroupName), "");

        public bool IsChecked
        {
            get => GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public string GroupName
        {
            get => GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        public event EventHandler CheckedChanged;

        public ToolStripRadioButtonMenuItem()
        {
            Click += OnClick;
        }

        public ToolStripRadioButtonMenuItem(string text) : this()
        {
            Header = text;
        }

        public ToolStripRadioButtonMenuItem(string text, Bitmap image) : this()
        {
            Header = text;
            Icon = new Avalonia.Controls.Image { Source = image, Width = 16, Height = 16 };
        }

        public ToolStripRadioButtonMenuItem(string text, Bitmap image, EventHandler onClick) : this()
        {
            Header = text;
            Icon = new Avalonia.Controls.Image { Source = image, Width = 16, Height = 16 };
            if (onClick != null)
            {
                CheckedChanged += onClick;
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (IsChecked)
                return;

            IsChecked = true;
            UncheckSiblings();
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UncheckSiblings()
        {
            if (Parent is ItemsControl parent)
            {
                foreach (var item in parent.Items)
                {
                    if (item is ToolStripRadioButtonMenuItem radioItem && radioItem != this)
                    {
                        if (string.IsNullOrEmpty(GroupName) || radioItem.GroupName == GroupName)
                        {
                            if (radioItem.IsChecked)
                            {
                                radioItem.IsChecked = false;
                                radioItem.CheckedChanged?.Invoke(radioItem, EventArgs.Empty);
                            }
                        }
                    }
                }
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsCheckedProperty)
            {
                // Update visual state - in Avalonia this would typically be done with styles
                // For now, we can use a simple indicator
                if (IsChecked)
                {
                    FontWeight = FontWeight.Bold;
                }
                else
                {
                    FontWeight = FontWeight.Normal;
                }
            }
        }
    }
}
