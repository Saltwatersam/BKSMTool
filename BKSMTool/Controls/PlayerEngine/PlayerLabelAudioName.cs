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
    /// Represents a custom label control that displays the name of the currently playing audio track.
    /// </summary>
    public class PlayerLabelAudioName : Label
    {
        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLabelAudioName"/> class.
        /// Sets control styles for optimized drawing, binds the label to the audio player's name,
        /// and registers for player state change events.
        /// </summary>
        public PlayerLabelAudioName()
        {
            // Enable optimized double buffering and painting in the WM_PAINT event
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Default text for design mode
            Text = "Designer Test Name";

            // Prevent initialization if in design mode (to avoid issues with missing DLLs)
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

            // Get the current instance of the audio player
            _thisPlayerEngine = AudioPlayer.Instance;

            // Bind the label's text to the audio player's "AudioName" property
            DataBindings.Add(nameof(Text), _thisPlayerEngine, nameof(_thisPlayerEngine.AudioName), false, DataSourceUpdateMode.OnPropertyChanged);

            // Subscribe to the player state change event
            AudioPlayer.PlayerStateChanged += PlayerStateChange;

            // Update visibility based on the initial player state
            PlayerStateChange(this, null);
        }

        #endregion

        #region PRIVATE PROPERTIES

        /// <summary>
        /// Holds a reference to the current instance of the audio player.
        /// </summary>
        private readonly AudioPlayer _thisPlayerEngine;

        #endregion

        #region HIDDEN PROPERTIES

        /// <summary>
        /// Overrides the <see cref="Text"/> property to make it non-editable and hidden from the designer and property editor.
        /// </summary>
        [Localizable(false)]
        [Bindable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DefaultValue("Designer Test Name")]
        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        /// <summary>
        /// Overrides the <see cref="AllowDrop"/> property to make it non-editable and hidden from the designer and property editor.
        /// </summary>
        [Bindable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AllowDrop { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Handles the player state change event to control the visibility of the label based on playback state.
        /// </summary>
        /// <param name="sender">The source of the event, usually the audio player.</param>
        /// <param name="e">The event arguments (can be null).</param>
        private void PlayerStateChange(object sender, EventArgs e)
        {
            // Set the visibility of the label depending on the player's state
            switch (_thisPlayerEngine.PlayerState)
            {
                case PlaybackState.Paused:
                    Visible = true;
                    break;
                case PlaybackState.Stopped:
                    Visible = false;
                    break;
                case PlaybackState.Playing:
                    Visible = true;
                    break;
            }
        }

        #endregion
    }
}
