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
using ZXing;
using System.Threading;
using System.IO;
using RedRose_VoucherScanner.Properties;
using ZBar;
//using OnBarcode.Barcode.BarcodeScanner;
//using BarcodeLib.BarcodeReader;

namespace RedRose_VoucherScanner
{
   public partial class MainForm : Form
   {
      public StringBuilder  sbConfigFileName = new StringBuilder();
      public StringBuilder  sbErrorMsg = new StringBuilder();
      public StringBuilder  sbFrontSide1FileName = new StringBuilder();
      public StringBuilder  sbFrontSide2FileName = new StringBuilder();
      public StringBuilder  sbRearSide1FileName = new StringBuilder();
      public StringBuilder  sbRearSide2FileName = new StringBuilder();
      public CFG  tConfig = new CFG();
      public MFS100DN  Mfs100 = new MFS100DN();
      public STATUS_NORMAL  tStatusSW = new STATUS_NORMAL();
      public STATUS_INQUIRY  tStatusInquiry = new STATUS_INQUIRY();
      public bool  bImagesAvailable = false;
      public bool  bOnCodelineCalledTwice = false;
      public bool  bFirstCall = false;
      public bool  bMatchFound = false;
      public bool  bInvertMatch = false;
      public int  iOldDocId = 0;
      public int  iSide = 0;
      public int  iResult = 0;
      public uint  wmOnCodeline = 0;
      public uint  wmOnRawFrontImage = 0;
      public uint  wmOnRawRearImage = 0;
      public uint  wmOnRawMicrImage = 0;
      public uint  wmOnRawOcrImage = 0;
      public uint  wmOnDocumentDone = 0;
      public uint  wmOnWarning = 0;
      public uint  wmOnError = 0;
      public uint  wmOnAllDone = 0;
      public Regex  reRegularExpression;
       //tolga
      public uint PUB_uiReadFont;
      public int PUB_iFeedMode;
      public StringBuilder PUB_sbValue;
      public CFG defaultCFG = new CFG();
      private string username;
      private string password;
      public string mainFolderPath;

      public string Username
      {
          get
          {
              return this.username;
          }
      }

      public string Password
      {
          get { return this.password; }
      }
      // Override default CreateParams to disable close icon (X)
      protected override CreateParams CreateParams
      {
         get
         {
            CreateParams parms = base.CreateParams;
            parms.ClassStyle |= 0x200;  // CS_NOCLOSE
            return parms;
         }
      }         

      // Override default WndProc to handle messages for callback functions
      protected override void WndProc(ref Message msgMessage)
      {
         if (msgMessage.Msg == wmOnCodeline)
         {
            // API event ON_CODELINE occured
            DelegateOnCodeline();
            return;
         }
         if (msgMessage.Msg == wmOnRawFrontImage)
         {
            // API event ON_RAW_FRONT occured
            // Enable next line if using Possibility #1 or #2
            // DelegateOnRawFront(); -> will not be used
            return;
         }
         if (msgMessage.Msg == wmOnRawRearImage)
         {
            // API event ON_RAW_BACK occured
            // Enable next line if using Possibility #1 or #2
            // DelegateOnRawRear(); -> will not be used
            return;
         }
         if (msgMessage.Msg == wmOnRawMicrImage)
         {
            // API event ON_RAW_MICR occured
            // Will not be used -> ignore it
            return;
         }
         if (msgMessage.Msg == wmOnRawOcrImage)
         {
            // API event ON_RAW_OCR_BUFF occured
            // Will not be used -> ignore it
            return;
         }
         if (msgMessage.Msg == wmOnDocumentDone)
         {
            // API event ON_DOCUMENT_DONE occured
            DelegateOnDocumentDone();
            return;
         }
         if (msgMessage.Msg == wmOnWarning)
         {
            // API event ON_WARNING occured
            DelegateOnWarning();
            // Will not be used -> ignore it
            return;
         }
         if (msgMessage.Msg == wmOnError)
         {
            // API event ON_ERROR occured
            DelegateOnError();
            return;
         }
         if (msgMessage.Msg == wmOnAllDone)
         {
            // API event ON_ALL_DONE occured
            DelegateOnAllDone();
            return;
         }
         // Handle other messages to standard handler
         base.WndProc(ref msgMessage);
      }

      public MainForm(string username, string password)
      {

          this.username = username;
          this.password = password;
         String  szPath;      

         InitializeComponent();
         // Get application path
        
         szPath = System.IO.Path.GetDirectoryName(System.Reflection.
                                  Assembly.GetExecutingAssembly().Location);
         sbConfigFileName.Length = 0;
         //sbConfigFileName.Insert(0, @"C:\RedRose\Images");
         sbConfigFileName.Insert(0, szPath);
         sbConfigFileName.Append("\\MFS100-Sample.ini");
         // Get window messages of callbacks
         wmOnCodeline = Mfs100.GetWindowMessage(WINDOW_MESSAGE_TYPE.EVENT_ON_CODELINE);
         wmOnRawFrontImage = Mfs100.GetWindowMessage(WINDOW_MESSAGE_TYPE.EVENT_ON_RAW_FRONT_IMAGE);
         wmOnRawRearImage = Mfs100.GetWindowMessage(WINDOW_MESSAGE_TYPE.EVENT_ON_RAW_REAR_IMAGE);
         wmOnRawMicrImage = Mfs100.GetWindowMessage(WINDOW_MESSAGE_TYPE.EVENT_ON_RAW_MICR_IMAGE);
         wmOnRawOcrImage = Mfs100.GetWindowMessage(WINDOW_MESSAGE_TYPE.EVENT_ON_RAW_OCR_IMAGE);
         wmOnDocumentDone = Mfs100.GetWindowMessage(WINDOW_MESSAGE_TYPE.EVENT_ON_DOCUMENT_DONE);
         wmOnWarning = Mfs100.GetWindowMessage(WINDOW_MESSAGE_TYPE.EVENT_ON_WARNING);
         wmOnError = Mfs100.GetWindowMessage(WINDOW_MESSAGE_TYPE.EVENT_ON_ERROR);
         wmOnAllDone = Mfs100.GetWindowMessage(WINDOW_MESSAGE_TYPE.EVENT_ON_ALL_DONE);
         // Initialisation of parameter structure
         tConfig.tMfsParameters.ciIndexes = new int[97] {        0x001, 0x002, 0x003, 0x004, 0x005, 0x006, 0x007, //   7
                                                          0x008, 0x009, 0x00A, 0x00B, 0x00C, 0x00D, 0x00E, 0x00F, // + 8
                                                          0x010, 0x011,        0x013, 0x014, 0x015, 0x016, 0x017, // + 7
                                                          0x018, 0x019, 0x01A, 0x01B, 0x01C, 0x01D, 0x01E, 0x01F, // + 8
                                                          0x020, 0x021, 0x022, 0x023, 0x024, 0x025, 0x026, 0x027, // + 8
                                                          0x028, 0x029, 0x02A, 0x02B, 0x02C, 0x02D, 0x02E, 0x02F, // + 8
                                                          0x030, 0x031, 0x032, 0x033, 0x034, 0x035, 0x036, 0x037, // + 8
                                                          0x038, 0x039, 0x03A, 0x03B, 0x03C,                      // + 5
                                                                        0x04A, 0x04B, 0x04C, 0x04D, 0x04E, 0x04F, // + 6
                                                          0x050, 0x051, 0x052, 0x053, 0x054, 0x055, 0x056, 0x057, // + 8
                                                          0x058, 0x059, 0x05A,                                    // + 3
                                                          0x080, 0x081, 0x082,                                    // + 3
                                                          0x090, 0x091, 0x092, 0x093,                             // + 4
                                                                                                    0x11E, 0x11F, // + 2
                                                          0x120, 0x121, 0x122, 0x123,                             // + 4
                                                                 0x129, 0x12A, 0x12B, 0x12C, 0x12D, 0x12E,        // + 6
                                                                               0x13B, 0x13C };                    // + 2
                                                                                                                  // ====
                                                                                                                  //  97
         tConfig.tMfsParameters.sbParameters = new StringBuilder[97];
         // Initialisation of configuration structure
         tConfig.tReading.sbSortPattern = new StringBuilder();
         tConfig.tEndorsing.sbFont = new StringBuilder();
         tConfig.tEndorsing.sbData = new StringBuilder();
         tConfig.tScanning.sbImageDirectory = new StringBuilder();
         
         tConfig.tScanning.tFrontSide1.sbFileName = new StringBuilder();
         tConfig.tScanning.tFrontSide2.sbFileName = new StringBuilder();
         tConfig.tScanning.tRearSide1.sbFileName = new StringBuilder();
         tConfig.tScanning.tRearSide2.sbFileName = new StringBuilder();
         // Initialisation of status structures
         tStatusSW.tSensors = new STATUS_SENSORS();
         tStatusSW.tMessages = new STATUS_MESSAGES();
         tStatusInquiry.sbName = new StringBuilder();
         tStatusInquiry.sbReader = new StringBuilder();
         tStatusInquiry.sbHWRevision = new StringBuilder();
         tStatusInquiry.sbSWRevision = new StringBuilder();
         // Hide buttons "Start", "Info", "Setup", "Eject forward", 
         // "Eject reverse" and "Exit" (the small exit button btnExit2)
         btnStart.Hide();
         btnInfo.Hide();
         btnSetup.Hide();
         btnOptions.Hide();
         btnEjectForward.Hide();
         btnEjectReverse.Hide();
         btnExit2.Hide();
         btnMICRtest.Hide();

        
      }

      private void pbImage_Click(object sender, EventArgs e)
      {
         System.IO.FileInfo  structFileInfo;
         System.IO.FileStream  structFileStream;

         // If images are available
         if (bImagesAvailable == true)
         {
            // Go to next image side
            switch (iSide)
            {
               case 0 :
               {
                  if (sbFrontSide2FileName.Length > 0)
                  {
                     iSide = 1;
                     break;
                  }
                  if (sbRearSide1FileName.Length > 0)
                  {
                     iSide = 2;
                     break;
                  }
                  if (sbRearSide2FileName.Length > 0)
                  {
                     iSide = 3;
                     break;
                  }
                  iSide = 0;
                  break;
               }
               case 1 :
               {
                  if (sbRearSide1FileName.Length > 0)
                  {
                     iSide = 2;
                     break;
                  }
                  if (sbRearSide2FileName.Length > 0)
                  {
                     iSide = 3;
                     break;
                  }
                  if (sbFrontSide1FileName.Length > 0)
                  {
                     iSide = 0;
                     break;
                  }
                  iSide = 1;
                  break;
               }
               case 2 :
               {
                  if (sbRearSide2FileName.Length > 0)
                  {
                     iSide = 3;
                     break;
                  }
                  if (sbFrontSide1FileName.Length > 0)
                  {
                     iSide = 0;
                     break;
                  }
                  if (sbFrontSide2FileName.Length > 0)
                  {
                     iSide = 1;
                     break;
                  }
                  iSide = 2;
                  break;
               }
               case 3 :
               {
                  if (sbFrontSide1FileName.Length > 0)
                  {
                     iSide = 0;
                     break;
                  }
                  if (sbFrontSide2FileName.Length > 0)
                  {
                     iSide = 1;
                     break;
                  }
                  if (sbRearSide1FileName.Length > 0)
                  {
                     iSide = 2;
                     break;
                  }
                  iSide = 3;
                  break;
               }
            }
            // Display image
            switch (iSide)
            {
               case 0 :
               {
                  structFileInfo = new System.IO.FileInfo(
                                           sbFrontSide1FileName.ToString());
                  try
                  {
                     structFileStream = structFileInfo.OpenRead();
                  }
                  catch
                  {
                     bImagesAvailable = false;
                     break;
                  }
                  pbImage.Image = 
                          System.Drawing.Image.FromStream(structFileStream);
                  structFileStream.Close();
                  break;
               }
               case 1 :
               {
                  structFileInfo = new System.IO.FileInfo(
                                           sbFrontSide2FileName.ToString());
                  try
                  {
                     structFileStream = structFileInfo.OpenRead();
                  }
                  catch
                  {
                     bImagesAvailable = false;
                     break;
                  }
                  pbImage.Image = 
                          System.Drawing.Image.FromStream(structFileStream);
                  structFileStream.Close();
                  break;
               }
               case 2 :
               {
                  structFileInfo = new System.IO.FileInfo(
                                            sbRearSide1FileName.ToString());
                  try
                  {
                     structFileStream = structFileInfo.OpenRead();
                  }
                  catch
                  {
                     bImagesAvailable = false;
                     break;
                  }
                  pbImage.Image = 
                          System.Drawing.Image.FromStream(structFileStream);
                  structFileStream.Close();
                  break;
               }
               case 3 :
               {
                  structFileInfo = new System.IO.FileInfo(
                                            sbRearSide2FileName.ToString());
                  try
                  {
                     structFileStream = structFileInfo.OpenRead();
                  }
                  catch
                  {
                     bImagesAvailable = false;
                     break;
                  }
                  pbImage.Image = 
                          System.Drawing.Image.FromStream(structFileStream);
                  structFileStream.Close();
                  break;
               }
            }
            Application.DoEvents();
         }
      }

