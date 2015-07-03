using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;
using System.Resources;
using RedRose_VoucherScanner.Properties;

namespace RedRose_VoucherScanner
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            cmbLanguage.SelectedIndex = 0;
        }

        public bool authorizedUser = false;

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                // For log in with out internet connection
                // purpose is being enable for MICR test mode without connection
                // username: RedRose
                // pw: NC
                if (txtUserName.Text != "RR" || txtPassword.Text != "NC")
                { 
                    String memberId = RestClient.Authorize(txtUserName.Text, txtPassword.Text); 
                }
                //Open Main form
                authorizedUser = true;
                MainForm myMF = new MainForm(txtUserName.Text, txtPassword.Text);
                this.Hide();
                myMF.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Main mainfrm = new Main();
            //mainfrm.loginUser = txtUserName.Text;
            //mainfrm.loginPass = txtPassword.Text;
            //string resp = mainfrm.UpdateBeneficiaries();
            //if (resp == "OK")
            //{
            //    mainfrm.Show();
            //    this.Hide();
            //}
            //else
            //{
            //    if (resp.Contains("Unauthorized"))
            //        lblStatus.Text = "Login Error!";
            //}
            //return;
            
            
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int selectedIndex = cmbLanguage.SelectedIndex;

            //switch (selectedIndex)
            //{
            //    case 0:
            //        strings.Culture = CultureInfo.CreateSpecificCulture("en");
            //        break;
            //    case 1:
            //        strings.Culture = CultureInfo.CreateSpecificCulture("tr");
            //        break;
            //    case 2:
            //        strings.Culture = CultureInfo.CreateSpecificCulture("fr");
            //        break;
            //    default:
            //        strings.Culture = CultureInfo.CreateSpecificCulture("en");
            //        break;
            //}

            //updateLabels();
        }

        private void updateLabels()
        {
        //    lblInfoMsg.Text = strings.login_InfoMsg;
        //    lblLanguage.Text = strings.login_Language;
        //    lblUsername.Text = strings.login_Username;
        //    lblPassword.Text = strings.login_Password;
        //    btnOK.Text = strings.login_Ok;
        //    btnClose.Text = strings.login_Cancel;
        //    this.Text = strings.login_Title;
        }


    }
}
