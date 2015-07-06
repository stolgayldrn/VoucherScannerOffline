using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WD_MFS100DN;
using System.Threading;
using System.IO;
 

namespace RedRose_VoucherScanner
{
    public partial class ValidForm : Form
    {
        public bool validated = false;
        public bool SkipOrActive = false;
        private MainForm parentMF;
        public bool btnContinueClicked = false;
        private Vendor selectedVendor = null;
        public List<ValidationResult> resultsPublic = new List<ValidationResult>();
        public int totalValue = 0;
        public statParams SP = new statParams();
        public List<String> rawBarcodes = new List<string>();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parms = base.CreateParams;
                parms.ClassStyle |= 0x200;  // CS_NOCLOSE
                return parms;
            }
        }
        
        public ValidForm(MainForm MF, Vendor selectedVendor)
        {
            if (selectedVendor == null)
                throw new InvalidOperationException("Selected vendor cannot be null!");
            this.selectedVendor = selectedVendor;
            this.parentMF = MF;
            InitializeComponent();

            btnClear.Enabled = false;
            btnClear.Visible = false;
            btnClear.Hide();

            btnValidate.Enabled = false;
            btnValidate.Visible = false;
            btnValidate.Hide();

            btnSubmit.Enabled = false;
            btnSubmit.Visible = false;
            btnSubmit.Hide();
            lblVendorName.Text = selectedVendor.ToString();
            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            validated = false; 
            btnClear.Enabled = false;
            btnClear.Visible = false;
            btnClear.Hide();

            btnValidate.Enabled = false;
            btnValidate.Visible = false;
            btnValidate.Hide();

            btnSubmit.Enabled = false;
            btnSubmit.Visible = false;
            btnSubmit.Hide();

            totalValue = 0;
            tbTotalValue.Text = null;
            tbTotalValue.Update();
            dgwValidateScreen.Rows.Clear();

            SP = new statParams();
            statisticsItemsUpdateAndShow(lbStatistics, SP);
            rawBarcodes.Clear();
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            int totalValue = 0;
            validated = true;
            //resultsPublic = resultsFake(this.barcodes);
            try
            {
                List<ValidationResult> results = new List<ValidationResult>();
                List<string> barcodes = new List<string>();
                for (int i = 0; i < dgwValidateScreen.Rows.Count; i++)
                {
                    ValidationResult vr = (ValidationResult)dgwValidateScreen.Rows[i].Tag;
                    barcodes.Add(vr.barcode);
                }
                results = RestClient.ValidateVoucherPages(barcodes, parentMF.Username, parentMF.Password);
                
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].errorMessage == null)
                    {
                        results[i].errorMessage = "Voucher is available for submission";
                        totalValue += Convert.ToInt32(results[i].value);
                    }
                }

                BindGrid(results);
                tbTotalValue.Text = totalValue.ToString();
                this.totalValue = totalValue;
                tbTotalValue.Update();

                //SP.imgCnt = barcodes.Count();
                //statisticsItemsUpdateAndShow(lbStatistics, SP);
                btnSubmit.Enabled = true;
                btnSubmit.Visible = true;
                btnSubmit.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),
                                  "Red Rose Scan-Solutions - MFS100",
                                   MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            //Gridview here

            
            
        }

        private void btnStartReading_Click(object sender, EventArgs e)
        {
            btnStartReading.Enabled = false;
            btnStartReading.Visible = false;
            btnStartReading.Hide();

            btnClear.Enabled = false;
            btnClear.Visible = false;
            btnClear.Hide();

            btnValidate.Enabled = false;
            btnValidate.Visible = false;
            btnValidate.Hide();

            btnSubmit.Enabled = false;
            btnSubmit.Visible = false;
            btnSubmit.Hide();
           
            // clean codeline (MICR history)
            try
            {
                File.WriteAllText("codeline.txt", string.Empty);
            }
            catch
            {
                MessageBox.Show(
                                  "Cannot open file codeline.txt for writing",
                                  "Red Rose Scan-Solutions - MFS100",
                                   MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }


            //Barcode reading thread starts
            BarcodeReading barcodeRead1 = new BarcodeReading(ref parentMF ,
                    parentMF.Mfs100);

           
            parentMF.VoucherScanning(this);
            //parentMF.voucherScanFake(this);
            System.Threading.Thread.Sleep(500);
            
           
            //Real scanning code
            List<String> rawBarcodesNew = barcodeRead1.getMyBarcodeList();
            rawBarcodes.AddRange(rawBarcodesNew);

            ////Delete this fake operation for better scanner
            //List<String> rawBarcodes = parentMF.voucherScanFake();

            for (int i = 0; i < rawBarcodesNew.Count; i++)
            {
                var vr = new ValidationResult();
                vr.barcode = rawBarcodesNew[i];
                SP.imgCnt++;
                if (vr.barcode == "")
                    SP.invCnt++;
                else
                    SP.validCnt++;
            }


            List<ValidationResult> tempResults = new List<ValidationResult>();
            foreach (var rb in rawBarcodes)
            {
                var vr = new ValidationResult();
                vr.barcode = rb;
                vr.errorMessage = "Validation Required!";
                tempResults.Add(vr);  
            }            

            BindGrid(tempResults);

            btnStartReading.Enabled = true;
            btnStartReading.Visible = true;
            btnStartReading.Show();

            btnClear.Enabled = true;
            btnClear.Visible = true;
            btnClear.Show();

            btnValidate.Enabled = true;
            btnValidate.Visible = true;
            btnValidate.Show();

            List<string> MICR_list = new List<string>();
            var lines = File.ReadAllLines("codeline.txt");
            foreach (var line in lines)
            {
                MICR_list.Add(line.ToString());                
            }

            duplicateCheck();
            //SP.imgCnt = rawBarcodes.Count();
            statisticsItemsUpdateAndShow(lbStatistics,SP);
            
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            voucherStatistics vStats = new voucherStatistics();
            totalValue = 0;
            try
            {
                VoucherPack req = new VoucherPack();
                req.barcodes = new List<string>();
                for (int i = 0; i < dgwValidateScreen.Rows.Count; i++)
                {
                    ValidationResult vr = (ValidationResult)dgwValidateScreen.Rows[i].Tag;
                    if (vr.errorMessage == null ||
                     vr.errorMessage == "Voucher is available for submission")
                    {
                        req.barcodes.Add(vr.barcode);
                        totalValue +=  Convert.ToInt32(vr.value);
                        if (vr.value == "5")
                            vStats.first++;
                        if (vr.value == "10")
                            vStats.second++;
                        if (vr.value == "20")
                            vStats.third++;
                        if (vr.value == "30")
                            vStats.fourth++;
                        if (vr.value == "50")
                            vStats.fifth++;
                    }

                }
                DialogResult result = MessageBox.Show("You will submit " + vStats.first.ToString() + " units 5 SSP, "
                    + vStats.second.ToString() + " units 10 SSP, "
                    + vStats.third.ToString() + " units 20 SSP, "
                    + vStats.fourth.ToString() + " units 30 SSP, "
                    + vStats.fifth.ToString() + " units 50 SSP "
                    +  "to " + selectedVendor,
                                                       "Submission Details",
                                                       MessageBoxButtons.YesNoCancel,
                                                       MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ValidationResult vrt = (ValidationResult)dgwValidateScreen.Rows[0].Tag;
                    req.vendorId = this.selectedVendor.id;
                    RestClient.UploadVouchers(req, parentMF.Username, parentMF.Password);

                    MessageBox.Show(totalValue.ToString() + " " + vrt.unitType + " is submitted to " +
                          selectedVendor, "Submission completed",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);


                    ///Clear everything
                    totalValue = 0;
                    tbTotalValue.Text = null;
                    dgwValidateScreen.Rows.Clear();
                    SP = new statParams();
                    statisticsItemsUpdateAndShow(lbStatistics, SP);

                    btnClear.Enabled = false;
                    btnClear.Visible = false;
                    btnClear.Hide();

                    btnValidate.Enabled = false;
                    btnValidate.Visible = false;
                    btnValidate.Hide();

                    btnSubmit.Enabled = false;
                    btnSubmit.Visible = false;
                    btnSubmit.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),
                                   "Red Rose Scan-Solutions - MFS100",
                                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            
        }

        private void VF_Closing(object sender, FormClosingEventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BindGrid(List<ValidationResult> results)
        {
            dgwValidateScreen.Rows.Clear();
            dgwValidateScreen.AutoGenerateColumns = false;

            for (int i = 0; i < results.Count; i++)
            {
                ValidationResult result = results[i];

                string valueText  = "";
                string status = "";
                if (result.errorMessage == null ||
                    result.errorMessage == "Voucher is available for submission")
                {
                    valueText = result.value + " " + result.unitType;
                    status = result.errorMessage;
                }
                else
                    status = result.errorMessage;

               int index = dgwValidateScreen.Rows.Add(i+1,result.barcode, valueText, status);
               dgwValidateScreen.Rows[index].Tag = result;
                
                /*row.Cells[0].Value = false;
                row.Cells[1].Value = result.barcode;
                row.Cells[2].Value = valueText;
                row.Cells[3].Value = status;*/
            }

        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            
            for( int i = 0; i< dgwValidateScreen.Rows.Count; i++)
            {
                dgwValidateScreen.Rows[i].Selected = true;
            }
        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            
            for (int i = 0; i < dgwValidateScreen.Rows.Count; i++)
            {
                dgwValidateScreen.Rows[i].Selected = false;
            }

        }

        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {            
                for (int i = 0; i < dgwValidateScreen.Rows.Count; i++)
                {
                    if (dgwValidateScreen.Rows[i].Selected)
                    {
                        bool next = false;
                        if (i + 1 < dgwValidateScreen.RowCount)
                        {
                            if (!dgwValidateScreen.Rows[i + 1].Selected)
                                next = true;
                        }

                        dgwValidateScreen.Rows.RemoveAt(i);
                        rawBarcodes.RemoveAt(i);
                        if (next)
                            dgwValidateScreen.Rows[i].Selected = false;
                        i--;
                        
                    }
                }
                dgwValidateScreen.Update();
        }

        private List<ValidationResult> resultsFake(List<String> barcodes)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            
            for (int i = 0; i < barcodes.Count; i++)
            {
                results.Add(new ValidationResult()
                {
                    barcode = barcodes[i],
                    errorMessage = "valid",
                    unitType = "USD",
                    value = "50"

                });
            }
            return results;
 
        }

        private void duplicateCheck()
        {
            List<string> barcodes = new List<string>();

            for (int i = 0; i < dgwValidateScreen.Rows.Count; i++)
                {
                    ValidationResult vr = (ValidationResult)dgwValidateScreen.Rows[i].Tag;
                    barcodes.Add(vr.barcode);
                    //SP.validCnt++;
                }

            for (int i = 1; i < barcodes.Count; i++)
            {
                for (int ii = 0; ii < i; ii++)
                {
                    if (barcodes[ii] == barcodes[i])
                    {
                        dgwValidateScreen.Rows.RemoveAt(i);
                        barcodes.RemoveAt(i);
                        i--;                        
                        SP.reusedCnt++;
                        SP.validCnt--;                       
                        
                        break;
 
                    }
                }
            }

            statisticsItemsUpdateAndShow(lbStatistics, SP);

        }

        private void statisticsItemsUpdateAndShow(ListBox lbStatistics, statParams mySP)
        {
            lbStatistics.Items.Clear();
            lbStatistics.Items.Add(mySP.imgCnt.ToString());
            lbStatistics.Items.Add(mySP.validCnt.ToString());
            lbStatistics.Items.Add(mySP.invCnt.ToString());
            lbStatistics.Items.Add(mySP.reusedCnt.ToString());
            lbStatistics.Update();
        }

      
    }
}