      private void btnLaunch_Click(object sender, EventArgs e)
      {
          if (Directory.Exists(@"C:\RedRose"))
          {
              if (!Directory.Exists(@"C:\RedRose\Images"))
                  Directory.CreateDirectory(@"C:\RedRose\Images");
          }
          else
          {
              Directory.CreateDirectory(@"C:\RedRose");
              Directory.CreateDirectory(@"C:\RedRose\Images");
          }

          /*********/
         bool  bValid;
         int  iValue = 0;

         // Initialize machine and report it
         lbEvents.Items.Add("Initializing MFS100...");
         lbEvents.Update();
         iResult = Mfs100.Init(sbErrorMsg);
         lbEvents.Items.Add("mfInit() = " + iResult.ToString());
         lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
         lbEvents.SelectedIndex = -1;
         lbEvents.Update();
         // If machine was not correctly initialized
         if (iResult < 0)
         {
            // Show error message         
            MessageBox.Show("Error while initializing MFS100\r\nmfInit() " +
                                           "returned " + iResult.ToString(),
                                "Red Rose Scan-Solutions - MFS100",
                                 MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return;
         }
         // Machine was correctly initialized
         // Disable and hide buttons "Search USB for MFS100" and "Exit"
         // (the big exit button btnExit1)
         btnLaunch.Enabled = false;
         btnLaunch.Visible = false;
         btnLaunch.Hide();
         btnExit1.Enabled = false;
         btnExit1.Visible = false;
         btnExit1.Hide();
         // Enable and show buttons "Start", "Info", "Setup",
         // "Eject forward", "Eject reverse" and "Exit" (the small exit
         // button btnExit2)
         btnStart.Enabled = true;
         btnStart.Visible = true;
         btnStart.Show();
         btnInfo.Enabled = true;
         btnInfo.Visible = true;
         btnInfo.Show();
         btnSetup.Enabled = true;
         btnSetup.Visible = true;
         btnSetup.Show();
         btnOptions.Enabled = true;
         btnOptions.Visible = true;
         btnOptions.Show();
         btnEjectForward.Enabled = true;
         btnEjectForward.Visible = true;
         btnEjectForward.Show();
         btnEjectReverse.Enabled = true;
         btnEjectReverse.Visible = true;
         btnEjectReverse.Show();
         btnExit2.Enabled = true;
         btnExit2.Visible = true;
         btnExit2.Show();
         btnMICRtest.Enabled = false;
         btnMICRtest.Visible = false;
         btnMICRtest.Hide();

         // Read configuration from file
         ReadConfiguration(tConfig);
         // Validate configuration
         bValid = ValidateConfiguration(tConfig);
         // If configuration is invalid
         if (bValid != true)
         {
            // Call setup dialog for correction
            btnSetup_Click(null, null);
         }
         // Activate delegates in class library
         // *** Possibility #1 *********************************************
         // If raw images shall be used and images shall be saved 
         // additionally by API
         // iResult = Mfs100.SetEvents(this.Handle, 
         //                      (int) WINDOW_MESSAGE_TYPE.EVENT_ON_CODELINE +
         //               (int) WINDOW_MESSAGE_TYPE.EVENT_ON_RAW_FRONT_IMAGE +
         //                (int) WINDOW_MESSAGE_TYPE.EVENT_ON_RAW_REAR_IMAGE +
         //                 (int) WINDOW_MESSAGE_TYPE.EVENT_ON_DOCUMENT_DONE +
         //                         (int) WINDOW_MESSAGE_TYPE.EVENT_ON_ERROR +
         //                       (int) WINDOW_MESSAGE_TYPE.EVENT_ON_WARNING +
         //                       (int) WINDOW_MESSAGE_TYPE.EVENT_ON_ALL_DONE,
         //                             (int) RAW_IMAGE_TYPE.IMAGE_FRONTSIDE +
         //                               (int) RAW_IMAGE_TYPE.IMAGE_REARSIDE,
         //                                                       sbErrorMsg);
         // // Report event setting
         // lbEvents.Items.Add("Setting events = " + iResult.ToString());
         // lbEvents.Items.Add("- event ON_CODELINE");
         // lbEvents.Items.Add("- event ON_RAW_FRONT");
         // lbEvents.Items.Add("- event ON_RAW_REAR");
         // lbEvents.Items.Add("- event ON_DOCUMENT_DONE");
         // lbEvents.Items.Add(" -event ON_WARNING");
         // lbEvents.Items.Add(" -event ON_ERROR");
         // lbEvents.Items.Add(" -event ON_ALL_DONE");
         // lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
         // lbEvents.SelectedIndex = -1;
         // lbEvents.Update();
         // *** End of possibility #1 **************************************
         // *** Possibility #2 *********************************************
         // If raw images shall be used and images shall not be saved 
         // additionally by API
         // iResult = Mfs100.SetEvents(this.Handle, 
         //                      (int) WINDOW_MESSAGE_TYPE.EVENT_ON_CODELINE +
         //               (int) WINDOW_MESSAGE_TYPE.EVENT_ON_RAW_FRONT_IMAGE +
         //                (int) WINDOW_MESSAGE_TYPE.EVENT_ON_RAW_REAR_IMAGE +
         //                 (int) WINDOW_MESSAGE_TYPE.EVENT_ON_DOCUMENT_DONE +
         //                         (int) WINDOW_MESSAGE_TYPE.EVENT_ON_ERROR +
         //                       (int) WINDOW_MESSAGE_TYPE.EVENT_ON_WARNING +
         //                       (int) WINDOW_MESSAGE_TYPE.EVENT_ON_ALL_DONE,
         //                                                    0, sbErrorMsg);
         //
         // // Report event setting
         // lbEvents.Items.Add("Setting events = " + iResult.ToString());
         // lbEvents.Items.Add("- event ON_CODELINE");
         // lbEvents.Items.Add("- event ON_RAW_FRONT");
         // lbEvents.Items.Add("- event ON_RAW_REAR");
         // lbEvents.Items.Add("- event ON_DOCUMENT_DONE");
         // lbEvents.Items.Add(" -event ON_WARNING");
         // lbEvents.Items.Add(" -event ON_ERROR");
         // lbEvents.Items.Add(" -event ON_ALL_DONE");
         // lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
         // lbEvents.SelectedIndex = -1;
         // lbEvents.Update();
         // *** End of Possibility #2 **************************************
         // *** Possibility #3 *********************************************
         // If raw images shall not be used
         iResult = Mfs100.SetEvents(this.Handle, 
                               (int) WINDOW_MESSAGE_TYPE.EVENT_ON_CODELINE +
                          (int) WINDOW_MESSAGE_TYPE.EVENT_ON_DOCUMENT_DONE +
                                  (int) WINDOW_MESSAGE_TYPE.EVENT_ON_ERROR +
                                (int) WINDOW_MESSAGE_TYPE.EVENT_ON_WARNING +
                                (int) WINDOW_MESSAGE_TYPE.EVENT_ON_ALL_DONE,
                                                             0, sbErrorMsg);
         // Report event setting
         lbEvents.Items.Add("Setting events = " + iResult.ToString());
         lbEvents.Items.Add("- event ON_CODELINE");
         lbEvents.Items.Add("- event ON_DOCUMENT_DONE");
         lbEvents.Items.Add(" -event ON_WARNING");
         lbEvents.Items.Add(" -event ON_ERROR");
         lbEvents.Items.Add(" -event ON_ALL_DONE");
         lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
         lbEvents.SelectedIndex = -1;
         lbEvents.Update();
         // *** End of Possibility #3 **************************************
         // Get doublefeed threshold value from machine and report it
         lbEvents.Items.Add("Getting doublefeed threshold...");
         lbEvents.Update();
         iResult = Mfs100.Calib(CALIB_ID.DOUBLEFEED_DETECTION, 
                                           CALIB_MODE.GET_CALIBRATION_VALUE, 
                                                    ref iValue, sbErrorMsg);
         lbEvents.Items.Add("mfCalib() = " + iResult.ToString());
         lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
         lbEvents.SelectedIndex = -1;
         lbEvents.Update();
         // If doublefeed threshold value could be read from machine
         if (iResult > 0)
         {
            // Report it
            lbEvents.Items.Add("Doublefeed threshold = " + 
                                                         iValue.ToString());
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
         }
         // Get software status of machine and report it
         GetStatus();
      }

      private void btnExit1_Click(object sender, EventArgs e)
      {
         // Since machine was not initialized yet, do nothing before closing
         // Terminate application
         this.Close();
      }

      private void btnStart_Click(object sender, EventArgs e)
      {
         bool  bSortByScanAndEject;
         bool  bErrorsFound;
         int  iFeedMode;
         int  iCounter;
         int  iPosOfSeperator;
         int  iIndex1;
         int  iIndex2;
         uint  uiReadFont;
         EJECT_POCKET  ePocket;
         String  str1stCodeline;
         String  str2ndCodeline;
         StringBuilder  sbCodeline = new StringBuilder();
         StringBuilder  sbLine = new StringBuilder();
         StringBuilder  sbValue = new StringBuilder();        
         System.IO.FileInfo  structFileInfo;
         System.IO.FileStream  structFileStream;
         System.IO.StreamWriter  swFile = null;

         try
         {
             FolderBrowserDialog fbd = new FolderBrowserDialog();
             DialogResult result = fbd.ShowDialog();
             mainFolderPath = (result == DialogResult.OK) ? fbd.SelectedPath : @"C:\RedRose\Images";
         }
          catch (Exception ex)
          {
              // Show error message         
              MessageBox.Show("Folder Browser Error: " + ex.ToString(), 
                                  Settings.Default.messageBoxTitle,
                                   MessageBoxButtons.OK, MessageBoxIcon.Stop);
              throw ;
          }
         

         // Initialize regular expressions

         try
         {
            // If sort pattern is not empty
            if (tConfig.tReading.sbSortPattern.Length > 0)
            {
               reRegularExpression = 
                       new Regex(tConfig.tReading.sbSortPattern.ToString());
               // Use normal match handling
               bInvertMatch = false;
            }
            else
            {
               // Sort pattern is empty
               // Search for errors (?-signs)
               reRegularExpression = new Regex("[?]");
               // Since a match (error) shall be sorted into reject pocket
               // -> force inversion of match (= SORT_PATTERN_NEGATIVE)
               bInvertMatch = true;
            }
         }
         catch
         {
            // Show error message         
            MessageBox.Show("Cannot create regular expression -> sort " + 
                                                "pattern has to be invalid",
                                Settings.Default.messageBoxTitle,
                                 MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return;
         }
         // Set document counter to 0
         sbValue.Insert(0, "1");
         ParameterListInsert(MFS_PARAMETER_ID.DOC_COUNT, sbValue);
         // Setting parameters and report it
         SetParameter(tConfig);
         // Write seperation line into file (append it)
         try
         {
            swFile = new System.IO.StreamWriter( "codeline.txt", true );
         }
         catch
         {
            // Show error message         
            MessageBox.Show("Cannot open file codeline.txt for writing",
                                Settings.Default.messageBoxTitle,
                                 MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return;
         }
         if (swFile != null)
         {
            sbLine.Length = 0;
            sbLine.Insert(0, "----------------------------------------");
            swFile.WriteLine(sbLine.ToString());
            swFile.Close();
         }
         // Evaluate feeding mode
         switch (tConfig.tFeeding.eSource)
         {
            case CFG.FEED_SOURCE.A6_NORMAL_SINGLE :
            {
               iFeedMode = (int) OPTIONS.SINGLE_FEED_MODE;
               break;
            }
            case CFG.FEED_SOURCE.A6_STRAIGHT_SINGLE :
            {
               iFeedMode = (int) OPTIONS.A6_STRAIGHT_TRACK;
               break;
            }
            case CFG.FEED_SOURCE.A4_SINGLE :
            {
               iFeedMode = (int) OPTIONS.A4_TRACK;
               iFeedMode += (int) OPTIONS.SINGLE_FEED_MODE;
               break;
            }
            case CFG.FEED_SOURCE.A4_BATCH :
            {
               iFeedMode = (int) OPTIONS.A4_TRACK;
               break;
            }
            default :
            {
               // CFG.FEED_SOURCE.A6_NORMAL_BATCH
               iFeedMode = (int) OPTIONS.A6_NORMAL_TRACK;
               break;
            }
         }
         if (tConfig.tFeeding.bPipelineEnabled == false)
         {
            iFeedMode |= (int) OPTIONS.A6_NO_PIPELINE;
         }
         if (tConfig.tScanning.bTrueColor == true)
         {
            iFeedMode |= (int) OPTIONS.TRUE_COLOR_MODE;
         }
         if ((tConfig.tScanning.tFrontSide1.eFormat == 
                                           WD_MFS100DN.IMAGE_FORMAT.NONE) &&
                                   (tConfig.tScanning.tFrontSide2.eFormat ==
                                           WD_MFS100DN.IMAGE_FORMAT.NONE) &&
                                    (tConfig.tScanning.tRearSide1.eFormat ==
                                           WD_MFS100DN.IMAGE_FORMAT.NONE) &&
                                    (tConfig.tScanning.tRearSide2.eFormat ==
                                             WD_MFS100DN.IMAGE_FORMAT.NONE))
         {
            iFeedMode |= (int) OPTIONS.NO_IMAGES;
         }
         if (tConfig.tReading.eSortMode == CFG.READ_SORTMODE.MICR_GOOD_BAD)
         {
            iFeedMode |= (int) OPTIONS.A6_SORT_BY_MICR;
         }
         if (tConfig.tScanning.bUseCalibration == false)
         {
            iFeedMode |= (int) OPTIONS.NO_IMAGE_CALIBRATION;
         }
         // Convert reading font from listbox value to API value
         uiReadFont = ConvertFont(tConfig.tReading.eFont);
         // Start machine
         switch (tConfig.tFeeding.eSource)
         {
            case CFG.FEED_SOURCE.A6_NORMAL_SINGLE :
            case CFG.FEED_SOURCE.A6_STRAIGHT_SINGLE :
            case CFG.FEED_SOURCE.A4_SINGLE :
            {
               // Single feed
               lbEvents.Items.Add("Starting scanner (single)...");
               lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
               lbEvents.SelectedIndex = -1;
               lbEvents.Update();
               iResult = Mfs100.Scan(iFeedMode, uiReadFont,
                                      tConfig.tFeeding.iTimeout, sbCodeline,
                                                                sbErrorMsg);
               lbEvents.Items.Add("mfScan() = " + iResult.ToString());
               lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
               lbEvents.SelectedIndex = -1;
               lbEvents.Update();
               // Report read result
               lbEvents.Items.Add("  sbCodeline = " + 
                                                     sbCodeline.ToString());
               lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
               lbEvents.SelectedIndex = -1;
               lbEvents.Update();
               // Write codeline into file (append it)
               try
               {
                  swFile = new System.IO.StreamWriter( "codeline.txt",
                                                                     true );
               }
               catch
               {
                  // Show error message         
                  MessageBox.Show(
                                "Cannot open file codeline.txt for writing",
                                "Red Rose Scan-Solutions - MFS100",
                                 MessageBoxButtons.OK, MessageBoxIcon.Stop);
               }
               if (swFile != null)
               {
                  sbLine.Length = 0;
                  sbLine.Insert(0, "Doc: 00001 = ");
                  sbLine.Append(sbCodeline.ToString());
                  swFile.WriteLine(sbLine.ToString());
                  swFile.Close();
               }
               // If scanning was successful
               if (iResult >= 0)
               {
                  if (tConfig.tScanning.tFrontSide1.eFormat == 
                                              WD_MFS100DN.IMAGE_FORMAT.NONE)
                  {
                     sbFrontSide1FileName.Length = 0;
                  }
                  else
                  {
                     // Create file name of front side image 1 for document
                     // 1 
                     sbValue.Length = 0;
                     sbValue.Insert(0, tConfig.tScanning.sbImageDirectory.
                                                                ToString());
                     sbValue.Append("\\");
                     sbValue.Append(tConfig.tScanning.tFrontSide1.
                                                     sbFileName.ToString());
                     Mfs100.BuildFileNameFromTemplate( sbFrontSide1FileName,
                                                                    sbValue,
                                      tConfig.tScanning.tFrontSide1.eFormat,
                                                            1, sbErrorMsg );
                      
                  }
                  if (tConfig.tScanning.tFrontSide2.eFormat == 
                                              WD_MFS100DN.IMAGE_FORMAT.NONE)
                  {
                     sbFrontSide2FileName.Length = 0;
                  }
                  else
                  {
                     // Create file name of front side image 2 for document
                     // 1 
                     sbValue.Length = 0;
                     sbValue.Insert(0, tConfig.tScanning.sbImageDirectory.
                                                                ToString());
                     sbValue.Append("\\");
                     sbValue.Append(tConfig.tScanning.tFrontSide2.
                                                     sbFileName.ToString());
                     Mfs100.BuildFileNameFromTemplate( sbFrontSide2FileName,
                                                                    sbValue,
                                      tConfig.tScanning.tFrontSide2.eFormat,
                                                            1, sbErrorMsg );
                  }
                  if (tConfig.tScanning.tRearSide1.eFormat == 
                                              WD_MFS100DN.IMAGE_FORMAT.NONE)
                  {
                     sbRearSide1FileName.Length = 0;
                  }
                  else
                  {
                     // Create file name of rear side image 1 for document 1
                     sbValue.Length = 0;
                     sbValue.Insert(0, tConfig.tScanning.sbImageDirectory.
                                                                ToString());
                     sbValue.Append("\\");
                     sbValue.Append(tConfig.tScanning.tRearSide1.
                                                     sbFileName.ToString());
                     Mfs100.BuildFileNameFromTemplate( sbRearSide1FileName,
                                                                    sbValue,
                                       tConfig.tScanning.tRearSide1.eFormat,
                                                            1, sbErrorMsg );
                  }
                  if (tConfig.tScanning.tRearSide2.eFormat == 
                                              WD_MFS100DN.IMAGE_FORMAT.NONE)
                  {
                     sbRearSide2FileName.Length = 0;
                  }
                  else
                  {
                     // Create file name of rear side image 2 for document 1
                     sbValue.Length = 0;
                     sbValue.Insert(0, tConfig.tScanning.sbImageDirectory.
                                                                ToString());
                     sbValue.Append("\\");
                     sbValue.Append(tConfig.tScanning.tRearSide2.
                                                     sbFileName.ToString());
                     Mfs100.BuildFileNameFromTemplate( sbRearSide2FileName,
                                                                    sbValue,
                                       tConfig.tScanning.tRearSide2.eFormat,
                                                            1, sbErrorMsg );
                  }
                  // Display image            
                  if (sbFrontSide1FileName.Length > 0)
                  {
                     // Front side
                     structFileInfo = new System.IO.FileInfo(
                                           sbFrontSide1FileName.ToString());
                     iSide = 0;
                  }
                  else
                  {
                     // Rear side
                     structFileInfo = new System.IO.FileInfo(
                                            sbRearSide1FileName.ToString());
                     iSide = 2;
                  }
                  try
                  {
                     structFileStream = structFileInfo.OpenRead();
                  }
                  catch
                  {
                     bImagesAvailable = false;
                     break;
                  }
                  pbImage.Image = 
                          System.Drawing.Image.FromStream(structFileStream);
                  structFileStream.Close();
                  bImagesAvailable = true;
                  Application.DoEvents();
               }
               break;
            }
            case CFG.FEED_SOURCE.RED_ROSE_SINGLE:
            {
                //listBoxEventItemAdd( lbEvents,"Starting scanner (batch - SF)...");                
                ////iResult = Mfs100.ScanFeeder(iFeedMode, uiReadFont,
                ////                     tConfig.tFeeding.iTimeout, sbErrorMsg);
             
                ////ValidForm validScreen = new ValidForm(this);
                ////validScreen.btnValidate.Hide();
                
                //iResult = Mfs100.Scan(iFeedMode, uiReadFont, 
                //    tConfig.tFeeding.iTimeout, sbCodeline, sbErrorMsg);
                //listBoxEventItemAdd( lbEvents,"mfScan() = " + iResult.ToString());
                   

                //// Write codeline into file (append it)
                //try
                //{
                //    swFile = new System.IO.StreamWriter("codeline.txt",
                //                                                        true);
                //}
                //catch
                //{
                //    // Show error message         
                //    MessageBox.Show(
                //                    "Cannot open file codeline.txt for writing",
                //                    "Red Rose Scan-Solutions - MFS100",
                //                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //}
                //if (swFile != null)
                //{
                //    sbLine.Length = 0;
                //    sbLine.Insert(0, "Doc: 00001 = ");
                //    sbLine.Append(sbCodeline.ToString());
                //    swFile.WriteLine(sbLine.ToString());
                //    swFile.Close();
                //}
                //// If scanning was successful
                //if (iResult >= 0)
                //{
                //    string[] datas = null;

                //    if (tConfig.tScanning.tFrontSide1.eFormat ==
                //                                WD_MFS100DN.IMAGE_FORMAT.NONE)
                //    {
                //        sbFrontSide1FileName.Length = 0;
                //    }
                //    else
                //    {
                //        // Create file name of front side image 1 for document
                //        // 1 
                //        sbValue.Length = 0;
                //        sbValue.Insert(0, tConfig.tScanning.sbImageDirectory.
                //                                                    ToString());
                //        sbValue.Append("\\");
                //        sbValue.Append(tConfig.tScanning.tFrontSide1.
                //                                        sbFileName.ToString());
                //        Mfs100.BuildFileNameFromTemplate(sbFrontSide1FileName,
                //               sbValue, tConfig.tScanning.tFrontSide1.eFormat,
                //                                                1, sbErrorMsg);
                //        string imgfilename = sbFrontSide1FileName.ToString(); 

                //        bool barcodeSuccess = true;

                //        try { datas = BarcodeScanner.Scan(imgfilename, tConfig.tBarcode); }
                //        catch
                //        {
                //            barcodeSuccess = false;

                //        }
                //        try { string str = datas[0]; }
                //        catch
                //        {
                //            barcodeSuccess = false;

                //        }
                //        if (barcodeSuccess)
                //        {                           
                //            // Report barcode read result
                //            listBoxEventItemAdd( lbEvents,"  barcode = " 
                //                + BarcodeType.QRCode.ToString() + datas[0]);
                //            // Report read result
                //            listBoxEventItemAdd( lbEvents,"  sbCodeline = " + sbCodeline.ToString());                               
                //            //listBoxEventItemAdd( validScreen.lbEvents,"  barcode = " 
                //            //+ BarcodeType.QRCode.ToString() + datas[0]);                                

                //            // Report read result
                //            //listBoxEventItemAdd( validScreen.lbEvents,"  sbCodeline = " + 
                //            //  sbCodeline.ToString());
                               
                //            string strBarcode, bar1, bar2, bar3, bar4, bar5,
                //                strMICR, micr1, micr2, micr3, micr4, micr5;
                //            int cnt = 0;

                //            strBarcode = datas[0];
                //            strMICR = sbCodeline.ToString();
                //            if (strBarcode.Length > 22 && strMICR.Length > 23)
                //            {

                //                bar1 = strBarcode.Substring(3, 5);
                //                bar2 = strBarcode.Substring(8, 7);
                //                bar3 = strBarcode.Substring(15, 3);
                //                bar4 = strBarcode.Substring(18, 3);
                //                bar5 = strBarcode.Substring(22, 3);

                //                micr1 = strMICR.Substring(1, 5);
                //                micr2 = strMICR.Substring(7, 7);
                //                micr3 = strMICR.Substring(15, 3);
                //                micr4 = strMICR.Substring(19, 3);
                //                micr5 = strMICR.Substring(23, 3);

                //                if (bar1 == micr1) cnt++;
                //                if (bar2 == micr2) cnt++;
                //                if (bar3 == micr3) cnt++;
                //                if (bar4 == micr4) cnt++;
                //                if (bar5 == micr5) cnt++;
                //            }
                //            if (cnt > 3)
                //            {
                //               // listBoxEventItemAdd( validScreen.lbEvents,"\nValid paper voucher");
                //            }
                //            else
                //            {
                //                // listBoxEventItemAdd( validScreen.lbEvents,"InValid paper voucher");                                
                //                //listBoxEventItemAdd( validScreen.lbEvents,"\nWaiting for press Continue");
                //            }
                //        }
                //        else
                //        {
                //            //listBoxEventItemAdd( validScreen.lbEvents, "\nBarcode reading error\n\n");                                                           
                //        }
                //        // Show setup dialog
                //        validScreen.Show();
                //        // if setup was canceled                               
                //        // Validate configuration
                //    }

                //    // Display image            
                //    if (sbFrontSide1FileName.Length > 0)
                //    {
                //        // Front side
                //        structFileInfo = new System.IO.FileInfo(
                //                                sbFrontSide1FileName.ToString());
                //        iSide = 0;
                //    }
                //    else
                //    {
                //        // Rear side
                //        structFileInfo = new System.IO.FileInfo(
                //                                sbRearSide1FileName.ToString());
                //        iSide = 2;
                //    }
                //    try
                //    {
                //        structFileStream = structFileInfo.OpenRead();
                //    }
                //    catch
                //    {
                //        bImagesAvailable = false;
                //        break;
                //    }
                //    pbImage.Image =
                //            System.Drawing.Image.FromStream(structFileStream);
                //    structFileStream.Close();
                //    bImagesAvailable = true;
                //    Application.DoEvents();
                //}
               
                break;
            }
            case CFG.FEED_SOURCE.RED_ROSE_BATCH:
            {               
                listBoxEventItemAdd( lbEvents,"Starting scanner (batch - SF)...");               
                iOldDocId = 0;
                //barcode reading thread starts
                
                PUB_uiReadFont = uiReadFont;
                PUB_iFeedMode = iFeedMode;
                PUB_sbValue = sbValue;
                defaultSettingsForRedRoseBatch(ref tConfig);               
                StoreConfiguration(tConfig);
                Vendor goalV = new Vendor();
                goalV.id = "Voucher";
                goalV.shopName = "Reader";
                
                
                new ValidForm(this, goalV).Show();
                //this.Close();
                
                //Vendor selectedVendor = new Vendor();
                // Vendor Select Form is created
                //VendorSelectForm vsf = new VendorSelectForm(this);
                //vsf.Show();
                //selectedVendor = vsf.getSelectedVendor();
                // Temp valid form
                //new ValidForm(this, new Vendor()
                //{
                //    id = Guid.NewGuid().ToString(),
                //    fullName = "akjkajda",
                //    shopName = "3928kfsdm"
                //}).Show();
                //Fill file name if empty
                
                
                //this.Close();

                
                // Update configuration file
                //StoreConfiguration(tConfig);
                //iResult = Mfs100.ScanFeeder(iFeedMode, uiReadFont,
                //                   tConfig.tFeeding.iTimeout, sbErrorMsg);
                              
                //while (true)
                //{
                //    listBoxEventItemAdd( lbEvents,"mfScan() = " + iResult.ToString());                   
                //     //If scanning was successful
                //    if (iResult >= 0)
                //    {
                //        string[] datas = null;
                //        if (tConfig.tScanning.tFrontSide1.eFormat ==
                //                                    WD_MFS100DN.IMAGE_FORMAT.NONE)
                //        {
                //            sbFrontSide1FileName.Length = 0;
                //        }
                //        else
                //        {
                //            // Create file name of front side image 1 for document
                //            // 1 
                //            sbValue.Length = 0;
                //            sbValue.Insert(0, tConfig.tScanning.sbImageDirectory.
                //                                                       ToString());
                //            sbValue.Append("\\");
                //            sbValue.Append(tConfig.tScanning.tFrontSide1.
                //                                            sbFileName.ToString());
                //            Mfs100.BuildFileNameFromTemplate(sbFrontSide1FileName,
                //                                                           sbValue,
                //                             tConfig.tScanning.tFrontSide1.eFormat,
                //                                                   1, sbErrorMsg);
                //            string imgfilename = sbFrontSide1FileName.ToString();
                //            string imgFormat = imgfilename.Substring(imgfilename.Length - 4);
                //            imgfilename = imgfilename.Substring(0, imgfilename.Length - 8);
                //            SP.imgCnt++;
                //            imgfilename = imgfilename + SP.imgCnt.ToString("D" + 4) + imgFormat;

                //            //bool barcodeSuccess = true;

                //            //try { datas = barcodeRead1.getMyBarcode(); }
                //            //catch(Exception ex)
                //            //{
                //            //    barcodeSuccess = false;
                //            //}                           
                //            //if (barcodeSuccess)
                //            //{
                //                //listBoxEventItemAdd( validScreen.lbEvents,SP.imgCnt 
                //                //    + "-----------------------");                               
                //                //// Report barcode read result
                //                //listBoxEventItemAdd( validScreen.lbEvents,"  barcode = " 
                //                //    + BarcodeType.QRCode.ToString() + datas[0]);                        
                //                //int cnt = 0;
                //                //string barcodeValue = "";
                //                //helpers.barcodeCompare(datas[0], sbCodeline.ToString(),
                //                //    ref barcodeValue, ref cnt);

                //                //if (cnt > 3)
                //                //{
                //                //    listBoxEventItemAdd( validScreen.lbEvents,"\nValid paper voucher");
                                    
                //                //    Voucher myVocuher = new Voucher();

                //                //    bool usedVocuher = false;
                //                //    for (int i = 0; i < validScreen.voucherList.Count(); i++)
                //                //    {
                //                //        if (validScreen.voucherList[i].barcodeNumber == datas[0])
                //                //        { usedVocuher = true; SP.reusedCnt++; break; }
                //                //    }
                //                //    if (!usedVocuher)
                //                //    {
                //                //        myVocuher.barcodeNumber = datas[0];
                //                //        myVocuher.currency = "USD";
                //                //        myVocuher.value = int.Parse(barcodeValue);
                //                //        myVocuher.organization = "NGOs";
                //                //        SP.totalValue += myVocuher.value;
                //                //        validScreen.voucherList.Add(myVocuher);

                //                //        SP.validCnt++;
                //                //    }

                //                //    validScreen.lbStatistics.Update();
                //                //    // Use DataAdapter to fill DataTable   
                //                //    //Statistics screen update
                //                //    statisticsItemsUpdateAndShow(SP, validScreen);
                //                //    //value update
                //                //    validScreen.lblTotalValue.Items.Clear();
                //                //    validScreen.lblTotalValue.Items.Add(SP.totalValue);
                //                //    validScreen.lblTotalValue.Update();
                //                //}
                //                //else
                //                //{
                //                //    listBoxEventItemAdd( validScreen.lbEvents,"InValid paper voucher");
                //                //    listBoxEventItemAdd( validScreen.lbEvents,"\nWaiting for press Continue");                                   
                //                //    //Statistics screen update
                //                //    statisticsItemsUpdateAndShow(SP, validScreen);

                //                //    SP.invCnt++;
                //                //}
                //            //}
                //            //else
                //            //{
                //            //    listBoxEventItemAdd( validScreen.lbEvents, 
                //            //        SP.imgCnt + "-----------------------------------");
                //            //    listBoxEventItemAdd( validScreen.lbEvents, "\nBarcode reading error\n\n");  
                //            //    //Statistics screen update
                //            //    statisticsItemsUpdateAndShow(SP, validScreen);                               

                //            //    SP.invCnt++;
                //            //}
                //            // Show setup dialog
                //            //validScreen.Show();
                //            // if setup was canceled                               
                //            // Validate configuration
                //        }

                //        // Display image            
                //        if (sbFrontSide1FileName.Length > 0)
                //        {
                //            // Front side
                //            structFileInfo = new System.IO.FileInfo(sbFrontSide1FileName.ToString());
                //            iSide = 0;
                //        }
                //        else
                //        {
                //            // Rear side
                //            structFileInfo = new System.IO.FileInfo(sbRearSide1FileName.ToString());
                //            iSide = 2;
                //        }
                //        try
                //        {
                //            structFileStream = structFileInfo.OpenRead();
                //        }
                //        catch
                //        {
                //            bImagesAvailable = false;
                //            break;
                //        }
                //        pbImage.Image =
                //                System.Drawing.Image.FromStream(structFileStream);
                //        structFileStream.Close();
                //        bImagesAvailable = true;
                //        Application.DoEvents();
                //    }
                //    // If single feed failed
                //    if (iResult < 0)
                //    {
                //        // End loop
                //        break;
                //    }
                //}
                
                break;
            }
            default :
            {
               // Batch feed
               // If sorting shall not be done
               if (tConfig.tReading.eSortMode == CFG.READ_SORTMODE.NONE)
               {
                  // Without sorting, all reading fonts can be handled by 
                  // ScanFeeder()
                  bSortByScanAndEject = false;
               }
               else
               {
                  // Sorting shall be done
                  // Check if sorting have to be done by Scan() and Eject()
                  switch (tConfig.tReading.eFont)
                  {
                     case CFG.READ_FONT.NONE :
                     case CFG.READ_FONT.CMC7_MAGNETIC :
                     case CFG.READ_FONT.E13B_MAGNETIC :
                     {
                        // No reading font or pure magnetic fonts can be 
                        // sorted by ScanFeeder()
                        bSortByScanAndEject = false;
                        break;
                     }
                     case CFG.READ_FONT.CMC7_OPTICAL :
                     case CFG.READ_FONT.CMC7_AND_OCRA :
                     case CFG.READ_FONT.CMC7_AND_OCRB :
                     case CFG.READ_FONT.E13B_OPTICAL :
                     case CFG.READ_FONT.E13B_MAGNETIC_AND_OPTICAL :
                     case CFG.READ_FONT.OCRA :
                     case CFG.READ_FONT.OCRA_ALPHA :
                     case CFG.READ_FONT.OCRA_AND_OCRB :
                     case CFG.READ_FONT.OCRB :
                     case CFG.READ_FONT.OCRB_ALPHA :
                     case CFG.READ_FONT.OCRB_UK :
                     default :
                     {
                        // All reading fonts containing optical read results
                        // have to be sorted by Scan() and Eject()
                        bSortByScanAndEject = true;
                     }
                     break;
                  }
               }
               // If sorting can be done by ScanFeeder()
               if (bSortByScanAndEject == false)
               {   
                  lbEvents.Items.Add("Starting scanner (batch - SF)...");
                  lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
                  lbEvents.SelectedIndex = -1;
                  lbEvents.Update();
                  iOldDocId = 0;
                  iResult = Mfs100.ScanFeeder(iFeedMode, uiReadFont,
                                     tConfig.tFeeding.iTimeout, sbErrorMsg);
                  lbEvents.Items.Add("mfScanFeeder() = " + iResult.ToString());
                  lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
                  lbEvents.SelectedIndex = -1;
                  lbEvents.Update();
                  // If batch scanning was started successfully
                  if (iResult >= 0)
                  {
                     // Prevent recursive start of batch scanning until 
                     // callback OnAllDone occured
                     btnStart.Enabled = false;
                  }
               }
               else
               {
                  // Sorting have to be done by Scan() and Eject()
                  lbEvents.Items.Add("Starting scanner (batch - SE)...");
                  lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
                  lbEvents.SelectedIndex = -1;
                  lbEvents.Update();
                  // Remove sort by MICR (which prevents stopping before 
                  // good pocket)
                  iFeedMode &= ~((int) OPTIONS.A6_SORT_BY_MICR);
                  // Set stopping before good pocket
                  iFeedMode |= (int) OPTIONS.A6_FASTSTOP_MODE;
                  // Process loop (break inside loop)
                  iCounter = 1;
                  while (true)
                  {
                     iResult = Mfs100.Scan(iFeedMode, uiReadFont,
                                      tConfig.tFeeding.iTimeout, sbCodeline,
                                                                sbErrorMsg);
                     lbEvents.Items.Add("mfScan() = " + iResult.ToString());
                     lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
                     lbEvents.SelectedIndex = -1;
                     lbEvents.Update();
                     // If single feed failed
                     if (iResult < 0)
                     {
                        // End loop
                        break;
                     }
                     // Report read result
                     lbEvents.Items.Add("  iDocId = " + iCounter.
                                                                ToString());
                     lbEvents.Items.Add("  sbCodeline = " + 
                                                     sbCodeline.ToString());
                     lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
                     lbEvents.SelectedIndex = -1;
                     lbEvents.Update();
                     // Write codeline into file (append it)
                     try
                     {
                        swFile = new System.IO.StreamWriter( "codeline.txt",
                                                                     true );
                     }
                     catch
                     {
                        // Show error message         
                        MessageBox.Show( 
                                "Cannot open file codeline.txt for writing",
                                "Walther Data GmbH Scan-Solutions - MFS100",
                                 MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        break;
                     }
                     if (swFile != null)
                     {
                        sbLine.Length = 0;
                        sbLine.Insert(0, "Doc: " + iCounter.ToString("D5") +
                                                                     " = ");
                        sbLine.Append(sbCodeline.ToString());
                        swFile.WriteLine(sbLine.ToString());
                        swFile.Close();
                     }
                     // Check if 2 codelines
                     iPosOfSeperator = sbCodeline.ToString().IndexOf('\x17');
                     // If 2 codelines found
                     if (iPosOfSeperator >= 0)
                     {
                        str1stCodeline = sbCodeline.ToString().Substring(0, 
                                                           iPosOfSeperator);
                        str2ndCodeline = sbCodeline.ToString().Substring(
                                                       iPosOfSeperator + 1);
                        // If 1st string (magnetic) is empty
                        if (str1stCodeline.Length <= 0)
                        {
                           // Only 2nd string (optical) counts
                           // Search errors in 2nd string (optical)
                           iIndex1 = str2ndCodeline.IndexOf('?');
                           iIndex2 = iIndex1;
                        }
                        else
                        {
                           // Search errors in 1st string (magnetic)
                           iIndex1 = str1stCodeline.IndexOf('?');
                           // Search errors in 2nd string (optical)
                           iIndex2 = str2ndCodeline.IndexOf('?');
                        }
                        // Only if both read results contain errors, 
                        // document has to be rejected
                        if ((iIndex1 >= 0) && (iIndex2 >= 0))
                        {
                           bErrorsFound = true;
                        }
                        else
                        {
                           bErrorsFound = false;
                        }
                     }
                     else
                     {
                        str1stCodeline = sbCodeline.ToString();
                        str2ndCodeline = "";
                        // Search errors in 1st string (magnetic or optical)
                        iIndex1 = str1stCodeline.IndexOf('?');
                        // If errors were found
                        if (iIndex1 >= 0)
                        {
                           bErrorsFound = true;
                        }
                        else
                        {
                           // No errors were found
                           bErrorsFound = false;
                        }
                     }
                     // Depending on sort mode
                     switch (tConfig.tReading.eSortMode)
                     {
                        case CFG.READ_SORTMODE.SORT_PATTERN_POSITIVE :
                        case CFG.READ_SORTMODE.SORT_PATTERN_NEGATIVE :
                        {
                           // Sort depending on regular expressions
                           bMatchFound = reRegularExpression.IsMatch(
                                                     sbCodeline.ToString());
                           // If sort pattern is negative or inversion is 
                           // forced
                           if ((tConfig.tReading.eSortMode == CFG.
                                     READ_SORTMODE.SORT_PATTERN_NEGATIVE) ||
                                                     (bInvertMatch == true))
                           {
                              // Invert match indicator
                              bMatchFound = !bMatchFound;
                           }
                           // If a match was found
                           if (bMatchFound == true)
                           {
                              // Sort to good pocket
                              ePocket = EJECT_POCKET.
                                        FORWARD_WITH_WAIT_FOR_END_OF_OPERATION;
                              // Show sort decision
                              lbEvents.Items.Add("Sort (RegEx):");
                              lbEvents.Items.Add( "  1 (accept pocket)");
                              lbEvents.SelectedIndex = 
                                                      lbEvents.Items.Count - 1;
                              lbEvents.SelectedIndex = -1;
                              lbEvents.Update();
                           }
                           else
                           {
                              // Sort to reject pocket
                              ePocket = EJECT_POCKET.
                                       BACKWARD_WITH_WAIT_FOR_END_OF_OPERATION;
                              // Show sort decision
                              lbEvents.Items.Add("Sort (RegEx):");
                              lbEvents.Items.Add( "  0 (reject pocket)");
                              lbEvents.SelectedIndex = 
                                                      lbEvents.Items.Count - 1;
                              lbEvents.SelectedIndex = -1;
                              lbEvents.Update();
                           }
                           break;
                        }
                        case CFG.READ_SORTMODE.MICR_GOOD_BAD :
                        {
                           // Sort good/bad
                           // If errors were not found
                           if (bErrorsFound == false)
                           {
                              // Sort to good pocket
                              ePocket = EJECT_POCKET.
                                     FORWARD_WITH_WAIT_FOR_END_OF_OPERATION;
                              // Show sort decision
                              lbEvents.Items.Add("Sort (Good/Bad):");
                              lbEvents.Items.Add( "  1 (accept pocket)");
                              lbEvents.SelectedIndex = 
                                                   lbEvents.Items.Count - 1;
                              lbEvents.SelectedIndex = -1;
                              lbEvents.Update();
                           }
                           else
                           {
                              // Errors were found
                              // Sort to reject pocket
                              ePocket = EJECT_POCKET.
                                    BACKWARD_WITH_WAIT_FOR_END_OF_OPERATION;
                              // Show sort decision
                              lbEvents.Items.Add("Sort (Good/Bad):");
                              lbEvents.Items.Add( "  0 (reject pocket)");
                              lbEvents.SelectedIndex = 
                                                   lbEvents.Items.Count - 1;
                              lbEvents.SelectedIndex = -1;
                              lbEvents.Update();
                           }
                           break;
                        }
                        default :
                        {
                           // CFG.READ_SORTMODE.NONE
                           // Always sort into good pocket
                           ePocket = EJECT_POCKET.
                                     FORWARD_WITH_WAIT_FOR_END_OF_OPERATION;
                           // Show sort decision
                           lbEvents.Items.Add("Sort (NONE):");
                           lbEvents.Items.Add( "  1 (fixed by MFS100)");
                           lbEvents.SelectedIndex = 
                                                   lbEvents.Items.Count - 1;
                           lbEvents.SelectedIndex = -1;
                           lbEvents.Update();
                           break;
                        }               
                     }
                     iResult = Mfs100.Eject(EJECT_MODE.NORMAL, ePocket, 0, 
                                                                sbErrorMsg);
                     lbEvents.Items.Add("mfEject() = " + 
                                                        iResult.ToString());
                     lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
                     lbEvents.SelectedIndex = -1;
                     lbEvents.Update();
                     // If eject failed
                     if (iResult < 0)
                     {
                        // End loop
                        break;
                     }
                     // Create file names
                     if (tConfig.tScanning.tFrontSide1.eFormat == 
                                              WD_MFS100DN.IMAGE_FORMAT.NONE)
                     {
                        sbFrontSide1FileName.Length = 0;
                     }
                     else
                     {
                        // Create file name of front side image 1
                        sbValue.Length = 0;
                        sbValue.Insert(0, tConfig.tScanning.
                                               sbImageDirectory.ToString());
                        sbValue.Append("\\");
                        sbValue.Append(tConfig.tScanning.tFrontSide1.
                                                     sbFileName.ToString());
                        Mfs100.BuildFileNameFromTemplate( 
                                              sbFrontSide1FileName, sbValue,
                                      tConfig.tScanning.tFrontSide1.eFormat,
                                                     iCounter, sbErrorMsg );
                     }
                     if (tConfig.tScanning.tFrontSide2.eFormat == 
                                              WD_MFS100DN.IMAGE_FORMAT.NONE)
                     {
                        sbFrontSide2FileName.Length = 0;
                     }
                     else
                     {
                        // Create file name of front side image 2
                        sbValue.Length = 0;
                        sbValue.Insert(0, tConfig.tScanning.
                                               sbImageDirectory.ToString());
                        sbValue.Append("\\");
                        sbValue.Append(tConfig.tScanning.tFrontSide2.
                                                     sbFileName.ToString());
                        Mfs100.BuildFileNameFromTemplate( 
                                              sbFrontSide2FileName, sbValue,
                                      tConfig.tScanning.tFrontSide2.eFormat,
                                                     iCounter, sbErrorMsg );
                     }
                     if (tConfig.tScanning.tRearSide1.eFormat == 
                                              WD_MFS100DN.IMAGE_FORMAT.NONE)
                     {
                        sbRearSide1FileName.Length = 0;
                     }
                     else
                     {
                        // Create file name of rear side image 1
                        sbValue.Length = 0;
                        sbValue.Insert(0, tConfig.tScanning.
                                               sbImageDirectory.ToString());
                        sbValue.Append("\\");
                        sbValue.Append(tConfig.tScanning.tRearSide1.
                                                     sbFileName.ToString());
                        Mfs100.BuildFileNameFromTemplate( 
                                               sbRearSide1FileName, sbValue,
                                       tConfig.tScanning.tRearSide1.eFormat,
                                                     iCounter, sbErrorMsg );
                     }
                     if (tConfig.tScanning.tRearSide2.eFormat == 
                                              WD_MFS100DN.IMAGE_FORMAT.NONE)
                     {
                        sbRearSide2FileName.Length = 0;
                     }
                     else
                     {
                        // Create file name of rear side image 2
                        sbValue.Length = 0;
                        sbValue.Insert(0, tConfig.tScanning.
                                               sbImageDirectory.ToString());
                        sbValue.Append("\\");
                        sbValue.Append(tConfig.tScanning.tRearSide2.
                                                     sbFileName.ToString());
                        Mfs100.BuildFileNameFromTemplate( 
                                               sbRearSide2FileName, sbValue,
                                       tConfig.tScanning.tRearSide2.eFormat,
                                                     iCounter, sbErrorMsg );
                     }
                     lbEvents.Items.Add("  sbFrontSide1FileName = " + 
                                           sbFrontSide1FileName.ToString());
                     lbEvents.Items.Add("  sbFrontSide2FileName = " + 
                                           sbFrontSide2FileName.ToString());
                     lbEvents.Items.Add("  sbRearSide1FileName = " + 
                                            sbRearSide1FileName.ToString());
                     lbEvents.Items.Add("  sbRearSide2FileName = " + 
                                            sbRearSide2FileName.ToString());
                     lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
                     lbEvents.SelectedIndex = -1;
                     lbEvents.Update();
                     // Display image            
                     if (sbFrontSide1FileName.Length > 0)
                     {
                        // Front side
                        structFileInfo = new System.IO.FileInfo(
                                           sbFrontSide1FileName.ToString());
                        iSide = 0;
                     }
                     else
                     {
                        // Rear side
                        structFileInfo = new System.IO.FileInfo(
                                            sbRearSide1FileName.ToString());
                        iSide = 2;
                     }
                     try
                     {
                        structFileStream = structFileInfo.OpenRead();
                     }
                     catch
                     {
                        bImagesAvailable = false;
                        break;
                     }
                     pbImage.Image = 
                          System.Drawing.Image.FromStream(structFileStream);
                     structFileStream.Close();
                     bImagesAvailable = true;
                     Application.DoEvents();
                     // Increase counter
                     ++iCounter;
                  }
                  // If feeder was empty
                  if (iResult == -18)
                  {
                     // Simulate good result
                     iResult = 1;
                  }
               }
               break;
            }
         }
         // If no error occured
         if (iResult >= 0)
         {
            lbEvents.Items.Add("Waiting for document...");
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
         }
      }

      private void btnInfo_Click(object sender, EventArgs e)
      {
         // Get software status of machine and report it
         GetStatus();
      }

      private void btnSetup_Click(object sender, EventArgs e)
      {
         DialogResult  drResult;
         bool  bValid;
         
         // Create setup class
         SetupForm frmSetup = new SetupForm(tConfig);
         // Do until configuration is valid
         do
         {
            // Show setup dialog
            drResult = frmSetup.ShowDialog();
            // if setup was canceled
            if (drResult != DialogResult.OK)
            {
               // Do not validate and store configuration
               return;
            }
            // Validate configuration
            bValid = ValidateConfiguration(tConfig);
         } while (bValid != true);
         // Update configuration file
         StoreConfiguration(tConfig);
      }

      private void btnEjectForward_Click(object sender, EventArgs e)
      {
         // Eject document forward and report it
         if ((tConfig.tFeeding.eSource == CFG.FEED_SOURCE.A4_SINGLE) ||
                     (tConfig.tFeeding.eSource == CFG.FEED_SOURCE.A4_BATCH))
         {
            // A4
            lbEvents.Items.Add("Ejecting document forward A4...");
            lbEvents.Update();
            iResult = Mfs100.Eject(EJECT_MODE.FORCED, 
                                    EJECT_POCKET.FORWARD_A4, 0, sbErrorMsg);
         }
         else
         {
            // A6
            lbEvents.Items.Add("Ejecting document forward A6...");
            lbEvents.Update();
            iResult = Mfs100.Eject(EJECT_MODE.FORCED, EJECT_POCKET.FORWARD,
                                                             0, sbErrorMsg);
         }
         lbEvents.Items.Add("mfEject() = " + iResult.ToString());
         lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
         lbEvents.SelectedIndex = -1;
         lbEvents.Update();
         // If ejection was successful
         if (iResult >= 0)
         {
            lbEvents.Items.Add("Document ejected");
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
         }
      }

      private void btnEjectReverse_Click(object sender, EventArgs e)
      {
         // Eject document reverse and report it
         if ((tConfig.tFeeding.eSource == CFG.FEED_SOURCE.A4_SINGLE) ||
                     (tConfig.tFeeding.eSource == CFG.FEED_SOURCE.A4_BATCH))
         {
            // A4 -> Show error message         
            MessageBox.Show("Ejecting reverse A4 is not implemented\r\n" +
                                         "Please eject forward A4 instead ",
                                "Walther Data GmbH Scan-Solutions - MFS100",
                                 MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return;
         }
         // A6
         lbEvents.Items.Add("Ejecting document reverse A6...");
         lbEvents.Update();
         iResult = Mfs100.Eject(EJECT_MODE.FORCED, EJECT_POCKET.BACKWARD, 0,
                                                                sbErrorMsg);
         lbEvents.Items.Add("mfEject() = " + iResult.ToString());
         lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
         lbEvents.SelectedIndex = -1;
         lbEvents.Update();
         // If ejection was successful
         if (iResult >= 0)
         {
            lbEvents.Items.Add("Document ejected");
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
         }
      }

      private void btnExit2_Click(object sender, EventArgs e)
      {
         // Deinitialize machine before closing
         iResult = Mfs100.Close(sbErrorMsg);
         // Terminate application
         this.Close();
      }

      public void DelegateOnRawFront()
      {
         byte[]  byValue = new byte[2];
         int  iDocId = 0;
         int  iImageHeight = 0;
         int  iImageWidth = 0;
         int  iCounter;
         BITS_PER_PIXEL  eBitsPerPixel = BITS_PER_PIXEL.GRAYSCALE_256;
         System.IO.MemoryStream  bufHeaderBuffer = 
                                               new System.IO.MemoryStream();
         System.IO.MemoryStream  bufImageBuffer = 
                                               new System.IO.MemoryStream();
         StringBuilder  sbLine = new StringBuilder();
         System.IO.FileStream  fsFile = null;

         // Get parameters
         if (Mfs100.GetOnRawImageParameters(RAW_IMAGE_TYPE.IMAGE_FRONTSIDE,
                                              ref iDocId, ref eBitsPerPixel,
                                          ref iImageHeight, ref iImageWidth,
                                        bufImageBuffer, sbErrorMsg) == true)
         {
            // Report callback call
            lbEvents.Items.Add("DelegateOnRawFront:");
            lbEvents.Items.Add("  DocId=" + iDocId.ToString());
            lbEvents.Items.Add("  BitsPerPixel=" + 
                                                  eBitsPerPixel.ToString());
            lbEvents.Items.Add("  ImageHeight=" + iImageHeight.ToString());
            lbEvents.Items.Add("  ImageWidth=" + iImageWidth.ToString());
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
            // Create file for image data
            try
            {
               fsFile = new System.IO.FileStream("Images\\MSRF" + 
                                             iDocId.ToString("D4") + ".tif",
                                                 System.IO.FileMode.Create);
            }
            catch
            {
               // Show error message         
               lbEvents.Items.Add("FileStream:");
               lbEvents.Items.Add(
                               "  Cannot open image data file for writing");
               lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
               lbEvents.SelectedIndex = -1;
               lbEvents.Update();
            }
            // If file could be created
            if (fsFile != null)
            {
               // Create BMP header from image parameters
               iResult = Mfs100.CreateBmpHeader(eBitsPerPixel, iImageHeight,
                                               iImageWidth, bufHeaderBuffer,
                                                                sbErrorMsg);
               // If BMP header could be created
               if (iResult > 0)
               {
                  // Write BMP header into file
                  bufHeaderBuffer.Position = 0;
                  iCounter = 0;
                  while (iCounter < bufHeaderBuffer.Length)
                  {
                     bufHeaderBuffer.Read(byValue, 0, 1);
                     fsFile.WriteByte(byValue[0]);
                     ++iCounter;
                  }
               }
               // Write Image data into file
               bufImageBuffer.Position = 0;
               iCounter = 0;
               while (iCounter < bufImageBuffer.Length)
               {
                  bufImageBuffer.Read(byValue, 0, 1);
                  fsFile.WriteByte(byValue[0]);
                  ++iCounter;
               }
               // Close file
               fsFile.Close();
            }
         }
         else
         {
            // Report callback call without available parameters
            lbEvents.Items.Add("DelegateOnRawFront:");
            lbEvents.Items.Add("  No parameters available");
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
         }
      }
      
      public void DelegateOnRawRear()
      {
         byte[]  byValue = new byte[2];
         int  iDocId = 0;
         int  iImageHeight = 0;
         int  iImageWidth = 0;
         int  iCounter;
         BITS_PER_PIXEL  eBitsPerPixel = BITS_PER_PIXEL.GRAYSCALE_256;
         System.IO.MemoryStream  bufHeaderBuffer = 
                                               new System.IO.MemoryStream();
         System.IO.MemoryStream  bufImageBuffer = 
                                               new System.IO.MemoryStream();
         StringBuilder  sbLine = new StringBuilder();
         System.IO.FileStream  fsFile = null;

         // Get parameters
         if (Mfs100.GetOnRawImageParameters(RAW_IMAGE_TYPE.IMAGE_REARSIDE,
                                              ref iDocId, ref eBitsPerPixel,
                                          ref iImageHeight, ref iImageWidth,
                                        bufImageBuffer, sbErrorMsg) == true)
         {
            // Report callback call
            lbEvents.Items.Add("DelegateOnRawRear:");
            lbEvents.Items.Add("  DocId=" + iDocId.ToString());
            lbEvents.Items.Add("  BitsPerPixel=" + 
                                                  eBitsPerPixel.ToString());
            lbEvents.Items.Add("  ImageHeight=" + iImageHeight.ToString());
            lbEvents.Items.Add("  ImageWidth=" + iImageWidth.ToString());
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
            // Create file for image data
            try
            {
               fsFile = new System.IO.FileStream("Images\\MSRR" + 
                                             iDocId.ToString("D4") + ".tif",
                                                 System.IO.FileMode.Create);
            }
            catch
            {
               // Show error message         
               lbEvents.Items.Add("FileStream:");
               lbEvents.Items.Add(
                               "  Cannot open image data file for writing");
               lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
               lbEvents.SelectedIndex = -1;
               lbEvents.Update();
            }
            // If file could be created
            if (fsFile != null)
            {
               // Create BMP header from image parameters
               iResult = Mfs100.CreateBmpHeader(eBitsPerPixel, iImageHeight,
                                               iImageWidth, bufHeaderBuffer,
                                                                sbErrorMsg);
               // If BMP header could be created
               if (iResult > 0)
               {
                  // Write BMP header into file
                  bufHeaderBuffer.Position = 0;
                  iCounter = 0;
                  while (iCounter < bufHeaderBuffer.Length)
                  {
                     bufHeaderBuffer.Read(byValue, 0, 1);
                     fsFile.WriteByte(byValue[0]);
                     ++iCounter;
                  }
               }
               // Write Image data into file
               bufImageBuffer.Position = 0;
               iCounter = 0;
               while (iCounter < bufImageBuffer.Length)
               {
                  bufImageBuffer.Read(byValue, 0, 1);
                  fsFile.WriteByte(byValue[0]);
                  ++iCounter;
               }
               // Close file
               fsFile.Close();
            }
         }
         else
         {
            // Report callback call without available parameters
            lbEvents.Items.Add("DelegateOnRawRear:");
            lbEvents.Items.Add("  No parameters available");
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
         }
      }

      public void DelegateOnCodeline()
      {
         int  iCode = 0;
         int  iDocId = 0;
         int  iIndex;
         StringBuilder  sbCodeline = new StringBuilder();
         StringBuilder  sbLine = new StringBuilder();
         System.IO.StreamWriter  swFile = null;
         
         // Get parameters
         if (Mfs100.GetOnCodelineParameters(ref iCode, ref iDocId, 
                                            sbCodeline, sbErrorMsg) == true)
         {
            // Report callback call
            lbEvents.Items.Add("DelegateOnCodeline:");
            lbEvents.Items.Add("  iCode = " + iCode.ToString());
            lbEvents.Items.Add("  iDocId = " + iDocId.ToString());
            lbEvents.Items.Add("  sbCodeline = " + sbCodeline.ToString());
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
            // Depending on sort mode
            switch (tConfig.tReading.eSortMode)
            {
               case CFG.READ_SORTMODE.SORT_PATTERN_POSITIVE :
               case CFG.READ_SORTMODE.SORT_PATTERN_NEGATIVE :
               {
                  // Sort depending on regular expressions
                  bMatchFound = reRegularExpression.IsMatch(sbCodeline.
                                                                ToString());
                  // If sort pattern is negative or inversion is forced
                  if ((tConfig.tReading.eSortMode == CFG.READ_SORTMODE.
                                                   SORT_PATTERN_NEGATIVE) ||
                                                     (bInvertMatch == true))
                  {
                     // Invert match indicator
                     bMatchFound = !bMatchFound;
                  }
                  // If a match was found
                  if (bMatchFound == true)
                  {
                     // Sort to good pocket
                     iResult = Mfs100.Sort(SORT_POCKET.ACCEPT_POCKET, 
                                                                sbErrorMsg);
                     // Show sort decision
                     lbEvents.Items.Add("Sort (RegEx):");
                     lbEvents.Items.Add( "  1 (accept pocket)");
                     lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
                     lbEvents.SelectedIndex = -1;
                     lbEvents.Update();
                  }
                  else
                  {
                     // Sort to reject pocket
                     iResult = Mfs100.Sort(SORT_POCKET.REJECT_POCKET, 
                                                                sbErrorMsg);
                     // Show sort decision
                     lbEvents.Items.Add("Sort (RegEx):");
                     lbEvents.Items.Add( "  0 (reject pocket)");
                     lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
                     lbEvents.SelectedIndex = -1;
                     lbEvents.Update();
                  }
                  break;
               }
               case CFG.READ_SORTMODE.MICR_GOOD_BAD :
               {
                  // Sort good/bad
                  // Search for errors
                  iIndex = sbCodeline.ToString().IndexOf('?');
                  // If errors were not found
                  if (iIndex < 0)
                  {
                     // Sort to good pocket
                     iResult = Mfs100.Sort(SORT_POCKET.ACCEPT_POCKET, 
                                                                sbErrorMsg);
                     // Show sort decision
                     lbEvents.Items.Add("Sort (Good/Bad):");
                     lbEvents.Items.Add( "  1 (accept pocket)");
                     lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
                     lbEvents.SelectedIndex = -1;
                     lbEvents.Update();
                  }
                  else
                  {
                     // Errors were found
                     // Sort to reject pocket
                     iResult = Mfs100.Sort(SORT_POCKET.REJECT_POCKET, 
                                                                sbErrorMsg);
                     // Show sort decision
                     lbEvents.Items.Add("Sort (Good/Bad):");
                     lbEvents.Items.Add( "  0 (reject pocket)");
                     lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
                     lbEvents.SelectedIndex = -1;
                     lbEvents.Update();
                  }
                  break;
               }
               default :
               {
                  // CFG.READ_SORTMODE.NONE
                  // Always sort into good pocket
                  // Show sort decision
                  lbEvents.Items.Add("Sort (NONE):");
                  lbEvents.Items.Add( "  1 (fixed by MFS100)");
                  lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
                  lbEvents.SelectedIndex = -1;
                  lbEvents.Update();
                  break;
               }               
            }
            // Write codeline into file (append it)
            try
            {
               swFile = new System.IO.StreamWriter( "codeline.txt", true );
            }
            catch
            {
               // Show error message         
               lbEvents.Items.Add("StreamWriter:");
               lbEvents.Items.Add(
                             "  Cannot open file codeline.txt for writing");
               lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
               lbEvents.SelectedIndex = -1;
               lbEvents.Update();
            }
            if (swFile != null)
            {
               sbLine.Insert(0, "Doc: " + iDocId.ToString("D5") + " = ");
               sbLine.Append(sbCodeline.ToString());
               swFile.WriteLine(sbLine.ToString());
               swFile.Close();
            }
         }
         else
         {
            // Report callback call without available parameters
            lbEvents.Items.Add("DelegateOnCodeline:");
            lbEvents.Items.Add("  No parameters available");
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
         }
      }

      public void DelegateOnDocumentDone()
      {
         int  iDocId = 0;
         int  iReadMode = 0;
         StringBuilder  sbCodeline = new StringBuilder();
         StringBuilder  sbFrontImage1FileName = new StringBuilder();
         StringBuilder  sbFrontImage2FileName = new StringBuilder();
         StringBuilder  sbRearImage1FileName = new StringBuilder();
         StringBuilder  sbRearImage2FileName = new StringBuilder();
         System.IO.FileInfo  structFileInfo;
         System.IO.FileStream  structFileStream;
         
         // Get parameters
         if (Mfs100.GetOnDocumentDoneParameters(ref iDocId, sbCodeline, 
                               sbFrontImage1FileName, sbFrontImage2FileName,
                                 sbRearImage1FileName, sbRearImage2FileName,
                                         ref iReadMode, sbErrorMsg) == true)
         {
            // Report callback call
            lbEvents.Items.Add("DelegateOnDocumentDone:");
            lbEvents.Items.Add("  iDocId = " + iDocId.ToString());
            lbEvents.Items.Add("  sbCodeline = " + sbCodeline.ToString());
            lbEvents.Items.Add("  sbFrontImage1FileName = " + 
                                          sbFrontImage1FileName.ToString());
            lbEvents.Items.Add("  sbFrontImage2FileName = " + 
                                          sbFrontImage2FileName.ToString());
            lbEvents.Items.Add("  sbRearImage1FileName = " + 
                                           sbRearImage1FileName.ToString());
            lbEvents.Items.Add("  sbRearImage2FileName = " + 
                                           sbRearImage2FileName.ToString());
            lbEvents.Items.Add("  iReadMode = " + iReadMode.ToString());
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
            sbFrontSide1FileName.Length = 0;
            sbFrontSide1FileName.Insert(0, 
                                          sbFrontImage1FileName.ToString());
            sbFrontSide2FileName.Length = 0;
            sbFrontSide2FileName.Insert(0, 
                                          sbFrontImage2FileName.ToString());
            sbRearSide1FileName.Length = 0;
            sbRearSide1FileName.Insert(0, sbRearImage1FileName.ToString());
            sbRearSide2FileName.Length = 0;
            sbRearSide2FileName.Insert(0, sbRearImage2FileName.ToString());
            // Display image            
            if (sbFrontImage1FileName.Length > 0)
            {
               // Front side
               structFileInfo = new System.IO.FileInfo(
                                          sbFrontImage1FileName.ToString());
               iSide = 0;
            }
            else
            {
               if (sbRearImage1FileName.Length > 0)
               {
                  // Rear side
                  structFileInfo = new System.IO.FileInfo(
                                           sbRearImage1FileName.ToString());
                  iSide = 2;
               }
               else
               {
                  // No image
                  structFileInfo = null;
                  iSide = 0;
                  bImagesAvailable = false;
                  return;
               }
            }
            try
            {
               structFileStream = structFileInfo.OpenRead();
            }
            catch
            {
               bImagesAvailable = false;
               return;
            }
            // Because of "pbImage.SizeMode = PictureBoxSizeMode.Zoom;" in
            // definition of picturebox, scaling will be done automatically
            pbImage.Image = 
                          System.Drawing.Image.FromStream(structFileStream);
            structFileStream.Close();
            bImagesAvailable = true;
            Application.DoEvents();
         }
         else
         {
            // Report callback call without available parameters
            lbEvents.Items.Add("DelegateOnDocumentDone:");
            lbEvents.Items.Add("  No parameters available");
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
         }
      }

      public void DelegateOnWarning()
      {
         int  iDocId = 0;
         int  iWarningCode = 0;
         StringBuilder  sbWarningDescription = new StringBuilder();
         
         // Get parameters
         if (Mfs100.GetOnWarningOrErrorParameters(false, ref iDocId, 
                                     ref iWarningCode, sbWarningDescription,
                                                        sbErrorMsg) == true)
         {
            // Report callback call
            lbEvents.Items.Add("DelegateOnWarning:");
            lbEvents.Items.Add("  iDocId = " + iDocId.ToString());
            lbEvents.Items.Add("  iWarningCode = " + 
                                                   iWarningCode.ToString());
            lbEvents.Items.Add("  sbWarningDescription = " + 
                                           sbWarningDescription.ToString());
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
         }
         else
         {
            // Report callback call without available parameters
            lbEvents.Items.Add("DelegateOnWarning:");
            lbEvents.Items.Add("  No parameters available");
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
         }
      }

      public void DelegateOnError()
      {
         int  iDocId = 0;
         int  iErrorCode = 0;
         StringBuilder  sbErrorDescription = new StringBuilder();
         
         // Get parameters
         if (Mfs100.GetOnWarningOrErrorParameters(true, ref iDocId, 
                                         ref iErrorCode, sbErrorDescription,
                                                        sbErrorMsg) == true)
         {
            // Report callback call
            lbEvents.Items.Add("DelegateOnError:");
            lbEvents.Items.Add("  iDocId = " + iDocId.ToString());
            lbEvents.Items.Add("  iErrorCode = " + iErrorCode.ToString());
            lbEvents.Items.Add("  sbErrorDescription = " + 
                                             sbErrorDescription.ToString());
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
         }
         else
         {
            // Report callback call without available parameters
            lbEvents.Items.Add("DelegateOnError:");
            lbEvents.Items.Add("  No parameters available");
            lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
            lbEvents.SelectedIndex = -1;
            lbEvents.Update();
         }
      }

      public void DelegateOnAllDone()
      {
         int  iNrOfDocumentsScanned = 0;

         // Get parameters
         Mfs100.GetOnAllDoneParameters(ref iNrOfDocumentsScanned, 
                                                                sbErrorMsg);
         // Report callback call
         lbEvents.Items.Add("DelegateOnAllDone:");
         lbEvents.Items.Add("  iNrOfDocumentsScanned = " + 
                                          iNrOfDocumentsScanned.ToString());
         lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
         lbEvents.SelectedIndex = -1;
         lbEvents.Update();
         // Allow start of batch scanning again since last batch scanning 
         // was now finished
         btnStart.Enabled = true;
      }

      private void SetParameter(CFG tConfig)
      {
         bool  bError;
         int  iCounter;
         int  iIndex;
         int  iResult;
         String  strErrorMsg;
         StringBuilder  sbValue = new StringBuilder();
         
         // Copy structure content into parameter list (convert values if
         // necessary)
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.iMaxImageWidth.ToString());
         ParameterListInsert(MFS_PARAMETER_ID.SCAN_LENGTH, sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.iMaxImageHeight.ToString());
         ParameterListInsert(MFS_PARAMETER_ID.END_YPOS, sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tFeeding.iTimeout.ToString());
         ParameterListInsert(MFS_PARAMETER_ID.FEEDER_TIMEOUT, sbValue);
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_DIRECTORY, 
                                        tConfig.tScanning.sbImageDirectory);
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_FRONTSIDE_FILENAME, 
                                  tConfig.tScanning.tFrontSide1.sbFileName);
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_REARSIDE_FILENAME, 
                                   tConfig.tScanning.tRearSide1.sbFileName);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                       ImageDpi2String(tConfig.tScanning.tFrontSide1.eDpi));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_FRONTSIDE_XRESOLUTION,
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                        ImageDpi2String(tConfig.tScanning.tRearSide1.eDpi));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_REARSIDE_XRESOLUTION, 
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, EndoMode2String(tConfig.tEndorsing.eMode));
         ParameterListInsert(MFS_PARAMETER_ID.INKJET_MODE, sbValue);
         if (tConfig.tEndorsing.eMode == CFG.ENDO_MODE.BITMAP)
         {
            // sbFont contains bitmap file name which must be set into 
            // parameter INKJET_STRING, a font file makes no sense in 
            // parameter INKJET_FONT if bitmap mode
            ParameterListInsert(MFS_PARAMETER_ID.INKJET_STRING, 
                                                 tConfig.tEndorsing.sbFont);
            sbValue.Length = 0;
            ParameterListInsert(MFS_PARAMETER_ID.INKJET_FONT, sbValue);
         }
         else
         {
            // sbData -> INKJET_STRING, sbFont -> INKJET_FONT
            ParameterListInsert(MFS_PARAMETER_ID.INKJET_STRING, 
                                                 tConfig.tEndorsing.sbData);
            ParameterListInsert(MFS_PARAMETER_ID.INKJET_FONT, 
                                                 tConfig.tEndorsing.sbFont);
         }
         sbValue.Length = 0;
         sbValue.Insert(0, 
                         ReadBlankMode2String(tConfig.tReading.eOcrBlanks));
         ParameterListInsert(MFS_PARAMETER_ID.OCR_BLANK_MODE, sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                 ImageFormat2String(tConfig.tScanning.tFrontSide1.eFormat));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_FRONTSIDE_TYPE,
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                  ImageFormat2String(tConfig.tScanning.tRearSide1.eFormat));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_REARSIDE_TYPE, sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, Bool2String(tConfig.tScanning.tFrontSide1.bCut));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_FRONTSIDE_FLAGS,
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, Bool2String(tConfig.tScanning.tRearSide1.bCut));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_REARSIDE_FLAGS,
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                     tConfig.tScanning.tFrontSide1.iJpegQuality.ToString());
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_FRONTSIDE_JPEGQUALITY, 
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                      tConfig.tScanning.tRearSide1.iJpegQuality.ToString());
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_REARSIDE_JPEGQUALITY, 
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                     ImageColor2String(tConfig.tScanning.tFrontSide1.eColor, 
                                      tConfig.tScanning.tRearSide1.eColor));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_COLORMODE, sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tEndorsing.iPosition.ToString());
         ParameterListInsert(MFS_PARAMETER_ID.INKJET_POSITION_IN_STEPS, 
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, EndoDensity2String(tConfig.tEndorsing.eDensity));
         ParameterListInsert(MFS_PARAMETER_ID.INKJET_NARROWPRINT, sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                        ReadBlankMode2String(tConfig.tReading.eMicrBlanks));
         ParameterListInsert(MFS_PARAMETER_ID.MICR_BLANKS, sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tEndorsing.iStartCounter.ToString());
         ParameterListInsert(MFS_PARAMETER_ID.INKJET_COUNTER_STARTVALUE, 
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tEndorsing.iSteps.ToString());
         ParameterListInsert(MFS_PARAMETER_ID.INKJET_COUNTER_INCREMENT, 
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                          tConfig.tFeeding.iDoublefeedThreshold.ToString());
         ParameterListInsert(MFS_PARAMETER_ID.DOUBLEFEED_THRESHOLD, 
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.iTempPeriod.ToString());
         ParameterListInsert(
                 MFS_PARAMETER_ID.TEMPERATURE_MINUTES_BETWEEN_RECALIBRATION, 
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.iTempTolerance.ToString());
         ParameterListInsert(MFS_PARAMETER_ID.TEMPERATURE_TOLERANCE, 
                                                                   sbValue);
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_FRONTSIDE2_FILENAME, 
                                  tConfig.tScanning.tFrontSide2.sbFileName);
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_REARSIDE2_FILENAME, 
                                   tConfig.tScanning.tRearSide2.sbFileName);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                       ImageDpi2String(tConfig.tScanning.tFrontSide2.eDpi));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_FRONTSIDE2_XRESOLUTION,
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                        ImageDpi2String(tConfig.tScanning.tRearSide2.eDpi));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_REARSIDE2_XRESOLUTION, 
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                 ImageFormat2String(tConfig.tScanning.tFrontSide2.eFormat));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_FRONTSIDE2_TYPE,
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                  ImageFormat2String(tConfig.tScanning.tRearSide2.eFormat));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_REARSIDE2_TYPE, sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, Bool2String(tConfig.tScanning.tFrontSide2.bCut));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_FRONTSIDE2_FLAGS,
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, Bool2String(tConfig.tScanning.tRearSide2.bCut));
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_REARSIDE2_FLAGS,
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                     tConfig.tScanning.tFrontSide2.iJpegQuality.ToString());
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_FRONTSIDE2_JPEGQUALITY, 
                                                                   sbValue);
         sbValue.Length = 0;
         sbValue.Insert(0, 
                      tConfig.tScanning.tRearSide2.iJpegQuality.ToString());
         ParameterListInsert(MFS_PARAMETER_ID.IMAGE_REARSIDE2_JPEGQUALITY, 
                                                                   sbValue);
         // For all settings in parameter list
         bError = false;
         strErrorMsg = "";
         lbEvents.Items.Add("Setting parameters...");
         lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
         lbEvents.SelectedIndex = -1;
         lbEvents.Update();
         iCounter = 0;
         while (iCounter < tConfig.tMfsParameters.ciIndexes.Length)
         {
            // If parameter shall be set into machine
            if (tConfig.tMfsParameters.sbParameters[iCounter] != null)
            {
               // Get parameter index
               iIndex = (int) 
                        tConfig.tMfsParameters.ciIndexes.GetValue(iCounter);
               // Get parameter value
               sbValue.Length = 0;
               sbValue.Insert(0, 
                  tConfig.tMfsParameters.sbParameters[iCounter].ToString());
               // Set parameter in machine and report it
               iResult = Mfs100.SetParameter((MFS_PARAMETER_ID) iIndex, 
                                                      sbValue, sbErrorMsg );
               lbEvents.Items.Add("mfSetParam(0x" + iIndex.ToString("X2") + 
                                      ") = " + sbValue.ToString() + " -> " + 
                                                        iResult.ToString());
               lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
               lbEvents.SelectedIndex = -1;
               lbEvents.Update();
               // If an error occur
               if (iResult < 0)
               {
                  // If previos error exist
                  if (bError == true)
                  {
                     // Add seperator to string
                     strErrorMsg += " +\r\n";
                  }
                  else
                  {
                     // Set flag
                     bError = true;
                  }
                  // Add error message to string
                  strErrorMsg += "Parameter 0x" + iIndex.ToString("X2") + 
                                              " could not be set in MFS100";
               }
            }
            ++iCounter;
         }
         // If an error occur
         if (bError == true)
         {
            // Show error message         
            MessageBox.Show(strErrorMsg, 
                                "Walther Data GmbH Scan-Solutions - MFS100",
                                 MessageBoxButtons.OK, MessageBoxIcon.Stop);
         }
      }

      private void GetStatus()
      {
         // Get software status of machine and report it
         lbEvents.Items.Add("Getting sensor status...");
         lbEvents.Update();
         iResult = Mfs100.GetStatusSW(tStatusSW, sbErrorMsg);
         lbEvents.Items.Add("mfGetStatus(0) = " + iResult.ToString());
         lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
         lbEvents.SelectedIndex = -1;
         lbEvents.Update();
         // If software status could be read from machine
         if (iResult >= 0)
         {
            if (tStatusSW.tSensors.bFeeder == false)
            {
               pbFeeder.BackColor = System.Drawing.Color.Lime;
            }
            else
            {
               pbFeeder.BackColor = System.Drawing.Color.Red;
            }
            pbFeeder.Update();
            if (tStatusSW.tSensors.bStartDocument == false)
            {
               pbStart.BackColor = System.Drawing.Color.Lime;
            }
            else
            {
               pbStart.BackColor = System.Drawing.Color.Red;
            }
            pbStart.Update();
            if (tStatusSW.tSensors.bCameraIn == false)
            {
               pbCamera.BackColor = System.Drawing.Color.Lime;
            }
            else
            {
               pbCamera.BackColor = System.Drawing.Color.Red;
            }
            pbCamera.Update();
            if (tStatusSW.tSensors.bCameraOut == false)
            {
               pbExit.BackColor = System.Drawing.Color.Lime;
            }
            else
            {
               pbExit.BackColor = System.Drawing.Color.Red;
            }
            pbExit.Update();
            if (tStatusSW.tSensors.bDoubleFeed == false)
            {
               pbDoublefeed.BackColor = System.Drawing.Color.Lime;
            }
            else
            {
               pbDoublefeed.BackColor = System.Drawing.Color.Red;
            }
            pbDoublefeed.Update();
            if (tStatusSW.tSensors.bStraightTrack == false)
            {
               pbStraight.BackColor = System.Drawing.Color.Lime;
            }
            else
            {
               pbStraight.BackColor = System.Drawing.Color.Red;
            }
            pbStraight.Update();
            if (tStatusSW.tSensors.bA4Feeder == false)
            {
               pbA4Feeder.BackColor = System.Drawing.Color.Lime;
            }
            else
            {
               pbA4Feeder.BackColor = System.Drawing.Color.Red;
            }
            pbA4Feeder.Update();
            if (tStatusSW.tSensors.bA4StartDocument == false)
            {
               pbA4Start.BackColor = System.Drawing.Color.Lime;
            }
            else
            {
               pbA4Start.BackColor = System.Drawing.Color.Red;
            }
            pbA4Start.Update();
            if (tStatusSW.tSensors.bA4DoubleFeed == false)
            {
               pbA4Doublefeed.BackColor = System.Drawing.Color.Lime;
            }
            else
            {
               pbA4Doublefeed.BackColor = System.Drawing.Color.Red;
            }
            pbA4Doublefeed.Update();
         }
         // Inquiry machine and report it
         lbEvents.Items.Add("Inquiry MFS100...");
         lbEvents.Update();
         iResult = Mfs100.GetStatusInquiry(tStatusInquiry, sbErrorMsg);
         lbEvents.Items.Add("mfGetStatus(1) = " + iResult.ToString());
         lbEvents.SelectedIndex = lbEvents.Items.Count - 1;
         lbEvents.SelectedIndex = -1;
         lbEvents.Update();
         // If inquiry could be read from machine
         if (iResult >= 0)
         {
            tbStatus.Text = tStatusInquiry.sbName.ToString();
            tbStatus.Update();
         }
         else
         {
            tbStatus.Text = "Unable to query MFS100";
         }
      }

      private void ReadConfiguration(CFG tConfig)
      {
         int  iCounter;
         int  iIndex;
         int  iValue;
         int  iDefault;
         StringBuilder  sbSection = new StringBuilder();
         StringBuilder  sbEntry = new StringBuilder();
         StringBuilder  sbDefault = new StringBuilder();
         StringBuilder  sbValue = new StringBuilder();
      
         // Read feeding configuration from file
         sbSection.Length = 0;
         sbSection.Insert(0, "Feeding");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Source");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.FEED_SOURCE.RED_ROSE_BATCH.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tFeeding.eSource = GetFeedSourceValue(sbValue, 
                                           CFG.FEED_SOURCE.RED_ROSE_SINGLE);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Pipeline");
         sbDefault.Length = 0;
         sbDefault.Insert(0, true.ToString());   // Enabled
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tFeeding.bPipelineEnabled = GetBooleanValue(sbValue, true);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Timeout");
         iDefault = 5000;   // default (ms)
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tFeeding.iTimeout = iValue;
         sbEntry.Length = 0;
         sbEntry.Insert(0, "DoublefeedThreshold");
         iDefault = 120;   // default
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tFeeding.iDoublefeedThreshold = iValue;
         // Read reading configuration from file
         sbSection.Length = 0;
         sbSection.Insert(0, "Reading");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Font");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.READ_FONT.NONE.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tReading.eFont = GetReadFontValue(sbValue, 
                                                        CFG.READ_FONT.NONE);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "MicrBlanks");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.READ_BLANKMODE.NONE.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tReading.eMicrBlanks = GetReadBlankmodeValue(sbValue, 
                                                 CFG.READ_BLANKMODE.NONE);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "OcrBlanks");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.READ_BLANKMODE.NORMAL.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tReading.eOcrBlanks = GetReadBlankmodeValue(sbValue, 
                                                 CFG.READ_BLANKMODE.NONE);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "SortMode");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.READ_SORTMODE.NONE.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tReading.eSortMode = GetReadSortmodeValue(sbValue, 
                                                    CFG.READ_SORTMODE.NONE);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "SortPattern");
         sbDefault.Length = 0;
         sbDefault.Insert(0, "");
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tReading.sbSortPattern.Length = 0;
         tConfig.tReading.sbSortPattern.Insert(0, sbValue.ToString());
         // Read endorsing configuration from file
         sbSection.Length = 0;
         sbSection.Insert(0, "Endorsing");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Mode");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.ENDO_MODE.NONE.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tEndorsing.eMode = GetEndoModeValue(sbValue, 
                                                        CFG.ENDO_MODE.NONE);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "StartCounter");
         iDefault = 1;
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tEndorsing.iStartCounter = iValue;
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Steps");
         iDefault = 1;   // default
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tEndorsing.iSteps = iValue;
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Font");
         sbDefault.Length = 0;
         sbDefault.Insert(0, "");   // internal standard font
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tEndorsing.sbFont.Length = 0;
         tConfig.tEndorsing.sbFont.Insert(0, sbValue.ToString());
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Position");
         iDefault = 100;   // default
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tEndorsing.iPosition = iValue;
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Density");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.ENDO_DENSITY.BOLD.ToString());   // default
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tEndorsing.eDensity = GetEndoDensityValue(sbValue, 
                                                     CFG.ENDO_DENSITY.BOLD);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Data");
         sbDefault.Length = 0;
         sbDefault.Insert(0, "WALTHER MFS100");
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tEndorsing.sbData.Length = 0;
         tConfig.tEndorsing.sbData.Insert(0, sbValue.ToString());
         // Read scanning configuration from file
         sbSection.Length = 0;
         sbSection.Insert(0, "Scanning");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "MaxImageWidth");
         iDefault = 250;   // default (mm)
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.iMaxImageWidth = iValue;
         sbEntry.Length = 0;
         sbEntry.Insert(0, "MaxImageHeight");
         iDefault = 420;   // default (pixel)
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.iMaxImageHeight = iValue;
         sbEntry.Length = 0;
         sbEntry.Insert(0, "TemperatureTolerance");
         iDefault = 0;   // off
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.iTempTolerance = iValue;
         sbEntry.Length = 0;
         sbEntry.Insert(0, "PeriodBetweenTemperatureCalibrations");
         iDefault = 2;   // default (minutes)
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.iTempPeriod = iValue;
         sbEntry.Length = 0;
         sbEntry.Insert(0, "TrueColor");
         sbDefault.Length = 0;
         sbDefault.Insert(0, false.ToString());   // off
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.bTrueColor = GetBooleanValue(sbValue, false);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "UseCalibration");
         sbDefault.Length = 0;
         sbDefault.Insert(0, true.ToString());   // on
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.bUseCalibration = GetBooleanValue(sbValue, 
                                                                     false);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "ImageDirectory");
         sbDefault.Length = 0;
         sbDefault.Insert(0, "C:\\RedRose\\Images");
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.sbImageDirectory.Length = 0;
         tConfig.tScanning.sbImageDirectory.Insert(0, sbValue.ToString());
         // Read scanning - front side 1 configuration from file
         sbSection.Length = 0;
         sbSection.Insert(0, "FrontSideImage1");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "FileName");
         sbDefault.Length = 0;
         sbDefault.Insert(0, "FS1-%04d");
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tFrontSide1.sbFileName.Length = 0;
         tConfig.tScanning.tFrontSide1.sbFileName.Insert(0, 
                                                        sbValue.ToString());
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Format");
         sbDefault.Length = 0;
         sbDefault.Insert(0, WD_MFS100DN.IMAGE_FORMAT.JPEG.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tFrontSide1.eFormat = GetImageFormatValue(
                                    sbValue, WD_MFS100DN.IMAGE_FORMAT.JPEG);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "JpegQuality");
         iDefault = 99;   // default
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tFrontSide1.iJpegQuality = iValue;
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Color");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.IMAGE_COLOR.RED.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tFrontSide1.eColor = GetImageColorValue(sbValue,
                                                       CFG.IMAGE_COLOR.RED);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Dpi");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.IMAGE_DPI._200.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tFrontSide1.eDpi = GetImageDpiValue(sbValue, 
                                                        CFG.IMAGE_DPI._200);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "CutDocument");
         sbDefault.Length = 0;
         sbDefault.Insert(0, true.ToString());   // on
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tFrontSide1.bCut = GetBooleanValue(sbValue, 
                                                                      true);
         // Read scanning - front side 2 configuration from file
         sbSection.Length = 0;
         sbSection.Insert(0, "FrontSideImage2");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "FileName");
         sbDefault.Length = 0;
         sbDefault.Insert(0, "FS2-%04d");
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tFrontSide2.sbFileName.Length = 0;
         tConfig.tScanning.tFrontSide2.sbFileName.Insert(0, 
                                                        sbValue.ToString());
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Format");
         sbDefault.Length = 0;
         sbDefault.Insert(0, WD_MFS100DN.IMAGE_FORMAT.NONE.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tFrontSide2.eFormat = GetImageFormatValue(
                                    sbValue, WD_MFS100DN.IMAGE_FORMAT.NONE);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "JpegQuality");
         iDefault = 50;   // default
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tFrontSide2.iJpegQuality = iValue;
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Color");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.IMAGE_COLOR.RED.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tFrontSide2.eColor = GetImageColorValue(sbValue,
                                                       CFG.IMAGE_COLOR.RED);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Dpi");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.IMAGE_DPI._200.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tFrontSide2.eDpi = GetImageDpiValue(sbValue, 
                                                        CFG.IMAGE_DPI._200);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "CutDocument");
         sbDefault.Length = 0;
         sbDefault.Insert(0, true.ToString());   // on
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tFrontSide2.bCut = GetBooleanValue(sbValue, 
                                                                      true);
         // Read scanning - rear side 1 configuration from file
         sbSection.Length = 0;
         sbSection.Insert(0, "RearSideImage1");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "FileName");
         sbDefault.Length = 0;
         sbDefault.Insert(0, "RS1-%04d");
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tRearSide1.sbFileName.Length = 0;
         tConfig.tScanning.tRearSide1.sbFileName.Insert(0, 
                                                        sbValue.ToString());
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Format");
         sbDefault.Length = 0;
         sbDefault.Insert(0, WD_MFS100DN.IMAGE_FORMAT.NONE.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tRearSide1.eFormat = GetImageFormatValue(sbValue, 
                                             WD_MFS100DN.IMAGE_FORMAT.NONE);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "JpegQuality");
         iDefault = 50;   // default
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tRearSide1.iJpegQuality = iValue;
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Color");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.IMAGE_COLOR.RED.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tRearSide1.eColor = GetImageColorValue(sbValue, 
                                                       CFG.IMAGE_COLOR.RED);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Dpi");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.IMAGE_DPI._200.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tRearSide1.eDpi = GetImageDpiValue(sbValue, 
                                                        CFG.IMAGE_DPI._200);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "CutDocument");
         sbDefault.Length = 0;
         sbDefault.Insert(0, true.ToString());   // on
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tRearSide1.bCut = GetBooleanValue(sbValue, true);
         // Read scanning - rear side 2 configuration from file
         sbSection.Length = 0;
         sbSection.Insert(0, "RearSideImage2");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "FileName");
         sbDefault.Length = 0;
         sbDefault.Insert(0, "RS2-%04d");
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tRearSide2.sbFileName.Length = 0;
         tConfig.tScanning.tRearSide2.sbFileName.Insert(0, 
                                                        sbValue.ToString());
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Format");
         sbDefault.Length = 0;
         sbDefault.Insert(0, WD_MFS100DN.IMAGE_FORMAT.NONE.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tRearSide2.eFormat = GetImageFormatValue(sbValue, 
                                             WD_MFS100DN.IMAGE_FORMAT.NONE);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "JpegQuality");
         iDefault = 50;   // default
         iValue = Mfs100.GetPrivProfileInt(sbSection, sbEntry, iDefault, 
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tRearSide2.iJpegQuality = iValue;
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Color");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.IMAGE_COLOR.RED.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tRearSide2.eColor = GetImageColorValue(sbValue, 
                                                       CFG.IMAGE_COLOR.RED);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Dpi");
         sbDefault.Length = 0;
         sbDefault.Insert(0, CFG.IMAGE_DPI._200.ToString());
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tRearSide2.eDpi = GetImageDpiValue(sbValue, 
                                                        CFG.IMAGE_DPI._200);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "CutDocument");
         sbDefault.Length = 0;
         sbDefault.Insert(0, true.ToString());   // on
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);
         tConfig.tScanning.tRearSide2.bCut = GetBooleanValue(sbValue, true);
         // Read other parameter values from configuration file
         sbSection.Length = 0;
         sbSection.Insert(0, "OtherParameters");

         ////
         sbSection.Length = 0;
         sbSection.Insert(0, "VoucherType");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "MICRReading");
         sbDefault.Length = 0;
         sbDefault.Insert(0, true.ToString());   // off
         Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
                                              sbConfigFileName, sbErrorMsg);     
        


         //sbEntry.Length = 0;
         //sbEntry.Insert(0, "tBarcode");
         //sbDefault.Length = 0;
         //sbDefault.Insert(0, tConfig.tBarcode.ToString());
         //Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault, sbValue,
         //                                     sbConfigFileName, sbErrorMsg);
         iCounter = 0;
         while (iCounter < tConfig.tMfsParameters.ciIndexes.Length)
         {
            iIndex = (int) 
                        tConfig.tMfsParameters.ciIndexes.GetValue(iCounter);
            sbEntry.Length = 0;
            sbEntry.Insert(0, iIndex.ToString("X3"));
            sbDefault.Length = 0;
            Mfs100.GetPrivProfileString(sbSection, sbEntry, sbDefault,
                                     sbValue, sbConfigFileName, sbErrorMsg);
            if (sbValue.Length > 0)
            {
               // If object not exists
               if (tConfig.tMfsParameters.sbParameters[iCounter] == null)
               {
                  // Create new object
                  tConfig.tMfsParameters.sbParameters[iCounter] =
                                                        new StringBuilder();
               }
               // Copy given value into array 
               tConfig.tMfsParameters.sbParameters[iCounter].Length = 0;
               tConfig.tMfsParameters.sbParameters[iCounter].Insert(0, 
                                                                   sbValue);
            }
            ++iCounter;
         }
      }

      private bool ValidateConfiguration(CFG tConfig)
      {
         bool  bInvalid;
         String  strErrorMsg;
         
         // Set flag as valid
         bInvalid = false;
         strErrorMsg = "";
         // Check feeding configuration
         if (ValidateFeedSourceValue(tConfig.tFeeding.eSource) != true)
         {
            // Set flag as invalid
            bInvalid = true;
            // Add error message to string
            strErrorMsg += "Feeding - Source is invalid";
         }
         // Remark: If feed source is A4 or straight track, reading is not 
         //         possible (no read head in track)
         switch (tConfig.tFeeding.eSource)
         {
            case CFG.FEED_SOURCE.A6_STRAIGHT_SINGLE :
            case CFG.FEED_SOURCE.A4_SINGLE :
            case CFG.FEED_SOURCE.A4_BATCH :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.NONE;
               tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;
               break;
            }
         }
         if ((tConfig.tFeeding.iTimeout < 0) || 
                                        (tConfig.tFeeding.iTimeout > 20000))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Feeding - Timeout is invalid";
         }
         if ((tConfig.tFeeding.iDoublefeedThreshold < 0) || 
                             (tConfig.tFeeding.iDoublefeedThreshold > 9999))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Feeding - Doublefeed threshold is invalid";
         }
         if (ValidateReadFontValue(tConfig.tReading.eFont) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Reading - Font is invalid";
         }
         if (ValidateReadBlankmodeValue(tConfig.tReading.eMicrBlanks) != 
                                                                       true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Reading - MICR blanks is invalid";
         }
         if (ValidateReadBlankmodeValue(tConfig.tReading.eOcrBlanks) !=
                                                                       true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Reading - OCR blanks is invalid";
         }
         if (ValidateReadSortmodeValue(tConfig.tReading.eSortMode) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Reading - Sorting is invalid";
         }
         if (ValidateEndoModeValue(tConfig.tEndorsing.eMode) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Endorsing - Mode is invalid";
         }
         if ((tConfig.tEndorsing.iStartCounter < 0) || 
                                 (tConfig.tEndorsing.iStartCounter > 99999))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Endorsing - Start counter is invalid";
         }
         if ((tConfig.tEndorsing.iSteps < 0) || 
                                          (tConfig.tEndorsing.iSteps > 100))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Endorsing - Step(s) is invalid";
         }
         if ((tConfig.tEndorsing.iPosition < 0) || 
                                     (tConfig.tEndorsing.iPosition > 10000))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Endorsing - Position is invalid";
         }
         if (ValidateEndoDensityValue(tConfig.tEndorsing.eDensity) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Endorsing - Density is invalid";
         }
         if (ValidateEndoDataValue(tConfig.tEndorsing.sbData) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Endorsing - Data is invalid";
         }
         if ((tConfig.tScanning.iMaxImageWidth < 1) || 
                                 (tConfig.tScanning.iMaxImageWidth > 10000))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Maximum image width is invalid";
         }
         if ((tConfig.tScanning.iMaxImageHeight < 1) || 
                                 (tConfig.tScanning.iMaxImageHeight > 1000))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Maximum image height is invalid";
         }
         if ((tConfig.tScanning.iTempTolerance < 0) || 
                                   (tConfig.tScanning.iTempTolerance > 100))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Temperature tolerance is invalid";
         }
         if ((tConfig.tScanning.iTempPeriod < 0) || 
                                       (tConfig.tScanning.iTempPeriod > 60))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Period between temperature calibra" +
                                                         "tions is invalid";
         }
         if (ValidateImageDirectoryValue(
                                tConfig.tScanning.sbImageDirectory) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Image directory is invalid";
         }
         if (ValidateImageFileNameValue(
                          tConfig.tScanning.tFrontSide1.sbFileName) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Front side image 1 - File name is " +
                                                                  "invalid";
         }
         if (ValidateImageFormatValue(
                             tConfig.tScanning.tFrontSide1.eFormat) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Front side image 1 - Format is " +
                                                                  "invalid";
         }
         if ((tConfig.tScanning.tFrontSide1.iJpegQuality < 1) || 
                          (tConfig.tScanning.tFrontSide1.iJpegQuality > 99))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Front side image 1 - JPEQ quality " +
                                                               "is invalid";
         }
         if (ValidateImageColorValue(
                              tConfig.tScanning.tFrontSide1.eColor) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Front side image 1 - Color is " +
                                                                  "invalid";
         }
         if (ValidateImageDpiValue(tConfig.tScanning.tFrontSide1.eDpi) != 
                                                                       true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Front side image 1 - DPI is invalid";
         }
         if (ValidateImageFileNameValue(
                          tConfig.tScanning.tFrontSide2.sbFileName) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Front side image 2 - File name is " +
                                                                  "invalid";
         }
         if (ValidateImageFormatValue(
                             tConfig.tScanning.tFrontSide2.eFormat) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Front side image 2 - Format is " +
                                                                  "invalid";
         }
         if ((tConfig.tScanning.tFrontSide2.iJpegQuality < 1) || 
                          (tConfig.tScanning.tFrontSide2.iJpegQuality > 99))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Front side image 2 - JPEQ quality " +
                                                               "is invalid";
         }
         // Remark: Front side image 2 - Color must be the same value as 
         //         Front side image 1 - Color
         tConfig.tScanning.tFrontSide2.eColor = 
                                       tConfig.tScanning.tFrontSide1.eColor;
         // Remark: Front side image 2 - DPI must be 100 DPI if Front side
         //         image 1 -DPI is set to 100 DPI
         if (tConfig.tScanning.tFrontSide1.eDpi == CFG.IMAGE_DPI._100)
         {
            tConfig.tScanning.tFrontSide2.eDpi = CFG.IMAGE_DPI._100;
         }
         else
         {
            if (ValidateImageDpiValue(tConfig.tScanning.tFrontSide2.eDpi) != 
                                                                       true)
            {
               // If flas is already set as invalid
               if (bInvalid == true)
               {
                  // Add seperator to string
                  strErrorMsg += " +\r\n";
               }
               else
               {
                  // Set flag as invalid
                  bInvalid = true;
               }
               // Add error message to string
               strErrorMsg += "Scanning - Front side image 2 - DPI is " + 
                                                                  "invalid";
            }
         }
         // Remark: Front side image 2 - Cut threshold must be the same 
         //         value as Front side image 1 - Cut threshold
         tConfig.tScanning.tFrontSide2.bCut = 
                                         tConfig.tScanning.tFrontSide1.bCut;
         if (ValidateImageFileNameValue(
                           tConfig.tScanning.tRearSide1.sbFileName) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Rear side image 1 - File name is " +
                                                                  "invalid";
         }
         if (ValidateImageFormatValue(
                              tConfig.tScanning.tRearSide1.eFormat) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Rear side image 1 - Format is " +
                                                                  "invalid";
         }
         if ((tConfig.tScanning.tRearSide1.iJpegQuality < 1) || 
                           (tConfig.tScanning.tRearSide1.iJpegQuality > 99))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Rear side image 1 - JPEQ quality " +
                                                               "is invalid";
         }
         if (ValidateImageColorValue(
                               tConfig.tScanning.tRearSide1.eColor) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Rear side image 1 - Color is " +
                                                                  "invalid";
         }
         if (ValidateImageDpiValue(tConfig.tScanning.tRearSide1.eDpi) != 
                                                                       true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Rear side image 1 - DPI is invalid";
         }
         if (ValidateImageFileNameValue(
                           tConfig.tScanning.tRearSide2.sbFileName) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Rear side image 2 - File name is " +
                                                                  "invalid";
         }
         if (ValidateImageFormatValue(
                              tConfig.tScanning.tRearSide2.eFormat) != true)
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Rear side image 2 - Format is " +
                                                                  "invalid";
         }
         if ((tConfig.tScanning.tRearSide2.iJpegQuality < 1) || 
                           (tConfig.tScanning.tRearSide2.iJpegQuality > 99))
         {
            // If flas is already set as invalid
            if (bInvalid == true)
            {
               // Add seperator to string
               strErrorMsg += " +\r\n";
            }
            else
            {
               // Set flag as invalid
               bInvalid = true;
            }
            // Add error message to string
            strErrorMsg += "Scanning - Rear side image 2 - JPEQ quality " +
                                                               "is invalid";
         }
         // Remark: Rear side image 2 - Color must be the same value as Rear
         //         side image 1 - Color
         tConfig.tScanning.tRearSide2.eColor = 
                                        tConfig.tScanning.tRearSide1.eColor;
         // Remark: Rear side image 2 - DPI must be 100 DPI if Rear side
         //         image 1 -DPI is set to 100 DPI
         if (tConfig.tScanning.tRearSide1.eDpi == CFG.IMAGE_DPI._100)
         {
            tConfig.tScanning.tRearSide2.eDpi = CFG.IMAGE_DPI._100;
         }
         else
         {
            if (ValidateImageDpiValue(tConfig.tScanning.tRearSide2.eDpi) != 
                                                                       true)
            {
               // If flas is already set as invalid
               if (bInvalid == true)
               {
                  // Add seperator to string
                  strErrorMsg += " +\r\n";
               }
               else
               {
                  // Set flag as invalid
                  bInvalid = true;
               }
               // Add error message to string
               strErrorMsg += "Scanning - Rear side image 2 - DPI is " + 
                                                                  "invalid";
            }
         }
         // Remark: Rear side image 2 - Cut threshold must be the same value
         //         as Rear side image 1 - Cut threshold
         tConfig.tScanning.tRearSide2.bCut = 
                                          tConfig.tScanning.tRearSide1.bCut;
         // In configuration contain invalidations
         if (bInvalid == true)
         {
            // Show error message         
            MessageBox.Show(strErrorMsg, 
                                "Walther Data GmbH Scan-Solutions - MFS100",
                                 MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return(false);
         }
         return(true);
      }

      public void StoreConfiguration(CFG tConfig)
      {
         StringBuilder  sbSection = new StringBuilder();
         StringBuilder  sbEntry = new StringBuilder();
         StringBuilder  sbValue = new StringBuilder();
      
         // Store feeding configuration to file
         sbSection.Length = 0;
         sbSection.Insert(0, "Feeding");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Source");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tFeeding.eSource.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Pipeline");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tFeeding.bPipelineEnabled.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Timeout");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tFeeding.iTimeout.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "DoublefeedThreshold");
         sbValue.Length = 0;
         sbValue.Insert(0, 
                          tConfig.tFeeding.iDoublefeedThreshold.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         // Store reading configuration to file
         sbSection.Length = 0;
         sbSection.Insert(0, "Reading");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Font");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tReading.eFont.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "MicrBlanks");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tReading.eMicrBlanks.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "OcrBlanks");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tReading.eOcrBlanks.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "SortMode");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tReading.eSortMode.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "SortPattern");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tReading.sbSortPattern.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         // Store endorsing configuration to file
         sbSection.Length = 0;
         sbSection.Insert(0, "Endorsing");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Mode");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tEndorsing.eMode.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "StartCounter");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tEndorsing.iStartCounter.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Steps");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tEndorsing.iSteps.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Font");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tEndorsing.sbFont.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Position");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tEndorsing.iPosition.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Density");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tEndorsing.eDensity.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Data");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tEndorsing.sbData.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         // Store scanning configuration to file
         sbSection.Length = 0;
         sbSection.Insert(0, "Scanning");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "MaxImageWidth");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.iMaxImageWidth.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "MaxImageHeight");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.iMaxImageHeight.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "TemperatureTolerance");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.iTempTolerance.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "PeriodBetweenTemperatureCalibrations");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.iTempPeriod.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "TrueColor");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.bTrueColor.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "UseCalibration");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.bUseCalibration.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "ImageDirectory");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.sbImageDirectory.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         // Store scanning - front side 1 configuration to file
         sbSection.Length = 0;
         sbSection.Insert(0, "FrontSideImage1");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "FileName");
         sbValue.Length = 0;
         sbValue.Insert(0, 
                       tConfig.tScanning.tFrontSide1.sbFileName.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Format");
         sbValue.Length = 0;
         sbValue.Insert(0, 
                          tConfig.tScanning.tFrontSide1.eFormat.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "JpegQuality");
         sbValue.Length = 0;
         sbValue.Insert(0, 
                     tConfig.tScanning.tFrontSide1.iJpegQuality.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Color");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tFrontSide1.eColor.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Dpi");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tFrontSide1.eDpi.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "CutDocument");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tFrontSide1.bCut.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         // Store scanning - front side 2 configuration to file
         sbSection.Length = 0;
         sbSection.Insert(0, "FrontSideImage2");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "FileName");
         sbValue.Length = 0;
         sbValue.Insert(0, 
                       tConfig.tScanning.tFrontSide2.sbFileName.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Format");
         sbValue.Length = 0;
         sbValue.Insert(0, 
                          tConfig.tScanning.tFrontSide2.eFormat.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "JpegQuality");
         sbValue.Length = 0;
         sbValue.Insert(0, 
                     tConfig.tScanning.tFrontSide2.iJpegQuality.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Color");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tFrontSide2.eColor.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Dpi");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tFrontSide2.eDpi.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "CutDocument");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tFrontSide2.bCut.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         // Store scanning - rear side 1 configuration to file
         sbSection.Length = 0;
         sbSection.Insert(0, "RearSideImage1");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "FileName");
         sbValue.Length = 0;
         sbValue.Insert(0, 
                        tConfig.tScanning.tRearSide1.sbFileName.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Format");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tRearSide1.eFormat.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "JpegQuality");
         sbValue.Length = 0;
         sbValue.Insert(0, 
                      tConfig.tScanning.tRearSide1.iJpegQuality.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Color");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tRearSide1.eColor.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Dpi");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tRearSide1.eDpi.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "CutDocument");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tRearSide1.bCut.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         // Store scanning - rear side 2 configuration to file
         sbSection.Length = 0;
         sbSection.Insert(0, "RearSideImage2");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "FileName");
         sbValue.Length = 0;
         sbValue.Insert(0, 
                        tConfig.tScanning.tRearSide2.sbFileName.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Format");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tRearSide2.eFormat.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "JpegQuality");
         sbValue.Length = 0;
         sbValue.Insert(0, 
                      tConfig.tScanning.tRearSide2.iJpegQuality.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Color");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tRearSide2.eColor.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "Dpi");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tRearSide2.eDpi.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);
         sbEntry.Length = 0;
         sbEntry.Insert(0, "CutDocument");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tScanning.tRearSide2.bCut.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue, 
                                              sbConfigFileName, sbErrorMsg);


          ////
         sbSection.Length = 0;
         sbSection.Insert(0, "VoucherType");
         sbEntry.Length = 0;
         sbEntry.Insert(0, "MICRReading");
         sbValue.Length = 0;
         sbValue.Insert(0,tConfig.MICR_Reading.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue,
                                              sbConfigFileName, sbErrorMsg);

         sbEntry.Length = 0;
         sbEntry.Insert(0, "tBarcode");
         sbValue.Length = 0;
         sbValue.Insert(0, tConfig.tBarcode.ToString());
         Mfs100.WritePrivProfileString(sbSection, sbEntry, sbValue,
                                              sbConfigFileName, sbErrorMsg);
      }

      private CFG.FEED_SOURCE GetFeedSourceValue(StringBuilder sbValue, 
                                                   CFG.FEED_SOURCE eDefault)
      {
         // Check if sbValue contains a valid enumeration
         if (sbValue.ToString() == 
                                CFG.FEED_SOURCE.A6_NORMAL_SINGLE.ToString())
         {
            return(CFG.FEED_SOURCE.A6_NORMAL_SINGLE);
         }
         if (sbValue.ToString() == 
                                 CFG.FEED_SOURCE.A6_NORMAL_BATCH.ToString())
         {
            return(CFG.FEED_SOURCE.A6_NORMAL_BATCH);
         }
         if (sbValue.ToString() == 
                              CFG.FEED_SOURCE.A6_STRAIGHT_SINGLE.ToString())
         {
            return(CFG.FEED_SOURCE.A6_STRAIGHT_SINGLE);
         }
         if (sbValue.ToString() == CFG.FEED_SOURCE.A4_SINGLE.ToString())
         {
            return(CFG.FEED_SOURCE.A4_SINGLE);
         }
         if (sbValue.ToString() == CFG.FEED_SOURCE.A4_BATCH.ToString())
         {
            return(CFG.FEED_SOURCE.A4_BATCH);
         }
         if (sbValue.ToString() == CFG.FEED_SOURCE.RED_ROSE_SINGLE.ToString())
         {
             return (CFG.FEED_SOURCE.RED_ROSE_SINGLE);
         }
         if (sbValue.ToString() == CFG.FEED_SOURCE.RED_ROSE_BATCH.ToString())
         {
             return (CFG.FEED_SOURCE.RED_ROSE_BATCH);
         }
         // Use default enumeration
         return(eDefault);
      }
      
      private bool GetBooleanValue(StringBuilder sbValue, Boolean bDefault)
      {
         // Check if sbValue contains a valid enumeration
         if (sbValue.ToString() == false.ToString())
         {
            return(false);
         }
         if (sbValue.ToString() == true.ToString())
         {
            return(true);
         }
         // Use default enumeration
         return(bDefault);
      }

      private CFG.READ_FONT GetReadFontValue(StringBuilder sbValue, 
                                                     CFG.READ_FONT eDefault)
      {
         // Check if sbValue contains a valid enumeration
         if (sbValue.ToString() == CFG.READ_FONT.NONE.ToString())
         {
            return(CFG.READ_FONT.NONE);
         }
         if (sbValue.ToString() == CFG.READ_FONT.CMC7_MAGNETIC.ToString())
         {
            return(CFG.READ_FONT.CMC7_MAGNETIC);
         }
         if (sbValue.ToString() == CFG.READ_FONT.CMC7_OPTICAL.ToString())
         {
            return(CFG.READ_FONT.CMC7_OPTICAL);
         }
         if (sbValue.ToString() == CFG.READ_FONT.CMC7_AND_OCRA.ToString())
         {
            return(CFG.READ_FONT.CMC7_AND_OCRA);
         }
         if (sbValue.ToString() == CFG.READ_FONT.CMC7_AND_OCRB.ToString())
         {
            return(CFG.READ_FONT.CMC7_AND_OCRB);
         }
         if (sbValue.ToString() == CFG.READ_FONT.E13B_MAGNETIC.ToString())
         {
            return(CFG.READ_FONT.E13B_MAGNETIC);
         }
         if (sbValue.ToString() == CFG.READ_FONT.E13B_OPTICAL.ToString())
         {
            return(CFG.READ_FONT.E13B_OPTICAL);
         }
         if (sbValue.ToString() == 
                         CFG.READ_FONT.E13B_MAGNETIC_AND_OPTICAL.ToString())
         {
            return(CFG.READ_FONT.E13B_MAGNETIC_AND_OPTICAL);
         }
         if (sbValue.ToString() == CFG.READ_FONT.OCRA.ToString())
         {
            return(CFG.READ_FONT.OCRA);
         }
         if (sbValue.ToString() == CFG.READ_FONT.OCRA_ALPHA.ToString())
         {
            return(CFG.READ_FONT.OCRA_ALPHA);
         }
         if (sbValue.ToString() == CFG.READ_FONT.OCRA_AND_OCRB.ToString())
         {
            return(CFG.READ_FONT.OCRA_AND_OCRB);
         }
         if (sbValue.ToString() == CFG.READ_FONT.OCRB.ToString())
         {
            return(CFG.READ_FONT.OCRB);
         }
         if (sbValue.ToString() == CFG.READ_FONT.OCRB_ALPHA.ToString())
         {
            return(CFG.READ_FONT.OCRB_ALPHA);
         }
         if (sbValue.ToString() == CFG.READ_FONT.OCRB_UK.ToString())
         {
            return(CFG.READ_FONT.OCRB_UK);
         }
         // Use default enumeration
         return(eDefault);
      }

      private CFG.READ_BLANKMODE GetReadBlankmodeValue(StringBuilder sbValue, 
                                                CFG.READ_BLANKMODE eDefault)
      {
         // Check if sbValue contains a valid enumeration
         if (sbValue.ToString() == CFG.READ_BLANKMODE.NONE.ToString())
         {
            return(CFG.READ_BLANKMODE.NONE);
         }
         if (sbValue.ToString() == 
                                CFG.READ_BLANKMODE._1_BLANK_ONLY.ToString())
         {
            return(CFG.READ_BLANKMODE._1_BLANK_ONLY);
         }
         if (sbValue.ToString() == CFG.READ_BLANKMODE.NORMAL.ToString())
         {
            return(CFG.READ_BLANKMODE.NORMAL);
         }
         // Use default enumeration
         return(eDefault);
      }

      private CFG.READ_SORTMODE GetReadSortmodeValue(StringBuilder sbValue, 
                                                 CFG.READ_SORTMODE eDefault)
      {
         // Check if sbValue contains a valid enumeration
         if (sbValue.ToString() == CFG.READ_SORTMODE.NONE.ToString())
         {
            return(CFG.READ_SORTMODE.NONE);
         }
         if (sbValue.ToString() == 
                                 CFG.READ_SORTMODE.MICR_GOOD_BAD.ToString())
         {
            return(CFG.READ_SORTMODE.MICR_GOOD_BAD);
         }
         if (sbValue.ToString() == 
                         CFG.READ_SORTMODE.SORT_PATTERN_POSITIVE.ToString())
         {
            return(CFG.READ_SORTMODE.SORT_PATTERN_POSITIVE);
         }
         if (sbValue.ToString() == 
                         CFG.READ_SORTMODE.SORT_PATTERN_NEGATIVE.ToString())
         {
            return(CFG.READ_SORTMODE.SORT_PATTERN_NEGATIVE);
         }
         // Extra rule to convert older ini files
         if (sbValue.ToString() == "SORT_PATTERN")
         {
            return(CFG.READ_SORTMODE.SORT_PATTERN_POSITIVE);
         }
         // Use default enumeration
         return(eDefault);
      }

      private CFG.ENDO_MODE GetEndoModeValue(StringBuilder sbValue, 
                                                     CFG.ENDO_MODE eDefault)
      {
         // Check if sbValue contains a valid enumeration
         if (sbValue.ToString() == CFG.ENDO_MODE.NONE.ToString())
         {
            return(CFG.ENDO_MODE.NONE);
         }
         if (sbValue.ToString() == CFG.ENDO_MODE.FIX_STRING.ToString())
         {
            return(CFG.ENDO_MODE.FIX_STRING);
         }
         if (sbValue.ToString() == 
                               CFG.ENDO_MODE.STRING_WITH_COUNTER.ToString())
         {
            return(CFG.ENDO_MODE.STRING_WITH_COUNTER);
         }
         if (sbValue.ToString() == CFG.ENDO_MODE.BITMAP.ToString())
         {
            return(CFG.ENDO_MODE.BITMAP);
         }
         // Use default enumeration
         return(eDefault);
      }

      private CFG.ENDO_DENSITY GetEndoDensityValue(StringBuilder sbValue, 
                                                  CFG.ENDO_DENSITY eDefault)
      {
         // Check if sbValue contains a valid enumeration
         if (sbValue.ToString() == CFG.ENDO_DENSITY.NORMAL.ToString())
         {
            return(CFG.ENDO_DENSITY.NORMAL);
         }
         if (sbValue.ToString() == CFG.ENDO_DENSITY.BOLD.ToString())
         {
            return(CFG.ENDO_DENSITY.BOLD);
         }
         // Use default enumeration
         return(eDefault);
      }

      private WD_MFS100DN.IMAGE_FORMAT GetImageFormatValue(
                                                      StringBuilder sbValue, 
                                          WD_MFS100DN.IMAGE_FORMAT eDefault)
      {
         // Check if sbValue contains a valid enumeration
         if (sbValue.ToString() == WD_MFS100DN.IMAGE_FORMAT.NONE.ToString())
         {
            return(WD_MFS100DN.IMAGE_FORMAT.NONE);
         }
         if (sbValue.ToString() == WD_MFS100DN.IMAGE_FORMAT.BMP.ToString())
         {
            return(WD_MFS100DN.IMAGE_FORMAT.BMP);
         }
         if (sbValue.ToString() == 
                                WD_MFS100DN.IMAGE_FORMAT.TIFF_G4.ToString())
         {
            return(WD_MFS100DN.IMAGE_FORMAT.TIFF_G4);
         }
         if (sbValue.ToString() == WD_MFS100DN.IMAGE_FORMAT.JPEG.ToString())
         {
            return(WD_MFS100DN.IMAGE_FORMAT.JPEG);
         }
         // Use default enumeration
         return(eDefault);
      }

      private CFG.IMAGE_COLOR GetImageColorValue(StringBuilder sbValue, 
                                                   CFG.IMAGE_COLOR eDefault)
      {
         // Check if sbValue contains a valid enumeration
         if (sbValue.ToString() == CFG.IMAGE_COLOR.RED.ToString())
         {
            return(CFG.IMAGE_COLOR.RED);
         }
         if (sbValue.ToString() == CFG.IMAGE_COLOR.GREEN.ToString())
         {
            return(CFG.IMAGE_COLOR.GREEN);
         }
         if (sbValue.ToString() == CFG.IMAGE_COLOR.BLUE.ToString())
         {
            return(CFG.IMAGE_COLOR.BLUE);
         }
         if (sbValue.ToString() == CFG.IMAGE_COLOR.COLOR.ToString())
         {
            return(CFG.IMAGE_COLOR.COLOR);
         }
         // Use default enumeration
         return(eDefault);
      }

      private CFG.IMAGE_DPI GetImageDpiValue(StringBuilder sbValue, 
                                                     CFG.IMAGE_DPI eDefault)
      {
         // Check if sbValue contains a valid enumeration
         if (sbValue.ToString() == CFG.IMAGE_DPI._100.ToString())
         {
            return(CFG.IMAGE_DPI._100);
         }
         if (sbValue.ToString() == CFG.IMAGE_DPI._200.ToString())
         {
            return(CFG.IMAGE_DPI._200);
         }
         // Use default enumeration
         return(eDefault);
      }

      private bool ValidateFeedSourceValue(CFG.FEED_SOURCE eValue)
      {
         // Check if eValue contains a valid enumeration
         if (eValue == CFG.FEED_SOURCE.A6_NORMAL_SINGLE)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.FEED_SOURCE.A6_NORMAL_BATCH)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.FEED_SOURCE.A6_STRAIGHT_SINGLE)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.FEED_SOURCE.A4_SINGLE)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.FEED_SOURCE.A4_BATCH)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.FEED_SOURCE.RED_ROSE_SINGLE)
         {
             // Valid enumeration found
             return (true);
         }
         if (eValue == CFG.FEED_SOURCE.RED_ROSE_BATCH)
         {
             // Valid enumeration found
             return (true);
         }
         // No valid enumeration found
         return(false);
      }
      
      private bool ValidateReadFontValue(CFG.READ_FONT eValue)
      {
         // Check if eValue contains a valid enumeration
         if (eValue == CFG.READ_FONT.NONE)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.CMC7_MAGNETIC)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.CMC7_OPTICAL)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.CMC7_AND_OCRA)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.CMC7_AND_OCRB)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.E13B_MAGNETIC)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.E13B_OPTICAL)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.E13B_MAGNETIC_AND_OPTICAL)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.OCRA)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.OCRA_ALPHA)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.OCRA_AND_OCRB)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.OCRB)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.OCRB_ALPHA)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_FONT.OCRB_UK)
         {
            // Valid enumeration found
            return(true);
         }
         // No valid enumeration found
         return(false);
      }

      private bool ValidateReadBlankmodeValue(CFG.READ_BLANKMODE eValue)
      {
         // Check if eValue contains a valid enumeration
         if (eValue == CFG.READ_BLANKMODE.NONE)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_BLANKMODE._1_BLANK_ONLY)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_BLANKMODE.NORMAL)
         {
            // Valid enumeration found
            return(true);
         }
         // No valid enumeration found
         return(false);
      }

      private bool ValidateReadSortmodeValue(CFG.READ_SORTMODE eValue)
      {
         // Check if eValue contains a valid enumeration
         if (eValue == CFG.READ_SORTMODE.NONE)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_SORTMODE.MICR_GOOD_BAD)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_SORTMODE.SORT_PATTERN_POSITIVE)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.READ_SORTMODE.SORT_PATTERN_NEGATIVE)
         {
            // Valid enumeration found
            return(true);
         }
         // No valid enumeration found
         return(false);
      }

      private bool ValidateEndoModeValue(CFG.ENDO_MODE eValue)
      {
         // Check if eValue contains a valid enumeration
         if (eValue == CFG.ENDO_MODE.NONE)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.ENDO_MODE.FIX_STRING)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.ENDO_MODE.STRING_WITH_COUNTER)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.ENDO_MODE.BITMAP)
         {
            // Valid enumeration found
            return(true);
         }
         // No valid enumeration found
         return(false);
      }

      private bool ValidateEndoDensityValue(CFG.ENDO_DENSITY eValue)
      {
         // Check if eValue contains a valid enumeration
         if (eValue == CFG.ENDO_DENSITY.NORMAL)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.ENDO_DENSITY.BOLD)
         {
            // Valid enumeration found
            return(true);
         }
         // No valid enumeration found
         return(false);
      }

      private bool ValidateEndoDataValue(StringBuilder sbValue)
      {
         int  iCounter;
         
         // Check if eValue contains invalid characters
         iCounter = 0;
         while (iCounter < sbValue.Length)
         {
            if ((char.IsLetterOrDigit(sbValue.ToString(), iCounter) !=
                            true) && (char.IsPunctuation(sbValue.ToString(),
                        iCounter) != true) && (sbValue.ToString().Substring(
                                                       iCounter, 1) != " "))
            {
               // Invalid character found
               return(false);
            }
            ++iCounter;
         }
         // No invalid character found
         return(true);
      }

      private bool ValidateImageDirectoryValue(StringBuilder sbValue)
      {
         if (System.IO.Directory.Exists(sbValue.ToString()) == true)
         {
            // Image directory does exist
            return(true);
         }
         // Image directory does not exist
         return(false);
      }

      private bool ValidateImageFileNameValue(StringBuilder sbValue)
      {
         int  iCounter;
         
         // Check if eValue contains invalid characters
         iCounter = 0;
         while (iCounter < sbValue.Length)
         {
            if ((char.IsLetterOrDigit(sbValue.ToString(),
                                                       iCounter) != true) &&
                       (sbValue.ToString().Substring(iCounter, 1) != "%") &&
                       (sbValue.ToString().Substring(iCounter, 1) != "-") &&
                         (sbValue.ToString().Substring(iCounter, 1) != "_"))
            {
               // Invalid character found
               return(false);
            }
            ++iCounter;
         }
         // No invalid character found
         return(true);
      }

      private bool ValidateImageFormatValue(WD_MFS100DN.IMAGE_FORMAT eValue)
      {
         // Check if eValue contains a valid enumeration
         if (eValue == WD_MFS100DN.IMAGE_FORMAT.NONE)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == WD_MFS100DN.IMAGE_FORMAT.BMP)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == WD_MFS100DN.IMAGE_FORMAT.TIFF_G4)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == WD_MFS100DN.IMAGE_FORMAT.JPEG)
         {
            // Valid enumeration found
            return(true);
         }
         // No valid enumeration found
         return(false);
      }

      private bool ValidateImageColorValue(CFG.IMAGE_COLOR eValue)
      {
         // Check if eValue contains a valid enumeration
         if (eValue == CFG.IMAGE_COLOR.RED)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.IMAGE_COLOR.GREEN)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.IMAGE_COLOR.BLUE)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.IMAGE_COLOR.COLOR)
         {
            // Valid enumeration found
            return(true);
         }
         // No valid enumeration found
         return(false);
      }

      private bool ValidateImageDpiValue(CFG.IMAGE_DPI eValue)
      {
         // Check if eValue contains a valid enumeration
         if (eValue == CFG.IMAGE_DPI._100)
         {
            // Valid enumeration found
            return(true);
         }
         if (eValue == CFG.IMAGE_DPI._200)
         {
            // Valid enumeration found
            return(true);
         }
         // No valid enumeration found
         return(false);
      }

      private bool ParameterListInsert(MFS_PARAMETER_ID eIndex,
                                                      StringBuilder sbValue)
      {
         int  iCounter;
         
         // Search whole array
         iCounter = 0;
         while (iCounter < tConfig.tMfsParameters.ciIndexes.Length)
         {
            // If index in constant list = given index
            if ((int) eIndex == (int) 
                        tConfig.tMfsParameters.ciIndexes.GetValue(iCounter))
            {
               // If object still not exists
               if (tConfig.tMfsParameters.sbParameters[iCounter] == null)
               {
                  // Create new object
                  tConfig.tMfsParameters.sbParameters[iCounter] =
                                                        new StringBuilder();
               }
               // Copy given value into array 
               tConfig.tMfsParameters.sbParameters[iCounter].Length = 0;
               tConfig.tMfsParameters.sbParameters[iCounter].Insert(0, 
                                                                   sbValue);
               // Mark index as found
               return(true);
            }
            ++iCounter;
         }
         // Mark index as not found
         return(false);
      }

      private String ImageDpi2String(CFG.IMAGE_DPI eDpi)
      {
         if (eDpi == CFG.IMAGE_DPI._100)
         {
            return("100");
         }
         return("200");
      }
      
      private String EndoMode2String(CFG.ENDO_MODE eMode)
      {
         if (eMode == CFG.ENDO_MODE.FIX_STRING)
         {
            return("1");
         }
         if (eMode == CFG.ENDO_MODE.STRING_WITH_COUNTER)
         {
            return("3");
         }
         if (eMode == CFG.ENDO_MODE.BITMAP)
         {
            return("2");
         }
            return("0");
      }
      
      private String ReadBlankMode2String(CFG.READ_BLANKMODE eBlankMode)
      {
         if (eBlankMode == CFG.READ_BLANKMODE.NONE)
         {
            return("0");
         }
         if (eBlankMode == CFG.READ_BLANKMODE._1_BLANK_ONLY)
         {
            return("1");
         }
         return("2");
      }
      
      private String ImageFormat2String(WD_MFS100DN.IMAGE_FORMAT eFormat)
      {
         if (eFormat == WD_MFS100DN.IMAGE_FORMAT.BMP)
         {
            return("2");
         }
         if (eFormat == WD_MFS100DN.IMAGE_FORMAT.TIFF_G4)
         {
            return("4");
         }
         if (eFormat == WD_MFS100DN.IMAGE_FORMAT.JPEG)
         {
            return("5");
         }
         return("0");
      }
      
      private String Bool2String(Boolean bBool)
      {
         if (bBool == false)
         {
            return("0");
         }
         else
         {
            return("1");
         }
      }
      
      private String ImageColor2String(CFG.IMAGE_COLOR eFrontColor, 
                                                 CFG.IMAGE_COLOR eRearColor)
      {
         int  iColor;
         
         switch (eFrontColor)
         {
            case CFG.IMAGE_COLOR.RED :
            {
               iColor = 0;
               switch (eRearColor)
               {
                  case CFG.IMAGE_COLOR.RED :
                  {
                     iColor += 0 * 16;
                     break;
                  }
                  case CFG.IMAGE_COLOR.GREEN :
                  {
                     iColor += 1 * 16;
                     break;
                  }
                  case CFG.IMAGE_COLOR.BLUE :
                  {
                     iColor += 2 * 16;
                     break;
                  }
                  default :
                  {
                     iColor += 4 * 16;
                     break;
                  }
               }
               break;
            }
            case CFG.IMAGE_COLOR.GREEN :
            {
               iColor = 1;
               switch (eRearColor)
               {
                  case CFG.IMAGE_COLOR.RED :
                  {
                     iColor += 0 * 16;
                     break;
                  }
                  case CFG.IMAGE_COLOR.GREEN :
                  {
                     iColor += 1 * 16;
                     break;
                  }
                  case CFG.IMAGE_COLOR.BLUE :
                  {
                     iColor += 2 * 16;
                     break;
                  }
                  default :
                  {
                     iColor += 4 * 16;
                     break;
                  }
               }
               break;
            }
            case CFG.IMAGE_COLOR.BLUE :
            {
               iColor = 2;
               switch (eRearColor)
               {
                  case CFG.IMAGE_COLOR.RED :
                  {
                     iColor += 0 * 16;
                     break;
                  }
                  case CFG.IMAGE_COLOR.GREEN :
                  {
                     iColor += 1 * 16;
                     break;
                  }
                  case CFG.IMAGE_COLOR.BLUE :
                  {
                     iColor += 2 * 16;
                     break;
                  }
                  default :
                  {
                     iColor += 4 * 16;
                     break;
                  }
               }
               break;
            }
            default :
            {
               iColor = 4;
               switch (eRearColor)
               {
                  case CFG.IMAGE_COLOR.RED :
                  {
                     iColor += 0 * 16;
                     break;
                  }
                  case CFG.IMAGE_COLOR.GREEN :
                  {
                     iColor += 1 * 16;
                     break;
                  }
                  case CFG.IMAGE_COLOR.BLUE :
                  {
                     iColor += 2 * 16;
                     break;
                  }
                  default :
                  {
                     iColor += 4 * 16;
                     break;
                  }
               }
               break;
            }
         }
         return(iColor.ToString());
      }
      
      private String EndoDensity2String(CFG.ENDO_DENSITY eDensity)
      {
         if (eDensity == CFG.ENDO_DENSITY.NORMAL)
         {
            return("1");
         }
         return("0");
      }
      
      private uint ConvertFont(CFG.READ_FONT eFont)
      {
         switch (eFont)
         {
            case CFG.READ_FONT.CMC7_MAGNETIC :
            {
               return((uint) WD_MFS100DN.FONT.CMC7_MICR);
            }
            case CFG.READ_FONT.CMC7_OPTICAL :
            {
               return((uint) WD_MFS100DN.FONT.CMC7_OPTICAL);
            }
            case CFG.READ_FONT.CMC7_AND_OCRA :
            {
               return((uint) WD_MFS100DN.FONT.CMC7_MICR + 
                                     (uint) WD_MFS100DN.FONT.OCRA_STANDARD);
            }
            case CFG.READ_FONT.CMC7_AND_OCRB :
            {
               return((uint) WD_MFS100DN.FONT.CMC7_MICR + 
                                     (uint) WD_MFS100DN.FONT.OCRB_STANDARD);
            }
            case CFG.READ_FONT.E13B_MAGNETIC :
            {
               return((uint) WD_MFS100DN.FONT.E13B_MICR);
            }
            case CFG.READ_FONT.E13B_OPTICAL :
            {
               return((uint) WD_MFS100DN.FONT.E13B_OPTICAL);
            }
            case CFG.READ_FONT.E13B_MAGNETIC_AND_OPTICAL :
            {
               return((uint) WD_MFS100DN.FONT.E13B_MICR_AND_OPTICAL);
            }
            case CFG.READ_FONT.OCRA :
            {
               return((uint) WD_MFS100DN.FONT.OCRA_STANDARD);
            }
            case CFG.READ_FONT.OCRA_ALPHA :
            {
               return((uint) WD_MFS100DN.FONT.OCRA_ALPHANUMERIC);
            }
            case CFG.READ_FONT.OCRA_AND_OCRB :
            {
               return((uint) WD_MFS100DN.FONT.OCRA_OCRB_MIXED);
            }
            case CFG.READ_FONT.OCRB :
            {
               return((uint) WD_MFS100DN.FONT.OCRB_STANDARD);
            }
            case CFG.READ_FONT.OCRB_ALPHA :
            {
               return((uint) WD_MFS100DN.FONT.OCRB_ALPHANUMERIC);
            }
            case CFG.READ_FONT.OCRB_UK :
            {
               return((uint) WD_MFS100DN.FONT.OCRB_UK);
            }
            case CFG.READ_FONT.NONE :
            default :
            {
               return((uint) WD_MFS100DN.FONT.NONE);
            }
         }
      }
