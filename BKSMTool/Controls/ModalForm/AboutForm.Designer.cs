namespace BKSMTool.Controls.ModalForm
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.AboutLabel = new System.Windows.Forms.Label();
            this.OkButton = new BKSMTool.Controls.NotSelectableButton();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.LinkLbl = new System.Windows.Forms.LinkLabel();
            this.InfoLinkLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AboutLabel
            // 
            this.AboutLabel.AutoEllipsis = true;
            this.AboutLabel.AutoSize = true;
            this.AboutLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AboutLabel.Location = new System.Drawing.Point(12, 9);
            this.AboutLabel.Name = "AboutLabel";
            this.AboutLabel.Size = new System.Drawing.Size(660, 210);
            this.AboutLabel.TabIndex = 0;
            this.AboutLabel.Text = resources.GetString("AboutLabel.Text");
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OkButton.Location = new System.Drawing.Point(608, 282);
            this.OkButton.Margin = new System.Windows.Forms.Padding(0);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(65, 25);
            this.OkButton.TabIndex = 1;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            // 
            // VersionLabel
            // 
            this.VersionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.VersionLabel.AutoEllipsis = true;
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.VersionLabel.Location = new System.Drawing.Point(12, 292);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(248, 15);
            this.VersionLabel.TabIndex = 2;
            this.VersionLabel.Text = "BKSMTool version : \"1.0.0.0\",  by Saltwatersam";
            // 
            // LinkLbl
            // 
            this.LinkLbl.AutoSize = true;
            this.LinkLbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LinkLbl.Location = new System.Drawing.Point(170, 229);
            this.LinkLbl.Margin = new System.Windows.Forms.Padding(1, 10, 3, 0);
            this.LinkLbl.Name = "LinkLbl";
            this.LinkLbl.Size = new System.Drawing.Size(230, 15);
            this.LinkLbl.TabIndex = 3;
            this.LinkLbl.TabStop = true;
            this.LinkLbl.Text = "https://saltwatersam.github.io/BKSMTool/";
            this.LinkLbl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLbl_LinkClicked);
            // 
            // InfoLinkLbl
            // 
            this.InfoLinkLbl.AutoSize = true;
            this.InfoLinkLbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InfoLinkLbl.Location = new System.Drawing.Point(12, 229);
            this.InfoLinkLbl.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.InfoLinkLbl.Name = "InfoLinkLbl";
            this.InfoLinkLbl.Size = new System.Drawing.Size(154, 15);
            this.InfoLinkLbl.TabIndex = 4;
            this.InfoLinkLbl.Text = "For more information, visit :";
            // 
            // AboutForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(682, 316);
            this.Controls.Add(this.InfoLinkLbl);
            this.Controls.Add(this.LinkLbl);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.AboutLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "BKSMTool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AboutLabel;
        private NotSelectableButton OkButton;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.LinkLabel LinkLbl;
        private System.Windows.Forms.Label InfoLinkLbl;
    }
}