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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using ManagedBass;

namespace BKSMTool.audio_player_converter
{
    public interface IAudioFile
    {
        AudioType Type { get; }
        byte[] AudioData { get; }
        int AudioDataLength { get; }
        string AudioName { get; }
        double AudioDurationInSeconds { get; }
        string AudioDurationAsString { get; }
        PlaybackState AudioState { get; set; }
    }

    public enum AudioType
    {
        Mp3,
        Ogg,
        Wav,
    }

    public enum PlaybackState
    {
        Stopped,
        Playing,
        Paused
    }

    public enum PlayerMode
    {
        NoLoop,
        LoopSingleFile,
        LoopList
    }

    /// <summary>
    /// The AudioPlayer class is used to play, pause, resume, stop, and manage audio files.
    /// It provides functionalities for handling different player modes, volume control, and audio file lists.
    /// </summary>
    public class AudioPlayer : INotifyPropertyChanged
    {
        #region CONSTRUCTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioPlayer"/> class.
        /// Sets up event listeners and initializes timers.
        /// </summary>
        private AudioPlayer()
        {
            _currentPlayer = PlayerEngineCore.Instance;
            Volume = 0.5f;
            PlayerEngineCore.AudioStarted += CoreAudioStarted;
            PlayerEngineCore.AudioPaused += CoreAudioPaused;
            PlayerEngineCore.AudioResumed += CoreAudioResumed;
            PlayerEngineCore.AudioStopped += CoreAudioStopped;
            PlayerEngineCore.AudioReachedEnd += CorePlaybackReachedEnd;
            PlayerEngineCore.AudioRestarted += CoreAudioRestarted;
            PlayerEngineCore.VolumeChanged += CoreVolumeChanged;
            PlayerEngineCore.AudioDurationChanged += CoreAudioDurationChanged;
            PlayerEngineCore.AudioPositionSet += CoreAudioPositionSet;
            InitTimer();
        }
        #endregion

        #region SINGLETONE
        private static AudioPlayer _instance;
        /// <summary>
        /// Gets the singleton instance of the <see cref="AudioPlayer"/> class.
        /// Ensures that the instance is created on the main thread.
        /// </summary>
        public static AudioPlayer Instance
        {
            get
            {
                if (_instance != null) return _instance;
                if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                {
                    _instance = new AudioPlayer();
                }
                else
                {
                    var uiSynchronization = SynchronizationContext.Current;
                    uiSynchronization.Send(_ => _instance = new AudioPlayer(), null);
                }

                return _instance;
            }
        }
        #endregion

        #region PROPERTIES
        #region PUBLIC PROPERTIES
        #region Volume
        /// <summary>
        /// Gets or sets the playback volume. Value ranges from 0.0f to 1.0f. Default is 0.5f.
        /// </summary>
        [DefaultValue(0.5f)]
        public float Volume
        {
            get => _currentPlayer.Volume;
            set
            {
                _currentPlayer.Volume = value;
                VolumeChanged?.Invoke(this, null);
                NotifyPropertyChanged(nameof(Volume));
            }
        }
        #endregion
        #region Current IAudioFile
        private IAudioFile _currentIAudioFile;
        /// <summary>
        /// Gets or sets the current audio file being played.
        /// </summary>
        public IAudioFile CurrentIAudioFile
        {
            get => _currentIAudioFile;
            private set
            {
                if (_currentIAudioFile == value) return;
                _currentIAudioFile = value;
                AudioFileChanged?.Invoke(this, null);
                NotifyPropertyChanged(nameof(CurrentIAudioFile));
            }
        }
        #endregion
        #region Audio Duration
        /// <summary>
        /// Gets the total duration of the current audio in seconds.
        /// </summary>
        public double AudioDuration => _currentPlayer.AudioDurationInSeconds;

