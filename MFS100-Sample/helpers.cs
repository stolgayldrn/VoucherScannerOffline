using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedRose_VoucherScanner
{
    public class helpers
    {
        public static void barcodeCompare(string strBarcode,string strMICR, ref string bar5, ref int cnt)
        {
            string  bar1, bar2, bar3, bar4, 
                     micr1, micr2, micr3, micr4, micr5;
            bar5 = "0";
            cnt = 0;
            

            if (strBarcode.Length > 22 && strMICR.Length > 23)
            {
                bar1 = strBarcode.Substring(3, 5);
                bar2 = strBarcode.Substring(8, 7);
                bar3 = strBarcode.Substring(15, 3);
                bar4 = strBarcode.Substring(18, 3);
                bar5 = strBarcode.Substring(22, 3);

                micr1 = strMICR.Substring(1, 5);
                micr2 = strMICR.Substring(7, 7);
                micr3 = strMICR.Substring(15, 3);
                micr4 = strMICR.Substring(19, 3);
                micr5 = strMICR.Substring(23, 3);

                if (bar1 == micr1) cnt++;
                if (bar2 == micr2) cnt++;
                if (bar3 == micr3) cnt++;
                if (bar4 == micr4) cnt++;
                if (bar5 == micr5) cnt++;
            }        

        }



       
    }
    
}
