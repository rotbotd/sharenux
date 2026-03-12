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
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using SkiaSharp;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public class MyPictureBox : UserControl
    {
        private readonly Avalonia.Controls.Image imageControl;
        private readonly TextBlock lblStatus;
        private readonly TextBlock lblImageSize;
        private readonly Grid mainGrid;
        private readonly Border imageBorder;

        private SKBitmap currentImage;
        private Avalonia.Media.Imaging.Bitmap displayBitmap;
        private bool isImageLoading;
        private readonly object imageLoadLock = new object();

        public SKBitmap Image
        {
            get => currentImage;
            private set
            {
                currentImage?.Dispose();
                currentImage = value;
                UpdateDisplayBitmap();
            }
        }

        private string statusText = "";

        public new string Text
        {
            get => statusText;
            set
            {
                statusText = value;
                if (string.IsNullOrEmpty(value))
                {
                    lblStatus.IsVisible = false;
                }
                else
                {
                    lblStatus.Text = value;
                    lblStatus.IsVisible = true;
                }
            }
        }

        public SKColor PictureBoxBackColor { get; set; } = SKColors.Transparent;

        public bool DrawCheckeredBackground { get; set; }

        public bool FullscreenOnClick { get; set; }

        public bool EnableRightClickMenu { get; set; }

        public bool ShowImageSizeLabel { get; set; }

        public bool IsValidImage => !isImageLoading && currentImage != null && currentImage.Width > 0 && currentImage.Height > 0;

        public MyPictureBox()
        {
            mainGrid = new Grid();

            imageBorder = new Border
            {
                ClipToBounds = true
            };

            imageControl = new Avalonia.Controls.Image
            {
                Stretch = Stretch.Uniform,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            imageBorder.Child = imageControl;

            lblStatus = new TextBlock
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                IsVisible = false
            };

            lblImageSize = new TextBlock
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 4),
                IsVisible = false
            };

            mainGrid.Children.Add(imageBorder);
            mainGrid.Children.Add(lblStatus);
            mainGrid.Children.Add(lblImageSize);

            Content = mainGrid;

            Text = "";
            UpdateTheme();

            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            PointerMoved += OnPointerMoved;
            PointerExited += OnPointerExited;
        }

        private void UpdateDisplayBitmap()
        {
            displayBitmap?.Dispose();
            displayBitmap = null;

            if (currentImage != null)
            {
                using var data = currentImage.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = new MemoryStream();
                data.SaveTo(stream);
                stream.Position = 0;
                displayBitmap = new Avalonia.Media.Imaging.Bitmap(stream);
            }

            imageControl.Source = displayBitmap;
            UpdateImageSizeLabel();
            AutoSetStretch();
        }

        private void UpdateImageSizeLabel()
        {
            if (IsValidImage)
            {
                lblImageSize.Text = $"{currentImage.Width} x {currentImage.Height}";
            }
        }

        public void UpdateTheme()
        {
            lblImageSize.Background = new SolidColorBrush(ShareXResources.Theme.GetAvaloniaBackgroundColor());
            lblImageSize.Foreground = new SolidColorBrush(ShareXResources.Theme.GetAvaloniaTextColor());
            lblStatus.Foreground = new SolidColorBrush(ShareXResources.Theme.GetAvaloniaTextColor());
        }

        public void UpdateCheckers(bool forceUpdate = false)
        {
            if (DrawCheckeredBackground && ShareXResources.Theme.CheckerSize > 0)
            {
                var checker = ImageHelpers.CreateCheckerPattern(
                    ShareXResources.Theme.CheckerSize,
                    ShareXResources.Theme.CheckerSize,
                    ShareXResources.Theme.CheckerColor,
                    ShareXResources.Theme.CheckerColor2);

                using var data = checker.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = new MemoryStream();
                data.SaveTo(stream);
                stream.Position = 0;

                var checkerBitmap = new Avalonia.Media.Imaging.Bitmap(stream);
                imageBorder.Background = new ImageBrush(checkerBitmap)
                {
                    TileMode = TileMode.Tile,
                    SourceRect = new RelativeRect(0, 0, checker.Width, checker.Height, RelativeUnit.Absolute)
                };
                checker.Dispose();
            }
            else
            {
                imageBorder.Background = null;
            }
        }

        public void LoadImage(SKBitmap img)
        {
            lock (imageLoadLock)
            {
                if (!isImageLoading)
                {
                    Reset();

                    if (img != null)
                    {
                        isImageLoading = true;
                        Image = img.Copy();
                        isImageLoading = false;
                    }
                    else
                    {
                        Image = null;
                    }
                }
            }
        }

        public void LoadImageFromFile(string filePath)
        {
            lock (imageLoadLock)
            {
                if (!isImageLoading)
                {
                    Reset();
                    isImageLoading = true;
                    Image = ImageHelpers.LoadImage(filePath);
                    isImageLoading = false;
                }
            }
        }

        public void LoadImageFromFileAsync(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                LoadImageAsync(filePath, isUrl: false);
            }
        }

        public void LoadImageFromURLAsync(string url)
        {
            if (!string.IsNullOrEmpty(url) && !url.StartsWith("ftp://") && !url.StartsWith("ftps://"))
            {
                LoadImageAsync(url, isUrl: true);
            }
        }

        private async void LoadImageAsync(string path, bool isUrl)
        {
            lock (imageLoadLock)
            {
                if (isImageLoading) return;
                isImageLoading = true;
            }

            Reset();
            Text = "Loading image...";

            try
            {
                SKBitmap bitmap = null;

                if (isUrl)
                {
                    using var client = new HttpClient();
                    var data = await client.GetByteArrayAsync(path);
                    bitmap = SKBitmap.Decode(data);
                }
                else
                {
                    bitmap = await Task.Run(() => SKBitmap.Decode(path));
                }

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Image = bitmap;
                    Text = "";
                });
            }
            catch
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Text = "";
                    Reset();
                });
            }
            finally
            {
                isImageLoading = false;
            }
        }

        public void Reset()
        {
            if (!isImageLoading && currentImage != null)
            {
                Image = null;
            }

            if (FullscreenOnClick)
            {
                Cursor = Cursor.Default;
            }
        }

        private void AutoSetStretch()
        {
            if (IsValidImage)
            {
                var bounds = Bounds;
                if (currentImage.Width > bounds.Width || currentImage.Height > bounds.Height)
                {
                    imageControl.Stretch = Stretch.Uniform;
                }
                else
                {
                    imageControl.Stretch = Stretch.None;
                }

                if (FullscreenOnClick)
                {
                    Cursor = new Cursor(StandardCursorType.Hand);
                }
            }

            UpdateImageSizeLabel();
        }

        private void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(this);

            if (FullscreenOnClick && point.Properties.IsLeftButtonPressed && IsValidImage)
            {
                IsEnabled = false;
                ImageViewer.ShowImage(currentImage);
                IsEnabled = true;
            }
        }

        private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var point = e.GetCurrentPoint(this);

            if (EnableRightClickMenu && point.Properties.PointerUpdateKind == PointerUpdateKind.RightButtonReleased && IsValidImage)
            {
                var menu = new ContextMenu();
                var copyItem = new MenuItem { Header = "Copy Image" };
                copyItem.Click += (s, args) =>
                {
                    if (IsValidImage)
                    {
                        ClipboardHelpers.CopyImage(currentImage);
                    }
                };
                menu.Items.Add(copyItem);
                menu.Open(this);
            }
        }

        private void OnPointerMoved(object sender, PointerEventArgs e)
        {
            lblImageSize.IsVisible = ShowImageSizeLabel && IsValidImage;
        }

        private void OnPointerExited(object sender, PointerEventArgs e)
        {
            lblImageSize.IsVisible = false;
        }

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateCheckers();
            AutoSetStretch();
        }
    }
}
