﻿// Copyright (c) 2024 Saltwatersam
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
    public partial class UpdateFormAutomatic : Form
    {
        /// <summary>
        /// Gets or sets the message displayed in the form.
        /// The message is displayed via the <see cref="messageLabel"/> control.
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
        /// Initializes a new instance of the <see cref="UpdateFormAutomatic"/> class.
        /// This constructor is responsible for initializing the form's components.
        /// </summary>
        public UpdateFormAutomatic()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for the "Don't Remind" button click.
        /// When clicked, this disables automatic update checks and sets the dialog result to <see cref="DialogResult.Cancel"/>.
        /// </summary>
        /// <param name="sender">The source of the event, typically the button being clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void DontRemindButton_Click(object sender, EventArgs e)
        {
            // Disable automatic update checks by updating the user settings
            UserConfigs.AutomaticCheckForUpdate = false;

            // Set the dialog result to cancel to indicate the user chose not to be reminded again
            DialogResult = DialogResult.Cancel;
        }
    }
}
