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
using System.Windows.Forms;

namespace BKSMTool.Controls
{
    /// <summary>
    /// A custom <see cref="Button"/> control that is not selectable.
    /// </summary>
    /// <remarks>
    /// This class inherits from the <see cref="Button"/> control and overrides certain control styles to make the button non-selectable.
    /// </remarks>
    public class NotSelectableButton : Button
    {
        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="NotSelectableButton"/> class.
        /// </summary>
        /// <remarks>
        /// Sets specific control styles to optimize double buffering and to prevent the button from being selectable.
        /// </remarks>
        public NotSelectableButton()
        {
            // Optimize double buffering to reduce flickering during painting
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Disable the button's ability to be selectable (i.e., no focus rectangle)
            SetStyle(ControlStyles.Selectable, false);
        }

        #endregion
    }
}
