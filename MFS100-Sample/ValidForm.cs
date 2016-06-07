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

using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
// InternalWorkbook
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
        public string xlsPath = "";
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

            parentMF.validFormIsOpen = true;
            lblVendorName.Text = selectedVendor.ToString();

            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.btnDuplicate, "Compare scanned vouchers with existing vouchers at chosen excel file");
            toolTip1.SetToolTip(this.btnSubmit, "Export scanned vouchers to excel file");
            toolTip1.SetToolTip(this.btnClear, "Clear all information about scanned vouchers");
            toolTip1.SetToolTip(this.btnClose, "Close the application");
            toolTip1.SetToolTip(this.cbScanImages, "Save images of scanned vouchers to chosen folder");
            
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
            // 1. Barcode reading thread starts
            BarcodeReading barcodeRead1 = new BarcodeReading(ref parentMF ,parentMF.Mfs100);
            // 2. Voucher Scanner 
            try
            {
                parentMF.VoucherScanning(this);
            }
            catch
            {
                MessageBox.Show("Scaning Failed",
                                  Settings.Default.messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            Thread.Sleep(500);

            List<string> rawBarcodesNew = barcodeRead1.getMyBarcodeList();
            rawBarcodes.AddRange(rawBarcodesNew);

            List<string> rawImagesNew = barcodeRead1.getMyImageList();
            rawImages.AddRange(rawImagesNew);
            ///////////////////////
            // Double Check
            //List<int> doubleCheckList=new List<int>();
            //for (int i = 0; i < rawBarcodesNew.Count; i++)
            //{
            //    var vr = new ValidationResult();
            //    vr.barcode = rawBarcodesNew[i];
            //    SP.imgCnt++;
            //    if (vr.barcode == "" || vr.barcode == "No barcode")
            //        doubleCheckList.Add(i);
            //}
            //for (int i = 0; i < doubleCheckList.Count; i++)
            //{
            //    string fnDouble = rawImagesNew[doubleCheckList[i]];
            //    string returnCode = "";
            //    barcodeRead1.readBarcodeFromList(fnDouble, returnCode);
            //}
            ///////////////////////
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
                    MessageBox.Show(@"Exception Occured while saving images " + ex.ToString(),Settings.Default.messageBoxTitle);
                }
            }
            DirectoryInfo directory = new DirectoryInfo(@"C:\RedRose\Images");
            //Clean the directory
            Empty(directory);

            if(ExportToXls(sender, e, parentMF.xlsSelected))
            MessageBox.Show("File is exported to:" + xlsPath, Settings.Default.messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnStartReading.Enabled = false;
            btnStartReading.Visible = false;
            btnStartReading.Hide();

            btnClear.Enabled = true;
            btnClear.Visible = true;
            btnClear.Show();

            btnDuplicate.Enabled = true;
            btnDuplicate.Visible = true;
            btnDuplicate.Show();
            
          
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
            lbStatistics.Items.Add(mySP.duplicateCnt.ToString());
            lbStatistics.Update();
        }
        private void MICR_control(List <string> barcodes, ref List< string> MICR_status)
        {            
            var lines = File.ReadAllLines("codeline.txt");
            int i = 0;
            foreach (var line in lines)
            {
                if (barcodes.Count > (i))
                {
                    string barcode = barcodes[i];
                    if (barcode.Length > 11)
                    {
                        barcode = barcode.Substring(0, 12);
                        string MICR_code = line.ToString();
                        MICR_status.Add(MICR_code.Length > 13 ? "Approved" : "No MICR code");
                    }
                    else MICR_status.Add("No barcode");
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
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString(),Settings.Default.messageBoxTitle);
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

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            if (!parentMF.xlsSelected)
            {
                MessageBox.Show("Xls file not selected",Settings.Default.messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            xlsPath = parentMF.xlsSelected ? parentMF.mainXlsPath : parentMF.mainFolderPath + "\\Outputs.xls";
            xlsPath = parentMF.mainXlsPath;
            List<string> prevBarcode = new List<string>();
            if (!File.Exists(xlsPath)) return;

            try
            {
                // create xls if not exists
                using (var fs = new FileStream(xlsPath, FileMode.Open, FileAccess.Read))
                {
                    wb = new HSSFWorkbook(fs);
                }
                // get sheet
                sh = (HSSFSheet) wb.GetSheet("Sheet1");
                int i = 1;
                while (sh.GetRow(i) != null)
                {
                    var cell = sh.GetRow(i).GetCell(1);
                    if (cell != null) prevBarcode.Add(cell.ToString());
                    i++;
                }
                List<int> existBarList = new List<int>();
                for (int j = 0; j < dgwValidateScreen.RowCount; j++)
                {
                    bool exist = false;
                    string bar = dgwValidateScreen[1, j].Value.ToString();
                    for (int k = 0; k < prevBarcode.Count; k++)
                    {
                        string temp = prevBarcode[k];
                        exist = (bar == prevBarcode[k]) ? true : false;
                        if (exist)
                        {
                            existBarList.Add(j);
                            break;
                        }
                    }
                }

                dgwValidateScreen.Rows[0].Selected = false;
                foreach (var ind in existBarList)
                    dgwValidateScreen.Rows[ind].Selected = true;

                SP.duplicateCnt = existBarList.Count;
                statisticsItemsUpdateAndShow(lbStatistics, SP);
            }
            catch (Exception ex)
            {
                MessageBox.Show("XLS writing error:\n " + ex.ToString(),
                    Settings.Default.messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                
            }
            
        }

        private bool ExportToXls(object sender, EventArgs e, bool xlsSelected)
        {
            bool done = false;
            xlsPath = xlsSelected ? parentMF.mainXlsPath : parentMF.mainFolderPath + "\\Outputs.xls";

            if (!xlsSelected)
            {
                while (File.Exists(xlsPath))
                {
                    bool write = RenameXls();
                    if (!write)
                        break;
                }
                
                done = WriteAtEmptyXls();
            }
            else
            {
                done = WriteBelowExistingXls();
            }
            return done;

        }

        private bool WriteBelowExistingXls()
        {
            
            // create xls if not exists
            try
            {
                using (var fs = new FileStream(xlsPath, FileMode.Open, FileAccess.ReadWrite))
                    wb = new HSSFWorkbook(fs);
                // get sheet
                sh = (HSSFSheet) wb.GetSheet("Sheet1");
                int lastRow = 0;
                if (sh != null) lastRow = sh.LastRowNum;

                int ii = 0;
                for (int i = lastRow; i < lastRow + dgwValidateScreen.RowCount; i++, ii++)
                {
                    if (sh.GetRow(i + 1) == null) sh.CreateRow(i + 1);

                    for (int j = 0; j < dgwValidateScreen.ColumnCount; j++)
                    {
                        if (sh.GetRow(i + 1).GetCell(j) == null) sh.GetRow(i + 1).CreateCell(j);

                        if (dgwValidateScreen[j, ii].Value != null)
                            sh.GetRow(i + 1).GetCell(j).SetCellValue(dgwValidateScreen[j, ii].Value.ToString());
                    }
                    //Add Date Column
                    string date = DateTime.Now.ToString("yyyyMMdd_HHmm");
                    if (sh.GetRow(i + 1).GetCell(3) == null) sh.GetRow(i + 1).CreateCell(3);

                    sh.GetRow(i + 1).GetCell(3).SetCellValue(date);
                    using (var fs = new FileStream(xlsPath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        wb.Write(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("XLS writing error:\n " + ex.ToString(),
                    Settings.Default.messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            return true;


        }
            
        private bool RenameXls()
        {
            DialogResult dresult = MessageBox.Show("The file at " + xlsPath +
                            " is exist. \n Do you want to rename as " + xlsPath.Substring(0, xlsPath.Length - 4) 
                            + "_2.xls", Settings.Default.messageBoxTitle, MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Question);
            xlsPath = (dresult == DialogResult.Yes)
                ? xlsPath.Substring(0, xlsPath.Length - 4) + "_2.xls"
                : xlsPath;
            bool result = (dresult == DialogResult.Yes) ?  true:  false;
            return result;
        }

        private bool WriteAtEmptyXls()
        {
            try
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());
                // create sheet
                sh = (HSSFSheet)wb.CreateSheet("Sheet1");
                WriteHeaderToEmptyXls();

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
                    //Add Date Column
                    string date = DateTime.Now.ToString("yyyyMMdd_HHmm");
                    if (sh.GetRow(i + 1).GetCell(3) == null) sh.GetRow(i + 1).CreateCell(3);
                    sh.GetRow(i + 1).GetCell(3).SetCellValue(date);
                }

                using (var fs = new FileStream(xlsPath, FileMode.Create, FileAccess.Write))
                {
                    wb.Write(fs);
                }

                //Write Statistics Screen to Right Side of XLS
                //WriteStatsToXls();
            }
            catch (Exception ex)
            {
                MessageBox.Show("XLS writing error:\n " + ex.ToString(),
                    Settings.Default.messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                return false;
            }

            return true;
        }

        private void WriteHeaderToEmptyXls()
        {
            sh.CreateRow(0);
            sh.GetRow(0).CreateCell(0);
            sh.GetRow(0).CreateCell(1);
            sh.GetRow(0).CreateCell(2);
            sh.GetRow(0).CreateCell(3);
            sh.GetRow(0).GetCell(0).SetCellValue("Order");
            sh.GetRow(0).GetCell(1).SetCellValue("Barcode");
            sh.GetRow(0).GetCell(2).SetCellValue("MICR Control");
            sh.GetRow(0).GetCell(3).SetCellValue("Date");
        }

        private void WriteStatsToXls()
        {
            for (int i = 2; i < 6; i++)
            {
                if (sh.GetRow(i) == null) sh.CreateRow(i);

                sh.GetRow(i).CreateCell(5);
                sh.GetRow(i).CreateCell(6);
            }

            sh.GetRow(2).GetCell(5).SetCellValue("Processed:");
            sh.GetRow(2).GetCell(6).SetCellValue(SP.imgCnt);

            sh.GetRow(3).GetCell(5).SetCellValue("Valid:");
            sh.GetRow(3).GetCell(6).SetCellValue(SP.validCnt);

            sh.GetRow(4).GetCell(5).SetCellValue("Invalid:");
            sh.GetRow(4).GetCell(6).SetCellValue(SP.invCnt);

            sh.GetRow(5).GetCell(5).SetCellValue("Reused:");
            sh.GetRow(5).GetCell(6).SetCellValue(SP.reusedCnt);
        }
    }
}
