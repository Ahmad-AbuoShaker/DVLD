using DVLD.Licenses.Local_Licenses;
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

namespace DVLD.Applications.Local_Driving_License
{
    public partial class ctrlDrivingLicenseApplicationInfo : UserControl
    {
        private int _lDrivingLicenseApplicationID;

        private int _LicenseID = -1;
        public int lDrivingLicenseApplicationID { get { return _lDrivingLicenseApplicationID; } }

        private clsLocalDrivingLicenseApplications _lDrivingLicenseApplication;
        public clsLocalDrivingLicenseApplications SelectedlDrivingLicenseApplication { get { return _lDrivingLicenseApplication; } }


        public ctrlDrivingLicenseApplicationInfo()
        {
            InitializeComponent();
        }

        public void ResetlDrivingLicenseApplicationInfo()
        {
            lblLocalDrivingLicenseApplicationID.Text = "[???]";
            lblAppliedFor.Text = "[???]";
            lblPassedTests.Text = "0";
            llShowLicenceInfo.Enabled = false;
            ctrlApplicationBasicInfo1.ResetApplicationInfo();
        }


        private void _FillDrivingLicenseApplicationInfo()
        {
            _LicenseID=_lDrivingLicenseApplication.GetActiveLicenseID();

            llShowLicenceInfo.Enabled = (_LicenseID != -1);


            _lDrivingLicenseApplicationID = _lDrivingLicenseApplication.LocalDrivingLicenseApplicationID;
            lblLocalDrivingLicenseApplicationID.Text = _lDrivingLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
            lblAppliedFor.Text = _lDrivingLicenseApplication.LicenseClassInfo.ClassName;
            lblPassedTests.Text = clsTests.GetPassedTestCount(_lDrivingLicenseApplicationID).ToString() + "/3";

            ctrlApplicationBasicInfo1.LoadApplicationInfo(_lDrivingLicenseApplication.ApplicationID);

        }

        public void LoadApplicationInfoByLocalDrivingAppID(int lDrivingLicenseApplicationID)
        {
            

            _lDrivingLicenseApplication = clsLocalDrivingLicenseApplications.FindByLocalDrivingAppLicationID(lDrivingLicenseApplicationID);

            if( _lDrivingLicenseApplication == null)
            {
                ResetlDrivingLicenseApplicationInfo();
                MessageBox.Show("Application not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            

            _FillDrivingLicenseApplicationInfo();


        }
        public void LoadApplicationInfoByApplicationID(int ApplicationID)
        {
            

            _lDrivingLicenseApplication = clsLocalDrivingLicenseApplications.FindByApplicationID(lDrivingLicenseApplicationID);

            if( _lDrivingLicenseApplication == null)
            {
                ResetlDrivingLicenseApplicationInfo();
                MessageBox.Show("Application not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _lDrivingLicenseApplicationID = _lDrivingLicenseApplication.LocalDrivingLicenseApplicationID;

            _FillDrivingLicenseApplicationInfo();
            ctrlApplicationBasicInfo1.LoadApplicationInfo(_lDrivingLicenseApplication.ApplicationID);

            llShowLicenceInfo.Enabled = _LicenseID!=-1;

        }

        private void llShowLicenceInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowLicenseInfo frm = new frmShowLicenseInfo(_LicenseID);
            frm.ShowDialog();
        }


    }
}
