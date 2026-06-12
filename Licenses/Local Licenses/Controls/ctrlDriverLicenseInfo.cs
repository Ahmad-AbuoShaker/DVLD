using DVLD.Classes;
using DVLD.Properties;
using DVLD_Buisness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Licenses.Local_Licenses.Controls
{
    public partial class ctrlDriverLicenseInfo : UserControl
    {
        private clsLicenses _License;

        private int _LicenseID = -1;

        public int LicenseID
        {
            get { return _LicenseID; }
        }

        public clsLicenses SelectedLicenseInfo
        {
            get { return _License; }
        }


        public ctrlDriverLicenseInfo()
        {
            InitializeComponent();
        }

        public void LoadLicenseInfo(int LicenseID)
        {
            _License=clsLicenses.Find(LicenseID);

            if (_License == null)
            {
                ResetLicenseInfo();
                MessageBox.Show("No License with LicenseID = " + LicenseID.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _FillLicenseInfo();

        }

        private void _LoadPersonImage()
        {
            if (_License.DriverInfo.PersonInfo.Gendor == 0)
                pbPersonImage.Image = Resources.Male_512;
            else
                pbPersonImage.Image = Resources.Female_512;

            string ImagePath = _License.DriverInfo.PersonInfo.ImagePath;
            if (ImagePath != "")
                if (File.Exists(ImagePath))
                    pbPersonImage.ImageLocation = ImagePath;
                else
                    MessageBox.Show("Could not find this image: = " + ImagePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void _FillLicenseInfo()
        {

            _LicenseID = _License.LicenseID;
            lblClass.Text = _License.LicenseClasseInfo.ClassName;
            lblFullName.Text = _License.DriverInfo.PersonInfo.FullName;
            lblLicenseID.Text = _License.LicenseID.ToString();
            lblNationalNo.Text = _License.DriverInfo.PersonInfo.NationalNo;
            lblGendor.Text = _License.DriverInfo.PersonInfo.Gendor == 0 ? "Male" : "Female";
            lblIssueDate.Text = clsFormat.DateToShort(_License.IssueDate);
            lblIssueReason.Text = _License.IssueReason.ToString();
            lblNotes.Text = _License.Notes.ToString();

            lblIsActive.Text = _License.IsActive ? "Yes" : "No";
            lblDateOfBirth.Text= clsFormat.DateToShort(_License.DriverInfo.PersonInfo.DateOfBirth);
            lblDriverID.Text = _License.DriverInfo.DriverID.ToString();
            lblExpirationDate.Text = clsFormat.DateToShort(_License.ExpirationDate);
            lblIsDetained.Text = clsDetainedLicense.IsLicenseDetained(_LicenseID) ? "Yes" : "No";

            _LoadPersonImage();
        }

        public void ResetLicenseInfo()
        {
            _LicenseID = -1;

            lblClass.Text = "[????]";
            lblFullName.Text = "[????]";
            lblLicenseID.Text = "[????]";
            lblNationalNo.Text = "[????]";
            pbGendor.Image = Resources.Man_32;
            lblGendor.Text = "[????]";
            lblIssueDate.Text = "[????]";
            lblIssueReason.Text = "[????]";
            lblNotes.Text = "[????]";

            lblIsActive.Text = "[????]";
            lblDateOfBirth.Text = "[????]";
            lblDriverID.Text = "[????]";
            lblExpirationDate.Text = "[????]";
            lblIsDetained.Text = "[????]";

            pbPersonImage.Image = Resources.Male_512;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
