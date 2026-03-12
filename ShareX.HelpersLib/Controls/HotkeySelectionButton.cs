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
using Avalonia.Interactivity;
using Avalonia.Media;
using System;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// A button that allows the user to select a hotkey by pressing keys.
    /// </summary>
    public class HotkeySelectionButton : Button
    {
        public event EventHandler HotkeyChanged;

        public static readonly StyledProperty<HotkeyInfo> HotkeyInfoProperty =
            AvaloniaProperty.Register<HotkeySelectionButton, HotkeyInfo>(nameof(HotkeyInfo));

        public HotkeyInfo HotkeyInfo
        {
            get => GetValue(HotkeyInfoProperty) ?? new HotkeyInfo();
            set => SetValue(HotkeyInfoProperty, value);
        }

        public bool EditingHotkey { get; private set; }

        private IBrush originalBackground;

        public HotkeySelectionButton()
        {
            HotkeyInfo = new HotkeyInfo();
            SetDefaultButtonText();
            Focusable = true;
        }

        private void SetDefaultButtonText()
        {
            Content = "Select a hotkey...";
        }

        public void Reset()
        {
            EditingHotkey = false;
            HotkeyInfo = new HotkeyInfo();
            SetDefaultButtonText();
        }

        private void StartEditing()
        {
            EditingHotkey = true;

            originalBackground = Background;
            Background = new SolidColorBrush(Color.FromRgb(225, 255, 225));
            SetDefaultButtonText();

            HotkeyInfo = new HotkeyInfo { Hotkey = Key.None, Win = false };

            OnHotkeyChanged();
        }

        private void StopEditing()
        {
            EditingHotkey = false;

            if (HotkeyInfo.IsOnlyModifiers)
            {
                HotkeyInfo = new HotkeyInfo { Hotkey = Key.None };
            }

            Background = originalBackground;

            OnHotkeyChanged();
            UpdateHotkeyText();
        }

        public void UpdateHotkey(HotkeyInfo hotkeyInfo)
        {
            HotkeyInfo = hotkeyInfo;
            UpdateHotkeyText();
        }

        private void UpdateHotkeyText()
        {
            Content = HotkeyInfo.ToString();
        }

        protected override void OnClick()
        {
            if (EditingHotkey)
            {
                StopEditing();
            }
            else
            {
                StartEditing();
            }

            base.OnClick();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (EditingHotkey)
            {
                StopEditing();
            }

            base.OnLostFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (EditingHotkey)
            {
                e.Handled = true;

                if (e.Key == Key.Escape)
                {
                    HotkeyInfo = new HotkeyInfo { Hotkey = Key.None };
                    StopEditing();
                }
                else if (e.Key == Key.LWin || e.Key == Key.RWin)
                {
                    var newInfo = new HotkeyInfo
                    {
                        Hotkey = HotkeyInfo.Hotkey,
                        Win = !HotkeyInfo.Win,
                        Control = HotkeyInfo.Control,
                        Shift = HotkeyInfo.Shift,
                        Alt = HotkeyInfo.Alt
                    };
                    HotkeyInfo = newInfo;
                    UpdateHotkeyText();
                }
                else
                {
                    var newInfo = new HotkeyInfo
                    {
                        Hotkey = e.Key,
                        Win = HotkeyInfo.Win,
                        Control = e.KeyModifiers.HasFlag(KeyModifiers.Control),
                        Shift = e.KeyModifiers.HasFlag(KeyModifiers.Shift),
                        Alt = e.KeyModifiers.HasFlag(KeyModifiers.Alt)
                    };

                    if (newInfo.IsValidHotkey)
                    {
                        HotkeyInfo = newInfo;
                        StopEditing();
                    }
                    else
                    {
                        HotkeyInfo = newInfo;
                        UpdateHotkeyText();
                    }
                }
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (EditingHotkey)
            {
                e.Handled = true;

                // PrintScreen doesn't trigger KeyDown on some platforms
                if (e.Key == Key.PrintScreen || e.Key == Key.Snapshot)
                {
                    var newInfo = new HotkeyInfo
                    {
                        Hotkey = e.Key,
                        Win = HotkeyInfo.Win,
                        Control = e.KeyModifiers.HasFlag(KeyModifiers.Control),
                        Shift = e.KeyModifiers.HasFlag(KeyModifiers.Shift),
                        Alt = e.KeyModifiers.HasFlag(KeyModifiers.Alt)
                    };
                    HotkeyInfo = newInfo;
                    StopEditing();
                }
            }

            base.OnKeyUp(e);
        }

        protected void OnHotkeyChanged()
        {
            HotkeyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Represents a hotkey combination.
    /// </summary>
    public class HotkeyInfo
    {
        public Key Hotkey { get; set; } = Key.None;
        public bool Win { get; set; }
        public bool Control { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }

        public bool IsOnlyModifiers => Hotkey == Key.None || Hotkey == Key.LeftCtrl || Hotkey == Key.RightCtrl ||
                                        Hotkey == Key.LeftShift || Hotkey == Key.RightShift ||
                                        Hotkey == Key.LeftAlt || Hotkey == Key.RightAlt ||
                                        Hotkey == Key.LWin || Hotkey == Key.RWin;

        public bool IsValidHotkey => Hotkey != Key.None && !IsOnlyModifiers;

        public override string ToString()
        {
            if (Hotkey == Key.None && !Win && !Control && !Shift && !Alt)
                return "None";

            var parts = new System.Collections.Generic.List<string>();

            if (Win) parts.Add("Win");
            if (Control) parts.Add("Ctrl");
            if (Shift) parts.Add("Shift");
            if (Alt) parts.Add("Alt");

            if (Hotkey != Key.None && !IsOnlyModifiers)
                parts.Add(Hotkey.ToString());

            return string.Join(" + ", parts);
        }
    }
}
