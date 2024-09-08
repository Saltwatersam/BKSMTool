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
using BKSMTool.Files.File_WEM;
using System.ComponentModel;
using static BKSMTool.Files.File_Audio.AudioOperations;
using System;
using BKSMTool.audio_player_converter;


namespace BKSMTool.Files.File_Audio
{
    /// <summary>
    /// Represents an audio file, providing functionality for handling audio data, including retrieving properties such as duration, sample rate, and channels.
    /// Implements <see cref="INotifyPropertyChanged"/> to notify when properties change, and <see cref="IDisposable"/> for proper resource management.
    /// </summary>
    public class AudioFile : INotifyPropertyChanged, IAudioFile, IDisposable
    {
        #region IAudioFile Properties

        private AudioType _type;

        /// <summary>
        /// Gets or sets the type of audio (e.g., OGG, WAV, etc.).
        /// </summary>
        public AudioType Type
        {
            get => _type;
            set
            {
                if (_type == value) return;
                _type = value;
                NotifyPropertyChanged(nameof(Type));
            }
        }

        private byte[] _audioData;

        /// <summary>
        /// Gets the byte array containing the audio data.
        /// </summary>
        public byte[] AudioData
        {
            get => _audioData;
            private set
            {
                if (_audioData == value) return;
                if (value != null)
                {
                    _audioData = value;
                    NotifyPropertyChanged(nameof(AudioData));
                    AudioDataLength = AudioData.Length;
                }
                else
                {
                    _audioData = null;
                }
            }
        }

        private int _audioDataLength;

        /// <summary>
        /// Gets the length of the audio data in bytes.
        /// </summary>
        public int AudioDataLength
        {
            get => _audioDataLength;
            private set
            {
                if (_audioDataLength == value) return;
                _audioDataLength = value;
                NotifyPropertyChanged(nameof(AudioDataLength));
            }
        }

        /// <summary>
        /// Gets the name of the audio, combining the ID and event name if available.
        /// </summary>
        public string AudioName => SourceFile.EventName != null ? $"{SourceFile.IdValue}  -  {SourceFile.EventName}" : $"{SourceFile.IdValue}";

        private double _audioDurationInSeconds;

        /// <summary>
        /// Gets the duration of the audio in seconds.
        /// </summary>
        public double AudioDurationInSeconds
        {
            get => _audioDurationInSeconds;
            private set
            {
                if (!(Math.Abs(_audioDurationInSeconds - value) > 0)) return;
                _audioDurationInSeconds = value;
                NotifyPropertyChanged(nameof(AudioDurationInSeconds));
                using (var thisConverter = new AudioOperations())
                {
                    AudioDurationAsString = thisConverter.DurationToReadableFormat(_audioDurationInSeconds);
                }
            }
        }

        private string _audioDurationAsString = "00:00";

        /// <summary>
        /// Gets the duration of the audio in a human-readable string format (e.g., mm:ss or hh:mm:ss).
        /// </summary>
        public string AudioDurationAsString
        {
            get => _audioDurationAsString;
            private set
            {
                if (_audioDurationAsString == value) return;
                _audioDurationAsString = value;
                NotifyPropertyChanged(nameof(AudioDurationAsString));
            }
        }

        private PlaybackState _audioState = PlaybackState.Stopped;

        /// <summary>
        /// Gets or sets the current playback state of the audio (e.g., Playing, Stopped, etc.).
        /// </summary>
        public PlaybackState AudioState
        {
            get => _audioState;
            set
            {
                if (_audioState == value) return;
                _audioState = value;
                NotifyPropertyChanged(nameof(AudioState));
            }
        }

        private int _audioChannels;

        /// <summary>
        /// Gets the number of channels in the audio file.
        /// </summary>
        public int AudioChannels
        {
            get => _audioChannels;
            private set
            {
                if (_audioChannels == value) return;
                _audioChannels = value;
                NotifyPropertyChanged(nameof(AudioChannels));
            }
        }

        private int _audioSampleRate;

