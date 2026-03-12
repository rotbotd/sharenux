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
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using System;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// A button that shows a dropdown menu when clicked.
    /// </summary>
    public class MenuButton : Button
    {
        public static readonly StyledProperty<ContextMenu> MenuProperty =
            AvaloniaProperty.Register<MenuButton, ContextMenu>(nameof(Menu));

        public static readonly StyledProperty<bool> ShowMenuUnderCursorProperty =
            AvaloniaProperty.Register<MenuButton, bool>(nameof(ShowMenuUnderCursor), false);

        public ContextMenu Menu
        {
            get => GetValue(MenuProperty);
            set => SetValue(MenuProperty, value);
        }

        public bool ShowMenuUnderCursor
        {
            get => GetValue(ShowMenuUnderCursorProperty);
            set => SetValue(ShowMenuUnderCursorProperty, value);
        }

        public MenuButton()
        {
            // Add dropdown arrow indicator
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 5
            };

            var contentPresenter = new ContentPresenter();
            contentPresenter.Bind(ContentPresenter.ContentProperty, this.GetObservable(ContentProperty));
            panel.Children.Add(contentPresenter);

            var arrow = new TextBlock
            {
                Text = "▼",
                FontSize = 8,
                VerticalAlignment = VerticalAlignment.Center
            };
            panel.Children.Add(arrow);
        }

        public void OpenMenu()
        {
            if (Menu != null)
            {
                Menu.Open(this);
            }
        }

        public void OpenMenu(Point position)
        {
            if (Menu != null)
            {
                Menu.Open(this);
            }
        }

        protected override void OnClick()
        {
            base.OnClick();

            if (Menu != null)
            {
                OpenMenu();
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var point = e.GetCurrentPoint(this);
            if (Menu != null && point.Properties.IsLeftButtonPressed)
            {
                if (ShowMenuUnderCursor)
                {
                    OpenMenu(e.GetPosition(this));
                }
                else
                {
                    OpenMenu();
                }
            }
        }
    }
}
