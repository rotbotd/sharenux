using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public partial class HashCheckerWindow : Window
    {
        private CancellationTokenSource? hashCts;
        private bool isWorking;

        public HashCheckerWindow()
        {
            InitializeComponent();
            
            // Populate hash types - default to SHA256
            cbHashType.ItemsSource = Enum.GetNames(typeof(HashType));
            cbHashType.SelectedIndex = (int)HashType.SHA256;
            
            // Wire up events
            btnFilePathBrowse.Click += BtnFilePathBrowse_Click;
            btnFilePathBrowse2.Click += BtnFilePathBrowse2_Click;
            btnStartHashCheck.Click += BtnStartHashCheck_Click;
            cbCompareTwoFiles.IsCheckedChanged += CbCompareTwoFiles_CheckedChanged;
            
            txtResult.TextChanged += TxtResult_TextChanged;
            txtTarget.TextChanged += TxtTarget_TextChanged;
            
            // Drag drop
            AddHandler(DragDrop.DropEvent, OnDrop);
            AddHandler(DragDrop.DragOverEvent, OnDragOver);
            
            // Initial state: file path 2 disabled (grayed out), not hidden
            UpdateFilePath2State();
        }

        private void UpdateFilePath2State()
        {
            bool compare = cbCompareTwoFiles.IsChecked == true;
            
            // Gray out / enable file path 2 controls
            lblFilePath2.IsEnabled = compare;
            txtFilePath2.IsEnabled = compare;
            btnFilePathBrowse2.IsEnabled = compare;
            

        }

        private void OnDragOver(object? sender, DragEventArgs e)
        {
            e.DragEffects = e.Data.Contains(DataFormats.Files) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnDrop(object? sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Files))
            {
                var files = e.Data.GetFiles()?.ToArray();
                if (files?.Length > 0 && files[0] is IStorageFile file)
                {
                    txtFilePath.Text = file.Path.LocalPath;
                }
            }
        }

        private async void BtnFilePathBrowse_Click(object? sender, RoutedEventArgs e)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select file",
                AllowMultiple = false
            });
            
            if (files.Count > 0)
            {
                txtFilePath.Text = files[0].Path.LocalPath;
            }
        }

        private async void BtnFilePathBrowse2_Click(object? sender, RoutedEventArgs e)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select file",
                AllowMultiple = false
            });
            
            if (files.Count > 0)
            {
                txtFilePath2.Text = files[0].Path.LocalPath;
            }
        }

        private void CbCompareTwoFiles_CheckedChanged(object? sender, RoutedEventArgs e)
        {
            UpdateFilePath2State();
        }

        private async void BtnStartHashCheck_Click(object? sender, RoutedEventArgs e)
        {
            if (isWorking)
            {
                hashCts?.Cancel();
                return;
            }

            string filePath = txtFilePath.Text ?? "";
            if (!File.Exists(filePath))
            {
                return;
            }

            HashType hashType = (HashType)cbHashType.SelectedIndex;
            
            isWorking = true;
            btnStartHashCheck.Content = "Cancel";
            pbProgress.Value = 0;
            txtResult.Text = "";

            hashCts = new CancellationTokenSource();
            
            try
            {
                string result;
                
                if (cbCompareTwoFiles.IsChecked == true)
                {
                    string filePath2 = txtFilePath2.Text ?? "";
                    if (!File.Exists(filePath2))
                    {
                        return;
                    }
                    
                    string hash1 = await ComputeHashAsync(filePath, hashType, hashCts.Token);
                    string hash2 = await ComputeHashAsync(filePath2, hashType, hashCts.Token);
                    
                    result = hash1;
                    txtTarget.Text = hash2;
                }
                else
                {
                    result = await ComputeHashAsync(filePath, hashType, hashCts.Token);
                }
                
                txtResult.Text = result;
            }
            catch (OperationCanceledException)
            {
                txtResult.Text = "Cancelled";
            }
            catch (Exception ex)
            {
                txtResult.Text = $"Error: {ex.Message}";
            }
            finally
            {
                isWorking = false;
                btnStartHashCheck.Content = "Check";
                pbProgress.Value = 100;
            }
        }

        private async Task<string> ComputeHashAsync(string filePath, HashType hashType, CancellationToken ct)
        {
            using var stream = File.OpenRead(filePath);
            using var hasher = CreateHashAlgorithm(hashType);
            
            var buffer = new byte[8192];
            long totalRead = 0;
            long fileLength = stream.Length;
            int bytesRead;
            
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
            {
                hasher.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                totalRead += bytesRead;
                pbProgress.Value = (double)totalRead / fileLength * 100;
            }
            
            hasher.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            return BitConverter.ToString(hasher.Hash!).Replace("-", "").ToLowerInvariant();
        }

        private static HashAlgorithm CreateHashAlgorithm(HashType type) => type switch
        {
            HashType.MD5 => MD5.Create(),
            HashType.SHA1 => SHA1.Create(),
            HashType.SHA256 => SHA256.Create(),
            HashType.SHA384 => SHA384.Create(),
            HashType.SHA512 => SHA512.Create(),
            _ => SHA256.Create()
        };

        private void TxtResult_TextChanged(object? sender, TextChangedEventArgs e) => UpdateResultColor();
        private void TxtTarget_TextChanged(object? sender, TextChangedEventArgs e) => UpdateResultColor();

        private void UpdateResultColor()
        {
            string result = txtResult.Text?.Trim() ?? "";
            string target = txtTarget.Text?.Trim() ?? "";
            
            if (string.IsNullOrEmpty(result) || string.IsNullOrEmpty(target))
            {
                txtResult.Foreground = Avalonia.Media.Brushes.White;
            }
            else if (result.Equals(target, StringComparison.OrdinalIgnoreCase))
            {
                txtResult.Foreground = Avalonia.Media.Brushes.LightGreen;
            }
            else
            {
                txtResult.Foreground = Avalonia.Media.Brushes.Red;
            }
        }
    }
}
