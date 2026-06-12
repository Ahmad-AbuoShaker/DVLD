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

namespace DVLD.Licenses.Controls
{
    public partial class ctrlDriverLicenses : UserControl
    {
        private int _DriverID = -1;
        private clsDrivers _Driver;
        private DataTable _dtLocalLicense;
        private DataTable _dtInternationalLicense;

        public ctrlDriverLicenses()
        {
            InitializeComponent();
        }

        private void _LoudLocalLicenses()
        {

            _dtLocalLicense = clsDrivers.GetAllLicenses(_DriverID);
            dgvLocalLicensesHistory.DataSource = _dtLocalLicense;

            lblLocalLicensesRecords.Text = dgvLocalLicensesHistory.RowCount.ToString();

            if (dgvLocalLicensesHistory.RowCount > 0)
            {
                dgvLocalLicensesHistory.Columns[0].HeaderText = "Lic.ID";
                dgvLocalLicensesHistory.Columns[0].Width = 90;

                dgvLocalLicensesHistory.Columns[1].HeaderText = "App.ID";
                dgvLocalLicensesHistory.Columns[1].Width = 90;

                dgvLocalLicensesHistory.Columns[2].HeaderText = "Class Name";
                dgvLocalLicensesHistory.Columns[2].Width = 300;

                dgvLocalLicensesHistory.Columns[3].HeaderText = "Issue Date";
                dgvLocalLicensesHistory.Columns[3].Width = 250;

                dgvLocalLicensesHistory.Columns[4].HeaderText = "Exprition Date";
                dgvLocalLicensesHistory.Columns[4].Width = 200;

                dgvLocalLicensesHistory.Columns[5].HeaderText = "Is Active";
                dgvLocalLicensesHistory.Columns[5].Width = 100;


            }

        }

        private void _LoudInternationalLicense()
        {
            _dtInternationalLicense=clsInternationalLicenses.GetDriverInternationalLicenses(_DriverID);

            lblLocalLicensesRecords.Text = dgvInternationalLicensesHistory.RowCount.ToString();
            dgvInternationalLicensesHistory.DataSource = _dtInternationalLicense;

            if (dgvLocalLicensesHistory.RowCount > 0)
            {
                dgvInternationalLicensesHistory.Columns[0].HeaderText = "int.License ID";
                dgvInternationalLicensesHistory.Columns[0].Width = 150;

                dgvInternationalLicensesHistory.Columns[1].HeaderText = "App.ID";
                dgvInternationalLicensesHistory.Columns[1].Width = 150;

                dgvInternationalLicensesHistory.Columns[2].HeaderText = "L.license ID";
                dgvInternationalLicensesHistory.Columns[2].Width = 150;

                dgvInternationalLicensesHistory.Columns[3].HeaderText = "Issue Date";
                dgvInternationalLicensesHistory.Columns[3].Width = 200;

                dgvInternationalLicensesHistory.Columns[4].HeaderText = "Exprition Date";
                dgvInternationalLicensesHistory.Columns[4].Width = 200;

                dgvInternationalLicensesHistory.Columns[5].HeaderText = "Is Active";
                dgvInternationalLicensesHistory.Columns[5].Width = 100;


            }
        }

        public void LoudInfo(int DriverID)
        {
            _DriverID = DriverID;
            _Driver=clsDrivers.Find(_DriverID);

            if( _Driver == null)
            {
                MessageBox.Show("There is no Driver with ID "+_DriverID,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            _LoudLocalLicenses();
            _LoudInternationalLicense();
        }
        public void LoudInfoByPersonID(int PersonID)
        {
            _Driver=clsDrivers.FindByPersonID(PersonID);

            if (_Driver == null)
            {
                MessageBox.Show("There is no Driver linked with Person ID " + PersonID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _DriverID = _Driver.DriverID;
            _LoudLocalLicenses();
            _LoudInternationalLicense();
        }

        public void Clear()
        {
            if (dgvLocalLicensesHistory != null) 
            {
                _dtLocalLicense.Clear();
                _dtInternationalLicense.Clear();
            }
        }

        private void showLicenseInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowLicenseInfo frm = new frmShowLicenseInfo((int)dgvLocalLicensesHistory.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
        }

        private void InternationalLicenseHistorytoolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
    }
}
