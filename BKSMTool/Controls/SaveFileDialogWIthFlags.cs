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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static BKSMTool.Miscellaneous.NativeMethods;

namespace BKSMTool.Controls
{
    /// <summary>
    /// Provides a customized Save File Dialog with additional flags and options such as title, 
    /// OK button text, file name, and filter management. This class wraps the underlying 
    /// system's file dialog and ensures proper resource management through the IDisposable pattern.
    /// </summary>
    /// <remarks>
    /// This dialog is supported only on Windows Vista or newer due to the use of the IFileDialog COM interface.
    /// It provides extended functionality such as specifying file filters, setting dialog labels, and more.
    /// Implements <see cref="IDisposable"/> to handle resource cleanup.
    /// </remarks>
    internal class SaveFileDialogWIthFlags : IDisposable
    {
        #region Variables
        private bool _disposed;

        /// <summary>
        /// Sets the title text for the dialog.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Sets the text for the OK button in the dialog.
        /// </summary>
        public string OkButtonText { get; set; }

        /// <summary>
        /// Sets or gets the file name displayed in the dialog.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the filter string that determines the types of files displayed in the dialog.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the index of the currently selected filter in the dialog.
        /// </summary>
        public uint FilterIndex { get; set; }
        #endregion

        #region ShowDialog Method
        /// <summary>
        /// Displays the file save dialog to the user.
        /// </summary>
        /// <param name="owner">The owner window of the dialog.</param>
        /// <returns>The <see cref="DialogResult"/> indicating the user's selection.</returns>
        /// <exception cref="NotSupportedException">Thrown if the operating system version is below Windows Vista.</exception>
        internal DialogResult ShowDialog(IWin32Window owner)
        {
            if (Environment.OSVersion.Version.Major < 6) // ShowVistaDialog
                throw new NotSupportedException("This dialog is only supported on Windows Vista or newer.");

            // ReSharper disable once SuspiciousTypeConversion.Global
            var dialogFrm = (IFileDialog)new FileSaveDialogRCW();
            var hResult = dialogFrm.GetOptions(out var options);
            if (hResult != SOk) return DialogResult.Cancel;
            options |= FosDoNotAddToRecent | FosOverwritePrompt | FosPathMustExist;
            hResult = dialogFrm.SetOptions(options);
            if (hResult != SOk) return DialogResult.Cancel;

            if (Title != null)
            {
                hResult = dialogFrm.SetTitle(Title);
                if (hResult != SOk) return DialogResult.Cancel;
            }

            if (OkButtonText != null)
            {
                hResult = dialogFrm.SetOkButtonLabel(OkButtonText);
                if (hResult != SOk) return DialogResult.Cancel;
            }

            if (FileName != null)
            {
                hResult = dialogFrm.SetFileName(FileName);
                if (hResult != SOk) return DialogResult.Cancel;
            }

            if (Filter != null)
            {
                var filterItems = GetFilterItems(Filter);
                hResult = dialogFrm.SetFileTypes((uint)filterItems.Length, filterItems);
                if (hResult != SOk) return DialogResult.Cancel;
                if (filterItems.Length > 0)
                {
                    dialogFrm.SetFileTypeIndex(FilterIndex);
                }
            }

            if (dialogFrm.Show(owner.Handle) != SOk) return DialogResult.Cancel;
            if (dialogFrm.GetResult(out var shellItem) != SOk) return DialogResult.Cancel;

            dialogFrm.GetFileTypeIndex(out var selectedFilterIndex); // Get selected filter index
            FilterIndex = selectedFilterIndex;

            shellItem.GetDisplayName(SigDnFileSysPath, out var path);
            if (string.IsNullOrEmpty(path))
            {
                return DialogResult.None;
            }

            var selectedExtension = GetSelectedExtension(Filter, FilterIndex);
            if (!Path.HasExtension(path))
            {
                path += selectedExtension;
            }
            FileName = path;
            Marshal.ReleaseComObject(shellItem);
            return DialogResult.OK;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Parses the filter string and returns an array of file type filters.
        /// </summary>
        /// <param name="filter">The filter string to parse.</param>
        /// <returns>An array of <see cref="ComdlgFilterspec"/> representing the filter items.</returns>
        /// <exception cref="ArgumentException">Thrown if the filter string is invalid.</exception>
        private static ComdlgFilterspec[] GetFilterItems(string filter)
        {
            var extensions = new List<ComdlgFilterspec>();
            if (string.IsNullOrEmpty(filter))
            {
                return Array.Empty<ComdlgFilterspec>();
            }

            var token = filter.Split('|');
            if (token.Length % 2 != 0)
            {
                throw new ArgumentException("Filter string is invalid.");
            }

            for (var i = 0; i < token.Length; i++)
            {
                ComdlgFilterspec extension;
                extension.pszName = token[i];
                extension.pszSpec = token[++i];
                extensions.Add(extension);
            }

            return extensions.ToArray();
        }

        /// <summary>
        /// Gets the selected file extension based on the filter string and filter index.
        /// </summary>
        /// <param name="filter">The filter string.</param>
        /// <param name="filterIndex">The index of the selected filter.</param>
        /// <returns>The selected file extension.</returns>
        /// <exception cref="ArgumentException">Thrown if the filter string is invalid.</exception>
        private static string GetSelectedExtension(string filter, uint filterIndex)
        {
            if (string.IsNullOrEmpty(filter)) return null;

            var token = filter.Split('|');
            if (token.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid filter string.");
            }

            var filterPos = (int)(filterIndex * 2) - 1;
            if (filterPos < 0 || filterPos >= token.Length) return null;

            var extension = token[filterPos].Replace("*", "");
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            return extension;
        }
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Releases all resources used by the <see cref="SaveFileDialogWIthFlags"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="SaveFileDialogWIthFlags"/> and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            // If disposing equals true, dispose all managed resources
            if (disposing)
            {
                // Dispose managed resources if any
            }

            // Dispose unmanaged resources

            _disposed = true;
        }

        /// <summary>
        /// Destructor to ensure resources are released when the object is garbage collected.
        /// </summary>
        ~SaveFileDialogWIthFlags()
        {
            Dispose(false);
        }
        #endregion
    }
}
