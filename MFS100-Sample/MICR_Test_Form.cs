using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WD_MFS100DN;
using System.IO;

namespace RedRose_VoucherScanner
{
    public partial class MICR_Test_Form : Form
    {

        private MainForm MF;
        private static statParams SP = new statParams();
        public MICR_Test_Form(MainForm parentMf)
        {
            this.MF = parentMf;
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
            InitializeComponent();
            btnPrintList.Enabled = false;
            btnPrintList.Visible = false;
            btnPrintList.Hide();
            
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            MF.MICR_Scanning();
            btnPrintList.Enabled = true;
            btnPrintList.Visible = true;
            btnPrintList.Show();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            SP = new statParams();
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
            btnPrintList.Enabled = true;
            btnPrintList.Visible = true;
            btnPrintList.Show();
            lbEvents.Items.Clear();
            lbEvents.Update();
            statisticsItemsUpdateAndShow(lbStatistics, SP);
            MF.iOldDocId = 0;
        }

        private void statisticsItemsUpdateAndShow( ListBox lbStatistics, statParams mySP)
        {
            lbStatistics.Items.Clear();
            lbStatistics.Items.Add(mySP.imgCnt.ToString());
            lbStatistics.Items.Add(mySP.validCnt.ToString());
            lbStatistics.Items.Add(mySP.invCnt.ToString());
            lbStatistics.Items.Add(mySP.reusedCnt.ToString());
            lbStatistics.Update();
        }
        private void listBoxEventItemAdd(ListBox lb, string lbItem)
        {
            lb.Items.Insert(0,lbItem);
            //lb.SelectedIndex = lb.Items.Count - 1;
            //lb.SelectedIndex = + 1;
            lb.Update();

        }

        private void btnPrintList_Click(object sender, EventArgs e)
        {
            
            List<string> MICR_list = new List<string>();
            var lines = File.ReadAllLines("codeline.txt");
            foreach (var line in lines)
            {
                MICR_list.Add(line.ToString());
                listBoxEventItemAdd(lbEvents, line);
                if(line.Length>13)
                    SP.validCnt++;
                else
                    SP.invCnt++;
                SP.imgCnt++;
            }

            statisticsItemsUpdateAndShow(lbStatistics, SP);
            btnPrintList.Enabled = false;
            btnPrintList.Visible = false;
            btnPrintList.Hide();
            
        }
        
        
    }
}
