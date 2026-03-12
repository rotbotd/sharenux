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
using Avalonia.Media.Imaging;
using ShareX.HelpersLib.Properties;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShareX.HelpersLib
{
    public class ImageViewer : Window
    {
        public SKBitmap CurrentImage { get; private set; }
        public string CurrentImageFilePath { get; private set; }
        public bool SupportWrap { get; set; }
        public bool CanNavigate => Images != null && Images.Length > 1;
        public bool CanNavigateLeft => CanNavigate && (SupportWrap || CurrentImageIndex > 0);
        public bool CanNavigateRight => CanNavigate && (SupportWrap || CurrentImageIndex < Images.Length - 1);
        public string[] Images { get; private set; }
        public int CurrentImageIndex { get; private set; }
        public int NavigationButtonWidth { get; set; } = 100;
        public string Status { get; private set; }

        private Avalonia.Controls.Image imageControl;
        private TextBlock lblStatus;
        private TextBlock lblLeft;
        private TextBlock lblRight;
        private Avalonia.Media.Imaging.Bitmap displayBitmap;

        private ImageViewer(SKBitmap img)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);
            LoadImage(img);
        }

        private ImageViewer(string[] images, int currentImageIndex = 0)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            Images = images;
            CurrentImageIndex = currentImageIndex;
            FilterImageFiles();
            LoadCurrentImage();
        }

        private void InitializeComponent()
        {
            Title = Resources.ShareXImageViewer;
            WindowState = WindowState.FullScreen;
            SystemDecorations = SystemDecorations.None;
            Background = Brushes.Black;
            Topmost = true;

            var grid = new Grid();

            imageControl = new Avalonia.Controls.Image
            {
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            grid.Children.Add(imageControl);

            lblStatus = new TextBlock
            {
                FontSize = 16,
                FontFamily = new FontFamily("Arial"),
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),
                Padding = new Thickness(10, 5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                IsVisible = false
            };
            grid.Children.Add(lblStatus);

            lblLeft = new TextBlock
            {
                Text = "‹",
                FontSize = 60,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(64, 0, 0, 0)),
                Width = NavigationButtonWidth,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Left,
                IsVisible = false,
                Cursor = new Cursor(StandardCursorType.Hand)
            };
            lblLeft.PointerPressed += LblLeft_PointerPressed;
            grid.Children.Add(lblLeft);

            lblRight = new TextBlock
            {
                Text = "›",
                FontSize = 60,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(64, 0, 0, 0)),
                Width = NavigationButtonWidth,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Right,
                IsVisible = false,
                Cursor = new Cursor(StandardCursorType.Hand)
            };
            lblRight.PointerPressed += LblRight_PointerPressed;
            grid.Children.Add(lblRight);

            Content = grid;

            KeyDown += ImageViewer_KeyDown;
            PointerPressed += ImageViewer_PointerPressed;
            PointerMoved += ImageViewer_PointerMoved;
            PointerWheelChanged += ImageViewer_PointerWheelChanged;
            Deactivated += ImageViewer_Deactivated;
        }

        private void LoadImage(SKBitmap img)
        {
            CurrentImage?.Dispose();
            displayBitmap?.Dispose();
            CurrentImage = img;

            if (img != null)
            {
                using var data = img.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = new MemoryStream();
                data.SaveTo(stream);
                stream.Position = 0;
                displayBitmap = new Avalonia.Media.Imaging.Bitmap(stream);
                imageControl.Source = displayBitmap;
            }
            else
            {
                imageControl.Source = null;
            }
        }

        private void LoadCurrentImage()
        {
            if (Images != null && Images.Length > 0)
            {
                CurrentImageIndex = CurrentImageIndex.Clamp(0, Images.Length - 1);
                CurrentImageFilePath = Images[CurrentImageIndex];
                SKBitmap img = ImageHelpers.LoadImage(CurrentImageFilePath);
                LoadImage(img);
            }

            UpdateStatus();
        }

        private void NavigateImage(int position)
        {
            if (CanNavigate)
            {
                int nextImageIndex = CurrentImageIndex + position;

                if (SupportWrap)
                {
                    if (nextImageIndex > Images.Length - 1)
                        nextImageIndex = 0;
                    else if (nextImageIndex < 0)
                        nextImageIndex = Images.Length - 1;
                }

                nextImageIndex = nextImageIndex.Clamp(0, Images.Length - 1);

                if (CurrentImageIndex != nextImageIndex)
                {
                    CurrentImageIndex = nextImageIndex;
                    LoadCurrentImage();
                }
            }
        }

        private void FilterImageFiles()
        {
            List<string> filteredImages = new List<string>();

            for (int i = 0; i < Images.Length; i++)
            {
                string imageFilePath = Images[i];
                bool isImageFile = !string.IsNullOrEmpty(imageFilePath) && FileHelpers.IsImageFile(imageFilePath);

                if (i == CurrentImageIndex)
                {
                    CurrentImageIndex = isImageFile ? filteredImages.Count : Math.Max(filteredImages.Count - 1, 0);
                }

                if (isImageFile)
                {
                    filteredImages.Add(imageFilePath);
                }
            }

            Images = filteredImages.ToArray();
        }

        private void UpdateStatus()
        {
            Status = "";

            if (CanNavigate)
            {
                AppendStatus($"{CurrentImageIndex + 1} / {Images.Length}");
            }

            string fileName = FileHelpers.GetFileNameSafe(CurrentImageFilePath);
            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = fileName.Truncate(128, "...");
                AppendStatus(fileName);
            }

            if (CurrentImage != null)
            {
                AppendStatus($"{CurrentImage.Width} x {CurrentImage.Height}");
            }

            lblStatus.IsVisible = !string.IsNullOrEmpty(Status);
            lblStatus.Text = Status;
        }

        private void AppendStatus(string text)
        {
            if (!string.IsNullOrEmpty(Status))
                Status += " │ ";
            Status += text;
        }

        public static void ShowImage(SKBitmap img)
        {
            if (img != null)
            {
                var tempImage = img.Copy();
                if (tempImage != null)
                {
                    var viewer = new ImageViewer(tempImage);
                    viewer.ShowDialog(null);
                }
            }
        }

        public static void ShowImage(string filePath)
        {
            var bmp = ImageHelpers.LoadImage(filePath);
            if (bmp != null)
            {
                var viewer = new ImageViewer(bmp);
                viewer.ShowDialog(null);
            }
        }

        public static void ShowImage(string[] files, int imageIndex = 0)
        {
            if (files != null && files.Length > 0)
            {
                var viewer = new ImageViewer(files, imageIndex);
                viewer.ShowDialog(null);
            }
        }

        private void ImageViewer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    NavigateImage(-1);
                    break;
                case Key.Right:
                    NavigateImage(1);
                    break;
                case Key.Escape:
                case Key.Enter:
                case Key.Space:
                    Close();
                    break;
            }
        }

        private void ImageViewer_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(this);
            if (point.Properties.IsLeftButtonPressed)
            {
                Close();
            }
        }

        private void ImageViewer_PointerMoved(object sender, PointerEventArgs e)
        {
            var pos = e.GetPosition(this);
            lblLeft.IsVisible = CanNavigateLeft && pos.X < NavigationButtonWidth;
            lblRight.IsVisible = CanNavigateRight && pos.X > Bounds.Width - NavigationButtonWidth;
            lblStatus.IsVisible = !string.IsNullOrEmpty(Status) && pos.Y < 50;
        }

        private void ImageViewer_PointerWheelChanged(object sender, PointerWheelEventArgs e)
        {
            if (CanNavigateLeft && e.Delta.Y > 0)
                NavigateImage(-1);
            else if (CanNavigateRight && e.Delta.Y < 0)
                NavigateImage(1);
        }

        private void ImageViewer_Deactivated(object sender, EventArgs e)
        {
            Close();
        }

        private void LblLeft_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            NavigateImage(-1);
            lblLeft.IsVisible = CanNavigateLeft;
            e.Handled = true;
        }

        private void LblRight_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            NavigateImage(1);
            lblRight.IsVisible = CanNavigateRight;
            e.Handled = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CurrentImage?.Dispose();
            displayBitmap?.Dispose();
        }
    }
}
