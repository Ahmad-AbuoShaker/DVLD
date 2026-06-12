
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsLicenseClassesData
    {
        public static DataTable GetAllLicenseClasses()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM LicenseClasses";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows) dt.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
            }

            return dt;
        }

        public static bool GetLicenseClassInfoByID(int LicenseClassID, ref string ClassName,
                                                 ref string ClassDescription, ref byte MinimumAllowedAge,
                                                 ref byte DefaultValidityLength, ref float ClassFees)
        {
            bool isFound = false;
            string query = @"SELECT * FROM LicenseClasses WHERE LicenseClassID = @LicenseClassID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            ClassName = (string)reader["ClassName"];
                            ClassDescription = (string)reader["ClassDescription"];
                            MinimumAllowedAge = (byte)reader["MinimumAllowedAge"];
                            DefaultValidityLength = (byte)reader["DefaultValidityLength"];
                            ClassFees = Convert.ToSingle(reader["ClassFees"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                isFound = false; }

            return isFound;
        }

        public static bool GetLicenseClassnfoByName(string ClassName, ref int LicenseClassID,
                                                 ref string ClassDescription, ref byte MinimumAllowedAge,
                                                 ref byte DefaultValidityLength, ref float ClassFees)
        {
            bool isFound = false;
            string query = @"SELECT * FROM LicenseClasses WHERE ClassName = @ClassName";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassName", ClassName);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            LicenseClassID = (int)reader["LicenseClassID"];
                            ClassDescription = (string)reader["ClassDescription"];
                            MinimumAllowedAge = (byte)reader["MinimumAllowedAge"];
                            DefaultValidityLength = (byte)reader["DefaultValidityLength"];
                            ClassFees = Convert.ToSingle(reader["ClassFees"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                isFound = false; }

            return isFound;
        }

        public static int AddNewLicenseClass(string ClassName, string ClassDescription, byte MinimumAllowedAge,
                                           byte DefaultValidityLength, float ClassFees)
        {
            string query = @"INSERT INTO LicenseClasses (ClassName, ClassDescription, MinimumAllowedAge, DefaultValidityLength, ClassFees) 
                             VALUES (@ClassName, @ClassDescription, @MinimumAllowedAge, @DefaultValidityLength, @ClassFees);
                             SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassName", ClassName);
                    command.Parameters.AddWithValue("@ClassDescription", ClassDescription);
                    command.Parameters.AddWithValue("@MinimumAllowedAge", MinimumAllowedAge);
                    command.Parameters.AddWithValue("@DefaultValidityLength", DefaultValidityLength);
                    command.Parameters.AddWithValue("@ClassFees", ClassFees);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    return (result != null && int.TryParse(result.ToString(), out int insertedID) ? insertedID : -1);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return -1; }
        }

        public static bool UpdateLicensesClasse(int LicenseClassID, string ClassName, string ClassDescription, byte MinimumAllowedAge,
                                           byte DefaultValidityLength, float ClassFees)
        {
            string query = @"UPDATE LicenseClasses 
                             SET ClassName = @ClassName, ClassDescription = @ClassDescription, 
                                 MinimumAllowedAge = @MinimumAllowedAge, DefaultValidityLength = @DefaultValidityLength, 
                                 ClassFees = @ClassFees 
                             WHERE LicenseClassID = @LicenseClassID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                    command.Parameters.AddWithValue("@ClassName", ClassName);
                    command.Parameters.AddWithValue("@ClassDescription", ClassDescription);
                    command.Parameters.AddWithValue("@MinimumAllowedAge", MinimumAllowedAge);
                    command.Parameters.AddWithValue("@DefaultValidityLength", DefaultValidityLength);
                    command.Parameters.AddWithValue("@ClassFees", ClassFees);

                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false; }
        }

        public static bool DeleteLicenseClass(int LicenseClassID)
        {
            string query = @"DELETE FROM LicenseClasses WHERE LicenseClassID = @LicenseClassID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false; }
        }



    }
}
