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
using BrightIdeasSoftware;
using System.Drawing;
using System.Windows.Forms;

namespace BKSMTool.Controls.PlayerEngine
{
    /// <summary>
    /// Custom renderer that visually represents whether an audio state has been modified by rendering
    /// a '*' symbol if the state has been changed.
    /// Inherits from <see cref="BaseRenderer"/> to draw custom content in a cell.
    /// </summary>
    public class AudioSavedStateRenderer : BaseRenderer
    {
        /// <summary>
        /// Renders the cell content to indicate if the audio state has been modified.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object used to perform the rendering.</param>
        /// <param name="r">The <see cref="Rectangle"/> that represents the area of the cell to be rendered.</param>
        public override void Render(Graphics g, Rectangle r)
        {
            // Clear the cell area to avoid drawing issues by drawing and filling a rectangle
            g.DrawRectangle(new Pen(base.GetBackgroundColor()), r); // Draw the cell border
            g.FillRectangle(new SolidBrush(base.GetBackgroundColor()), r); // Fill the cell background

            // Get the value of the cell, which indicates whether the audio state is modified (true/false)
            var isModified = (bool)Aspect;

            // String format setup for drawing the text
            var fmt = new StringFormat(StringFormatFlags.NoWrap)
            {
                LineAlignment = StringAlignment.Center, // Align text vertically to the center
                Trimming = StringTrimming.EllipsisCharacter // Trim long text with ellipsis
            };

            // Align the text horizontally based on the column's alignment setting
            switch (this.Column.TextAlign)
            {
                case HorizontalAlignment.Center:
                    fmt.Alignment = StringAlignment.Center;
                    break;
                case HorizontalAlignment.Left:
                    fmt.Alignment = StringAlignment.Near;
                    break;
                case HorizontalAlignment.Right:
                    fmt.Alignment = StringAlignment.Far;
                    break;
            }

            // Draw a '*' symbol if the state is modified, otherwise draw an empty string
            g.DrawString(isModified ? "*" : "", this.Font, this.TextBrush, r, fmt);
        }
    }
}
