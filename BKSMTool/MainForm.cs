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

using BKSMTool.Files.File_Audio;
using BKSMTool.Files.File_WEM;
using BrightIdeasSoftware;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BKSMTool.Files.File_BNK;
using BKSMTool.Miscellaneous;
using static BKSMTool.Miscellaneous.NativeMethods;
using System;
using System.IO;
using System.Text;
using BKSMTool.audio_player_converter;
using BKSMTool.Files;
using BKSMTool.Controls;
using BKSMTool.Controls.ModalForm;

namespace BKSMTool
{
    /// <summary>
    /// MainForm is the primary user interface of the application, providing various functionalities
    /// related to audio file management, including loading, displaying, manipulating, and extracting
    /// audio data from BNK files. It also manages tooltips for the title bar, supports undo/redo operations,
    /// and integrates with an audio player for playback functionalities.
    /// </summary>
    public partial class MainForm : Form, INotifyPropertyChanged
    {
        #region ToolTip for TitleBar

        #region Events

        // Event triggered when the mouse hovers over the non-client area (e.g., the title bar) of the form.
        public event EventHandler NonClientMouseHover;

        // Handles the NonClientMouseHover event. Displays a tooltip showing the full path of the loaded BNK file
        // when the mouse is over the title bar and a BNK file is loaded.
        private void OnNonClientMouseHover(object sender, EventArgs e)
        {
            if (TitleBarRectangle.Contains(Cursor.Position) && IsBnkFileLoaded)
            {
                // Show the tooltip with the full path of the loaded BNK file.
                FormToolTip.Show(
                    _bnkFile.FullPath,
                    this,
                    TitleBarRectangle.Left - this.Left + 1,
                    TitleBarRectangle.Top - this.Top - 2,
                    5000 // Tooltip duration in milliseconds
                );
            }
        }

        // Event triggered when the mouse leaves the non-client area of the form.
        public event EventHandler NonClientMouseLeave;

        // Handles the NonClientMouseLeave event. Hides the tooltip if the mouse leaves the title bar area.
        private void OnNonClientMouseLeave(object sender, EventArgs e)
        {
            if (!TitleBarRectangle.Contains(Cursor.Position))
            {
                // Hide the tooltip when the mouse is no longer over the title bar.
                FormToolTip.Hide(this);
            }
        }

        // Event triggered when the mouse moves over the non-client area of the form.
        public event EventHandler NonClientMouseMove;

        #endregion

        #region Properties

        // Struct used to track mouse events like hover and leave in the non-client area.
        private TrackMouseEventStruct _mouseEventStructTracker;

        // Property that returns the rectangle area of the title bar, used to determine if the mouse is over the title bar.
        public Rectangle TitleBarRectangle => GetTitleBarRectangle(Handle);

        #endregion

        #region Methods

        // Overrides the WndProc method to handle specific Windows messages related to mouse events in the non-client area.
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WmNcMouseMove:
                    // Set up tracking for mouse hover and leave events in the non-client area.
                    _mouseEventStructTracker.HWNdTrack = this.Handle;
                    _mouseEventStructTracker.cbSize = (uint)Marshal.SizeOf(_mouseEventStructTracker);
                    _mouseEventStructTracker.dwFlags = TmeHover | TmeLeave | TmeNonClient;
                    _mouseEventStructTracker.dwHoverTime = 500; // Time in milliseconds before triggering the hover event.
                    _ = TrackMouseEvent(ref _mouseEventStructTracker);

                    // Trigger the NonClientMouseMove event to notify subscribers that the mouse moved in the non-client area.
                    NonClientMouseMove?.Invoke(this, EventArgs.Empty);
                    break;
                case WmNcMouseHover:
                    // Trigger the NonClientMouseHover event to notify subscribers that the mouse is hovering in the non-client area.
                    NonClientMouseHover?.Invoke(this, EventArgs.Empty);
                    break;
                case WmNcMouseLeave:
                    // Trigger the NonClientMouseLeave event to notify subscribers that the mouse has left the non-client area.
                    NonClientMouseLeave?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        #endregion

        #endregion

        #region Local Properties

        //*************************************//
        //********* Local Properties **********//
        //*************************************//

        // Represents the BNK file being interfaced.
        private Bnk _bnkFile;

        // Indicates whether a BNK file is currently being opened.
        private bool _isOpeningBnkFile;
        public bool IsOpeningBnkFile
        {
            get => _isOpeningBnkFile;
            private set
            {
                if (_isOpeningBnkFile == value) return;
                _isOpeningBnkFile = value;
                toolStripMenuItem_Open.Enabled = !value;
                NotifyPropertyChanged(nameof(IsOpeningBnkFile));
            }
        }

        // Indicates whether a BNK file is loaded.
        private bool _isBnkFileLoaded;
        public bool IsBnkFileLoaded
        {
            get => _isBnkFileLoaded;
            set
            {
                if (_isBnkFileLoaded == value) return;
                _isBnkFileLoaded = value;
                toolStripMenuItem_Close.Enabled = value;
                toolStripMenuItem_SaveAs.Enabled = value;

                FileContainsWem = _bnkFile?.WemLibrary.Count > 0;
                olv_AudioListView.ContextMenuStrip = FileContainsWem ? contextMenuStrip_WEMList : null;

                NotifyPropertyChanged(nameof(IsBnkFileLoaded));
            }
        }

        // Indicates whether the BNK file has been saved.
        private bool _isBnkFileSaved = true;
        public bool IsBnkFileSaved
        {
            get => _isBnkFileSaved;
            set
            {
                if (_isBnkFileSaved == value) return;
                _isBnkFileSaved = value;
                toolStripMenuItem_Save.Enabled = !_isBnkFileSaved;
                NotifyPropertyChanged(nameof(IsBnkFileSaved));
            }
        }

        // Name of the application.
        private string _applicationName = "BKSMTool";
        public string ApplicationName
        {
            get => IsBnkFileLoaded ? $"BKSMTool:   {_applicationName}" : "BKSMTool";
            set
            {
                if (_applicationName == value) return;
                _applicationName = value;
                NotifyPropertyChanged(nameof(ApplicationName));
            }
        }

        // Indicates whether the loaded BNK file contains WEM audio files.
        private bool _fileContainsWem;
        public bool FileContainsWem
        {
            get => _fileContainsWem;
            set
            {
                if (_fileContainsWem == value) return;
                _fileContainsWem = value;
                AssignTxTFileToolStripMenuItem.Enabled = value;
                extractAllAudiosToolStripMenuItem.Enabled = value;
                extractAudioToolStripMenuItem.Enabled = value;
                replaceSelectedAudioToolStripMenuItem.Enabled = value;
                NotifyPropertyChanged(nameof(FileContainsWem));
            }
        }

        // Manages audio playback.
        private readonly AudioPlayer _audioPlayerEngine;

        // Task handling audio extraction.
        private Task _extractionAudioTask;
        public Task ExtractionAudioTask
        {
            get => _extractionAudioTask;
            set
            {
                if (_extractionAudioTask == value) return;
                _extractionAudioTask = value;

                // Enable or disable UI elements based on whether the extraction task is running.
                var isTaskRunning = _extractionAudioTask != null;
                SetExtractionTaskUiState(!isTaskRunning);
            }
        }

        private CancellationTokenSource _extractionCancellationTokenSource;

        // Task for replacing audio in the BNK file.
        private Task<byte[]> _replaceAudioTask;
        public Task<byte[]> ReplaceAudioTask
        {
            get => _replaceAudioTask;
            set
            {
                if (_replaceAudioTask == value) return;
                _replaceAudioTask = value;

                // Enable or disable UI elements based on whether the replacement task is running.
                SetReplacementTaskUiState(_replaceAudioTask == null);
            }
        }

        private CancellationTokenSource _replacementCancellationTokenSource;

        // Manages undo/redo operations.
        private readonly UndoRedoManager _undoRedoManager = new UndoRedoManager();

        // Task for assigning event names to WEM audio files.
        private Task _assignEventNamesTask;
        public Task AssignEventNamesTask
        {
            get => _assignEventNamesTask;
            set
            {
                if (_assignEventNamesTask == value) return;
                _assignEventNamesTask = value;

                // Enable or disable UI elements based on whether the assignment task is running.
                SetAssignEventNamesTaskUiState(_assignEventNamesTask == null);
            }
        }

        private CancellationTokenSource _assignEventNamesCancellationTokenSource;

        #region Event on Property Changed

        // Event triggered when a property value changes.
        public event PropertyChangedEventHandler PropertyChanged;

        // Notifies listeners that a property value has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Private Helper Methods

        // Sets the UI state based on the extraction task's state.
        private void SetExtractionTaskUiState(bool enabled)
        {
            replaceSelectedAudioToolStripMenuItem.Enabled = enabled;
            toolStripMenuItem_BNKReplaceAudio.Enabled = enabled;
            extractAudioToolStripMenuItem.Enabled = enabled;
            extractAllAudiosToolStripMenuItem.Enabled = enabled;
            toolStripMenuItem_BNKExtractAudio.Enabled = enabled;
            toolStripMenuItem_BNKExtractAllAudio.Enabled = enabled;
            AssignTxTFileToolStripMenuItem.Enabled = enabled;
        }

        // Sets the UI state based on the replacement task's state.
        private void SetReplacementTaskUiState(bool enabled)
        {
            replaceSelectedAudioToolStripMenuItem.Enabled = enabled;
            toolStripMenuItem_BNKReplaceAudio.Enabled = enabled;
            extractAudioToolStripMenuItem.Enabled = enabled;
            extractAllAudiosToolStripMenuItem.Enabled = enabled;
            toolStripMenuItem_BNKExtractAudio.Enabled = enabled;
            toolStripMenuItem_BNKExtractAllAudio.Enabled = enabled;
            AssignTxTFileToolStripMenuItem.Enabled = enabled;
            toolStripMenuItem_SaveAs.Enabled = enabled && !IsBnkFileSaved;
            toolStripMenuItem_Save.Enabled = enabled && !IsBnkFileSaved;
        }

        // Sets the UI state based on the assign event names task's state.
        private void SetAssignEventNamesTaskUiState(bool enabled)
        {
            replaceSelectedAudioToolStripMenuItem.Enabled = enabled;
            toolStripMenuItem_BNKReplaceAudio.Enabled = enabled;
            extractAudioToolStripMenuItem.Enabled = enabled;
            extractAllAudiosToolStripMenuItem.Enabled = enabled;
            toolStripMenuItem_BNKExtractAudio.Enabled = enabled;
            toolStripMenuItem_BNKExtractAllAudio.Enabled = enabled;
            AssignTxTFileToolStripMenuItem.Enabled = enabled;
        }

