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
    /// Represents the command that displays help information for the application.
    /// Inherits from the base <see cref="Command"/> class and implements help-related logic.
    /// </summary>
    public class HelpCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommand"/> class
        /// and sets the command name to "help".
        /// </summary>
        public HelpCommand()
        {
            CommandName = "help";
        }

        /// <summary>
        /// Executes the help command. Displays general help information when no arguments are provided,
        /// and shows an error message for invalid arguments.
        /// </summary>
        /// <param name="args">The array of arguments passed from the command line interface.</param>
        /// <returns>A task representing the completion of the command execution.</returns>
        public override Task Execute(string[] args)
        {
            if (args == null || args.Length == 0) // No arguments are passed
            {
                ShowHelp(); // Display general help information
                return Task.CompletedTask;
            }

            // If any argument is passed, it's considered invalid and help is shown
            var jointedArgs = string.Join(" ", args);
            Console.WriteLine($@"Invalid argument: {jointedArgs}");
            ShowHelp(); // Show help after displaying the invalid argument
            return Task.CompletedTask;
        }

        /// <summary>
        /// Displays help information for all available commands and their usage.
        /// </summary>
        protected override void ShowHelp()
        {
            // Output the general usage and list of commands
            Console.WriteLine(new StringBuilder()
                .Append("\nUsage:")
                .Append("\n  BKSMTool.exe [command] [options]") // General usage pattern
                .Append("\n\nCommands:")
                .Append("\n  ExtractAll           Extract all audios from a BNK file.") // Description for the "ExtractAll" command
                .Append("\n  Help                 Display help information.") // Description for the "Help" command
                .Append("\n  Version              Display version.") // Description for the "Version" command
                .Append("\nUse 'BKSMTool.exe [command] --help' to see options for a specific command.").ToString());
        }
    }
}