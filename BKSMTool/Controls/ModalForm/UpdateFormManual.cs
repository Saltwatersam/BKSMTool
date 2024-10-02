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
using System.Windows.Forms;
using BKSMTool.Miscellaneous;

namespace BKSMTool.Controls.ModalForm
{
    /// <summary>
    /// Form for displaying a message when an update is available.
    /// </summary>
    public partial class UpdateFormManual : Form
    {
        /// <summary>
        /// Gets or sets the message displayed in the form.
        /// The message is shown via the <see cref="messageLabel"/> control.
        /// </summary>
        public string Message
        {
            get => messageLabel.Text;
            set => messageLabel.Text = value;
        }

        /// <summary>
        /// Gets or sets the title of the form window.
        /// The title is set via the <see cref="Form.Text"/> property of the form.
        /// </summary>
        public string Title
        {
            get => Text;
            set => Text = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFormManual"/> class.
        /// This constructor is responsible for initializing the form's components
        /// and setting the button text based on the current state of automatic update checks.
        /// </summary>
        public UpdateFormManual()
        {
            InitializeComponent();

            // Set the button text based on the status of automatic update checks
            EnableDisableAutomaticUpdateCheck.Text = UserConfigs.AutomaticCheckForUpdate
                ? @"Disable automatic update checks"
                : @"Enable automatic update checks";
        }

        /// <summary>
        /// Event handler for enabling or disabling automatic update checks.
        /// When the button is clicked, it toggles the setting and updates the button text accordingly.
        /// </summary>
        /// <param name="sender">The source of the event, typically the button being clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void EnableDisableAutoUpdateButton_Click(object sender, EventArgs e)
        {
            // Toggle the automatic update check setting
            UserConfigs.AutomaticCheckForUpdate = !UserConfigs.AutomaticCheckForUpdate;

            // Update the button text based on the new state
            EnableDisableAutomaticUpdateCheck.Text = UserConfigs.AutomaticCheckForUpdate
                ? @"Disable automatic update checks"
                : @"Enable automatic update checks";
        }
    }
}