////////////////////// Added by Tolga Yildiran ///////////////////////////
      private void btnOptions_Click(object sender, EventArgs e)
      {
          DialogResult drResult;
          bool bValid;

          // Create setup class
          Setup2Form frmSetup = new Setup2Form(tConfig,this);
          // Do until configuration is valid
          do
          {
              // Show setup dialog
              drResult = frmSetup.ShowDialog();
              // if setup was canceled
              if (drResult != DialogResult.OK)
              {
                  // Do not validate and store configuration
                  return;
              }
              // Validate configuration
              bValid = ValidateConfiguration(tConfig);
          } while (bValid != true);
          // Update configuration file
          StoreConfiguration(tConfig);

      }

      

      private void statisticsItemsUpdateAndShow(statParams mySP, ValidForm myVF)
      {
          myVF.lbStatistics.Items.Clear();
          myVF.lbStatistics.Items.Add(mySP.imgCnt.ToString());
          myVF.lbStatistics.Items.Add(mySP.validCnt.ToString());
          myVF.lbStatistics.Items.Add(mySP.invCnt.ToString());
          myVF.lbStatistics.Items.Add(mySP.reusedCnt.ToString());
          myVF.lbStatistics.Update();
       }
      private void listBoxEventItemAdd( ListBox lb, string lbItem)
      {
          lb.Items.Add(lbItem);
          lb.SelectedIndex = lb.Items.Count - 1;
          lb.SelectedIndex = -1;
          lb.Update();
 
      }

      public void VoucherScanning(int  my_iFeedMode, uint my_uiReadFont, 
          CFG my_Config, StringBuilder my_sbErrorMsg)         
      {
          iResult = Mfs100.ScanFeeder(my_iFeedMode, my_uiReadFont,
                           my_Config.tFeeding.iTimeout, my_sbErrorMsg);
      }
      public void VoucherScanning(ValidForm myVS)
      {
          iResult = Mfs100.ScanFeeder(PUB_iFeedMode, PUB_uiReadFont,
                           tConfig.tFeeding.iTimeout, sbErrorMsg);
          
         
          listBoxEventItemAdd(lbEvents, "mfScan() = " + iResult.ToString());
          //If scanning was successful
          while(true)
          {

              /// Set an event handler for document scanning done
              iResult = Mfs100.SetEvents(this.Handle,                               
                                (int)WINDOW_MESSAGE_TYPE.EVENT_ON_ALL_DONE,
                                                             0, sbErrorMsg);
              
              if (iResult < 0)
              {
                  if (tConfig.tScanning.tFrontSide1.eFormat ==
                                              WD_MFS100DN.IMAGE_FORMAT.NONE)
                  {
                      sbFrontSide1FileName.Length = 0;
                  }
                  else
                  {
                      //Create file name of front side image 1 for document 1 
                      PUB_sbValue.Length = 0;
                      PUB_sbValue.Insert(0, tConfig.tScanning.sbImageDirectory.ToString());
                      PUB_sbValue.Append("\\");
                      PUB_sbValue.Append(tConfig.tScanning.tFrontSide1.sbFileName.ToString());
                      Mfs100.BuildFileNameFromTemplate(sbFrontSide1FileName, PUB_sbValue,
                                    tConfig.tScanning.tFrontSide1.eFormat, 1, sbErrorMsg);
                  }
                  Application.DoEvents();
                 
                 
              }
              // If single feed failed
              if(iResult > 0)
              {
                  break;
                  //TODO: show message                

              }

  //            if (iResult < 0)
  //            {
  //                break;
  ////TODO: show message                
                  
  //            }

             
          }

             
          
      }

      public List<String> voucherScanFake()
      {
          List<String> tempBarcodes = new List<String>();


          // vendor a verilmiş olanlar
            //tempBarcodes.Add("90037-001-D85");
            //tempBarcodes.Add("90037-002-4BC");
            //tempBarcodes.Add("90037-003-7AF");
            //tempBarcodes.Add("90037-004-6EB");
            //tempBarcodes.Add("90037-005-5D0");
            //tempBarcodes.Add("90037-006-C3F");
            //tempBarcodes.Add("90037-007-A36");
            //tempBarcodes.Add("90037-008-9B4");
            //tempBarcodes.Add("90037-009-3F7");
            //tempBarcodes.Add("90037-010-024");
            //tempBarcodes.Add("90037-011-6B7");
            //tempBarcodes.Add("90037-012-60D");
            //tempBarcodes.Add("90037-013-6F2");
            //tempBarcodes.Add("90037-014-67E");
            //tempBarcodes.Add("90037-015-4A4");
            //tempBarcodes.Add("90037-016-5B3");
            //tempBarcodes.Add("90037-017-B7E");
            //tempBarcodes.Add("90037-018-48E");
            //tempBarcodes.Add("90037-019-B7B");
            //tempBarcodes.Add("90037-020-894");
            //tempBarcodes.Add("90037-021-A67");
            //tempBarcodes.Add("90037-022-4A6");
            //tempBarcodes.Add("90037-023-DFE");
            //tempBarcodes.Add("90037-024-409");
            //tempBarcodes.Add("90037-025-29B");
//tempBarcodes.Add("90004-001-1D4");
//tempBarcodes.Add("90004-002-5E0");
//tempBarcodes.Add("90004-003-BEE");
//tempBarcodes.Add("90004-004-91A");
//tempBarcodes.Add("90004-005-B05");
//tempBarcodes.Add("90004-006-0E3");
//tempBarcodes.Add("90004-007-7C6");
//tempBarcodes.Add("90004-008-14E");
//tempBarcodes.Add("90004-009-6AE");
//tempBarcodes.Add("90004-010-F9A");
//tempBarcodes.Add("90004-011-4E7");
//tempBarcodes.Add("90004-012-E56");
//tempBarcodes.Add("90004-013-590");
//tempBarcodes.Add("90004-014-886");
//tempBarcodes.Add("90004-015-E9F");
//tempBarcodes.Add("90004-016-FB7");
//tempBarcodes.Add("90004-017-1E6");
//tempBarcodes.Add("90004-018-1FE");
//tempBarcodes.Add("90004-019-FAC");
//tempBarcodes.Add("90004-020-AE6");
//tempBarcodes.Add("90004-021-27E");
//tempBarcodes.Add("90004-022-27B");
//tempBarcodes.Add("90004-023-2A3");
//tempBarcodes.Add("90004-024-656");
//tempBarcodes.Add("90004-025-9AD");
          tempBarcodes.Add("90007-001-726");
          tempBarcodes.Add("90007-002-381");
          tempBarcodes.Add("90007-003-879");
          tempBarcodes.Add("90007-004-B03");
          tempBarcodes.Add("90007-005-67D");
          tempBarcodes.Add("90007-006-147");
          tempBarcodes.Add("90007-007-522");
          tempBarcodes.Add("90007-008-D70");
          tempBarcodes.Add("90007-009-4FB");
          tempBarcodes.Add("90007-010-E02");
          tempBarcodes.Add("90007-011-242");
          tempBarcodes.Add("90007-012-A73");
          tempBarcodes.Add("90007-013-500");
          tempBarcodes.Add("90007-014-248");
          tempBarcodes.Add("90007-015-F79");
          tempBarcodes.Add("90007-016-168");
          tempBarcodes.Add("90007-017-48B");
          tempBarcodes.Add("90007-018-2BB");
          tempBarcodes.Add("90007-019-E49");
          tempBarcodes.Add("90007-020-805");
          tempBarcodes.Add("90007-021-329");
          tempBarcodes.Add("90007-022-CAA");
          tempBarcodes.Add("90007-023-7C6");
          tempBarcodes.Add("90007-024-70F");
          tempBarcodes.Add("90007-025-C5D");
          //
          tempBarcodes.Add("90007-001-726");
          tempBarcodes.Add("90007-002-381");
          tempBarcodes.Add("90007-003-879");
          tempBarcodes.Add("90007-004-B03");
          tempBarcodes.Add("90007-005-67D");
          tempBarcodes.Add("90007-006-147");



          /*
           * tempBarcodes.Add("90008-001-8D0");
tempBarcodes.Add("90008-002-D75");
tempBarcodes.Add("90008-003-B94");
tempBarcodes.Add("90008-004-608");
tempBarcodes.Add("90008-005-CD0");
tempBarcodes.Add("90008-006-B9D");
tempBarcodes.Add("90008-007-C48");
tempBarcodes.Add("90008-008-AEE");
tempBarcodes.Add("90008-009-686");
tempBarcodes.Add("90008-010-1B7");
tempBarcodes.Add("90008-011-D93");
tempBarcodes.Add("90008-012-99E");
tempBarcodes.Add("90008-013-3BB");
tempBarcodes.Add("90008-014-71B");
tempBarcodes.Add("90008-015-4FE");
tempBarcodes.Add("90008-016-807");
tempBarcodes.Add("90008-017-39B");
tempBarcodes.Add("90008-018-EB4");
tempBarcodes.Add("90008-019-C67");
tempBarcodes.Add("90008-020-359");
tempBarcodes.Add("90008-021-E1A");
tempBarcodes.Add("90008-022-B0A");
tempBarcodes.Add("90008-023-FEC");
tempBarcodes.Add("90008-024-E0D");
tempBarcodes.Add("90008-025-0D4");*/

          /*
           *Invalid olanlar:


tempBarcodes.Add("90009-001-E94");
tempBarcodes.Add("90009-002-707");
tempBarcodes.Add("90009-003-F26");
tempBarcodes.Add("90009-004-ACE");
tempBarcodes.Add("90009-005-D4D");
tempBarcodes.Add("90009-006-347");
tempBarcodes.Add("90009-007-239");
tempBarcodes.Add("90009-008-255");
tempBarcodes.Add("90009-009-574");
tempBarcodes.Add("90009-010-DFB");
tempBarcodes.Add("90009-011-274");
tempBarcodes.Add("90009-012-7BD");
tempBarcodes.Add("90009-013-848");
tempBarcodes.Add("90009-014-99D");
tempBarcodes.Add("90009-015-608");
tempBarcodes.Add("90009-016-9BC");
tempBarcodes.Add("90009-017-F82");
tempBarcodes.Add("90009-018-ED8");
tempBarcodes.Add("90009-019-2B3");
tempBarcodes.Add("90009-020-812");
tempBarcodes.Add("90009-021-731");
tempBarcodes.Add("90009-022-1BC");
tempBarcodes.Add("90009-023-C59");
tempBarcodes.Add("90009-024-439");
tempBarcodes.Add("90009-025-650");
           * */


          /*Invalid 2. grup
           * tempBarcodes.Add("90011-001-78C");
tempBarcodes.Add("90011-002-E83");
tempBarcodes.Add("90011-003-F57");
tempBarcodes.Add("90011-004-4AD");
tempBarcodes.Add("90011-005-43E");
tempBarcodes.Add("90011-006-2EA");
tempBarcodes.Add("90011-007-F48");
tempBarcodes.Add("90011-008-380");
tempBarcodes.Add("90011-009-17F");
tempBarcodes.Add("90011-010-8BD");
tempBarcodes.Add("90011-011-8CC");
tempBarcodes.Add("90011-012-7E8");
tempBarcodes.Add("90011-013-C7B");
tempBarcodes.Add("90011-014-835");
tempBarcodes.Add("90011-015-AEC");
tempBarcodes.Add("90011-016-02C");
tempBarcodes.Add("90011-017-401");
tempBarcodes.Add("90011-018-281");
tempBarcodes.Add("90011-019-DEB");
tempBarcodes.Add("90011-020-D43");
tempBarcodes.Add("90011-021-3A6");
tempBarcodes.Add("90011-022-166");
tempBarcodes.Add("90011-023-0A0");
tempBarcodes.Add("90011-024-D42");
tempBarcodes.Add("90011-025-CC9");*/

          return tempBarcodes;
 
      }

      public void MICR_Scanning()
      {
          CFG.READ_FONT RF = tConfig.tReading.eFont;

          //string ExePath = System.Windows.Forms.Application.ExecutablePath;
          //if (Path.GetDirectoryName(ExePath).EndsWith("\\"))
          //{
          //    ExePath = Path.GetDirectoryName(ExePath);
          //}
          //else
          //{
          //    ExePath = Path.GetDirectoryName(ExePath) + "\\";
          //}

          //string codelinePath = ExePath + "codeline.txt";
    

          try
          {
              File.WriteAllText("codeline.txt", string.Empty);
          }
          catch
          {
              MessageBox.Show(
                                "Cannot open file codeline.txt for writing",
                                Settings.Default.messageBoxTitle,
                                 MessageBoxButtons.OK, MessageBoxIcon.Stop);
          }
          //tConfig.tReading.eFont = CFG.READ_FONT.E13B_MAGNETIC_AND_OPTICAL;
          uint temp_uiReadFont = ConvertFont(tConfig.tReading.eFont);
          iResult = Mfs100.ScanFeeder(PUB_iFeedMode, temp_uiReadFont,
                           tConfig.tFeeding.iTimeout, sbErrorMsg);
          listBoxEventItemAdd(lbEvents, "mfScan() = " + iResult.ToString());
          //If scanning was successful
          if (iResult >= 0)
          {              
              //Application.DoEvents();
          }
          // If single feed failed
          else
          {
              //myVS.btnStartReading.Enabled = true;
              //myVS.btnStartReading.Visible = true;
              //myVS.btnStartReading.Show();
          }
          
          tConfig.tReading.eFont = RF; 
 
      }
      private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
      {
          Application.Exit();
      }

      private void btnMICRtest_Click(object sender, EventArgs e)
      {
          defaultSettingsForRedRoseBatch(ref tConfig);
          MICR_Test_Form MTForm = new MICR_Test_Form(this);
          MTForm.Show();
      }
      public void defaultSettingsForRedRoseBatch(ref CFG tConfig)
      {
          defaultCFG = tConfig;
          
          defaultCFG.tFeeding.eSource = CFG.FEED_SOURCE.RED_ROSE_BATCH;
          defaultCFG.tFeeding.bPipelineEnabled = true;
          defaultCFG.tFeeding.iTimeout = 5000;
          defaultCFG.tFeeding.iDoublefeedThreshold = 120;
          //
          defaultCFG.tReading.eFont = CFG.READ_FONT.E13B_MAGNETIC;
          defaultCFG.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NORMAL;
          defaultCFG.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NONE;
          defaultCFG.tReading.eSortMode = CFG.READ_SORTMODE.NONE;
          //
          defaultCFG.tEndorsing.eMode = CFG.ENDO_MODE.NONE;
          //
          defaultCFG.tScanning.iMaxImageWidth = 250;
          defaultCFG.tScanning.iMaxImageHeight = 450;
          defaultCFG.tScanning.iTempTolerance = 0;
          defaultCFG.tScanning.iTempPeriod = 2;
          defaultCFG.tScanning.bUseCalibration = true;
          if (Directory.Exists(@"C:\RedRose"))
          {
              if (!Directory.Exists(@"C:\RedRose\Images"))
                  Directory.CreateDirectory(@"C:\RedRose\Images");
          }
          else
          {
              Directory.CreateDirectory(@"C:\RedRose");
              Directory.CreateDirectory(@"C:\RedRose\Images");
          }
          DirectoryInfo directory = new DirectoryInfo(@"C:\RedRose\Images");
          //Clean the directory
          Empty(directory);

          defaultCFG.tScanning.sbImageDirectory = new StringBuilder();
          string foldPath = "C:\\RedRose\\Images";
          defaultCFG.tScanning.sbImageDirectory.Insert(0, value: foldPath);
          defaultCFG.tBarcode = BarcodeFormat.CODE_128;
          //
          defaultCFG.tScanning.tFrontSide1.sbFileName = new StringBuilder();
          defaultCFG.tScanning.tFrontSide1.sbFileName.Insert(0,"FS-1%04d");
          defaultCFG.tScanning.tFrontSide1.eFormat = IMAGE_FORMAT.JPEG;
          defaultCFG.tScanning.tFrontSide1.iJpegQuality = 99;
          defaultCFG.tScanning.tFrontSide1.eColor = CFG.IMAGE_COLOR.BLUE;
          defaultCFG.tScanning.tFrontSide1.eDpi = CFG.IMAGE_DPI._200;
          defaultCFG.tScanning.tFrontSide1.bCut = true;
          //
          defaultCFG.tScanning.tFrontSide2.eFormat = IMAGE_FORMAT.NONE;
          defaultCFG.tScanning.tRearSide1.eFormat = IMAGE_FORMAT.NONE;
          defaultCFG.tScanning.tRearSide2.eFormat = IMAGE_FORMAT.NONE;
          //


          tConfig = defaultCFG;
         
 
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
          }

      }

      public void setupFileNameCheck(ref CFG cfg)
      {
          if (cfg.tScanning.tFrontSide1.sbFileName.ToString() == "")
              cfg.tScanning.tFrontSide1.sbFileName.Insert(0, "FS-1%04d");

      }

   }
}
