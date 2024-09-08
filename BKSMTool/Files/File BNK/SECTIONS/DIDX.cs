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

using System.Collections.Generic;
using System;

namespace BKSMTool.Files.File_BNK.SECTIONS
{
    #region Structure of elements in DIDX
    /// <summary>
    /// Represents information about a WEM file, including its ID, start offset, and size within a BNK file.
    /// </summary>
    public struct WemInfo
    {
        //*************************************//
        //********** Local Variables **********//
        //*************************************//

        /// <summary>
        /// Gets or sets the ID of the WEM file (4 bytes).
        /// </summary>
        public uint FileId { get; set; }

        /// <summary>
        /// Gets or sets the start offset of the WEM file in the BNK file (4 bytes).
        /// </summary>
        public uint StartOffset { get; set; }

        /// <summary>
        /// Gets or sets the size of the WEM file (4 bytes).
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WemInfo"/> struct with the specified WEM file ID, start offset, and size.
        /// </summary>
        /// <param name="wemFileId">The ID of the WEM file.</param>
        /// <param name="wemFileOffset">The start offset of the WEM file.</param>
        /// <param name="wemFileLength">The size of the WEM file in bytes.</param>
        public WemInfo(uint wemFileId, uint wemFileOffset, uint wemFileLength)
        {
            FileId = wemFileId;
            StartOffset = wemFileOffset;
            Size = wemFileLength;
        }
    }
    #endregion

    /// <summary>
    /// Represents the DIDX (Data Index) section inside a BNK file, containing information about the WEM files within the BNK file.
    /// Implements <see cref="IDisposable"/> to ensure proper resource management.
    /// </summary>
    public class Didx : FourccStruct, IDisposable
    {
        // Flag to indicate whether the object has been disposed
        private bool _disposed;

        /// <summary>
        /// 4 bytes storing the length of the DIDX section in the BNK file.
        /// </summary>
        public byte[] SectionLength = new byte[4];

        /// <summary>
        /// A list of <see cref="WemInfo"/> objects representing each WEM file's ID, starting offset, and size within the BNK file.
        /// </summary>
        public readonly List<WemInfo> WemFilesInfo = new List<WemInfo>();

        /// <summary>
        /// The number of WEM files contained in the BNK file, calculated from the section length of the DIDX section.
        /// </summary>
        public uint NumberOfWem;

        #region Dispose
        /// <summary>
        /// Releases all resources used by the <see cref="Didx"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Didx"/> and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Release managed resources
                StartOffset = 0;
                Size = 0;
                NumberOfWem = 0;
                SectionLength = null;
                WemFilesInfo.Clear();
            }

            // Release unmanaged resources, if any

            _disposed = true;
        }

        /// <summary>
        /// Destructor to ensure resources are released when the object is garbage collected.
        /// </summary>
        ~Didx()
        {
            Dispose(false);
        }
        #endregion
    }
}
