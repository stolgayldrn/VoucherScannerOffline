using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedRose_VoucherScanner
{
    public partial class VendorSelectForm : Form
    {
        private MainForm mainForm;
        private Vendor selectedVendor = null;
        public VendorSelectForm(MainForm mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
            // get vendor list
            //randomVendor(ref vendorList);
             cbVendorList.Items.Clear();
            try
            {
               List<Vendor> vendors = RestClient.GetVendors(mainForm.Username, mainForm.Password);
              
                for(int i = 0; i < vendors.Count(); i++)
                {
                    cbVendorList.Items.Add(vendors[i]);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(
                               "Could not get vendor lest due to: " + e.ToString(),
                               "Walther Data GmbH Scan-Solutions - MFS100",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (selectedVendor == null)
            {
                MessageBox.Show(
                              "User needs to select a vendor",
                              "Walther Data GmbH Scan-Solutions - MFS100",
                               MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                //Show barcode reading gui
                new ValidForm(this.mainForm, selectedVendor).Show();
                this.Close();
            }
        }

        private void cbVendorList_SelectedIndexChanged(object sender, EventArgs e)
        {
           selectedVendor = (Vendor)cbVendorList.SelectedItem;
        }

        public Vendor getSelectedVendor()
        {
            return selectedVendor;
        }
    }
}