        #endregion

        #endregion

        #region Constructor
        //*********************************//
        //********** Constructor **********//
        //*********************************//

        public MainForm()
        {
            // Initialize the form without setting an active control.
            this.ActiveControl = null;

            // Initialize all UI components.
            InitializeComponent();

            // Instantiate or retrieve the existing instance of the audio player engine.
            _audioPlayerEngine = AudioPlayer.Instance;

            // Disable the use of Application.Idle in ObjectListView to improve performance.
            olv_AudioListView.CanUseApplicationIdle = false;

            // Subscribe to non-client area mouse events for showing tooltips.
            NonClientMouseHover += OnNonClientMouseHover;
            NonClientMouseLeave += OnNonClientMouseLeave;

            // Bind the form's Text and Name properties to the ApplicationName property.
            DataBindings.Add(nameof(Text), this, nameof(ApplicationName));
            DataBindings.Add(nameof(Name), this, nameof(ApplicationName));

            // Bind the visibility of panel1 to the FileContainsWem property.
            panel1.DataBindings.Add(nameof(Visible), this, nameof(FileContainsWem));

            // Subscribe to audio player state and mode change events.
            AudioPlayer.PlayerStateChanged += PlayerEngine_Event_PlaybackStateChanged;
            AudioPlayer.PlayerModeChanged += PlayerEngine_Event_ModeChanged;

            _audioPlayerEngine.Mode = Enum.TryParse(Properties.Settings.Default.PlayerMode,out PlayerMode mode) ? mode : PlayerMode.NoLoop;
            _audioPlayerEngine.Volume = Properties.Settings.Default.Volume;

            // Set focus to the audio list view after initialization.
            olv_AudioListView.Select();

            // Add PreviewKeyDown and KeyPress event handlers to all controls within the form.
            AddHandlersToAllControls(this);
        }

        #endregion

        #region Controls Events
        //*********************************************//
        //************** Control Events ***************//
        //*********************************************//

        #region Open bank audio file
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Open bank audio file" button.
        /// Opens a new BNK file using an OpenFileDialog.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private async void OpenBankAudioFile_Event_Click(object sender, EventArgs e)
        {
            await OpenNewFileFromOpenFileDialog();
        }
        #endregion

        #region Close bank audio file
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Close bank audio file" button.
        /// Closes the current BNK file asynchronously and triggers garbage collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private async void CloseBankAudioFile_Event_Click(object sender, EventArgs e)
        {
            if (await CloseFileAsync())
            {
                GC.Collect();
            }
        }
        #endregion

        #region Exit App
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Exit" button.
        /// Exits the application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private async void ExitApp_Event_Click(object sender, EventArgs e)
        {
            await Exit();
        }
        #endregion

        #region Save
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Save" button.
        /// Saves the current BNK file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private async void Save_Event_Click(object sender, EventArgs e)
        {
            await Save();
        }
        #endregion

        #region Save As
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Save As" button.
        /// Saves the current BNK file with a new name or location.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private async void SaveAs_Event_Click(object sender, EventArgs e)
        {
            await SaveAs();
        }
        #endregion

        #region Assign file to bank audio
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Assign file to bank audio" button.
        /// Assigns a file to the BNK audio asynchronously.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private async void Assign_Event_Click(object sender, EventArgs e)
        {
            await AssignFile();
        }
        #endregion

        #region Extract Selected Audio
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Extract Selected Audio" button.
        /// Extracts the selected audio from the BNK file asynchronously.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private async void ExtractSelectedAudio_Event_Click(object sender, EventArgs e)
        {
            await ExtractAudio();
        }
        #endregion

        #region Extract All Audios
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Extract All Audios" button.
        /// Extracts all audios from the BNK file asynchronously.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private async void ExtractAllAudios_Event_Click(object sender, EventArgs e)
        {
            await ExtractAllAudios();
        }
        #endregion

        #region Replace Audio
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Replace Audio" button.
        /// Replaces the selected audio in the BNK file asynchronously.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private async void ReplaceSelectedAudio_Event_Click(object sender, EventArgs e)
        {
            await ReplaceAudio();
        }
        #endregion

        #region Undo
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Undo" button.
        /// Reverts the last action performed in the application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void Undo_Event_Click(object sender, EventArgs e)
        {
            Undo();
        }
        #endregion

        #region Redo
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Redo" button.
        /// Re-applies the last undone action in the application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void Redo_Event_Click(object sender, EventArgs e)
        {
            Redo();
        }
        #endregion

        #region Filter Box
        #region KeyDown
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "KeyDown" event of the FilterBox.
        /// Handles key presses to manage filtering in the audio list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void FilterBox_Event_KeyDown(object sender, KeyEventArgs e)
        {
            FilterBoxKeyDown(e);
        }
        #endregion
        #region TextChanged
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "TextChanged" event of the FilterBox.
        /// Updates the filter criteria as the user types in the FilterBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void FilterBox_Event_TextChanged(object sender, EventArgs e)
        {
            FilterBox_TextChanged();
        }
        #endregion
        #endregion

        #region ListView
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "FormatRow" event of the AudioListView.
        /// Formats rows in the AudioListView based on specific criteria.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void AudioListView_Event_FormatRow(object sender, FormatRowEventArgs e)
        {
            AudioListView_FormatRow(e);
        }

        //====================================================================================================//
        /// <summary>
        /// Event handler for the "ItemsChanged" event of the AudioListView.
        /// Responds to changes in the items of the AudioListView.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void AudioListView_Event_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            AudioListView_ItemChanged();
        }

        //====================================================================================================//
        /// <summary>
        /// Event handler for the "SelectionChanged" event of the AudioListView.
        /// Handles changes in the selected items of the AudioListView.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void AudioListView_Event_SelectionChanged(object sender, EventArgs e)
        {
            AudioListView_SelectionChanged(sender);
        }

        //====================================================================================================//
        /// <summary>
        /// Event handler for the "ItemActivate" event of the AudioListView.
        /// Handles double-click or enter key actions on items in the AudioListView.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void AudioListView_Event_ItemActivate(object sender, EventArgs e)
        {
            AudioListView_ItemActivate();
        }

        //====================================================================================================//
        /// <summary>
        /// Event handler for the "ButtonClick" event of the AudioListView.
        /// Handles button clicks within the AudioListView.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void AudioListView_Event_ButtonClick(object sender, CellClickEventArgs e)
        {
            AudioListView_ButtonClick(e);
        }

        //====================================================================================================//
        /// <summary>
        /// Event handler for the "KeyDown" event of the AudioListView.
        /// Handles key presses while the AudioListView has focus.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="keys">The event data.</param>
        //====================================================================================================//
        private void AudioListView_Event_KeyDown(object sender, KeyEventArgs keys)
        {
            AudioListView_KeyDown(sender, keys);
        }
        #endregion

        #region Player Events
        #region Mode changed
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "ModeChanged" event of the PlayerEngine.
        /// Responds to changes in the audio player's mode (e.g., loop, shuffle).
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void PlayerEngine_Event_ModeChanged(object sender, EventArgs e)
        {
            Player_ModeChanged();
        }
        #endregion
        #region State changed
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "PlaybackStateChanged" event of the PlayerEngine.
        /// Responds to changes in the playback state of the audio player (e.g., play, pause, stop).
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void PlayerEngine_Event_PlaybackStateChanged(object sender, EventArgs e)
        {
            Player_PlaybackStateChanged();
        }
        #endregion
        #endregion

        #region Player controls event
        #region Mode button
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Mode" button of the PlayerEngine.
        /// Toggles the playback mode of the audio player.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void PlayerMode_Event_Click(object sender, EventArgs e)
        {
            PlayerMode_Click();
        }
        #endregion
        #region Previous audio button
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Previous audio" button of the PlayerEngine.
        /// Skips to the previous audio track in the playlist.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void PlayerPreviousAudio_Event_Click(object sender, EventArgs e)
        {
            PlayerPreviousAudio_Click();
        }
        #endregion
        #region Next audio button
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Next audio" button of the PlayerEngine.
        /// Skips to the next audio track in the playlist.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void PlayerNextAudio_Event_Click(object sender, EventArgs e)
        {
            PlayerNextAudio_Click();
        }
        #endregion
        #region Play/Pause button
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "Play/Pause" button of the PlayerEngine.
        /// Toggles playback between play and pause.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void PlayerPlayPause_Event_Click(object sender, EventArgs e)
        {
            PlayerButtonPlayPause_Click();
        }
        #endregion
        #endregion

