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

namespace BKSMTool.Controls.ModalForm
{
    /// <summary>
    /// Form for displaying a message when no updates are available.
    /// </summary>
    public partial class NoUpdateFormManual : Form
    {
        /// <summary>
        /// Gets or sets the message displayed in the form.
        /// The message is displayed through the <see cref="messageLabel"/> control.
        /// </summary>
        public string Message
        {
            get => messageLabel.Text;
            set => messageLabel.Text = value;
        }

        /// <summary>
        /// Gets or sets the title of the form window.
        /// The title is displayed through the <see cref="Form.Text"/> property of the form.
        /// </summary>
        public string Title
        {
            get => Text;
            set => Text = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoUpdateFormManual"/> class.
        /// Sets the button text based on the current status of automatic update checks.
        /// </summary>
        public NoUpdateFormManual()
        {
            InitializeComponent();
            // Set the button text depending on whether automatic update checks are enabled or disabled
            EnableDisableAutomaticUpdateCheck.Text = Properties.Settings.Default.AutomaticCheckForUpdate
                ? @"Disable automatic update checks"
                : @"Enable automatic update checks";
        }

        /// <summary>
        /// Event handler for enabling or disabling automatic update checks.
        /// When the button is clicked, the status of automatic update checks is toggled.
        /// The button text updates accordingly to reflect the new status.
        /// </summary>
        /// <param name="sender">The source of the event, typically the button being clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void EnableDisableAutoUpdateButton_Click(object sender, EventArgs e)
        {
            // Toggle the setting for automatic update checks
            Properties.Settings.Default.AutomaticCheckForUpdate = !Properties.Settings.Default.AutomaticCheckForUpdate;

            // Update the button text based on the new state
            EnableDisableAutomaticUpdateCheck.Text = Properties.Settings.Default.AutomaticCheckForUpdate
                ? @"Disable automatic update checks"
                : @"Enable automatic update checks";
        }
    }
}
