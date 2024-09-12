using System.Windows.Forms;
using BKSMTool.Controls;
using BKSMTool.Controls.PlayerEngine;

namespace BKSMTool
{
    partial class MainForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            BrightIdeasSoftware.HeaderStateStyle headerStateStyle7 = new BrightIdeasSoftware.HeaderStateStyle();
            BrightIdeasSoftware.HeaderStateStyle headerStateStyle8 = new BrightIdeasSoftware.HeaderStateStyle();
            BrightIdeasSoftware.HeaderStateStyle headerStateStyle9 = new BrightIdeasSoftware.HeaderStateStyle();
            BrightIdeasSoftware.HeaderStateStyle headerStateStyle10 = new BrightIdeasSoftware.HeaderStateStyle();
            BrightIdeasSoftware.HeaderStateStyle headerStateStyle11 = new BrightIdeasSoftware.HeaderStateStyle();
            BrightIdeasSoftware.HeaderStateStyle headerStateStyle12 = new BrightIdeasSoftware.HeaderStateStyle();
            this.statusStripForFileStatus = new System.Windows.Forms.StatusStrip();
            this.toolStrip_ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStrip_LabelForStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.headerFormatStyle2 = new BrightIdeasSoftware.HeaderFormatStyle();
            this.headerFormatStyle1 = new BrightIdeasSoftware.HeaderFormatStyle();
            this.gpBox_FilterBox = new System.Windows.Forms.GroupBox();
            this.txtbox_FilterBox = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem_FILE = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.separator0 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.separator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_Close = new System.Windows.Forms.ToolStripMenuItem();
            this.separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.separator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_bySaltwatersam = new System.Windows.Forms.ToolStripTextBox();
            this.separator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_EDIT = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Redo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.replaceSelectedAudioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractAudioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractAllAudiosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.AssignTxTFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_WEMList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_BNKReplaceAudio = new System.Windows.Forms.ToolStripMenuItem();
            this.separator17 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_BNKExtractAudio = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_BNKExtractAllAudio = new System.Windows.Forms.ToolStripMenuItem();
            this.FormToolTip = new System.Windows.Forms.ToolTip(this.components);
            this._playerHidableTrackbarVolume = new BKSMTool.Controls.PlayerEngine.PlayerHidableTrackbarVolume();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.player_lbl_AudioPosition = new BKSMTool.Controls.PlayerEngine.PlayerLabelAudioPosition();
            this.player_lbl_AudioDuration = new BKSMTool.Controls.PlayerEngine.PlayerLabelAudioDuration();
            this.player_TrackBar_AudioPosition = new BKSMTool.Controls.PlayerEngine.PlayerTrackBarAudioPosition();
            this.notSelectable_Btn_PlayerMode = new BKSMTool.Controls.NotSelectableButton();
            this.notSelectable_btn_PlayerPreviousAudio = new BKSMTool.Controls.NotSelectableButton();
            this.notSelectable_btn_PlayerNextAudio = new BKSMTool.Controls.NotSelectableButton();
            this.notSelectable_btn_PlayerPlayPause = new BKSMTool.Controls.NotSelectableButton();
            this.player_lbl_AudioName = new BKSMTool.Controls.PlayerEngine.PlayerLabelAudioName();
            this.lbl_NumberOfAudios = new System.Windows.Forms.Label();
            this.olv_AudioListView = new BrightIdeasSoftware.ObjectListView();
            this.OLVC_AudioState = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.SoundPlayerStateRenderer = new BKSMTool.Controls.PlayerEngine.SoundPlayerStateRenderer();
            this.OLVC_ID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.OLVC_RelatedName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.OLVC_DURATION = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.OLVC_DataArraySizeAsString = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.OLVC_IsModified = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.audioSavedStateRenderer1 = new BKSMTool.Controls.PlayerEngine.AudioSavedStateRenderer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutBKSMToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStripForFileStatus.SuspendLayout();
            this.gpBox_FilterBox.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.contextMenuStrip_WEMList.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olv_AudioListView)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripForFileStatus
            // 
            this.statusStripForFileStatus.AllowDrop = true;
            this.statusStripForFileStatus.AllowMerge = false;
            resources.ApplyResources(this.statusStripForFileStatus, "statusStripForFileStatus");
            this.statusStripForFileStatus.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStripForFileStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_ProgressBar,
            this.toolStrip_LabelForStatus});
            this.statusStripForFileStatus.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStripForFileStatus.Name = "statusStripForFileStatus";
            this.statusStripForFileStatus.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStripForFileStatus.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.statusStripForFileStatus.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // toolStrip_ProgressBar
            // 
            this.toolStrip_ProgressBar.Maximum = 1;
            this.toolStrip_ProgressBar.Name = "toolStrip_ProgressBar";
            resources.ApplyResources(this.toolStrip_ProgressBar, "toolStrip_ProgressBar");
            this.toolStrip_ProgressBar.Step = 1;
            this.toolStrip_ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // toolStrip_LabelForStatus
            // 
            this.toolStrip_LabelForStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStrip_LabelForStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStrip_LabelForStatus.Name = "toolStrip_LabelForStatus";
            resources.ApplyResources(this.toolStrip_LabelForStatus, "toolStrip_LabelForStatus");
            // 
            // headerFormatStyle2
            // 
            headerStateStyle7.BackColor = System.Drawing.Color.Gray;
            headerStateStyle7.FrameColor = System.Drawing.SystemColors.AppWorkspace;
            this.headerFormatStyle2.Hot = headerStateStyle7;
            headerStateStyle8.BackColor = System.Drawing.Color.Gray;
            headerStateStyle8.FrameColor = System.Drawing.SystemColors.AppWorkspace;
            this.headerFormatStyle2.Normal = headerStateStyle8;
            headerStateStyle9.BackColor = System.Drawing.Color.Gray;
            headerStateStyle9.FrameColor = System.Drawing.SystemColors.AppWorkspace;
            this.headerFormatStyle2.Pressed = headerStateStyle9;
            // 
            // headerFormatStyle1
            // 
            headerStateStyle10.BackColor = System.Drawing.Color.Gray;
            headerStateStyle10.FrameColor = System.Drawing.SystemColors.AppWorkspace;
            headerStateStyle10.FrameWidth = 2F;
            this.headerFormatStyle1.Hot = headerStateStyle10;
            headerStateStyle11.BackColor = System.Drawing.Color.Gray;
            headerStateStyle11.FrameColor = System.Drawing.SystemColors.AppWorkspace;
            headerStateStyle11.FrameWidth = 2F;
            this.headerFormatStyle1.Normal = headerStateStyle11;
            headerStateStyle12.BackColor = System.Drawing.Color.Gray;
            headerStateStyle12.FrameColor = System.Drawing.SystemColors.AppWorkspace;
            headerStateStyle12.FrameWidth = 2F;
            this.headerFormatStyle1.Pressed = headerStateStyle12;
            // 
            // gpBox_FilterBox
            // 
            this.gpBox_FilterBox.Controls.Add(this.txtbox_FilterBox);
            resources.ApplyResources(this.gpBox_FilterBox, "gpBox_FilterBox");
            this.gpBox_FilterBox.Name = "gpBox_FilterBox";
            this.gpBox_FilterBox.TabStop = false;
            this.gpBox_FilterBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.gpBox_FilterBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // txtbox_FilterBox
            // 
            this.txtbox_FilterBox.AllowDrop = true;
            resources.ApplyResources(this.txtbox_FilterBox, "txtbox_FilterBox");
            this.txtbox_FilterBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtbox_FilterBox.Name = "txtbox_FilterBox";
            this.txtbox_FilterBox.TabStop = false;
            this.txtbox_FilterBox.TextChanged += new System.EventHandler(this.FilterBox_Event_TextChanged);
            this.txtbox_FilterBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterBox_Event_KeyDown);
            // 
            // menuStrip
            // 
            this.menuStrip.AllowDrop = true;
            this.menuStrip.AllowMerge = false;
            this.menuStrip.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_FILE,
            this.toolStripMenuItem_EDIT,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.menuStrip.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // toolStripMenuItem_FILE
            // 
            this.toolStripMenuItem_FILE.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Open,
            this.separator0,
            this.toolStripMenuItem_Save,
            this.toolStripMenuItem_SaveAs,
            this.separator1,
            this.toolStripMenuItem_Close,
            this.separator2,
            this.toolStripMenuItem_Exit,
            this.separator3,
            this.toolStripMenuItem_bySaltwatersam,
            this.separator4});
            this.toolStripMenuItem_FILE.Name = "toolStripMenuItem_FILE";
            resources.ApplyResources(this.toolStripMenuItem_FILE, "toolStripMenuItem_FILE");
            // 
            // toolStripMenuItem_Open
            // 
            resources.ApplyResources(this.toolStripMenuItem_Open, "toolStripMenuItem_Open");
            this.toolStripMenuItem_Open.Name = "toolStripMenuItem_Open";
            this.toolStripMenuItem_Open.Click += new System.EventHandler(this.OpenBankAudioFile_Event_Click);
            // 
            // separator0
            // 
            this.separator0.Name = "separator0";
            resources.ApplyResources(this.separator0, "separator0");
            // 
            // toolStripMenuItem_Save
            // 
            resources.ApplyResources(this.toolStripMenuItem_Save, "toolStripMenuItem_Save");
            this.toolStripMenuItem_Save.Name = "toolStripMenuItem_Save";
            this.toolStripMenuItem_Save.Click += new System.EventHandler(this.Save_Event_Click);
            // 
            // toolStripMenuItem_SaveAs
            // 
            resources.ApplyResources(this.toolStripMenuItem_SaveAs, "toolStripMenuItem_SaveAs");
            this.toolStripMenuItem_SaveAs.Name = "toolStripMenuItem_SaveAs";
            this.toolStripMenuItem_SaveAs.Click += new System.EventHandler(this.SaveAs_Event_Click);
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            resources.ApplyResources(this.separator1, "separator1");
            // 
            // toolStripMenuItem_Close
            // 
            resources.ApplyResources(this.toolStripMenuItem_Close, "toolStripMenuItem_Close");
            this.toolStripMenuItem_Close.Image = global::BKSMTool.Properties.Resources.delete_folder;
            this.toolStripMenuItem_Close.Name = "toolStripMenuItem_Close";
            this.toolStripMenuItem_Close.Click += new System.EventHandler(this.CloseBankAudioFile_Event_Click);
            // 
            // separator2
            // 
            this.separator2.Name = "separator2";
            resources.ApplyResources(this.separator2, "separator2");
            // 
            // toolStripMenuItem_Exit
            // 
            this.toolStripMenuItem_Exit.Image = global::BKSMTool.Properties.Resources.logout;
            this.toolStripMenuItem_Exit.Name = "toolStripMenuItem_Exit";
            resources.ApplyResources(this.toolStripMenuItem_Exit, "toolStripMenuItem_Exit");
            this.toolStripMenuItem_Exit.Click += new System.EventHandler(this.ExitApp_Event_Click);
            // 
            // separator3
            // 
            this.separator3.Name = "separator3";
            resources.ApplyResources(this.separator3, "separator3");
            // 
            // toolStripMenuItem_bySaltwatersam
            // 
            this.toolStripMenuItem_bySaltwatersam.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.toolStripMenuItem_bySaltwatersam, "toolStripMenuItem_bySaltwatersam");
            this.toolStripMenuItem_bySaltwatersam.ForeColor = System.Drawing.Color.Transparent;
            this.toolStripMenuItem_bySaltwatersam.Name = "toolStripMenuItem_bySaltwatersam";
            this.toolStripMenuItem_bySaltwatersam.ReadOnly = true;
            // 
            // separator4
            // 
            this.separator4.Name = "separator4";
            resources.ApplyResources(this.separator4, "separator4");
            // 
            // toolStripMenuItem_EDIT
            // 
            this.toolStripMenuItem_EDIT.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Undo,
            this.toolStripMenuItem_Redo,
            this.toolStripSeparator4,
            this.replaceSelectedAudioToolStripMenuItem});
            this.toolStripMenuItem_EDIT.Name = "toolStripMenuItem_EDIT";
            resources.ApplyResources(this.toolStripMenuItem_EDIT, "toolStripMenuItem_EDIT");
            // 
            // toolStripMenuItem_Undo
            // 
            resources.ApplyResources(this.toolStripMenuItem_Undo, "toolStripMenuItem_Undo");
            this.toolStripMenuItem_Undo.Image = global::BKSMTool.Properties.Resources.turn_left;
            this.toolStripMenuItem_Undo.Name = "toolStripMenuItem_Undo";
            this.toolStripMenuItem_Undo.Click += new System.EventHandler(this.Undo_Event_Click);
            // 
            // toolStripMenuItem_Redo
            // 
            resources.ApplyResources(this.toolStripMenuItem_Redo, "toolStripMenuItem_Redo");
            this.toolStripMenuItem_Redo.Image = global::BKSMTool.Properties.Resources.turn_right;
            this.toolStripMenuItem_Redo.Name = "toolStripMenuItem_Redo";
            this.toolStripMenuItem_Redo.Click += new System.EventHandler(this.Redo_Event_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // replaceSelectedAudioToolStripMenuItem
            // 
            resources.ApplyResources(this.replaceSelectedAudioToolStripMenuItem, "replaceSelectedAudioToolStripMenuItem");
            this.replaceSelectedAudioToolStripMenuItem.Image = global::BKSMTool.Properties.Resources.change_sound;
            this.replaceSelectedAudioToolStripMenuItem.Name = "replaceSelectedAudioToolStripMenuItem";
            this.replaceSelectedAudioToolStripMenuItem.Click += new System.EventHandler(this.ReplaceSelectedAudio_Event_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractAudioToolStripMenuItem,
            this.extractAllAudiosToolStripMenuItem,
            this.toolStripSeparator5,
            this.AssignTxTFileToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
            // 
            // extractAudioToolStripMenuItem
            // 
            resources.ApplyResources(this.extractAudioToolStripMenuItem, "extractAudioToolStripMenuItem");
            this.extractAudioToolStripMenuItem.Image = global::BKSMTool.Properties.Resources.download;
            this.extractAudioToolStripMenuItem.Name = "extractAudioToolStripMenuItem";
            this.extractAudioToolStripMenuItem.Click += new System.EventHandler(this.ExtractSelectedAudio_Event_Click);
            // 
            // extractAllAudiosToolStripMenuItem
            // 
            resources.ApplyResources(this.extractAllAudiosToolStripMenuItem, "extractAllAudiosToolStripMenuItem");
            this.extractAllAudiosToolStripMenuItem.Image = global::BKSMTool.Properties.Resources.downloadAll;
            this.extractAllAudiosToolStripMenuItem.Name = "extractAllAudiosToolStripMenuItem";
            this.extractAllAudiosToolStripMenuItem.Click += new System.EventHandler(this.ExtractAllAudios_Event_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // AssignTxTFileToolStripMenuItem
            // 
            resources.ApplyResources(this.AssignTxTFileToolStripMenuItem, "AssignTxTFileToolStripMenuItem");
            this.AssignTxTFileToolStripMenuItem.Image = global::BKSMTool.Properties.Resources.file;
            this.AssignTxTFileToolStripMenuItem.Name = "AssignTxTFileToolStripMenuItem";
            this.AssignTxTFileToolStripMenuItem.Click += new System.EventHandler(this.Assign_Event_Click);
            // 
            // contextMenuStrip_WEMList
            // 
            this.contextMenuStrip_WEMList.AllowDrop = true;
            resources.ApplyResources(this.contextMenuStrip_WEMList, "contextMenuStrip_WEMList");
            this.contextMenuStrip_WEMList.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip_WEMList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_BNKReplaceAudio,
            this.separator17,
            this.toolStripMenuItem_BNKExtractAudio,
            this.toolStripSeparator3,
            this.toolStripMenuItem_BNKExtractAllAudio});
            this.contextMenuStrip_WEMList.Name = "contextMenuStrip1";
            this.contextMenuStrip_WEMList.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.contextMenuStrip_WEMList.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // toolStripMenuItem_BNKReplaceAudio
            // 
            this.toolStripMenuItem_BNKReplaceAudio.Image = global::BKSMTool.Properties.Resources.change_sound;
            this.toolStripMenuItem_BNKReplaceAudio.Name = "toolStripMenuItem_BNKReplaceAudio";
            resources.ApplyResources(this.toolStripMenuItem_BNKReplaceAudio, "toolStripMenuItem_BNKReplaceAudio");
            this.toolStripMenuItem_BNKReplaceAudio.Click += new System.EventHandler(this.ReplaceSelectedAudio_Event_Click);
            // 
            // separator17
            // 
            this.separator17.Name = "separator17";
            resources.ApplyResources(this.separator17, "separator17");
            // 
            // toolStripMenuItem_BNKExtractAudio
            // 
            this.toolStripMenuItem_BNKExtractAudio.Image = global::BKSMTool.Properties.Resources.download;
            this.toolStripMenuItem_BNKExtractAudio.Name = "toolStripMenuItem_BNKExtractAudio";
            resources.ApplyResources(this.toolStripMenuItem_BNKExtractAudio, "toolStripMenuItem_BNKExtractAudio");
            this.toolStripMenuItem_BNKExtractAudio.Click += new System.EventHandler(this.ExtractSelectedAudio_Event_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripMenuItem_BNKExtractAllAudio
            // 
            this.toolStripMenuItem_BNKExtractAllAudio.Image = global::BKSMTool.Properties.Resources.downloadAll;
            this.toolStripMenuItem_BNKExtractAllAudio.Name = "toolStripMenuItem_BNKExtractAllAudio";
            resources.ApplyResources(this.toolStripMenuItem_BNKExtractAllAudio, "toolStripMenuItem_BNKExtractAllAudio");
            this.toolStripMenuItem_BNKExtractAllAudio.Click += new System.EventHandler(this.ExtractAllAudios_Event_Click);
            // 
            // _playerHidableTrackbarVolume
            // 
            resources.ApplyResources(this._playerHidableTrackbarVolume, "_playerHidableTrackbarVolume");
            this._playerHidableTrackbarVolume.BackColor = System.Drawing.Color.Transparent;
            this._playerHidableTrackbarVolume.Name = "_playerHidableTrackbarVolume";
            this._playerHidableTrackbarVolume.TabStop = false;
            this.FormToolTip.SetToolTip(this._playerHidableTrackbarVolume, resources.GetString("_playerHidableTrackbarVolume.ToolTip"));
            this._playerHidableTrackbarVolume.Volume = 0.5F;
            // 
            // panel1
            // 
            this.panel1.AllowDrop = true;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.lbl_NumberOfAudios);
            this.panel1.Controls.Add(this.olv_AudioListView);
            this.panel1.Controls.Add(this.gpBox_FilterBox);
            this.panel1.Name = "panel1";
            this.panel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.panel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // panel3
            // 
            this.panel3.AllowDrop = true;
            this.panel3.Controls.Add(this.player_lbl_AudioPosition);
            this.panel3.Controls.Add(this.player_lbl_AudioDuration);
            this.panel3.Controls.Add(this.player_TrackBar_AudioPosition);
            this.panel3.Controls.Add(this.notSelectable_Btn_PlayerMode);
            this.panel3.Controls.Add(this.notSelectable_btn_PlayerPreviousAudio);
            this.panel3.Controls.Add(this.notSelectable_btn_PlayerNextAudio);
            this.panel3.Controls.Add(this.notSelectable_btn_PlayerPlayPause);
            this.panel3.Controls.Add(this._playerHidableTrackbarVolume);
            this.panel3.Controls.Add(this.player_lbl_AudioName);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            this.panel3.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.panel3.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // player_lbl_AudioPosition
            // 
            resources.ApplyResources(this.player_lbl_AudioPosition, "player_lbl_AudioPosition");
            this.player_lbl_AudioPosition.BackColor = System.Drawing.Color.Transparent;
            this.player_lbl_AudioPosition.Name = "player_lbl_AudioPosition";
            // 
            // player_lbl_AudioDuration
            // 
            resources.ApplyResources(this.player_lbl_AudioDuration, "player_lbl_AudioDuration");
            this.player_lbl_AudioDuration.BackColor = System.Drawing.Color.Transparent;
            this.player_lbl_AudioDuration.Name = "player_lbl_AudioDuration";
            // 
            // player_TrackBar_AudioPosition
            // 
            resources.ApplyResources(this.player_TrackBar_AudioPosition, "player_TrackBar_AudioPosition");
            this.player_TrackBar_AudioPosition.BackColor = System.Drawing.Color.Transparent;
            this.player_TrackBar_AudioPosition.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.player_TrackBar_AudioPosition.BarPenColorTop = System.Drawing.Color.DimGray;
            this.player_TrackBar_AudioPosition.BorderRoundRectSize = new System.Drawing.Size(9, 9);
            this.player_TrackBar_AudioPosition.DrawSemitransparentThumb = false;
            this.player_TrackBar_AudioPosition.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.player_TrackBar_AudioPosition.ElapsedPenColorBottom = System.Drawing.Color.DimGray;
            this.player_TrackBar_AudioPosition.ElapsedPenColorTop = System.Drawing.Color.DimGray;
            this.player_TrackBar_AudioPosition.Name = "player_TrackBar_AudioPosition";
            this.player_TrackBar_AudioPosition.TabStop = false;
            this.player_TrackBar_AudioPosition.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.player_TrackBar_AudioPosition.ThumbOuterColor = System.Drawing.Color.Gray;
            this.player_TrackBar_AudioPosition.ThumbPenColor = System.Drawing.Color.Black;
            this.player_TrackBar_AudioPosition.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.player_TrackBar_AudioPosition.ThumbSize = new System.Drawing.Size(8, 8);
            // 
            // notSelectable_Btn_PlayerMode
            // 
            resources.ApplyResources(this.notSelectable_Btn_PlayerMode, "notSelectable_Btn_PlayerMode");
            this.notSelectable_Btn_PlayerMode.BackgroundImage = global::BKSMTool.Properties.Resources.SingleButton;
            this.notSelectable_Btn_PlayerMode.Name = "notSelectable_Btn_PlayerMode";
            this.notSelectable_Btn_PlayerMode.TabStop = false;
            this.notSelectable_Btn_PlayerMode.UseVisualStyleBackColor = true;
            this.notSelectable_Btn_PlayerMode.Click += new System.EventHandler(this.PlayerMode_Event_Click);
            // 
            // notSelectable_btn_PlayerPreviousAudio
            // 
            resources.ApplyResources(this.notSelectable_btn_PlayerPreviousAudio, "notSelectable_btn_PlayerPreviousAudio");
            this.notSelectable_btn_PlayerPreviousAudio.BackgroundImage = global::BKSMTool.Properties.Resources.LastButton;
            this.notSelectable_btn_PlayerPreviousAudio.Name = "notSelectable_btn_PlayerPreviousAudio";
            this.notSelectable_btn_PlayerPreviousAudio.TabStop = false;
            this.notSelectable_btn_PlayerPreviousAudio.UseVisualStyleBackColor = true;
            this.notSelectable_btn_PlayerPreviousAudio.Click += new System.EventHandler(this.PlayerPreviousAudio_Event_Click);
            // 
            // notSelectable_btn_PlayerNextAudio
            // 
            resources.ApplyResources(this.notSelectable_btn_PlayerNextAudio, "notSelectable_btn_PlayerNextAudio");
            this.notSelectable_btn_PlayerNextAudio.BackgroundImage = global::BKSMTool.Properties.Resources.NextButton;
            this.notSelectable_btn_PlayerNextAudio.Name = "notSelectable_btn_PlayerNextAudio";
            this.notSelectable_btn_PlayerNextAudio.TabStop = false;
            this.notSelectable_btn_PlayerNextAudio.UseVisualStyleBackColor = true;
            this.notSelectable_btn_PlayerNextAudio.Click += new System.EventHandler(this.PlayerNextAudio_Event_Click);
            // 
            // notSelectable_btn_PlayerPlayPause
            // 
            resources.ApplyResources(this.notSelectable_btn_PlayerPlayPause, "notSelectable_btn_PlayerPlayPause");
            this.notSelectable_btn_PlayerPlayPause.BackgroundImage = global::BKSMTool.Properties.Resources.PlayButton;
            this.notSelectable_btn_PlayerPlayPause.Name = "notSelectable_btn_PlayerPlayPause";
            this.notSelectable_btn_PlayerPlayPause.TabStop = false;
            this.notSelectable_btn_PlayerPlayPause.UseVisualStyleBackColor = true;
            this.notSelectable_btn_PlayerPlayPause.Click += new System.EventHandler(this.PlayerPlayPause_Event_Click);
            // 
            // player_lbl_AudioName
            // 
            resources.ApplyResources(this.player_lbl_AudioName, "player_lbl_AudioName");
            this.player_lbl_AudioName.BackColor = System.Drawing.Color.Transparent;
            this.player_lbl_AudioName.Name = "player_lbl_AudioName";
            // 
            // lbl_NumberOfAudios
            // 
            this.lbl_NumberOfAudios.AllowDrop = true;
            resources.ApplyResources(this.lbl_NumberOfAudios, "lbl_NumberOfAudios");
            this.lbl_NumberOfAudios.Name = "lbl_NumberOfAudios";
            this.lbl_NumberOfAudios.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.lbl_NumberOfAudios.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // olv_AudioListView
            // 
            this.olv_AudioListView.AllColumns.Add(this.OLVC_AudioState);
            this.olv_AudioListView.AllColumns.Add(this.OLVC_ID);
            this.olv_AudioListView.AllColumns.Add(this.OLVC_RelatedName);
            this.olv_AudioListView.AllColumns.Add(this.OLVC_DURATION);
            this.olv_AudioListView.AllColumns.Add(this.OLVC_DataArraySizeAsString);
            this.olv_AudioListView.AllColumns.Add(this.OLVC_IsModified);
            this.olv_AudioListView.AllowDrop = true;
            this.olv_AudioListView.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            resources.ApplyResources(this.olv_AudioListView, "olv_AudioListView");
            this.olv_AudioListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.olv_AudioListView.CausesValidation = false;
            this.olv_AudioListView.CellEditUseWholeCell = false;
            this.olv_AudioListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.OLVC_AudioState,
            this.OLVC_ID,
            this.OLVC_RelatedName,
            this.OLVC_DURATION,
            this.OLVC_DataArraySizeAsString,
            this.OLVC_IsModified});
            this.olv_AudioListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.olv_AudioListView.FullRowSelect = true;
            this.olv_AudioListView.HasCollapsibleGroups = false;
            this.olv_AudioListView.HeaderFormatStyle = this.headerFormatStyle1;
            this.olv_AudioListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.olv_AudioListView.HideSelection = false;
            this.olv_AudioListView.IsSearchOnSortColumn = false;
            this.olv_AudioListView.MultiSelect = false;
            this.olv_AudioListView.Name = "olv_AudioListView";
            this.olv_AudioListView.PersistentCheckBoxes = false;
            this.olv_AudioListView.SelectAllOnControlA = false;
            this.olv_AudioListView.SelectColumnsMenuStaysOpen = false;
            this.olv_AudioListView.SelectColumnsOnRightClick = false;
            this.olv_AudioListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.olv_AudioListView.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(224)))), ((int)(((byte)(247)))));
            this.olv_AudioListView.SelectedForeColor = System.Drawing.Color.Black;
            this.olv_AudioListView.ShowFilterMenuOnRightClick = false;
            this.olv_AudioListView.ShowGroups = false;
            this.olv_AudioListView.ShowImagesOnSubItems = true;
            this.olv_AudioListView.ShowSortIndicators = false;
            this.olv_AudioListView.SortGroupItemsByPrimaryColumn = false;
            this.olv_AudioListView.TabStop = false;
            this.olv_AudioListView.TriggerCellOverEventsWhenOverHeader = false;
            this.olv_AudioListView.UnfocusedSelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(224)))), ((int)(((byte)(247)))));
            this.olv_AudioListView.UseAlternatingBackColors = true;
            this.olv_AudioListView.UseCellFormatEvents = true;
            this.olv_AudioListView.UseCompatibleStateImageBehavior = false;
            this.olv_AudioListView.UseFiltering = true;
            this.olv_AudioListView.UseHotItem = true;
            this.olv_AudioListView.UseNotifyPropertyChanged = true;
            this.olv_AudioListView.UseOverlays = false;
            this.olv_AudioListView.View = System.Windows.Forms.View.Details;
            this.olv_AudioListView.ButtonClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.AudioListView_Event_ButtonClick);
            this.olv_AudioListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.AudioListView_Event_FormatRow);
            this.olv_AudioListView.ItemsChanged += new System.EventHandler<BrightIdeasSoftware.ItemsChangedEventArgs>(this.AudioListView_Event_ItemsChanged);
            this.olv_AudioListView.SelectionChanged += new System.EventHandler(this.AudioListView_Event_SelectionChanged);
            this.olv_AudioListView.ItemActivate += new System.EventHandler(this.AudioListView_Event_ItemActivate);
            this.olv_AudioListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.olv_AudioListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.olv_AudioListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AudioListView_Event_KeyDown);
            // 
            // OLVC_AudioState
            // 
            this.OLVC_AudioState.AspectName = "AudioState";
            this.OLVC_AudioState.AspectToStringFormat = "";
            this.OLVC_AudioState.AutoCompleteEditor = false;
            this.OLVC_AudioState.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.OLVC_AudioState.ButtonSize = new System.Drawing.Size(24, 24);
            this.OLVC_AudioState.ButtonSizing = BrightIdeasSoftware.OLVColumn.ButtonSizingMode.FixedBounds;
            this.OLVC_AudioState.EnableButtonWhenItemIsDisabled = true;
            this.OLVC_AudioState.Groupable = false;
            this.OLVC_AudioState.HeaderCheckBoxUpdatesRowCheckBoxes = false;
            this.OLVC_AudioState.HeaderFormatStyle = this.headerFormatStyle2;
            this.OLVC_AudioState.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.OLVC_AudioState.ImageAspectName = "";
            this.OLVC_AudioState.IsButton = true;
            this.OLVC_AudioState.IsEditable = false;
            this.OLVC_AudioState.MaximumWidth = 25;
            this.OLVC_AudioState.MinimumWidth = 25;
            this.OLVC_AudioState.Renderer = this.SoundPlayerStateRenderer;
            this.OLVC_AudioState.ShowTextInHeader = false;
            this.OLVC_AudioState.Sortable = false;
            resources.ApplyResources(this.OLVC_AudioState, "OLVC_AudioState");
            this.OLVC_AudioState.UseFiltering = false;
            // 
            // SoundPlayerStateRenderer
            // 
            this.SoundPlayerStateRenderer.ButtonSize = new System.Drawing.Size(24, 24);
            this.SoundPlayerStateRenderer.CellVerticalAlignment = System.Drawing.StringAlignment.Center;
            this.SoundPlayerStateRenderer.ImageSize = new System.Drawing.Size(14, 14);
            this.SoundPlayerStateRenderer.MaximumButtonSize = new System.Drawing.Size(-1, -1);
            this.SoundPlayerStateRenderer.MaximumImageSize = new System.Drawing.Size(-1, -1);
            this.SoundPlayerStateRenderer.MinimumButtonSize = new System.Drawing.Size(-1, -1);
            this.SoundPlayerStateRenderer.MinimumImageSize = new System.Drawing.Size(-1, -1);
            this.SoundPlayerStateRenderer.PauseImage = global::BKSMTool.Properties.Resources.PlayButton;
            this.SoundPlayerStateRenderer.PlayImage = global::BKSMTool.Properties.Resources.PauseButton;
            this.SoundPlayerStateRenderer.Spacing = 0;
            this.SoundPlayerStateRenderer.StopImage = global::BKSMTool.Properties.Resources.PlayButton;
            this.SoundPlayerStateRenderer.UseGdiTextRendering = false;
            // 
            // OLVC_ID
            // 
            this.OLVC_ID.AspectName = "WemId";
            this.OLVC_ID.AutoCompleteEditor = false;
            this.OLVC_ID.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.OLVC_ID.Groupable = false;
            this.OLVC_ID.HeaderCheckBoxUpdatesRowCheckBoxes = false;
            this.OLVC_ID.HeaderFormatStyle = this.headerFormatStyle1;
            this.OLVC_ID.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.OLVC_ID.IsEditable = false;
            this.OLVC_ID.MaximumWidth = 80;
            this.OLVC_ID.MinimumWidth = 80;
            this.OLVC_ID.Sortable = false;
            resources.ApplyResources(this.OLVC_ID, "OLVC_ID");
            // 
            // OLVC_RelatedName
            // 
            this.OLVC_RelatedName.AspectName = "WemEventName";
            this.OLVC_RelatedName.AutoCompleteEditor = false;
            this.OLVC_RelatedName.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.OLVC_RelatedName.FillsFreeSpace = true;
            this.OLVC_RelatedName.Groupable = false;
            this.OLVC_RelatedName.HeaderCheckBoxUpdatesRowCheckBoxes = false;
            this.OLVC_RelatedName.HeaderFormatStyle = this.headerFormatStyle1;
            this.OLVC_RelatedName.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.OLVC_RelatedName.IsEditable = false;
            this.OLVC_RelatedName.Sortable = false;
            resources.ApplyResources(this.OLVC_RelatedName, "OLVC_RelatedName");
            // 
            // OLVC_DURATION
            // 
            this.OLVC_DURATION.AspectName = "AudioDurationAsString";
            this.OLVC_DURATION.AutoCompleteEditor = false;
            this.OLVC_DURATION.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.OLVC_DURATION.Groupable = false;
            this.OLVC_DURATION.HeaderCheckBoxUpdatesRowCheckBoxes = false;
            this.OLVC_DURATION.HeaderFormatStyle = this.headerFormatStyle1;
            this.OLVC_DURATION.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.OLVC_DURATION.IsEditable = false;
            this.OLVC_DURATION.MaximumWidth = 90;
            this.OLVC_DURATION.MinimumWidth = 90;
            this.OLVC_DURATION.Searchable = false;
            this.OLVC_DURATION.Sortable = false;
            resources.ApplyResources(this.OLVC_DURATION, "OLVC_DURATION");
            // 
            // OLVC_DataArraySizeAsString
            // 
            this.OLVC_DataArraySizeAsString.AspectName = "WemSizeAsString";
            this.OLVC_DataArraySizeAsString.AutoCompleteEditor = false;
            this.OLVC_DataArraySizeAsString.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.OLVC_DataArraySizeAsString.Groupable = false;
            this.OLVC_DataArraySizeAsString.HeaderCheckBoxUpdatesRowCheckBoxes = false;
            this.OLVC_DataArraySizeAsString.HeaderFormatStyle = this.headerFormatStyle1;
            this.OLVC_DataArraySizeAsString.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.OLVC_DataArraySizeAsString.IsEditable = false;
            this.OLVC_DataArraySizeAsString.MaximumWidth = 85;
            this.OLVC_DataArraySizeAsString.MinimumWidth = 85;
            this.OLVC_DataArraySizeAsString.Searchable = false;
            this.OLVC_DataArraySizeAsString.Sortable = false;
            resources.ApplyResources(this.OLVC_DataArraySizeAsString, "OLVC_DataArraySizeAsString");
            // 
            // OLVC_IsModified
            // 
            this.OLVC_IsModified.AspectName = "IsModified";
            this.OLVC_IsModified.AspectToStringFormat = "";
            this.OLVC_IsModified.AutoCompleteEditor = false;
            this.OLVC_IsModified.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.OLVC_IsModified.Groupable = false;
            this.OLVC_IsModified.HeaderCheckBoxUpdatesRowCheckBoxes = false;
            this.OLVC_IsModified.HeaderFormatStyle = this.headerFormatStyle2;
            this.OLVC_IsModified.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.OLVC_IsModified.Hideable = false;
            this.OLVC_IsModified.IsEditable = false;
            this.OLVC_IsModified.MaximumWidth = 25;
            this.OLVC_IsModified.MinimumWidth = 25;
            this.OLVC_IsModified.Renderer = this.audioSavedStateRenderer1;
            this.OLVC_IsModified.Searchable = false;
            this.OLVC_IsModified.ShowTextInHeader = false;
            this.OLVC_IsModified.Sortable = false;
            resources.ApplyResources(this.OLVC_IsModified, "OLVC_IsModified");
            this.OLVC_IsModified.UseFiltering = false;
            // 
            // panel2
            // 
            this.panel2.AllowDrop = true;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Name = "panel2";
            this.panel2.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.panel2.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "PlaybackState";
            this.olvColumn1.AspectToStringFormat = "";
            this.olvColumn1.AutoCompleteEditor = false;
            this.olvColumn1.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.olvColumn1.ButtonSizing = BrightIdeasSoftware.OLVColumn.ButtonSizingMode.CellBounds;
            this.olvColumn1.EnableButtonWhenItemIsDisabled = true;
            this.olvColumn1.Groupable = false;
            this.olvColumn1.HeaderCheckBoxUpdatesRowCheckBoxes = false;
            this.olvColumn1.HeaderFormatStyle = this.headerFormatStyle2;
            this.olvColumn1.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumn1.ImageAspectName = "";
            this.olvColumn1.IsButton = true;
            this.olvColumn1.MaximumWidth = 23;
            this.olvColumn1.MinimumWidth = 23;
            this.olvColumn1.Renderer = this.SoundPlayerStateRenderer;
            this.olvColumn1.ShowTextInHeader = false;
            this.olvColumn1.Sortable = false;
            resources.ApplyResources(this.olvColumn1, "olvColumn1");
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutBKSMToolToolStripMenuItem,
            this.checkForUpdateToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // aboutBKSMToolToolStripMenuItem
            // 
            this.aboutBKSMToolToolStripMenuItem.Name = "aboutBKSMToolToolStripMenuItem";
            resources.ApplyResources(this.aboutBKSMToolToolStripMenuItem, "aboutBKSMToolToolStripMenuItem");
            // 
            // checkForUpdateToolStripMenuItem
            // 
            this.checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
            resources.ApplyResources(this.checkForUpdateToolStripMenuItem, "checkForUpdateToolStripMenuItem");
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStripForFileStatus);
            this.Controls.Add(this.menuStrip);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExitApp_Event_Click);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.MainForm_PreviewKeyDown);
            this.statusStripForFileStatus.ResumeLayout(false);
            this.statusStripForFileStatus.PerformLayout();
            this.gpBox_FilterBox.ResumeLayout(false);
            this.gpBox_FilterBox.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.contextMenuStrip_WEMList.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olv_AudioListView)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStripForFileStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStrip_LabelForStatus;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_FILE;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Open;
        private System.Windows.Forms.ToolStripSeparator separator0;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Save;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SaveAs;
        private System.Windows.Forms.ToolStripSeparator separator1;
        private System.Windows.Forms.ToolStripSeparator separator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Exit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_EDIT;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Undo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Redo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Close;
        private System.Windows.Forms.ToolStripSeparator separator3;
        private System.Windows.Forms.ToolStripSeparator separator4;
        private System.Windows.Forms.ToolStripProgressBar toolStrip_ProgressBar;
        private ToolStripTextBox toolStripMenuItem_bySaltwatersam;
        private ContextMenuStrip contextMenuStrip_WEMList;
        private ToolStripMenuItem toolStripMenuItem_BNKReplaceAudio;
        private ToolStripSeparator separator17;
        private ToolStripMenuItem toolStripMenuItem_BNKExtractAudio;
        private GroupBox gpBox_FilterBox;
        private TextBox txtbox_FilterBox;
        private ToolTip FormToolTip;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolStripMenuItem_BNKExtractAllAudio;
        private BrightIdeasSoftware.HeaderFormatStyle headerFormatStyle1;
        private BrightIdeasSoftware.HeaderFormatStyle headerFormatStyle2;
        private SoundPlayerStateRenderer SoundPlayerStateRenderer;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private Panel panel1;
        private BrightIdeasSoftware.ObjectListView olv_AudioListView;
        private BrightIdeasSoftware.OLVColumn OLVC_ID;
        private BrightIdeasSoftware.OLVColumn OLVC_RelatedName;
        private BrightIdeasSoftware.OLVColumn OLVC_DURATION;
        private BrightIdeasSoftware.OLVColumn OLVC_DataArraySizeAsString;
        private PlayerLabelAudioPosition player_lbl_AudioPosition;
        private PlayerTrackBarAudioPosition player_TrackBar_AudioPosition;
        private PlayerLabelAudioName player_lbl_AudioName;
        private PlayerLabelAudioDuration player_lbl_AudioDuration;
        private NotSelectableButton notSelectable_Btn_PlayerMode;
        private NotSelectableButton notSelectable_btn_PlayerPreviousAudio;
        private NotSelectableButton notSelectable_btn_PlayerNextAudio;
        private NotSelectableButton notSelectable_btn_PlayerPlayPause;
        private PlayerHidableTrackbarVolume _playerHidableTrackbarVolume;
        private Label lbl_NumberOfAudios;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem replaceSelectedAudioToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem extractAudioToolStripMenuItem;
        private ToolStripMenuItem extractAllAudiosToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem AssignTxTFileToolStripMenuItem;
        private Panel panel2;
        private Panel panel3;
        private AudioSavedStateRenderer audioSavedStateRenderer1;
        private BrightIdeasSoftware.OLVColumn OLVC_AudioState;
        private BrightIdeasSoftware.OLVColumn OLVC_IsModified;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutBKSMToolToolStripMenuItem;
        private ToolStripMenuItem checkForUpdateToolStripMenuItem;
    }
}

