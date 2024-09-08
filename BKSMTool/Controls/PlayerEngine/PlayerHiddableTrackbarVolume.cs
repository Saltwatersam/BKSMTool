// Copyright (c) 2024 Saltwatersam
// 
// This file is part of BKSMTool.
// 
// BKSMTool is licensed under the GPLv3 License:
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System.ComponentModel;
using System.Windows.Forms;
using System;
using BKSMTool.audio_player_converter;

namespace BKSMTool.Controls.PlayerEngine
{
    /// <summary>
    /// Custom UserControl that represents a volume control with a hidden, sliding trackbar. 
    /// The volume can be adjusted by the user and is synchronized with the audio player's volume.
    /// </summary>
    public partial class PlayerHidableTrackbarVolume : UserControl
    {
        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerHidableTrackbarVolume"/> class.
        /// Sets up the sliding and timeout timers, binds the trackbar to the audio player's volume, and handles events.
        /// </summary>
        public PlayerHidableTrackbarVolume()
        {
            InitializeComponent();
            Init_Timer_Sliding_IN_OUT();
            Init_Timer_Timeout();

            // Event handlers for mouse actions and value changes on the trackbar
            VolumeTrackbar.MouseLeave += AdvancedTrackbar_MouseLeave;
            VolumeTrackbar.MouseMove += AdvancedTrackbar_MouseMove;
            VolumeTrackbar.ValueChanged += AdvancedTrackbar_ValueChanged;

            // Event handler for the button that shows/hides the volume bar
            notSelectableButtonShowVolumeBar.Click += notSelectableButtonShowVolumeBar_Click;

            // Avoid initializing if in design mode (to prevent issues with missing DLLs)
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

            // Get the current instance of the audio player
            _thisPlayerEngine = AudioPlayer.Instance;

            // Bind the volume control to the audio player's "Volume" property
            DataBindings.Add(nameof(Volume), _thisPlayerEngine, nameof(_thisPlayerEngine.Volume), false, DataSourceUpdateMode.OnPropertyChanged);

            // Subscribe to the volume changed event of the audio player
            AudioPlayer.VolumeChanged += PlayerVolumeChanged;
            PlayerVolumeChanged(this, null); // Update the initial state

            // Trackbar scroll event to update volume
            VolumeTrackbar.Scroll += AdvancedTrackbar_scroll;
            AdvancedTrackbar_scroll(this, null); // Initialize the scroll handler
            VolumeTrackbar.Visible = false; // Hide trackbar initially
            VolumeTrackbar.Width = 0; // Set initial width to 0
        }

        #endregion

        #region PUBLIC PROPERTIES

        private float _volume = 0.5f;

        /// <summary>
        /// Gets or sets the current volume value. The value is constrained between 0.0 and 1.0.
        /// </summary>
        public float Volume
        {
            get => _volume;
            set
            {
                if (Math.Abs(_volume - value) < 0.001) return;
                _volume = Math.Max(0.0f, Math.Min(1.0f, value));
                UpdateTrackbar(); // Update the trackbar value when the volume changes
            }
        }

        #endregion

        #region PRIVATE PROPERTIES

        /// <summary>
        /// Holds a reference to the current instance of the audio player.
        /// </summary>
        private readonly AudioPlayer _thisPlayerEngine;

        /// <summary>
        /// Timer to handle the sliding in/out animation of the trackbar.
        /// </summary>
        private static readonly Timer TimerSlidingInOut = new Timer();

        /// <summary>
        /// Timer to handle timeout before the volume bar hides after user inactivity.
        /// </summary>
        private static readonly Timer TimerTimeout = new Timer();

        private bool _showVolumeBar; // Indicates whether the volume bar is currently visible

        #endregion

        #region METHODS

        /// <summary>
        /// Updates the trackbar position to reflect the current volume.
        /// </summary>
        private void UpdateTrackbar()
        {
            VolumeTrackbar.Scroll -= AdvancedTrackbar_scroll;
            VolumeTrackbar.Value = (decimal)Volume;
            VolumeTrackbar.Scroll += AdvancedTrackbar_scroll;
        }

        #endregion

        #region EVENTS

