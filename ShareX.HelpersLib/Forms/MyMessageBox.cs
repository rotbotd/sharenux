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
using ShareX.HelpersLib.Properties;
using System;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public class MyMessageBox : Window
    {
        private const int LabelHorizontalPadding = 15;
        private const int LabelVerticalPadding = 20;
        private const int ButtonPadding = 10;

        private DialogResult dialogResult = DialogResult.None;
        private DialogResult button1Result = DialogResult.OK;
        private DialogResult button2Result = DialogResult.No;

        public bool IsChecked { get; private set; }

        public MyMessageBox(string text, string caption, MessageBoxButtons buttons = MessageBoxButtons.OK, string checkBoxText = null, bool isChecked = false)
        {
            Title = caption;
            Width = 300;
            MinWidth = 180;
            SizeToContent = SizeToContent.Height;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CanResize = false;
            Topmost = true;

            var mainPanel = new StackPanel { Spacing = 0 };

            var labelPanel = new StackPanel
            {
                Margin = new Thickness(LabelHorizontalPadding, LabelVerticalPadding),
                Spacing = 10
            };

            var labelText = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 400
            };
            labelPanel.Children.Add(labelText);

            if (checkBoxText != null)
            {
                IsChecked = isChecked;
                var checkBox = new CheckBox
                {
                    Content = checkBoxText,
                    IsChecked = isChecked
                };
                checkBox.IsCheckedChanged += (s, e) => IsChecked = checkBox.IsChecked ?? false;
                labelPanel.Children.Add(checkBox);
            }

            mainPanel.Children.Add(labelPanel);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(ButtonPadding),
                Spacing = ButtonPadding,
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240))
            };

            var button1 = new Button
            {
                Width = 80,
                Height = 26,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            button1.Click += (s, e) =>
            {
                dialogResult = button1Result;
                Close();
            };

            var button2 = new Button
            {
                Width = 80,
                Height = 26,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            button2.Click += (s, e) =>
            {
                dialogResult = button2Result;
                Close();
            };

            switch (buttons)
            {
                default:
                case MessageBoxButtons.OK:
                    button1.Content = Resources.MyMessageBox_MyMessageBox_OK;
                    button1Result = DialogResult.OK;
                    button2.IsVisible = false;
                    break;
                case MessageBoxButtons.OKCancel:
                    button1.Content = Resources.MyMessageBox_MyMessageBox_OK;
                    button1Result = DialogResult.OK;
                    button2.Content = Resources.MyMessageBox_MyMessageBox_Cancel;
                    button2Result = DialogResult.Cancel;
                    break;
                case MessageBoxButtons.YesNo:
                    button1.Content = Resources.MyMessageBox_MyMessageBox_Yes;
                    button1Result = DialogResult.Yes;
                    button2.Content = Resources.MyMessageBox_MyMessageBox_No;
                    button2Result = DialogResult.No;
                    break;
            }

            buttonPanel.Children.Add(button1);
            if (button2.IsVisible)
                buttonPanel.Children.Add(button2);

            var borderPanel = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                Child = buttonPanel
            };

            mainPanel.Children.Add(borderPanel);

            Content = mainPanel;

            ShareXResources.ApplyTheme(this);
        }

        public new DialogResult ShowDialog()
        {
            base.ShowDialog(null).GetAwaiter().GetResult();
            return dialogResult;
        }

        public async Task<DialogResult> ShowDialogAsync(Window owner = null)
        {
            await base.ShowDialog(owner);
            return dialogResult;
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            var messageBox = new MyMessageBox(text, caption, buttons);
            return messageBox.ShowDialog();
        }
    }
}
