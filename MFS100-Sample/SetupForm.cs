using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using  OnBarcode.Barcode.BarcodeScanner;
//using BarcodeLib.BarcodeReader;

namespace RedRose_VoucherScanner
{
   public partial class SetupForm : Form
   {
      public CFG  tConfig;
      String  strEndoFont;
         
      protected override CreateParams CreateParams
      {
         get
         {
            CreateParams parms = base.CreateParams;
            parms.ClassStyle |= 0x200;  // CS_NOCLOSE
            return parms;
         }
      }         

      public SetupForm(CFG tClassAdress)
      {
         InitializeComponent();
         // Copy structure reference 
         tConfig = tClassAdress;
         // Preset Feeding - Source
         cbFeedSource.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tFeeding.eSource)
         {
            case CFG.FEED_SOURCE.A6_NORMAL_SINGLE :
            {
               cbFeedSource.SelectedIndex = 0;
               break;
            }
            case CFG.FEED_SOURCE.A6_NORMAL_BATCH :
            {
               cbFeedSource.SelectedIndex = 1;
               break;
            }
            case CFG.FEED_SOURCE.A6_STRAIGHT_SINGLE :
            {
               cbFeedSource.SelectedIndex = 2;
               break;
            }
            case CFG.FEED_SOURCE.A4_SINGLE :
            {
               cbFeedSource.SelectedIndex = 3;
               break;
            }
            case CFG.FEED_SOURCE.A4_BATCH :
            {
               cbFeedSource.SelectedIndex = 4;
               break;
            }
            case CFG.FEED_SOURCE.RED_ROSE_SINGLE:
            {
                cbFeedSource.SelectedIndex = 5;
                break;
            }
            case CFG.FEED_SOURCE.RED_ROSE_BATCH:
            {
                cbFeedSource.SelectedIndex = 6;
                break;
            }
         }
         // Preset Feeding - Pipeline
         cbPipeline.DropDownStyle = ComboBoxStyle.DropDownList;
         if (tConfig.tFeeding.bPipelineEnabled == true)
         {
            cbPipeline.SelectedIndex = 0;
         }
         else
         {
            cbPipeline.SelectedIndex = 1;
         }
         // Preset Feeding - Timeout
         tbFeedTimeout.Text = tConfig.tFeeding.iTimeout.ToString();
         // Preset Feeding - Doublefeed threshold
         tbDoublefeedThreshold.Text = 
                           tConfig.tFeeding.iDoublefeedThreshold.ToString();
         // Preset Reading - Font
         cbReadFont.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tReading.eFont)
         {
            case CFG.READ_FONT.NONE :
            {
               cbReadFont.SelectedIndex = 0;
               break;
            }
            case CFG.READ_FONT.CMC7_MAGNETIC :
            {
               cbReadFont.SelectedIndex = 1;
               break;
            }
            case CFG.READ_FONT.CMC7_OPTICAL :
            {
               cbReadFont.SelectedIndex = 2;
               break;
            }
            case CFG.READ_FONT.CMC7_AND_OCRA :
            {
               cbReadFont.SelectedIndex = 3;
               break;
            }
            case CFG.READ_FONT.CMC7_AND_OCRB :
            {
               cbReadFont.SelectedIndex = 4;
               break;
            }
            case CFG.READ_FONT.E13B_MAGNETIC :
            {
               cbReadFont.SelectedIndex = 5;
               break;
            }
            case CFG.READ_FONT.E13B_OPTICAL :
            {
               cbReadFont.SelectedIndex = 6;
               break;
            }
            case CFG.READ_FONT.E13B_MAGNETIC_AND_OPTICAL :
            {
               cbReadFont.SelectedIndex = 7;
               break;
            }
            case CFG.READ_FONT.OCRA :
            {
               cbReadFont.SelectedIndex = 8;
               break;
            }
            case CFG.READ_FONT.OCRA_ALPHA :
            {
               cbReadFont.SelectedIndex = 9;
               break;
            }
            case CFG.READ_FONT.OCRA_AND_OCRB :
            {
               cbReadFont.SelectedIndex = 10;
               break;
            }
            case CFG.READ_FONT.OCRB :
            {
               cbReadFont.SelectedIndex = 11;
               break;
            }
            case CFG.READ_FONT.OCRB_ALPHA :
            {
               cbReadFont.SelectedIndex = 12;
               break;
            }
            case CFG.READ_FONT.OCRB_UK :
            {
               cbReadFont.SelectedIndex = 13;
               break;
            }
         }
         // Preset Reading - MICR blanks
         cbMicrBlanks.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tReading.eMicrBlanks)
         {
            case CFG.READ_BLANKMODE.NONE :
            {
               cbMicrBlanks.SelectedIndex = 0;
               break;
            }
            case CFG.READ_BLANKMODE._1_BLANK_ONLY :
            {
               cbMicrBlanks.SelectedIndex = 1;
               break;
            }
            case CFG.READ_BLANKMODE.NORMAL :
            {
               cbMicrBlanks.SelectedIndex = 2;
               break;
            }
         }
         // Preset Reading - OCR blanks
         cbOcrBlanks.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tReading.eOcrBlanks)
         {
            case CFG.READ_BLANKMODE.NONE :
            {
               cbOcrBlanks.SelectedIndex = 0;
               break;
            }
            case CFG.READ_BLANKMODE._1_BLANK_ONLY :
            {
               cbOcrBlanks.SelectedIndex = 1;
               break;
            }
            case CFG.READ_BLANKMODE.NORMAL :
            {
               cbOcrBlanks.SelectedIndex = 2;
               break;
            }
         }
         // Preset Reading - Sorting
         cbSorting.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tReading.eSortMode)
         {
            case CFG.READ_SORTMODE.NONE :
            {
               cbSorting.SelectedIndex = 0;
               break;
            }
            case CFG.READ_SORTMODE.MICR_GOOD_BAD :
            {
               cbSorting.SelectedIndex = 1;
               break;
            }
            case CFG.READ_SORTMODE.SORT_PATTERN_POSITIVE :
            {
               cbSorting.SelectedIndex = 2;
               break;
            }
            case CFG.READ_SORTMODE.SORT_PATTERN_NEGATIVE :
            {
               cbSorting.SelectedIndex = 3;
               break;
            }
         }
         // Preset Reading - Sort pattern (regular expression)
         tbSortPattern.Text = tConfig.tReading.sbSortPattern.ToString();
         // Preset Endorsing - Mode
         cbEndoMode.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tEndorsing.eMode)
         {
            case CFG.ENDO_MODE.NONE :
            {
               cbEndoMode.SelectedIndex = 0;
               break;
            }
            case CFG.ENDO_MODE.FIX_STRING :
            {
               cbEndoMode.SelectedIndex = 1;
               break;
            }
            case CFG.ENDO_MODE.STRING_WITH_COUNTER :
            {
               cbEndoMode.SelectedIndex = 2;
               break;
            }
            case CFG.ENDO_MODE.BITMAP :
            {
               cbEndoMode.SelectedIndex = 3;
               break;
            }
         }
         // Preset Endorsing - Start counter
         tbEndoStartCounter.Text = 
                                tConfig.tEndorsing.iStartCounter.ToString();
         // Preset Endorsing - Step(s)
         tbEndoCounterSteps.Text = tConfig.tEndorsing.iSteps.ToString();
         // Preset Endorsing - Font
         strEndoFont = tConfig.tEndorsing.sbFont.ToString();
         tbEndoFont.Text = GetFileTitle(strEndoFont);
         // Preset Endorsing - Position
         tbEndoPos.Text = tConfig.tEndorsing.iPosition.ToString();
         // Preset Endorsing - Density
         cbEndoDensity.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tEndorsing.eDensity)
         {
            case CFG.ENDO_DENSITY.NORMAL :
            {
               cbEndoDensity.SelectedIndex = 0;
               break;
            }
            case CFG.ENDO_DENSITY.BOLD :
            {
               cbEndoDensity.SelectedIndex = 1;
               break;
            }
         }
         // Preset Endorsing - Data
         tbEndoData.Text = tConfig.tEndorsing.sbData.ToString();
         // Preset Scanning - Maximum image width
         tbMaxImageWidth.Text = tConfig.tScanning.iMaxImageWidth.ToString();
         // Preset Scanning - Maximum image height
         tbMaxImageHeight.Text = 
                               tConfig.tScanning.iMaxImageHeight.ToString();
         // Preset Scanning - Temperature tolerance
         tbTempTolerance.Text = tConfig.tScanning.iTempTolerance.ToString();
         // Preset Scanning - Period between temperature calibrations
         tbTempPeriod.Text = tConfig.tScanning.iTempPeriod.ToString();
         // Preset Scanning - True color
         if ((tConfig.tScanning.tFrontSide1.eColor != 
                                                   CFG.IMAGE_COLOR.COLOR) && 
                                     (tConfig.tScanning.tRearSide1.eColor !=
                                                     CFG.IMAGE_COLOR.COLOR))
         {
            chkbTrueColor.Checked = false;
         }
         else
         {
            if (tConfig.tScanning.bTrueColor == true)
            {
               chkbTrueColor.Checked = true;
            }
            else
            {
               chkbTrueColor.Checked = false;
            }
         }
         // Preset Scanning - Use Calibration
         if (tConfig.tScanning.bUseCalibration == true)
         {
            chkbUseCalibration.Checked = true;
         }
         else
         {
            chkbUseCalibration.Checked = false;
         }
         // Preset Scanning - Image directory
         tbImageDirectory.Text = 
                              tConfig.tScanning.sbImageDirectory.ToString();
         // Preset Scanning - Front side image 1 - File name
         tbFrontSideImage1FileName.Text = 
                        tConfig.tScanning.tFrontSide1.sbFileName.ToString();
         // Preset Scanning - Front side image 1 - Format
         cbFrontSideImage1Format.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tScanning.tFrontSide1.eFormat)
         {
            case WD_MFS100DN.IMAGE_FORMAT.NONE :
            {
               cbFrontSideImage1Format.SelectedIndex = 0;
               break;
            }
            case WD_MFS100DN.IMAGE_FORMAT.BMP :
            {
               cbFrontSideImage1Format.SelectedIndex = 1;
               break;
            }
            case WD_MFS100DN.IMAGE_FORMAT.TIFF_G4 :
            {
               cbFrontSideImage1Format.SelectedIndex = 2;
               break;
            }
            case WD_MFS100DN.IMAGE_FORMAT.JPEG :
            {
               cbFrontSideImage1Format.SelectedIndex = 3;
               break;
            }
         }
         // Preset Scanning - Front side image 1 - JPEG quality
         tbFrontSideImage1JpegQuality.Text = 
                      tConfig.tScanning.tFrontSide1.iJpegQuality.ToString();
         // Preset Scanning - Front side image 1 + 2 - Color
         cbFrontSideImage1Color.DropDownStyle = ComboBoxStyle.DropDownList;
         tConfig.tScanning.tFrontSide2.eColor = 
                                       tConfig.tScanning.tFrontSide1.eColor;
         switch (tConfig.tScanning.tFrontSide1.eColor)
         {
            case CFG.IMAGE_COLOR.RED :
            {
               cbFrontSideImage1Color.SelectedIndex = 0;
               cbFrontSideImage2Color.SelectedIndex = 0;
               break;
            }
            case CFG.IMAGE_COLOR.GREEN :
            {
               cbFrontSideImage1Color.SelectedIndex = 1;
               cbFrontSideImage2Color.SelectedIndex = 1;
               break;
            }
            case CFG.IMAGE_COLOR.BLUE :
            {
               cbFrontSideImage1Color.SelectedIndex = 2;
               cbFrontSideImage2Color.SelectedIndex = 2;
               break;
            }
            case CFG.IMAGE_COLOR.COLOR :
            {
               cbFrontSideImage1Color.SelectedIndex = 3;
               cbFrontSideImage2Color.SelectedIndex = 3;
               break;
            }
         }
         // Preset Scanning - Front side image 1 - DPI
         cbFrontSideImage1Dpi.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tScanning.tFrontSide1.eDpi)
         {
            case CFG.IMAGE_DPI._100 :
            {
               cbFrontSideImage1Dpi.SelectedIndex = 0;
               tConfig.tScanning.tFrontSide2.eDpi = CFG.IMAGE_DPI._100;
               break;
            }
            case CFG.IMAGE_DPI._200 :
            {
               cbFrontSideImage1Dpi.SelectedIndex = 1;
               break;
            }
         }
         // Preset Scanning - Front side image 1 + 2 - Cut document
         tConfig.tScanning.tFrontSide2.bCut =
                                         tConfig.tScanning.tFrontSide1.bCut;
         if (tConfig.tScanning.tFrontSide1.bCut == true)
         {
            chkbFrontSideImage1CutDoc.Checked = true;
            chkbFrontSideImage2CutDoc.Checked = true;
         }
         else
         {
            chkbFrontSideImage1CutDoc.Checked = false;
            chkbFrontSideImage2CutDoc.Checked = false;
         }
         // Preset Scanning - Front side image 2 - File name
         tbFrontSideImage2FileName.Text = 
                        tConfig.tScanning.tFrontSide2.sbFileName.ToString();
         // Preset Scanning - Front side image 2 - Format
         cbFrontSideImage2Format.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tScanning.tFrontSide2.eFormat)
         {
            case WD_MFS100DN.IMAGE_FORMAT.NONE :
            {
               cbFrontSideImage2Format.SelectedIndex = 0;
               break;
            }
            case WD_MFS100DN.IMAGE_FORMAT.BMP :
            {
               cbFrontSideImage2Format.SelectedIndex = 1;
               break;
            }
            case WD_MFS100DN.IMAGE_FORMAT.TIFF_G4 :
            {
               cbFrontSideImage2Format.SelectedIndex = 2;
               break;
            }
            case WD_MFS100DN.IMAGE_FORMAT.JPEG :
            {
               cbFrontSideImage2Format.SelectedIndex = 3;
               break;
            }
         }
         // Preset Scanning - Front side image 2 - JPEG quality
         tbFrontSideImage2JpegQuality.Text = 
                      tConfig.tScanning.tFrontSide2.iJpegQuality.ToString();
         // Preset Scanning - Front side image 2 - Color is not necessary 
         // since color setting of 2nd image must be the same as 1st image
         // Preset Scanning - Front side image 2 - DPI
         // Remark: If 1st image is set to 100 DPI, 2nd image is fixed to 
         //         100 DPI too
         cbFrontSideImage2Dpi.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tScanning.tFrontSide2.eDpi)
         {
            case CFG.IMAGE_DPI._100 :
            {
               cbFrontSideImage2Dpi.SelectedIndex = 0;
               break;
            }
            case CFG.IMAGE_DPI._200 :
            {
               cbFrontSideImage2Dpi.SelectedIndex = 1;
               break;
            }
         }
         // Preset Scanning - Front side image 2 - Cut document is not 
         // necessary since cut threshold setting of 2nd image must be the 
         // same as 1st image
         // Preset Scanning - Rear side image 1 - File name
         tbRearSideImage1FileName.Text = 
                         tConfig.tScanning.tRearSide1.sbFileName.ToString();
         // Preset Scanning - Rear side image 1 - Format
         cbRearSideImage1Format.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tScanning.tRearSide1.eFormat)
         {
            case WD_MFS100DN.IMAGE_FORMAT.NONE :
            {
               cbRearSideImage1Format.SelectedIndex = 0;
               break;
            }
            case WD_MFS100DN.IMAGE_FORMAT.BMP :
            {
               cbRearSideImage1Format.SelectedIndex = 1;
               break;
            }
            case WD_MFS100DN.IMAGE_FORMAT.TIFF_G4 :
            {
               cbRearSideImage1Format.SelectedIndex = 2;
               break;
            }
            case WD_MFS100DN.IMAGE_FORMAT.JPEG :
            {
               cbRearSideImage1Format.SelectedIndex = 3;
               break;
            }
         }
         // Preset Scanning - Rear side image 1 - JPEG quality
         tbRearSideImage1JpegQuality.Text = 
                       tConfig.tScanning.tRearSide1.iJpegQuality.ToString();
         // Preset Scanning - Rear side image 1 + 2 - Color
         cbRearSideImage1Color.DropDownStyle = ComboBoxStyle.DropDownList;
         tConfig.tScanning.tRearSide2.eColor = 
                                        tConfig.tScanning.tRearSide1.eColor;
         switch (tConfig.tScanning.tRearSide1.eColor)
         {
            case CFG.IMAGE_COLOR.RED :
            {
               cbRearSideImage1Color.SelectedIndex = 0;
               cbRearSideImage2Color.SelectedIndex = 0;
               break;
            }
            case CFG.IMAGE_COLOR.GREEN :
            {
               cbRearSideImage1Color.SelectedIndex = 1;
               cbRearSideImage2Color.SelectedIndex = 1;
               break;
            }
            case CFG.IMAGE_COLOR.BLUE :
            {
               cbRearSideImage1Color.SelectedIndex = 2;
               cbRearSideImage2Color.SelectedIndex = 2;
               break;
            }
            case CFG.IMAGE_COLOR.COLOR :
            {
               cbRearSideImage1Color.SelectedIndex = 3;
               cbRearSideImage2Color.SelectedIndex = 3;
               break;
            }
         }
         // Preset Scanning - Rear side image 1 - DPI
         cbRearSideImage1Dpi.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tScanning.tRearSide1.eDpi)
         {
            case CFG.IMAGE_DPI._100 :
            {
               cbRearSideImage1Dpi.SelectedIndex = 0;
               tConfig.tScanning.tRearSide2.eDpi = CFG.IMAGE_DPI._100;
               break;
            }
            case CFG.IMAGE_DPI._200 :
            {
               cbRearSideImage1Dpi.SelectedIndex = 1;
               break;
            }
         }
         // Preset Scanning - Rear side image 1 + 2 - Cut document
         tConfig.tScanning.tRearSide2.bCut =
                                          tConfig.tScanning.tRearSide1.bCut;
         if (tConfig.tScanning.tRearSide1.bCut == true)
         {
            chkbRearSideImage1CutDoc.Checked = true;
            chkbRearSideImage2CutDoc.Checked = true;
         }
         else
         {
            chkbRearSideImage1CutDoc.Checked = false;
            chkbRearSideImage2CutDoc.Checked = false;
         }
         // Preset Scanning - Rear side image 2 - File name
         tbRearSideImage2FileName.Text = 
                         tConfig.tScanning.tRearSide2.sbFileName.ToString();
         // Preset Scanning - Rear side image 2 - Format
         cbRearSideImage2Format.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tScanning.tRearSide2.eFormat)
         {
            case WD_MFS100DN.IMAGE_FORMAT.NONE :
            {
               cbRearSideImage2Format.SelectedIndex = 0;
               break;
            }
            case WD_MFS100DN.IMAGE_FORMAT.BMP :
            {
               cbRearSideImage2Format.SelectedIndex = 1;
               break;
            }
            case WD_MFS100DN.IMAGE_FORMAT.TIFF_G4 :
            {
               cbRearSideImage2Format.SelectedIndex = 2;
               break;
            }
            case WD_MFS100DN.IMAGE_FORMAT.JPEG :
            {
               cbRearSideImage2Format.SelectedIndex = 3;
               break;
            }
         }
         // Preset Scanning - Rear side image 2 - JPEG quality
         tbRearSideImage2JpegQuality.Text = 
                       tConfig.tScanning.tRearSide2.iJpegQuality.ToString();
         // Preset Scanning - Rear side image 2 - Color is not necessary 
         // since color setting of 2nd image must be the same as 1st image
         // Preset Scanning - Rear side image 2 - DPI
         // Remark: If 1st image is set to 100 DPI, 2nd image is fixed to 
         //         100 DPI too
         cbRearSideImage2Dpi.DropDownStyle = ComboBoxStyle.DropDownList;
         switch (tConfig.tScanning.tRearSide2.eDpi)
         {
            case CFG.IMAGE_DPI._100 :
            {
               cbRearSideImage2Dpi.SelectedIndex = 0;
               break;
            }
            case CFG.IMAGE_DPI._200 :
            {
               cbRearSideImage2Dpi.SelectedIndex = 1;
               break;
            }
         }
         // Preset Scanning - Rear side image 2 - Cut document is not 
         // necessary since cut threshold setting of 2nd image must be the 
         // same as 1st image
      }

      private void cbFeedSource_SelectedIndexChanged(object sender,
                                                                EventArgs e)
      {
         switch (cbFeedSource.SelectedIndex)
         {
            case 0 :
            {
               // CFG.FEED_SOURCE.A6_NORMAL_SINGLE
               lblPipeline.Enabled = false;
               cbPipeline.Enabled = false;
               lblFeedTimeout1.Enabled = false;
               tbFeedTimeout.Enabled = false;
               lblFeedTimeout2.Enabled = false;
               lblDoublefeedThreshold.Enabled = true;
               tbDoublefeedThreshold.Enabled = true;
               // Enable reading
               lblReadFont.Enabled = true;
               cbReadFont.Enabled = true;
               // Enable encoding
               lblEndoMode.Enabled = true;
               cbEndoMode.Enabled = true;
               break;
            }
            case 1 :
            {
               // CFG.FEED_SOURCE.A6_NORMAL_BATCH
               lblPipeline.Enabled = true;
               cbPipeline.Enabled = true;
               lblFeedTimeout1.Enabled = true;
               tbFeedTimeout.Enabled = true;
               lblFeedTimeout2.Enabled = true;
               lblDoublefeedThreshold.Enabled = true;
               tbDoublefeedThreshold.Enabled = true;
               // Enable reading
               lblReadFont.Enabled = true;
               cbReadFont.Enabled = true;
               // Enable encoding
               lblEndoMode.Enabled = true;
               cbEndoMode.Enabled = true;
               break;
            }
            case 2 :
            {
               // CFG.FEED_SOURCE.A6_STRAIGHT_SINGLE
               lblPipeline.Enabled = false;
               cbPipeline.Enabled = false;
               lblFeedTimeout1.Enabled = false;
               tbFeedTimeout.Enabled = false;
               lblFeedTimeout2.Enabled = false;
               lblDoublefeedThreshold.Enabled = false;
               tbDoublefeedThreshold.Enabled = false;
               // Disable reading
               lblReadFont.Enabled = false;
               cbReadFont.Enabled = false;
               cbReadFont.SelectedIndex = 0;
               cbReadFont_SelectedIndexChanged(sender, e);
               // Disable encoding
               lblEndoMode.Enabled = false;
               cbEndoMode.Enabled = false;
               cbEndoMode.SelectedIndex = 0;
               cbEndoMode_SelectedIndexChanged(sender, e);
               break;
            }
            case 3 :
            {
               // CFG.FEED_SOURCE.A4_SINGLE
               lblPipeline.Enabled = false;
               cbPipeline.Enabled = false;
               lblFeedTimeout1.Enabled = false;
               tbFeedTimeout.Enabled = false;
               lblFeedTimeout2.Enabled = false;
               lblDoublefeedThreshold.Enabled = false;
               tbDoublefeedThreshold.Enabled = false;
               // Disable reading
               lblReadFont.Enabled = false;
               cbReadFont.Enabled = false;
               cbReadFont.SelectedIndex = 0;
               cbReadFont_SelectedIndexChanged(sender, e);
               // Disable encoding
               lblEndoMode.Enabled = false;
               cbEndoMode.Enabled = false;
               cbEndoMode.SelectedIndex = 0;
               cbEndoMode_SelectedIndexChanged(sender, e);
               break;
            }
            case 5:
            {
                // CFG.FEED_SOURCE.RED_ROSE_SINGLE
                lblPipeline.Enabled = false;
                cbPipeline.Enabled = false;
                lblFeedTimeout1.Enabled = false;
                tbFeedTimeout.Enabled = false;
                lblFeedTimeout2.Enabled = false;
                lblDoublefeedThreshold.Enabled = true;
                tbDoublefeedThreshold.Enabled = true;
                // Enable reading
                lblReadFont.Enabled = true;
                cbReadFont.Enabled = true;
                // Enable encoding
                lblEndoMode.Enabled = true;
                cbEndoMode.Enabled = true;
                break;
            }
            case 6:
            {
                // CFG.FEED_SOURCE.RED_ROSE_BATCH
                lblPipeline.Enabled = true;
                cbPipeline.Enabled = true;
                lblFeedTimeout1.Enabled = true;
                tbFeedTimeout.Enabled = true;
                lblFeedTimeout2.Enabled = true;
                lblDoublefeedThreshold.Enabled = true;
                tbDoublefeedThreshold.Enabled = true;
                // Enable reading
                lblReadFont.Enabled = true;
                cbReadFont.Enabled = true;
                // Enable encoding
                lblEndoMode.Enabled = true;
                cbEndoMode.Enabled = true;
                break;
            }
            default :
            {
               // CFG.FEED_SOURCE.A4_BATCH
               lblPipeline.Enabled = false;
               cbPipeline.Enabled = false;
               lblFeedTimeout1.Enabled = true;
               tbFeedTimeout.Enabled = true;
               lblFeedTimeout2.Enabled = true;
               lblDoublefeedThreshold.Enabled = false;
               tbDoublefeedThreshold.Enabled = false;
               // Disable reading
               lblReadFont.Enabled = false;
               cbReadFont.Enabled = false;
               cbReadFont.SelectedIndex = 0;
               cbReadFont_SelectedIndexChanged(sender, e);
               // Disable encoding
               lblEndoMode.Enabled = false;
               cbEndoMode.Enabled = false;
               cbEndoMode.SelectedIndex = 0;
               cbEndoMode_SelectedIndexChanged(sender, e);
               break;
            }
         }
      }

      private void tbFeedTimeout_TextChanged(object sender, EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
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

      private void tbDoublefeedThreshold_TextChanged(object sender,
                                                                EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
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

      private void cbReadFont_SelectedIndexChanged(object sender,
                                                                EventArgs e)
      {
         switch (cbReadFont.SelectedIndex)
         {
            case 0 :
            {
               // CFG.READ_FONT.NONE
               lblMicrBlanks.Enabled = false;
               cbMicrBlanks.Enabled = false;
               lblOcrBlanks.Enabled = false;
               cbOcrBlanks.Enabled = false;
               lblSorting.Enabled = false;
               cbSorting.Enabled = false;
               lblSortPattern.Enabled = false;
               tbSortPattern.Enabled = false;
               break;
            }
            case 1 :
            case 5 :
            {
               // All pure magnetic fonts
               lblMicrBlanks.Enabled = true;
               cbMicrBlanks.Enabled = true;
               lblOcrBlanks.Enabled = false;
               cbOcrBlanks.Enabled = false;
               lblSorting.Enabled = true;
               cbSorting.Enabled = true;
               if ((cbSorting.SelectedIndex == 2) ||
                                             (cbSorting.SelectedIndex == 3))
               {
                  lblSortPattern.Enabled = true;
                  tbSortPattern.Enabled = true;
               }
               else
               {
                  lblSortPattern.Enabled = false;
                  tbSortPattern.Enabled = false;
               }
               break;
            }
            case 2 :
            case 6 :
            case 8 :
            case 9 :
            case 10 :
            case 11 :
            case 12 :
            case 13 :
            {
               // All pure optical fonts
               lblMicrBlanks.Enabled = false;
               cbMicrBlanks.Enabled = false;
               lblOcrBlanks.Enabled = true;
               cbOcrBlanks.Enabled = true;
               lblSorting.Enabled = true;
               cbSorting.Enabled = true;
               if ((cbSorting.SelectedIndex == 2) ||
                                             (cbSorting.SelectedIndex == 3))
               {
                  lblSortPattern.Enabled = true;
                  tbSortPattern.Enabled = true;
               }
               else
               {
                  lblSortPattern.Enabled = false;
                  tbSortPattern.Enabled = false;
               }
               break;
            }
            default :
            {
               // All mixed fonts
               lblMicrBlanks.Enabled = true;
               cbMicrBlanks.Enabled = true;
               lblOcrBlanks.Enabled = true;
               cbOcrBlanks.Enabled = true;
               lblSorting.Enabled = true;
               cbSorting.Enabled = true;
               if ((cbSorting.SelectedIndex == 2) ||
                                             (cbSorting.SelectedIndex == 3))
               {
                  lblSortPattern.Enabled = true;
                  tbSortPattern.Enabled = true;
               }
               else
               {
                  lblSortPattern.Enabled = false;
                  tbSortPattern.Enabled = false;
               }
               break;
            }
         }
      }

      private void cbSorting_SelectedIndexChanged(object sender,
                                                                EventArgs e)
      {
         if ((cbSorting.SelectedIndex == 2) ||
                                             (cbSorting.SelectedIndex == 3))
         {
            lblSortPattern.Enabled = true;
            tbSortPattern.Enabled = true;
         }
         else
         {
            lblSortPattern.Enabled = false;
            tbSortPattern.Enabled = false;
         }
      }

      private void cbEndoMode_SelectedIndexChanged(object sender,
                                                                EventArgs e)
      {
         switch (cbEndoMode.SelectedIndex)
         {
            case 0 :
            {
               // CFG.ENDO_MODE.NONE
               lblEndoStartCounter.Enabled = false;
               tbEndoStartCounter.Enabled = false;
               lblEndoCounterSteps.Enabled = false;
               tbEndoCounterSteps.Enabled = false;
               lblEndoFont.Enabled = false;
               tbEndoFont.Enabled = false;
               tbEndoFont.BackColor = System.Drawing.SystemColors.Control;
               btnEndoFontSelect.Enabled = false;
               btnEndoFontClear.Enabled = false;
               lblEndoPos.Enabled = false;
               tbEndoPos.Enabled = false;
               lblEndoDensity.Enabled = false;
               cbEndoDensity.Enabled = false;
               lblEndoData.Enabled = false;
               tbEndoData.Enabled = false;
               lblEndoFont.Text = "Font:";
               break;
            }
            case 1 :
            {
               // CFG.ENDO_MODE.FIX_STRING
               lblEndoStartCounter.Enabled = false;
               tbEndoStartCounter.Enabled = false;
               lblEndoCounterSteps.Enabled = false;
               tbEndoCounterSteps.Enabled = false;
               lblEndoFont.Enabled = true;
               tbEndoFont.Enabled = true;
               tbEndoFont.BackColor = System.Drawing.SystemColors.Window;
               btnEndoFontSelect.Enabled = true;
               btnEndoFontClear.Enabled = true;
               lblEndoPos.Enabled = true;
               tbEndoPos.Enabled = true;
               lblEndoDensity.Enabled = true;
               cbEndoDensity.Enabled = true;
               lblEndoData.Enabled = true;
               tbEndoData.Enabled = true;
               lblEndoFont.Text = "Font:";
               break;
            }
            case 2 :
            {
               // CFG.ENDO_MODE.STRING_WITH_COUNTER
               lblEndoStartCounter.Enabled = true;
               tbEndoStartCounter.Enabled = true;
               lblEndoCounterSteps.Enabled = true;
               tbEndoCounterSteps.Enabled = true;
               lblEndoFont.Enabled = true;
               tbEndoFont.Enabled = true;
               tbEndoFont.BackColor = System.Drawing.SystemColors.Window;
               btnEndoFontSelect.Enabled = true;
               btnEndoFontClear.Enabled = true;
               lblEndoPos.Enabled = true;
               tbEndoPos.Enabled = true;
               lblEndoDensity.Enabled = true;
               cbEndoDensity.Enabled = true;
               lblEndoData.Enabled = true;
               tbEndoData.Enabled = true;
               lblEndoFont.Text = "Font:";
               break;
            }
            default :
            {
               // CFG.ENDO_MODE.BITMAP
               lblEndoStartCounter.Enabled = false;
               tbEndoStartCounter.Enabled = false;
               lblEndoCounterSteps.Enabled = false;
               tbEndoCounterSteps.Enabled = false;
               lblEndoFont.Enabled = true;
               tbEndoFont.Enabled = true;
               tbEndoFont.BackColor = System.Drawing.SystemColors.Window;
               btnEndoFontSelect.Enabled = true;
               btnEndoFontClear.Enabled = true;
               lblEndoPos.Enabled = true;
               tbEndoPos.Enabled = true;
               lblEndoDensity.Enabled = false;
               cbEndoDensity.Enabled = false;
               lblEndoData.Enabled = false;
               tbEndoData.Enabled = false;
               lblEndoFont.Text = "Bitmap:";
               break;
            }
         }
      }

      private void tbEndoStartCounter_TextChanged(object sender,
                                                                EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all digits from text
         iCounter = 0;
         while (iCounter < tbEndoStartCounter.TextLength)
         {
            if (char.IsDigit(tbEndoStartCounter.Text, iCounter) == true)
            {
               strBuffer += tbEndoStartCounter.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only digits, remove non-digit chars
         if (strBuffer != tbEndoStartCounter.Text)
         {
            tbEndoStartCounter.Text = strBuffer;
            tbEndoStartCounter.SelectionStart = strBuffer.Length;
         }
      }

      private void tbEndoCounterSteps_TextChanged(object sender,
                                                                EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all digits from text
         iCounter = 0;
         while (iCounter < tbEndoCounterSteps.TextLength)
         {
            if (char.IsDigit(tbEndoCounterSteps.Text, iCounter) == true)
            {
               strBuffer += tbEndoCounterSteps.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only digits, remove non-digit chars
         if (strBuffer != tbEndoCounterSteps.Text)
         {
            tbEndoCounterSteps.Text = strBuffer;
            tbEndoCounterSteps.SelectionStart = strBuffer.Length;
         }
      }

      private void btnEndoFontSelect_Click(object sender, EventArgs e)
      {
         OpenFileDialog  ofdGetFontFileName = new OpenFileDialog();

         // If actually no font directory is given
         if (GetFilePath(strEndoFont) == "")
         {
            // Select actual directory
            ofdGetFontFileName.InitialDirectory = 
                                  System.IO.Directory.GetCurrentDirectory();
            ofdGetFontFileName.FileName = "";
         }
         else
         {
            // Select given directory
            ofdGetFontFileName.InitialDirectory = GetFilePath(strEndoFont);
            ofdGetFontFileName.FileName = strEndoFont;
         }
         if (cbEndoMode.SelectedIndex > 2)
         {
            // Bitmap
            // Preset file dialog structure
            ofdGetFontFileName.Filter = "bitmap files (*.bmp)|*.bmp";
            ofdGetFontFileName.FilterIndex = 1;
            ofdGetFontFileName.RestoreDirectory = true;
            ofdGetFontFileName.Title = "Select bitmap file";
         }
         else
         {
            // Fix string or String + counter
            // Preset file dialog structure
            ofdGetFontFileName.Filter = "font files (*.ini)|*.ini";
            ofdGetFontFileName.FilterIndex = 1;
            ofdGetFontFileName.RestoreDirectory = true;
            ofdGetFontFileName.Title = "Select font file";
         }
         // If font or bitmap file was selected
         if (ofdGetFontFileName.ShowDialog() == DialogResult.OK)
         {
            // Update font or bitmap file in textbox (2 variables because of
            // path)
            strEndoFont = ofdGetFontFileName.FileName;
            tbEndoFont.Text = ofdGetFontFileName.SafeFileName;
         }
      }

      private void btnEndoFontClear_Click(object sender, EventArgs e)
      {
         strEndoFont = "";
         tbEndoFont.Text = "";
      }

      private void tbEndoPos_TextChanged(object sender, EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all digits from text
         iCounter = 0;
         while (iCounter < tbEndoPos.TextLength)
         {
            if (char.IsDigit(tbEndoPos.Text, iCounter) == true)
            {
               strBuffer += tbEndoPos.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only digits, remove non-digit chars
         if (strBuffer != tbEndoPos.Text)
         {
            tbEndoPos.Text = strBuffer;
            tbEndoPos.SelectionStart = strBuffer.Length;
         }
      }

      private void tbEndoData_TextChanged(object sender, EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all allowed characters from text
         iCounter = 0;
         while (iCounter < tbEndoData.TextLength)
         {
            if ((char.IsLetterOrDigit(tbEndoData.Text, iCounter) == true) ||
                  (char.IsPunctuation(tbEndoData.Text, iCounter) == true) ||
                            (tbEndoData.Text.Substring(iCounter, 1) == " "))
            {
               strBuffer += tbEndoData.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only allowed chars, remove non-allowed
         // chars
         if (strBuffer != tbEndoData.Text)
         {
            tbEndoData.Text = strBuffer;
            tbEndoData.SelectionStart = strBuffer.Length;
         }
      }

      private void tbMaxImageWidth_TextChanged(object sender, EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
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
         String  strBuffer = "";
         int  iCounter;
         
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

      private void tbTempTolerance_TextChanged(object sender, EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all digits from text
         iCounter = 0;
         while (iCounter < tbTempTolerance.TextLength)
         {
            if (char.IsDigit(tbTempTolerance.Text, iCounter) == true)
            {
               strBuffer += tbTempTolerance.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only digits, remove non-digit chars
         if (strBuffer != tbTempTolerance.Text)
         {
            tbTempTolerance.Text = strBuffer;
            tbTempTolerance.SelectionStart = strBuffer.Length;
         }
      }

      private void tbTempPeriod_TextChanged(object sender, EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all digits from text
         iCounter = 0;
         while (iCounter < tbTempPeriod.TextLength)
         {
            if (char.IsDigit(tbTempPeriod.Text, iCounter) == true)
            {
               strBuffer += tbTempPeriod.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only digits, remove non-digit chars
         if (strBuffer != tbTempPeriod.Text)
         {
            tbTempPeriod.Text = strBuffer;
            tbTempPeriod.SelectionStart = strBuffer.Length;
         }
      }

      private void btnImageDirectory_Click(object sender, EventArgs e)
      {
         FolderBrowserDialog  fbdGetImageDirectory = 
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

      private void tbFrontSideImage1FileName_TextChanged(object sender,
                                                                EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all allowed characters from text
         iCounter = 0;
         while (iCounter < tbFrontSideImage1FileName.TextLength)
         {
            if ((char.IsLetterOrDigit(tbFrontSideImage1FileName.Text,
                                                       iCounter) == true) ||
                         (tbFrontSideImage1FileName.Text.Substring(iCounter,
                                                               1) == "%") ||
                         (tbFrontSideImage1FileName.Text.Substring(iCounter,
                                                               1) == "-") ||
                         (tbFrontSideImage1FileName.Text.Substring(iCounter,
                                                                 1) == "_"))
            {
               strBuffer += 
                      tbFrontSideImage1FileName.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only allowed chars, remove non-allowed
         // chars
         if (strBuffer != tbFrontSideImage1FileName.Text)
         {
            tbFrontSideImage1FileName.Text = strBuffer;
            tbFrontSideImage1FileName.SelectionStart = strBuffer.Length;
         }
      }

      private void cbFrontSideImage1Format_SelectedIndexChanged(
                                                 object sender, EventArgs e)
      {
         switch (cbFrontSideImage1Format.SelectedIndex)
         {
            case 0 :
            {
               // CFG.IMAGE_FORMAT.NONE
               lblFrontSideImage1FileName.Enabled = false;
               tbFrontSideImage1FileName.Enabled = false;
               lblFrontSideImage1JpegQuality.Enabled = false;
               tbFrontSideImage1JpegQuality.Enabled = false;
               lblFrontSideImage1Color.Enabled = false;
               cbFrontSideImage1Color.Enabled = false;
               lblFrontSideImage1Dpi.Enabled = false;
               cbFrontSideImage1Dpi.Enabled = false;
               chkbFrontSideImage1CutDoc.Enabled = false;
               // Disable front side image 2
               lblFrontSideImage2Format.Enabled = false;
               cbFrontSideImage2Format.Enabled = false;
               cbFrontSideImage2Format.SelectedIndex = 0;
               cbFrontSideImage2Format_SelectedIndexChanged(sender, e);
               break;
            }
            case 1 :
            case 2 :
            {
               // CFG.IMAGE_FORMAT.BMP
               // CFG.IMAGE_FORMAT.TIFF_G4
               lblFrontSideImage1FileName.Enabled = true;
               tbFrontSideImage1FileName.Enabled = true;
               lblFrontSideImage1JpegQuality.Enabled = false;
               tbFrontSideImage1JpegQuality.Enabled = false;
               lblFrontSideImage1Color.Enabled = true;
               cbFrontSideImage1Color.Enabled = true;
               lblFrontSideImage1Dpi.Enabled = true;
               cbFrontSideImage1Dpi.Enabled = true;
               chkbFrontSideImage1CutDoc.Enabled = true;
               // Enable front side image 2
               lblFrontSideImage2Format.Enabled = true;
               cbFrontSideImage2Format.Enabled = true;
               break;
            }
            default :
            {
               // CFG.IMAGE_FORMAT.JPEG
               lblFrontSideImage1FileName.Enabled = true;
               tbFrontSideImage1FileName.Enabled = true;
               lblFrontSideImage1JpegQuality.Enabled = true;
               tbFrontSideImage1JpegQuality.Enabled = true;
               lblFrontSideImage1Color.Enabled = true;
               cbFrontSideImage1Color.Enabled = true;
               lblFrontSideImage1Dpi.Enabled = true;
               cbFrontSideImage1Dpi.Enabled = true;
               chkbFrontSideImage1CutDoc.Enabled = true;
               // Enable front side image 2
               lblFrontSideImage2Format.Enabled = true;
               cbFrontSideImage2Format.Enabled = true;
               break;
            }
         }
      }

      private void tbFrontSideImage1JpegQuality_TextChanged(object sender,
                                                                EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all digits from text
         iCounter = 0;
         while (iCounter < tbFrontSideImage1JpegQuality.TextLength)
         {
            if (char.IsDigit(tbFrontSideImage1JpegQuality.Text,
                                                          iCounter) == true)
            {
               strBuffer += 
                   tbFrontSideImage1JpegQuality.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only digits, remove non-digit chars
         if (strBuffer != tbFrontSideImage1JpegQuality.Text)
         {
            tbFrontSideImage1JpegQuality.Text = strBuffer;
            tbFrontSideImage1JpegQuality.SelectionStart = strBuffer.Length;
         }
      }

      private void cbFrontSideImage1Color_SelectedIndexChanged(
                                                 object sender, EventArgs e)
      {
         // There is no independent color setting for 2nd front side
         cbFrontSideImage2Color.SelectedIndex = 
                                       cbFrontSideImage1Color.SelectedIndex;
         // If no image format is color
         if ((cbFrontSideImage1Color.SelectedIndex != 
                                             (int) CFG.IMAGE_COLOR.COLOR) &&
                                     (cbRearSideImage1Color.SelectedIndex !=
                                               (int) CFG.IMAGE_COLOR.COLOR))
         {
            // Clear true color and disable setting
            chkbTrueColor.Checked = false;
            chkbTrueColor.Enabled = false;
         }
         else
         {
            // Enable true color
            chkbTrueColor.Enabled = true;
         }
      }

      private void cbFrontSideImage1Dpi_SelectedIndexChanged(object sender,
                                                                EventArgs e)
      {
         // If 100 DPI
         if (cbFrontSideImage1Dpi.SelectedIndex == 0)
         {
            // Fix 2nd front side image to 100 DPI too
            cbFrontSideImage2Dpi.SelectedIndex = 0;
            lblFrontSideImage2Dpi.Enabled = false;
            cbFrontSideImage2Dpi.Enabled = false;
         }
         else
         {
            // 200 DPI
            // If 2nd front side image is not NONE
            if (cbFrontSideImage2Format.SelectedIndex != 0)
            {
               // Allow changes at 2nd front side image DPI setting
               lblFrontSideImage2Dpi.Enabled = true;
               cbFrontSideImage2Dpi.Enabled = true;
            }
         }
      }

      private void chkbFrontSideImage1CutDoc_CheckedChanged(object sender,
                                                                EventArgs e)
      {
         // There is no independent cut threshold setting for 2nd front side
         chkbFrontSideImage2CutDoc.Checked = 
                                          chkbFrontSideImage1CutDoc.Checked;
      }

      private void tbFrontSideImage2FileName_TextChanged(object sender,
                                                                EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all allowed characters from text
         iCounter = 0;
         while (iCounter < tbFrontSideImage2FileName.TextLength)
         {
            if ((char.IsLetterOrDigit(tbFrontSideImage2FileName.Text,
                                                       iCounter) == true) ||
                         (tbFrontSideImage2FileName.Text.Substring(iCounter,
                                                               1) == "%") ||
                         (tbFrontSideImage2FileName.Text.Substring(iCounter,
                                                               1) == "-") ||
                         (tbFrontSideImage2FileName.Text.Substring(iCounter,
                                                                 1) == "_"))
            {
               strBuffer += 
                      tbFrontSideImage2FileName.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only allowed chars, remove non-allowed
         // chars
         if (strBuffer != tbFrontSideImage2FileName.Text)
         {
            tbFrontSideImage2FileName.Text = strBuffer;
            tbFrontSideImage2FileName.SelectionStart = strBuffer.Length;
         }
      }

      private void cbFrontSideImage2Format_SelectedIndexChanged(
                                                 object sender, EventArgs e)
      {
         switch (cbFrontSideImage2Format.SelectedIndex)
         {
            case 0 :
            {
               // CFG.IMAGE_FORMAT.NONE
               lblFrontSideImage2FileName.Enabled = false;
               tbFrontSideImage2FileName.Enabled = false;
               lblFrontSideImage2JpegQuality.Enabled = false;
               tbFrontSideImage2JpegQuality.Enabled = false;
               lblFrontSideImage2Dpi.Enabled = false;
               cbFrontSideImage2Dpi.Enabled = false;
               break;
            }
            case 1 :
            case 2 :
            {
               // CFG.IMAGE_FORMAT.BMP
               // CFG.IMAGE_FORMAT.TIFF_G4
               lblFrontSideImage2FileName.Enabled = true;
               tbFrontSideImage2FileName.Enabled = true;
               lblFrontSideImage2JpegQuality.Enabled = false;
               tbFrontSideImage2JpegQuality.Enabled = false;
               // If 1st front side image is not set to 100 DPI
               if (cbFrontSideImage1Dpi.SelectedIndex != 0)
               {
                  lblFrontSideImage2Dpi.Enabled = true;
                  cbFrontSideImage2Dpi.Enabled = true;
               }
               break;
            }
            default :
            {
               // CFG.IMAGE_FORMAT.JPEG
               lblFrontSideImage2FileName.Enabled = true;
               tbFrontSideImage2FileName.Enabled = true;
               lblFrontSideImage2JpegQuality.Enabled = true;
               tbFrontSideImage2JpegQuality.Enabled = true;
               // If 1st front side image is not set to 100 DPI
               if (cbFrontSideImage1Dpi.SelectedIndex != 0)
               {
                  lblFrontSideImage2Dpi.Enabled = true;
                  cbFrontSideImage2Dpi.Enabled = true;
               }
               break;
            }
         }
      }

      private void tbFrontSideImage2JpegQuality_TextChanged(object sender,
                                                                EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all digits from text
         iCounter = 0;
         while (iCounter < tbFrontSideImage2JpegQuality.TextLength)
         {
            if (char.IsDigit(tbFrontSideImage2JpegQuality.Text,
                                                          iCounter) == true)
            {
               strBuffer += 
                   tbFrontSideImage2JpegQuality.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only digits, remove non-digit chars
         if (strBuffer != tbFrontSideImage2JpegQuality.Text)
         {
            tbFrontSideImage2JpegQuality.Text = strBuffer;
            tbFrontSideImage2JpegQuality.SelectionStart = strBuffer.Length;
         }
      }

      private void tbRearSideImage1FileName_TextChanged(object sender,
                                                                EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all allowed characters from text
         iCounter = 0;
         while (iCounter < tbRearSideImage1FileName.TextLength)
         {
            if ((char.IsLetterOrDigit(tbRearSideImage1FileName.Text,
                                                       iCounter) == true) ||
                          (tbRearSideImage1FileName.Text.Substring(iCounter,
                                                               1) == "%") ||
                          (tbRearSideImage1FileName.Text.Substring(iCounter,
                                                               1) == "-") ||
                          (tbRearSideImage1FileName.Text.Substring(iCounter,
                                                                 1) == "_"))
            {
               strBuffer += 
                       tbRearSideImage1FileName.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only allowed chars, remove non-allowed
         // chars
         if (strBuffer != tbRearSideImage1FileName.Text)
         {
            tbRearSideImage1FileName.Text = strBuffer;
            tbRearSideImage1FileName.SelectionStart = strBuffer.Length;
         }
      }

      private void cbRearSideImage1Format_SelectedIndexChanged(
                                                 object sender, EventArgs e)
      {
         switch (cbRearSideImage1Format.SelectedIndex)
         {
            case 0 :
            {
               // CFG.IMAGE_FORMAT.NONE
               lblRearSideImage1FileName.Enabled = false;
               tbRearSideImage1FileName.Enabled = false;
               lblRearSideImage1JpegQuality.Enabled = false;
               tbRearSideImage1JpegQuality.Enabled = false;
               lblRearSideImage1Color.Enabled = false;
               cbRearSideImage1Color.Enabled = false;
               lblRearSideImage1Dpi.Enabled = false;
               cbRearSideImage1Dpi.Enabled = false;
               chkbRearSideImage1CutDoc.Enabled = false;
               // Disable rear side image 2
               lblRearSideImage2Format.Enabled = false;
               cbRearSideImage2Format.Enabled = false;
               cbRearSideImage2Format.SelectedIndex = 0;
               cbRearSideImage2Format_SelectedIndexChanged(sender, e);
               break;
            }
            case 1 :
            case 2 :
            {
               // CFG.IMAGE_FORMAT.BMP
               // CFG.IMAGE_FORMAT.TIFF_G4
               lblRearSideImage1FileName.Enabled = true;
               tbRearSideImage1FileName.Enabled = true;
               lblRearSideImage1JpegQuality.Enabled = false;
               tbRearSideImage1JpegQuality.Enabled = false;
               lblRearSideImage1Color.Enabled = true;
               cbRearSideImage1Color.Enabled = true;
               lblRearSideImage1Dpi.Enabled = true;
               cbRearSideImage1Dpi.Enabled = true;
               chkbRearSideImage1CutDoc.Enabled = true;
               // Enable rear side image 2
               lblRearSideImage2Format.Enabled = true;
               cbRearSideImage2Format.Enabled = true;
               break;
            }
            default :
            {
               // CFG.IMAGE_FORMAT.JPEG
               lblRearSideImage1FileName.Enabled = true;
               tbRearSideImage1FileName.Enabled = true;
               lblRearSideImage1JpegQuality.Enabled = true;
               tbRearSideImage1JpegQuality.Enabled = true;
               lblRearSideImage1Color.Enabled = true;
               cbRearSideImage1Color.Enabled = true;
               lblRearSideImage1Dpi.Enabled = true;
               cbRearSideImage1Dpi.Enabled = true;
               chkbRearSideImage1CutDoc.Enabled = true;
               // Enable rear side image 2
               lblRearSideImage2Format.Enabled = true;
               cbRearSideImage2Format.Enabled = true;
               break;
            }
         }
      }

      private void tbRearSideImage1JpegQuality_TextChanged(object sender,
                                                                EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all digits from text
         iCounter = 0;
         while (iCounter < tbRearSideImage1JpegQuality.TextLength)
         {
            if (char.IsDigit(tbRearSideImage1JpegQuality.Text,
                                                          iCounter) == true)
            {
               strBuffer += 
                    tbRearSideImage1JpegQuality.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only digits, remove non-digit chars
         if (strBuffer != tbRearSideImage1JpegQuality.Text)
         {
            tbRearSideImage1JpegQuality.Text = strBuffer;
            tbRearSideImage1JpegQuality.SelectionStart = strBuffer.Length;
         }
      }

      private void cbRearSideImage1Color_SelectedIndexChanged(object sender,
                                                                EventArgs e)
      {
         // There is no independent color setting for 2nd rear side
         cbRearSideImage2Color.SelectedIndex = 
                                        cbRearSideImage1Color.SelectedIndex;
         // If no image format is color
         if ((cbFrontSideImage1Color.SelectedIndex != 
                                             (int) CFG.IMAGE_COLOR.COLOR) &&
                                     (cbRearSideImage1Color.SelectedIndex !=
                                               (int) CFG.IMAGE_COLOR.COLOR))
         {
            // Clear true color and disable setting
            chkbTrueColor.Checked = false;
            chkbTrueColor.Enabled = false;
         }
         else
         {
            // Enable true color
            chkbTrueColor.Enabled = true;
         }
      }

      private void cbRearSideImage1Dpi_SelectedIndexChanged(object sender,
                                                                EventArgs e)
      {
         // If 100 DPI
         if (cbRearSideImage1Dpi.SelectedIndex == 0)
         {
            // Fix 2nd rear side image to 100 DPI too
            cbRearSideImage2Dpi.SelectedIndex = 0;
            lblRearSideImage2Dpi.Enabled = false;
            cbRearSideImage2Dpi.Enabled = false;
         }
         else
         {
            // 200 DPI
            // If 2nd rear side image is not NONE
            if (cbRearSideImage2Format.SelectedIndex != 0)
            {
               // Allow changes at 2nd rear side image DPI setting
               lblRearSideImage2Dpi.Enabled = true;
               cbRearSideImage2Dpi.Enabled = true;
            }
         }
      }

      private void chkbRearSideImage1CutDoc_CheckedChanged(object sender,
                                                                EventArgs e)
      {
         // There is no independent cut threshold setting for 2nd rear side
         chkbRearSideImage2CutDoc.Checked = 
                                           chkbRearSideImage1CutDoc.Checked;
      }

      private void tbRearSideImage2FileName_TextChanged(object sender,
                                                                EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all allowed characters from text
         iCounter = 0;
         while (iCounter < tbRearSideImage2FileName.TextLength)
         {
            if ((char.IsLetterOrDigit(tbRearSideImage2FileName.Text,
                                                       iCounter) == true) ||
                          (tbRearSideImage2FileName.Text.Substring(iCounter,
                                                               1) == "%") ||
                          (tbRearSideImage2FileName.Text.Substring(iCounter,
                                                               1) == "-") ||
                          (tbRearSideImage2FileName.Text.Substring(iCounter,
                                                                 1) == "_"))
            {
               strBuffer += 
                       tbRearSideImage2FileName.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only allowed chars, remove non-allowed
         // chars
         if (strBuffer != tbRearSideImage2FileName.Text)
         {
            tbRearSideImage2FileName.Text = strBuffer;
            tbRearSideImage2FileName.SelectionStart = strBuffer.Length;
         }
      }

      private void cbRearSideImage2Format_SelectedIndexChanged(
                                                 object sender, EventArgs e)
      {
         switch (cbRearSideImage2Format.SelectedIndex)
         {
            case 0 :
            {
               // CFG.IMAGE_FORMAT.NONE
               lblRearSideImage2FileName.Enabled = false;
               tbRearSideImage2FileName.Enabled = false;
               lblRearSideImage2JpegQuality.Enabled = false;
               tbRearSideImage2JpegQuality.Enabled = false;
               lblRearSideImage2Dpi.Enabled = false;
               cbRearSideImage2Dpi.Enabled = false;
               break;
            }
            case 1 :
            case 2 :
            {
               // CFG.IMAGE_FORMAT.BMP
               // CFG.IMAGE_FORMAT.TIFF_G4
               lblRearSideImage2FileName.Enabled = true;
               tbRearSideImage2FileName.Enabled = true;
               lblRearSideImage2JpegQuality.Enabled = false;
               tbRearSideImage2JpegQuality.Enabled = false;
               // If 1st rear side image is not set to 100 DPI
               if (cbRearSideImage1Dpi.SelectedIndex != 0)
               {
                  lblRearSideImage2Dpi.Enabled = true;
                  cbRearSideImage2Dpi.Enabled = true;
               }
               break;
            }
            default :
            {
               // CFG.IMAGE_FORMAT.JPEG
               lblRearSideImage2FileName.Enabled = true;
               tbRearSideImage2FileName.Enabled = true;
               lblRearSideImage2JpegQuality.Enabled = true;
               tbRearSideImage2JpegQuality.Enabled = true;
               // If 1st rear side image is not set to 100 DPI
               if (cbRearSideImage1Dpi.SelectedIndex != 0)
               {
                  lblRearSideImage2Dpi.Enabled = true;
                  cbRearSideImage2Dpi.Enabled = true;
               }
               break;
            }
         }
      }

      private void tbRearSideImage2JpegQuality_TextChanged(object sender,
                                                                EventArgs e)
      {
         String  strBuffer = "";
         int  iCounter;
         
         // Extract all digits from text
         iCounter = 0;
         while (iCounter < tbRearSideImage2JpegQuality.TextLength)
         {
            if (char.IsDigit(tbRearSideImage2JpegQuality.Text,
                                                          iCounter) == true)
            {
               strBuffer += 
                    tbRearSideImage2JpegQuality.Text.Substring(iCounter, 1);
            }
            ++iCounter;
         }
         // If text contains not only digits, remove non-digit chars
         if (strBuffer != tbRearSideImage2JpegQuality.Text)
         {
            tbRearSideImage2JpegQuality.Text = strBuffer;
            tbRearSideImage2JpegQuality.SelectionStart = strBuffer.Length;
         }
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         // Get value of Feeding - Source
         switch (cbFeedSource.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tFeeding.eSource = CFG.FEED_SOURCE.A6_NORMAL_SINGLE;
               break;
            }
            case 1 :
            {
               tConfig.tFeeding.eSource = CFG.FEED_SOURCE.A6_NORMAL_BATCH;
               break;
            }
            case 2 :
            {
               tConfig.tFeeding.eSource = CFG.FEED_SOURCE.A6_STRAIGHT_SINGLE;
               break;
            }
            case 3 :
            {
               tConfig.tFeeding.eSource = CFG.FEED_SOURCE.A4_SINGLE;
               break;
            }
            case 4 :
            {
               tConfig.tFeeding.eSource = CFG.FEED_SOURCE.A4_BATCH;
               break;
            }
            case 5:
            {
                tConfig.tFeeding.eSource = CFG.FEED_SOURCE.RED_ROSE_SINGLE;
                break;
            }
            case 6:
            {
                tConfig.tFeeding.eSource = CFG.FEED_SOURCE.RED_ROSE_BATCH;
                break;
            }
         }
         // Get value of Feeding - Pipeline
         switch (cbPipeline.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tFeeding.bPipelineEnabled = true;
               break;
            }
            case 1 :
            {
               tConfig.tFeeding.bPipelineEnabled = false;
               break;
            }
         }
         // Get value of Feeding - Timeout
         tConfig.tFeeding.iTimeout = Convert.ToInt32(tbFeedTimeout.Text);
         // Get value of Feeding - Doublefeed threshold
         tConfig.tFeeding.iDoublefeedThreshold = 
                                Convert.ToInt32(tbDoublefeedThreshold.Text);
         // Get value of Reading - Font
         switch (cbReadFont.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.NONE;
               break;
            }
            case 1 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.CMC7_MAGNETIC;
               break;
            }
            case 2 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.CMC7_OPTICAL;
               break;
            }
            case 3 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.CMC7_AND_OCRA;
               break;
            }
            case 4 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.CMC7_AND_OCRB;
               break;
            }
            case 5 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.E13B_MAGNETIC;
               break;
            }
            case 6 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.E13B_OPTICAL;
               break;
            }
            case 7 :
            {
               tConfig.tReading.eFont = 
                                    CFG.READ_FONT.E13B_MAGNETIC_AND_OPTICAL;
               break;
            }
            case 8 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.OCRA;
               break;
            }
            case 9 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.OCRA_ALPHA;
               break;
            }
            case 10 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.OCRA_AND_OCRB;
               break;
            }
            case 11 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.OCRB;
               break;
            }
            case 12 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.OCRB_ALPHA;
               break;
            }
            case 13 :
            {
               tConfig.tReading.eFont = CFG.READ_FONT.OCRB_UK;
               break;
            }
         }
         // Get value of Reading - MICR blanks
         switch (cbMicrBlanks.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NONE;
               break;
            }
            case 1 :
            {
               tConfig.tReading.eMicrBlanks = 
                                           CFG.READ_BLANKMODE._1_BLANK_ONLY;
               break;
            }
            case 2 :
            {
               tConfig.tReading.eMicrBlanks = CFG.READ_BLANKMODE.NORMAL;
               break;
            }
         }
         // Get value of Reading - OCR blanks
         switch (cbOcrBlanks.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NONE;
               break;
            }
            case 1 :
            {
               tConfig.tReading.eOcrBlanks = 
                                           CFG.READ_BLANKMODE._1_BLANK_ONLY;
               break;
            }
            case 2 :
            {
               tConfig.tReading.eOcrBlanks = CFG.READ_BLANKMODE.NORMAL;
               break;
            }
         }
         // Get value of Reading - Sorting
         switch (cbSorting.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tReading.eSortMode = CFG.READ_SORTMODE.NONE;
               break;
            }
            case 1 :
            {
               tConfig.tReading.eSortMode = CFG.READ_SORTMODE.MICR_GOOD_BAD;
               break;
            }
            case 2 :
            {
               tConfig.tReading.eSortMode = 
                                    CFG.READ_SORTMODE.SORT_PATTERN_POSITIVE;
               break;
            }
            case 3 :
            {
               tConfig.tReading.eSortMode = 
                                    CFG.READ_SORTMODE.SORT_PATTERN_NEGATIVE;
               break;
            }
         }
         // Get value of Reading - Sort pattern (regular expression)
         tConfig.tReading.sbSortPattern.Length = 0;
         tConfig.tReading.sbSortPattern.Insert(0, tbSortPattern.Text);
         // Get value of Endorsing - Mode
         switch (cbEndoMode.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tEndorsing.eMode = CFG.ENDO_MODE.NONE;
               break;
            }
            case 1 :
            {
               tConfig.tEndorsing.eMode = CFG.ENDO_MODE.FIX_STRING;
               break;
            }
            case 2 :
            {
               tConfig.tEndorsing.eMode = CFG.ENDO_MODE.STRING_WITH_COUNTER;
               break;
            }
            case 3 :
            {
               tConfig.tEndorsing.eMode = CFG.ENDO_MODE.BITMAP;
               break;
            }
         }
         // Get value of Endorsing - Start counter
         tConfig.tEndorsing.iStartCounter = 
                                   Convert.ToInt32(tbEndoStartCounter.Text);
         // Get value of Endorsing - Step(s)
         tConfig.tEndorsing.iSteps = 
                                   Convert.ToInt32(tbEndoCounterSteps.Text);
         // Get value of Endorsing - Font
         // Remark: string instead of textbox to get also path of font file
         tConfig.tEndorsing.sbFont.Length = 0;
         tConfig.tEndorsing.sbFont.Insert(0, strEndoFont);
         // Get value of Endorsing - Position
         tConfig.tEndorsing.iPosition = Convert.ToInt32(tbEndoPos.Text);
         // Get value of Endorsing - Density
         switch (cbEndoDensity.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tEndorsing.eDensity = CFG.ENDO_DENSITY.NORMAL;
               break;
            }
            case 1 :
            {
               tConfig.tEndorsing.eDensity = CFG.ENDO_DENSITY.BOLD;
               break;
            }
         }
         // Get value of Endorsing - Data
         tConfig.tEndorsing.sbData.Length = 0;
         tConfig.tEndorsing.sbData.Insert(0, tbEndoData.Text);
         // Get value of Scanning - Maximum image width
         tConfig.tScanning.iMaxImageWidth = 
                                      Convert.ToInt32(tbMaxImageWidth.Text);
         // Get value of Scanning - Maximum image height
         tConfig.tScanning.iMaxImageHeight = 
                                     Convert.ToInt32(tbMaxImageHeight.Text);
         // Get value of Scanning - Temperature tolerance
         tConfig.tScanning.iTempTolerance = 
                                      Convert.ToInt32(tbTempTolerance.Text);
         // Get value of Scanning - Period between temperature calibrations
         tConfig.tScanning.iTempPeriod = 
                                         Convert.ToInt32(tbTempPeriod.Text);
         // Get value of Scanning - True color
         if (chkbTrueColor.Checked == true)
         {
            tConfig.tScanning.bTrueColor = true;
         }
         else
         {
            tConfig.tScanning.bTrueColor = false;
         }
         // Get value of Scanning - Use Calibration
         if (chkbUseCalibration.Checked == true)
         {
            tConfig.tScanning.bUseCalibration = true;
         }
         else
         {
            tConfig.tScanning.bUseCalibration = false;
         }
         // Get value of Scanning - Image directory
         tConfig.tScanning.sbImageDirectory.Length = 0;
         tConfig.tScanning.sbImageDirectory.Insert(0, 
                                                     tbImageDirectory.Text);
         // Get value of Scanning - Front side image 1 - File name
         tConfig.tScanning.tFrontSide1.sbFileName.Length = 0;
         tConfig.tScanning.tFrontSide1.sbFileName.Insert(0, 
                                            tbFrontSideImage1FileName.Text);
         // Get value of Scanning - Front side image 1 - Format
         switch (cbFrontSideImage1Format.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tScanning.tFrontSide1.eFormat = 
                                              WD_MFS100DN.IMAGE_FORMAT.NONE;
               break;
            }
            case 1 :
            {
               tConfig.tScanning.tFrontSide1.eFormat = 
                                               WD_MFS100DN.IMAGE_FORMAT.BMP;
               break;
            }
            case 2 :
            {
               tConfig.tScanning.tFrontSide1.eFormat = 
                                           WD_MFS100DN.IMAGE_FORMAT.TIFF_G4;
               break;
            }
            case 3 :
            {
               tConfig.tScanning.tFrontSide1.eFormat = 
                                              WD_MFS100DN.IMAGE_FORMAT.JPEG;
               break;
            }
         }
         // Get value of Scanning - Front side image 1 - JPEG quality
         tConfig.tScanning.tFrontSide1.iJpegQuality = 
                         Convert.ToInt32(tbFrontSideImage1JpegQuality.Text);
         // Get value of Scanning - Front side image 1 - Color
         switch (cbFrontSideImage1Color.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tScanning.tFrontSide1.eColor = CFG.IMAGE_COLOR.RED;
               break;
            }
            case 1 :
            {
               tConfig.tScanning.tFrontSide1.eColor = CFG.IMAGE_COLOR.GREEN;
               break;
            }
            case 2 :
            {
               tConfig.tScanning.tFrontSide1.eColor = CFG.IMAGE_COLOR.BLUE;
               break;
            }
            case 3 :
            {
               tConfig.tScanning.tFrontSide1.eColor = CFG.IMAGE_COLOR.COLOR;
               break;
            }
         }
         // Get value of Scanning - Front side image 1 - DPI
         switch (cbFrontSideImage1Dpi.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tScanning.tFrontSide1.eDpi = CFG.IMAGE_DPI._100;
               break;
            }
            case 1 :
            {
               tConfig.tScanning.tFrontSide1.eDpi = CFG.IMAGE_DPI._200;
               break;
            }
         }
         // Get value of Scanning - Front side image 1 - Cut document
         if (chkbFrontSideImage1CutDoc.Checked == true)
         {
            tConfig.tScanning.tFrontSide1.bCut = true;
         }
         else
         {
            tConfig.tScanning.tFrontSide1.bCut = false;
         }
         // Get value of Scanning - Front side image 2 - File name
         tConfig.tScanning.tFrontSide2.sbFileName.Length = 0;
         tConfig.tScanning.tFrontSide2.sbFileName.Insert(0, 
                                            tbFrontSideImage2FileName.Text);
         // Get value of Scanning - Front side image 2 - Format
         switch (cbFrontSideImage2Format.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tScanning.tFrontSide2.eFormat = 
                                              WD_MFS100DN.IMAGE_FORMAT.NONE;
               break;
            }
            case 1 :
            {
               tConfig.tScanning.tFrontSide2.eFormat = 
                                               WD_MFS100DN.IMAGE_FORMAT.BMP;
               break;
            }
            case 2 :
            {
               tConfig.tScanning.tFrontSide2.eFormat = 
                                           WD_MFS100DN.IMAGE_FORMAT.TIFF_G4;
               break;
            }
            case 3 :
            {
               tConfig.tScanning.tFrontSide2.eFormat = 
                                              WD_MFS100DN.IMAGE_FORMAT.JPEG;
               break;
            }
         }
         // Get value of Scanning - Front side image 2 - JPEG quality
         tConfig.tScanning.tFrontSide2.iJpegQuality = 
                         Convert.ToInt32(tbFrontSideImage2JpegQuality.Text);
         // Get value of Scanning - Front side image 2 - Color
         switch (cbFrontSideImage2Color.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tScanning.tFrontSide2.eColor = CFG.IMAGE_COLOR.RED;
               break;
            }
            case 1 :
            {
               tConfig.tScanning.tFrontSide2.eColor = CFG.IMAGE_COLOR.GREEN;
               break;
            }
            case 2 :
            {
               tConfig.tScanning.tFrontSide2.eColor = CFG.IMAGE_COLOR.BLUE;
               break;
            }
            case 3 :
            {
               tConfig.tScanning.tFrontSide2.eColor = CFG.IMAGE_COLOR.COLOR;
               break;
            }
         }
         // Get value of Scanning - Front side image 2 - DPI
         switch (cbFrontSideImage2Dpi.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tScanning.tFrontSide2.eDpi = CFG.IMAGE_DPI._100;
               break;
            }
            case 1 :
            {
               tConfig.tScanning.tFrontSide2.eDpi = CFG.IMAGE_DPI._200;
               break;
            }
         }
         // Get value of Scanning - Front side image 2 - Cut document
         if (chkbFrontSideImage2CutDoc.Checked == true)
         {
            tConfig.tScanning.tFrontSide2.bCut = true;
         }
         else
         {
            tConfig.tScanning.tFrontSide2.bCut = false;
         }
         // Get value of Scanning - Rear side image 1 - File name
         tConfig.tScanning.tRearSide1.sbFileName.Length = 0;
         tConfig.tScanning.tRearSide1.sbFileName.Insert(0, 
                                             tbRearSideImage1FileName.Text);
         // Get value of Scanning - Rear side image 1 - Format
         switch (cbRearSideImage1Format.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tScanning.tRearSide1.eFormat = 
                                              WD_MFS100DN.IMAGE_FORMAT.NONE;
               break;
            }
            case 1 :
            {
               tConfig.tScanning.tRearSide1.eFormat = 
                                               WD_MFS100DN.IMAGE_FORMAT.BMP;
               break;
            }
            case 2 :
            {
               tConfig.tScanning.tRearSide1.eFormat = 
                                           WD_MFS100DN.IMAGE_FORMAT.TIFF_G4;
               break;
            }
            case 3 :
            {
               tConfig.tScanning.tRearSide1.eFormat = 
                                              WD_MFS100DN.IMAGE_FORMAT.JPEG;
               break;
            }
         }
         // Get value of Scanning - Rear side image 1 - JPEG quality
         tConfig.tScanning.tRearSide1.iJpegQuality = 
                          Convert.ToInt32(tbRearSideImage1JpegQuality.Text);
         // Get value of Scanning - Rear side image 1 - Color
         switch (cbRearSideImage1Color.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tScanning.tRearSide1.eColor = CFG.IMAGE_COLOR.RED;
               break;
            }
            case 1 :
            {
               tConfig.tScanning.tRearSide1.eColor = CFG.IMAGE_COLOR.GREEN;
               break;
            }
            case 2 :
            {
               tConfig.tScanning.tRearSide1.eColor = CFG.IMAGE_COLOR.BLUE;
               break;
            }
            case 3 :
            {
               tConfig.tScanning.tRearSide1.eColor = CFG.IMAGE_COLOR.COLOR;
               break;
            }
         }
         // Get value of Scanning - Rear side image 1 - DPI
         switch (cbRearSideImage1Dpi.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tScanning.tRearSide1.eDpi = CFG.IMAGE_DPI._100;
               break;
            }
            case 1 :
            {
               tConfig.tScanning.tRearSide1.eDpi = CFG.IMAGE_DPI._200;
               break;
            }
         }
         // Get value of Scanning - Rear side image 1 - Cut document
         if (chkbRearSideImage1CutDoc.Checked == true)
         {
            tConfig.tScanning.tRearSide1.bCut = true;
         }
         else
         {
            tConfig.tScanning.tRearSide1.bCut = false;
         }
         // Get value of Scanning - Rear side image 2 - File name
         tConfig.tScanning.tRearSide2.sbFileName.Length = 0;
         tConfig.tScanning.tRearSide2.sbFileName.Insert(0, 
                                             tbRearSideImage2FileName.Text);
         // Get value of Scanning - Rear side image 2 - Format
         switch (cbRearSideImage2Format.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tScanning.tRearSide2.eFormat = 
                                              WD_MFS100DN.IMAGE_FORMAT.NONE;
               break;
            }
            case 1 :
            {
               tConfig.tScanning.tRearSide2.eFormat = 
                                               WD_MFS100DN.IMAGE_FORMAT.BMP;
               break;
            }
            case 2 :
            {
               tConfig.tScanning.tRearSide2.eFormat = 
                                           WD_MFS100DN.IMAGE_FORMAT.TIFF_G4;
               break;
            }
            case 3 :
            {
               tConfig.tScanning.tRearSide2.eFormat = 
                                              WD_MFS100DN.IMAGE_FORMAT.JPEG;
               break;
            }
         }
         // Get value of Scanning - Rear side image 2 - JPEG quality
         tConfig.tScanning.tRearSide2.iJpegQuality = 
                          Convert.ToInt32(tbRearSideImage2JpegQuality.Text);
         // Get value of Scanning - Rear side image 2 - Color
         switch (cbRearSideImage2Color.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tScanning.tRearSide2.eColor = CFG.IMAGE_COLOR.RED;
               break;
            }
            case 1 :
            {
               tConfig.tScanning.tRearSide2.eColor = CFG.IMAGE_COLOR.GREEN;
               break;
            }
            case 2 :
            {
               tConfig.tScanning.tRearSide2.eColor = CFG.IMAGE_COLOR.BLUE;
               break;
            }
            case 3 :
            {
               tConfig.tScanning.tRearSide2.eColor = CFG.IMAGE_COLOR.COLOR;
               break;
            }
         }
         // Get value of Scanning - Rear side image 2 - DPI
         switch (cbRearSideImage2Dpi.SelectedIndex)
         {
            case 0 :
            {
               tConfig.tScanning.tRearSide2.eDpi = CFG.IMAGE_DPI._100;
               break;
            }
            case 1 :
            {
               tConfig.tScanning.tRearSide2.eDpi = CFG.IMAGE_DPI._200;
               break;
            }
         }
         // Get value of Scanning - Rear side image 2 - Cut document
         if (chkbRearSideImage2CutDoc.Checked == true)
         {
            tConfig.tScanning.tRearSide2.bCut = true;
         }
         else
         {
            tConfig.tScanning.tRearSide2.bCut = false;
         }

         if (tConfig.tScanning.tFrontSide1.sbFileName.ToString() == "" ||
                      tConfig.tScanning.tFrontSide1.sbFileName.ToString() == null)
             tConfig.tScanning.tFrontSide1.sbFileName.Insert(0, "FS-1%04d");

         // Set result to OK
         this.DialogResult = DialogResult.OK;
         //Exit setup form
         this.Close();
      }

      private void btnCancel_Click(object sender, EventArgs e)
      {
         // Set result to Cancel
         this.DialogResult = DialogResult.Cancel;
         //Exit setup form
         this.Close();
      }

      private String GetFilePath(String strPathFileName)
      {
         int  iIndex;
      
         iIndex = strPathFileName.LastIndexOf('\\');
         if (iIndex >= 0)
         {
            return(strPathFileName.Substring(0, iIndex));
         }
         else
         {
            return("");
         }
      }

      private String GetFileTitle(String strPathFileName)
      {
         int  iIndex;
      
         iIndex = strPathFileName.LastIndexOf('\\');
         if (iIndex >= 0)
         {
            return(strPathFileName.Substring(iIndex + 1));
         }
         else
         {
            return(strPathFileName);
         }
      }

      

      private void lblRearSideImage1FileName_Click(object sender, EventArgs e)
      {

      }

      private void lblReadFont_Click(object sender, EventArgs e)
      {

      }
   }
}
