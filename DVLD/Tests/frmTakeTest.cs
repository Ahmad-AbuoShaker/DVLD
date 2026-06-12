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
using static DVLD_Buisness.clsTestType;

namespace DVLD.Tests
{
    public partial class frmTakeTest : Form
    {
        private int _AppointmentID;

        private int _TestID = -1;
        private clsTests _Test;




        public frmTakeTest(int AppointmentID)
        {
            InitializeComponent();
            _AppointmentID = AppointmentID;

        }

        private void frmTakeTest_Load(object sender, EventArgs e)
        {
            ctrlSecheduledTest1.LoadInfo(_AppointmentID);
            _TestID=ctrlSecheduledTest1.TestID;

            if (ctrlSecheduledTest1.TestAppointmentID == -1)
                btnSave.Enabled = false;
            else
                btnSave.Enabled = true;


            if (_TestID != -1) 
            {
                _Test = clsTests.FindByID(_TestID);

                btnSave.Enabled = false;

                if (_Test.TestResult)
                    rbPass.Checked = true;
                else
                    rbFail.Checked = true;

                lblUserMessage.Enabled = true;
                rbPass.Enabled = false;
                rbFail.Enabled = false;

                txtNotes.Text = _Test.Notes;
            }

            else
            {
                _Test = new clsTests();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Are you sure you want to save? After that you cannot change the Pass/Fail results after you save?.",
                     "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No
            )
            {
                return;
            }



            _Test.TestAppointmentID = _AppointmentID;
            _Test.Notes = txtNotes.Text;
            _Test.TestResult=rbPass.Checked;
            _Test.CreatedByUserID=clsGlobal.CurrentUser.UserID;

            if (_Test.Save())
            {
                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                this.Close();

            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
