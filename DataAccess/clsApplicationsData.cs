
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DVLD_DataAccess
{
    public class clsApplicationsData
    {

        public static DataTable GetAllApplications()
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM Applications";

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

        public static bool GetApplicationInfoByID(int ApplicationID,
             ref int ApplicantPersonID, ref DateTime ApplicationDate, ref int ApplicationTypeID,
             ref byte ApplicationStatus, ref DateTime LastStatusDate,
             ref float PaidFees, ref int CreatedByUserID)
        {
            bool isFound = false;
            string query = "SELECT * FROM Applications WHERE ApplicationID = @ApplicationID";

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
                            ApplicantPersonID = (int)reader["ApplicantPersonID"];
                            ApplicationDate = (DateTime)reader["ApplicationDate"];
                            ApplicationTypeID = (int)reader["ApplicationTypeID"];
                            ApplicationStatus = (byte)reader["ApplicationStatus"];
                            LastStatusDate = (DateTime)reader["LastStatusDate"];
                            PaidFees = Convert.ToSingle(reader["PaidFees"]);
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

        public static bool IsApplicationExist(int ApplicationID)
        {
            string query = "SELECT Found = 1 FROM Applications WHERE ApplicationID = @ApplicationID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return false; }
        }

        public static int AddNewApplication(int ApplicantPersonID, DateTime ApplicationDate,
             int ApplicationTypeID, byte ApplicationStatus, DateTime LastStatusDate,
             float PaidFees, int CreatedByUserID)
        {
            string query = @"INSERT INTO Applications (ApplicantPersonID,ApplicationDate, ApplicationTypeID, 
                             ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID) 
                             VALUES (@ApplicantPersonID, @ApplicationDate, @ApplicationTypeID, 
                             @ApplicationStatus, @LastStatusDate, @PaidFees, @CreatedByUserID);
                             SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                    command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                    command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    return (result != null && int.TryParse(result.ToString(), out int id) ? id : -1);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return -1; }
        }

        public static bool UpdateApplication(int ApplicationID, int ApplicantPersonID,
              DateTime ApplicationDate, int ApplicationTypeID, byte ApplicationStatus,
              DateTime LastStatusDate, float PaidFees, int CreatedByUserID)
        {
            string query = @"UPDATE Applications SET ApplicantPersonID = @ApplicantPersonID, ApplicationDate = @ApplicationDate,
                             ApplicationTypeID = @ApplicationTypeID, ApplicationStatus = @ApplicationStatus,
                             LastStatusDate = @LastStatusDate, PaidFees = @PaidFees, CreatedByUserID = @CreatedByUserID
                             WHERE ApplicationID = @ApplicationID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                    command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                    command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return false; }
        }

        public static bool DeleteApplication(int ApplicationID)
        {
            string query = "DELETE FROM Applications WHERE ApplicationID = @ApplicationID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return false; }
        }

        public static int GetActiveApplicationID(int PersonID, int ApplicationTypeID)
        {
            string query = @"SELECT ApplicationID FROM Applications
                             WHERE ApplicantPersonID = @PersonID AND ApplicationTypeID = @ApplicationTypeID AND ApplicationStatus = 1";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    return (result != null && int.TryParse(result.ToString(), out int id) ? id : -1);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return -1; }
        }

        public static int GetActiveApplicationIDForLicenseClass(int PersonID, int ApplicationTypeID, int LicenseClassID)
        {
            string query = @"SELECT Applications.ApplicationID From Applications 
                             INNER JOIN LocalDrivingLicenseApplications ON Applications.ApplicationID = LocalDrivingLicenseApplications.ApplicationID
                             WHERE ApplicantPersonID = @ApplicantPersonID AND ApplicationTypeID = @ApplicationTypeID 
                             AND LocalDrivingLicenseApplications.LicenseClassID = @LicenseClassID AND ApplicationStatus = 1";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // تصحيح: اسم البارامتر كان @PersonID في الإسناد و @ApplicantPersonID في الكويري
                    command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    return (result != null && int.TryParse(result.ToString(), out int id) ? id : -1);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return -1; }
        }

        public static bool UpdataStatus(int ApplicationID, byte ApplicationStatus)
        {
            string query = "UPDATE Applications SET ApplicationStatus = @ApplicationStatus WHERE ApplicationID = @ApplicationID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return false; }
        }

        public static bool DoesPersonHaveActiveApplication(int PersonID, int ApplicationTypeID)
        {
            return (GetActiveApplicationID(PersonID, ApplicationTypeID) != -1);
        }



    }
}
