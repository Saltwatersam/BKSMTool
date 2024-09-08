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

using System.Threading.Tasks;

namespace BKSMTool.CLI.Commands
{
    /// <summary>
    /// Abstract base class for defining commands in the command line interface.
    /// Each derived class must implement the logic for executing the command and displaying help information.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Gets or sets the name of the command.
        /// This property is typically set in the constructor of the derived class.
        /// </summary>
        public string CommandName { get; protected set; }

        /// <summary>
        /// Executes the command with the provided arguments.
        /// Each derived class must implement its own logic for processing the arguments and performing the associated action.
        /// </summary>
        /// <param name="args">The array of arguments passed from the command line interface.</param>
        /// <returns>A task representing the asynchronous operation of the command.</returns>
        public abstract Task Execute(string[] args);

        /// <summary>
        /// Displays help information for the command.
        /// Each derived class must implement its own logic for showing the relevant help or usage instructions.
        /// </summary>
        protected abstract void ShowHelp();
    }
}
