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
using BKSMTool.Files.File_BNK.SECTIONS;
using System;

namespace BKSMTool.Files.File_WEM
{
    #region enumeration of WEM Types
    //Enumertion of each WEM Types that was used in games
    //PCM_2 = 0xFFFE,         /* "PCM for Wwise Authoring" */
    //IMA = 0x0002,           /* newer Wwise (variable, probably means "platform's ADPCM") */
    //VORBIS = 0xFFFF,
    //PCM = 0x0001,         /* older Wwise */
    //IMA2 = 0x0069,        /* older Wwise [Spiderman Web of Shadows (X360), LotR Conquest (PC)] */
    //XWMA = 0x0161,        /* WMAv2 */
    //XWMA_2 = 0x0162,      /* WMAPro */
    //DSP = 0xFFF0,
    //XMA2 = 0x0165,        /* XMA2-chunk XMA (Wwise doesn't use XMA1) */
    //XMA2_2 = 0x0166,      /* fmt-chunk XMA */
    //AAC = 0xAAC0,
    //HEVAG = 0xFFFB,       /* "VAG" */
    //ATRAC9 = 0xFFFC,
    //OPUSNX = 0x3039,      /* renamed from "OPUS" on Wwise 2018.1 */
    //OPUS = 0x3040,
    //OPUSCPR,              // ?????
    //OPUSWW = 0x3041,      /* "OPUS_WEM", added on Wwise 2019.2.3, replaces OPUS */
    //PTADPCM = 0x8311,     /* added on Wwise 2019.1, replaces IMA */

    //Trove uses:
    //0xFFFE (PCM)
    //0x0002 (IMA)
    //0xFFFF (VORBIS)
    #endregion
    /// <summary>
    /// Represents a WEM (Wwise Encoded Media) audio file, encapsulating its data, metadata, and functionality 
    /// for modifying, saving, and tracking changes to the file. Implements <see cref="INotifyPropertyChanged"/> to 
    /// notify clients when properties are updated and <see cref="IDisposable"/> for proper resource management.
    /// </summary>
    public class Wem : INotifyPropertyChanged, IDisposable
    {
        #region Local Properties
        //********************************************//
        //********** Variables of the Class **********//
        //********************************************//

        // Flag indicating whether the object has been disposed
        private bool _disposed;

        // Source of the DIDX chunk (meta-information of the WEM file)
        private WemInfo _wemDidxSource;

        // Source of the DATA chunk (actual audio data)
        private WemData _wemDataSource;

        // Size in bytes of the WEM file
        private uint _size;

        // Audio data of the WEM file
        private byte[] _data;

        // Event name associated with the audio in the WEM file
        private string _eventName;

        // Saved data of the WEM file (for comparison with current data)
        private byte[] _savedData;

        // Flag to determine if the WEM file data has been modified
        private bool _isModified;

        /// <summary>
        /// Gets or sets the audio data of the WEM file.
        /// </summary>
        /// <remarks>
        /// If the data is set to a new value, the property change is notified, and 
        /// the size is automatically updated.
        /// </remarks>
        public byte[] Data
        {
            get => _data;
            set
            {
                if (_data == value) return;
                if (value != null)
                {
                    _data = value;
                    NotifyPropertyChanged(nameof(Data));
                    Size = (uint)_data.Length;
                }
                else
                {
                    _data = null;
                    NotifyPropertyChanged(nameof(Data));
                    Size = 0;
                }
                IsModified = _data != _savedData;
            }
        }

        /// <summary>
        /// Gets the size of the WEM file in bytes.
        /// </summary>
        public uint Size
        {
            get => _size;
            private set
            {
                if (_size == value) return;
                _size = value;
                NotifyPropertyChanged(nameof(Size));
            }
        }

        /// <summary>
        /// Gets the ID of the WEM file.
        /// </summary>
        public uint IdValue => _wemDidxSource.FileId;

        /// <summary>
        /// Gets or sets the event name associated with the audio in the WEM file.
        /// </summary>
        public string EventName
        {
            get => _eventName;
            set
            {
                if (_eventName == value) return;
                _eventName = value;
                NotifyPropertyChanged(nameof(EventName));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the WEM file's data has been modified.
        /// </summary>
        public bool IsModified
        {
            get => _isModified;
            set
            {
                if (_isModified == value) return;
                _isModified = value;
                NotifyPropertyChanged(nameof(IsModified));
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Wem"/> class.
        /// </summary>
        /// <param name="infoSource">The source of the DIDX chunk containing WEM meta-information.</param>
        /// <param name="dataSource">The source of the DATA chunk containing the audio data.</param>
        public Wem(WemInfo infoSource, WemData dataSource)
        {
            _wemDidxSource = infoSource;
            _wemDataSource = dataSource;
            Size = (uint)_wemDataSource.Data.Length;
            _savedData = _wemDataSource.Data;
            Data = _wemDataSource.Data;
        }
        #endregion

        #region METHODS
        #region INotifyPropertyChanged Members
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners of a change to a property value.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Save
        /// <summary>
        /// Saves the current WEM file data, marking it as unmodified.
        /// </summary>
        public void Save()
        {
            _savedData = Data;
            IsModified = false;
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Releases all resources used by the <see cref="Wem"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Wem"/> and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Release managed resources
                _wemDidxSource.StartOffset = 0;
                _wemDidxSource.Size = 0;
                _wemDidxSource.FileId = 0;
                Data = null;
                _wemDataSource = null;
                Size = 0;
                EventName = null;
            }

            // Release unmanaged resources here, if any

            _disposed = true;
        }

        /// <summary>
        /// Destructor to release resources when the object is garbage collected.
        /// </summary>
        ~Wem()
        {
            Dispose(false);
        }
        #endregion
        #endregion
    }
}
