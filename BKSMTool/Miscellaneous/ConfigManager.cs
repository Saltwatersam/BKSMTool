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
using System.IO;
using BKSMTool.audio_player_converter;

namespace BKSMTool.Miscellaneous
{
    /// <summary>
    /// Manages the configuration file for the application. This class is responsible for
    /// saving and loading the application's configuration settings, specifically related
    /// to the audio player's mode and volume.
    /// </summary>
    public static class ConfigManager
    {
        #region Methods

        #region Get Configuration File Path

        /// <summary>
        /// Gets the full path to the configuration file. The configuration file is stored in
        /// the AppData directory under a folder named "BKSMTool".
        /// </summary>
        /// <returns>The full path to the configuration file.</returns>
        private static string GetConfigFilePath()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataFolder, "BKSMTool");

            // Ensure the directory exists, create if it doesn't
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            return Path.Combine(appFolder, "BKSMTool.cfg");
        }

        #endregion

        #region Save Configuration File

        /// <summary>
        /// Saves the player's configuration settings (mode and volume) to a configuration file.
        /// </summary>
        /// <param name="mode">The current player mode (e.g., Loop, NoLoop).</param>
        /// <param name="volume">The current volume level.</param>
        public static void SaveConfig(PlayerMode mode, float volume)
        {
            var filePath = GetConfigFilePath();

            // Write configuration settings to the file
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"PlayerMode = {mode}");
                writer.WriteLine($"Volume = {volume}");
            }
        }

        #endregion

        #region Load Configuration File

        /// <summary>
        /// Loads the player's configuration settings from the configuration file.
        /// If the file doesn't exist, it returns default values.
        /// </summary>
        /// <returns>A tuple containing the player mode and volume level.</returns>
        public static (PlayerMode mode, float volume) LoadConfig()
        {
            var filePath = GetConfigFilePath();
            var mode = PlayerMode.NoLoop; // Default player mode
            var volume = 0.5f; // Default volume level

            // If configuration file exists, read and parse its content
            if (!File.Exists(filePath)) return (mode, volume);
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (!line.Contains(" = ")) continue;

                var parts = line.Split(new[] { " = " }, StringSplitOptions.None);
                if (parts[0] == "PlayerMode" && Enum.TryParse(parts[1], out PlayerMode parsedMode))
                {
                    mode = parsedMode;
                }
                if (parts[0] == "Volume" && float.TryParse(parts[1], out var parsedVolume))
                {
                    volume = parsedVolume;
                }
            }

            return (mode, volume);
        }

        #endregion

        #endregion
    }
}
