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
using System.Web.Configuration;
using RedRose_VoucherScanner.Properties;
using ListBox = System.Windows.Forms.ListBox;

using NPOI.HSSF.Model; // InternalWorkbook
using NPOI.HSSF.UserModel; // HSSFWorkbook, HSSFSheet



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
        public List<string> rawBarcodes = new List<string>();
        public List<string> rawImages = new List<string>();
        public List<string> MICR_status= new List<string>();
        private List<ValidationResult> tempResults;
        public string xlsPath = "Test.xls";
        HSSFWorkbook wb;
        HSSFSheet sh;

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

            btnDuplicate.Enabled = false;
            btnDuplicate.Visible = false;
            btnDuplicate.Hide();
            
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

            btnDuplicate.Enabled = false;
            btnDuplicate.Visible = false;
            btnDuplicate.Hide();

            btnStartReading.Enabled = true;
            btnStartReading.Visible = true;
            btnStartReading.Show();

            btnSubmit.Enabled = false;
            btnSubmit.Visible = false;
            btnSubmit.Hide();

            totalValue = 0;
            dgwValidateScreen.Rows.Clear();
            MICR_status.Clear();
            SP = new statParams();
            statisticsItemsUpdateAndShow(lbStatistics, SP);
            rawBarcodes.Clear();
            rawImages.Clear();
            tempResults.Clear();           
           
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            //int totalValue = 0;
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
                
                //for (int i = 0; i < results.Count; i++)
                //{
                //    if (results[i].errorMessage == null)
                //    {
                //        results[i].errorMessage = "Voucher is available for submission";
                //        totalValue += Convert.ToInt32(results[i].value);
                //    }
                //}

                BindGrid(results);

                //SP.imgCnt = barcodes.Count();
                //statisticsItemsUpdateAndShow(lbStatistics, SP);
                btnSubmit.Enabled = true;
                btnSubmit.Visible = true;
                btnSubmit.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),
                                  Settings.Default.messageBoxTitle,
                                   MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnStartReading_Click(object sender, EventArgs e)
        {
            btnStartReading.Enabled = false;
            btnStartReading.Visible = false;
            btnStartReading.Hide();

            btnClear.Enabled = false;
            btnClear.Visible = false;
            btnClear.Hide();

            btnDuplicate.Enabled = false;
            btnDuplicate.Visible = false;
            btnDuplicate.Hide();

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
                MessageBox.Show("Cannot open file codeline.txt for writing",
                                  Settings.Default.messageBoxTitle,MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            //Barcode reading thread starts
            BarcodeReading barcodeRead1 = new BarcodeReading(ref parentMF ,parentMF.Mfs100);
            parentMF.VoucherScanning(this);
            Thread.Sleep(500);

            List<string> rawBarcodesNew = barcodeRead1.getMyBarcodeList();
            rawBarcodes.AddRange(rawBarcodesNew);

            List<string> rawImagesNew = barcodeRead1.getMyImageList();
            rawImages.AddRange(rawImagesNew);

            for (int i = 0; i < rawBarcodesNew.Count; i++)
            {
                var vr = new ValidationResult();
                vr.barcode = rawBarcodesNew[i];
                SP.imgCnt++;
                if (vr.barcode == "" || vr.barcode == "No barcode") SP.invCnt++;
                else SP.validCnt++;
            }

            List < string > MICR_statusNew = new List<string>();
            MICR_control(rawBarcodesNew, ref MICR_statusNew);
            MICR_status.AddRange(MICR_statusNew);

            tempResults = new List<ValidationResult>();
            int n = 0;
            foreach (var rb in rawBarcodes)
            {
                var vr = new ValidationResult();
                vr.barcode = rb;
                //vr.errorMessage = "New";
                vr.MICR_status = MICR_status[n];
                tempResults.Add(vr);
                n++;
            }            

            btnStartReading.Enabled = true;
            btnStartReading.Visible = true;
            btnStartReading.Show();

            btnClear.Enabled = true;
            btnClear.Visible = true;
            btnClear.Show();

            btnDuplicate.Enabled = true;
            btnDuplicate.Visible = true;
            btnDuplicate.Show();

            btnSubmit.Enabled = true;
            btnSubmit.Visible = true;
            btnSubmit.Show();
          

            BindGrid(tempResults);
            duplicateCheck();
            BindGrid(tempResults);

            //SP.imgCnt = rawBarcodes.Count();
            statisticsItemsUpdateAndShow(lbStatistics,SP);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string outputPath;
            string date = DateTime.Now.ToString("yyyyMMdd_HHmm__");
            if (cbScanImages.Checked)
            {
                try
                {
                    for (int i = 0; i < rawImages.Count; i++)
                    {
                        outputPath = parentMF.mainFolderPath + "\\" + rawBarcodes[i] + ".jpg";
                        if (rawBarcodes[i] != "No barcode")
                        {
                            if (!File.Exists(outputPath))
                                File.Copy(rawImages[i], outputPath);
                            else
                            {
                                File.Delete(outputPath);
                                File.Copy(rawImages[i], outputPath);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Exception Occured while saving images " + ex.ToString());
                }
            }
            DirectoryInfo directory = new DirectoryInfo(@"C:\RedRose\Images");
            //Clean the directory
            Empty(directory);

            btnStartReading.Enabled = false;
            btnStartReading.Visible = false;
            btnStartReading.Hide();

            btnClear.Enabled = true;
            btnClear.Visible = true;
            btnClear.Show();

            btnDuplicate.Enabled = true;
            btnDuplicate.Visible = true;
            btnDuplicate.Show();
            
            ExportToXls(sender, e);
            MessageBox.Show("File is exported to:" + xlsPath);
        }

        public void Empty(DirectoryInfo directory)
        {
            try
            {
                foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
                foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.ToString(), "Red Rose Scan-Solutions - MFS100", 
                //  MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //int error = 1;
            }

        }

        private void VF_Closing(object sender, FormClosingEventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo(@"C:\RedRose\Images");
            //Clean the directory
            Empty(directory);
            rawBarcodes.Clear();
            rawImages.Clear();
            parentMF.Close();
            this.Close();
        }

        private void BindGrid(List<ValidationResult> results)
        {
            dgwValidateScreen.Rows.Clear();
            dgwValidateScreen.AutoGenerateColumns = false;

            for (int i = 0; i < results.Count; i++)
            {
                ValidationResult result = results[i];

                //string valueText  = "";
                //string status = "";               
                //if (result.errorMessage == null ||
                //    result.errorMessage == "Voucher is available for submission")
                //{
                //    valueText = result.value + " " + result.unitType;
                //    status = result.errorMessage;
                //}
                //else
                //    status = result.errorMessage;

                int index = dgwValidateScreen.Rows.Add(i + 1, result.barcode,  result.MICR_status);
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
                        rawImages.RemoveAt(i);
                        if (next)dgwValidateScreen.Rows[i].Selected = false;
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
                    //errorMessage = "valid",
                    unitType = "USD",
                    //value = "50"
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
                    if ((barcodes[ii] == barcodes[i]) &&(barcodes[ii]!="No barcode"))
                    {
                        dgwValidateScreen.Rows.RemoveAt(ii);
                        barcodes.RemoveAt(ii);
                        rawBarcodes.RemoveAt(ii);
                        rawImages.RemoveAt(ii);
                        MICR_status.RemoveAt(ii);
                        tempResults.RemoveAt(ii);
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
        private void MICR_control(List <string> barcodes, ref List< string> MICR_status)
        {            
            var lines = File.ReadAllLines("codeline.txt");
            int i = 0;
            foreach (var line in lines)
            {
                string barcode = barcodes[i];
                if (barcode.Length > 11)
                {
                    barcode = barcode.Substring(0, 12);
                    string MICR_code = line.ToString();
                    MICR_status.Add(MICR_code.Length > 13 ? "Approved" : "No MICR code");
                }
                else MICR_status.Add("No barcode");
                i++;
            }

        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }


        /*****************************************************
         *                  xls read write                   *
         *****************************************************/

        private void Form1_Load(object sender, EventArgs e)
        {
            xlsPath = parentMF.mainFolderPath + "\\Test.xls";
            // create xls if not exists
            if (!File.Exists(xlsPath))
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());
                // create sheet
                sh = (HSSFSheet)wb.CreateSheet("Sheet1");
                // 1 rows, 2 columns
                for (int i = 0; i < dgwValidateScreen.RowCount; i++)
                {
                    var r = sh.CreateRow(i);
                    for (int j = 0; j < 5; j++)
                    {
                        r.CreateCell(j);
                    }
                }

                using (var fs = new FileStream(xlsPath, FileMode.Create, FileAccess.Write))
                {
                    wb.Write(fs);
                }
            }

            using (var fs = new FileStream(xlsPath, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(fs);

                //for (int i = 0; i < wb.Count; i++)
                //{
                //    comboBox1.Items.Add(wb.GetSheetAt(i).SheetName);
                //}
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            xlsPath = parentMF.mainFolderPath + "\\Output.xls";
            if (!File.Exists(xlsPath)) return;
            
            // create xls if not exists
            using (var fs = new FileStream(xlsPath, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(fs);
            }
            // get sheet
            sh = (HSSFSheet)wb.GetSheet("Sheet1");

            int i = 0;
            while (sh.GetRow(i) != null)
            {
                // add necessary columns
                if (dgwValidateScreen.Columns.Count < sh.GetRow(i).Cells.Count)
                {
                    for (int j = 0; j < sh.GetRow(i).Cells.Count; j++)
                        dgwValidateScreen.Columns.Add("", "");
                }

                // add row
                dgwValidateScreen.Rows.Add();
                // write row value
                for (int j = 0; j < sh.GetRow(i).Cells.Count; j++)
                {
                    var cell = sh.GetRow(i).GetCell(j);
                    if (cell != null)
                    {
                        // TODO: you can add more cell types capability, e. g. formula
                        switch (cell.CellType)
                        {
                            case NPOI.SS.UserModel.CellType.Numeric:
                                dgwValidateScreen[j, i].Value = sh.GetRow(i).GetCell(j).NumericCellValue;
                                break;
                            case NPOI.SS.UserModel.CellType.String:
                                dgwValidateScreen[j, i].Value = sh.GetRow(i).GetCell(j).StringCellValue;
                                break;
                            case NPOI.SS.UserModel.CellType.Unknown:
                                dgwValidateScreen[j, i].Value = sh.GetRow(i).GetCell(j).StringCellValue;
                                break;
                        }
                    }
                }
                i++;
            }
        }

        private void ExportToXls(object sender, EventArgs e)
        {
            string date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            xlsPath = parentMF.mainFolderPath + "\\Outputs__" + date + ".xls";

            if (!File.Exists(xlsPath)) WriteToXls();
            else
            {
                xlsPath = xlsPath.Substring(0, xlsPath.Length - 4) + "_2.xls";
                WriteToXls();
            }
        }

        private void WriteToXls()
        {
            wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());
            // create sheet
            sh = (HSSFSheet) wb.CreateSheet("Main");

            sh.CreateRow(0);
            sh.GetRow(0).CreateCell(0);
            sh.GetRow(0).CreateCell(1);
            sh.GetRow(0).CreateCell(2);
            sh.GetRow(0).GetCell(0).SetCellValue("Order");
            sh.GetRow(0).GetCell(1).SetCellValue("Barcode");
            sh.GetRow(0).GetCell(2).SetCellValue("MICR Control");

            for (int i = 0; i < dgwValidateScreen.RowCount; i++)
            {
                if (sh.GetRow(i + 1) == null)
                    sh.CreateRow(i + 1);
                //TODO: else

                for (int j = 0; j < dgwValidateScreen.ColumnCount; j++)
                {
                    if (sh.GetRow(i + 1).GetCell(j) == null) sh.GetRow(i + 1).CreateCell(j);

                    if (dgwValidateScreen[j, i].Value != null)
                        sh.GetRow(i + 1).GetCell(j).SetCellValue(dgwValidateScreen[j, i].Value.ToString());
                }
            }

            //Write Statistics
            for (int i = 2; i < 6; i++)
            {
                if (sh.GetRow(i) == null) sh.CreateRow(i);

                sh.GetRow(i).CreateCell(4);
                sh.GetRow(i).CreateCell(5);
            }

            sh.GetRow(2).GetCell(4).SetCellValue("Processed:");
            sh.GetRow(2).GetCell(5).SetCellValue(SP.imgCnt);

            sh.GetRow(3).GetCell(4).SetCellValue("Valid:");
            sh.GetRow(3).GetCell(5).SetCellValue(SP.validCnt);

            sh.GetRow(4).GetCell(4).SetCellValue("Invalid:");
            sh.GetRow(4).GetCell(5).SetCellValue(SP.invCnt);

            sh.GetRow(5).GetCell(4).SetCellValue("Reused:");
            sh.GetRow(5).GetCell(5).SetCellValue(SP.reusedCnt);
            try
            {
                using (var fs = new FileStream(xlsPath, FileMode.Create, FileAccess.Write))
                {
                    wb.Write(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("XLS writing error:\n " + ex.ToString(),
                    Settings.Default.messageBoxTitle,MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}
