namespace RedRose_VoucherScanner
{
    partial class VendorSelectForm
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
            this.btnNext = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbVendorList = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnNext
            // 
            this.btnNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.Location = new System.Drawing.Point(112, 113);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(156, 50);
            this.btnNext.TabIndex = 0;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(29, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(335, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please Select a Vendor from Vendor List";
            // 
            // cbVendorList
            // 
            this.cbVendorList.FormattingEnabled = true;
            this.cbVendorList.Items.AddRange(new object[] {
            "code 128",
            "QR code"});
            this.cbVendorList.Location = new System.Drawing.Point(33, 69);
            this.cbVendorList.Name = "cbVendorList";
            this.cbVendorList.Size = new System.Drawing.Size(331, 21);
            this.cbVendorList.TabIndex = 22;
            this.cbVendorList.SelectedIndexChanged += new System.EventHandler(this.cbVendorList_SelectedIndexChanged);
            // 
            // VendorSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Maroon;
            this.ClientSize = new System.Drawing.Size(394, 178);
            this.Controls.Add(this.cbVendorList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnNext);
            this.Name = "VendorSelectForm";
            this.Text = "Vendor Selection Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbVendorList;
    }
}