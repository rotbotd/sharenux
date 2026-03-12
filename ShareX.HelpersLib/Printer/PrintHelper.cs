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

using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public class PrintHelper : IDisposable
    {
        public PrintType PrintType { get; private set; }
        public SKBitmap Image { get; private set; }
        public string Text { get; private set; }
        public PrintSettings Settings { get; set; }

        public bool Printable
        {
            get
            {
                return Settings != null && ((PrintType == PrintType.Image && Image != null) ||
                    (PrintType == PrintType.Text && !string.IsNullOrEmpty(Text)));
            }
        }

        public PrintHelper(SKBitmap image)
        {
            PrintType = PrintType.Image;
            Image = image;
        }

        public PrintHelper(string text)
        {
            PrintType = PrintType.Text;
            Text = text;
        }

        public void Dispose()
        {
            // No managed resources to dispose in cross-platform implementation
        }

        public void ShowPreview()
        {
            // Cross-platform print preview would require a custom window
            // For now, just print directly
            if (Printable)
            {
                Print();
            }
        }

        public bool Print()
        {
            if (!Printable)
                return false;

            try
            {
                if (PrintType == PrintType.Image)
                {
                    return PrintImage();
                }
                else if (PrintType == PrintType.Text)
                {
                    return PrintText();
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }

            return false;
        }

        private bool PrintImage()
        {
            // Save image to temp file and use system print command
            string tempFile = Path.Combine(Path.GetTempPath(), $"sharex_print_{Guid.NewGuid()}.png");

            try
            {
                using var stream = File.OpenWrite(tempFile);
                Image.Encode(stream, SKEncodedImageFormat.Png, 100);
                stream.Close();

                return PrintFile(tempFile);
            }
            finally
            {
                // Clean up temp file after a delay
                _ = Task.Delay(30000).ContinueWith(_ =>
                {
                    try { File.Delete(tempFile); } catch { }
                });
            }
        }

        private bool PrintText()
        {
            // Save text to temp file and use system print command
            string tempFile = Path.Combine(Path.GetTempPath(), $"sharex_print_{Guid.NewGuid()}.txt");

            try
            {
                File.WriteAllText(tempFile, Text);
                return PrintFile(tempFile);
            }
            finally
            {
                _ = Task.Delay(30000).ContinueWith(_ =>
                {
                    try { File.Delete(tempFile); } catch { }
                });
            }
        }

        private bool PrintFile(string filePath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Use lpr on Linux
                var psi = new ProcessStartInfo
                {
                    FileName = "lpr",
                    Arguments = $"\"{filePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                process?.WaitForExit(10000);
                return process?.ExitCode == 0;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // Use lpr on macOS
                var psi = new ProcessStartInfo
                {
                    FileName = "lpr",
                    Arguments = $"\"{filePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                process?.WaitForExit(10000);
                return process?.ExitCode == 0;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Use shell execute with "print" verb on Windows
                var psi = new ProcessStartInfo
                {
                    FileName = filePath,
                    Verb = "print",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                return process != null;
            }

            return false;
        }
    }

}
