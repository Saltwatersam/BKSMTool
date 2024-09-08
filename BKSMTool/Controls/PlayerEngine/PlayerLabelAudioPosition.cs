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
using BKSMTool.audio_player_converter;

namespace BKSMTool.Controls.PlayerEngine
{
    /// <summary>
    /// Represents a custom label control that displays the audio position of an audio player.
    /// </summary>
    public class PlayerLabelAudioPosition : Label
    {
        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLabelAudioPosition"/> class.
        /// Sets some control styles for optimized drawing and data binding with the audio player's current position.
        /// </summary>
        public PlayerLabelAudioPosition()
        {
            // Enable optimized double buffering and painting in WM_PAINT event
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Initialize text to show the audio position as "00:00"
            Text = "00:00";

            // Prevent initialization when in design mode (avoiding potential issues with missing DLLs)
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

            // Bind the label text to the audio player's "AudioPositionAsString" property
            var thisPlayerEngine = AudioPlayer.Instance;
            DataBindings.Add(nameof(Text), thisPlayerEngine, nameof(thisPlayerEngine.AudioPositionAsString), false, DataSourceUpdateMode.OnPropertyChanged);
        }

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
        [DefaultValue("00:00")]
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
    }
}
