
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsLicensesData
    {
        public static DataTable GetAllLicenses()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM Licenses";

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

        public static DataTable GetDriverLicenses(int DriverID)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT LicenseID, ApplicationID, ClassName, IssueDate, ExpirationDate, IsActive
                             FROM Licenses INNER JOIN LicenseClasses ON LicenseClass = LicenseClassID
                             WHERE (DriverID = @DriverID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DriverID", DriverID);
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

        public static int AddNewLicense(int ApplicationID, int DriverID, int LicenseClassID,
                                 DateTime IssueDate, DateTime ExpirationDate, string Notes, float PaidFees,
                                 bool IsActive, short IssueReason, int CreatedByUserID)
        {
            string query = @"INSERT INTO Licenses (ApplicationID, DriverID, LicenseClass, IssueDate, ExpirationDate, Notes, PaidFees, IsActive, IssueReason, CreatedByUserID)
                             VALUES (@ApplicationID, @DriverID, @LicenseClass, @IssueDate, @ExpirationDate, @Notes, @PaidFees, @IsActive, @IssueReason, @CreatedByUserID);
                             SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    command.Parameters.AddWithValue("@LicenseClass", LicenseClassID);
                    command.Parameters.AddWithValue("@IssueDate", IssueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
                    command.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(Notes) ? (object)DBNull.Value : Notes);
                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@IsActive", IsActive);
                    command.Parameters.AddWithValue("@IssueReason", IssueReason);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

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

        public static bool UpdateLicense(int LicenseID, int ApplicationID, int DriverID, int LicenseClassID,
                                 DateTime IssueDate, DateTime ExpirationDate, string Notes, float PaidFees,
                                 bool IsActive, byte IssueReason, int CreatedByUserID)
        {
            string query = @"UPDATE Licenses
                             SET ApplicationID = @ApplicationID, DriverID = @DriverID, LicenseClass = @LicenseClass,
                                 IssueDate = @IssueDate, ExpirationDate = @ExpirationDate, Notes = @Notes,
                                 PaidFees = @PaidFees, IsActive = @IsActive, IssueReason = @IssueReason,
                                 CreatedByUserID = @CreatedByUserID
                             WHERE LicenseID = @LicenseID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    command.Parameters.AddWithValue("@LicenseClass", LicenseClassID);
                    command.Parameters.AddWithValue("@IssueDate", IssueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
                    command.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(Notes) ? (object)DBNull.Value : Notes);
                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@IsActive", IsActive);
                    command.Parameters.AddWithValue("@IssueReason", IssueReason);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false; }
        }

        public static bool GetLicenseInfoByLicenseID(int LicenseID, ref int ApplicationID, ref int DriverID, ref int LicenseClassID,
                                             ref DateTime IssueDate, ref DateTime ExpirationDate, ref string Notes, ref float PaidFees,
                                             ref bool IsActive, ref byte IssueReason, ref int CreatedByUserID)
        {
            bool isFound = false;
            string query = "SELECT * FROM Licenses WHERE LicenseID = @LicenseID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            ApplicationID = (int)reader["ApplicationID"];
                            DriverID = (int)reader["DriverID"];
                            LicenseClassID = (int)reader["LicenseClass"];
                            IssueDate = (DateTime)reader["IssueDate"];
                            ExpirationDate = (DateTime)reader["ExpirationDate"];
                            Notes = (reader["Notes"] == DBNull.Value) ? "" : (string)reader["Notes"];
                            PaidFees = Convert.ToSingle(reader["PaidFees"]);
                            IsActive = (bool)reader["IsActive"];
                            IssueReason = (byte)reader["IssueReason"];
                            CreatedByUserID = (int)reader["CreatedByUserID"];
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

        public static int GetActiveLicenseIDByPersonID(int PersonID, int LicenseClassID)
        {
            string query = @"SELECT Licenses.LicenseID
                             FROM Licenses INNER JOIN Drivers ON Licenses.DriverID = Drivers.DriverID
                             WHERE Licenses.LicenseClass = @LicenseClass 
                             AND Drivers.PersonID = @PersonID AND IsActive=1";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@LicenseClass", LicenseClassID);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    return (result != null && int.TryParse(result.ToString(), out int id) ? id : -1);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return -1; }
        }

        public static bool GetLicenseInfoByApplicationID(int ApplicationID, ref int LicenseID, ref int DriverID, ref int LicenseClassID,
                                                 ref DateTime IssueDate, ref DateTime ExpirationDate, ref string Notes, ref float PaidFees,
                                                 ref bool IsActive, ref byte IssueReason, ref int CreatedByUserID)
        {
            bool isFound = false;
            string query = "SELECT * FROM Licenses WHERE ApplicationID = @ApplicationID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            LicenseID = (int)reader["LicenseID"];
                            DriverID = (int)reader["DriverID"];
                            LicenseClassID = (int)reader["LicenseClass"];
                            IssueDate = (DateTime)reader["IssueDate"];
                            ExpirationDate = (DateTime)reader["ExpirationDate"];
                            Notes = (reader["Notes"] == DBNull.Value) ? "" : (string)reader["Notes"];
                            PaidFees = Convert.ToSingle(reader["PaidFees"]);
                            IsActive = (bool)reader["IsActive"];
                            IssueReason = (byte)reader["IssueReason"];
                            CreatedByUserID = (int)reader["CreatedByUserID"];
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

        public static bool DeactivateLicense(int LicenseID)
        {
            string query = "UPDATE Licenses SET IsActive = 0 WHERE LicenseID=@LicenseID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
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
