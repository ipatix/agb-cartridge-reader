namespace AGB_Cartridge_Reader
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.drComboBox = new System.Windows.Forms.ComboBox();
            this.drButtonGo = new System.Windows.Forms.Button();
            this.drButtonDot = new System.Windows.Forms.Button();
            this.drTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dsComboBox = new System.Windows.Forms.ComboBox();
            this.dsButtonGo = new System.Windows.Forms.Button();
            this.dsTextBox = new System.Windows.Forms.TextBox();
            this.dsButtonDot = new System.Windows.Forms.Button();
            this.ofdSave = new System.Windows.Forms.OpenFileDialog();
            this.sfdSave = new System.Windows.Forms.SaveFileDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.usComboBox = new System.Windows.Forms.ComboBox();
            this.usButtonGo = new System.Windows.Forms.Button();
            this.usButtonDot = new System.Windows.Forms.Button();
            this.usTextBox = new System.Windows.Forms.TextBox();
            this.readerPort = new System.IO.Ports.SerialPort(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.totalBytesLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.bytesLabel = new System.Windows.Forms.Label();
            this.transceiver = new System.ComponentModel.BackgroundWorker();
            this.sfdROM = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.drComboBox);
            this.groupBox1.Controls.Add(this.drButtonGo);
            this.groupBox1.Controls.Add(this.drButtonDot);
            this.groupBox1.Controls.Add(this.drTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 77);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Download ROM";
            // 
            // drComboBox
            // 
            this.drComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drComboBox.FormattingEnabled = true;
            this.drComboBox.Items.AddRange(new object[] {
            "Auto",
            "4 MiB",
            "8 MiB",
            "16 MiB",
            "32 MiB"});
            this.drComboBox.Location = new System.Drawing.Point(103, 46);
            this.drComboBox.Name = "drComboBox";
            this.drComboBox.Size = new System.Drawing.Size(91, 21);
            this.drComboBox.TabIndex = 4;
            // 
            // drButtonGo
            // 
            this.drButtonGo.Location = new System.Drawing.Point(6, 45);
            this.drButtonGo.Name = "drButtonGo";
            this.drButtonGo.Size = new System.Drawing.Size(91, 23);
            this.drButtonGo.TabIndex = 2;
            this.drButtonGo.Text = "Download ROM";
            this.drButtonGo.UseVisualStyleBackColor = true;
            this.drButtonGo.Click += new System.EventHandler(this.drButtonGo_Click);
            // 
            // drButtonDot
            // 
            this.drButtonDot.Location = new System.Drawing.Point(167, 18);
            this.drButtonDot.Name = "drButtonDot";
            this.drButtonDot.Size = new System.Drawing.Size(27, 22);
            this.drButtonDot.TabIndex = 1;
            this.drButtonDot.Text = "...";
            this.drButtonDot.UseVisualStyleBackColor = true;
            this.drButtonDot.Click += new System.EventHandler(this.drButtonDot_Click);
            // 
            // drTextBox
            // 
            this.drTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.drTextBox.Location = new System.Drawing.Point(6, 19);
            this.drTextBox.Name = "drTextBox";
            this.drTextBox.ReadOnly = true;
            this.drTextBox.Size = new System.Drawing.Size(155, 20);
            this.drTextBox.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dsComboBox);
            this.groupBox2.Controls.Add(this.dsButtonGo);
            this.groupBox2.Controls.Add(this.dsTextBox);
            this.groupBox2.Controls.Add(this.dsButtonDot);
            this.groupBox2.Location = new System.Drawing.Point(218, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 77);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Download Save";
            // 
            // dsComboBox
            // 
            this.dsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dsComboBox.FormattingEnabled = true;
            this.dsComboBox.Items.AddRange(new object[] {
            "Auto",
            "EEPROM 512 B",
            "EEPROM 8 KiB",
            "SRAM 32 KiB",
            "FLASH 64 KiB",
            "FLASH 128 KiB"});
            this.dsComboBox.Location = new System.Drawing.Point(103, 46);
            this.dsComboBox.Name = "dsComboBox";
            this.dsComboBox.Size = new System.Drawing.Size(91, 21);
            this.dsComboBox.TabIndex = 6;
            // 
            // dsButtonGo
            // 
            this.dsButtonGo.Location = new System.Drawing.Point(6, 45);
            this.dsButtonGo.Name = "dsButtonGo";
            this.dsButtonGo.Size = new System.Drawing.Size(91, 23);
            this.dsButtonGo.TabIndex = 5;
            this.dsButtonGo.Text = "Download Save";
            this.dsButtonGo.UseVisualStyleBackColor = true;
            this.dsButtonGo.Click += new System.EventHandler(this.dsButtonGo_Click);
            // 
            // dsTextBox
            // 
            this.dsTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.dsTextBox.Location = new System.Drawing.Point(6, 19);
            this.dsTextBox.Name = "dsTextBox";
            this.dsTextBox.ReadOnly = true;
            this.dsTextBox.Size = new System.Drawing.Size(155, 20);
            this.dsTextBox.TabIndex = 4;
            // 
            // dsButtonDot
            // 
            this.dsButtonDot.Location = new System.Drawing.Point(167, 18);
            this.dsButtonDot.Name = "dsButtonDot";
            this.dsButtonDot.Size = new System.Drawing.Size(27, 22);
            this.dsButtonDot.TabIndex = 3;
            this.dsButtonDot.Text = "...";
            this.dsButtonDot.UseVisualStyleBackColor = true;
            this.dsButtonDot.Click += new System.EventHandler(this.dsButtonDot_Click);
            // 
            // ofdSave
            // 
            this.ofdSave.FileName = "openFileDialog2";
            this.ofdSave.Filter = "GBA Save Files|*.sav|All Files|*.*";
            // 
            // sfdSave
            // 
            this.sfdSave.Filter = "GBA Save Files|*.sav|All Files|*.*";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.usComboBox);
            this.groupBox3.Controls.Add(this.usButtonGo);
            this.groupBox3.Controls.Add(this.usButtonDot);
            this.groupBox3.Controls.Add(this.usTextBox);
            this.groupBox3.Location = new System.Drawing.Point(424, 12);
            this.groupBox3.MaximumSize = new System.Drawing.Size(200, 77);
            this.groupBox3.MinimumSize = new System.Drawing.Size(200, 77);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 77);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Upload Save";
            // 
            // usComboBox
            // 
            this.usComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.usComboBox.FormattingEnabled = true;
            this.usComboBox.Items.AddRange(new object[] {
            "Auto",
            "EEPROM 512 B",
            "EEPROM 8 KiB",
            "SRAM 32 KiB",
            "FLASH 64 KiB",
            "FLASH 128 KiB"});
            this.usComboBox.Location = new System.Drawing.Point(103, 46);
            this.usComboBox.Name = "usComboBox";
            this.usComboBox.Size = new System.Drawing.Size(91, 21);
            this.usComboBox.TabIndex = 3;
            // 
            // usButtonGo
            // 
            this.usButtonGo.Location = new System.Drawing.Point(6, 45);
            this.usButtonGo.Name = "usButtonGo";
            this.usButtonGo.Size = new System.Drawing.Size(91, 23);
            this.usButtonGo.TabIndex = 3;
            this.usButtonGo.Text = "Upload Save";
            this.usButtonGo.UseVisualStyleBackColor = true;
            this.usButtonGo.Click += new System.EventHandler(this.usButtonGo_Click);
            // 
            // usButtonDot
            // 
            this.usButtonDot.Location = new System.Drawing.Point(167, 18);
            this.usButtonDot.Name = "usButtonDot";
            this.usButtonDot.Size = new System.Drawing.Size(27, 22);
            this.usButtonDot.TabIndex = 4;
            this.usButtonDot.Text = "...";
            this.usButtonDot.UseVisualStyleBackColor = true;
            this.usButtonDot.Click += new System.EventHandler(this.usButtonDot_Click);
            // 
            // usTextBox
            // 
            this.usTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.usTextBox.Location = new System.Drawing.Point(6, 19);
            this.usTextBox.Name = "usTextBox";
            this.usTextBox.ReadOnly = true;
            this.usTextBox.Size = new System.Drawing.Size(155, 20);
            this.usTextBox.TabIndex = 3;
            // 
            // readerPort
            // 
            this.readerPort.BaudRate = 1000000;
            this.readerPort.PortName = "COM4";
            this.readerPort.ReadTimeout = 1000;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(103, 11);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(406, 39);
            this.progressBar1.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.totalBytesLabel);
            this.groupBox4.Controls.Add(this.cancelButton);
            this.groupBox4.Controls.Add(this.bytesLabel);
            this.groupBox4.Controls.Add(this.progressBar1);
            this.groupBox4.Location = new System.Drawing.Point(12, 95);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(612, 56);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Progress";
            // 
            // totalBytesLabel
            // 
            this.totalBytesLabel.AutoSize = true;
            this.totalBytesLabel.Location = new System.Drawing.Point(6, 19);
            this.totalBytesLabel.Name = "totalBytesLabel";
            this.totalBytesLabel.Size = new System.Drawing.Size(63, 13);
            this.totalBytesLabel.TabIndex = 7;
            this.totalBytesLabel.Text = "Bytes: none";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(515, 11);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(91, 39);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // bytesLabel
            // 
            this.bytesLabel.AutoSize = true;
            this.bytesLabel.Location = new System.Drawing.Point(6, 37);
            this.bytesLabel.Name = "bytesLabel";
            this.bytesLabel.Size = new System.Drawing.Size(73, 13);
            this.bytesLabel.TabIndex = 5;
            this.bytesLabel.Text = "Bytes/s: none";
            // 
            // transceiver
            // 
            this.transceiver.WorkerReportsProgress = true;
            this.transceiver.WorkerSupportsCancellation = true;
            this.transceiver.DoWork += new System.ComponentModel.DoWorkEventHandler(this.transceiver_DoWork);
            this.transceiver.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.transceiver_ProgressChanged);
            this.transceiver.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.transceiver_RunWorkerCompleted);
            // 
            // sfdROM
            // 
            this.sfdROM.Filter = "GBA-ROMs|*.gba|All Files|*.*";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 162);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(652, 201);
            this.MinimumSize = new System.Drawing.Size(652, 201);
            this.Name = "MainForm";
            this.Text = "AGB Cartridge Reader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button drButtonGo;
        private System.Windows.Forms.Button drButtonDot;
        private System.Windows.Forms.TextBox drTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox dsComboBox;
        private System.Windows.Forms.Button dsButtonGo;
        private System.Windows.Forms.TextBox dsTextBox;
        private System.Windows.Forms.Button dsButtonDot;
        private System.Windows.Forms.OpenFileDialog ofdSave;
        private System.Windows.Forms.SaveFileDialog sfdSave;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox usTextBox;
        private System.Windows.Forms.Button usButtonDot;
        private System.Windows.Forms.ComboBox usComboBox;
        private System.Windows.Forms.Button usButtonGo;
        private System.IO.Ports.SerialPort readerPort;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox drComboBox;
        private System.Windows.Forms.Label bytesLabel;
        private System.ComponentModel.BackgroundWorker transceiver;
        private System.Windows.Forms.SaveFileDialog sfdROM;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label totalBytesLabel;
    }
}

