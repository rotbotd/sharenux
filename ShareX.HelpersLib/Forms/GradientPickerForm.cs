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
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ShareX.HelpersLib.Properties;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace ShareX.HelpersLib
{
    public class GradientPickerForm : Window
    {
        public delegate void GradientChangedEventHandler();
        public event GradientChangedEventHandler GradientChanged;

        public GradientInfo Gradient { get; private set; }

        private bool isReady;
        private ComboBox cbGradientType;
        private ListBox lvGradientPoints;
        private ListBox lvPresets;
        private Avalonia.Controls.Image pbPreview;
        private ColorButton cbtnCurrentColor;
        private NumericUpDown nudLocation;
        private Button btnAdd;
        private Button btnRemove;
        private Button btnClear;
        private Button btnReverse;
        private Button btnOK;
        private Button btnCancel;

        private ObservableCollection<GradientStopItem> gradientStopItems = new ObservableCollection<GradientStopItem>();
        private ObservableCollection<GradientPresetItem> presetItems = new ObservableCollection<GradientPresetItem>();
        private DialogResult dialogResult = DialogResult.Cancel;

        public GradientPickerForm(GradientInfo gradient)
        {
            Gradient = gradient;
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            cbGradientType.ItemsSource = Helpers.GetLocalizedEnumDescriptions<LinearGradientMode>();
            cbGradientType.SelectedIndex = (int)Gradient.Type;
            UpdateGradientList(true);
        }

        private void InitializeComponent()
        {
            Title = Resources.GradientPickerForm_GradientPickerForm_Gradient_picker;
            Width = 750;
            Height = 500;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CanResize = true;

            var mainGrid = new Grid
            {
                RowDefinitions = new RowDefinitions("*,Auto"),
                ColumnDefinitions = new ColumnDefinitions("300,*"),
                Margin = new Thickness(10)
            };

            // Left panel - gradient stops
            var leftPanel = new StackPanel { Spacing = 10 };

            // Gradient type
            var typePanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
            typePanel.Children.Add(new TextBlock { Text = Resources.GradientPickerForm_GradientPickerForm_Type, VerticalAlignment = VerticalAlignment.Center });
            cbGradientType = new ComboBox { Width = 150 };
            cbGradientType.SelectionChanged += CbGradientType_SelectionChanged;
            typePanel.Children.Add(cbGradientType);
            leftPanel.Children.Add(typePanel);

            // Preview
            var previewBorder = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Height = 50
            };
            pbPreview = new Avalonia.Controls.Image { Stretch = Stretch.Fill };
            previewBorder.Child = pbPreview;
            leftPanel.Children.Add(previewBorder);

            // Gradient stops list
            leftPanel.Children.Add(new TextBlock { Text = Resources.GradientPickerForm_GradientPickerForm_Gradient_stops });
            lvGradientPoints = new ListBox
            {
                Height = 150,
                ItemsSource = gradientStopItems
            };
            lvGradientPoints.SelectionChanged += LvGradientPoints_SelectionChanged;
            lvGradientPoints.DoubleTapped += LvGradientPoints_DoubleTapped;
            leftPanel.Children.Add(lvGradientPoints);

            // Color and location
            var colorPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
            colorPanel.Children.Add(new TextBlock { Text = Resources.GradientPickerForm_GradientPickerForm_Color, VerticalAlignment = VerticalAlignment.Center });
            cbtnCurrentColor = new ColorButton { Width = 80, Height = 26 };
            cbtnCurrentColor.ColorChanged += CbtnCurrentColor_ColorChanged;
            colorPanel.Children.Add(cbtnCurrentColor);
            colorPanel.Children.Add(new TextBlock { Text = Resources.GradientPickerForm_GradientPickerForm_Location, VerticalAlignment = VerticalAlignment.Center });
            nudLocation = new NumericUpDown { Width = 80, Minimum = 0, Maximum = 100, Increment = 1, Value = 0 };
            nudLocation.ValueChanged += NudLocation_ValueChanged;
            colorPanel.Children.Add(nudLocation);
            colorPanel.Children.Add(new TextBlock { Text = "%", VerticalAlignment = VerticalAlignment.Center });
            leftPanel.Children.Add(colorPanel);

            // Buttons
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5 };
            btnAdd = new Button { Content = Resources.GradientPickerForm_GradientPickerForm_Add };
            btnAdd.Click += BtnAdd_Click;
            buttonPanel.Children.Add(btnAdd);
            btnRemove = new Button { Content = Resources.GradientPickerForm_GradientPickerForm_Remove };
            btnRemove.Click += BtnRemove_Click;
            buttonPanel.Children.Add(btnRemove);
            btnClear = new Button { Content = Resources.GradientPickerForm_GradientPickerForm_Clear };
            btnClear.Click += BtnClear_Click;
            buttonPanel.Children.Add(btnClear);
            btnReverse = new Button { Content = Resources.GradientPickerForm_GradientPickerForm_Reverse };
            btnReverse.Click += BtnReverse_Click;
            buttonPanel.Children.Add(btnReverse);
            leftPanel.Children.Add(buttonPanel);

            Grid.SetRow(leftPanel, 0);
            Grid.SetColumn(leftPanel, 0);
            mainGrid.Children.Add(leftPanel);

            // Right panel - presets
            var rightPanel = new StackPanel { Spacing = 10, Margin = new Thickness(10, 0, 0, 0) };
            rightPanel.Children.Add(new TextBlock { Text = Resources.GradientPickerForm_GradientPickerForm_Presets });

            var presetsScroll = new ScrollViewer { Height = 350 };
            var presetsWrap = new WrapPanel();
            lvPresets = new ListBox
            {
                ItemsSource = presetItems,
                ItemsPanel = new FuncTemplate<Panel>(() => new WrapPanel())
            };
            lvPresets.SelectionChanged += LvPresets_SelectionChanged;
            presetsScroll.Content = lvPresets;
            rightPanel.Children.Add(presetsScroll);

            Grid.SetRow(rightPanel, 0);
            Grid.SetColumn(rightPanel, 1);
            mainGrid.Children.Add(rightPanel);

            // Bottom buttons
            var bottomPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 10,
                Margin = new Thickness(0, 10, 0, 0)
            };
            btnOK = new Button { Content = Resources.MyMessageBox_MyMessageBox_OK, Width = 80 };
            btnOK.Click += BtnOK_Click;
            bottomPanel.Children.Add(btnOK);
            btnCancel = new Button { Content = Resources.MyMessageBox_MyMessageBox_Cancel, Width = 80 };
            btnCancel.Click += BtnCancel_Click;
            bottomPanel.Children.Add(btnCancel);

            Grid.SetRow(bottomPanel, 1);
            Grid.SetColumnSpan(bottomPanel, 2);
            mainGrid.Children.Add(bottomPanel);

            Content = mainGrid;

            Opened += GradientPickerForm_Opened;
        }

        protected virtual void OnGradientChanged()
        {
            GradientChanged?.Invoke();
        }

        private void AddPresets()
        {
            GradientInfo[] gradients = GetPresetGradients();

            presetItems.Clear();

            foreach (var gradient in gradients)
            {
                gradient.Type = Gradient.Type;
                var bmp = gradient.CreateGradientPreview(64, 64, true);
                presetItems.Add(new GradientPresetItem { Gradient = gradient, Preview = ConvertToAvaloniaBitmap(bmp) });
                bmp.Dispose();
            }
        }

        private Avalonia.Media.Imaging.Bitmap ConvertToAvaloniaBitmap(SKBitmap skBitmap)
        {
            using var data = skBitmap.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream();
            data.SaveTo(stream);
            stream.Position = 0;
            return new Avalonia.Media.Imaging.Bitmap(stream);
        }

        private GradientInfo[] GetPresetGradients()
        {
            // Return a subset of presets for now - the full list is huge
            return new GradientInfo[]
            {
                new GradientInfo(SKColor.Parse("#B80BC3"), SKColor.Parse("#6236FF")),
                new GradientInfo(SKColor.Parse("#FF0387"), SKColor.Parse("#FF8F03")),
                new GradientInfo(SKColor.Parse("#00BB8A"), SKColor.Parse("#0069A3")),
                new GradientInfo(SKColor.Parse("#98968E"), SKColor.Parse("#DBD3D8")),
                new GradientInfo(SKColor.Parse("#091E3A"), SKColor.Parse("#2F80ED"), SKColor.Parse("#2D9EE0")),
                new GradientInfo(SKColor.Parse("#9400D3"), SKColor.Parse("#4B0082")),
                new GradientInfo(SKColor.Parse("#C84E89"), SKColor.Parse("#F15F79")),
                new GradientInfo(SKColor.Parse("#00F5A0"), SKColor.Parse("#00D9F5")),
                new GradientInfo(SKColor.Parse("#F7941E"), SKColor.Parse("#72C6EF"), SKColor.Parse("#00A651")),
                new GradientInfo(SKColor.Parse("#F7941E"), SKColor.Parse("#004E8F")),
                new GradientInfo(SKColor.Parse("#FF0000"), SKColor.Parse("#FF00FF"), SKColor.Parse("#0000FF"), SKColor.Parse("#00FFFF"), SKColor.Parse("#00FF00"), SKColor.Parse("#FFFF00"), SKColor.Parse("#FF0000"))
            };
        }

        private void UpdateGradientList(bool selectFirst = false)
        {
            isReady = false;
            Gradient.Sort();

            gradientStopItems.Clear();
            foreach (GradientStop gradientStop in Gradient.Colors)
            {
                gradientStopItems.Add(new GradientStopItem { GradientStop = gradientStop });
            }

            if (selectFirst && gradientStopItems.Count > 0)
            {
                lvGradientPoints.SelectedIndex = 0;
            }

            isReady = true;
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (isReady)
            {
                var bmp = Gradient.CreateGradientPreview((int)pbPreview.Bounds.Width.Clamp(1, 1000), (int)pbPreview.Bounds.Height.Clamp(1, 100), true);
                pbPreview.Source = ConvertToAvaloniaBitmap(bmp);
                bmp.Dispose();

                OnGradientChanged();
            }
        }

        private GradientStop GetSelectedGradientStop()
        {
            if (lvGradientPoints.SelectedItem is GradientStopItem item)
            {
                return item.GradientStop;
            }
            return null;
        }

        private void SelectGradientStop(GradientStop gradientStop)
        {
            for (int i = 0; i < gradientStopItems.Count; i++)
            {
                if (gradientStopItems[i].GradientStop == gradientStop)
                {
                    lvGradientPoints.SelectedIndex = i;
                    return;
                }
            }
        }

        private void GradientPickerForm_Opened(object sender, EventArgs e)
        {
            AddPresets();
            UpdatePreview();
        }

        private void CbGradientType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isReady)
            {
                Gradient.Type = (LinearGradientMode)cbGradientType.SelectedIndex;
                UpdatePreview();
                AddPresets();
            }
        }

        private void BtnAdd_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SKColor color = cbtnCurrentColor.Color;
            float offset = (float)(nudLocation.Value ?? 0);
            GradientStop gradientStop = new GradientStop(color, offset);
            Gradient.Colors.Add(gradientStop);
            UpdateGradientList();
            SelectGradientStop(gradientStop);
        }

        private void BtnRemove_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            GradientStop gradientStop = GetSelectedGradientStop();

            if (gradientStop != null)
            {
                Gradient.Colors.Remove(gradientStop);
                UpdateGradientList();
            }
        }

        private void BtnClear_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Gradient.Clear();
            UpdateGradientList();
        }

        private void BtnReverse_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Gradient.IsValid)
            {
                Gradient.Reverse();
                UpdateGradientList();
            }
        }

        private void CbtnCurrentColor_ColorChanged(SKColor color)
        {
            GradientStop gradientStop = GetSelectedGradientStop();

            if (gradientStop != null)
            {
                gradientStop.Color = color;
                UpdateGradientList();
                SelectGradientStop(gradientStop);
            }
        }

        private void NudLocation_ValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {
            if (isReady)
            {
                GradientStop gradientStop = GetSelectedGradientStop();

                if (gradientStop != null)
                {
                    gradientStop.Location = (float)(nudLocation.Value ?? 0);
                    UpdateGradientList();
                    SelectGradientStop(gradientStop);
                }
            }
        }

        private void LvGradientPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GradientStop gradientStop = GetSelectedGradientStop();

            if (gradientStop != null)
            {
                isReady = false;
                cbtnCurrentColor.Color = gradientStop.Color;
                nudLocation.Value = (decimal)gradientStop.Location;
                isReady = true;
            }
        }

        private void LvGradientPoints_DoubleTapped(object sender, TappedEventArgs e)
        {
            if (lvGradientPoints.SelectedItem != null)
            {
                cbtnCurrentColor.ShowColorDialog();
            }
        }

        private void LvPresets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isReady && lvPresets.SelectedItem is GradientPresetItem presetItem)
            {
                Gradient = presetItem.Gradient.Copy();
                UpdateGradientList(true);
                lvPresets.SelectedItem = null;
            }
        }

        private void BtnOK_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            dialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            dialogResult = DialogResult.Cancel;
            Close();
        }

        public new DialogResult ShowDialog()
        {
            base.ShowDialog(null).GetAwaiter().GetResult();
            return dialogResult;
        }
    }

    public class GradientStopItem
    {
        public GradientStop GradientStop { get; set; }
        public override string ToString() => $" {GradientStop.Location:0.##}% - #{GradientStop.Color.Red:X2}{GradientStop.Color.Green:X2}{GradientStop.Color.Blue:X2}";
    }

    public class GradientPresetItem
    {
        public GradientInfo Gradient { get; set; }
        public Avalonia.Media.Imaging.Bitmap Preview { get; set; }
    }
}
