﻿// Copyright (c) 2024 Saltwatersam
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

namespace BKSMTool.Files.File_BNK.SECTIONS
{
    /// <summary>
    /// Represents the BKHD (Bank Header) section in a BNK file, containing information such as the version number and ID.
    /// Implements <see cref="IDisposable"/> to ensure proper resource management.
    /// </summary>
    public class Bkhd : FourccStruct, IDisposable
    {
        // Flag to indicate whether the object has been disposed
        private bool _disposed;

        /// <summary>
        /// Gets or sets the version number of the BKHD section (4 bytes).
        /// </summary>
        public uint VersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the ID of the BKHD section (4 bytes).
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Stores any unmanaged or unknown leftover bytes at the end of the section.
        /// </summary>
        public byte[] LeftBytes = new byte[] { };

        #region Dispose
        /// <summary>
        /// Releases all resources used by the <see cref="Bkhd"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Bkhd"/> and optionally releases managed resources.
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
                VersionNumber = 0;
                Id = 0;
                LeftBytes = null;
            }

            // Release unmanaged resources, if any

            _disposed = true;
        }

        /// <summary>
        /// Destructor to ensure resources are released when the object is garbage collected.
        /// </summary>
        ~Bkhd()
        {
            Dispose(false);
        }
        #endregion
    }
}