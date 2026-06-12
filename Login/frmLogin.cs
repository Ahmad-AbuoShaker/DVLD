using DVLD.Classes;
using DVLD_Buisness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Login
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            clsUser User = clsUser.FindByUserNameAndPassword(txtUserName.Text,clsUtil.HashPassword(txtPassword.Text));
            if (User != null) 
            {
                if(chkRememberMe.Checked)
                {
                    clsGlobal.RememberUsernameAndPassword(User.UserName, User.Password);
                }
                else
                {
                    clsGlobal.RememberUsernameAndPassword("", "");
                }
                if (!User.IsActive)
                {
                    txtUserName.Focus();
                    MessageBox.Show("Your accound is not Active, Contact Admin.", "In Active Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
              
                clsGlobal.CurrentUser = User;
                this.Hide();
                frmMain frm = new frmMain(this);
                frm.Show();

            }
            else
            {
                MessageBox.Show("Invalid User Name or Password", "In Active Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            string Username = "", Passwort = "";
            if (clsGlobal.GetStoredCredential(ref Username, ref Passwort))
                chkRememberMe.Checked = true;
            else
                chkRememberMe.Checked = false;


            txtUserName.Text = Username;
            txtPassword.Text = Passwort;

        }

       

        
    }
}
