using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using OnBarcode.Barcode.BarcodeScanner;
//using BarcodeLib.BarcodeReader;
using ZXing;

//Tolga Yildiran
namespace RedRose_VoucherScanner
{
    public partial class Setup2Form : Form
    {
        public CFG tConfig;        
        public MainForm s2_MF;       
        public Setup2Form(CFG tClassAdress, MainForm myMF)
        {
            InitializeComponent();
            // Copy structure reference 
            tConfig = tClassAdress;
            s2_MF = myMF;
            // Preset Feeding - Timeout
            tbFeedTimeout.Text = tConfig.tFeeding.iTimeout.ToString();
            // Preset Feeding - Doublefeed threshold
            tbDoublefeedThreshold.Text =
                              tConfig.tFeeding.iDoublefeedThreshold.ToString();
      
            // Preset Scanning - Maximum image width
            tbMaxImageWidth.Text = tConfig.tScanning.iMaxImageWidth.ToString();
            // Preset Scanning - Maximum image height
            tbMaxImageHeight.Text =
                                  tConfig.tScanning.iMaxImageHeight.ToString();           
            // Preset Scanning - Image directory
            tbImageDirectory.Text =
                                 tConfig.tScanning.sbImageDirectory.ToString();
            // Preset Scanning - Front side image 1 - File name
           
                     
            
            cbReadFont.DropDownStyle = ComboBoxStyle.DropDownList;
            switch(tConfig.tReading.eFont)
            {
                case CFG.READ_FONT.NONE:
                    {
                        cbReadFont.SelectedIndex = 0;
                        break;
                    }
                case CFG.READ_FONT.E13B_MAGNETIC:
                    {
                        cbReadFont.SelectedIndex = 1;
                        break; 
                    }
                case CFG.READ_FONT.E13B_MAGNETIC_AND_OPTICAL:
                    {
                        cbReadFont.SelectedIndex = 2;
                        break; 
                    }
                case CFG.READ_FONT.E13B_OPTICAL:
                    {
                        cbReadFont.SelectedIndex = 3;
                        break;
                    }
                case CFG.READ_FONT.OCRB:
                    {
                        cbReadFont.SelectedIndex = 1;
                        break; 
                    }
                    
            }
           
            
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Set result to Cancel
            this.DialogResult = DialogResult.Cancel;
            //Exit setup form
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Get value of Feeding - Source
            
            
            // Get value of Feeding - Timeout
            tConfig.tFeeding.iTimeout = Convert.ToInt32(tbFeedTimeout.Text);
            // Get value of Feeding - Doublefeed threshold
            tConfig.tFeeding.iDoublefeedThreshold =
                                   Convert.ToInt32(tbDoublefeedThreshold.Text);           
            // Get value of Scanning - Maximum image width
            tConfig.tScanning.iMaxImageWidth =
                                         Convert.ToInt32(tbMaxImageWidth.Text);
            // Get value of Scanning - Maximum image height
            tConfig.tScanning.iMaxImageHeight =
                                        Convert.ToInt32(tbMaxImageHeight.Text);          
            // Get value of Scanning - Image directory
            tConfig.tScanning.sbImageDirectory.Length = 0;
            tConfig.tScanning.sbImageDirectory.Insert(0,
                                                        tbImageDirectory.Text);

            tConfig.tBarcode = BarcodeFormat.CODE_128;

            switch (cbReadFont.SelectedIndex)
            {
                case 0:
                    {
                        //None
                        tConfig.tReading.eFont = CFG.READ_FONT.NONE;
                        tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NONE;
                        tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NONE;
                        tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;
                        break;
                    }
                case 1:
                    {
                        // All pure magnetic fonts
                        tConfig.tReading.eFont = CFG.READ_FONT.E13B_MAGNETIC;
                        tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;
                        break;
                    }
                case 2:
                    tConfig.tReading.eFont = CFG.READ_FONT.E13B_MAGNETIC_AND_OPTICAL;
                    tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NORMAL;
                    tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NORMAL;
                    tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;
                    break;
                case 3:
                    tConfig.tReading.eFont = CFG.READ_FONT.E13B_OPTICAL;
                    tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NORMAL;
                    tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NORMAL;
                    tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;
                    break;
                case 4:
                    {
                        // All pure optical fonts
                        tConfig.tReading.eFont = CFG.READ_FONT.OCRB;
                        tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;
                        break;
                    }
                default:
                    {
                        /// All pure magnetic fonts
                        tConfig.tReading.eFont = CFG.READ_FONT.E13B_MAGNETIC;
                        tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NONE;
                        tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;
                        break;
                    }
                    
            }

            if (tConfig.tScanning.tFrontSide1.sbFileName.ToString() == "" ||
                        tConfig.tScanning.tFrontSide1.sbFileName.ToString() == null)
                tConfig.tScanning.tFrontSide1.sbFileName.Insert(0, "FS-1%04d");


            s2_MF.StoreConfiguration(tConfig);
            // Set result to OK
            this.DialogResult = DialogResult.OK;
            //Exit setup form
            this.Close();
        }

      

