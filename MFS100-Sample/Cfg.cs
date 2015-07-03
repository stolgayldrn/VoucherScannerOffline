using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BarcodeLib.BarcodeReader;

namespace RedRose_VoucherScanner
{
   // USE AS SEPERATE CLASS TO BE ABLE TO GET VALUES FROM SETUP-FORM BACK TO
   // MAIN FORM (A STRUCTURE IS NOT ABLE TO GIVE THE VALUES BACK) !!!
   public class CFG
   {
      public enum FEED_SOURCE
      {
         A6_NORMAL_SINGLE = 0,
         A6_NORMAL_BATCH = 1,
         A6_STRAIGHT_SINGLE = 2,
         A4_SINGLE = 3,
         A4_BATCH = 4,
         RED_ROSE_SINGLE = 5,
         RED_ROSE_BATCH = 6
      };
      public enum READ_FONT
      {
         NONE = 0,
         CMC7_MAGNETIC = 1,
         CMC7_OPTICAL = 2,
         CMC7_AND_OCRA = 3,
         CMC7_AND_OCRB = 4,
         E13B_MAGNETIC = 5,
         E13B_OPTICAL = 6,
         E13B_MAGNETIC_AND_OPTICAL = 7,
         OCRA = 8,
         OCRA_ALPHA = 9,
         OCRA_AND_OCRB = 10,
         OCRB = 11,
         OCRB_ALPHA = 12,
         OCRB_UK = 13
      };
      public enum READ_BLANKMODE
      {
         NONE = 0,
         _1_BLANK_ONLY = 1,
         NORMAL = 2
      };
      public enum READ_SORTMODE
      {
         NONE = 0,
         MICR_GOOD_BAD = 1,
         SORT_PATTERN_POSITIVE = 2,
         SORT_PATTERN_NEGATIVE = 3
      };
      public enum ENDO_MODE
      {
         NONE = 0,
         FIX_STRING = 1,
         STRING_WITH_COUNTER = 2,
         BITMAP = 3
      };
      public enum ENDO_DENSITY
      {
         NORMAL = 0,
         BOLD = 1
      };
      public enum IMAGE_COLOR
      {
         RED = 0,
         GREEN = 1,
         BLUE = 2,
         COLOR = 3
      };
      public enum IMAGE_DPI
      {
         _100 = 0,
         _200 = 1
      };

      public enum BARCODE
      {
          CODE128 = 0,
          QR = 1
      };

      public struct FEEDING
      {
         public FEED_SOURCE  eSource;
         public bool  bPipelineEnabled;
         public int  iTimeout;
         public int  iDoublefeedThreshold;
      }

      public struct READING
      {
         public READ_FONT  eFont;
         public READ_BLANKMODE  eMicrBlanks;
         public READ_BLANKMODE  eOcrBlanks;
         public READ_SORTMODE  eSortMode;
         public StringBuilder  sbSortPattern;
      }

      public struct ENDORSING
      {
         public ENDO_MODE  eMode;
         public int  iStartCounter;
         public int  iSteps;
         public StringBuilder  sbFont;
         public int  iPosition;
         public ENDO_DENSITY  eDensity;
         public StringBuilder sbData;
      }
      
      public struct IMAGE
      {
         public StringBuilder sbFileName;
         public WD_MFS100DN.IMAGE_FORMAT eFormat;
         public int  iJpegQuality;
         public IMAGE_COLOR  eColor;
         public IMAGE_DPI  eDpi;
         public bool  bCut;
      }
      
      public struct SCANNING
      {
         public int  iMaxImageWidth;
         public int  iMaxImageHeight;
         public int  iTempTolerance;
         public int  iTempPeriod;
         public bool  bTrueColor;
         public bool  bUseCalibration;
         public StringBuilder  sbImageDirectory;
         public IMAGE  tFrontSide1;
         public IMAGE  tFrontSide2;
         public IMAGE  tRearSide1;
         public IMAGE  tRearSide2;
      }

      public struct MFS_PARAMETERS
      {
         public int[]  ciIndexes;
         public StringBuilder[]  sbParameters;
      }

      public FEEDING  tFeeding;
      public READING  tReading;
      public ENDORSING  tEndorsing;
      public SCANNING  tScanning;
      public MFS_PARAMETERS  tMfsParameters;
       //Added by Tolga Yildiran
      public int tBarcode = BarcodeReader.CODE128;
      public bool MICR_Reading;
   }
}
