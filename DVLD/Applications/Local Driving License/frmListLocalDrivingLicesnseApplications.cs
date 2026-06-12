using DVLD.Licenses;
using DVLD.Licenses.Local_Licenses;
using DVLD.Tests;
using DVLD_Buisness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DVLD.Applications.Local_Driving_License
{
    public partial class frmListLocalDrivingLicesnseApplications : Form
    {
        private DataTable _dtLocalDrivingLicesnseApplications;

        public frmListLocalDrivingLicesnseApplications()
        {
            InitializeComponent();
        }

        private void frmListLocalDrivingLicesnseApplications_Load(object sender, EventArgs e)
        {
            _dtLocalDrivingLicesnseApplications = clsLocalDrivingLicenseApplications.GetAllLocalDrivingLicenseApplications();
            dgvLocalDrivingLicenseApplications.DataSource = _dtLocalDrivingLicesnseApplications;

            cbFilterBy.SelectedIndex = 0;
            lblRecordsCount.Text=dgvLocalDrivingLicenseApplications.RowCount.ToString();

            if (dgvLocalDrivingLicenseApplications.RowCount>0)
            {
                dgvLocalDrivingLicenseApplications.Columns[0].HeaderText = "L.D.L.AppID";
                dgvLocalDrivingLicenseApplications.Columns[0].Width = 90;

                dgvLocalDrivingLicenseApplications.Columns[1].HeaderText = "Driving Class";
                dgvLocalDrivingLicenseApplications.Columns[1].Width = 270;

                dgvLocalDrivingLicenseApplications.Columns[2].HeaderText = "National No.";
                dgvLocalDrivingLicenseApplications.Columns[2].Width = 120;

                dgvLocalDrivingLicenseApplications.Columns[3].HeaderText = "Full Name";
                dgvLocalDrivingLicenseApplications.Columns[3].Width = 320;

                dgvLocalDrivingLicenseApplications.Columns[4].HeaderText = "Appliction Data";
                dgvLocalDrivingLicenseApplications.Columns[4].Width = 200;

                dgvLocalDrivingLicenseApplications.Columns[5].HeaderText = "Passed Tests";
                dgvLocalDrivingLicenseApplications.Columns[5].Width = 110;


            }
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilterBy.Text != "None");

            txtFilterValue.Text = "";
            txtFilterValue.Focus();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = "";

            switch (cbFilterBy.Text)
            {
                case "L.D.L.AppID":
                    FilterColumn = "LocalDrivingLicenseApplicationID";
                    break;

                case "National No.":
                    FilterColumn = "NationalNo";
                    break;

                case "Full Name":
                    FilterColumn = "FullName";
                    break;

                case "Status":
                    FilterColumn = "Status";
                    break;
                default:
                    FilterColumn = "None";
                    break;

            }

            if (txtFilterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtLocalDrivingLicesnseApplications.DefaultView.RowFilter = "";
                lblRecordsCount.Text = _dtLocalDrivingLicesnseApplications.DefaultView.Count.ToString();
                return;
            }


            if (FilterColumn == "LocalDrivingLicenseApplicationID")
            {
                _dtLocalDrivingLicesnseApplications.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtFilterValue.Text.Trim());
            }
            else
            {
                _dtLocalDrivingLicesnseApplications.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, txtFilterValue.Text.Trim());
            }

            lblRecordsCount.Text = _dtLocalDrivingLicesnseApplications.DefaultView.Count.ToString();
        }

        private void btnAddNewApplication_Click(object sender, EventArgs e)
        {
           frmAddUpdateLocalDrivingLicesnseApplication frm =new frmAddUpdateLocalDrivingLicesnseApplication();
            frm.ShowDialog();
            frmListLocalDrivingLicesnseApplications_Load(null,null);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmLocalDrivingLicenseApplicationInfo frm = new frmLocalDrivingLicenseApplicationInfo(
                                      (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            frmListLocalDrivingLicesnseApplications_Load(null, null);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateLocalDrivingLicesnseApplication frm = new frmAddUpdateLocalDrivingLicesnseApplication(
                                     (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            frmListLocalDrivingLicesnseApplications_Load(null, null);
        }

        private void DeleteApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are sure you do want to delete this application","Confirm",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
            {
                clsLocalDrivingLicenseApplications localDrivingLicenseApplications = clsLocalDrivingLicenseApplications.FindByLocalDrivingAppLicationID(
                          (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value);
                if(localDrivingLicenseApplications != null )      
                {
                    if (localDrivingLicenseApplications.Delete())
                    {
                        MessageBox.Show("Application Deleted Successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //refresh the form again.
                        frmListLocalDrivingLicesnseApplications_Load(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Could not delete applicatoin, other data depends on it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    } 
                }
            }
        }

        private void CancelApplicaitonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are sure you do want to cancel this application", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                clsLocalDrivingLicenseApplications localDrivingLicenseApplications = clsLocalDrivingLicenseApplications.FindByLocalDrivingAppLicationID(
                         (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value);

                if (localDrivingLicenseApplications != null)
                {

                    if (localDrivingLicenseApplications.Cancel())
                    {
                        MessageBox.Show("Application canceled Successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //refresh the form again.
                        frmListLocalDrivingLicesnseApplications_Load(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Could not cancel applicatoin, other data depends on it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

       
        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;
            clsLocalDrivingLicenseApplications localDrivingLicenseApplication = clsLocalDrivingLicenseApplications.FindByLocalDrivingAppLicationID(LocalDrivingLicenseApplicationID);

            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(localDrivingLicenseApplication.ApplicantPersonID);
            frm.ShowDialog();
        }

        private void cmsApplications_Opening(object sender, CancelEventArgs e)
        {

            clsLocalDrivingLicenseApplications LocalDrivingLicenseApplication= clsLocalDrivingLicenseApplications.FindByLocalDrivingAppLicationID(
                                                             (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value);

            string Status = dgvLocalDrivingLicenseApplications.CurrentRow.Cells[6].Value.ToString();
            bool LicenseExists = LocalDrivingLicenseApplication.IsLicenseIssued();

            byte TotalPassedTests = byte.Parse(dgvLocalDrivingLicenseApplications.CurrentRow.Cells[5].Value.ToString());


            issueDrivingLicenseFirstTimeToolStripMenuItem.Enabled = (!LicenseExists && TotalPassedTests == 3);
            showLicenseToolStripMenuItem.Enabled = LicenseExists;


            editToolStripMenuItem.Enabled = (!LicenseExists && LocalDrivingLicenseApplication.ApplicationStatus == clsApplications.enApplicationStatus.New);
            DeleteApplicationToolStripMenuItem.Enabled = LocalDrivingLicenseApplication.ApplicationStatus == clsApplications.enApplicationStatus.New;
            CancelApplicaitonToolStripMenuItem.Enabled = LocalDrivingLicenseApplication.ApplicationStatus == clsApplications.enApplicationStatus.New;
            ScheduleTestsMenue.Enabled = !LicenseExists;

            bool PassedVisionTest = LocalDrivingLicenseApplication.DoesPassTestType(clsTestType.enTestType.VisionTest); ;
            bool PassedWrittenTest = LocalDrivingLicenseApplication.DoesPassTestType(clsTestType.enTestType.WrittenTest);
            bool PassedStreetTest = LocalDrivingLicenseApplication.DoesPassTestType(clsTestType.enTestType.StreetTest);

            ScheduleTestsMenue.Enabled = (!PassedVisionTest || !PassedWrittenTest || !PassedStreetTest) && (LocalDrivingLicenseApplication.ApplicationStatus == clsApplications.enApplicationStatus.New);

            if (ScheduleTestsMenue.Enabled)
            {
                scheduleVisionTestToolStripMenuItem.Enabled = !PassedVisionTest;

                scheduleWrittenTestToolStripMenuItem.Enabled = PassedVisionTest && !PassedWrittenTest;

                scheduleStreetTestToolStripMenuItem.Enabled = PassedVisionTest && PassedWrittenTest && !PassedStreetTest;
            }
        }

        private void _ScheduleTest(clsTestType.enTestType TestType)
        {

            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;
            frmListTestAppointments frm = new frmListTestAppointments(LocalDrivingLicenseApplicationID, TestType);
            frm.ShowDialog();
            //refresh
            frmListLocalDrivingLicesnseApplications_Load(null, null);

        }

        private void scheduleVisionTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ScheduleTest(clsTestType.enTestType.VisionTest);
        }

        private void scheduleWrittenTestToolStripMenuItem_Click(object sender, EventArgs e)
        {

            _ScheduleTest(clsTestType.enTestType.WrittenTest);
        }

        private void scheduleStreetTestToolStripMenuItem_Click(object sender, EventArgs e)
        {

            _ScheduleTest(clsTestType.enTestType.StreetTest);
        }

        private void issueDrivingLicenseFirstTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;
            frmIssueDriverLicenseFirstTime frm = new frmIssueDriverLicenseFirstTime(LocalDrivingLicenseApplicationID);
            frm.ShowDialog();
            //refresh
            frmListLocalDrivingLicesnseApplications_Load(null, null);
        }

        private void showLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;
            int LicenseID=clsLocalDrivingLicenseApplications.FindByLocalDrivingAppLicationID
                (LocalDrivingLicenseApplicationID).GetActiveLicenseID();
            if (LicenseID != -1)
            {
                frmShowLicenseInfo frm = new frmShowLicenseInfo(LicenseID);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("No License Found!", "No License", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
