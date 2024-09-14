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
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BKSMTool.Controls;
using BKSMTool.Controls.ModalForm;

namespace BKSMTool.Miscellaneous
{
    /// <summary>
    /// The VersionChecker class is responsible for checking if a new version of the application is available
    /// by comparing the current version of the assembly with the version stored in a Version.txt file hosted
    /// on GitHub Pages.
    /// 
    /// It retrieves the latest version information from the GitHub URL and compares it with the version of the
    /// currently running application. If a newer version exists, the user can be notified.
    /// </summary>
    public class VersionChecker
    {
        // URL pointing to the Version.txt file hosted on GitHub Pages
        private const string VersionUrl = "https://saltwatersam.github.io/BKSMTool/BKSMTool/Version.txt";

        // URL pointing to the GitHub releases page for downloading the latest version
        private const string DownloadPageUrl = "https://saltwatersam.github.io/BKSMTool/";

        // Stores the latest version of the application retrieved from GitHub
        private static Version _latestVersion;

        // Stores the current version of the application
        private static Version _currentVersion;

        /// <summary>
        /// Automatically checks for an available update when the application starts.
        /// This method will not display error messages in case of connection failures.
        /// If an update is available, it offers the user the option to visit the download page.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        public static async Task AutomaticCheckAvailableUpdate()
        {
            try
            {
                // Check if an update is available
                var isUpdateAvailable = await IsUpdateAvailable();
                if (isUpdateAvailable)
                {
                    using (var form = new UpdateFormAutomatic())
                    {
                        // Create a message to inform the user about the new version
                        form.Message = new StringBuilder()
                            .Append($"A new version ({_latestVersion}) of the application is available!")
                            .Append("\n")
                            .Append("Do you want to download it?")
                            .ToString();
                        form.Title = $"{Application.ProductName} {_currentVersion}";

                        var result = form.ShowDialog();
                        switch (result)
                        {
                            case DialogResult.Yes:
                                // Open the download page in the user's default web browser
                                Logger.Log("User opted to download the latest version.");
                                System.Diagnostics.Process.Start(DownloadPageUrl);
                                break;
                            case DialogResult.No:
                                Logger.Log("User chose not to download the latest version.");
                                break;
                            case DialogResult.None:
                                Logger.Log("User opted to be reminded later.");
                                break;
                            default:
                                Logger.Log("User closed the dialog.");
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Logger.Log("Unable to check for updates. Please check your internet connection.");
                // Suppress the exception as this is a silent automatic check
            }
        }

        /// <summary>
        /// Checks for an available update when the user explicitly requests it.
        /// Displays error messages in case of connection failures or other issues.
        /// If an update is available, it offers the user the option to visit the download page.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        public static async Task CheckAvailableUpdate()
        {
            try
            {
                // Check if an update is available
                var isUpdateAvailable = await IsUpdateAvailable();
                if (isUpdateAvailable)
                {
                    using (var form = new UpdateFormManual())
                    {
                        // Inform the user that a new version is available
                        form.Message = new StringBuilder()
                            .Append($"A new version ({_latestVersion}) of the application is available!")
                            .Append("\n")
                            .Append("Do you want to download it?")
                            .ToString();
                        form.Title = $"{Application.ProductName} {_currentVersion}";

                        var result = form.ShowDialog();
                        switch (result)
                        {
                            case DialogResult.Yes:
                                // Open the download page in the user's default web browser
                                Logger.Log("User opted to download the latest version.");
                                System.Diagnostics.Process.Start(DownloadPageUrl);
                                break;
                            case DialogResult.No:
                                Logger.Log("User chose not to download the latest version.");
                                break;
                            default:
                                Logger.Log("User closed the dialog.");
                                break;
                        }
                    }
                }
                else
                {
                    // Notify the user that they already have the latest version
                    using (var form = new NoUpdateFormManual())
                    {
                        form.Message = new StringBuilder()
                            .Append("You already have the latest version.")
                            .ToString();
                        form.Title = $"{Application.ProductName} {_currentVersion}";

                        var result = form.ShowDialog();
                        switch (result)
                        {
                            case DialogResult.Yes:
                                // Open the download page
                                Logger.Log("User opted to visit the download page.");
                                System.Diagnostics.Process.Start(DownloadPageUrl);
                                break;
                            case DialogResult.No:
                                Logger.Log("User chose not to visit the download page.");
                                break;
                            default:
                                Logger.Log("User closed the dialog.");
                                break;
                        }
                    }
                }
            }
            catch (HttpRequestException)
            {
                // Show an error message in case of an HTTP request failure (e.g., no internet)
                MessageBox.Show(@"Unable to check for updates. Please check your internet connection.",
                                @"Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Show a more generic error message if something else goes wrong
                MessageBox.Show($@"An error occurred while checking for updates: {ex.Message}",
                                @"Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Compares the current version of the application with the version available on GitHub.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if a newer version is available, <c>false</c> otherwise.
        /// </returns>
        private static async Task<bool> IsUpdateAvailable()
        {
            try
            {
                // Retrieve the current version of the application from the assembly
                _currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                Logger.Log($"Current version: {_currentVersion}");

                // Fetch the latest version from the GitHub-hosted Version.txt file
                _latestVersion = await GetLatestVersionFromGithub();
                if (_latestVersion == null)
                {
                    Logger.Log("Unable to retrieve the latest version from GitHub.");
                    return false;
                }

                Logger.Log($"Latest version on GitHub: {_latestVersion}");

                // Compare the current version with the latest version
                return _latestVersion > _currentVersion;
            }
            catch (HttpRequestException)
            {
                // Handle errors related to the HTTP request (e.g., network issues)
                throw;
            }
            catch (Exception ex)
            {
                // Handle unexpected errors during the update check
                Logger.Log($"Error occurred while checking for updates: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the latest version from the Version.txt file on GitHub.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="Version"/> object representing the latest version, or <c>null</c> if an error occurred.
        /// </returns>
        private static async Task<Version> GetLatestVersionFromGithub()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Send an HTTP request to fetch the Version.txt file from GitHub
                    var response = await client.GetAsync(VersionUrl);
                    response.EnsureSuccessStatusCode();

                    // Read the version string from the response
                    var version = await response.Content.ReadAsStringAsync();
                    return new Version(version);
                }
                catch (HttpRequestException httpEx)
                {
                    // Log specific HTTP request errors
                    Logger.Log($"HTTP Error while fetching version: {httpEx.Message}");
                    return null;
                }
            }
        }
    }
}