        /// <summary>
        /// Gets the sample rate of the audio file.
        /// </summary>
        public int AudioSampleRate
        {
            get => _audioSampleRate;
            private set
            {
                if (_audioSampleRate == value) return;
                _audioSampleRate = value;
                NotifyPropertyChanged(nameof(AudioSampleRate));
            }
        }
        #endregion

        #region Properties
        // Flag to indicate whether the object has been disposed
        private bool _disposed;

        /// <summary>
        /// Gets the associated WEM file as the source of this audio.
        /// </summary>
        public Wem SourceFile;

        /// <summary>
        /// Gets the WEM ID of the audio file.
        /// </summary>
        public uint WemId => SourceFile.IdValue;

        /// <summary>
        /// Gets the WEM event name of the audio file.
        /// </summary>
        public string WemEventName => SourceFile.EventName;

        private string _wemSizeAsString;

        /// <summary>
        /// Gets the size of the WEM file as a human-readable string.
        /// </summary>
        public string WemSizeAsString
        {
            get => _wemSizeAsString;
            private set
            {
                if (_wemSizeAsString == value) return;
                _wemSizeAsString = value;
                NotifyPropertyChanged(nameof(WemSizeAsString));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the audio file has been modified.
        /// </summary>
        public bool IsModified => SourceFile.IsModified;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFile"/> class with the given WEM file as its source.
        /// </summary>
        /// <param name="source">The source WEM file for this audio.</param>
        public AudioFile(Wem source)
        {
            SourceFile = source;
            SourceFile.PropertyChanged += Source_PropertyChanged;
            WemSizeAsString = FileOperations.SizeToReadableFormat(SourceFile.Size);
        }
        #endregion

        #region METHODS

        #region PropertyChanged Event of SourceFile (WEM)
        /// <summary>
        /// Handles property changes from the source WEM file, updating relevant properties in the audio file.
        /// </summary>
        private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != SourceFile) return;
            if (e.PropertyName == nameof(Wem.IsModified))
            {
                NotifyPropertyChanged(nameof(IsModified));
            }

            if (e.PropertyName == nameof(Wem.Data))
            {
                WemSizeAsString = FileOperations.SizeToReadableFormat(SourceFile.Size);
                RebuildAudio();
            }
            if (e.PropertyName == nameof(Wem.Size))
            {
                WemSizeAsString = FileOperations.SizeToReadableFormat(SourceFile.Size);
            }
        }
        #endregion

        #region Rebuild Audio from WEM
        /// <summary>
        /// Rebuilds the audio file from the WEM data.
        /// </summary>
        public void RebuildAudio()
        {
            using (var audioOperation = new AudioOperations())
            {
                AudioData = audioOperation.ConvertWemToStandardAudio(SourceFile.Data);
                switch (audioOperation.GetAudioFileType(AudioData))
                {
                    case AudioTypeFormat.OggVorbis:
                        Type = AudioType.Ogg;
                        break;
                    case AudioTypeFormat.WavPcm:
                        Type = AudioType.Wav;
                        break;
                    case AudioTypeFormat.WavImaAdpcm:
                        Type = AudioType.Wav;
                        break;
                }
                AudioDurationInSeconds = audioOperation.RetrieveAudioDuration(this);
                AudioChannels = audioOperation.RetrieveAudioChannels(this);
                AudioSampleRate = audioOperation.RetrieveAudioSampleRate(this);
            }
        }
        #endregion

        #region INotifyPropertyChanged
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

        #region Dispose
        /// <summary>
        /// Releases all resources used by the <see cref="AudioFile"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="AudioFile"/> and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                SourceFile.PropertyChanged -= Source_PropertyChanged;
                SourceFile = null;
                AudioData = null;
                AudioDataLength = 0;
                AudioDurationInSeconds = 0;
                AudioDurationAsString = null;
                AudioState = PlaybackState.Stopped;
                AudioChannels = 0;
                AudioSampleRate = 0;
            }

            // Dispose unmanaged resources

            _disposed = true;
        }

        /// <summary>
        /// Destructor to ensure resources are released when the object is garbage collected.
        /// </summary>
        ~AudioFile()
        {
            Dispose(false);
        }
        #endregion
        #endregion
    }
}
