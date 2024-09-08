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

namespace BKSMTool.Files.File_BNK.SECTIONS
{
    /// <summary>
    /// Represents the FXPR section inside a BNK file, containing data related to sound effects processing.
    /// Implements <see cref="IDisposable"/> to ensure proper resource management.
    /// </summary>
    public class Fxpr : FourccStruct, IDisposable
    {
        // Flag to indicate whether the object has been disposed
        private bool _disposed;

        /// <summary>
        /// 4 bytes storing the length of the FXPR section in the BNK file.
        /// </summary>
        public byte[] SectionLength = new byte[4];

        /// <summary>
        /// Stores any unknown or unmanaged leftover bytes at the end of the FXPR section.
        /// </summary>
        public byte[] LeftBytes = { };

        #region Dispose
        /// <summary>
        /// Releases all resources used by the <see cref="Fxpr"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Fxpr"/> and optionally releases managed resources.
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
                SectionLength = null;
                LeftBytes = null;
            }

            // Release unmanaged resources, if any

            _disposed = true;
        }

        /// <summary>
        /// Destructor to ensure resources are released when the object is garbage collected.
        /// </summary>
        ~Fxpr()
        {
            Dispose(false);
        }
        #endregion
    }
}
