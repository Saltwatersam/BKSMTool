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

using System;
using BrightIdeasSoftware;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BKSMTool.audio_player_converter;

namespace BKSMTool.Controls.PlayerEngine
{
    /// <summary>
    /// A custom renderer that displays the state of a sound player (Play, Pause, Stop) as a button inside a cell.
    /// </summary>
    /// <remarks>
    /// This renderer allows customizing the alignment and size of the button and the images representing different sound states.
    /// </remarks>
    public class SoundPlayerStateRenderer : BaseRenderer
    {
        #region Properties

        /// <summary>
        /// Defines the alignment options for the button and image within the cell.
        /// </summary>
        public enum Alignment
        {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            Center,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }

        #region Button Properties

        /// <summary>
        /// Gets or sets the alignment of the button within the cell.
        /// </summary>
        [Browsable(true),
         Category("Button"),
         Description("Button alignment within the cell."),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always),
         DefaultValue(Alignment.Center)]
        public Alignment ButtonAlignment { get; set; } = Alignment.Center;

        /// <summary>
        /// Gets or sets the size of the button. If set to -1, the button will be bound to the cell size.
        /// </summary>
        [Browsable(true),
         Category("Button"),
         Description("Size of the button. When set to -1, the button is bound to the cell size."),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always)]
        public Size ButtonSize
        {
            get => _buttonSize;
            set
            {
                if (MaximumButtonSize.Width != -1 && value.Width > MaximumButtonSize.Width)
                {
                    _buttonSize.Width = MaximumButtonSize.Width;
                }
                else if (MinimumButtonSize.Width != -1 && value.Width < MinimumButtonSize.Width)
                {
                    _buttonSize.Width = MinimumButtonSize.Width;
                }
                else
                {
                    _buttonSize.Width = value.Width;
                }
                if (MaximumButtonSize.Height != -1 && value.Height > MaximumButtonSize.Height)
                {
                    _buttonSize.Height = MaximumButtonSize.Height;
                }
                else if (MinimumButtonSize.Height != -1 && value.Height < MinimumButtonSize.Height)
                {
                    _buttonSize.Height = MinimumButtonSize.Height;
                }
                else
                {
                    _buttonSize.Height = value.Height;
                }
            }
        }
        private Size _buttonSize = new Size(-1, -1);

        /// <summary>
        /// Gets or sets the maximum size of the button. Set to -1 to remove the maximum size restriction.
        /// </summary>
        [Browsable(true),
         Category("Button"),
         Description("Maximum size of the button. Set to -1 to remove the size limit."),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always)]
        public Size MaximumButtonSize { get; set; } = new Size(-1, -1);

        /// <summary>
        /// Gets or sets the minimum size of the button. Set to -1 to remove the minimum size restriction.
        /// </summary>
        [Browsable(true),
         Category("Button"),
         Description("Minimum size of the button. Set to -1 to remove the size limit."),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always)]
        public Size MinimumButtonSize { get; set; } = new Size(-1, -1);

        #endregion

        #region Button Images Properties

        /// <summary>
        /// Gets or sets the alignment of the image within the button.
        /// </summary>
        [Browsable(true),
         Category("Button Images"),
         Description("Alignment of the image within the button."),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always),
         DefaultValue(Alignment.Center)]
        public Alignment ImageAlignment { get; set; } = Alignment.Center;

        /// <summary>
        /// Gets or sets the size of the image. If set to -1, the image will be bound to the button size.
        /// </summary>
        [Browsable(true),
         Category("Button Images"),
         Description("Size of the image. When set to -1, the image is bound to the button size."),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always)]
        public Size ImageSize
        {
            get => _imageSize;
            set
            {
                if (MaximumImageSize.Width != -1 && value.Width > MaximumImageSize.Width)
                {
                    _imageSize.Width = MaximumImageSize.Width;
                }
                else if (MinimumImageSize.Width != -1 && value.Width < MinimumImageSize.Width)
                {
                    _imageSize.Width = MinimumImageSize.Width;
                }
                else
                {
                    _imageSize.Width = value.Width;
                }
                if (MaximumImageSize.Height != -1 && value.Height > MaximumImageSize.Height)
                {
                    _imageSize.Height = MaximumImageSize.Height;
                }
                else if (MinimumImageSize.Height != -1 && value.Height < MinimumImageSize.Height)
                {
                    _imageSize.Height = MinimumImageSize.Height;
                }
                else
                {
                    _imageSize.Height = value.Height;
                }
            }
        }
        private Size _imageSize = new Size(-1, -1);

        /// <summary>
        /// Gets or sets the maximum size of the image. Set to -1 to remove the size restriction.
        /// </summary>
        [Browsable(true),
         Category("Button Images"),
         Description("Maximum size of the image. Set to -1 to remove the size limit."),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always)]
        public Size MaximumImageSize { get; set; } = new Size(-1, -1);

        /// <summary>
        /// Gets or sets the minimum size of the image. Set to -1 to remove the size restriction.
        /// </summary>
        [Browsable(true),
         Category("Button Images"),
         Description("Minimum size of the image. Set to -1 to remove the size limit."),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always)]
        public Size MinimumImageSize { get; set; } = new Size(-1, -1);

        /// <summary>
        /// Gets or sets the image displayed when the sound is in the Play state.
        /// </summary>
        [Browsable(true),
         Category("Button Images"),
         Description("Image used when sound is in Play state."),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always)]
        public Image PlayImage { get; set; } = null;

        /// <summary>
        /// Gets or sets the image displayed when the sound is in the Pause state.
        /// </summary>
        [Browsable(true),
         Category("Button Images"),
         Description("Image used when sound is in Pause state."),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always)]
        public Image PauseImage { get; set; } = null;

        /// <summary>
        /// Gets or sets the image displayed when the sound is in the Stop state.
        /// </summary>
        [Browsable(true),
         Category("Button Images"),
         Description("Image used when sound is in Stop state."),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always)]
        public Image StopImage { get; set; } = null;

        #endregion

        /// <summary>
        /// Indicates whether the mouse is over the row.
        /// </summary>
        protected bool IsRowHot => this.ListView != null && this.ListView.HotRowIndex == this.ListItem.Index;

        /// <summary>
        /// Gets the audio player instance associated with this renderer.
        /// </summary>
        public AudioPlayer ThisPlayerEngine { get; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundPlayerStateRenderer"/> class.
        /// </summary>
        public SoundPlayerStateRenderer()
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                ThisPlayerEngine = AudioPlayer.Instance;
            }
        }

        /// <summary>
        /// Generates the rectangle for the button based on the cell size and button alignment.
        /// </summary>
        /// <param name="cell">The cell rectangle.</param>
        /// <returns>The rectangle for the button.</returns>
        private Rectangle GenerateButtonRectangle(Rectangle cell)
        {
            var cellCenter = new Point(cell.X + (cell.Width / 2), cell.Y + (cell.Height / 2));
            var thisButton = new Rectangle(0, 0, 0, 0)
            {
                Width = ButtonSize.Width == -1 ? cell.Width : ButtonSize.Width,
                Height = ButtonSize.Height == -1 ? cell.Height : ButtonSize.Height
            };

            switch (ButtonAlignment)
            {
                case Alignment.TopLeft:
                    thisButton.X = cell.X;
                    thisButton.Y = cell.Y;
                    break;
                case Alignment.TopCenter:
                    thisButton.X = (cellCenter.X - (thisButton.Width / 2));
                    thisButton.Y = cell.Y;
                    break;
                case Alignment.TopRight:
                    thisButton.X = (cell.X + cell.Width) - thisButton.Width;
                    thisButton.Y = cell.Y;
                    break;
                case Alignment.MiddleLeft:
                    thisButton.X = cell.X;
                    thisButton.Y = cellCenter.Y - (thisButton.Height / 2);
                    break;
                case Alignment.Center:
                    thisButton.X = cellCenter.X - (thisButton.Width / 2);
                    thisButton.Y = cellCenter.Y - (thisButton.Height / 2);
                    break;
                case Alignment.MiddleRight:
                    thisButton.X = (cell.X + cell.Width) - thisButton.Width;
                    thisButton.Y = cellCenter.Y - (thisButton.Height / 2);
                    break;
                case Alignment.BottomLeft:
                    thisButton.X = cell.X;
                    thisButton.Y = (cell.Y + cell.Height) - thisButton.Height;
                    break;
                case Alignment.BottomCenter:
                    thisButton.X = cellCenter.X - (thisButton.Width / 2);
                    thisButton.Y = (cell.Y + cell.Height) - thisButton.Height;
                    break;
                case Alignment.BottomRight:
                    thisButton.X = (cell.X + cell.Width) - thisButton.Width;
                    thisButton.Y = (cell.Y + cell.Height) - thisButton.Height;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return thisButton;
        }

        /// <summary>
        /// Generates the rectangle for the image within the button.
        /// </summary>
        /// <param name="button">The button rectangle.</param>
        /// <returns>The rectangle for the image.</returns>
        private Rectangle GenerateImageRectangle(Rectangle button)
        {
            var cellCenter = new Point(button.X + (button.Width / 2), button.Y + (button.Height / 2));
            var thisImage = new Rectangle(0, 0, 0, 0)
            {
                Width = ImageSize.Width == -1 ? button.Width : ImageSize.Width,
                Height = ImageSize.Height == -1 ? button.Height : ImageSize.Height
            };

            switch (ImageAlignment)
            {
                case Alignment.TopLeft:
                    thisImage.X = button.X;
                    thisImage.Y = button.Y;
                    break;
                case Alignment.TopCenter:
                    thisImage.X = (cellCenter.X - (thisImage.Width / 2));
                    thisImage.Y = button.Y;
                    break;
                case Alignment.TopRight:
                    thisImage.X = (button.X + button.Width) - thisImage.Width;
                    thisImage.Y = button.Y;
                    break;
                case Alignment.MiddleLeft:
                    thisImage.X = button.X;
                    thisImage.Y = cellCenter.Y - (thisImage.Height / 2);
                    break;
                case Alignment.Center:
                    thisImage.X = cellCenter.X - (thisImage.Width / 2);
                    thisImage.Y = cellCenter.Y - (thisImage.Height / 2);
                    break;
                case Alignment.MiddleRight:
                    thisImage.X = (button.X + button.Width) - thisImage.Width;
                    thisImage.Y = cellCenter.Y - (thisImage.Height / 2);
                    break;
                case Alignment.BottomLeft:
                    thisImage.X = button.X;
                    thisImage.Y = (button.Y + button.Height) - thisImage.Height;
                    break;
                case Alignment.BottomCenter:
                    thisImage.X = cellCenter.X - (thisImage.Width / 2);
                    thisImage.Y = (button.Y + button.Height) - thisImage.Height;
                    break;
                case Alignment.BottomRight:
                    thisImage.X = (button.X + button.Width) - thisImage.Width;
                    thisImage.Y = (button.Y + button.Height) - thisImage.Height;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return thisImage;
        }

        /// <summary>
        /// Determines the image to display based on the audio playback state.
        /// </summary>
        /// <returns>The image corresponding to the current audio state.</returns>
        private Image ImageToDisplay()
        {
            switch (((IAudioFile)RowObject).AudioState)
            {
                case PlaybackState.Playing:
                    return PlayImage;
                case PlaybackState.Paused:
                    return PauseImage;
                case PlaybackState.Stopped:
                    return StopImage;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Draws the button and its image inside the cell.
        /// </summary>
        /// <param name="g">The graphics object.</param>
        /// <param name="cell">The cell rectangle.</param>
        protected override void DrawImageAndText(Graphics g, Rectangle cell)
        {
            if (!IsRowHot && ((IAudioFile)RowObject).AudioState == PlaybackState.Stopped)
            {
                return;
            }

            var buttonRectangle = GenerateButtonRectangle(cell);
            var imageRectangle = GenerateImageRectangle(buttonRectangle);
            var image = ImageToDisplay();

            if (image == null)
            {
                ButtonRenderer.DrawButton(g, buttonRectangle, CalculatePushButtonState());
            }
            else
            {
                ButtonRenderer.DrawButton(g, buttonRectangle, image, imageRectangle, false, CalculatePushButtonState());
            }
        }

        /// <summary>
        /// Determines what part of the control is under the given point.
        /// </summary>
        /// <param name="g">The graphics object.</param>
        /// <param name="hti">The hit test information.</param>
        /// <param name="bounds">The cell bounds.</param>
        /// <param name="x">The X coordinate of the point.</param>
        /// <param name="y">The Y coordinate of the point.</param>
        protected override void StandardHitTest(Graphics g, OlvListViewHitTestInfo hti, Rectangle bounds, int x, int y)
        {
            var r = ApplyCellPadding(bounds);
            if (r.Contains(x, y))
                hti.HitTestLocation = HitTestLocation.Button;
        }

        /// <summary>
        /// Determines the state of the button (e.g., normal, hot, pressed, disabled).
        /// </summary>
        /// <returns>The state of the button.</returns>
        protected PushButtonState CalculatePushButtonState()
        {
            if (!this.ListItem.Enabled && !this.Column.EnableButtonWhenItemIsDisabled)
                return PushButtonState.Disabled;

            return this.IsButtonHot && ObjectListView.IsLeftMouseDown ? PushButtonState.Pressed : PushButtonState.Hot;
        }

        /// <summary>
        /// Determines whether the mouse is over the button.
        /// </summary>
        protected bool IsButtonHot => this.IsCellHot && this.ListView.HotCellHitLocation == HitTestLocation.Button;
    }
}