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
using ManagedBass;

namespace BKSMTool.audio_player_converter
{
    /// <summary>
    /// Singleton class that manages the initialization and disposal of the Bass audio library.
    /// Implements the IDisposable pattern to ensure proper resource management.
    /// </summary>
    public sealed class BassInstancing : IDisposable
    {
        #region Properties
        private bool _disposed; // Indicates whether the object has been disposed
        private bool _isInitialized; // Indicates whether Bass has been successfully initialized
        private static readonly object LockObj = new object(); // Lock object for thread safety
        #endregion

        #region Constructor
        /// <summary>
        /// Private constructor to prevent direct instantiation.
        /// </summary>
        private BassInstancing()
        {
            // Private constructor to prevent instantiation from outside
        }
        #endregion

        #region Singleton Instance
        private static BassInstancing _instance;

        /// <summary>
        /// Gets the singleton instance of <see cref="BassInstancing"/>.
        /// </summary>
        public static BassInstancing Instance
        {
            get
            {
                lock (LockObj)
                {
                    if (_instance != null) return _instance;
                    _instance = new BassInstancing();
                    _instance.InitializeBass(); // Initialize Bass upon instance creation
                }
                return _instance;
            }
        }
        #endregion

        #region METHODS

        #region Initialize Bass
        /// <summary>
        /// Initializes the Bass audio library if it hasn't been initialized yet.
        /// Throws an exception if initialization fails.
        /// </summary>
        private void InitializeBass()
        {
            if (_isInitialized) return; // Prevent multiple initializations
            _isInitialized = Bass.Init(); // Initialize Bass

            if (!_isInitialized)
            {
                HandleBassInitError(); // Handle errors during initialization
            }
        }
        #endregion

        #region Handle Bass Init Errors
        /// <summary>
        /// Handles Bass initialization errors and throws appropriate exceptions based on the error code.
        /// </summary>
        private void HandleBassInitError()
        {
            var errorCode = Bass.LastError;
            switch (errorCode)
            {
                case Errors.Already:
                    // Device is already initialized
                    _isInitialized = true;
                    break;
                case Errors.Driver:
                    // No available device driver
                    throw new InvalidOperationException("No available device driver.");
                default:
                    // Other initialization errors
                    throw new InvalidOperationException("Bass_Init error: " + errorCode);
            }
        }
        #endregion

        #endregion

        #region Dispose
        /// <summary>
        /// Releases the resources used by the <see cref="BassInstancing"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Suppresses finalization to avoid multiple calls
        }

        /// <summary>
        /// Disposes of the resources, both managed and unmanaged, used by the instance.
        /// </summary>
        /// <param name="disposing">Indicates whether the method was called by user code.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Release managed resources
                if (_isInitialized)
                {
                    Bass.Free(); // Free Bass resources
                    _isInitialized = false;
                }
            }

            // Dispose unmanaged resources here, if any

            _disposed = true;
        }

        /// <summary>
        /// Destructor for <see cref="BassInstancing"/> to ensure resources are released.
        /// </summary>
        ~BassInstancing()
        {
            Dispose(false);
        }
        #endregion
    }
}
