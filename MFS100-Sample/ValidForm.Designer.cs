namespace RedRose_VoucherScanner
{
    partial class ValidForm
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
            this.btnClear = new System.Windows.Forms.Button();
            this.lblStat = new System.Windows.Forms.Label();
            this.lbStatistics = new System.Windows.Forms.ListBox();
            this.voucherBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnStartReading = new System.Windows.Forms.Button();
            this.lblVendorName = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.dgwValidateScreen = new System.Windows.Forms.DataGridView();
            this.order = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.barcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.micrCtrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnDeleteSelected = new System.Windows.Forms.Button();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.btnRead = new System.Windows.Forms.Button();
            this.cbScanImages = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.voucherBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgwValidateScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(509, 166);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(147, 42);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "&Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblStat
            // 
            this.lblStat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStat.Location = new System.Drawing.Point(460, 211);
            this.lblStat.Name = "lblStat";
            this.lblStat.Size = new System.Drawing.Size(99, 68);
            this.lblStat.TabIndex = 10;
            this.lblStat.Text = "Proccessed:\r\n           Valid:\r\n        Invalid:\r\n       Reused:";
            this.lblStat.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbStatistics
            // 
            this.lbStatistics.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatistics.FormattingEnabled = true;
            this.lbStatistics.HorizontalScrollbar = true;
            this.lbStatistics.ItemHeight = 16;
            this.lbStatistics.Location = new System.Drawing.Point(566, 211);
            this.lbStatistics.Name = "lbStatistics";
            this.lbStatistics.Size = new System.Drawing.Size(90, 68);
            this.lbStatistics.TabIndex = 9;
            // 
            // btnStartReading
            // 
            this.btnStartReading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartReading.Location = new System.Drawing.Point(509, 118);
            this.btnStartReading.Name = "btnStartReading";
            this.btnStartReading.Size = new System.Drawing.Size(147, 42);
            this.btnStartReading.TabIndex = 13;
            this.btnStartReading.Text = "&Start Scanning";
            this.btnStartReading.UseVisualStyleBackColor = true;
            this.btnStartReading.Click += new System.EventHandler(this.btnStartReading_Click);
            // 
            // lblVendorName
            // 
            this.lblVendorName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVendorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVendorName.Location = new System.Drawing.Point(415, 30);
            this.lblVendorName.Name = "lblVendorName";
            this.lblVendorName.Size = new System.Drawing.Size(272, 19);
            this.lblVendorName.TabIndex = 15;
            this.lblVendorName.Text = "vendor name";
            this.lblVendorName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Location = new System.Drawing.Point(509, 433);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(147, 42);
            this.btnSubmit.TabIndex = 16;
            this.btnSubmit.Text = "&Save";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // dgwValidateScreen
            // 
            this.dgwValidateScreen.AllowUserToAddRows = false;
            this.dgwValidateScreen.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwValidateScreen.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.order,
            this.barcode,
            this.micrCtrl});
            this.dgwValidateScreen.Location = new System.Drawing.Point(32, 55);
            this.dgwValidateScreen.Name = "dgwValidateScreen";
            this.dgwValidateScreen.ReadOnly = true;
            this.dgwValidateScreen.RowTemplate.ReadOnly = true;
            this.dgwValidateScreen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgwValidateScreen.Size = new System.Drawing.Size(386, 680);
            this.dgwValidateScreen.TabIndex = 17;
            // 
            // order
            // 
            this.order.HeaderText = "Order";
            this.order.Name = "order";
            this.order.ReadOnly = true;
            this.order.Width = 40;
            // 
            // barcode
            // 
            this.barcode.FillWeight = 200F;
            this.barcode.HeaderText = "Barcode";
            this.barcode.Name = "barcode";
            this.barcode.ReadOnly = true;
            this.barcode.Width = 200;
            // 
            // micrCtrl
            // 
            this.micrCtrl.HeaderText = "MICR Control";
            this.micrCtrl.Name = "micrCtrl";
            this.micrCtrl.ReadOnly = true;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(509, 541);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(147, 42);
            this.btnClose.TabIndex = 18;
            this.btnClose.Text = "&Exit";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(32, 26);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 19;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnDeleteSelected
            // 
            this.btnDeleteSelected.Location = new System.Drawing.Point(314, 26);
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.btnDeleteSelected.Size = new System.Drawing.Size(104, 23);
            this.btnDeleteSelected.TabIndex = 20;
            this.btnDeleteSelected.Text = "Delete Selected";
            this.btnDeleteSelected.UseVisualStyleBackColor = true;
            this.btnDeleteSelected.Click += new System.EventHandler(this.btnDeleteSelected_Click);
            // 
            // btnDeselectAll
            // 
            this.btnDeselectAll.Location = new System.Drawing.Point(113, 26);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(75, 23);
            this.btnDeselectAll.TabIndex = 21;
            this.btnDeselectAll.Text = "Deselect All";
            this.btnDeselectAll.UseVisualStyleBackColor = true;
            this.btnDeselectAll.Click += new System.EventHandler(this.btnDeselectAll_Click);
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(484, 322);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(75, 23);
            this.btnRead.TabIndex = 23;
            this.btnRead.Text = "Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Visible = false;
            this.btnRead.Click += new System.EventHandler(this.button1_Click);
            // 
            // cbScanImages
            // 
            this.cbScanImages.AutoSize = true;
            this.cbScanImages.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.cbScanImages.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbScanImages.Location = new System.Drawing.Point(436, 52);
            this.cbScanImages.Name = "cbScanImages";
            this.cbScanImages.Size = new System.Drawing.Size(251, 28);
            this.cbScanImages.TabIndex = 24;
            this.cbScanImages.Text = "Save Images with Barcode";
            this.cbScanImages.UseVisualStyleBackColor = true;
            // 
            // ValidForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(699, 769);
            this.Controls.Add(this.cbScanImages);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.btnDeselectAll);
            this.Controls.Add(this.btnDeleteSelected);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dgwValidateScreen);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.lblVendorName);
            this.Controls.Add(this.btnStartReading);
            this.Controls.Add(this.lblStat);
            this.Controls.Add(this.lbStatistics);
            this.Controls.Add(this.btnClear);
            this.Name = "ValidForm";
            this.Text = "Voucher Reading Screen";
            ((System.ComponentModel.ISupportInitialize)(this.voucherBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgwValidateScreen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblStat;
        public System.Windows.Forms.ListBox lbStatistics;
        public System.Windows.Forms.BindingSource voucherBindingSource;
        public System.Windows.Forms.Button btnStartReading;
        private System.Windows.Forms.Label lblVendorName;
        public System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.DataGridView dgwValidateScreen;
        public System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnDeleteSelected;
        private System.Windows.Forms.Button btnDeselectAll;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.DataGridViewTextBoxColumn order;
        private System.Windows.Forms.DataGridViewTextBoxColumn barcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn micrCtrl;
        private System.Windows.Forms.CheckBox cbScanImages;
    }
}