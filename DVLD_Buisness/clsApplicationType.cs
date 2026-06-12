using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Buisness
{
    public class clsApplicationType
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public string Title { get; set; }
        public float Fees { get; set; }

        public clsApplicationType()
        {
            this.ID = -1;
            this.Title = "";
            this.Fees = 0;
            Mode = enMode.AddNew;
        }

        public clsApplicationType(int ApplicationTypeID, string ApplicationTypeTitle, float ApplicationFees)
        {
            this.ID = ApplicationTypeID;
            this.Title = ApplicationTypeTitle;
            this.Fees = ApplicationFees;
            Mode = enMode.Update;
        }

        public static DataTable GetAllApplicationType()
        {
            return clsApplicationTypeData.GetAllApplicationType();
        }

        public static bool IsApplicationTypeExist(string ApplicationTypeTitle)
        {
            return clsApplicationType.IsApplicationTypeExist(ApplicationTypeTitle);
        }

        public static clsApplicationType Find(int ApplicationTypeID)
        {
            string ApplicationTypeTitl = "";
            float ApplicationFees = 0;
            bool IsFound = clsApplicationTypeData.GetApplicationTypeInfoByID
                                            (ApplicationTypeID,ref ApplicationTypeTitl, ref ApplicationFees);
            if (IsFound)
            {
                return new clsApplicationType(ApplicationTypeID, ApplicationTypeTitl, ApplicationFees);
            }
            else
            {
                return null;
            }
        }

        public static clsApplicationType Find(string ApplicationTypeTitl)
        {
            int ApplicationTypeID = -1;
            float ApplicationFees = 0;
            bool IsFound = clsApplicationTypeData.GetApplicationTypeInfoByTitle
                                            (ApplicationTypeTitl, ref ApplicationTypeID, ref ApplicationFees);
            if (IsFound)
            {
                return new clsApplicationType(ApplicationTypeID, ApplicationTypeTitl, ApplicationFees);
            }
            else
            {
                return null;
            }
        }

        private  bool _AddNewApplicationType()
        {
            this.ID = clsApplicationTypeData.AddNewApplicationType(this.Title, 
                                                                                         this.Fees);
            return this.ID > -1;
        }

        private bool _UpdateApplicationType()
        {
            return clsApplicationTypeData.UpdateApplicationTypeID(this.ID, 
                                            this.Title, this.Fees);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(_AddNewApplicationType())
                    { Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateApplicationType();

            }
            return false;
        }

        public bool DeleteApplicationType() 
        {
           return clsApplicationTypeData.DeleteApplicationTypeID(this.ID);
        }
        
    
    }
}
