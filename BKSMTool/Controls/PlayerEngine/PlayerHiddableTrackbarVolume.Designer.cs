namespace BKSMTool.Controls.PlayerEngine
{
    partial class PlayerHidableTrackbarVolume
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

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.notSelectableButtonShowVolumeBar = new BKSMTool.Controls.NotSelectableButton();
            this.VolumeTrackbar = new BKSMTool.Controls.AdvancedTrackbar();
            this.SuspendLayout();
            // 
            // notSelectable_Button_ShowVolumeBar
            // 
            this.notSelectableButtonShowVolumeBar.BackgroundImage = global::BKSMTool.Properties.Resources.Volume_Medium;
            this.notSelectableButtonShowVolumeBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.notSelectableButtonShowVolumeBar.Location = new System.Drawing.Point(0, -1);
            this.notSelectableButtonShowVolumeBar.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.notSelectableButtonShowVolumeBar.Name = "notSelectableButtonShowVolumeBar";
            this.notSelectableButtonShowVolumeBar.Size = new System.Drawing.Size(27, 27);
            this.notSelectableButtonShowVolumeBar.TabIndex = 45;
            this.notSelectableButtonShowVolumeBar.TabStop = false;
            this.notSelectableButtonShowVolumeBar.UseVisualStyleBackColor = true;
            // 
            // advancedTrackbar1
            // 
            this.VolumeTrackbar.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.VolumeTrackbar.BarPenColorTop = System.Drawing.Color.DimGray;
            this.VolumeTrackbar.BorderRoundRectSize = new System.Drawing.Size(9, 9);
            this.VolumeTrackbar.ColorSchema = BKSMTool.Controls.AdvancedTrackbar.ColorSchemas.DarkGrey;
            this.VolumeTrackbar.DrawSemitransparentThumb = false;
            this.VolumeTrackbar.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.VolumeTrackbar.ElapsedPenColorBottom = System.Drawing.Color.DimGray;
            this.VolumeTrackbar.ElapsedPenColorTop = System.Drawing.Color.DimGray;
            this.VolumeTrackbar.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.VolumeTrackbar.LargeChange = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.VolumeTrackbar.Location = new System.Drawing.Point(33, 2);
            this.VolumeTrackbar.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.VolumeTrackbar.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.VolumeTrackbar.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.VolumeTrackbar.Name = "VolumeTrackbar";
            this.VolumeTrackbar.Padding = 2;
            this.VolumeTrackbar.ScaleDivisions = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.VolumeTrackbar.ScaleSubDivisions = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.VolumeTrackbar.ShowDivisionsText = false;
            this.VolumeTrackbar.ShowSmallScale = false;
            this.VolumeTrackbar.Size = new System.Drawing.Size(101, 20);
            this.VolumeTrackbar.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.VolumeTrackbar.TabIndex = 46;
            this.VolumeTrackbar.Text = "advancedTrackbar1";
            this.VolumeTrackbar.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.VolumeTrackbar.ThumbOuterColor = System.Drawing.Color.Gray;
            this.VolumeTrackbar.ThumbPenColor = System.Drawing.Color.Black;
            this.VolumeTrackbar.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.VolumeTrackbar.ThumbSize = new System.Drawing.Size(8, 16);
            this.VolumeTrackbar.TickAdd = 0F;
            this.VolumeTrackbar.TickColor = System.Drawing.Color.White;
            this.VolumeTrackbar.TickDivide = 0F;
            this.VolumeTrackbar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.VolumeTrackbar.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // Player_HiddableTrackbar_Volume
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.VolumeTrackbar);
            this.Controls.Add(this.notSelectableButtonShowVolumeBar);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "PlayerHidableTrackbarVolume";
            this.Size = new System.Drawing.Size(137, 26);
            this.ResumeLayout(false);

        }

        #endregion
        private NotSelectableButton notSelectableButtonShowVolumeBar;
        private BKSMTool.Controls.AdvancedTrackbar VolumeTrackbar;
    }
}
