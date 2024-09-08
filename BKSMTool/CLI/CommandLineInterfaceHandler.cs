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
using BKSMTool.CLI.Commands;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text;

namespace BKSMTool.CLI
{
    /// <summary>
    /// A static class that handles the command line interface (CLI) for the application. 
    /// It allocates a console, parses and executes commands provided by the user.
    /// </summary>
    public static class CommandLineInterfaceHandler
    {
        /// <summary>
        /// Imports the AllocConsole function from kernel32.dll to allocate a new console for the application.
        /// </summary>
        /// <returns>Returns true if the console is successfully allocated; otherwise, false.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        /// <summary>
        /// A dictionary containing the available CLI commands. Each command is mapped to its corresponding <see cref="Command"/> object.
        /// </summary>
        private static readonly Dictionary<string, Command> Commands;

        /// <summary>
        /// Static constructor to initialize the list of available commands.
        /// </summary>
        static CommandLineInterfaceHandler()
        {
            // Initialize the available commands
            Commands = new Dictionary<string, Command>
        {
            { "extractall", new ExtractAllCommand() }, // Command to extract all data
            { "help", new HelpCommand() },             // Command to display help information
            { "version", new VersionCommand() }        // Command to show version information
        };
        }

        /// <summary>
        /// Executes the specified command with the provided arguments.
        /// </summary>
        /// <param name="args">An array of arguments passed from the CLI. The first argument is the command name.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task Execute(string[] args)
        {
            // Get the command name (the first argument) and convert it to lowercase
            var commandName = args[0].ToLower();

            // Check if the command exists in the dictionary
            if (Commands.TryGetValue(commandName, out var command))
            {
                // Execute the command with the remaining arguments
                await command.Execute(args.Skip(1).ToArray());
            }
            else
            {
                // If the command is not recognized, display an error message and execute the help command
                Console.WriteLine($@"Unknown command: {args[0]}");
                await Commands["help"].Execute(args);
            }
        }

        /// <summary>
        /// Starts the command line interface, allocates a console, and handles the command execution.
        /// </summary>
        /// <param name="args">An array of arguments passed from the CLI.</param>
        public static void RunCli(string[] args)
        {
            // Allocate a console for displaying output
            AllocConsole();

            // Set the console title
            Console.Title = @"BKSMTool";

            // Display a header banner
            Console.WriteLine(@"========================================================================================================================");
            Console.WriteLine(@"|                                                        BKSMTool                                                      |");
            Console.WriteLine(@"|                                                    by Saltwater_sam                                                  |");
            Console.WriteLine(@"========================================================================================================================");
            Console.WriteLine(new StringBuilder().Append("\n").ToString());

            // Execute the CLI command asynchronously
            Execute(args).GetAwaiter().GetResult();

            Console.WriteLine(new StringBuilder().Append("\n").ToString());

            // Wait for the user to press the Enter key before exiting
            Console.WriteLine(@"Press <Enter> to exit...");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }
    }
}
