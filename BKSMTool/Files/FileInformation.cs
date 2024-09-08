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

using System.ComponentModel;
using System;
using System.IO;

namespace BKSMTool.Files
{
    /// <summary>
    /// Represents file information and provides properties for accessing and manipulating
    /// common file attributes such as name, path, extension, and save status.
    /// Implements INotifyPropertyChanged to notify UI or other components when properties change.
    /// </summary>
    public class FileInformation : INotifyPropertyChanged
    {
        // Public file stream representing the file's current stream.
        public FileStream FileStream { get; set; }

        #region Private Fields

        private string _fullPath;                  // Full path to the file including file name.
        private string _name;                      // File name including extension.
        private string _nameWithoutExtension;      // File name without extension.
        private string _extension;                 // File extension (e.g., ".txt", ".exe").
        private string _shortenedPath;             // Shortened version of the file path.
        private bool _saved = true;                // Indicates whether the file is saved.

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the file extension.
        /// </summary>
        public string Extension
        {
            get => _extension;
            protected set
            {
                if (_extension == value) return;
                _extension = value;
                NotifyPropertyChanged(nameof(Extension));
            }
        }

        /// <summary>
        /// Gets or sets the full path of the file, including the file name.
        /// </summary>
        public string FullPath
        {
            get => _fullPath;
            set
            {
                if (_fullPath == value) return;
                if (value != null)
                {
                    _fullPath = value;
                    NotifyPropertyChanged(nameof(FullPath));
                    Name = Path.GetFileName(FullPath);
                    NameWithoutExtension = Path.GetFileNameWithoutExtension(FullPath);
                    Extension = Path.GetExtension(FullPath);
                    ShortenedPath = FileOperations.ShrinkPath(FullPath, 25); // Adjust length for display purposes.
                }
                else
                {
                    _fullPath = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the file name, including the extension.
        /// </summary>
        public string Name
        {
            get => _name;
            protected set
            {
                if (_name == value) return;
                _name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Gets or sets the file name without the extension.
        /// </summary>
        public string NameWithoutExtension
        {
            get => _nameWithoutExtension;
            protected set
            {
                if (_nameWithoutExtension == value) return;
                _nameWithoutExtension = value;
                NotifyPropertyChanged(nameof(NameWithoutExtension));
            }
        }

        /// <summary>
        /// Gets or sets the shortened version of the full file path.
        /// </summary>
        public string ShortenedPath
        {
            get => _shortenedPath;
            protected set
            {
                if (_shortenedPath == value) return;
                _shortenedPath = value;
                NotifyPropertyChanged(nameof(ShortenedPath));
            }
        }

        /// <summary>
        /// Gets or sets whether the file is saved.
        /// </summary>
        public bool IsSaved
        {
            get => _saved;
            set
            {
                if (_saved == value) return;
                _saved = value;
                NotifyPropertyChanged(nameof(IsSaved));
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInformation"/> class
        /// using the specified file stream.
        /// </summary>
        /// <param name="newFile">The file stream to initialize with.</param>
        /// <exception cref="ArgumentNullException">Thrown when newFile is null.</exception>
        public FileInformation(FileStream newFile)
        {
            FileStream = newFile ?? throw new ArgumentNullException(nameof(newFile));
            FullPath = newFile.Name; // Set full path using the file stream name.
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}