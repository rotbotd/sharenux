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
using Avalonia.Media;
using Avalonia.Threading;
using SkiaSharp;
using System;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// A button that animates its foreground color between two colors.
    /// </summary>
    public class ToolStripButtonColorAnimation : Button, IDisposable
    {
        public static readonly StyledProperty<SKColor> FromColorProperty =
            AvaloniaProperty.Register<ToolStripButtonColorAnimation, SKColor>(nameof(FromColor), SKColors.Black);

        public static readonly StyledProperty<SKColor> ToColorProperty =
            AvaloniaProperty.Register<ToolStripButtonColorAnimation, SKColor>(nameof(ToColor), SKColors.Red);

        public static readonly StyledProperty<float> AnimationSpeedProperty =
            AvaloniaProperty.Register<ToolStripButtonColorAnimation, float>(nameof(AnimationSpeed), 1f);

        public SKColor FromColor
        {
            get => GetValue(FromColorProperty);
            set => SetValue(FromColorProperty, value);
        }

        public SKColor ToColor
        {
            get => GetValue(ToColorProperty);
            set => SetValue(ToColorProperty, value);
        }

        public float AnimationSpeed
        {
            get => GetValue(AnimationSpeedProperty);
            set => SetValue(AnimationSpeedProperty, value);
        }

        private DispatcherTimer timer;
        private float progress;
        private float direction = 1;
        private float speed;

        public ToolStripButtonColorAnimation()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += Timer_Tick;
        }

        public void StartAnimation()
        {
            speed = AnimationSpeed / (1000f / 100f);
            timer.Start();
        }

        public void StopAnimation()
        {
            timer.Stop();
        }

        public void ResetAnimation()
        {
            StopAnimation();
            Foreground = new SolidColorBrush(FromColor.ToAvaloniaColor());
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            progress += direction * speed;

            if (progress < 0)
            {
                progress = 0;
                direction = -direction;
            }
            else if (progress > 1)
            {
                progress = 1;
                direction = -direction;
            }

            var lerpedColor = ColorHelpers.Lerp(FromColor, ToColor, progress);
            Foreground = new SolidColorBrush(lerpedColor.ToAvaloniaColor());
        }

        public void Dispose()
        {
            timer?.Stop();
        }
    }
}
