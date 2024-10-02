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

using BKSMTool.audio_player_converter;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BKSMTool.Miscellaneous
{
    //==========================================================================//
    /// <summary>
    /// Manages the user configuration file for the application. 
    /// </summary>
    //==========================================================================//
    public static class UserConfigs
    {
        #region PROPERTIES

        //==========================================================================//
        /// <summary>
        /// Indicates whether automatic updates should be checked at startup of application.
        /// </summary>
        //==========================================================================//
        [UserConfigsProperty]
        public static bool AutomaticCheckForUpdate { get; set; } = true;

        //==========================================================================//
        /// <summary>
        /// Defines the player mode used by the application's player. 
        /// The mode can be NoLoop, Loop, etc...
        /// </summary>
        //==========================================================================//
        [UserConfigsProperty]
        public static PlayerMode PlayerMode { get; set; } = PlayerMode.NoLoop;

        //==========================================================================//
        /// <summary>
        /// Specifies the volume level for the application's player.
        /// The value is a float between 0.0 and 1.0, where 0.5 is the default.
        /// </summary>
        //==========================================================================//
        [UserConfigsProperty]
        public static float Volume { get; set; } = 0.5f;

        #endregion

        #region METHODS
        #region Get configuation file path
        //==========================================================================//
        /// <summary>
        /// Gets the path to the configuration file in the user's application data folder.
        /// If the application directory doesn't exist, it will be created.
        /// </summary>
        /// <returns>Returns the full path of the configuration file.</returns>
        //==========================================================================//
        private static string GetUserConfigsFilePath()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataFolder, Assembly.GetExecutingAssembly().GetName().Name);
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            return Path.Combine(appFolder, Assembly.GetExecutingAssembly().GetName().Name + ".cfg");
        }
        #endregion

        #region Save configuration file
        //==========================================================================//
        /// <summary>
        /// Saves the user configuration properties to a file.
        /// All properties marked with the [ConfigProperty] attribute will be saved 
        /// in the form of key-value pairs.
        /// </summary>
        //==========================================================================//
        public static void Save()
        {
            var filePath = GetUserConfigsFilePath();

            using (var writer = new StreamWriter(filePath))
            {
                var properties = typeof(UserConfigs).GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(prop => Attribute.IsDefined(prop, typeof(UserConfigsPropertyAttribute)));

                foreach (var property in properties)
                {
                    var value = property.GetValue(null);
                    writer.WriteLine($"{property.Name} = {value}");
                }
            }
        }

        #endregion

        #region Load configuration file
        //==========================================================================//
        /// <summary>
        /// Loads the user configuration properties from a file.
        /// The method reads the configuration file and applies values to properties 
        /// marked with the [ConfigProperty] attribute, converting them to their correct types.
        /// </summary>
        //==========================================================================//
        public static void Load()
        {
            var filePath = GetUserConfigsFilePath();

            // If the config file doesn't exist, return without loading
            if (!File.Exists(filePath))
                return;

            var lines = File.ReadAllLines(filePath);

            var properties = typeof(UserConfigs).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(prop => Attribute.IsDefined(prop, typeof(UserConfigsPropertyAttribute))).ToList();

            // Iterate over each line in the configuration file
            foreach (var line in lines)
            {
                if (!line.Contains(" = ")) continue;

                var parts = line.Split(new[] { " = " }, StringSplitOptions.None);
                var propertyName = parts[0];
                var propertyValue = parts[1];

                // Find the corresponding property based on its name
                var property = properties.FirstOrDefault(prop => prop.Name == propertyName);
                if (property == null) continue;

                // Handle various property types and try to parse values
                if (property.PropertyType == typeof(int) && int.TryParse(propertyValue, out var intValue))
                {
                    property.SetValue(null, intValue);
                }
                else if (property.PropertyType == typeof(float) && float.TryParse(propertyValue, out var floatValue))
                {
                    property.SetValue(null, floatValue);
                }
                else if (property.PropertyType == typeof(bool) && bool.TryParse(propertyValue, out var boolValue))
                {
                    property.SetValue(null, boolValue);
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(null, propertyValue);
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(null, propertyValue);
                }
                else if (property.PropertyType.IsEnum)
                {
                    try
                    {
                        // Attempt to parse the enum value
                        var enumValue = Enum.Parse(property.PropertyType, propertyValue);
                        property.SetValue(null, enumValue);
                    }
                    catch (ArgumentException)
                    {
                        // If parsing fails, use the default enum value (the first one)
                        var defaultEnumValue = Enum.GetValues(property.PropertyType).GetValue(0);
                        property.SetValue(null, defaultEnumValue);
                    }
                }
            }
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// Attribute that is used to mark properties that should be automatically saved 
    /// and loaded in the configuration file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UserConfigsPropertyAttribute : Attribute
    {
    }
}
