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
using Avalonia.Layout;
using Avalonia.Media;
using ShareX.HelpersLib.Properties;
using System;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public class InputBox : Window
    {
        public string InputText { get; private set; }

        private TextBox txtInputText;
        private Button btnOK;
        private Button btnCancel;
        private DialogResult dialogResult = DialogResult.Cancel;

        private InputBox(string title, string inputText = null, string okText = null, string cancelText = null)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            InputText = inputText;

            Title = "ShareX - " + title;
            if (!string.IsNullOrEmpty(InputText))
                txtInputText.Text = InputText;
            if (!string.IsNullOrEmpty(okText))
                btnOK.Content = okText;
            if (!string.IsNullOrEmpty(cancelText))
                btnCancel.Content = cancelText;
        }

        private void InitializeComponent()
        {
            Width = 384;
            MinWidth = 384;
            MaxWidth = 1000;
            SizeToContent = SizeToContent.Height;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CanResize = true;
            Topmost = true;

            var mainPanel = new StackPanel
            {
                Margin = new Thickness(10),
                Spacing = 10
            };

            txtInputText = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            txtInputText.KeyDown += TxtInputText_KeyDown;
            mainPanel.Children.Add(txtInputText);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 10
            };

            btnOK = new Button
            {
                Content = Resources.MyMessageBox_MyMessageBox_OK,
                Width = 80,
                Height = 26,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            btnOK.Click += BtnOK_Click;
            buttonPanel.Children.Add(btnOK);

            btnCancel = new Button
            {
                Content = Resources.MyMessageBox_MyMessageBox_Cancel,
                Width = 80,
                Height = 26,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            btnCancel.Click += BtnCancel_Click;
            buttonPanel.Children.Add(btnCancel);

            mainPanel.Children.Add(buttonPanel);

            Content = mainPanel;

            Opened += InputBox_Opened;
        }

        private void InputBox_Opened(object sender, EventArgs e)
        {
            txtInputText.Focus();
            txtInputText.SelectAll();
        }

        private void TxtInputText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnOK_Click(sender, e);
            }
            else if (e.Key == Key.Escape)
            {
                BtnCancel_Click(sender, e);
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            InputText = txtInputText.Text;
            dialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            dialogResult = DialogResult.Cancel;
            Close();
        }

        public new DialogResult ShowDialog()
        {
            base.ShowDialog(null).GetAwaiter().GetResult();
            return dialogResult;
        }

        public static string Show(string title, string inputText = null, string okText = null, string cancelText = null)
        {
            var form = new InputBox(title, inputText, okText, cancelText);
            if (form.ShowDialog() == DialogResult.OK)
            {
                return form.InputText;
            }
            return null;
        }
    }
}
