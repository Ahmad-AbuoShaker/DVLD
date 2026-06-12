using DVLD_DataAccess;
using System.Data;

namespace DVLD_Buisness
{
    public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int PersonID { set; get; }
        public int UserID { set; get; }
        public string UserName { set; get; }
        public clsPerson PersonInfo;
        public string Password { set; get; }
        public bool IsActive { set; get; }

        public clsUser()
        {
            this.UserID = -1;
            this.PersonID = -1;
            this.UserName = "";
            this.Password = "";
            this.IsActive = true;
            Mode = enMode.AddNew;
        }

        private clsUser(int UserID, int PersonID,
            string UserName, string Password, bool IsActive)
        {
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.UserName = UserName;
            this.PersonInfo = clsPerson.Find(PersonID);
            this.Password = Password;
            this.IsActive = IsActive;
            Mode = enMode.Update;
        }


        public static DataTable GetAllUser()
        {
            return clsUserData.GetAllUser();
        }

        private bool _UpdateUser()
        {
            return clsUserData.UpdateUser(this.UserID, this.PersonID,
                this.UserName, this.Password, this.IsActive);
        }

        private bool _AddNewUser()
        {
            this.UserID = clsUserData.AddNewUser(this.PersonID,
                    this.UserName, this.Password, this.IsActive);
            return (this.UserID > -1);

        }

        public bool DeleteUser()
        {
            return clsUserData.DeleteUser(this.UserID);
        }
        public static bool DeleteUser(int UserID)
        {
            return clsUserData.DeleteUser(UserID);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                case enMode.Update:
                    return _UpdateUser();
            }

            return false;
        }

        public static clsUser FindByUserID(int UserID)
        {
            string UserName = "", Password = "";
            int PersonID = -1;

            bool IsActive = true;
            bool IsFound = clsUserData.GetUserInfoByID
                                (
                                    UserID, ref PersonID, ref UserName,
                                    ref Password, ref IsActive
                                );
            if (IsFound)
                return new clsUser(UserID, PersonID, UserName, Password, IsActive);
            else
                return null;
        }

        public static clsUser FindByPersonID(int PersonID)
        {
            string UserName = "", Password = "";
            int UserID = -1;

            bool IsActive = true;
            bool IsFound = clsUserData.GetUserInfoByPersonID
                                (
                                    PersonID, ref UserID, ref UserName,
                                    ref Password, ref IsActive
                                );
            if (IsFound)
                return new clsUser(UserID, PersonID, UserName, Password, IsActive);
            else
                return null;
        }

        public static clsUser FindByUserNameAndPassword(string UserName, string Password)
        {
            int UserID = -1, PersonID = -1;
            bool IsActive = true;
            bool IsFound = clsUserData.GetUserInfoByUserNameAndPassword
                                (
                                    UserName, Password, ref UserID, ref PersonID, ref IsActive
                                );
            if (IsFound)
                return new clsUser(UserID, PersonID, UserName, Password, IsActive);
            else
                return null;

        }

        public static bool isUserExist(int UserID)
        {
            return clsUserData.IsUserExist(UserID);
        }

        public static bool isUserExist(string UserName)
        {
            return clsUserData.IsUserExist(UserName);
        }

        public static bool isUserExistForPersonID(int PersonID)
        {
            return clsUserData.IsUserExistForPersonID(PersonID);
        }

    }
}