        private string _audioDurationAsString = "00:00";
        /// <summary>
        /// Gets the duration of the current audio file as a formatted string.
        /// </summary>
        public string AudioDurationAsString
        {
            get => _audioDurationAsString;
            private set
            {
                _audioDurationAsString = value;
                NotifyPropertyChanged(nameof(AudioDurationAsString));
            }
        }
        #endregion
        #region Audio Position
        private double _audioPosition = -1;
        /// <summary>
        /// Gets or sets the current position of the audio playback in seconds.
        /// </summary>
        public double AudioPosition
        {
            get => _audioPosition;
            set
            {
                if (Math.Abs(_audioPosition - value) < 0) return;
                _audioPosition = value;
                AudioPositionChanged?.Invoke(this, null);
                NotifyPropertyChanged(nameof(AudioPosition));
                AudioPositionAsString = _audioPosition < 0 ? "00:00" : ConvertTimeToString(value);
            }
        }

        private string _audioPositionAsString = "00:00";
        /// <summary>
        /// Gets the current playback position as a formatted string.
        /// </summary>
        public string AudioPositionAsString
        {
            get => _audioPositionAsString;
            private set
            {
                _audioPositionAsString = value;
                NotifyPropertyChanged(nameof(AudioPositionAsString));
            }
        }
        #endregion

        #region Player Mode
        private PlayerMode _mode;
        /// <summary>
        /// Gets or sets the current mode of the player (e.g., NoLoop, LoopSingleFile, LoopList).
        /// </summary>
        public PlayerMode Mode
        {
            get => _mode;
            set
            {
                if (_mode == value) return;
                _mode = value;
                PlayerModeChanged?.Invoke(this, null);
                NotifyPropertyChanged(nameof(Mode));
            }
        }
        #endregion
        #region Player State
        /// <summary>
        /// Gets the current state of the player (Stopped, Playing, Paused).
        /// Default value is Stopped.
        /// </summary>
        [DefaultValue(PlaybackState.Stopped)]
        public PlaybackState PlayerState
        {
            get => _playerState;
            private set
            {
                if (_playerState == value) return;
                _playerState = value;
                switch (_playerState)
                {
                    case PlaybackState.Playing:
                    {
                        PlayerStarted?.Invoke(this, null);
                    }
                        break;
                    case PlaybackState.Paused:
                    {
                        PlayerPaused?.Invoke(this, null);
                    }
                        break;
                    case PlaybackState.Stopped:
                    {
                        PlayerStopped?.Invoke(this, null);
                    }
                        break;
                    default:
                    {
                        throw new Exception("Unknown State");
                    }
                }
                PlayerStateChanged?.Invoke(this, null);
                NotifyPropertyChanged(nameof(PlaybackState));
            }
        }
        #endregion

        #region Audio Name
        private string _audioName;
        /// <summary>
        /// Gets the name of the current audio file being played.
        /// </summary>
        public string AudioName
        {
            get => _audioName;
            private set
            {
                if (_audioName == value) return;
                _audioName = value;
                NotifyPropertyChanged(nameof(AudioName));
            }
        }

        #endregion
        #endregion

        #region PRIVATE PROPERTIES
        private readonly PlayerEngineCore _currentPlayer;
        private PlaybackState _playerState;
        private static readonly System.Windows.Forms.Timer TimerAudioPositionInSeconds = new System.Windows.Forms.Timer();
        private List<IAudioFile> _iAudioList = new List<IAudioFile>();
        private int _iAudioFileIndex;
        #endregion
        #endregion

        #region EVENTS
        public static event EventHandler AudioStarted;
        public static event EventHandler AudioPaused;
        public static event EventHandler AudioResumed;
        public static event EventHandler AudioRestarted;
        public static event EventHandler AudioStopped;
        public static event EventHandler AudioEnded;
        public static event EventHandler AudioPositionSet;
        public static event EventHandler VolumeChanged;
        public static event EventHandler AudioDurationChanged;
        public static event EventHandler AudioPositionChanged;
        public static event EventHandler PlayerModeChanged;
        public static event EventHandler PlayerStarted;
        public static event EventHandler PlayerPaused;
        public static event EventHandler PlayerStopped;
        public static event EventHandler PlayerStateChanged;
        public static event EventHandler AudioFileChanged;

