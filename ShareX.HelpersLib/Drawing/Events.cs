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
using SkiaSharp;
using Avalonia.Input;

namespace ShareX.HelpersLib
{
    [Flags]
    public enum MouseButtons
    {
        None = 0,
        Left = 1,
        Right = 2,
        Middle = 4,
        XButton1 = 8,
        XButton2 = 16
    }

    public class MouseEventArgs : EventArgs
    {
        public MouseButtons Button { get; }
        public int Clicks { get; }
        public int X { get; }
        public int Y { get; }
        public int Delta { get; }
        public SKPointI Location => new SKPointI(X, Y);

        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            Button = button;
            Clicks = clicks;
            X = x;
            Y = y;
            Delta = delta;
        }

        public static MouseEventArgs FromAvalonia(PointerEventArgs e, Avalonia.Visual? relativeTo = null)
        {
            var pos = relativeTo != null ? e.GetPosition(relativeTo) : e.GetPosition(null);
            var props = e.GetCurrentPoint(relativeTo).Properties;
            
            MouseButtons button = MouseButtons.None;
            if (props.IsLeftButtonPressed) button |= MouseButtons.Left;
            if (props.IsRightButtonPressed) button |= MouseButtons.Right;
            if (props.IsMiddleButtonPressed) button |= MouseButtons.Middle;
            
            return new MouseEventArgs(button, 1, (int)pos.X, (int)pos.Y, 0);
        }
    }

    public class PaintEventArgs : EventArgs, IDisposable
    {
        public Graphics Graphics { get; }
        public SKRectI ClipRectangle { get; }

        public PaintEventArgs(Graphics graphics, SKRectI clipRect)
        {
            Graphics = graphics;
            ClipRectangle = clipRect;
        }

        public void Dispose()
        {
            Graphics.Dispose();
        }
    }

    public class KeyEventArgs : EventArgs
    {
        public Key KeyCode { get; }
        public bool Handled { get; set; }
        public bool Alt { get; }
        public bool Control { get; }
        public bool Shift { get; }

        public KeyEventArgs(Key keyCode, bool alt = false, bool control = false, bool shift = false)
        {
            KeyCode = keyCode;
            Alt = alt;
            Control = control;
            Shift = shift;
        }

        public static KeyEventArgs FromAvalonia(KeyEventArgs e)
        {
            return new KeyEventArgs(
                e.Key,
                e.KeyModifiers.HasFlag(KeyModifiers.Alt),
                e.KeyModifiers.HasFlag(KeyModifiers.Control),
                e.KeyModifiers.HasFlag(KeyModifiers.Shift));
        }
    }

    public class DragEventArgs : EventArgs
    {
        public object? Data { get; }
        public int X { get; }
        public int Y { get; }
        public DragDropEffects Effect { get; set; }
        public DragDropEffects AllowedEffect { get; }

        public DragEventArgs(object? data, int x, int y, DragDropEffects allowedEffect)
        {
            Data = data;
            X = x;
            Y = y;
            AllowedEffect = allowedEffect;
            Effect = DragDropEffects.None;
        }
    }
}
