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
using System.Text;
using System.Threading.Tasks;

namespace BKSMTool.CLI.Commands
{
    /// <summary>
    /// Represents the command that displays the version of the application.
    /// Inherits from the base <see cref="Command"/> class and implements version-related logic.
    /// </summary>
    public class VersionCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionCommand"/> class
        /// and sets the command name to "version".
        /// </summary>
        public VersionCommand()
        {
            CommandName = "version";
        }

        /// <summary>
        /// Executes the version command with the given arguments.
        /// Displays the version if no arguments are provided or displays help if the -h or --help option is used.
        /// </summary>
        /// <param name="args">The array of arguments passed from the command line interface.</param>
        /// <returns>A task representing the completion of the command execution.</returns>
        public override Task Execute(string[] args)
        {
            if (args == null || args.Length == 0)  // No arguments are passed
            {
                ShowVersion(); // Display the version information
                return Task.CompletedTask;
            }
            else // Some arguments are passed
            {
                if (args.Length > 1) // More than one argument is passed
                {
                    var jointedArgs = string.Join(" ", args);
                    Console.WriteLine($@"Invalid argument: {jointedArgs}");
                    ShowHelp(); // Show help in case of invalid arguments
                    return Task.CompletedTask;
                }
                else // Only one argument is passed
                {
                    switch (args[0])
                    {
                        case "-h":
                        case "--help": // Display help when -h or --help is provided
                            ShowHelp();
                            return Task.CompletedTask;
                        default: // Handle invalid arguments
                            Console.WriteLine($@"Invalid argument: {args[0]}");
                            ShowHelp();
                            return Task.CompletedTask;
                    }
                }
            }
        }

        /// <summary>
        /// Displays the current version of the application.
        /// </summary>
        protected void ShowVersion()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine(new StringBuilder().Append("\nBKSMTool Version : ").Append(version).ToString());
        }

        /// <summary>
        /// Displays help information for the version command.
        /// </summary>
        protected override void ShowHelp()
        {
            Console.WriteLine(new StringBuilder()
                .Append("\nUsage:")
                .Append("\n  BKSMTool.exe version [options]")
                .Append("\n\nOptions:")
                .Append("\n  -h, --help                 Display this help screen. (Optional)").ToString());
        }
    }
}
