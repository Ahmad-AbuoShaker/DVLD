
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
    public class clsInternationalLicenseData
    {
        public static bool GetInternationalLicenseInfoByID(int InternationalLicenseID,
            ref int ApplicationID, ref int DriverID, ref int IssuedUsingLocalLicenseID,
            ref DateTime IssueDate, ref DateTime ExpirationDate, ref bool IsActive, ref int CreatedByUserID)
        {
            bool isFound = false;
            string query = "SELECT * FROM InternationalLicenses WHERE InternationalLicenseID = @InternationalLicenseID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            ApplicationID = (int)reader["ApplicationID"];
                            DriverID = (int)reader["DriverID"];
                            IssuedUsingLocalLicenseID = (int)reader["IssuedUsingLocalLicenseID"];
                            IssueDate = (DateTime)reader["IssueDate"];
                            ExpirationDate = (DateTime)reader["ExpirationDate"];
                            IsActive = (bool)reader["IsActive"];
                            CreatedByUserID = (int)reader["CreatedByUserID"]; // تم التصحيح هنا
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

        public static DataTable GetAllInternationalLicenses()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT InternationalLicenseID, ApplicationID, DriverID,
                             IssuedUsingLocalLicenseID, IssueDate, ExpirationDate, IsActive
                             FROM InternationalLicenses ORDER BY IsActive, ExpirationDate DESC";

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

        public static DataTable GetDriverInternationalLicenses(int DriverID)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT InternationalLicenseID, ApplicationID,
                             IssuedUsingLocalLicenseID, IssueDate, ExpirationDate, IsActive
                             FROM InternationalLicenses WHERE DriverID=@DriverID
                             ORDER BY ExpirationDate DESC";

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
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); }

            return dt;
        }

        public static int AddNewInternationalLicense(int ApplicationID,
             int DriverID, int IssuedUsingLocalLicenseID,
             DateTime IssueDate, DateTime ExpirationDate, bool IsActive, int CreatedByUserID)
        {
            string query = @"UPDATE InternationalLicenses SET IsActive=0 WHERE DriverID=@DriverID;
                             INSERT INTO InternationalLicenses (ApplicationID, DriverID, IssuedUsingLocalLicenseID, IssueDate, ExpirationDate, IsActive, CreatedByUserID)
                             VALUES (@ApplicationID, @DriverID, @IssuedUsingLocalLicenseID, @IssueDate, @ExpirationDate, @IsActive, @CreatedByUserID);
                             SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
                    command.Parameters.AddWithValue("@IssueDate", IssueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
                    command.Parameters.AddWithValue("@IsActive", IsActive);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    return (result != null && int.TryParse(result.ToString(), out int insertedID) ? insertedID : -1);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return -1; }
        }

        public static bool UpdateInternationalLicense(int InternationalLicenseID, int ApplicationID,
             int DriverID, int IssuedUsingLocalLicenseID,
             DateTime IssueDate, DateTime ExpirationDate, bool IsActive, int CreatedByUserID)
        {
            string query = @"UPDATE InternationalLicenses
                             SET ApplicationID=@ApplicationID, DriverID = @DriverID,
                                 IssuedUsingLocalLicenseID = @IssuedUsingLocalLicenseID, IssueDate = @IssueDate,
                                 ExpirationDate = @ExpirationDate, IsActive = @IsActive, CreatedByUserID = @CreatedByUserID
                             WHERE InternationalLicenseID=@InternationalLicenseID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
                    command.Parameters.AddWithValue("@IssueDate", IssueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
                    command.Parameters.AddWithValue("@IsActive", IsActive);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return false; }
        }

        public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
        {
            string query = @"SELECT TOP 1 InternationalLicenseID FROM InternationalLicenses 
                             WHERE DriverID=@DriverID AND IsActive = 1
                             ORDER BY ExpirationDate DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    return (result != null && int.TryParse(result.ToString(), out int id) ? id : -1);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return -1; }
        }
    }
}