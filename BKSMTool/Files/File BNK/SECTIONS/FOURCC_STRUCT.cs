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
namespace BKSMTool.Files.File_BNK.SECTIONS
{
    /// <summary>
    /// Represents a basic structure with a start offset and size, commonly used in file sections within a BNK file or other structured file formats.
    /// </summary>
    public class FourccStruct
    {
        /// <summary>
        /// Gets or sets the start offset of the section or data block.
        /// </summary>
        public long StartOffset = 0;

        /// <summary>
        /// Gets or sets the size of the section or data block.
        /// </summary>
        public uint Size = 0;
    }
}
