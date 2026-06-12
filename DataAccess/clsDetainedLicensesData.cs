
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
    public class clsDetainedLicensesData
    {
        public static bool GetDetainedLicenseInfoByID(int DetainID,
               ref int LicenseID, ref DateTime DetainDate,
               ref float FineFees, ref int CreatedByUserID,
               ref bool IsReleased, ref DateTime ReleaseDate,
               ref int ReleasedByUserID, ref int ReleaseApplicationID)
        {
            bool isFound = false;
            string query = "SELECT * FROM DetainedLicenses WHERE DetainID = @DetainID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DetainID", DetainID);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            LicenseID = (int)reader["LicenseID"];
                            DetainDate = (DateTime)reader["DetainDate"];
                            FineFees = Convert.ToSingle(reader["FineFees"]);
                            CreatedByUserID = (int)reader["CreatedByUserID"];
                            IsReleased = (bool)reader["IsReleased"];
                            ReleaseDate = (reader["ReleaseDate"] == DBNull.Value) ? DateTime.MaxValue : (DateTime)reader["ReleaseDate"];
                            ReleasedByUserID = (reader["ReleasedByUserID"] == DBNull.Value) ? -1 : (int)reader["ReleasedByUserID"];
                            ReleaseApplicationID = (reader["ReleaseApplicationID"] == DBNull.Value) ? -1 : (int)reader["ReleaseApplicationID"];
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

        public static bool GetDetainedLicenseInfoByLicenseID(int LicenseID,
         ref int DetainID, ref DateTime DetainDate,
         ref float FineFees, ref int CreatedByUserID,
         ref bool IsReleased, ref DateTime ReleaseDate,
         ref int ReleasedByUserID, ref int ReleaseApplicationID)
        {
            bool isFound = false;
            string query = "SELECT top 1 * FROM DetainedLicenses WHERE LicenseID = @LicenseID order by DetainID desc";

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
                            DetainID = (int)reader["DetainID"];
                            DetainDate = (DateTime)reader["DetainDate"];
                            FineFees = Convert.ToSingle(reader["FineFees"]);
                            CreatedByUserID = (int)reader["CreatedByUserID"];
                            IsReleased = (bool)reader["IsReleased"];
                            ReleaseDate = (reader["ReleaseDate"] == DBNull.Value) ? DateTime.MaxValue : (DateTime)reader["ReleaseDate"];
                            ReleasedByUserID = (reader["ReleasedByUserID"] == DBNull.Value) ? -1 : (int)reader["ReleasedByUserID"];
                            ReleaseApplicationID = (reader["ReleaseApplicationID"] == DBNull.Value) ? -1 : (int)reader["ReleaseApplicationID"];
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

        public static DataTable GetAllDetainedLicenses()
        {
            DataTable dt = new DataTable();
            string query = "select * from detainedLicenses_View order by IsReleased ,DetainID;";

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

        public static int AddNewDetainedLicense(int LicenseID, DateTime DetainDate, float FineFees, int CreatedByUserID)
        {
            string query = @"INSERT INTO dbo.DetainedLicenses (LicenseID, DetainDate, FineFees, CreatedByUserID, IsReleased)
                             VALUES (@LicenseID, @DetainDate, @FineFees, @CreatedByUserID, 0);
                             SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    command.Parameters.AddWithValue("@DetainDate", DetainDate);
                    command.Parameters.AddWithValue("@FineFees", FineFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    return (result != null && int.TryParse(result.ToString(), out int id) ? id : -1);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return -1; }
        }

        public static bool UpdateDetainedLicense(int DetainID, int LicenseID, DateTime DetainDate, float FineFees, int CreatedByUserID)
        {
            string query = @"UPDATE dbo.DetainedLicenses SET LicenseID = @LicenseID, DetainDate = @DetainDate, 
                             FineFees = @FineFees, CreatedByUserID = @CreatedByUserID WHERE DetainID=@DetainID;";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // تصحيح: اسم البارامتر يجب أن يطابق الكويري
                    command.Parameters.AddWithValue("@DetainID", DetainID);
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    command.Parameters.AddWithValue("@DetainDate", DetainDate);
                    command.Parameters.AddWithValue("@FineFees", FineFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return false; }
        }

        public static bool IsLicenseDetained(int LicenseID)
        {
            string query = "SELECT Found=1 FROM DetainedLicenses WHERE LicenseID = @LicenseID AND IsReleased = 0";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return false; }
        }

        public static bool ReleaseDetainedLicense(int DetainID, int ReleasedByUserID, DateTime ReleaseDate, int ReleaseApplicationID)
        {
            string query = @"UPDATE dbo.DetainedLicenses
                             SET IsReleased = 1, ReleaseDate = @ReleaseDate, 
                                 ReleasedByUserID=@ReleasedByUserID, ReleaseApplicationID = @ReleaseApplicationID   
                             WHERE DetainID=@DetainID;";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DetainID", DetainID);
                    command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
                    command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);
                    command.Parameters.AddWithValue("@ReleaseDate", ReleaseDate);

                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return false; }
        }

    }
}