        #region Core Events
        #region Core audio Started
        public void CoreAudioStarted(object sender, EventArgs e)
        {
            AudioStarted?.Invoke(this, e);
            PlayerState = PlaybackState.Playing;
            CurrentIAudioFile.AudioState = PlaybackState.Playing;
            TimerAudioPositionInSeconds.Enabled = true;
        }
        #endregion
        #region Core audio Paused
        public void CoreAudioPaused(object sender, EventArgs e)
        {
            AudioPaused?.Invoke(this, e);
            PlayerState = PlaybackState.Paused;
            CurrentIAudioFile.AudioState = PlaybackState.Paused;
            TimerAudioPositionInSeconds.Enabled = true;
        }
        #endregion
        #region Core audio Resumed
        public void CoreAudioResumed(object sender, EventArgs e)
        {
            AudioResumed?.Invoke(this, e);
            PlayerState = PlaybackState.Playing;
            CurrentIAudioFile.AudioState = PlaybackState.Playing;
            TimerAudioPositionInSeconds.Enabled = true;
        }
        #endregion
        #region core audio Restarted
        public void CoreAudioRestarted(object sender, EventArgs e)
        {
            AudioRestarted?.Invoke(this, e);
            AudioPosition = 0;
            PlayerState = PlaybackState.Playing;
            CurrentIAudioFile.AudioState = PlaybackState.Playing;
            TimerAudioPositionInSeconds.Enabled = true;
        }
        #endregion
        #region Core audio Stopped
        public void CoreAudioStopped(object sender, EventArgs e)
        {
            AudioStopped?.Invoke(this, e);
            TimerAudioPositionInSeconds.Enabled = false;
            PlayerState = PlaybackState.Stopped;
            if (CurrentIAudioFile != null)
            {
                CurrentIAudioFile.AudioState = PlaybackState.Stopped;

            }
            CurrentIAudioFile = null;
            _iAudioList = null;
            _iAudioFileIndex = 0;
            AudioName = null;
            AudioPosition = -1;
        }
        #endregion
        #region Core audio Ended
        public void CorePlaybackReachedEnd(object sender, EventArgs e)
        {
            AudioEnded?.Invoke(this, e);
            switch (Mode)
            {
                case PlayerMode.LoopList:
                    try
                    {
                        NextAudio();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                    break;
                case PlayerMode.LoopSingleFile:
                    try
                    {
                        _currentPlayer.RestartAudio();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                    break;
                case PlayerMode.NoLoop:
                    try
                    {
                        _currentPlayer.StopAudio();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                    break;
                default:
                    throw new Exception("Unknown State");
            }
        }
        #endregion
        #region Core audio PositionSet
        public void CoreAudioPositionSet(object sender, EventArgs e)
        {
            AudioPositionSet?.Invoke(this, e);
        }
        #endregion

        #region Core Volume
        public void CoreVolumeChanged(object sender, EventArgs e)
        {
            VolumeChanged?.Invoke(this, null);
            NotifyPropertyChanged(nameof(Volume));
        }
        #endregion

        #region Core Audio Duration
        public void CoreAudioDurationChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged(nameof(AudioDuration));
            AudioDurationAsString = AudioDuration < 0 ? "00:00" : ConvertTimeToString(AudioDuration);
            AudioDurationChanged?.Invoke(this, null);
        }
        #endregion

        #endregion
        #endregion

        #region METHODS
        #region Timer Update position
        /// <summary>
        /// Initializes the timer that updates the audio position in real time.
        /// </summary>
        private void InitTimer()
        {
            TimerAudioPositionInSeconds.Interval = 1;
            TimerAudioPositionInSeconds.Tick += OnTickEvent;
        }
        private void OnTickEvent(object sender, EventArgs e)
        {
            AudioPosition = _currentPlayer.CurrentPosition;
        }
        #endregion
        #region Play from IAudioFile
        /// <summary>
        /// Plays or pauses the specified <see cref="IAudioFile"/>.
        /// </summary>
        /// <param name="thisAudioFile">The audio file to be played or paused.</param>
        private void PlayPauseIAudioFile(IAudioFile thisAudioFile)
        {
            if (CurrentIAudioFile != null && thisAudioFile == CurrentIAudioFile)
            {
                switch (PlayerState)
                {
                    case PlaybackState.Playing:
                        try
                        {
                            _currentPlayer.PauseAudio();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        return;
                    case PlaybackState.Paused:
                        try
                        {
                            _currentPlayer.ResumeAudio();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                IAudioFile tempFile = null;
                if (CurrentIAudioFile != null && CurrentIAudioFile != thisAudioFile)
                {
                    tempFile = CurrentIAudioFile;
                }
                try
                {
                    _currentPlayer.StartAudio(thisAudioFile.AudioData);
                    if (tempFile != null)
                    {
                        tempFile.AudioState = PlaybackState.Stopped;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                CurrentIAudioFile = thisAudioFile;
                AudioName = CurrentIAudioFile.AudioName;
            }

        }
        #endregion
        #region Play from IAudioFile
        /// <summary>
        /// Plays or replays the specified <see cref="IAudioFile"/>.
        /// If the audio file is already playing, it restarts the playback. If the file is paused, it resumes from the beginning.
        /// </summary>
        /// <param name="thisAudioFile">The audio file to be played or replayed.</param>
        private void PlayReplayIAudioFile(IAudioFile thisAudioFile)
        {
            if (CurrentIAudioFile != null && thisAudioFile == CurrentIAudioFile)
            {
                switch (PlayerState)
                {
                    case PlaybackState.Playing:
                        try
                        {
                            _currentPlayer.RestartAudio();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        return;
                    case PlaybackState.Paused:
                        try
                        {
                            _currentPlayer.RestartAudio();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                IAudioFile tempFile = null;
                if (CurrentIAudioFile != null && CurrentIAudioFile != thisAudioFile)
                {
                    tempFile = CurrentIAudioFile;
                }
                try
                {
                    _currentPlayer.StartAudio(thisAudioFile.AudioData);
                    if (tempFile != null)
                    {
                        tempFile.AudioState = PlaybackState.Stopped;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                CurrentIAudioFile = thisAudioFile;
                AudioName = CurrentIAudioFile.AudioName;
            }

        }
        #endregion

        #region [PLAY] from list audio object (IAudioFile)
        /// <summary>
        /// Plays an audio file from a playlist at the specified index.
        /// </summary>
        /// <param name="playlist">The list of audio files.</param>
        /// <param name="index">The index of the audio file to be played.</param>
        /// <exception cref="Exception">Thrown if the playlist is empty or null.</exception>
        public void PlayList(List<IAudioFile> playlist, int index = 0)
        {

            if (playlist.Count == 0 || playlist == null)
            {
                throw new Exception("List is empty.");
            }

            if (index < 0)
            {
                index = 0;
            }

            if (index >= playlist.Count)
            {
                index = playlist.Count - 1;
            }

            PlayPauseIAudioFile(playlist[index]);
            _iAudioList = playlist;
            _iAudioFileIndex = index;
        }
        #endregion
        #region [PLAY] from list audio object (IAudioFile)
        /// <summary>
        /// Plays the previous or next audio file from the playlist at the specified index.
        /// </summary>
        /// <param name="playlist">The list of audio files.</param>
        /// <param name="index">The index of the audio file to be played.</param>
        /// <exception cref="Exception">Thrown if the playlist is empty or null.</exception>
        public void PlayPreviousNextList(List<IAudioFile> playlist, int index = 0)
        {

            if (playlist.Count == 0 || playlist == null)
            {
                throw new Exception("List is empty.");
            }

            if (index < 0)
            {
                index = 0;
            }

            if (index >= playlist.Count)
            {
                index = playlist.Count - 1;
            }

            PlayReplayIAudioFile(playlist[index]);
            _iAudioList = playlist;
            _iAudioFileIndex = index;
        }
        #endregion
        #region [PAUSE]
        /// <summary>
        /// Pauses the current audio playback.
        /// </summary>
        public void Pause()
        {
            try
            {
                _currentPlayer.PauseAudio();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion
        #region [RESUME]
        /// <summary>
        /// Resumes the audio playback if it was paused.
        /// </summary>
        public void Resume()
        {
            try
            {
                _currentPlayer.ResumeAudio();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion
        #region [STOP]
        /// <summary>
        /// Stops the current audio playback.
        /// </summary>
        public void Stop()
        {
            try
            {
                _currentPlayer.StopAudio();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion
        #region [Next] audio
        /// <summary>
        /// Plays the next audio file in the playlist. If the end of the list is reached, playback loops back to the first file.
        /// </summary>
        /// <exception cref="Exception">Thrown if the playlist is empty.</exception>
        public void NextAudio()
        {
            if (_iAudioList == null || _iAudioList.Count <= 0)
            {
                throw new Exception("List is empty.");
            }
            var nextAudioFileIndex = (_iAudioFileIndex + 1) % _iAudioList.Count;
            if (CurrentIAudioFile == _iAudioList[nextAudioFileIndex])
            {
                try
                {
                    _currentPlayer.RestartAudio();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                try
                {
                    PlayPauseIAudioFile(_iAudioList[nextAudioFileIndex]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                _iAudioFileIndex = nextAudioFileIndex;
            }
        }
        #endregion
        #region [Previous] audio
        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Plays the previous audio file in the playlist. 
        /// If the first audio file is currently playing, it loops back to the last audio file in the playlist.
        /// </summary>
        /// <exception cref="Exception">Thrown if the playlist is empty.</exception>
        public void PreviousAudio()
        {
            if (_iAudioList == null || _iAudioList.Count <= 0)
            {
                throw new Exception("List is empty.");
            }

            int nextAudioFileIndex;
            if (_iAudioFileIndex == 0)
            {
                nextAudioFileIndex = _iAudioList.Count - 1;
            }
            else
            {
                nextAudioFileIndex = _iAudioFileIndex - 1;
            }

            if (CurrentIAudioFile == _iAudioList[nextAudioFileIndex])
            {
                try
                {
                    _currentPlayer.RestartAudio();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                try
                {
                    PlayPauseIAudioFile(_iAudioList[nextAudioFileIndex]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                _iAudioFileIndex = nextAudioFileIndex;
            }
        }
        #endregion

        #region Convert Time To String

        private static string ConvertTimeToString(double time)
        {
            var duration = TimeSpan.FromSeconds(time);

            return duration.ToString(duration.TotalHours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss");
        }
        #endregion
        #region Set Player Position
        public void SetPlayerPosition(double position)
        {
            try
            {
                if (position < 0)
                {
                    _currentPlayer.CurrentPosition = 0;
                }
                else
                {
                    _currentPlayer.CurrentPosition = position;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion
        #endregion

        #region INotifyPropertyChanged Members       
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    #region Player Engine Core
    /// <summary>
    /// PlayerEngineCore is a singleton class responsible for managing audio playback.
    /// It controls audio streaming, volume, position, and supports operations such as play, pause, resume, and stop.
    /// </summary>
    public sealed class PlayerEngineCore
    {
        #region CONSTRUCTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerEngineCore"/> class.
        /// It also initializes the audio engine and sets up synchronization context for handling events.
        /// </summary>
        /// <exception cref="Exception">Thrown if the Bass audio engine fails to initialize.</exception>
        private PlayerEngineCore()
        {
            try
            {
                _ = BassInstancing.Instance;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize Bass.", ex);
            }
            _syncAudioReachedEnd = OnAudioReachedEnd;
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        //Handle Constructor as singleton class
        private static PlayerEngineCore _instance;
        public static PlayerEngineCore Instance => _instance ?? (_instance = new PlayerEngineCore());

        #endregion

        #region PROPERTIES
        #region PUBLIC PROPERTIES
        #region Volume
        //====================================================================================================//
        /// <summary>Gets or sets the volume of the playback</summary>
        /// <value>
        ///   <para>
        ///     from 0.0f to 1.0f
        ///   </para>
        ///   <para>Default value = 0.5f</para>
        /// </value>
        //====================================================================================================//
        private float _volume;
        public float Volume
        {
            get
            {
                lock (_volumeLock)
                {
                    return _volume;
                }
            }
            set
            {
                lock (_volumeLock)
                {
                    var temp = Math.Max(0.0f, Math.Min(1.0f, value));
                    if (Math.Abs(_volume - temp) < 0.001) return;
                    _volume = temp;
                    if (Bass.ChannelSetAttribute(_streamHandle, ChannelAttribute.Volume, _volume))
                    {
                        OnVolumeChanged();
                    }
                }
            }
        }
        private readonly object _volumeLock = new object();
        #endregion
        #region Audio duration
        private double _audioDurationInSeconds = -1;
        /// <summary>
        /// Gets the duration of the current audio in seconds.
        /// </summary>
        public double AudioDurationInSeconds
        {
            get
            {
                lock (_audioDurationInSecondsLock)
                {
                    return _audioDurationInSeconds;
                }
            }
            private set
            {
                lock (_audioDurationInSecondsLock)
                {
                    if (!(Math.Abs(_audioDurationInSeconds - value) > 0)) return;
                    _audioDurationInSeconds = value;
                    _syncContext.Post(_ => AudioDurationChanged?.Invoke(this, EventArgs.Empty), null);
                }
            }
        }
        private readonly object _audioDurationInSecondsLock = new object();
        #endregion
        #region Audio Position
        /// <summary>
        /// Gets or sets the current position of the audio stream in seconds.
        /// </summary>
        /// <exception cref="Exception">Thrown if setting the audio position fails.</exception>
        public double CurrentPosition
        {
            get
            {
                lock (_currentPositionLock)
                {
                    return Bass.ChannelBytes2Seconds(_streamHandle, Bass.ChannelGetPosition(_streamHandle));
                }
            }
            set
            {
                lock (_currentPositionLock)
                {
                    var length = Bass.ChannelSeconds2Bytes(_streamHandle, value);
                    if (length < 0)
                    {
                        throw new Exception($"Failed to convert seconds to bytes.\n\n{Bass.LastError}");
                    }

                    var lengthInBytes = Bass.ChannelGetLength(_streamHandle);
                    if (lengthInBytes < 0)
                    {
                        throw new Exception($"Failed to get length of audio stream.\n\n{Bass.LastError}");
                    }

                    if (length == lengthInBytes)
                    {
                        length -= 1;
                    }

                    if (Bass.ChannelSetPosition(_streamHandle, length))
                    {
                        OnPositionSet();
                    }
                    else
                    {
                        throw new Exception($"Failed to set position of audio stream.\n\n{Bass.LastError}");
                    }
                }
            }
        }
        private readonly object _currentPositionLock = new object();
        #endregion
        #endregion

        #region PRIVATE PROPERTIES
        private readonly SyncProcedure _syncAudioReachedEnd;
        private readonly SynchronizationContext _syncContext;
        private int _streamHandle;
        private GCHandle _hGcFile;
        #endregion
        #endregion

        #region EVENTS
        /// <summary>Occurs when [playback started].</summary>
        public static event EventHandler AudioStarted;
        /// <summary>Occurs when [playback paused].</summary>
        public static event EventHandler AudioPaused;
        /// <summary>Occurs when [playback resumed].</summary>
        public static event EventHandler AudioResumed;

        public static event EventHandler AudioRestarted;
        /// <summary>Occurs when [playback stopped].</summary>
        public static event EventHandler AudioStopped;
        /// <summary>Occurs when [playback reached end].</summary>
        public static event EventHandler AudioReachedEnd;
        /// <summary>Occurs when [volume changed].</summary>
        public static event EventHandler VolumeChanged;

        /// <summary>Occurs when [audio duration changed].</summary>
        public static event EventHandler AudioDurationChanged;

        public static event EventHandler AudioPositionSet;

        #region Audio Started
        private void OnAudioStarted()
        {
            _syncContext.Post(_ => AudioStarted?.Invoke(this, EventArgs.Empty), null);
        }
        #endregion
        #region Audio Paused
        private void OnAudioPaused()
        {
            _syncContext.Post(_ => AudioPaused?.Invoke(this, EventArgs.Empty), null);
        }
        #endregion
        #region Audio Resumed
        private void OnAudioResumed()
        {
            _syncContext.Post(_ => AudioResumed?.Invoke(this, EventArgs.Empty), null);
        }
        #endregion
        #region Audio Restarted
        private void OnAudioRestarted()
        {
            _syncContext.Post(_ => AudioRestarted?.Invoke(this, EventArgs.Empty), null);
        }
        #endregion
        #region Audio Stopped
        private void OnAudioStopped()
        {
            _syncContext.Post(_ => AudioStopped?.Invoke(this, EventArgs.Empty), null);
        }
        #endregion
        #region Audio Reached End
        private void OnAudioReachedEnd(int handle, int channel, int data, IntPtr user)
        {
            _syncContext.Post(_ => AudioReachedEnd?.Invoke(this, EventArgs.Empty), null);
        }
        #endregion
        #region Volume Changed
        private void OnVolumeChanged()
        {
            _syncContext.Post(_ => VolumeChanged?.Invoke(this, EventArgs.Empty), null);
        }
        #endregion
        #region Position Set
        private void OnPositionSet()
        {
            _syncContext.Post(_ => AudioPositionSet?.Invoke(this, EventArgs.Empty), null);
        }
        #endregion
        #endregion

        #region METHODS
        #region [Start] playback from byte array
        /// <summary>
        /// Starts playback of the provided byte array audio stream.
        /// </summary>
        /// <param name="audio">The byte array containing the audio data.</param>
        /// <exception cref="ArgumentException">Thrown if the audio byte array is null or empty.</exception>
        /// <exception cref="Exception">Thrown if the audio stream fails to initialize or play.</exception>
        public void StartAudio(byte[] audio)
        {
            if (audio == null || audio.Length == 0)
            {
                throw new ArgumentException("The audio byte array is null or empty.");
            }

            try
            {
                if (_streamHandle != 0)
                {
                    ResetStream();
                }

                _hGcFile = GCHandle.Alloc(audio, GCHandleType.Pinned);
                _streamHandle = Bass.CreateStream(_hGcFile.AddrOfPinnedObject(), 0, audio.Length);

                if (_streamHandle == 0)
                {
                    throw new Exception("Failed to create audio stream from byte array." + " (" + Bass.LastError + ")");
                }

                if (Bass.ChannelSetSync(_streamHandle, SyncFlags.End, 0, _syncAudioReachedEnd) == 0)
                {
                    throw new Exception("Failed to synchronize stream with application." + " (" + Bass.LastError + ")");
                }

                if (!Bass.ChannelSetAttribute(_streamHandle, ChannelAttribute.Volume, Volume))
                {
                    throw new Exception("Failed to set volume attribute to the audio stream." + " (" + Bass.LastError + ")");
                }
                if (Bass.ChannelPlay(_streamHandle))
                {
                    // Get the length of the stream in bytes
                    var lengthInBytes = Bass.ChannelGetLength(_streamHandle);
                    if (lengthInBytes == -1)
                    {
                        throw new Exception("Failed to retrieve length of audio." + " (" + Bass.LastError + ")");
                    }
                    // Calculate the length of the stream in seconds
                    var tempAudioDurationInSeconds = Bass.ChannelBytes2Seconds(_streamHandle, lengthInBytes);
                    if (tempAudioDurationInSeconds < 0)
                    {
                        throw new Exception("Failed to convert audio length to it's duration in seconds" + " (" + Bass.LastError + ")");
                    }
                    AudioDurationInSeconds = tempAudioDurationInSeconds;
                    OnAudioStarted();
                }
                else
                {
                    throw new Exception("Failed to start audio stream." + " (" + Bass.LastError + ")");
                }
            }
            catch (Exception)
            {
                ResetStream();
                throw;
            }
            finally
            {
                if (_hGcFile.IsAllocated)
                {
                    _hGcFile.Free();
                }
            }
        }
        #endregion
        #region [RESTART]
        /// <summary>
        /// Restarts playback of the current audio stream from the beginning.
        /// </summary>
        /// <exception cref="Exception">Thrown if the stream fails to restart or mute.</exception>
        public void RestartAudio()
        {
            if (_streamHandle == 0) return;
            if (!Bass.ChannelSlideAttribute(_streamHandle, ChannelAttribute.Volume, 0, 0))
            {
                throw new Exception("Failed mute on start of the audio stream." + " (" + Bass.LastError + ")");
            }
            if (!Bass.ChannelSetPosition(_streamHandle, 0L))
            {
                throw new Exception("Failed to reset audio stream position." + " (" + Bass.LastError + ")");
            }
            if (!Bass.ChannelPlay(_streamHandle))
            {
                throw new Exception("Failed to restart audio stream." + " (" + Bass.LastError + ")");
            }
            if (!Bass.ChannelSlideAttribute(_streamHandle, ChannelAttribute.Volume, Volume, 50))
            {
                throw new Exception("Failed to fade in volume attribute to the audio stream." + " (" + Bass.LastError + ")");
            }
            OnAudioRestarted();
        }
        #endregion
        #region [PAUSE]
        /// <summary>
        /// Pauses the audio playback, fading out the volume over 50ms.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if the volume fade or pause operation fails. If the audio stream is not playing, no exception is thrown.
        /// </exception>
        public void PauseAudio()
        {
            if (_streamHandle == 0) return;
            if (!Bass.ChannelSlideAttribute(_streamHandle, ChannelAttribute.Volume, 0, 50))
            {
                throw new Exception("Failed to fade out volume attribute to the audio stream." + " (" + Bass.LastError + ")");
            }
            if (!Bass.ChannelPause(_streamHandle))
            {
                if (Bass.LastError == Errors.NotPlaying)
                {
                    return;
                }
                throw new Exception("Failed to pause audio stream." + " (" + Bass.LastError + ")");
            }
            OnAudioPaused();
        }
        #endregion
        #region [RESUME]
        /// <summary>
        /// Resumes the audio playback, muting the volume first and then fading it in over 50ms.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if the mute, play, or volume fade-in operation fails.
        /// </exception>
        public void ResumeAudio()
        {
            if (_streamHandle == 0) return;
            if (!Bass.ChannelSlideAttribute(_streamHandle, ChannelAttribute.Volume, 0, 0))
            {
                throw new Exception("Failed mute on start of the audio stream." + " (" + Bass.LastError + ")");
            }
            if (!Bass.ChannelPlay(_streamHandle))
            {
                throw new Exception("Failed to resume audio stream." + " (" + Bass.LastError + ")");
            }
            if (!Bass.ChannelSlideAttribute(_streamHandle, ChannelAttribute.Volume, Volume, 50))
            {
                throw new Exception("Failed to fade in volume attribute to the audio stream." + " (" + Bass.LastError + ")");
            }
            OnAudioResumed();
        }
        #endregion
        #region [STOP]
        /// <summary>
        /// Stops the audio playback, fades out the volume over 50ms, and frees the audio stream resources.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if the volume fade-out, stop, or free stream operation fails.
        /// </exception>
        public void StopAudio()
        {
            if (_hGcFile.IsAllocated)
            {
                _hGcFile.Free();
            }
            if (_streamHandle != 0)
            {
                if (!Bass.ChannelSlideAttribute(_streamHandle, ChannelAttribute.Volume, 0, 50))
                {
                    throw new Exception("Failed to fade in volume attribute to the audio stream." + " (" + Bass.LastError + ")");
                }
                if (!Bass.ChannelStop(_streamHandle))
                {
                    throw new Exception("Failed to stop audio stream." + " (" + Bass.LastError + ")");
                }
                if (!Bass.StreamFree(_streamHandle))
                {
                    throw new Exception("Failed to free audio stream." + " (" + Bass.LastError + ")");
                }
            }
            AudioDurationInSeconds = -1;
            _streamHandle = 0;
            OnAudioStopped();
        }
        #endregion
        #region Reset Stream
        /// <summary>
        /// Resets the current audio stream by fading out the volume, stopping the playback, 
        /// and freeing the resources associated with the stream. The audio duration is also reset.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if the volume fade-out, stop, or free stream operation fails.
        /// </exception>
        public void ResetStream()
        {
            if (_hGcFile.IsAllocated)
            {
                _hGcFile.Free();
            }
            if (_streamHandle != 0)
            {
                if (!Bass.ChannelSlideAttribute(_streamHandle, ChannelAttribute.Volume, 0, 50))
                {
                    throw new Exception("Failed to fade in volume attribute to the audio stream." + " (" + Bass.LastError + ")");
                }
                if (!Bass.ChannelStop(_streamHandle))
                {
                    throw new Exception("Failed to stop audio stream." + " (" + Bass.LastError + ")");
                }
                if (!Bass.StreamFree(_streamHandle))
                {
                    throw new Exception("Failed to free audio stream." + " (" + Bass.LastError + ")");
                }
            }
            AudioDurationInSeconds = -1;
            _streamHandle = 0;
        }
        #endregion
        #endregion
    }
    #endregion
}