        /// <summary>
        /// Event handler to update the volume button's icon based on the audio player's current volume.
        /// </summary>
        private void PlayerVolumeChanged(object sender, EventArgs e)
        {
            if (_thisPlayerEngine.Volume == 0.0f)
            {
                notSelectableButtonShowVolumeBar.BackgroundImage = Properties.Resources.Volume_Mute;
            }
            else if (_thisPlayerEngine.Volume > 0.0f && _thisPlayerEngine.Volume <= 0.25f)
            {
                notSelectableButtonShowVolumeBar.BackgroundImage = Properties.Resources.Volume_VeryLow;
            }
            else if (_thisPlayerEngine.Volume > 0.25f && _thisPlayerEngine.Volume <= 0.50f)
            {
                notSelectableButtonShowVolumeBar.BackgroundImage = Properties.Resources.Volume_Low;
            }
            else if (_thisPlayerEngine.Volume > 0.50f && _thisPlayerEngine.Volume <= 0.75f)
            {
                notSelectableButtonShowVolumeBar.BackgroundImage = Properties.Resources.Volume_Medium;
            }
            else if (_thisPlayerEngine.Volume > 0.75f && _thisPlayerEngine.Volume <= 1.0f)
            {
                notSelectableButtonShowVolumeBar.BackgroundImage = Properties.Resources.Volume_High;
            }
        }

        /// <summary>
        /// Event handler to update the audio player's volume when the trackbar is scrolled.
        /// </summary>
        private void AdvancedTrackbar_scroll(object sender, ScrollEventArgs e)
        {
            _thisPlayerEngine.Volume = Convert.ToSingle(VolumeTrackbar.Value);
        }

        /// <summary>
        /// Event handler for when the mouse leaves the trackbar. Restarts the timeout to hide the volume bar.
        /// </summary>
        private static void AdvancedTrackbar_MouseLeave(object sender, EventArgs e)
        {
            ResetTimeout();
        }

        /// <summary>
        /// Event handler for when the mouse moves over the trackbar. Restarts the timeout to keep the volume bar visible.
        /// </summary>
        private static void AdvancedTrackbar_MouseMove(object sender, EventArgs e)
        {
            ResetTimeout();
        }

        /// <summary>
        /// Event handler for when the trackbar's value changes. Restarts the timeout.
        /// </summary>
        private static void AdvancedTrackbar_ValueChanged(object sender, EventArgs e)
        {
            ResetTimeout();
        }

        /// <summary>
        /// Initializes the sliding in/out timer for the volume bar animation.
        /// </summary>
        private void Init_Timer_Sliding_IN_OUT()
        {
            TimerSlidingInOut.Tick += Timer_Sliding_IN_OUT_Tick;
            TimerSlidingInOut.Interval = 5; // Set the timer interval for smooth sliding
            TimerSlidingInOut.Enabled = false;
        }

        /// <summary>
        /// Initializes the timeout timer to hide the volume bar after a period of inactivity.
        /// </summary>
        private void Init_Timer_Timeout()
        {
            TimerTimeout.Tick += Timer_Timeout_Tick;
            TimerTimeout.Interval = 2500; // Set the timeout interval (2.5 seconds)
            TimerTimeout.Enabled = false;
        }

        /// <summary>
        /// Restarts the sliding animation timer.
        /// </summary>
        private static void RestartSlidingTimer()
        {
            TimerSlidingInOut.Stop();
            TimerSlidingInOut.Start();
        }

        /// <summary>
        /// Resets the timeout timer, delaying the hiding of the volume bar.
        /// </summary>
        private static void ResetTimeout()
        {
            TimerTimeout.Stop();
            TimerTimeout.Start();
        }

        /// <summary>
        /// Event handler for the timeout timer tick, toggles the visibility of the volume bar and restarts the sliding animation.
        /// </summary>
        private void Timer_Timeout_Tick(object sender, EventArgs e)
        {
            _showVolumeBar = !_showVolumeBar;
            RestartSlidingTimer();
        }

        /// <summary>
        /// Event handler for the sliding animation timer tick. Handles the smooth sliding in/out of the volume bar.
        /// </summary>
        private void Timer_Sliding_IN_OUT_Tick(object sender, EventArgs e)
        {
            if (_showVolumeBar)
            {
                // Slide the volume bar in
                if (VolumeTrackbar.Width >= 103)
                {
                    VolumeTrackbar.Visible = true;
                    TimerSlidingInOut.Stop();
                    ResetTimeout();
                    return;
                }
                VolumeTrackbar.Width += 15; // Increase width to slide in
            }
            else
            {
                // Slide the volume bar out
                if (VolumeTrackbar.Width <= 0)
                {
                    VolumeTrackbar.Visible = false;
                    TimerTimeout.Stop();
                    TimerSlidingInOut.Stop();
                    return;
                }
                VolumeTrackbar.Width -= 20; // Decrease width to slide out
            }
        }

        /// <summary>
        /// Event handler for the button click that toggles the visibility of the volume bar.
        /// </summary>
        private void notSelectableButtonShowVolumeBar_Click(object sender, EventArgs e)
        {
            _showVolumeBar = !_showVolumeBar;
            RestartSlidingTimer(); // Restart the sliding animation
        }

        #endregion
    }
}
