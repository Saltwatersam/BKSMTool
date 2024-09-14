namespace BKSMTool.Controls.ModalForm
{
    partial class NoUpdateFormManual
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NoUpdateFormManual));
            this.messageLabel = new System.Windows.Forms.Label();
            this.EnableDisableAutomaticUpdateCheck = new BKSMTool.Controls.NotSelectableButton();
            this.OkButton = new BKSMTool.Controls.NotSelectableButton();
            this.SuspendLayout();
            // 
            // messageLabel
            // 
            this.messageLabel.AutoEllipsis = true;
            this.messageLabel.AutoSize = true;
            this.messageLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageLabel.Location = new System.Drawing.Point(12, 9);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(191, 15);
            this.messageLabel.TabIndex = 0;
            this.messageLabel.Text = "You already have the latest version.";
            // 
            // EnableDisableAutomaticUpdateCheck
            // 
            this.EnableDisableAutomaticUpdateCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EnableDisableAutomaticUpdateCheck.AutoSize = true;
            this.EnableDisableAutomaticUpdateCheck.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.EnableDisableAutomaticUpdateCheck.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnableDisableAutomaticUpdateCheck.Location = new System.Drawing.Point(119, 66);
            this.EnableDisableAutomaticUpdateCheck.Margin = new System.Windows.Forms.Padding(0);
            this.EnableDisableAutomaticUpdateCheck.Name = "EnableDisableAutomaticUpdateCheck";
            this.EnableDisableAutomaticUpdateCheck.Size = new System.Drawing.Size(188, 25);
            this.EnableDisableAutomaticUpdateCheck.TabIndex = 3;
            this.EnableDisableAutomaticUpdateCheck.Text = "Enable automatic update checks";
            this.EnableDisableAutomaticUpdateCheck.UseVisualStyleBackColor = true;
            this.EnableDisableAutomaticUpdateCheck.Click += new System.EventHandler(this.EnableDisableAutoUpdateButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkButton.AutoSize = true;
            this.OkButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OkButton.Location = new System.Drawing.Point(11, 66);
            this.OkButton.Margin = new System.Windows.Forms.Padding(0);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(32, 25);
            this.OkButton.TabIndex = 1;
            this.OkButton.Text = "Ok";
            this.OkButton.UseVisualStyleBackColor = true;
            // 
            // NoUpdateFormManual
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(316, 100);
            this.Controls.Add(this.EnableDisableAutomaticUpdateCheck);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.messageLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NoUpdateFormManual";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "No update available.";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label messageLabel;
        private NotSelectableButton OkButton;
        private NotSelectableButton EnableDisableAutomaticUpdateCheck;
    }
}