namespace RedRose_VoucherScanner
{
    partial class Setup2Form
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
            this.gbFeeding = new System.Windows.Forms.GroupBox();
            this.tbDoublefeedThreshold = new System.Windows.Forms.TextBox();
            this.lblDoublefeedThreshold = new System.Windows.Forms.Label();
            this.lblFeedTimeout2 = new System.Windows.Forms.Label();
            this.tbFeedTimeout = new System.Windows.Forms.TextBox();
            this.lblFeedTimeout1 = new System.Windows.Forms.Label();
            this.gbScanning = new System.Windows.Forms.GroupBox();
            this.btnImageDirectory = new System.Windows.Forms.Button();
            this.tbImageDirectory = new System.Windows.Forms.TextBox();
            this.lblImageDirectory = new System.Windows.Forms.Label();
            this.lblMaxImageHeight2 = new System.Windows.Forms.Label();
            this.tbMaxImageHeight = new System.Windows.Forms.TextBox();
            this.lblMaxImageHeight1 = new System.Windows.Forms.Label();
            this.lblMaxImageWidth2 = new System.Windows.Forms.Label();
            this.tbMaxImageWidth = new System.Windows.Forms.TextBox();
            this.lblMaxImageWidth1 = new System.Windows.Forms.Label();
            this.gbVoucherType = new System.Windows.Forms.GroupBox();
            this.cbReadFont = new System.Windows.Forms.ComboBox();
            this.lblReadFont = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbFeeding.SuspendLayout();
            this.gbScanning.SuspendLayout();
            this.gbVoucherType.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbFeeding
            // 
            this.gbFeeding.Controls.Add(this.tbDoublefeedThreshold);
            this.gbFeeding.Controls.Add(this.lblDoublefeedThreshold);
            this.gbFeeding.Controls.Add(this.lblFeedTimeout2);
            this.gbFeeding.Controls.Add(this.tbFeedTimeout);
            this.gbFeeding.Controls.Add(this.lblFeedTimeout1);
            this.gbFeeding.Location = new System.Drawing.Point(298, 12);
            this.gbFeeding.Name = "gbFeeding";
            this.gbFeeding.Size = new System.Drawing.Size(208, 74);
            this.gbFeeding.TabIndex = 1;
            this.gbFeeding.TabStop = false;
            this.gbFeeding.Text = "Feeding";
            // 
            // tbDoublefeedThreshold
            // 
            this.tbDoublefeedThreshold.Location = new System.Drawing.Point(126, 17);
            this.tbDoublefeedThreshold.Name = "tbDoublefeedThreshold";
            this.tbDoublefeedThreshold.Size = new System.Drawing.Size(50, 20);
            this.tbDoublefeedThreshold.TabIndex = 8;
            this.tbDoublefeedThreshold.TextChanged += new System.EventHandler(this.tbDoublefeedThreshold_TextChanged);
            // 
            // lblDoublefeedThreshold
            // 
            this.lblDoublefeedThreshold.Location = new System.Drawing.Point(6, 16);
            this.lblDoublefeedThreshold.Name = "lblDoublefeedThreshold";
            this.lblDoublefeedThreshold.Size = new System.Drawing.Size(115, 20);
            this.lblDoublefeedThreshold.TabIndex = 7;
            this.lblDoublefeedThreshold.Text = "Doublefeed threshold:";
            this.lblDoublefeedThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblFeedTimeout2
            // 
            this.lblFeedTimeout2.Location = new System.Drawing.Point(171, 40);
            this.lblFeedTimeout2.Name = "lblFeedTimeout2";
            this.lblFeedTimeout2.Size = new System.Drawing.Size(30, 20);
            this.lblFeedTimeout2.TabIndex = 6;
            this.lblFeedTimeout2.Text = "ms";
            this.lblFeedTimeout2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbFeedTimeout
            // 
            this.tbFeedTimeout.Location = new System.Drawing.Point(126, 40);
            this.tbFeedTimeout.Name = "tbFeedTimeout";
            this.tbFeedTimeout.Size = new System.Drawing.Size(40, 20);
            this.tbFeedTimeout.TabIndex = 5;
            this.tbFeedTimeout.TextChanged += new System.EventHandler(this.tbFeedTimeout_TextChanged);
            // 
            // lblFeedTimeout1
            // 
            this.lblFeedTimeout1.Location = new System.Drawing.Point(66, 40);
            this.lblFeedTimeout1.Name = "lblFeedTimeout1";
            this.lblFeedTimeout1.Size = new System.Drawing.Size(55, 20);
            this.lblFeedTimeout1.TabIndex = 4;
            this.lblFeedTimeout1.Text = "Timeout:";
            this.lblFeedTimeout1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gbScanning
            // 
            this.gbScanning.Controls.Add(this.btnImageDirectory);
            this.gbScanning.Controls.Add(this.tbImageDirectory);
            this.gbScanning.Controls.Add(this.lblImageDirectory);
            this.gbScanning.Controls.Add(this.lblMaxImageHeight2);
            this.gbScanning.Controls.Add(this.tbMaxImageHeight);
            this.gbScanning.Controls.Add(this.lblMaxImageHeight1);
            this.gbScanning.Controls.Add(this.lblMaxImageWidth2);
            this.gbScanning.Controls.Add(this.tbMaxImageWidth);
            this.gbScanning.Controls.Add(this.lblMaxImageWidth1);
            this.gbScanning.Location = new System.Drawing.Point(14, 101);
            this.gbScanning.Name = "gbScanning";
            this.gbScanning.Size = new System.Drawing.Size(539, 104);
            this.gbScanning.TabIndex = 4;
            this.gbScanning.TabStop = false;
            this.gbScanning.Text = "Scanning";
            // 
            // btnImageDirectory
            // 
            this.btnImageDirectory.Location = new System.Drawing.Point(425, 51);
            this.btnImageDirectory.Name = "btnImageDirectory";
            this.btnImageDirectory.Size = new System.Drawing.Size(90, 30);
            this.btnImageDirectory.TabIndex = 15;
            this.btnImageDirectory.Text = "Change";
            this.btnImageDirectory.UseVisualStyleBackColor = true;
            this.btnImageDirectory.Click += new System.EventHandler(this.btnImageDirectory_Click);
            // 
            // tbImageDirectory
            // 
            this.tbImageDirectory.BackColor = System.Drawing.SystemColors.Window;
            this.tbImageDirectory.Location = new System.Drawing.Point(105, 55);
            this.tbImageDirectory.Name = "tbImageDirectory";
            this.tbImageDirectory.ReadOnly = true;
            this.tbImageDirectory.Size = new System.Drawing.Size(314, 20);
            this.tbImageDirectory.TabIndex = 14;
            // 
            // lblImageDirectory
            // 
            this.lblImageDirectory.Location = new System.Drawing.Point(10, 56);
            this.lblImageDirectory.Name = "lblImageDirectory";
            this.lblImageDirectory.Size = new System.Drawing.Size(90, 20);
            this.lblImageDirectory.TabIndex = 13;
            this.lblImageDirectory.Text = "Image directory:";
            this.lblImageDirectory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMaxImageHeight2
            // 
            this.lblMaxImageHeight2.Location = new System.Drawing.Point(493, 24);
            this.lblMaxImageHeight2.Name = "lblMaxImageHeight2";
            this.lblMaxImageHeight2.Size = new System.Drawing.Size(40, 20);
            this.lblMaxImageHeight2.TabIndex = 5;
            this.lblMaxImageHeight2.Text = "pixel";
            this.lblMaxImageHeight2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbMaxImageHeight
            // 
            this.tbMaxImageHeight.Location = new System.Drawing.Point(387, 26);
            this.tbMaxImageHeight.Name = "tbMaxImageHeight";
            this.tbMaxImageHeight.Size = new System.Drawing.Size(100, 20);
            this.tbMaxImageHeight.TabIndex = 4;
            this.tbMaxImageHeight.TextChanged += new System.EventHandler(this.tbMaxImageHeight_TextChanged);
            // 
            // lblMaxImageHeight1
            // 
            this.lblMaxImageHeight1.Location = new System.Drawing.Point(258, 24);
            this.lblMaxImageHeight1.Name = "lblMaxImageHeight1";
            this.lblMaxImageHeight1.Size = new System.Drawing.Size(125, 20);
            this.lblMaxImageHeight1.TabIndex = 3;
            this.lblMaxImageHeight1.Text = "Maximum image height:";
            this.lblMaxImageHeight1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMaxImageWidth2
            // 
            this.lblMaxImageWidth2.Location = new System.Drawing.Point(234, 25);
            this.lblMaxImageWidth2.Name = "lblMaxImageWidth2";
            this.lblMaxImageWidth2.Size = new System.Drawing.Size(30, 20);
            this.lblMaxImageWidth2.TabIndex = 2;
            this.lblMaxImageWidth2.Text = "mm";
            this.lblMaxImageWidth2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbMaxImageWidth
            // 
            this.tbMaxImageWidth.Location = new System.Drawing.Point(129, 25);
            this.tbMaxImageWidth.Name = "tbMaxImageWidth";
            this.tbMaxImageWidth.Size = new System.Drawing.Size(100, 20);
            this.tbMaxImageWidth.TabIndex = 1;
            this.tbMaxImageWidth.TextChanged += new System.EventHandler(this.tbMaxImageWidth_TextChanged);
            // 
            // lblMaxImageWidth1
            // 
            this.lblMaxImageWidth1.Location = new System.Drawing.Point(-1, 25);
            this.lblMaxImageWidth1.Name = "lblMaxImageWidth1";
            this.lblMaxImageWidth1.Size = new System.Drawing.Size(125, 20);
            this.lblMaxImageWidth1.TabIndex = 0;
            this.lblMaxImageWidth1.Text = "Maximum image width:";
            this.lblMaxImageWidth1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gbVoucherType
            // 
            this.gbVoucherType.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.gbVoucherType.Controls.Add(this.cbReadFont);
            this.gbVoucherType.Controls.Add(this.lblReadFont);
            this.gbVoucherType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbVoucherType.Location = new System.Drawing.Point(14, 12);
            this.gbVoucherType.Name = "gbVoucherType";
            this.gbVoucherType.Size = new System.Drawing.Size(264, 74);
            this.gbVoucherType.TabIndex = 24;
            this.gbVoucherType.TabStop = false;
            this.gbVoucherType.Text = "Voucher Type";
            // 
            // cbReadFont
            // 
            this.cbReadFont.FormattingEnabled = true;
            this.cbReadFont.Items.AddRange(new object[] {
            "None",
            "E13B magnetic",
            "E13B optical",
            "E13B magnetic + optical",
            "OCRB"});
            this.cbReadFont.Location = new System.Drawing.Point(95, 43);
            this.cbReadFont.Name = "cbReadFont";
            this.cbReadFont.Size = new System.Drawing.Size(152, 24);
            this.cbReadFont.TabIndex = 23;
            this.cbReadFont.SelectedIndexChanged += new System.EventHandler(this.cbReadFont_SelectedIndexChanged);
            // 
            // lblReadFont
            // 
            this.lblReadFont.Location = new System.Drawing.Point(30, 46);
            this.lblReadFont.Name = "lblReadFont";
            this.lblReadFont.Size = new System.Drawing.Size(59, 21);
            this.lblReadFont.TabIndex = 22;
            this.lblReadFont.Text = "Font:";
            this.lblReadFont.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(298, 224);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(250, 79);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(9, 224);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(250, 79);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // Setup2Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(564, 323);
            this.Controls.Add(this.gbVoucherType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbScanning);
            this.Controls.Add(this.gbFeeding);
            this.Name = "Setup2Form";
            this.Text = "Options";
            this.gbFeeding.ResumeLayout(false);
            this.gbFeeding.PerformLayout();
            this.gbScanning.ResumeLayout(false);
            this.gbScanning.PerformLayout();
            this.gbVoucherType.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbFeeding;
        private System.Windows.Forms.TextBox tbDoublefeedThreshold;
        private System.Windows.Forms.Label lblDoublefeedThreshold;
        private System.Windows.Forms.Label lblFeedTimeout2;
        private System.Windows.Forms.TextBox tbFeedTimeout;
        private System.Windows.Forms.Label lblFeedTimeout1;
        private System.Windows.Forms.GroupBox gbScanning;
        private System.Windows.Forms.Button btnImageDirectory;
        private System.Windows.Forms.TextBox tbImageDirectory;
        private System.Windows.Forms.Label lblImageDirectory;
        private System.Windows.Forms.Label lblMaxImageHeight2;
        private System.Windows.Forms.TextBox tbMaxImageHeight;
        private System.Windows.Forms.Label lblMaxImageHeight1;
        private System.Windows.Forms.Label lblMaxImageWidth2;
        private System.Windows.Forms.TextBox tbMaxImageWidth;
        private System.Windows.Forms.Label lblMaxImageWidth1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox gbVoucherType;
        private System.Windows.Forms.ComboBox cbReadFont;
        private System.Windows.Forms.Label lblReadFont;
    }
}