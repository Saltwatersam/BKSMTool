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
using System.Reflection;
using System.Windows.Forms;

namespace BKSMTool.Controls.ModalForm
{
    /// <summary>
    /// Form for displaying a message when no updates are available.
    /// </summary>
    public partial class AboutForm : Form
    {
        private const string ApplicationUrl = "https://saltwatersam.github.io/BKSMTool/";

        /// <summary>
        /// Initializes a new instance of the <see cref="NoUpdateFormManual"/> class.
        /// Sets the button text based on the current status of automatic update checks.
        /// </summary>
        public AboutForm()
        {
            InitializeComponent();
            VersionLabel.Text = $@"BKSMTool version: ""{Assembly.GetExecutingAssembly().GetName().Version}"",  by Saltwatersam";
        }

        private void LinkLbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(ApplicationUrl);
        }
    }
}
