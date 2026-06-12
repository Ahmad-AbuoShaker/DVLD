using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Buisness
{
    public class clsDrivers
    {
        public enum enMode { AddNew = 0, Update = 1 };

        public enMode Mode = enMode.AddNew;
        public int DriverID {  get; set; }
        public int PersonID {  get; set; }
        public clsPerson PersonInfo  { get; }
        public int CreatedByUserID {  get; set; }
        public DateTime CreatedDate {  get; set; }

        

        public clsDrivers()
        {
            this.DriverID = -1;
            this.PersonID = -1;
            this.CreatedByUserID = -1;
            this.CreatedDate=DateTime.Now;
            Mode = enMode.AddNew;
        }

        public clsDrivers(int DriverID, int PersonID, int CreatedByUserID, DateTime CreatedDate)
        {
            this.DriverID = DriverID;
            this.PersonID = PersonID;
            this.PersonInfo=clsPerson.Find(PersonID);
            this.CreatedByUserID = CreatedByUserID;
            this.CreatedDate = CreatedDate;
            Mode = enMode.Update;
        }

        public static int GetDriverIDByPersonID(int PersonID)
        {
            return clsDriversData.GetDriverIDByPersonID(PersonID);
        }

        private bool _AddNew()
        {
            this.DriverID = clsDriversData.AddNewDriver(this.PersonID, this.CreatedByUserID, DateTime.Now);
            return this.DriverID > -1;
        }

        private bool _Update()
        {
            return clsDriversData.UpdateDriver(this.DriverID, this.PersonID, this.CreatedByUserID);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(_AddNew())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else { return false; }
                      

                case enMode.Update:
                    return _Update();
            
            }

            return false;
        }
        public static clsDrivers Find(int DriverID)
        {
            int PersonID = -1, CreatedByUserID=-1;
            DateTime CreatedDate = DateTime.Now;
            if(clsDriversData.GetDriverInfoByID(DriverID,
                             ref PersonID,ref CreatedByUserID,ref CreatedDate))
            {
                return new clsDrivers(DriverID,PersonID, CreatedByUserID,CreatedDate);
            }

            return null;
        }

        public static clsDrivers FindByPersonID(int PersonID)
        {
            int DriverID = -1, CreatedByUserID=-1;
            DateTime CreatedDate = DateTime.Now;
            if(clsDriversData.GetDriverInfoByPersonID(PersonID,
                             ref DriverID, ref CreatedByUserID,ref CreatedDate))
            {
                return new clsDrivers(DriverID,PersonID, CreatedByUserID,CreatedDate);
            }

            return null;
        }

        public static DataTable GetAllLicenses(int DriverID)
        {
            return clsLicenses.GetDriverLicenses(DriverID);
        }
        public static DataTable GetAllDriver()
        {
            return clsDriversData.GetAllDrivers();
        }
    }
}
