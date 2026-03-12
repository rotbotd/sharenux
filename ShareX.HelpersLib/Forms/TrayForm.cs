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

using Avalonia.Controls;
using System;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// Base window for tray icon functionality.
    /// In Avalonia, tray icons are handled via TrayIcon class in desktop extensions.
    /// </summary>
    public class TrayForm : Window
    {
        protected TrayIcon TrayIcon { get; set; }

        public TrayForm()
        {
            // Tray icon setup would be done via Avalonia.Desktop's TrayIcon
            // This requires the application to use Avalonia.Desktop lifetime
            ShowInTaskbar = false;
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            // Start hidden - show via tray icon
            Hide();
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            // Minimize to tray instead of closing
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }
    }

    /// <summary>
    /// Wrapper for Avalonia's TrayIcon functionality.
    /// Actual implementation requires Avalonia.Desktop package.
    /// </summary>
    public class TrayIcon : IDisposable
    {
        public string Text { get; set; } = "ShareX";
        public bool Visible { get; set; }
        public NativeMenu Menu { get; set; }

        public event EventHandler Click;
        public event EventHandler DoubleClick;

        public TrayIcon()
        {
        }

        public void ShowBalloonTip(int timeout, string title, string text, ToolTipIcon icon)
        {
            // Platform-specific notification - would use libnotify on Linux
            // For now, this is a placeholder
        }

        public void Dispose()
        {
        }
    }

    public enum ToolTipIcon
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }
}