        #region PreviewKeyDown
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "PreviewKeyDown" event of the MainForm and its controls.
        /// Handles special key presses before the standard key processing occurs.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            HandlePreviewKeyDown(e);
        }
        #endregion

        #region KeyPress
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "KeyPress" event of the MainForm and its controls.
        /// Handles standard key presses.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKeyPress(e);
        }
        #endregion

        #region DragDrop
        //====================================================================================================//
        /// <summary>
        /// Event handler for the "DragEnter" event of the MainForm.
        /// Checks if the dragged data is a file and sets the drag-and-drop effect accordingly.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        //====================================================================================================//
        /// <summary>
        /// Event handler for the "DragDrop" event of the MainForm.
        /// Opens a new BNK file if a valid file is dropped onto the form.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private async void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 1)
            {
                await OpenNewFileFromDragDrop(files[0]);
            }
            else
            {
                MessageBox.Show(@"Application only supports one file at a time.", @"Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        #endregion

        #region Check for Update
        private async void CheckForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await VersionChecker.CheckAvailableUpdate();
        }
        #endregion

        #region About
        private void AboutBKSMToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog();
            }
        }
        #endregion

        #region Wiki
        private void wikiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Saltwatersam/BKSMTool/wiki");
        }
        #endregion

        #endregion

        #region Methods
        //*************************************//
        //************** Methods **************//
        //*************************************//

        #region Open file from Open File Dialog
        //====================================================================================================//
        /// <summary>
        /// Opens a new BNK file selected by the user using an OpenFileDialog.
        /// </summary>
        //====================================================================================================//
        private async Task OpenNewFileFromOpenFileDialog()
        {
            string fileToOpen;

            #region Open File Dialog
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = @"Select Bank Audio File";
                openFileDialog.Filter = @"Bank audio (*.bnk)|*.bnk";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = false;

                // Show the OpenFileDialog.
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileToOpen = openFileDialog.FileName;
                }
                else
                {
                    return; // User canceled the dialog.
                }
            }
            #endregion

            await OpenNewFile(fileToOpen);
        }
        #endregion

        #region Open file from DragDrop
        //====================================================================================================//
        /// <summary>
        /// Opens a new BNK file that was dragged and dropped onto the form.
        /// </summary>
        /// <param name="newFilePath">The path of the file dropped by the user.</param>
        //====================================================================================================//
        private async Task OpenNewFileFromDragDrop(string newFilePath)
        {
            await OpenNewFile(newFilePath);
        }
        #endregion

        #region Open new file
        //====================================================================================================//
        /// <summary>
        /// Handles the process of opening a new BNK file, including parsing, gathering WEM audios, 
        /// converting them, assigning event names, and displaying the data.
        /// </summary>
        /// <param name="newFilePath">The path of the new file to open.</param>
        //====================================================================================================//
        private async Task OpenNewFile(string newFilePath)
        {
            FileStream fileStream;
            string fileNameToOpen;

            switch (IsBnkFileLoaded)
            {
                // If the file is already opened, do nothing.
                case true when newFilePath == _bnkFile.FullPath:
                    toolStrip_LabelForStatus.Text = $@"File ""{_bnkFile.Name}"" is already open.";
                    FlashWindow(this.Handle);
                    return;

                // If another file is open, try to close it before opening a new one.
                case true when await CloseFileAsync() == false:
                    FlashWindow(this.Handle);
                    return;
            }

            if (string.IsNullOrEmpty(newFilePath))
            {
                IsOpeningBnkFile = false;
                HideAndResetProgressBar();
                toolStrip_LabelForStatus.Text = @"No file selected for open.";
                MessageBox.Show(@"No file selected for open.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Log(@"No file selected for open.");
                return;
            }
            else
            {
                fileNameToOpen = Path.GetFileName(newFilePath);
                IsOpeningBnkFile = true;
                Logger.Log($"Opening file \"{newFilePath}\"...");
            }

            #region (Step 1) Create a FileStream to handle the file
            try
            {
                toolStrip_LabelForStatus.Text = $@"Opening file ""{fileNameToOpen}""...";
                Logger.Log($"Opening file \"{newFilePath}\"...");
                fileStream = FileOperations.StartFileStream(newFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                Logger.Log($"FileStream created for \"{newFilePath}\".");
            }
            catch (Exception ex)
            {
                toolStrip_LabelForStatus.Text = $@"Error while opening ""{fileNameToOpen}"". File was ignored.";
                MessageBox.Show(
                    new StringBuilder().Append("Error while opening \"")
                        .Append(fileNameToOpen)
                        .Append("\": \n\n")
                        .Append(ex.Message)
                        .Append("\n\nBank audio file will be ignored.")
                        .ToString(),
                    @"Opening Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Log($"Error while opening \"{newFilePath}\": \n\n{ex.Message}\n{ex.StackTrace}\n\nBank audio file ignored.");
                IsOpeningBnkFile = false;
                HideAndResetProgressBar();
                return;
            }
            #endregion

            #region (Step 2) Parse BNK file
            try
            {
                toolStrip_LabelForStatus.Text = $@"Parsing ""{fileNameToOpen}""...";
                Logger.Log($"Parsing \"{newFilePath}\"...");
                _bnkFile = await BnkFileHandler.PerformParsing(fileStream);
                LogChunksFound(_bnkFile, newFilePath);
            }
            catch (Exception ex)
            {
                toolStrip_LabelForStatus.Text = $@"Error while parsing ""{fileNameToOpen}"". File was ignored.";
                MessageBox.Show(
                    new StringBuilder().Append("Error while parsing \"")
                        .Append(fileNameToOpen)
                        .Append("\": \n\n")
                        .Append(ex.Message)
                        .Append("\n\nBank audio file will be ignored.")
                        .ToString(),
                    @"Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Log($"Error while parsing \"{newFilePath}\": \n\n{ex.Message}\n{ex.StackTrace}\n\nBank audio file ignored.");
                IsOpeningBnkFile = false;
                HideAndResetProgressBar();
                return;
            }
            #endregion

            #region Check if the BNK file is valid
            if (_bnkFile == null)
            {
                DisplayFileError($"The BNK file \"{fileNameToOpen}\" is invalid.");
                toolStrip_LabelForStatus.Text = $@"Error while parsing ""{fileNameToOpen}"". File was ignored.";
                IsOpeningBnkFile = false;
                return;
            }

            if (_bnkFile.DidxChunk?.WemFilesInfo.Count == 0)
            {
                DisplayFileError($"The BNK file \"{fileNameToOpen}\" does not reference any audio files.");
                toolStrip_LabelForStatus.Text = $@"Error while parsing ""{fileNameToOpen}"". File was ignored.";
                IsOpeningBnkFile = false;
                return;
            }

            if (_bnkFile.DataChunk?.WemFiles == null)
            {
                DisplayFileError($"The BNK file \"{fileNameToOpen}\" does not contain any audio data.");
                toolStrip_LabelForStatus.Text = $@"Error while parsing ""{fileNameToOpen}"". File was ignored.";
                IsOpeningBnkFile = false;
                return;
            }
            #endregion

            #region (Step 3) Gather WEM audios
            try
            {
                if (_bnkFile.DidxChunk?.WemFilesInfo.Count > 0)
                {
                    InitializeProgressBar(_bnkFile.DidxChunk.WemFilesInfo.Count * 3, true);
                }

                var progressQueue = new ProgressQueue<int>(i => toolStrip_ProgressBar.PerformStep());
                var progress = new Progress<int>(i => progressQueue.Report(i));

                toolStrip_LabelForStatus.Text = $@"Gathering WEM audios of ""{fileNameToOpen}""...";
                Logger.Log($"Gathering WEM audios of \"{newFilePath}\"...");
                await BnkFileHandler.GatherWem(_bnkFile, progress);
                await progressQueue.WaitForQueueToEmptyAsync();
                await Task.Delay(150);
                Logger.Log($"WEM audios of \"{newFilePath}\" gathered. Audio found: {_bnkFile.WemLibrary.Count}");
            }
            catch (Exception ex)
            {
                HandleWemAudioError(ex, fileNameToOpen, newFilePath);
                return;
            }
            #endregion

            #region (Step 4) Convert WEM to playable audio
            try
            {
                var progressQueue = new ProgressQueue<int>(i => toolStrip_ProgressBar.PerformStep());
                var progress = new Progress<int>(i => progressQueue.Report(i));

                toolStrip_LabelForStatus.Text = $@"Converting WEM audios of ""{fileNameToOpen}""...";
                Logger.Log($"Converting WEM audios of \"{newFilePath}\"...");
                await BnkFileHandler.ConvertWemToStandardAudio(_bnkFile, progress);
                await progressQueue.WaitForQueueToEmptyAsync();
                await Task.Delay(150);
                Logger.Log($"WEM audios of \"{newFilePath}\" converted.");
            }
            catch (Exception ex)
            {
                HandleWemAudioError(ex, fileNameToOpen, newFilePath);
                return;
            }
            #endregion

            #region (Step 5) Automatic assignment of event names to audios
            try
            {
                var progressQueue = new ProgressQueue<int>(i => toolStrip_ProgressBar.PerformStep());
                var progress = new Progress<int>(i => progressQueue.Report(i));

                var eventNameFile = Path.Combine(Path.GetDirectoryName(newFilePath) ?? string.Empty,
                    Path.GetFileNameWithoutExtension(newFilePath) + ".txt");
                toolStrip_LabelForStatus.Text = $@"Assigning event names to audios of ""{fileNameToOpen}""...";
                Logger.Log($"Assigning event names to audios of \"{newFilePath}\"...");
                await BnkFileHandler.AssignEventNames(_bnkFile.WemLibrary, eventNameFile, CancellationToken.None, progress);
                await progressQueue.WaitForQueueToEmptyAsync();
                await Task.Delay(150);
                Logger.Log($"Event names assigned to audios of \"{newFilePath}\".");
            }
            catch (Exception ex)
            {
                Logger.Log($"Error during automatic assignment of event names: {ex}");
                // Continue despite the error as it's not critical
            }
            #endregion

            #region (Step 6) Display the file
            try
            {
                toolStrip_LabelForStatus.Text = $@"Displaying data of ""{fileNameToOpen}""...";
                Logger.Log($"Displaying data of \"{newFilePath}\"...");
                DisplayData();
                await Task.Delay(150);
                Logger.Log($"Data of \"{newFilePath}\" displayed.");
                toolStrip_LabelForStatus.Text = $@"""{_bnkFile.Name}"" successfully loaded.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    new StringBuilder().Append("Error while displaying \"")
                        .Append(_bnkFile.Name)
                        .Append("\": \n\"Missing or broken interface.\"\n\nBank audio file will be ignored.")
                        .ToString(),
                    @"Display Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStrip_LabelForStatus.Text = $@"Error while displaying data of ""{_bnkFile.Name}"". Bank audio file was ignored.";
                Logger.Log($"Error while displaying data of \"{newFilePath}\".\n\n{ex.Message}\n{ex.StackTrace}");
                _bnkFile?.Dispose();
                return;
            }
            #endregion

            HideAndResetProgressBar();
            IsOpeningBnkFile = false;
        }
        #endregion

        #region Display data in form
        //====================================================================================================//
        /// <summary>
        /// Displays the data of the opened BNK file in the form.
        /// </summary>
        //====================================================================================================//
        private void DisplayData()
        {
            olv_AudioListView.SetObjects(_bnkFile.AudioLibrary);

            _bnkFile.PropertyChanged += OnBnkFilePropertyChanged;
            OnBnkFilePropertyChanged(this, new PropertyChangedEventArgs(nameof(_bnkFile.ShortenedPath)));
            OnBnkFilePropertyChanged(this, new PropertyChangedEventArgs(nameof(_bnkFile.IsSaved)));
            if (DoesContainEventName())
            {
                ShowEventNames();
            }
            else
            {
                HideEventNames();
            }
            IsBnkFileLoaded = true;

            UpdateAudioListViewSelection();
        }
        #endregion

        #region Helper Methods
        //====================================================================================================//
        /// <summary>
        /// Logs the chunks found in the BNK file.
        /// </summary>
        //====================================================================================================//
        private static void LogChunksFound(Bnk bnkFile, string filePath)
        {
            var logString = $"File \"{filePath}\" parsed. Chunks found:";
            if (bnkFile.BkhdChunk != null) logString += "\n- \"BKHD\"";
            if (bnkFile.DataChunk != null) logString += "\n- \"DATA\"";
            if (bnkFile.DidxChunk != null) logString += "\n- \"DIDX\"";
            if (bnkFile.EnvsChunk != null) logString += "\n- \"ENVS\"";
            if (bnkFile.FxprChunk != null) logString += "\n- \"FXPR\"";
            if (bnkFile.HircChunk != null) logString += "\n- \"HIRC\"";
            if (bnkFile.InitChunk != null) logString += "\n- \"INIT\"";
            if (bnkFile.PlatChunk != null) logString += "\n- \"PLAT\"";
            if (bnkFile.StidChunk != null) logString += "\n- \"STID\"";
            if (bnkFile.StmgChunk != null) logString += "\n- \"STMG\"";
            Logger.Log(logString);
        }
        //====================================================================================================//
        /// <summary>
        /// Displays an error message for an invalid or incomplete BNK file.
        /// </summary>
        //====================================================================================================//
        private static void DisplayFileError(string message)
        {
            MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Logger.Log($"{message}\n\nBank audio file ignored.");
        }

        //====================================================================================================//
        /// <summary>
        /// Handles errors related to WEM audio gathering or conversion.
        /// </summary>
        //====================================================================================================//
        private void HandleWemAudioError(Exception ex, string fileName, string filePath)
        {
            toolStrip_LabelForStatus.Text = $@"Error while processing WEM audios of ""{fileName}"". Bank audio file was ignored.";
            MessageBox.Show(
                new StringBuilder().Append("Error while processing WEM audios of \"")
                    .Append(fileName)
                    .Append("\": \n\n")
                    .Append(ex.Message)
                    .Append("\n\nBank audio file will be ignored.")
                    .ToString(),
                @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Logger.Log($"Error while processing WEM audios of \"{filePath}\".\n\n{ex.Message}\n{ex.StackTrace}\n\nBank audio file ignored.");
            IsOpeningBnkFile = false;
            _bnkFile?.Dispose();
            HideAndResetProgressBar();
        }
        #endregion

        #region BNK File Property Changed
        //====================================================================================================//
        /// <summary>
        /// Handles the PropertyChanged event of the BNK file.
        /// Updates the application name or save status based on the property that changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        //====================================================================================================//
        private void OnBnkFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Bnk.ShortenedPath):
                    ApplicationName = _bnkFile.ShortenedPath;
                    break;
                case nameof(Bnk.IsSaved):
                    IsBnkFileSaved = _bnkFile.IsSaved;
                    break;
            }
        }
        #endregion

        #region Check if BNK File Contains Event Names
        //====================================================================================================//
        /// <summary>
        /// Checks if the BNK file contains any event names.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the BNK file has some event names assigned to it, <c>false</c> otherwise.
        /// </returns>
        //====================================================================================================//
        private bool DoesContainEventName()
        {
            return _bnkFile.WemLibrary.Any(wem => wem.EventName != null);
        }
        #endregion

        #region Hide Event Names
        //====================================================================================================//
        /// <summary>
        /// Hides the audio event names and adjusts other columns in the <see cref="DataGridView"/>.
        /// </summary>
        //====================================================================================================//
        public void HideEventNames()
        {
            OLVC_RelatedName.IsVisible = false;
            OLVC_ID.MaximumWidth = -1;
            OLVC_ID.MinimumWidth = -1;
            OLVC_ID.TextAlign = HorizontalAlignment.Left;
            OLVC_ID.FillsFreeSpace = true;
            olv_AudioListView.RebuildColumns();
        }
        #endregion

        #region Show Event Names
        //====================================================================================================//
        /// <summary>
        /// Shows the audio event names and adjusts other columns in the <see cref="DataGridView"/>.
        /// </summary>
        //====================================================================================================//
        public void ShowEventNames()
        {
            OLVC_RelatedName.IsVisible = true;
            OLVC_ID.MaximumWidth = 80;
            OLVC_ID.MinimumWidth = 80;
            OLVC_ID.TextAlign = HorizontalAlignment.Center;
            OLVC_ID.FillsFreeSpace = false;
            olv_AudioListView.RebuildColumns();
        }
        #endregion

        #region Hide and Reset Progress Bar
        //====================================================================================================//
        /// <summary>
        /// Hides and resets the progress bar to its initial state.
        /// </summary>
        //====================================================================================================//
        private void HideAndResetProgressBar()
        {
            toolStrip_ProgressBar.Visible = false;
            toolStrip_ProgressBar.Value = 0;
            toolStrip_ProgressBar.Maximum = 1;
            toolStrip_ProgressBar.Minimum = 0;
            toolStrip_ProgressBar.Step = 1;
        }
        #endregion

        #region Initialize Progress Bar
        //====================================================================================================//
        /// <summary>
        /// Initializes the progress bar with the specified maximum value and visibility.
        /// </summary>
        /// <param name="maximum">The maximum value of the progress bar.</param>
        /// <param name="visible">Indicates whether the progress bar should be visible.</param>
        //====================================================================================================//
        private void InitializeProgressBar(int maximum, bool visible)
        {
            toolStrip_ProgressBar.Value = 0;
            toolStrip_ProgressBar.Minimum = 0;
            toolStrip_ProgressBar.Step = 1;
            toolStrip_ProgressBar.Maximum = maximum;
            toolStrip_ProgressBar.Visible = visible;
        }
        #endregion

        #region Close
        //====================================================================================================//
        /// <summary>Closes the selected BNK file.</summary>
        /// <param name="askToSave">if set to <c>true</c> [ask the user to save].</param>
        //====================================================================================================//
        public async Task<bool> CloseFileAsync(bool askToSave = true)
        {
            if (!IsBnkFileLoaded) return true;
            var fileName = _bnkFile.Name;
            var filePath = _bnkFile.FullPath;

            #region Handle ongoing extractions
            if (_extractionAudioTask != null && !_extractionAudioTask.IsCompleted)
            {
                var dr =
                    MessageBox.Show(
                        new StringBuilder()
                            .Append("Extraction of audio(s) is ongoing\n\nDo you want to cancel extraction ?")
                            .ToString(),
                        @"Cancelling Extraction?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch (dr)
                {
                    case DialogResult.Yes:
                        try
                        {
                            Logger.Log("Cancelling audio extraction in order to close file...");
                            _extractionCancellationTokenSource?.Cancel();
                            toolStrip_LabelForStatus.Text = @"Cancelling audio extraction...";
                            await _extractionAudioTask;
                            Logger.Log("Audio extraction successfully cancelled.");
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(
                                $"Error while cancelling audio extraction in order to close file \"{filePath}\":\n\n{ex.Message}\n{ex.StackTrace}");
                        }

                        break;
                    case DialogResult.Cancel:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            #endregion

            #region Handle ongoing replacement
            if (_replaceAudioTask != null && !_replaceAudioTask.IsCompleted)
            {
                var dr =
                    MessageBox.Show(
                        new StringBuilder()
                            .Append("Replacement of audio is ongoing\n\nDo you want to cancel replacement ?")
                            .ToString(),
                        @"Cancelling Replacement?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch (dr)
                {
                    case DialogResult.Yes:
                        try
                        {
                            Logger.Log("Cancelling audio replacement in order to close file...");
                            _replacementCancellationTokenSource?.Cancel();
                            toolStrip_LabelForStatus.Text = @"Cancelling audio replacement...";
                            await _replaceAudioTask;
                            Logger.Log("Audio replacement successfully cancelled.");
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(
                                $"Error while cancelling audio replacement in order to close file \"{filePath}\":\n\n{ex.Message}\n{ex.StackTrace}");
                        }

                        break;
                    case DialogResult.Cancel:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            #endregion

            #region Handle ongoing assigning event names
            if (_assignEventNamesTask != null && !_assignEventNamesTask.IsCompleted)
            {
                var dr =
                    MessageBox.Show(
                        new StringBuilder()
                            .Append(
                                "Assigning event names to audio is ongoing\n\nDo you want to cancel assigning event names ?")
                            .ToString(),
                        @"Cancelling Assigning Event Names?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch (dr)
                {
                    case DialogResult.Yes:
                        try
                        {
                            Logger.Log("Cancelling assigning event names in order to close file...");
                            _assignEventNamesCancellationTokenSource?.Cancel();
                            toolStrip_LabelForStatus.Text = @"Cancelling assigning event names...";
                            await _assignEventNamesTask;
                            Logger.Log("Assigning event names successfully cancelled.");
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(
                                $"Error while cancelling assigning event names in order to close file \"{filePath}\":\n\n{ex.Message}\n{ex.StackTrace}");
                        }

                        break;
                    case DialogResult.Cancel:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            #endregion

            #region unsaved changes
            if (!_bnkFile.IsSaved) //if file is not saved :
            {
                if (askToSave)
                {
                    var dr = MessageBox.Show(@"Do you want to save changes?", $@"Save {fileName} ?",
                        MessageBoxButtons.YesNoCancel);

                    switch (dr)
                    {
                        //ask the user to save changes made
                        case DialogResult.Yes:
                            Logger.Log($"Saving file \"{filePath}\" before closing...");
                            //save if the user agree
                            await Save();
                            break;
                        case DialogResult.Abort:
                            //abort the action if the user abort
                            return false;
                        case DialogResult.No:
                            //do nothing if the user don't want to save
                            break;
                        case DialogResult.Cancel:
                            //cancel the action if the user cancel
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            #endregion

            Logger.Log($"Closing \"{filePath}\"...");
            if (FileContainsWem && _audioPlayerEngine.PlayerState == PlaybackState.Playing ||
                _audioPlayerEngine.PlayerState == PlaybackState.Paused)
            {
                _audioPlayerEngine.Stop();
            }

            olv_AudioListView.ClearObjects();
            // Clear undo/redo stacks
            _undoRedoManager.Clear();
            UpdateUndoRedoButtons();
            _bnkFile.PropertyChanged -= OnBnkFilePropertyChanged;
            foreach (var audioFile in _bnkFile.AudioLibrary)
            {
                audioFile.Dispose();
            }

            _bnkFile.AudioLibrary.Clear();
            _bnkFile.AudioLibrary = null;
            _bnkFile.Dispose();
            _bnkFile = null;
            IsBnkFileLoaded = false;
            toolStrip_LabelForStatus.Text = $@"""{fileName}"" has been closed.";
            txtbox_FilterBox.Text = "";
            Logger.Log($"\"{filePath}\" has been closed.");

            return true;
        }
        #endregion
        #region Exit/Close Main software
        //====================================================================================================//
        /// <summary>Stop the program</summary>
        //====================================================================================================//
        private async Task Exit()
        {
            await CloseFileAsync();
            Properties.Settings.Default.PlayerMode = _audioPlayerEngine.Mode.ToString();
            Properties.Settings.Default.Volume = _audioPlayerEngine.Volume;
            Properties.Settings.Default.Save();
            Application.Exit();
        }
        #endregion

        #region Save
        //====================================================================================================//
        /// <summary>
        /// Saves any changes made to the currently loaded BNK file (<see cref="_bnkFile" />).
        /// </summary>
        //====================================================================================================//
        public async Task Save()
        {
            if (_bnkFile == null)
            {
                Logger.Log("No file is currently loaded to save.");
                toolStrip_LabelForStatus.Text = @"No file is currently loaded to save.";
                return;
            }

            try
            {
                // Log and perform the save operation
                Logger.Log($"Saving \"{_bnkFile.FullPath}\"...");
                await BnkFileHandler.SaveChanges(_bnkFile);

                // Update status and log success
                toolStrip_LabelForStatus.Text = $@"""{_bnkFile.Name}"" was successfully saved.";
                Logger.Log($"\"{_bnkFile.FullPath}\" was successfully saved.");
            }
            catch (Exception ex)
            {
                // Handle and log any errors that occur during the save operation
                toolStrip_LabelForStatus.Text = $@"Error while trying to save ""{_bnkFile.Name}"".";
                MessageBox.Show(
                    $@"Error while trying to save ""{_bnkFile.Name}"":

                    {ex.Message}",
                    _bnkFile.Name,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log($"Error while trying to save \"{_bnkFile.FullPath}\":\n{ex.Message}\n{ex.StackTrace}");
            }
        }
        #endregion

        #region SaveAs
        //====================================================================================================//
        /// <summary>
        /// Saves the currently loaded BNK file as a new file and closes the original BNK file without changes.
        /// </summary>
        //====================================================================================================//
        private async Task SaveAs()
        {
            var originalFilePath = _bnkFile.FullPath;
            var originalFileName = _bnkFile.Name;
            var newFilePath = string.Empty;

            try
            {
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Title = @"Save Bank Audio As...";
                    saveFileDialog.FileName = originalFileName;
                    saveFileDialog.Filter = @"Bank audio (*.bnk)|*.bnk";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.OverwritePrompt = true;

                    // Show the dialog and get the new file path
                    if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
                    newFilePath = saveFileDialog.FileName;

                    // Create a new file stream for the new file
                    var newFileStream = FileOperations.StartFileStream(newFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

                    // Close the old file stream and update the BNK file properties
                    _bnkFile.FileStream.Close();
                    _bnkFile.FileStream = newFileStream;
                    _bnkFile.FullPath = newFilePath;

                    // Save the BNK file to the new location
                    Logger.Log($"Saving \"{originalFilePath}\" as \"{newFilePath}\"...");
                    await BnkFileHandler.SaveChanges(_bnkFile);

                    // Update the status and log success
                    toolStrip_LabelForStatus.Text = $@"Successfully saved ""{originalFileName}"" as ""{_bnkFile.Name}"".";
                    Logger.Log($"Successfully saved \"{originalFilePath}\" as \"{_bnkFile.FullPath}\".");
                    ApplicationName = _bnkFile.ShortenedPath;
                }
            }
            catch (Exception ex)
            {
                // Handle and log any errors that occur during the save operation
                toolStrip_LabelForStatus.Text =
                    $@"Error while trying to save ""{originalFileName}"" as ""{Path.GetFileName(newFilePath)}"".";
                MessageBox.Show(
                    new StringBuilder().Append("Error while trying to save \"")
                        .Append(originalFileName)
                        .Append("\" as \"")
                        .Append(Path.GetFileName(newFilePath))
                        .Append("\":\n\n")
                        .Append(ex.Message)
                        .ToString(),
                    Path.GetFileName(newFilePath),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log(
                    $"Error while trying to save \"{originalFilePath}\" as \"{newFilePath}\":\n\n{ex.Message}\n{ex.StackTrace}");
            }
        }
        #endregion

        #region Assign txt file
        //====================================================================================================//
        /// <summary>
        /// Assign a .TXT file containing audio event names to the BNK file.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if the file to assign is unreachable, does not match the expected format, or is corrupted.
        /// </exception>
        //====================================================================================================//
        public async Task AssignFile()
        {
            using (var openFileDialog = new OpenFileDialogWithFlags())
            {
                openFileDialog.Title = @"Select .txt file to assign to the bank audio";
                openFileDialog.Filter = @"Text File (*.txt)|*.txt";
                openFileDialog.FilterIndex = 1;

                // Show the OpenFileDialog and process the selected file
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        // Initialize the progress bar
                        InitializeProgressBar(_bnkFile.WemLibrary.Count, true);

                        var fileName = Path.GetFileName(openFileDialog.FileName);
                        toolStrip_LabelForStatus.Text = $@"Assigning file ""{fileName}"" to bank audio...";
                        Logger.Log($"Assigning file \"{openFileDialog.FileName}\" to bank audio \"{_bnkFile.FullPath}\"...");

                        _assignEventNamesCancellationTokenSource = new CancellationTokenSource();

                        #region Feedback of the ongoing action
                        var progressQueue = new ProgressQueue<int>(i => toolStrip_ProgressBar.PerformStep());
                        var progress = new Progress<int>(i => progressQueue.Report(i));
                        #endregion

                        // Start the task to assign event names
                        AssignEventNamesTask = BnkFileHandler.AssignEventNames(
                            _bnkFile.WemLibrary,
                            openFileDialog.FileName,
                            _assignEventNamesCancellationTokenSource.Token,
                            progress);

                        await AssignEventNamesTask;
                        await progressQueue.WaitForQueueToEmptyAsync();
                        await Task.Delay(150);

                        // Update the status and log success
                        toolStrip_LabelForStatus.Text = $@"""{fileName}"" successfully assigned.";
                        Logger.Log($"\"{openFileDialog.FileName}\" successfully assigned to \"{_bnkFile.FullPath}\".");
                    }
                    catch(OperationCanceledException)
                    {
                        toolStrip_LabelForStatus.Text = $@"Assigning file was canceled.";
                        Logger.Log($"Assigning file was successfully canceled.");
                    }
                    catch (Exception ex)
                    {
                        // Handle and log any errors that occur during the file assignment process
                        var fileName = Path.GetFileName(openFileDialog.FileName);
                        toolStrip_LabelForStatus.Text = $@"Error while assigning file: ""{fileName}"".";
                        MessageBox.Show(
                            new StringBuilder().Append("Error while assigning file: \"")
                                .Append(fileName)
                                .Append("\".\n\n")
                                .Append(ex.Message)
                                .ToString(),
                            fileName,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        Logger.Log($"Error while assigning file \"{openFileDialog.FileName}\" to \"{_bnkFile.FullPath}\":\n{ex.Message}\n{ex.StackTrace}.");
                    }
                    finally
                    {
                        // Reset the progress bar and clean up
                        HideAndResetProgressBar();
                        AssignEventNamesTask = null;
                        _assignEventNamesCancellationTokenSource = null;
                    }

                    // Update the display of event names based on their presence
                    if (DoesContainEventName())
                    {
                        ShowEventNames();
                    }
                    else
                    {
                        HideEventNames();
                    }
                }
            }
        }
        #endregion

        #region Replace Audio
        //====================================================================================================//
        /// <summary>
        /// Replaces the currently selected audio with a new one chosen by the user.
        /// </summary>
        /// <remarks>
        /// Displays an <see cref="OpenFileDialog"/> allowing the user to choose an audio file in .MP3, .WAV, .OGG, or .WEM format 
        /// to replace the existing audio selected in the interface.
        /// </remarks>
        //====================================================================================================//
        private async Task ReplaceAudio()
        {
            // Ensure there is a selected audio file to replace
            if (olv_AudioListView.GetItemCount() <= 0 || olv_AudioListView.SelectedIndex == -1) return;

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = @"Select an audio file to replace the currently selected audio";
                openFileDialog.Filter =
                    @"Wwise audio file (*.wem)|*.wem| Wave file (*.wav)|*.wav| MPEG Layer-3 file (*.mp3)|*.mp3| OGG file (*.ogg)|*.ogg";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = false;

                var selectedFile = (AudioFile)olv_AudioListView.SelectedObject;
                var wemToReplace = selectedFile.SourceFile;
                var audioNameToReplace = selectedFile.AudioName;

                // Stop playback if the selected file is currently playing
                if (selectedFile.AudioState != PlaybackState.Stopped)
                {
                    _audioPlayerEngine.Stop();
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var fileName = openFileDialog.FileName;
                    try
                    {
                        InitializeProgressBar(2, true); // Initialize the progress bar for 2 major steps

                        #region Feedback of the ongoing action
                        var progressQueue = new ProgressQueue<int>(i => toolStrip_ProgressBar.PerformStep());
                        var progress = new Progress<int>(i => progressQueue.Report(i));
                        #endregion

                        toolStrip_LabelForStatus.Text = $@"Replacing ""{audioNameToReplace}""...";
                        Logger.Log($"Replacing \"{audioNameToReplace}\" with \"{fileName}\" in \"{_bnkFile.FullPath}\"...");

                        _replacementCancellationTokenSource = new CancellationTokenSource();

                        // Start the task to get the new WEM file data
                        ReplaceAudioTask = WemReplacementHandler.GetNewWemFile(fileName, _replacementCancellationTokenSource.Token, progress);
                        var newWemData = await ReplaceAudioTask;

                        await progressQueue.WaitForQueueToEmptyAsync();
                        await Task.Delay(150);

                        // Create a command for undo/redo functionality
                        var previousData = wemToReplace.Data;
                        var command = new WemCommand(wemToReplace, previousData, newWemData);
                        _undoRedoManager.ExecuteCommand(command);
                        UpdateUndoRedoButtons();

                        // Update the selected audio file with the new WEM data
                        selectedFile.SourceFile.Data = newWemData;
                        toolStrip_LabelForStatus.Text = $@"""{audioNameToReplace}"" successfully replaced!";
                        Logger.Log($"\"{audioNameToReplace}\" successfully replaced!");
                    }
                    catch (OperationCanceledException)
                    {
                        toolStrip_LabelForStatus.Text = $@"Replacement of ""{audioNameToReplace}"" canceled.";
                        Logger.Log($"Replacement of \"{audioNameToReplace}\" was successfully canceled.");
                    }
                    catch (Exception ex)
                    {
                        // Handle and log any errors that occur during the replacement process
                        toolStrip_LabelForStatus.Text = $@"Error while replacing ""{audioNameToReplace}"". File was ignored.";
                        MessageBox.Show(
                            new StringBuilder().Append("Error while replacing \"")
                                .Append(audioNameToReplace)
                                .Append("\":\n\n")
                                .Append(ex.Message)
                                .Append("\n\nReplacement aborted.")
                                .ToString(),
                            @"Replacing Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        Logger.Log(
                            $"Error while replacing \"{audioNameToReplace}\" with \"{fileName}\" in \"{_bnkFile.FullPath}\":\n\n{ex.Message}\n{ex.StackTrace}\n\nReplacement aborted.");
                    }
                    finally
                    {
                        // Reset the progress bar and clean up
                        HideAndResetProgressBar();
                        _replacementCancellationTokenSource.Dispose();
                        ReplaceAudioTask = null;
                    }
                }
            }
        }
        #endregion

        #region Extract audio
        //====================================================================================================//
        /// <summary>
        /// Extracts the selected audio file to a user-specified location.
        /// </summary>
        //====================================================================================================//
        private async Task ExtractAudio()
        {
            // Ensure there is a selected audio file to extract
            if (olv_AudioListView.GetItemCount() <= 0 || olv_AudioListView.SelectedIndex == -1) return;

            using (var saveFileDialog = new SaveFileDialogWIthFlags())
            {
                saveFileDialog.Title = @"Select a location to extract the selected audio";
                saveFileDialog.Filter = @"Wwise encoded media file (*.wem)|*.wem| Wwise encoded audio format file (*.ogg or *.wav)|*.xxx| Wave (PCM) audio file (*.wav)|*.wav| Ogg (VORBIS) audio file (*.ogg)|*.ogg| MPEG Layer-3 audio file (*.mp3)|*.mp3";
                saveFileDialog.FilterIndex = 1;

                var selectedFile = (AudioFile)olv_AudioListView.SelectedObject;
                saveFileDialog.FileName = selectedFile.AudioName;

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        InitializeProgressBar(3, true); // Initialize the progress bar for 3 major steps

                        #region Feedback of the ongoing action
                        var progressQueue = new ProgressQueue<int>(i => toolStrip_ProgressBar.PerformStep());
                        var progress = new Progress<int>(i => progressQueue.Report(i));
                        #endregion

                        var fileName = saveFileDialog.FileName;
                        var fileExtension = Path.GetExtension(fileName);

                        toolStrip_LabelForStatus.Text = $@"Extracting ""{selectedFile.AudioName}""...";
                        Logger.Log($"Extracting \"{selectedFile.AudioName}\" from \"{_bnkFile.FullPath}\" to format \"{fileExtension}\"...");

                        _extractionCancellationTokenSource = new CancellationTokenSource();

                        // Start the task to extract the audio
                        ExtractionAudioTask = AudioExtractionHandler.ExtractSingleAudio(
                            fileName,
                            selectedFile,
                            _extractionCancellationTokenSource.Token,
                            progress);

                        await ExtractionAudioTask;
                        await progressQueue.WaitForQueueToEmptyAsync();
                        await Task.Delay(150);

                        // Update status and log success
                        toolStrip_LabelForStatus.Text = $@"""{selectedFile.AudioName}"" successfully extracted!";
                        Logger.Log($"\"{selectedFile.AudioName}\" successfully extracted!");
                    }
                    catch (OperationCanceledException)
                    {
                        toolStrip_LabelForStatus.Text = $@"Extraction of ""{selectedFile.AudioName}"" canceled.";
                        Logger.Log($"Extraction of \"{selectedFile.AudioName}\" was successfully canceled.");
                    }
                    catch (Exception ex)
                    {
                        // Handle and log any errors that occur during extraction
                        toolStrip_LabelForStatus.Text = $@"Error while extracting ""{selectedFile.AudioName}"".";
                        MessageBox.Show(
                            new StringBuilder().Append("Error while extracting \"")
                                .Append(selectedFile.AudioName)
                                .Append("\":\n\n")
                                .Append(ex.Message)
                                .Append("\n\nExtraction aborted.")
                                .ToString(),
                            @"Extraction Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        Logger.Log(
                            $"Error while extracting \"{selectedFile.AudioName}\" from \"{_bnkFile.FullPath}\" to format \"{Path.GetExtension(saveFileDialog.FileName)}\":\n\n{ex.Message}\n{ex.StackTrace}\n\nExtraction aborted.");
                    }
                    finally
                    {
                        // Reset the progress bar and clean up
                        HideAndResetProgressBar();
                        _extractionCancellationTokenSource.Dispose();
                        ExtractionAudioTask = null;
                    }
                }
            }
        }
        #endregion

        #region Extract all audio
        //====================================================================================================//
        /// <summary>
        /// Extracts all the audio files in the bank audio file to a user-specified location.
        /// </summary>
        //====================================================================================================//
        private async Task ExtractAllAudios()
        {
            // Ensure there are audio files to extract
            if (olv_AudioListView.GetItemCount() <= 0) return;

            using (var saveFileDialog = new SaveFileDialogWIthFlags())
            {
                saveFileDialog.Title = @"Select a location to extract all audio files";
                saveFileDialog.Filter =
                    @"Wwise encoded media file (*.wem)|*.wem| Wwise encoded audio format file (*.ogg or *.wav)|*.xxx| Wave (PCM) audio file (*.wav)|*.wav| Ogg (VORBIS) audio file (*.ogg)|*.ogg| MPEG Layer-3 audio file (*.mp3)|*.mp3";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.FileName = "default names (ID - event name)";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        // Initialize the progress bar for the number of audio files to be extracted
                        InitializeProgressBar(_bnkFile.AudioLibrary.Count, true);

                        #region Feedback of the ongoing action
                        var progressQueue = new ProgressQueue<int>(i => toolStrip_ProgressBar.PerformStep());
                        var progress = new Progress<int>(i => progressQueue.Report(i));
                        #endregion

                        toolStrip_LabelForStatus.Text = @"Extracting audio files...";
                        Logger.Log($"Extracting all audio files ({_bnkFile.AudioLibrary.Count} files) from \"{_bnkFile.FullPath}\" to format \"{Path.GetExtension(saveFileDialog.FileName)}\"...");

                        _extractionCancellationTokenSource = new CancellationTokenSource();

                        // Start the task to extract all audio files
                        ExtractionAudioTask = AudioExtractionHandler.ExtractAllAudio(
                            saveFileDialog.FileName,
                            _bnkFile.AudioLibrary,
                            _extractionCancellationTokenSource.Token,
                            progress);

                        await ExtractionAudioTask;
                        await progressQueue.WaitForQueueToEmptyAsync();
                        await Task.Delay(150);

                        // Update status and log success
                        toolStrip_LabelForStatus.Text = @"Audio files successfully extracted!";
                        Logger.Log($"{_bnkFile.AudioLibrary.Count} audio files successfully extracted!");
                    }
                    catch (OperationCanceledException)
                    {
                        toolStrip_LabelForStatus.Text = @"Extraction canceled.";
                        Logger.Log("Extraction was successfully canceled.");
                    }
                    catch (Exception ex)
                    {
                        // Handle and log any errors that occur during extraction
                        toolStrip_LabelForStatus.Text = @"An error occurred during extraction. Extraction aborted.";
                        MessageBox.Show(
                            new StringBuilder().Append("Error while extracting audio files:\n\n")
                                .Append(ex.Message)
                                .Append("\n\nExtraction aborted.")
                                .ToString(),
                            @"Extraction Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        Logger.Log(
                            $"Error while extracting {_bnkFile.AudioLibrary.Count} audio files from \"{_bnkFile.FullPath}\" to format \"{Path.GetExtension(saveFileDialog.FileName)}\":\n\n{ex.Message}\n{ex.StackTrace}\n\nExtraction aborted.");
                    }
                    finally
                    {
                        // Reset the progress bar and clean up
                        HideAndResetProgressBar();
                        _extractionCancellationTokenSource.Dispose();
                        ExtractionAudioTask = null;
                    }
                }
            }
        }
        #endregion

        #region Undo
        //====================================================================================================//
        /// <summary>
        /// Undoes the last action performed, such as an audio replacement.
        /// Stops any audio playback before undoing the action.
        /// </summary>
        //====================================================================================================//
        private void Undo()
        {
            // Stop playback if the selected audio file is currently playing or paused
            var selectedFile = (AudioFile)olv_AudioListView.SelectedObject;
            if (selectedFile != null && selectedFile.AudioState != PlaybackState.Stopped)
            {
                _audioPlayerEngine.Stop();
            }

            // Perform the undo operation and update the UI buttons
            _undoRedoManager.Undo();
            UpdateUndoRedoButtons();
        }
        #endregion

        #region Redo
        //====================================================================================================//
        /// <summary>
        /// Redoes the last action that was undone, such as an audio replacement.
        /// Stops any audio playback before redoing the action.
        /// </summary>
        //====================================================================================================//
        private void Redo()
        {
            // Stop playback if the selected audio file is currently playing or paused
            var selectedFile = (AudioFile)olv_AudioListView.SelectedObject;
            if (selectedFile != null && selectedFile.AudioState != PlaybackState.Stopped)
            {
                _audioPlayerEngine.Stop();
            }

            // Perform the redo operation and update the UI buttons
            _undoRedoManager.Redo();
            UpdateUndoRedoButtons();
        }
        #endregion

        #region Handle Preview Key Down
        //====================================================================================================//
        /// <summary>
        /// Handles the PreviewKeyDown event of the MainForm controls.
        /// <para>- Prevents the default handling of the Tab key</para>
        /// <para>- Handles the Media keys (Play/Pause, Next, Previous, Stop)</para>
        /// <para>- Focuses the search TextBox when Ctrl + F is pressed</para>
        /// </summary>
        /// <param name="e">The event data for the PreviewKeyDown event.</param>
        //====================================================================================================//
        private void HandlePreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            // Handle specific key presses
            switch (e.KeyCode)
            {
                case Keys.Tab:
                    e.IsInputKey = true; // Prevent default handling of the Tab key
                    break;

                // Media key handling
                case Keys.MediaPlayPause:
                    PlayerPlayPause_Event_Click(this, EventArgs.Empty); // Play/Pause
                    break;

                case Keys.MediaNextTrack:
                    PlayerNextAudio_Event_Click(this, EventArgs.Empty); // Next track
                    break;

                case Keys.MediaPreviousTrack:
                    PlayerPreviousAudio_Event_Click(this, EventArgs.Empty); // Previous track
                    break;

                case Keys.MediaStop:
                    _audioPlayerEngine.Stop(); // Stop playback
                    break;
            }

            // Handle Ctrl + F to focus the search TextBox
            if (e.Control && e.KeyCode == Keys.F)
            {
                txtbox_FilterBox.Focus();
            }
        }
        #endregion

        #region Handle Key Press
        //====================================================================================================//
        /// <summary>
        /// Handles the KeyPress event for the MainForm controls.
        /// <para>- Suppresses the Tab key to prevent it from being processed.</para>
        /// </summary>
        /// <param name="e">The event data for the KeyPress event.</param>
        //====================================================================================================//
        private static void HandleKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Tab)
            {
                e.Handled = true; // Suppress the Tab key to prevent default behavior
            }
        }
        #endregion

        #region FilterBox KeyDown
        //====================================================================================================//
        /// <summary>
        /// Handles the KeyDown event for the FilterBox control.
        /// Suppresses the default behavior for the Enter and Escape keys.
        /// </summary>
        /// <param name="e">The event data for the KeyDown event.</param>
        //====================================================================================================//
        private static void FilterBoxKeyDown(KeyEventArgs e)
        {
            // Suppress default behavior for Enter and Escape keys
            if (e.KeyCode != Keys.Enter && e.KeyCode != Keys.Escape) return;
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        #endregion

        #region FilterBox TextChanged
        //====================================================================================================//
        /// <summary>
        /// Handles the TextChanged event of the FilterBox.
        /// Updates the filter criteria and refreshes the audio list view selection.
        /// </summary>
        //====================================================================================================//
        private void FilterBox_TextChanged()
        {
            UpdateFilter(); // Update the filter criteria based on the current text in the FilterBox
            UpdateAudioListViewSelection(); // Refresh the audio list view to reflect the updated filter
        }
        #endregion

        #region Audio List View

        #region Audio List View Format Row
        //====================================================================================================//
        /// <summary>
        /// Handles the FormatRow event of the AudioListView, formatting the appearance of rows based on the 
        /// state of the audio file (e.g., modified, playing).
        /// </summary>
        /// <param name="e">The event data for the FormatRow event.</param>
        //====================================================================================================//
        private static void AudioListView_FormatRow(FormatRowEventArgs e)
        {
            var currentFile = (AudioFile)e.Model;

            // Set font style based on whether the audio file is modified
            e.Item.Font = new Font("Segoe UI", 9f, currentFile.IsModified ? FontStyle.Bold : FontStyle.Regular);

            // Prefix an asterisk (*) if the file is modified, otherwise remove it
            if (currentFile.IsModified && !e.Item.Text.Contains("*"))
            {
                e.Item.Text = @"*" + e.Item.Text;
            }
            else if (!currentFile.IsModified && e.Item.Text.Contains("*"))
            {
                e.Item.Text = e.Item.Text.Remove(0, 1);
            }

            // Set background color based on playback state
            if (currentFile.AudioState != PlaybackState.Stopped)
            {
                e.Item.BackColor = Color.FromArgb(137, 215, 129);
                e.Item.SelectedBackColor = Color.FromArgb(137, 215, 129);
            }
            else
            {
                e.Item.SelectedBackColor = Color.FromArgb(191, 224, 247);
            }
        }

        #endregion

        #region Audio List View Item Changed
        //====================================================================================================//
        /// <summary>
        /// Handles the ItemChanged event of the AudioListView, updating the selection in the list view.
        /// </summary>
        //====================================================================================================//
        private void AudioListView_ItemChanged()
        {
            UpdateAudioListViewSelection();
        }
        #endregion

        #region Audio List View Selection Changed
        //====================================================================================================//
        /// <summary>
        /// Handles the SelectionChanged event of the AudioListView, updating the selection if none is selected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        //====================================================================================================//
        private void AudioListView_SelectionChanged(object sender)
        {
            var currentListView = (ObjectListView)sender;
            if (currentListView.SelectedIndex == -1)
            {
                UpdateAudioListViewSelection();
            }
        }
        #endregion

        #region Audio List View Item Activate
        //====================================================================================================//
        /// <summary>
        /// Handles the ItemActivate event of the AudioListView, controlling playback of the selected audio.
        /// </summary>
        //====================================================================================================//
        private void AudioListView_ItemActivate()
        {
            if (olv_AudioListView.GetItemCount() <= 0 || olv_AudioListView.SelectedObject == null) return;
            if (olv_AudioListView.HotCellHitLocation == HitTestLocation.Button) return;

            var selectedAudioFile = (IAudioFile)olv_AudioListView.SelectedObject;

            try
            {
                if (_audioPlayerEngine.PlayerState == PlaybackState.Stopped || _audioPlayerEngine.CurrentIAudioFile != selectedAudioFile)
                {
                    var listDisplayed = olv_AudioListView.FilteredObjects.Cast<IAudioFile>().ToList();
                    var index = olv_AudioListView.SelectedIndex;
                    _audioPlayerEngine.PlayList(listDisplayed, index);
                }
                else if (_audioPlayerEngine.PlayerState == PlaybackState.Paused)
                {
                    _audioPlayerEngine.Resume();
                }
                else if (_audioPlayerEngine.PlayerState == PlaybackState.Playing)
                {
                    _audioPlayerEngine.Pause();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    new StringBuilder().Append("Error while trying to manage audio playback.\n\n")
                        .Append(ex.Message)
                        .ToString(), @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStrip_LabelForStatus.Text = @"Error while trying to manage audio playback.";
                Logger.Log($"Error while trying to manage audio \"{selectedAudioFile.AudioName}\" of \"{_bnkFile.FullPath}\":\n\n{ex.Message}\n{ex.StackTrace}");
            }
        }
        #endregion

        #region Audio List View Button Click
        //====================================================================================================//
        /// <summary>
        /// Handles the ButtonClick event of the AudioListView, controlling playback when a button is clicked.
        /// </summary>
        /// <param name="e">The event data for the ButtonClick event.</param>
        //====================================================================================================//
        private void AudioListView_ButtonClick(CellClickEventArgs e)
        {
            var selectedAudioFile = e.Item.RowObject as IAudioFile;
            olv_AudioListView.SelectedIndex = e.RowIndex;

            try
            {
                if (selectedAudioFile != null && selectedAudioFile.AudioState == PlaybackState.Stopped)
                {
                    var listDisplayed = olv_AudioListView.FilteredObjects.Cast<IAudioFile>().ToList();
                    var index = olv_AudioListView.SelectedIndex;
                    _audioPlayerEngine.PlayList(listDisplayed, index);
                }
                else if (_audioPlayerEngine.PlayerState == PlaybackState.Paused)
                {
                    _audioPlayerEngine.Resume();
                }
                else if (_audioPlayerEngine.PlayerState == PlaybackState.Playing)
                {
                    _audioPlayerEngine.Pause();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    new StringBuilder().Append("Error while trying to manage audio playback.\n\n")
                        .Append(ex.Message)
                        .ToString(), @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStrip_LabelForStatus.Text = @"Error while trying to manage audio playback.";
                Logger.Log($"Error while trying to manage audio \"{selectedAudioFile?.AudioName}\" of \"{_bnkFile.FullPath}\":\n\n{ex.Message}\n{ex.StackTrace}");
            }
        }
        #endregion

        #region Audio List View Key Down
        //====================================================================================================//
        /// <summary>
        /// Handles the KeyDown event of the AudioListView, managing navigation and activation of items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="keys">The event data for the KeyDown event.</param>
        //====================================================================================================//
        private void AudioListView_KeyDown(object sender, KeyEventArgs keys)
        {
            var keyCode = keys.KeyData;
            if (keyCode == Keys.Enter)
            {
                AudioListView_Event_ItemActivate(sender, keys);
            }
            else if ((keyCode == (Keys.Home | Keys.Control)) || (keyCode == (Keys.Up | Keys.Control)))
            {
                if (olv_AudioListView.GetItemCount() > 0)
                {
                    olv_AudioListView.SelectedIndex = 0;
                }
            }
            else if ((keyCode == (Keys.End | Keys.Control)) || (keyCode == (Keys.Down | Keys.Control)))
            {
                if (olv_AudioListView.GetItemCount() > 0)
                {
                    olv_AudioListView.SelectedIndex = olv_AudioListView.GetItemCount() - 1;
                }
            }
            else
            {
                switch (keyCode)
                {
                    case Keys.Up:
                        if (olv_AudioListView.GetItemCount() > 0)
                        {
                            olv_AudioListView.SelectedIndex = olv_AudioListView.SelectedIndex == 0
                                ? olv_AudioListView.GetItemCount() - 1
                                : olv_AudioListView.SelectedIndex - 1;
                        }
                        break;
                    case Keys.Down:
                        if (olv_AudioListView.GetItemCount() > 0)
                        {
                            olv_AudioListView.SelectedIndex = olv_AudioListView.SelectedIndex == olv_AudioListView.GetItemCount() - 1
                                ? 0
                                : olv_AudioListView.SelectedIndex + 1;
                        }
                        break;
                }
            }

            keys.Handled = true;
            keys.SuppressKeyPress = true;
            if (olv_AudioListView.SelectedIndex != -1)
            {
                olv_AudioListView.EnsureVisible(olv_AudioListView.SelectedIndex);
            }
        }
        #endregion

        #endregion

        #region Player

        #region Player Mode
        //====================================================================================================//
        /// <summary>
        /// Handles the ModeChanged event of the Player, updating the UI to reflect the current playback mode.
        /// </summary>
        //====================================================================================================//
        private void Player_ModeChanged()
        {
            switch (_audioPlayerEngine.Mode)
            {
                case PlayerMode.LoopSingleFile:
                    notSelectable_Btn_PlayerMode.BackgroundImage = Properties.Resources.Loop1Button;
                    FormToolTip.SetToolTip(notSelectable_Btn_PlayerMode, "Loop the selected audio.");
                    break;
                case PlayerMode.LoopList:
                    notSelectable_Btn_PlayerMode.BackgroundImage = Properties.Resources.LoopButton;
                    FormToolTip.SetToolTip(notSelectable_Btn_PlayerMode, "Loop through all audio files in the list.");
                    break;
                case PlayerMode.NoLoop:
                    notSelectable_Btn_PlayerMode.BackgroundImage = Properties.Resources.SingleButton;
                    FormToolTip.SetToolTip(notSelectable_Btn_PlayerMode, "Play selected audio once.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(); // Ensure all PlayerModes are handled
            }
        }
        #endregion

        #region Player Playback State
        //====================================================================================================//
        /// <summary>
        /// Handles the PlaybackStateChanged event of the Player, updating the play/pause button to reflect the current state.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        //====================================================================================================//
        private void Player_PlaybackStateChanged()
        {
            switch (_audioPlayerEngine.PlayerState)
            {
                case PlaybackState.Playing:
                    notSelectable_btn_PlayerPlayPause.BackgroundImage = Properties.Resources.PauseButton;
                    break;
                case PlaybackState.Paused:
                case PlaybackState.Stopped:
                    notSelectable_btn_PlayerPlayPause.BackgroundImage = Properties.Resources.PlayButton;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(); // Ensure all PlaybackStates are handled
            }

            // Redraw the player control buttons to reflect the updated state
            notSelectable_btn_PlayerPreviousAudio.Invalidate();
            notSelectable_btn_PlayerNextAudio.Invalidate();
            notSelectable_btn_PlayerPlayPause.Invalidate();
        }
        #endregion

        #region Player Buttons

        #region Player Button Mode
        //====================================================================================================//
        /// <summary>
        /// Handles the Click event of the PlayerMode button, cycling through the available playback modes.
        /// </summary>
        //====================================================================================================//
        private void PlayerMode_Click()
        {
            try
            {
                // Cycle through the playback modes
                switch (_audioPlayerEngine.Mode)
                {
                    case PlayerMode.LoopSingleFile:
                        _audioPlayerEngine.Mode = PlayerMode.LoopList;
                        break;
                    case PlayerMode.LoopList:
                        _audioPlayerEngine.Mode = PlayerMode.NoLoop;
                        break;
                    case PlayerMode.NoLoop:
                        _audioPlayerEngine.Mode = PlayerMode.LoopSingleFile;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(); // Ensure all PlayerModes are handled
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    new StringBuilder().Append("Error while trying to change player mode.\n\n")
                        .Append(ex.Message)
                        .ToString(), @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Log($"Error while trying to change player mode.\n\n{ex.Message}\n{ex.StackTrace}");
            }
        }
        #endregion

        #region Player Button Previous Audio
        //====================================================================================================//
        /// <summary>
        /// Handles the Click event of the PlayerPreviousAudio button, playing the previous audio in the list.
        /// </summary>
        //====================================================================================================//
        private void PlayerPreviousAudio_Click()
        {
            if (olv_AudioListView.GetItemCount() <= 0 || olv_AudioListView.SelectedObject == null) return;

            var listDisplayed = olv_AudioListView.FilteredObjects.Cast<IAudioFile>().ToList();
            olv_AudioListView.SelectedIndex = olv_AudioListView.SelectedIndex == 0
                ? listDisplayed.Count - 1
                : olv_AudioListView.SelectedIndex - 1;
            if (olv_AudioListView.SelectedIndex != -1)
            {
                olv_AudioListView.EnsureVisible(olv_AudioListView.SelectedIndex);
            }
            var index = olv_AudioListView.SelectedIndex;
            try
            {
                _audioPlayerEngine.PlayPreviousNextList(listDisplayed, index);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    new StringBuilder().Append("Error while trying to play previous audio \"")
                        .Append(listDisplayed[index].AudioName)
                        .Append("\":\n\n")
                        .Append(ex.Message)
                        .ToString(), @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Log($"Error while trying to play previous audio \"{listDisplayed[index].AudioName}\" of \"{_bnkFile.FullPath}\":\n\n{ex.Message}\n{ex.StackTrace}");
            }
        }
        #endregion

        #region Player Button Next Audio
        //====================================================================================================//
        /// <summary>
        /// Handles the Click event of the PlayerNextAudio button, playing the next audio in the list.
        /// </summary>
        //====================================================================================================//
        private void PlayerNextAudio_Click()
        {
            if (olv_AudioListView.GetItemCount() <= 0 || olv_AudioListView.SelectedObject == null) return;

            var listDisplayed = olv_AudioListView.FilteredObjects.Cast<IAudioFile>().ToList();
            olv_AudioListView.SelectedIndex = (olv_AudioListView.SelectedIndex + 1) % listDisplayed.Count;
            if (olv_AudioListView.SelectedIndex != -1)
            {
                olv_AudioListView.EnsureVisible(olv_AudioListView.SelectedIndex);
            }
            var index = olv_AudioListView.SelectedIndex;
            try
            {
                _audioPlayerEngine.PlayPreviousNextList(listDisplayed, index);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    new StringBuilder().Append("Error while trying to play next audio \"")
                        .Append(listDisplayed[index].AudioName)
                        .Append("\":\n\n")
                        .Append(ex.Message)
                        .ToString(), @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Log($"Error while trying to play next audio \"{listDisplayed[index].AudioName}\" of \"{_bnkFile.FullPath}\":\n\n{ex.Message}\n{ex.StackTrace}");
            }
        }
        #endregion

        #region Player Button Play/Pause
        //====================================================================================================//
        /// <summary>
        /// Handles the Click event of the PlayerPlayPause button, toggling between play and pause.
        /// </summary>
        //====================================================================================================//
        private void PlayerButtonPlayPause_Click()
        {
            if (olv_AudioListView.GetItemCount() <= 0 || olv_AudioListView.SelectedObject == null) return;

            var listDisplayed = olv_AudioListView.FilteredObjects.Cast<IAudioFile>().ToList();
            var index = olv_AudioListView.SelectedIndex;

            try
            {
                switch (_audioPlayerEngine.PlayerState)
                {
                    case PlaybackState.Stopped:
                        _audioPlayerEngine.PlayList(listDisplayed, index);
                        break;
                    case PlaybackState.Playing:
                        _audioPlayerEngine.Pause();
                        break;
                    case PlaybackState.Paused:
                        _audioPlayerEngine.Resume();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(); // Ensure all PlaybackStates are handled
                }
            }
            catch (Exception ex)
            {
                string action;
                switch (_audioPlayerEngine.PlayerState)
                {
                    case PlaybackState.Stopped:
                        action = "play";
                        break;
                    case PlaybackState.Playing:
                        action = "pause";
                        break;
                    case PlaybackState.Paused:
                        action = "resume";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                MessageBox.Show(
                    new StringBuilder().Append("Error while trying to ")
                        .Append(action)
                        .Append(" audio \"")
                        .Append(listDisplayed[index].AudioName)
                        .Append("\":\n\n")
                        .Append(ex.Message)
                        .ToString(),
                    @"Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Logger.Log(
                    $"Error while trying to {action} audio \"{listDisplayed[index].AudioName}\" of \"{_bnkFile.FullPath}\":\n\n{ex.Message}\n{ex.StackTrace}");

                toolStrip_LabelForStatus.Text = new StringBuilder().Append("Error while trying to ")
                    .Append(action)
                    .Append(" audio.")
                    .ToString();
            }
        }
        #endregion

        #endregion

        #endregion

        #region Add Handlers PreviewKeyDown and KeyPress to controls
        //====================================================================================================//
        /// <summary>
        /// Recursively adds the PreviewKeyDown and KeyPress event handlers to the specified control and all its child controls.
        /// </summary>
        /// <param name="parent">The parent control to which the event handlers should be added.</param>
        //====================================================================================================//
        private void AddHandlersToAllControls(Control parent)
        {
            // Iterate through each child control of the parent
            foreach (Control control in parent.Controls)
            {
                // Add the PreviewKeyDown event handler
                control.PreviewKeyDown += MainForm_PreviewKeyDown;

                // Add the KeyPress event handler
                control.KeyPress += MainForm_KeyPress;

                // Recursively add the handlers to the child controls if the control has children
                if (control.HasChildren)
                {
                    AddHandlersToAllControls(control);
                }
            }
        }
        #endregion

        #region Update visibility of the undo and redo buttons
        //====================================================================================================//
        /// <summary>
        /// Updates the enabled state of the undo and redo buttons based on the current state of the undo/redo manager.
        /// </summary>
        //====================================================================================================//
        private void UpdateUndoRedoButtons()
        {
            // Enable or disable the Undo button based on whether there are actions to undo
            toolStripMenuItem_Undo.Enabled = _undoRedoManager.CanUndo;

            // Enable or disable the Redo button based on whether there are actions to redo
            toolStripMenuItem_Redo.Enabled = _undoRedoManager.CanRedo;
        }
        #endregion

        #region Update Filter
        //====================================================================================================//
        /// <summary>
        /// Updates the filter of the <see cref="ObjectListView" /> based on the current text in the filter box.
        /// </summary>
        //====================================================================================================//
        private void UpdateFilter()
        {
            if (string.IsNullOrEmpty(txtbox_FilterBox.Text))
            {
                // If the filter box is empty, remove any applied filters
                olv_AudioListView.ModelFilter = null;
            }
            else
            {
                // If the filter box is not empty, apply a text match filter based on the input
                var filterKey = new[] { txtbox_FilterBox.Text };
                olv_AudioListView.ModelFilter = TextMatchFilter.Contains(olv_AudioListView, filterKey);
            }

            // Update the context menu based on whether any items remain after filtering
            olv_AudioListView.ContextMenuStrip = olv_AudioListView.GetItemCount() > 0 ? contextMenuStrip_WEMList : null;
        }
        #endregion

        #region Update Audio List View Selection
        //====================================================================================================//
        /// <summary>
        /// Updates the selection in the audio list view. If no item is selected, selects the first item.
        /// Also updates the number of audio files currently displayed.
        /// </summary>
        //====================================================================================================//
        private void UpdateAudioListViewSelection()
        {
            // If no BNK file is loaded or if the file doesn't contain any WEM files, exit the method
            if (!IsBnkFileLoaded || !FileContainsWem) return;

            // If no item is currently selected in the list view, select the first item
            if (olv_AudioListView.SelectedIndex == -1)
            {
                olv_AudioListView.SelectedIndex = 0;
            }

            // Update the display with the number of audio files currently shown in the list
            UpdateNumberOfAudioDisplayed();
        }
        #endregion

        #region Update Number of audio displayed
        //====================================================================================================//
        /// <summary>
        /// Updates the label displaying the number of audio files currently shown in the list view.
        /// If a filter is applied, it shows the number of filtered items versus the total number of items.
        /// </summary>
        //====================================================================================================//
        private void UpdateNumberOfAudioDisplayed()
        {
            // Check if the list view is currently applying a filter
            // Display the number of filtered items out of the total number of items in the audio library
            lbl_NumberOfAudios.Text = olv_AudioListView.IsFiltering ? $"{olv_AudioListView.GetItemCount()} / {_bnkFile.AudioLibrary.Count} audio(s)" :
                // Display the total number of items in the audio library when no filter is applied
                $"{_bnkFile.AudioLibrary.Count} audio(s)";
        }
        #endregion

        #endregion
    }
}