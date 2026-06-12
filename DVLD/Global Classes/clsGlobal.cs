using DVLD_Buisness;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace DVLD.Classes
{
    internal static  class clsGlobal
    {
        public static clsUser CurrentUser;

        private static string _keyPath = @"HKEY_CURRENT_USER\SOFTWARE\DVLD";
        private static string _valueUserName = "CurrentUserName";
        private static string _valuePassword = "CurrentUserPassword";

        public static bool RememberUsernameAndPassword(string Username, string Password)
        {
            try
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
                    using (RegistryKey key = baseKey.CreateSubKey(_keyPath))
                    {
                        if (key != null)
                        {
                            if (string.IsNullOrEmpty(Username))
                            {
                                key.DeleteValue(_valueUserName, false); 
                                key.DeleteValue(_valuePassword, false);
                            }
                            else
                            {
                                // تخزين القيم
                                key.SetValue(_valueUserName, Username);
                                key.SetValue(_valuePassword, Password);
                            }
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registry Error: {ex.Message}");
                return false;
            }
        }

        public static bool GetStoredCredential(ref string Username, ref string Password)
        {
            try
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
                    using (RegistryKey key = baseKey.OpenSubKey(_keyPath))
                    {
                        if (key != null)
                        {
                            Username = key.GetValue(_valueUserName) as string;
                            Password = key.GetValue(_valuePassword) as string;

                            return !(string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password));
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

       
    }
}