        private void tbFeedTimeout_TextChanged(object sender, EventArgs e)
        {
            String strBuffer = "";
            int iCounter;

            // Extract all digits from text
            iCounter = 0;
            while (iCounter < tbFeedTimeout.TextLength)
            {
                if (char.IsDigit(tbFeedTimeout.Text, iCounter) == true)
                {
                    strBuffer += tbFeedTimeout.Text.Substring(iCounter, 1);
                }
                ++iCounter;
            }
            // If text contains not only digits, remove non-digit chars
            if (strBuffer != tbFeedTimeout.Text)
            {
                tbFeedTimeout.Text = strBuffer;
                tbFeedTimeout.SelectionStart = strBuffer.Length;
            }
        }

        private void tbDoublefeedThreshold_TextChanged(object sender, EventArgs e)
        {
            String strBuffer = "";
            int iCounter;

            // Extract all digits from text
            iCounter = 0;
            while (iCounter < tbDoublefeedThreshold.TextLength)
            {
                if (char.IsDigit(tbDoublefeedThreshold.Text, iCounter) == true)
                {
                    strBuffer += tbDoublefeedThreshold.Text.Substring(iCounter,
                                                                              1);
                }
                ++iCounter;
            }
            // If text contains not only digits, remove non-digit chars
            if (strBuffer != tbDoublefeedThreshold.Text)
            {
                tbDoublefeedThreshold.Text = strBuffer;
                tbDoublefeedThreshold.SelectionStart = strBuffer.Length;
            }
        }

        private void tbMaxImageWidth_TextChanged(object sender, EventArgs e)
        {
            String strBuffer = "";
            int iCounter;

            // Extract all digits from text
            iCounter = 0;
            while (iCounter < tbMaxImageWidth.TextLength)
            {
                if (char.IsDigit(tbMaxImageWidth.Text, iCounter) == true)
                {
                    strBuffer += tbMaxImageWidth.Text.Substring(iCounter, 1);
                }
                ++iCounter;
            }
            // If text contains not only digits, remove non-digit chars
            if (strBuffer != tbMaxImageWidth.Text)
            {
                tbMaxImageWidth.Text = strBuffer;
                tbMaxImageWidth.SelectionStart = strBuffer.Length;
            }
        }

        private void tbMaxImageHeight_TextChanged(object sender, EventArgs e)
        {
            String strBuffer = "";
            int iCounter;

            // Extract all digits from text
            iCounter = 0;
            while (iCounter < tbMaxImageHeight.TextLength)
            {
                if (char.IsDigit(tbMaxImageHeight.Text, iCounter) == true)
                {
                    strBuffer += tbMaxImageHeight.Text.Substring(iCounter, 1);
                }
                ++iCounter;
            }
            // If text contains not only digits, remove non-digit chars
            if (strBuffer != tbMaxImageHeight.Text)
            {
                tbMaxImageHeight.Text = strBuffer;
                tbMaxImageHeight.SelectionStart = strBuffer.Length;
            }
        }

        

        private void btnImageDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdGetImageDirectory =
                                                  new FolderBrowserDialog();

            // If actually no image directory is given
            if (tbImageDirectory.Text == "")
            {
                // Select actual directory
                fbdGetImageDirectory.SelectedPath =
                                      System.IO.Directory.GetCurrentDirectory();
            }
            else
            {
                // Select given directory
                fbdGetImageDirectory.SelectedPath = tbImageDirectory.Text;
            }
            // Preset file dialog structure
            fbdGetImageDirectory.Description = "Select image directory";
            fbdGetImageDirectory.ShowNewFolderButton = true;
            // If image directory was selected
            if (fbdGetImageDirectory.ShowDialog() == DialogResult.OK)
            {
                // Update image directory in textbox
                tbImageDirectory.Text = fbdGetImageDirectory.SelectedPath;
            }
        }
       

        private void cbReadFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbReadFont.SelectedIndex)
            {
                case 0:
                    {
                        //None
                        tConfig.tReading.eFont = CFG.READ_FONT.NONE;
                        tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NONE;
                        tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NONE;
                        tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;                               
                        break;
                    }
                case 1:                
                    {
                        // All pure magnetic fonts
                        tConfig.tReading.eFont = CFG.READ_FONT.E13B_MAGNETIC;
                        tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;                        
                        break;
                    }
                case 2:
                        tConfig.tReading.eFont = CFG.READ_FONT.E13B_MAGNETIC_AND_OPTICAL;
                        tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;
                        break;
                case 3:
                        tConfig.tReading.eFont = CFG.READ_FONT.E13B_OPTICAL;
                        tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;
                        break;
                case 4:                
                    {
                        // All pure optical fonts
                        tConfig.tReading.eFont = CFG.READ_FONT.OCRB;
                        tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE; 
                        break;
                    }
                default:
                    {
                        /// All pure magnetic fonts
                        tConfig.tReading.eFont = CFG.READ_FONT.E13B_MAGNETIC;
                        tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NORMAL;
                        tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NONE;
                        tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;
                        break;
                    }
            }
        }

        
    }
}
