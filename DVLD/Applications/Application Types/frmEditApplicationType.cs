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

namespace DVLD.Applications.Application_Types
{
    public partial class frmEditApplicationType : Form
    {

        private int _ApplicationTypeID = -1;
        clsApplicationType _ApplicationType;
        public frmEditApplicationType(int ApplicationTypeID)
        {
            InitializeComponent();
            _ApplicationTypeID = ApplicationTypeID;
            
        }
        private void frmEditApplicationType_Load(object sender, EventArgs e)
        {
            _LoudApplicationTypeInfo();
        }

        private void _LoudApplicationTypeInfo()
        {
            _ApplicationType = clsApplicationType.Find(_ApplicationTypeID);
            if (_ApplicationType == null)
            {
                MessageBox.Show("No Application Type with ID = " + _ApplicationTypeID, "ApplicationType Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            lblApplicationTypeID.Text = _ApplicationTypeID.ToString();
            txtTitle.Text = _ApplicationType.Title;
            txtFees.Text = _ApplicationType.Fees.ToString();
        }

        private void txtTitle_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtTitle, "This field is required!");
                return;
            }
            else
            {
                errorProvider1.SetError(txtTitle, null);
            }

            if (txtTitle.Text.Trim() != _ApplicationType.Title
                               && clsApplicationType.IsApplicationTypeExist(txtTitle.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtTitle, "That application Type already exists!");
            }
            else
            {
                errorProvider1.SetError(txtTitle, null);
            }
        }

        private void txtFees_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
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
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }


            _ApplicationType.Title = txtTitle.Text.Trim();
            _ApplicationType.Fees = Convert.ToSingle(txtFees.Text.Trim());


            if (_ApplicationType.Save())
            {
                lblApplicationTypeID.Text = _ApplicationType.ID.ToString();

                lblTitle.Text = "Update Application Type";

                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {

                MessageBox.Show("Error: Data is not Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void txtFees_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFees.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtFees, "This field is required!");
                return;
            }
            else
            {
                errorProvider1.SetError(txtFees, null);
            }
            if (!clsValidatoin.IsNumber(txtFees.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtFees, "Invalid Number.");
            }
            else
            {
                errorProvider1.SetError(txtFees, null);
            }
            

        }
    }
}
