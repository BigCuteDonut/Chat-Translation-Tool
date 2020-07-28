namespace TranslateTool
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.Output = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.Input = new System.Windows.Forms.TextBox();
            this.OutputPadding = new System.Windows.Forms.Panel();
            this.InputPadding = new System.Windows.Forms.Panel();
            this.EnterButton = new System.Windows.Forms.Button();
            this.ResizeSE = new System.Windows.Forms.Label();
            this.ResizeS = new System.Windows.Forms.Label();
            this.MoveButton = new System.Windows.Forms.Label();
            this.ResizeN = new System.Windows.Forms.Label();
            this.ResizeNE = new System.Windows.Forms.Label();
            this.ResizeSW = new System.Windows.Forms.Label();
            this.ResizeNW = new System.Windows.Forms.Label();
            this.ResizeW = new System.Windows.Forms.Label();
            this.ResizeE = new System.Windows.Forms.Label();
            this.SettingsButton = new System.Windows.Forms.Panel();
            this.CloseButton = new System.Windows.Forms.Panel();
            this.AutoShowButton = new System.Windows.Forms.Panel();
            this.ClickThroughButton = new System.Windows.Forms.Panel();
            this.AutoScrollButton = new System.Windows.Forms.Panel();
            this.InputPadding.SuspendLayout();
            this.SuspendLayout();
            // 
            // Output
            // 
            this.Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(29)))), ((int)(((byte)(44)))));
            this.Output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Output.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.Output.Location = new System.Drawing.Point(12, 47);
            this.Output.MaxLength = 82767;
            this.Output.Multiline = true;
            this.Output.Name = "Output";
            this.Output.ReadOnly = true;
            this.Output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Output.Size = new System.Drawing.Size(448, 262);
            this.Output.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 5;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Input
            // 
            this.Input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Input.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(29)))), ((int)(((byte)(44)))));
            this.Input.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Input.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Input.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.Input.Location = new System.Drawing.Point(12, 319);
            this.Input.Multiline = true;
            this.Input.Name = "Input";
            this.Input.Size = new System.Drawing.Size(446, 64);
            this.Input.TabIndex = 1;
            this.Input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Input_KeyDown);
            // 
            // OutputPadding
            // 
            this.OutputPadding.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputPadding.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(29)))), ((int)(((byte)(44)))));
            this.OutputPadding.Location = new System.Drawing.Point(8, 43);
            this.OutputPadding.Name = "OutputPadding";
            this.OutputPadding.Size = new System.Drawing.Size(454, 268);
            this.OutputPadding.TabIndex = 9;
            // 
            // InputPadding
            // 
            this.InputPadding.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputPadding.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(29)))), ((int)(((byte)(44)))));
            this.InputPadding.Controls.Add(this.EnterButton);
            this.InputPadding.Location = new System.Drawing.Point(8, 315);
            this.InputPadding.Name = "InputPadding";
            this.InputPadding.Size = new System.Drawing.Size(452, 72);
            this.InputPadding.TabIndex = 10;
            // 
            // EnterButton
            // 
            this.EnterButton.Font = new System.Drawing.Font("Consolas", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnterButton.Location = new System.Drawing.Point(420, 23);
            this.EnterButton.Name = "EnterButton";
            this.EnterButton.Size = new System.Drawing.Size(2, 2);
            this.EnterButton.TabIndex = 0;
            this.EnterButton.Text = "button1";
            this.EnterButton.UseVisualStyleBackColor = true;
            this.EnterButton.Click += new System.EventHandler(this.TranslateButton_Click);
            // 
            // ResizeSE
            // 
            this.ResizeSE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ResizeSE.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(63)))));
            this.ResizeSE.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.ResizeSE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResizeSE.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResizeSE.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ResizeSE.Location = new System.Drawing.Point(462, 390);
            this.ResizeSE.Margin = new System.Windows.Forms.Padding(0);
            this.ResizeSE.Name = "ResizeSE";
            this.ResizeSE.Size = new System.Drawing.Size(10, 10);
            this.ResizeSE.TabIndex = 4;
            this.ResizeSE.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ResizeXY_MouseDown);
            this.ResizeSE.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ResizeXY_MouseUp);
            // 
            // ResizeS
            // 
            this.ResizeS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResizeS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(63)))));
            this.ResizeS.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.ResizeS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResizeS.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResizeS.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ResizeS.Location = new System.Drawing.Point(6, 390);
            this.ResizeS.Margin = new System.Windows.Forms.Padding(0);
            this.ResizeS.Name = "ResizeS";
            this.ResizeS.Size = new System.Drawing.Size(456, 10);
            this.ResizeS.TabIndex = 15;
            this.ResizeS.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ResizeY_MouseDown);
            this.ResizeS.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ResizeY_MouseUp);
            // 
            // MoveButton
            // 
            this.MoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MoveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(63)))));
            this.MoveButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.MoveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MoveButton.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.MoveButton.Location = new System.Drawing.Point(13, 9);
            this.MoveButton.Name = "MoveButton";
            this.MoveButton.Size = new System.Drawing.Size(283, 30);
            this.MoveButton.TabIndex = 2;
            this.MoveButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label1_MouseDown);
            this.MoveButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label1_MouseUp);
            // 
            // ResizeN
            // 
            this.ResizeN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResizeN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(63)))));
            this.ResizeN.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.ResizeN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResizeN.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResizeN.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ResizeN.Location = new System.Drawing.Point(6, 0);
            this.ResizeN.Margin = new System.Windows.Forms.Padding(0);
            this.ResizeN.Name = "ResizeN";
            this.ResizeN.Size = new System.Drawing.Size(456, 6);
            this.ResizeN.TabIndex = 17;
            this.ResizeN.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ResizeY_MouseDown);
            this.ResizeN.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ResizeY_MouseUp);
            // 
            // ResizeNE
            // 
            this.ResizeNE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ResizeNE.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            this.ResizeNE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResizeNE.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResizeNE.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ResizeNE.Location = new System.Drawing.Point(462, 0);
            this.ResizeNE.Margin = new System.Windows.Forms.Padding(0);
            this.ResizeNE.Name = "ResizeNE";
            this.ResizeNE.Size = new System.Drawing.Size(10, 6);
            this.ResizeNE.TabIndex = 16;
            this.ResizeNE.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ResizeXY_MouseDown);
            this.ResizeNE.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ResizeXY_MouseUp);
            // 
            // ResizeSW
            // 
            this.ResizeSW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ResizeSW.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(63)))));
            this.ResizeSW.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            this.ResizeSW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResizeSW.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResizeSW.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ResizeSW.Location = new System.Drawing.Point(0, 390);
            this.ResizeSW.Margin = new System.Windows.Forms.Padding(0);
            this.ResizeSW.Name = "ResizeSW";
            this.ResizeSW.Size = new System.Drawing.Size(6, 10);
            this.ResizeSW.TabIndex = 18;
            this.ResizeSW.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ResizeXY_MouseDown);
            this.ResizeSW.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ResizeXY_MouseUp);
            // 
            // ResizeNW
            // 
            this.ResizeNW.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(63)))));
            this.ResizeNW.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.ResizeNW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResizeNW.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResizeNW.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ResizeNW.Location = new System.Drawing.Point(0, 0);
            this.ResizeNW.Margin = new System.Windows.Forms.Padding(0);
            this.ResizeNW.Name = "ResizeNW";
            this.ResizeNW.Size = new System.Drawing.Size(6, 6);
            this.ResizeNW.TabIndex = 19;
            this.ResizeNW.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ResizeXY_MouseDown);
            this.ResizeNW.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ResizeXY_MouseUp);
            // 
            // ResizeW
            // 
            this.ResizeW.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ResizeW.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(63)))));
            this.ResizeW.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.ResizeW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResizeW.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResizeW.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ResizeW.Location = new System.Drawing.Point(0, 6);
            this.ResizeW.Margin = new System.Windows.Forms.Padding(0);
            this.ResizeW.Name = "ResizeW";
            this.ResizeW.Size = new System.Drawing.Size(6, 384);
            this.ResizeW.TabIndex = 20;
            this.ResizeW.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ResizeX_MouseDown);
            this.ResizeW.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ResizeX_MouseUp);
            // 
            // ResizeE
            // 
            this.ResizeE.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResizeE.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(63)))));
            this.ResizeE.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.ResizeE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResizeE.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResizeE.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ResizeE.Location = new System.Drawing.Point(462, 6);
            this.ResizeE.Margin = new System.Windows.Forms.Padding(0);
            this.ResizeE.Name = "ResizeE";
            this.ResizeE.Size = new System.Drawing.Size(10, 384);
            this.ResizeE.TabIndex = 21;
            this.ResizeE.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ResizeX_MouseDown);
            this.ResizeE.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ResizeX_MouseUp);
            // 
            // SettingsButton
            // 
            this.SettingsButton.BackgroundImage = global::TranslateTool.Properties.Resources.Settings;
            this.SettingsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SettingsButton.Location = new System.Drawing.Point(8, 2);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(40, 40);
            this.SettingsButton.TabIndex = 15;
            this.SettingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
            this.SettingsButton.MouseEnter += new System.EventHandler(this.SettingsButton_MouseEnter);
            this.SettingsButton.MouseLeave += new System.EventHandler(this.SettingsButton_MouseLeave);
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.BackgroundImage = global::TranslateTool.Properties.Resources.Close;
            this.CloseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.CloseButton.Location = new System.Drawing.Point(422, 2);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(40, 40);
            this.CloseButton.TabIndex = 14;
            this.CloseButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CloseButton_Click);
            this.CloseButton.MouseEnter += new System.EventHandler(this.CloseButton_MouseEnter);
            this.CloseButton.MouseLeave += new System.EventHandler(this.CloseButton_MouseLeave);
            // 
            // AutoShowButton
            // 
            this.AutoShowButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AutoShowButton.BackgroundImage = global::TranslateTool.Properties.Resources.AutoShowDisabled;
            this.AutoShowButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.AutoShowButton.Location = new System.Drawing.Point(381, 2);
            this.AutoShowButton.Name = "AutoShowButton";
            this.AutoShowButton.Size = new System.Drawing.Size(40, 40);
            this.AutoShowButton.TabIndex = 13;
            this.AutoShowButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AutoShowButton_Click);
            this.AutoShowButton.MouseEnter += new System.EventHandler(this.AutoShowButton_MouseEnter);
            this.AutoShowButton.MouseLeave += new System.EventHandler(this.AutoShowButton_MouseLeave);
            // 
            // ClickThroughButton
            // 
            this.ClickThroughButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ClickThroughButton.BackgroundImage = global::TranslateTool.Properties.Resources.ClickThroughDisabled;
            this.ClickThroughButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClickThroughButton.Location = new System.Drawing.Point(299, 2);
            this.ClickThroughButton.Name = "ClickThroughButton";
            this.ClickThroughButton.Size = new System.Drawing.Size(40, 40);
            this.ClickThroughButton.TabIndex = 12;
            // 
            // AutoScrollButton
            // 
            this.AutoScrollButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AutoScrollButton.BackgroundImage = global::TranslateTool.Properties.Resources.AutoScroll;
            this.AutoScrollButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.AutoScrollButton.Location = new System.Drawing.Point(340, 2);
            this.AutoScrollButton.Name = "AutoScrollButton";
            this.AutoScrollButton.Size = new System.Drawing.Size(40, 40);
            this.AutoScrollButton.TabIndex = 11;
            this.AutoScrollButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AutoScrollButton_Click);
            this.AutoScrollButton.MouseEnter += new System.EventHandler(this.AutoScrollButton_MouseEnter);
            this.AutoScrollButton.MouseLeave += new System.EventHandler(this.AutoScrollButton_MouseLeave);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(41)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(468, 396);
            this.Controls.Add(this.SettingsButton);
            this.Controls.Add(this.ResizeE);
            this.Controls.Add(this.ResizeW);
            this.Controls.Add(this.ResizeNW);
            this.Controls.Add(this.ResizeSW);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.AutoShowButton);
            this.Controls.Add(this.ClickThroughButton);
            this.Controls.Add(this.AutoScrollButton);
            this.Controls.Add(this.MoveButton);
            this.Controls.Add(this.ResizeN);
            this.Controls.Add(this.ResizeNE);
            this.Controls.Add(this.ResizeS);
            this.Controls.Add(this.Input);
            this.Controls.Add(this.Output);
            this.Controls.Add(this.OutputPadding);
            this.Controls.Add(this.ResizeSE);
            this.Controls.Add(this.InputPadding);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(120, 120);
            this.Name = "Form1";
            this.Text = "TranslateDebug";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Lime;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.InputPadding.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Output;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox Input;
        private System.Windows.Forms.Label MoveButton;
        private System.Windows.Forms.Label ResizeSE;
        private System.Windows.Forms.Panel OutputPadding;
        private System.Windows.Forms.Panel InputPadding;
        private System.Windows.Forms.Button EnterButton;
        private System.Windows.Forms.Panel AutoScrollButton;
        private System.Windows.Forms.Panel ClickThroughButton;
        private System.Windows.Forms.Panel AutoShowButton;
        private System.Windows.Forms.Panel CloseButton;
        private System.Windows.Forms.Label ResizeS;
        private System.Windows.Forms.Label ResizeN;
        private System.Windows.Forms.Label ResizeNE;
        private System.Windows.Forms.Label ResizeSW;
        private System.Windows.Forms.Label ResizeNW;
        private System.Windows.Forms.Label ResizeW;
        private System.Windows.Forms.Label ResizeE;
        private System.Windows.Forms.Panel SettingsButton;
    }
}

