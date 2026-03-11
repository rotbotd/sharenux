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

using System;

namespace ShareX.HelpersLib
{
    // Stub replacements for WinForms dialogs
    // TODO: Implement with Avalonia dialogs
    
    public enum DialogResult
    {
        None,
        OK,
        Cancel,
        Abort,
        Retry,
        Ignore,
        Yes,
        No
    }

    public class OpenFileDialog : IDisposable
    {
        public string FileName { get; set; } = "";
        public string Title { get; set; } = "";
        public string Filter { get; set; } = "";
        public string InitialDirectory { get; set; } = "";
        public bool Multiselect { get; set; } = false;
        public string[] FileNames { get; set; } = Array.Empty<string>();

        public DialogResult ShowDialog()
        {
            // TODO: Implement with Avalonia file picker
            return DialogResult.Cancel;
        }

        public void Dispose() { }
    }

    public class SaveFileDialog : IDisposable
    {
        public string FileName { get; set; } = "";
        public string Title { get; set; } = "";
        public string Filter { get; set; } = "";
        public string InitialDirectory { get; set; } = "";
        public string DefaultExt { get; set; } = "";
        public bool OverwritePrompt { get; set; } = true;

        public DialogResult ShowDialog()
        {
            // TODO: Implement with Avalonia file picker
            return DialogResult.Cancel;
        }

        public void Dispose() { }
    }

    public class FolderBrowserDialog : IDisposable
    {
        public string SelectedPath { get; set; } = "";
        public string Description { get; set; } = "";

        public DialogResult ShowDialog()
        {
            // TODO: Implement with Avalonia folder picker
            return DialogResult.Cancel;
        }

        public void Dispose() { }
    }
}
