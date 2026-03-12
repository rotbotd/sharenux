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
using Avalonia.Input;
using Avalonia.Media;
using ShareX.HelpersLib.Properties;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.HelpersLib
{
    public class CodeMenu : ContextMenu
    {
        public Point MenuLocationOffset { get; set; }
        public bool MenuLocationBottom { get; set; }

        private TextBox textBox;

        public CodeMenu(TextBox tb, CodeMenuItem[] items)
        {
            textBox = tb;

            FontFamily = new FontFamily("Lucida Console");
            FontSize = 11;

            foreach (CodeMenuItem item in items)
            {
                var menuItem = new MenuItem 
                { 
                    Header = $"{item.Name} - {item.Description}",
                    Tag = item.Name 
                };

                menuItem.Click += (sender, e) =>
                {
                    if (textBox != null && sender is MenuItem mi)
                    {
                        string text = mi.Tag?.ToString() ?? "";
                        textBox.AppendTextToSelection(text);
                    }
                };

                if (string.IsNullOrWhiteSpace(item.Category))
                {
                    Items.Add(menuItem);
                }
                else
                {
                    MenuItem parentItem = null;
                    foreach (var existing in Items.OfType<MenuItem>())
                    {
                        if (existing.Tag?.ToString() == item.Category)
                        {
                            parentItem = existing;
                            break;
                        }
                    }

                    if (parentItem == null)
                    {
                        parentItem = new MenuItem 
                        { 
                            Header = item.Category, 
                            Tag = item.Category 
                        };
                        Items.Add(parentItem);
                    }

                    parentItem.Items.Add(menuItem);
                }
            }

            Items.Add(new Separator());

            var closeItem = new MenuItem { Header = Resources.CodeMenu_Create_Close };
            closeItem.Click += (sender, e) => Close();
            Items.Add(closeItem);

            if (textBox != null)
            {
                textBox.GotFocus += (sender, e) =>
                {
                    if (Items.Count > 0)
                    {
                        Open(textBox);
                    }
                };

                textBox.LostFocus += (sender, e) =>
                {
                    Close();
                };

                textBox.KeyDown += (sender, e) =>
                {
                    if (e.Key == Key.Enter || e.Key == Key.Escape)
                    {
                        Close();
                        e.Handled = true;
                    }
                };
            }
        }

        public static CodeMenu Create<TEntry>(TextBox tb, TEntry[] ignoreList, CodeMenuItem[] extraItems) where TEntry : CodeMenuEntry
        {
            List<CodeMenuItem> items = new List<CodeMenuItem>();

            if (extraItems != null)
            {
                items.AddRange(extraItems);
            }

            IEnumerable<CodeMenuItem> codeMenuItems = Helpers.GetValueFields<TEntry>().Where(x => !ignoreList.Contains(x)).
                Select(x => new CodeMenuItem(x.ToPrefixString(), x.Description, x.Category));

            items.AddRange(codeMenuItems);

            return new CodeMenu(tb, items.ToArray());
        }

        public static CodeMenu Create<TEntry>(TextBox tb, params TEntry[] ignoreList) where TEntry : CodeMenuEntry
        {
            return Create(tb, ignoreList, (CodeMenuItem[])null);
        }
    }
}
