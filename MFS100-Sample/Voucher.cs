using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedRose_VoucherScanner
{
    public class VoucherPack
    {
        public string vendorId;
        public List<string> barcodes;
    }

    public class statParams
    {
        public int imgCnt = 0;
        public int validCnt = 0;
        public int invCnt= 0;
        public int totalValue = 0;
        public int reusedCnt = 0;
    }

    public  class ValidationResult
    {
        public string barcode;
        public string errorMessage;
        public string value;
        public string unitType;
        public string MICR_status;
    }

    public class voucherStatistics
    {
        public int first = 0;
        public int second = 0;
        public int third = 0;
        public int fourth = 0;
        public int fifth = 0;        
    };
}
