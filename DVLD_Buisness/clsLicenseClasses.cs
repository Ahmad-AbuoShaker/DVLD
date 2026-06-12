using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Buisness
{
    public class clsLicenseClasses
    {
        private enum enMode { AddNew=1,Update=0 }
        private enMode Mode = enMode.AddNew;

        public int LicenseClassID { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public byte MinimumAllowedAge { get; set; }
        public byte DefaultValidityLength { get; set; }
        public float ClassFees { get; set; }

        public clsLicenseClasses()
        {
            this.LicenseClassID = -1;
            this.ClassName = "";
            this.ClassDescription = "";
            this.MinimumAllowedAge = 0;
            this.DefaultValidityLength = 0;
            this.ClassFees = 0;

            Mode = enMode.AddNew;
        }

        public clsLicenseClasses(int LicenseClassID, string ClassName,
                                              string ClassDescription, byte MinimumAllowedAge,
                                                   byte DefaultValidityLength, float ClassFees)
        {
            this.LicenseClassID = LicenseClassID;
            this.ClassName = ClassName;
            this.ClassDescription = ClassDescription;
            this.MinimumAllowedAge = MinimumAllowedAge;
            this.DefaultValidityLength = DefaultValidityLength;
            this.ClassFees = ClassFees;
            Mode = enMode.Update;
            
        }

        public static DataTable GetAllLicenseClasses()
        {
            return clsLicenseClassesData.GetAllLicenseClasses();
        }

        private bool _AddNew()
        {
            this.LicenseClassID=clsLicenseClassesData.AddNewLicenseClass(this.ClassName, this.ClassDescription,this.MinimumAllowedAge,
                                 this.DefaultValidityLength,this.ClassFees);

            return this.LicenseClassID > -1;
        }

        private bool _Update()
        {
            return clsLicenseClassesData.UpdateLicensesClasse(this.LicenseClassID, this.ClassName, this.ClassDescription, this.MinimumAllowedAge,
                                 this.DefaultValidityLength, this.ClassFees);
        }


        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    return _AddNew();

                case enMode.Update:
                    return _Update();

            }
            return false;
        }
        public static clsLicenseClasses Find(int LicenseClassID)
        {
            string ClassName = "", ClassDescription = "";
            byte MinimumAllowedAge = 0, DefaultValidityLength = 0;
            float ClassFees = 0;

            if (clsLicenseClassesData.GetLicenseClassInfoByID(LicenseClassID, ref ClassName,
                        ref ClassDescription, ref MinimumAllowedAge, ref DefaultValidityLength, ref ClassFees))
            {
                return new clsLicenseClasses(LicenseClassID, ClassName, ClassDescription,
                                                    MinimumAllowedAge, DefaultValidityLength, ClassFees);
            }

            return null;
            
        }
        public static clsLicenseClasses Find(string ClassName)
        {
            int LicenseClassID = 0;
            string ClassDescription = "";
            byte MinimumAllowedAge = 0, DefaultValidityLength = 0;
            float ClassFees = 0;

            if (clsLicenseClassesData.GetLicenseClassnfoByName(ClassName, ref LicenseClassID,
                        ref ClassDescription, ref MinimumAllowedAge, ref DefaultValidityLength, ref ClassFees))
            {
                return new clsLicenseClasses(LicenseClassID, ClassName, ClassDescription,
                                                    MinimumAllowedAge, DefaultValidityLength, ClassFees);
            }

            return null;
            
        }


    }
}
