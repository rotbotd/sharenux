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
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.HelpersLib
{
    public static class FormExtensions
    {
        public static void ForceActivate(this Window window)
        {
            if (window != null)
            {
                window.Show();
                
                if (window.WindowState == WindowState.Minimized)
                {
                    window.WindowState = WindowState.Normal;
                }

                window.Activate();
                window.Topmost = true;
                window.Topmost = false;
            }
        }

        public static void CloseOnEscape(this Window window)
        {
            window.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    window.Close();
                }
            };
        }

        public static void InvokeSafe(this Control control, Action action)
        {
            if (control != null)
            {
                Dispatcher.UIThread.Post(action);
            }
        }

        public static async void InvokeSafeAsync(this Control control, Action action)
        {
            if (control != null)
            {
                await Dispatcher.UIThread.InvokeAsync(action);
            }
        }

        public static void AppendLine(this TextBox textBox, string text = "")
        {
            if (textBox != null)
            {
                textBox.Text += text + Environment.NewLine;
            }
        }

        public static void AppendTextToSelection(this TextBox textBox, string text)
        {
            if (textBox != null && !string.IsNullOrEmpty(text))
            {
                int caretIndex = textBox.CaretIndex;
                string currentText = textBox.Text ?? "";
                textBox.Text = currentText.Insert(caretIndex, text);
                textBox.CaretIndex = caretIndex + text.Length;
            }
        }

        public static void SetValue(this NumericUpDown nud, decimal number)
        {
            if (nud != null)
            {
                decimal min = nud.Minimum ?? decimal.MinValue;
                decimal max = nud.Maximum ?? decimal.MaxValue;
                nud.Value = Math.Clamp(number, min, max);
            }
        }

        public static void SetValue(this Slider slider, double number)
        {
            if (slider != null)
            {
                slider.Value = Math.Clamp(number, slider.Minimum, slider.Maximum);
            }
        }

        public static IEnumerable<TreeViewItem> All(this TreeViewItem item)
        {
            yield return item;
            
            if (item.Items != null)
            {
                foreach (var child in item.Items.OfType<TreeViewItem>())
                {
                    foreach (var descendant in child.All())
                    {
                        yield return descendant;
                    }
                }
            }
        }

        public static IEnumerable<TreeViewItem> AllItems(this TreeView treeView)
        {
            if (treeView?.Items != null)
            {
                foreach (var item in treeView.Items.OfType<TreeViewItem>())
                {
                    foreach (var descendant in item.All())
                    {
                        yield return descendant;
                    }
                }
            }
        }

        public static void SelectTabWithoutFocus(this TabControl tabControl, TabItem tabItem)
        {
            if (tabControl != null && tabItem != null)
            {
                tabControl.SelectedItem = tabItem;
            }
        }
    }
}
