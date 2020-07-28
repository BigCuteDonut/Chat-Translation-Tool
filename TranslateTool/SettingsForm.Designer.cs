namespace TranslateTool
{
    partial class SettingsForm
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
            this.InfoCheck = new System.Windows.Forms.CheckBox();
            this.CloseButton = new System.Windows.Forms.Panel();
            this.AutoHideCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // InfoCheck
            // 
            this.InfoCheck.AutoSize = true;
            this.InfoCheck.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InfoCheck.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.InfoCheck.Location = new System.Drawing.Point(12, 50);
            this.InfoCheck.Name = "InfoCheck";
            this.InfoCheck.Size = new System.Drawing.Size(375, 26);
            this.InfoCheck.TabIndex = 0;
            this.InfoCheck.Text = "Show addition info when translating input.";
            this.InfoCheck.UseVisualStyleBackColor = true;
            this.InfoCheck.CheckedChanged += new System.EventHandler(this.InfoCheck_CheckedChanged);
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.BackgroundImage = global::TranslateTool.Properties.Resources.Close;
            this.CloseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.CloseButton.Location = new System.Drawing.Point(387, 0);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(40, 40);
            this.CloseButton.TabIndex = 15;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Clicked);
            this.CloseButton.MouseEnter += new System.EventHandler(this.CloseButton_MouseEnter);
            this.CloseButton.MouseLeave += new System.EventHandler(this.CloseButton_MouseLeave);
            // 
            // AutoHideCheck
            // 
            this.AutoHideCheck.AutoSize = true;
            this.AutoHideCheck.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AutoHideCheck.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.AutoHideCheck.Location = new System.Drawing.Point(12, 82);
            this.AutoHideCheck.Name = "AutoHideCheck";
            this.AutoHideCheck.Size = new System.Drawing.Size(401, 26);
            this.AutoHideCheck.TabIndex = 16;
            this.AutoHideCheck.Text = "Prevent window from becoming transparent.";
            this.AutoHideCheck.UseVisualStyleBackColor = true;
            this.AutoHideCheck.CheckedChanged += new System.EventHandler(this.AutoHideCheck_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(427, 274);
            this.Controls.Add(this.AutoHideCheck);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.InfoCheck);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.Deactivate += new System.EventHandler(this.SettingsForm_Deactivate);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel CloseButton;
        public System.Windows.Forms.CheckBox InfoCheck;
        public System.Windows.Forms.CheckBox AutoHideCheck;
    }
}