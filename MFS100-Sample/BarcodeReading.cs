using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WD_MFS100DN;
//using OnBarcode.Barcode.BarcodeScanner;
using System.Threading;
using System.IO;
//using BarcodeLib.BarcodeReader;
using ZXing;

namespace RedRose_VoucherScanner
{
    public partial class BarcodeReading : Form
    {
        IBarcodeReader reader = new BarcodeReader();
        List<BarcodeFormat> barcodeReaderPosFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_128, BarcodeFormat.DATA_MATRIX};

        //BarcodeFormat br_Tbarcode;
        CFG br_Tconfig;
        MFS100DN br_Mfs100;
       
        public string[] br_myBarcode = null;
        private List<string> br_myBarcodeList = new List<string>();
        //public BarcodeReading(ref CFG myTconfig, MFS100DN myMFS100, ref ValidForm myVS, ref statParams mySP)
        public BarcodeReading(ref MainForm MF, MFS100DN myMFS100)
        {
           
            
            br_Tconfig = MF.tConfig;
            br_Mfs100 = myMFS100;
            //br_Tbarcode = br_Tconfig.tBarcode;
            //barcodeReaderPosFormats = new List<BarcodeFormat> {br_Tbarcode};
            //br_VS = myVS;
            //br_SP = mySP;

            DirectoryInfo directory = new DirectoryInfo(br_Tconfig.tScanning.sbImageDirectory.ToString());
            //Clean the directory
            Empty(directory);

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = br_Tconfig.tScanning.sbImageDirectory.ToString();
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = br_Tconfig.tScanning.tFrontSide1.eFormat.ToString();
            switch (watcher.Filter)
            {
                case "JPEG":
                    {
                        watcher.Filter = "*.jpg";
                        break;
                    }
                case "None":
                    {
                        watcher.Filter = "*.tif";
                        break;
                    }
                case "BMP":
                    {
                        watcher.Filter = "*.bmp";
                        break;
                    }
                case "TIFF_G4":
                    {
                        watcher.Filter = "*.tif";
                        break;
                    }
            }
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;           
            watcher.Created += new FileSystemEventHandler(OnChanged);

            //string[] filePaths = Directory.GetFiles(@"C:\Users\m.alsadi\Desktop\MFS_outs\", "*.jpg");
        }

        public string[] getMyBarcode()
        {
            return br_myBarcode; 
        }

        public List<string> getMyBarcodeList()
        {
            return br_myBarcodeList;
        }

        public void OnChanged(object sender, FileSystemEventArgs e)
        {
            string imgFileName = e.FullPath;
           
            try 
            {
                //using OnBarcode.Barcode.BarcodeScanner;
               //br_myBarcode = BarcodeScanner.Scan(imgFileName, br_Tbarcode);

                //using BarcodeLib.BarcodeReader;
                //br_myBarcode = BarcodeReader.read(imgFileName, BarcodeReader.CODE128);
             
                //br_myBarcode = getBarcode(new Bitmap(imgFileName));


                //using ZXing;
                string barcode = getBarcode(new Bitmap(imgFileName));
                if(barcode == "")
                    br_myBarcodeList.Add("No barcode");
                else
                    br_myBarcodeList.Add(barcode);
            }
            catch(Exception ex)          
            {
                br_myBarcodeList.Add("No barcode");
               
            }

        }

        public void setupFileNameCheck(ref CFG cfg)
        {            
            if (cfg.tScanning.tFrontSide1.sbFileName.ToString() == "")
                cfg.tScanning.tFrontSide1.sbFileName.Insert(0,"FS-1%04d");
 
        }

        public void Empty( DirectoryInfo directory)
        {
            try
            {
                foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
                foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "Red Rose Scan-Solutions - MFS100", 
                  //  MessageBoxButtons.OK, MessageBoxIcon.Stop);
                int error = 1;

            }
           
        }

        private string getBarcode(Bitmap img)
        {
            if (img == null)
                return "";

            string text = "";
            if (img.Width > 0 && img.Height > 0)
            {
                reader.Options.TryHarder = true;
                reader.Options.PureBarcode = true;
                reader.Options.PossibleFormats = barcodeReaderPosFormats;
                Result resultDecode = reader.Decode(img);
                if (resultDecode != null)
                    text = resultDecode.Text;
            }
            return text;
        }
     } 
}
