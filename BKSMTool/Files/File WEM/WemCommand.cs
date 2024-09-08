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

namespace BKSMTool.Files.File_WEM
{
    /// <summary>
    /// Represents a command for modifying WEM audio file data, supporting undo and redo operations.
    /// </summary>
    public class WemCommand
    {
        /// <summary>
        /// Gets the original data of the WEM file before the modification.
        /// </summary>
        public byte[] PreviousData { get; }

        /// <summary>
        /// Gets the new data to be applied to the WEM file.
        /// </summary>
        public byte[] NewData { get; }

        /// <summary>
        /// Gets the WEM file that this command operates on.
        /// </summary>
        public Wem WemFile { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WemCommand"/> class.
        /// </summary>
        /// <param name="wemFile">The WEM file to be modified.</param>
        /// <param name="previousData">The original data of the WEM file.</param>
        /// <param name="newData">The new data to be applied to the WEM file.</param>
        public WemCommand(Wem wemFile, byte[] previousData, byte[] newData)
        {
            WemFile = wemFile;
            PreviousData = previousData;
            NewData = newData;
        }

        /// <summary>
        /// Executes the command by replacing the WEM file's data with the new data.
        /// </summary>
        public void Execute()
        {
            WemFile.Data = NewData;
        }

        /// <summary>
        /// Reverts the command, restoring the WEM file's data to its previous state.
        /// </summary>
        public void Unexecute()
        {
            WemFile.Data = PreviousData;
        }
    }
}
