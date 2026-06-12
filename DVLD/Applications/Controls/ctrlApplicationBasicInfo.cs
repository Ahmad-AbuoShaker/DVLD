using DVLD.People;
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
using static System.Net.Mime.MediaTypeNames;

namespace DVLD.Applications.Controls
{
    public partial class ctrlApplicationBasicInfo : UserControl
    {
        private int _ApplicaitionID;

        public int ApplicaitionID { get { return _ApplicaitionID; } }

        private clsApplications _Application;
        public clsApplications SelectedApplication {  get { return _Application; } }

        public ctrlApplicationBasicInfo()
        {
            InitializeComponent();
        }

        public void ResetApplicationInfo()
        {
            lblApplicationID.Text = "[????]";
            lblStatus.Text = "[????]";
            lblFees.Text = "[????]";
            lblType.Text = "[????]";
            lblApplicant.Text = "[????]";
            lblDate.Text = "[????]";
            lblStatusDate.Text = "[????]";
            lblCreatedByUser.Text = "[????]";
        }

        private void _FillApplicationInfo()
        {

            _ApplicaitionID = _Application.ApplicationID;
            lblApplicationID.Text = _Application.ApplicationID.ToString();
            lblStatus.Text = _Application.StatusText;
            lblFees.Text = _Application.PaidFees.ToString("0.00");
            lblType.Text = clsApplicationType.Find(_Application.ApplicationTypeID).Title;
            lblApplicant.Text = _Application.PersonInfo.FullName;
            lblDate.Text = _Application.ApplicationDate.ToShortDateString();
            lblStatusDate.Text = _Application.LastStatusDate.ToShortDateString();
            lblCreatedByUser.Text = clsUser.FindByUserID(_Application.CreatedByUserID).UserName;
        }

        public void LoadApplicationInfo(int ApplicationID)
        {
            _ApplicaitionID = ApplicationID;

            _Application = clsApplications.FindBaseApplication(_ApplicaitionID);
            

            if (_Application == null)
            {
                ResetApplicationInfo();
                MessageBox.Show("Application not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _FillApplicationInfo();

        }

        private void llViewPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowPersonInfo frm = new frmShowPersonInfo(_Application.ApplicantPersonID);
            frm.ShowDialog();
        }
    }
}
