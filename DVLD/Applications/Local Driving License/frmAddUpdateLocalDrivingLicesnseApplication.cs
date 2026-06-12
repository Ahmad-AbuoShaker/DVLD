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
using static DVLD.People.Controls.ctrlPersonCardWithFilter;

namespace DVLD.Applications.Local_Driving_License
{
    public partial class frmAddUpdateLocalDrivingLicesnseApplication : Form
    {
        private enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;

        private int _SelectedPersonID = -1;
        private int _LocalDrivingLicenseApplicationID = -1;
        private clsLocalDrivingLicenseApplications _LocalDrivingLicenseApplication;


        public frmAddUpdateLocalDrivingLicesnseApplication()
        {
            InitializeComponent();
            _LocalDrivingLicenseApplicationID = -1;
            _Mode = enMode.AddNew;
        }
        public frmAddUpdateLocalDrivingLicesnseApplication(int LocalDrivingLicenseApplicationID)
        {
            InitializeComponent();
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _Mode = enMode.Update;
        }

        private void _FillLicenseClassesInComoboBox()
        {
            DataTable dt=clsLicenseClasses.GetAllLicenseClasses();

            foreach(DataRow row in dt.Rows)
            {
                cbLicenseClass.Items.Add(row["ClassName"]);
            }

        }

        private void _ResetDefualtValues()
        {
            _FillLicenseClassesInComoboBox();

            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Local Driving Licesnse Application";
                this.Text = "Add New Local Driving Licesnse Application";
                _LocalDrivingLicenseApplication = new clsLocalDrivingLicenseApplications();

                tpApplicationInfo.Enabled = false;

                ctrlPersonCardWithFilter1.FilterFocus();

                cbLicenseClass.SelectedIndex = 1;
                lblFees.Text = clsApplicationType.Find((int)clsApplications.enApplicationType.NewDrivingLicense).Fees.ToString();
                lblApplicationDate.Text = DateTime.Now.ToShortDateString();
                lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;

            }
            else
            {
                lblTitle.Text = "Update Local Driving Licesnse Application";
                this.Text = "Update Local Driving Licesnse Application";


                tpApplicationInfo.Enabled = true;
                btnSave.Enabled = true;
            }


        }

        private void _LoudData()
        {
            _LocalDrivingLicenseApplication = clsLocalDrivingLicenseApplications.FindByLocalDrivingAppLicationID(_LocalDrivingLicenseApplicationID);
            ctrlPersonCardWithFilter1.FilterEnabled = false;


            if (_LocalDrivingLicenseApplication == null)
            {
                MessageBox.Show("No Application with ID = " + _LocalDrivingLicenseApplicationID, "Application Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();

                return;
            }


            ctrlPersonCardWithFilter1.LoadPersonInfo(_LocalDrivingLicenseApplication.ApplicantPersonID);

            lblLocalDrivingLicebseApplicationID.Text = _LocalDrivingLicenseApplicationID.ToString();
            lblApplicationDate.Text = clsFormat.DateToShort(_LocalDrivingLicenseApplication.ApplicationDate);
            lblFees.Text = _LocalDrivingLicenseApplication.PaidFees.ToString();
            cbLicenseClass.SelectedIndex = cbLicenseClass.FindString(clsLicenseClasses.Find(_LocalDrivingLicenseApplication.LicenseClassID).ClassName);
            lblCreatedByUser.Text = _LocalDrivingLicenseApplication.CreatedByUserID.ToString();

        }

        private void frmAddUpdateLocalDrivingLicesnseApplication_Load(object sender, EventArgs e)
        {
            _ResetDefualtValues();

            if (_Mode == enMode.Update)
            {
                _LoudData();
            }
        }

        private void btnApplicationInfoNext_Click(object sender, EventArgs e)
        {

            if (ctrlPersonCardWithFilter1.PersonID == -1)
            {
                MessageBox.Show("Please Select a Person", "Select a Person", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlPersonCardWithFilter1.FilterFocus();

            }
            else
            {
                btnSave.Enabled = true;
                tpApplicationInfo.Enabled = true;
                tcApplicationInfo.SelectedTab = tcApplicationInfo.TabPages["tpApplicationInfo"];
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fields are not valid!, put the mouse over the red icon(s) to see the erro", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            int LicenseClassID = clsLicenseClasses.Find(cbLicenseClass.Text).LicenseClassID;

            if (clsLicenses.IsLicenseExistByPersonID(ctrlPersonCardWithFilter1.PersonID, LicenseClassID))
            {
                MessageBox.Show("Person already have a license with the samee applied driving class ,Choose diffrent driving class."
                                    , "Not allowed", MessageBoxButtons.OK);
                return;
            }

            if (_Mode == enMode.AddNew || _LocalDrivingLicenseApplication.LicenseClassInfo.LicenseClassID != LicenseClassID)
            {
                int ActiveApplicationID = clsLocalDrivingLicenseApplications.GetActiveApplicationIDForLicenseClass(ctrlPersonCardWithFilter1.PersonID,
                                       clsApplications.enApplicationType.NewDrivingLicense, LicenseClassID);

                if (ActiveApplicationID != -1)
                {
                    MessageBox.Show($"Person already has an active application for this class with ID = {ActiveApplicationID}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            _LocalDrivingLicenseApplication.ApplicantPersonID = ctrlPersonCardWithFilter1.PersonID;
            _LocalDrivingLicenseApplication.ApplicationDate = DateTime.Now;
            _LocalDrivingLicenseApplication.ApplicationStatus = clsApplications.enApplicationStatus.New;
            _LocalDrivingLicenseApplication.ApplicationTypeID = (int)clsApplications.enApplicationType.NewDrivingLicense;
            _LocalDrivingLicenseApplication.PaidFees = Convert.ToSingle(lblFees.Text);
            _LocalDrivingLicenseApplication.CreatedByUserID = clsGlobal.CurrentUser.UserID;
            _LocalDrivingLicenseApplication.LicenseClassID = LicenseClassID;


            if (_LocalDrivingLicenseApplication.Save())
            {
                lblLocalDrivingLicebseApplicationID.Text = _LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
                //change form mode to update.
                _Mode = enMode.Update;
                lblTitle.Text = "Update Local Driving License Application";

                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void ctrlPersonCardWithFilter1_OnPersonSelected(object sender, PersonSelectedEventArgs e)
        {
            _SelectedPersonID = e.PersonID;

        }

        private void frmAddUpdateLocalDrivingLicesnseApplication_Activated(object sender, EventArgs e)
        {
            ctrlPersonCardWithFilter1.FilterFocus();
        }
    }
}

