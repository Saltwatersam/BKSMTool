namespace BKSMTool.Controls.ModalForm
{
    partial class UpdateFormAutomatic
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateFormAutomatic));
            this.messageLabel = new System.Windows.Forms.Label();
            this.dontRemindButton = new BKSMTool.Controls.NotSelectableButton();
            this.noButton = new BKSMTool.Controls.NotSelectableButton();
            this.yesButton = new BKSMTool.Controls.NotSelectableButton();
            this.SuspendLayout();
            // 
            // messageLabel
            // 
            this.messageLabel.AutoEllipsis = true;
            this.messageLabel.AutoSize = true;
            this.messageLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageLabel.Location = new System.Drawing.Point(12, 9);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(284, 30);
            this.messageLabel.TabIndex = 0;
            this.messageLabel.Text = "A new version (x.x.x.x) of the application is available.\r\nDo you want to download" +
    " it ?";
            // 
            // dontRemindButton
            // 
            this.dontRemindButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.dontRemindButton.AutoSize = true;
            this.dontRemindButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dontRemindButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dontRemindButton.Location = new System.Drawing.Point(178, 66);
            this.dontRemindButton.Margin = new System.Windows.Forms.Padding(0);
            this.dontRemindButton.Name = "dontRemindButton";
            this.dontRemindButton.Size = new System.Drawing.Size(129, 25);
            this.dontRemindButton.TabIndex = 3;
            this.dontRemindButton.Text = "No, Don\'t remind me";
            this.dontRemindButton.UseVisualStyleBackColor = true;
            this.dontRemindButton.Click += new System.EventHandler(this.DontRemindButton_Click);
            // 
            // noButton
            // 
            this.noButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.noButton.AutoSize = true;
            this.noButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.noButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.noButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noButton.Location = new System.Drawing.Point(53, 66);
            this.noButton.Margin = new System.Windows.Forms.Padding(0);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(33, 25);
            this.noButton.TabIndex = 2;
            this.noButton.Text = "No";
            this.noButton.UseVisualStyleBackColor = true;
            // 
            // yesButton
            // 
            this.yesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.yesButton.AutoSize = true;
            this.yesButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.yesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.yesButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yesButton.Location = new System.Drawing.Point(9, 66);
            this.yesButton.Margin = new System.Windows.Forms.Padding(0);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(34, 25);
            this.yesButton.TabIndex = 1;
            this.yesButton.Text = "Yes";
            this.yesButton.UseVisualStyleBackColor = true;
            // 
            // UpdateFormAutomatic
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(316, 100);
            this.Controls.Add(this.dontRemindButton);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.yesButton);
            this.Controls.Add(this.messageLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateFormAutomatic";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Update Available!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label messageLabel;
        private NotSelectableButton yesButton;
        private NotSelectableButton noButton;
        private NotSelectableButton dontRemindButton;
    }
}