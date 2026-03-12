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

namespace ShareX.HelpersLib
{
    public class DoubleLabeledNumericUpDown : UserControl
    {
        private readonly TextBlock lblText;
        private readonly NumericUpDown nudValue;
        private readonly TextBlock lblText2;
        private readonly NumericUpDown nudValue2;

        public new string Text
        {
            get => lblText.Text;
            set => lblText.Text = value;
        }

        public string Text2
        {
            get => lblText2.Text;
            set => lblText2.Text = value;
        }

        public decimal Value
        {
            get => nudValue.Value ?? 0;
            set => nudValue.SetValue(value);
        }

        public decimal Value2
        {
            get => nudValue2.Value ?? 0;
            set => nudValue2.SetValue(value);
        }

        public decimal Maximum
        {
            get => nudValue.Maximum ?? decimal.MaxValue;
            set
            {
                nudValue.Maximum = value;
                nudValue2.Maximum = value;
            }
        }

        public decimal Minimum
        {
            get => nudValue.Minimum ?? decimal.MinValue;
            set
            {
                nudValue.Minimum = value;
                nudValue2.Minimum = value;
            }
        }

        public decimal Increment
        {
            get => nudValue.Increment;
            set
            {
                nudValue.Increment = value;
                nudValue2.Increment = value;
            }
        }

        public event EventHandler ValueChanged;

        public DoubleLabeledNumericUpDown()
        {
            lblText = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
            nudValue = new NumericUpDown { Width = 60, Minimum = 0, Maximum = 100 };
            lblText2 = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
            nudValue2 = new NumericUpDown { Width = 60, Minimum = 0, Maximum = 100 };

            nudValue.ValueChanged += OnValueChanged;
            nudValue2.ValueChanged += OnValueChanged;

            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 4
            };

            panel.Children.Add(lblText);
            panel.Children.Add(nudValue);
            panel.Children.Add(lblText2);
            panel.Children.Add(nudValue2);

            Content = panel;
        }

        private void OnValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {
            ValueChanged?.Invoke(sender, e);
        }
    }
}
