using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Buisness
{
    public class clsLicenses
    {

        public enum enMode { AddNew = 0, Update = 1 };

        public enMode Mode = enMode.AddNew;
        public enum enIssueReason { FirstTime = 1, Renew = 2, DamagedReplacement = 3, LostReplacement = 4 };
        public int LicenseID {  get; set; }
        public int ApplicationID {  get; set; }
        public int DriverID {  get; set; }
        public clsDrivers DriverInfo { get; }
        public int LicenseClassID {  get; set; }
        public clsLicenseClasses LicenseClasseInfo { get; }
        public DateTime IssueDate {  get; set; }
        public DateTime ExpirationDate {  get; set; }
        public string Notes {  get; set; }
        public float PaidFees {  get; set; }
        public bool IsActive {  get; set; }
        public enIssueReason IssueReason { set; get; }
        public string IssueReasonText
        {
            get
            {
                return GetIssueReasonText(this.IssueReason);
            }
        }
        public int CreatedByUserID {  get; set; }
        public clsUser CreatedByUserInfo { get; }

        public clsDetainedLicense DetainedInfo
        { 
            get { return clsDetainedLicense.FindByLicenseID(this.LicenseID); }
        }

        public bool IsDetained
        {
            get { return clsDetainedLicense.IsLicenseDetained(this.LicenseID); }
        }

        public clsLicenses()
        {
            this.LicenseID = -1;
            this.ApplicationID = -1;
            this.DriverID= -1;
            this.LicenseClassID= -1;
            this.IssueDate = DateTime.Now;
            this.ExpirationDate = DateTime.Now;
            this.Notes = "";
            this.PaidFees = 0;
            this.IsActive = false;
            this.IssueReason = 0;
            this.CreatedByUserID = -1;
            Mode = enMode.AddNew;
        }
        public clsLicenses(int LicenseID, int ApplicationID, int DriverID, int LicenseClassID,
             DateTime IssueDate, DateTime ExpirationDate, string Notes, float PaidFees,
             bool IsActive, enIssueReason IssueReason, int CreatedByUserID)
        {
            this.LicenseID = LicenseID;
            this.ApplicationID = ApplicationID;
            this.DriverID = DriverID;
            this.LicenseClassID = LicenseClassID;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
            this.Notes = Notes;
            this.PaidFees = PaidFees;
            this.IsActive = IsActive;
            this.IssueReason = IssueReason;
            this.CreatedByUserID = CreatedByUserID;

            this.DriverInfo = clsDrivers.Find(this.DriverID);
            this.LicenseClasseInfo = clsLicenseClasses.Find(this.LicenseClassID);

           // this.DetainedInfo = clsDetainedLicense.FindByLicenseID(this.LicenseID);

            Mode = enMode.Update;
        }

        private bool _AddNewLicense() 
        {
            //call DataAccess Layer 

            this.LicenseID = clsLicensesData.AddNewLicense(this.ApplicationID, this.DriverID, this.LicenseClassID,
               this.IssueDate, this.ExpirationDate, this.Notes, this.PaidFees,
               this.IsActive, (byte)this.IssueReason, this.CreatedByUserID);


            return (this.LicenseID != -1);
        }

        private bool _UpdateLicense()
        {
            //call DataAccess Layer 

            return clsLicensesData.UpdateLicense(this.ApplicationID, this.LicenseID, this.DriverID, this.LicenseClassID,
               this.IssueDate, this.ExpirationDate, this.Notes, this.PaidFees,
               this.IsActive, (byte)this.IssueReason, this.CreatedByUserID);
        }


        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewLicense())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateLicense();

            }

            return false;
        }

        public static clsLicenses Find(int LicenseID)
        {
            int ApplicationID = -1, DriverID = -1, LicenseClassID = -1, CreatedByUserID = -1;
            DateTime IssueDate= DateTime.Now, ExpirationDate = DateTime.Now;
            string Notes = "";
            float PaidFees = 0;
            bool IsActive = false;
            byte IssueReason = 0;


            if (clsLicensesData.GetLicenseInfoByLicenseID(LicenseID, ref ApplicationID, ref DriverID, ref LicenseClassID, ref IssueDate,
                ref ExpirationDate, ref Notes, ref PaidFees, ref IsActive, ref IssueReason, ref CreatedByUserID))
            {
                return new clsLicenses(LicenseID, ApplicationID, DriverID, LicenseClassID, IssueDate,
                ExpirationDate, Notes, PaidFees, IsActive, (enIssueReason)IssueReason, CreatedByUserID);
            }

            return null;
        }

        public static DataTable GetAllLicenses()
        {
            return clsLicensesData.GetAllLicenses();
        }
        public static DataTable GetDriverLicenses(int DriverID)
        {
            return clsLicensesData.GetDriverLicenses(DriverID);
        }
        public static bool IsLicenseExistByPersonID(int PersonID, int LicenseClassID) 
        {
            return clsLicensesData.GetActiveLicenseIDByPersonID(PersonID, LicenseClassID) > -1 ;
        }
       
        public Boolean IsLicenseExpired() 
        {
            return DateTime.Now > this.ExpirationDate;
        }
        public bool DeactivateCurrentLicense()
        {
            return clsLicensesData.DeactivateLicense(this.LicenseID);
        }
        public static string GetIssueReasonText(enIssueReason IssueReason)
        {

            switch (IssueReason)
            {
                case enIssueReason.FirstTime:
                    return "First Time";
                case enIssueReason.Renew:
                    return "Renew";
                case enIssueReason.DamagedReplacement:
                    return "Replacement for Damaged";
                case enIssueReason.LostReplacement:
                    return "Replacement for Lost";
                default:
                    return "First Time";
            }
        }
        public static int GetActiveLicenseIDByPersonID(int PersonID, int LicenseClassID)
        {
            return clsLicensesData.GetActiveLicenseIDByPersonID(PersonID, LicenseClassID);
        }
        public static bool DoesPersonHaveActiveLicense(int PersonID, int LicenseClassID)
        {
            return (GetActiveLicenseIDByPersonID(clsDrivers.GetDriverIDByPersonID(PersonID), LicenseClassID) != -1);
        }

      
        public int Detain(float FineFees,int CreatedByUserID)
        {
            clsDetainedLicense detainedLicense = new clsDetainedLicense();
            detainedLicense.LicenseID = this.LicenseID;
            detainedLicense.DetainDate = DateTime.Now;
            detainedLicense.FineFees = Convert.ToSingle(FineFees);
            detainedLicense.CreatedByUserID = CreatedByUserID;

            if (!detainedLicense.Save())
            {
                return -1;
            }

            return detainedLicense.DetainID;

        }

        public bool ReleaseDetainedLicense(int ReleasedByUserID,ref int ApplicationID)
        {
            clsApplications Application = new clsApplications();

            Application.ApplicantPersonID = this.DriverInfo.PersonID;
            Application.ApplicationDate = DateTime.Now;
            Application.ApplicationTypeID = (int)clsApplications.enApplicationType.ReleaseDetainedDrivingLicsense;
            Application.ApplicationStatus = clsApplications.enApplicationStatus.Completed;
            Application.CreatedByUserID = CreatedByUserID;
            Application.LastStatusDate = DateTime.Now;
            Application.PaidFees = clsApplicationType.Find((int)clsApplications.enApplicationType.ReleaseDetainedDrivingLicsense).Fees;

            if (!Application.Save())
            {
                ApplicationID = -1;
                return false;
            }
            ApplicationID = Application.ApplicationID;

            return (this.DetainedInfo.ReleaseDetainedLicense(CreatedByUserID, ApplicationID));
            
        }

        public clsLicenses RenewLicense(string Notes , int CreatedByUserID)
        {
            clsApplications Application = new clsApplications();

            Application.ApplicantPersonID = this.DriverInfo.PersonID;
            Application.ApplicationDate=DateTime.Now;
            Application.ApplicationTypeID = (int)clsApplications.enApplicationType.RenewDrivingLicense;
            Application.ApplicationStatus = clsApplications.enApplicationStatus.Completed;
            Application.CreatedByUserID = CreatedByUserID;
            Application.LastStatusDate = DateTime.Now;
            Application.PaidFees = clsApplicationType.Find((int)clsApplications.enApplicationType.RenewDrivingLicense).Fees;

            if (!Application.Save())
            {
                return null;
            }

            clsLicenses NewLicense = new clsLicenses();

            NewLicense.ApplicationID = Application.ApplicationID;
            NewLicense.CreatedByUserID= CreatedByUserID;
            NewLicense.DriverID= this.DriverInfo.DriverID;
            NewLicense.IssueDate = DateTime.Now;
            NewLicense.ExpirationDate = DateTime.Now.AddYears(this.LicenseClasseInfo.DefaultValidityLength);
            NewLicense.Notes = Notes;
            NewLicense.PaidFees = this.LicenseClasseInfo.ClassFees;
            NewLicense.IssueReason = enIssueReason.Renew;
            NewLicense.IsActive = true;
            NewLicense.LicenseClassID = this.LicenseClassID;

            if (!NewLicense.Save())
            {
                return null;
            }

            DeactivateCurrentLicense();

            return NewLicense;
        }

        public clsLicenses Replace(enIssueReason IssueReason, int CreatedByUserID)
        {
            clsApplications Application = new clsApplications();

            Application.ApplicantPersonID = this.DriverInfo.PersonID;
            Application.ApplicationDate = DateTime.Now;

            Application.ApplicationTypeID = (IssueReason == enIssueReason.DamagedReplacement) ?
                (int)clsApplications.enApplicationType.ReplaceDamagedDrivingLicense :
                (int)clsApplications.enApplicationType.ReplaceLostDrivingLicense;

            Application.ApplicationStatus = clsApplications.enApplicationStatus.Completed;
            Application.LastStatusDate = DateTime.Now;
            Application.PaidFees = clsApplicationType.Find(Application.ApplicationTypeID).Fees;
            Application.CreatedByUserID = CreatedByUserID;

            if (!Application.Save())
            {
                return null;
            }

            clsLicenses NewLicense = new clsLicenses();

            NewLicense.ApplicationID = Application.ApplicationID;
            NewLicense.DriverID = this.DriverID;
            NewLicense.LicenseClassID = this.LicenseClassID;
            NewLicense.IssueDate = DateTime.Now;
            NewLicense.ExpirationDate = this.ExpirationDate;
            NewLicense.Notes = this.Notes;
            NewLicense.PaidFees = 0;// no fees for the license because it's a replacement.
            NewLicense.IsActive = true;
            NewLicense.IssueReason = IssueReason;
            NewLicense.CreatedByUserID = CreatedByUserID;



            if (!NewLicense.Save())
            {
                return null;
            }

            //we need to deactivate the old License.
            DeactivateCurrentLicense();

            return NewLicense;
        }

    }
}
