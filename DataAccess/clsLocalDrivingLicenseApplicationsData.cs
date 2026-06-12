
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
    public class clsLocalDrivingLicenseApplicationsData
    {
        public static DataTable GetAllLocalDrivingLicenseApplications()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT * FROM LocalDrivingLicenseApplications_View ";

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
            catch { }

            return dt;
        }

        public static bool GetLocalDrivingLicenseApplicationInfoByID(int LocalDrivingLicenseApplicationID,
                                                                   ref int ApplicationID, ref int LicenseClassID)
        {
            bool isFound = false;
            string query = @"SELECT * FROM LocalDrivingLicenseApplications 
                             WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            ApplicationID = (int)reader["ApplicationID"];
                            LicenseClassID = (int)reader["LicenseClassID"];
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

        public static bool GetLocalDrivingLicenseApplicationInfoByApplicationID(int ApplicationID, ref int LocalDrivingLicenseApplicationID,
                                                                              ref int LicenseClassID)
        {
            bool isFound = false;
            string query = "SELECT * FROM LocalDrivingLicenseApplications WHERE ApplicationID = @ApplicationID";

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
                            LocalDrivingLicenseApplicationID = (int)reader["LocalDrivingLicenseApplicationID"];
                            LicenseClassID = (int)reader["LicenseClassID"];
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

        public static int AddNewLocalDrivingLicenseApplication(int ApplicationID, int LicenseClassID)
        {
            string query = @"INSERT INTO LocalDrivingLicenseApplications (ApplicationID, LicenseClassID)
                             VALUES (@ApplicationID, @LicenseClassID);
                             SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
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

        public static bool UpdateLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID,
                                                             int ApplicationID, int LicenseClassID)
        {
            string query = @"UPDATE LocalDrivingLicenseApplications
                             SET ApplicationID = @ApplicationID, LicenseClassID = @LicenseClassID
                             WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
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

        public static bool DeleteLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID)
        {
            string query = @"DELETE FROM LocalDrivingLicenseApplications
                             WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false; }
        }

        public static bool DoesPassTestType(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            string query = @"SELECT top 1 TestResult
                             FROM LocalDrivingLicenseApplications INNER JOIN
                                  TestAppointments ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID INNER JOIN
                                  Tests ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                             WHERE (LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID) 
                             AND (TestAppointments.TestTypeID = @TestTypeID)
                             ORDER BY TestAppointments.TestAppointmentID desc";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    connection.Open();
                    object result = command.ExecuteScalar();

                    return (result != null && bool.TryParse(result.ToString(), out bool returnedResult) ? returnedResult : false);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false; }
        }

        public static bool DoesAttendTestType(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            string query = @"SELECT top 1 Found=1
                             FROM LocalDrivingLicenseApplications INNER JOIN
                                  TestAppointments ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID INNER JOIN
                                  Tests ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                             WHERE (LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID) 
                             AND (TestAppointments.TestTypeID = @TestTypeID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    connection.Open();
                    return (command.ExecuteScalar() != null);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false; }
        }

        public static byte TotalTrialsPerTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            string query = @"SELECT TotalTrialsPerTest = count(TestID)
                             FROM LocalDrivingLicenseApplications INNER JOIN
                                  TestAppointments ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID INNER JOIN
                                  Tests ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                             WHERE (LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID) 
                             AND (TestAppointments.TestTypeID = @TestTypeID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    connection.Open();
                    object result = command.ExecuteScalar();

                    return (result != null && byte.TryParse(result.ToString(), out byte Trials) ? Trials : (byte)0);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return 0; }
        }

        public static bool IsThereAnActiveScheduledTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            string query = @"SELECT top 1 Found=1
                             FROM LocalDrivingLicenseApplications INNER JOIN
                                  TestAppointments ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID 
                             WHERE (LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID)  
                             AND (TestAppointments.TestTypeID = @TestTypeID) and isLocked=0
                             ORDER BY TestAppointments.TestAppointmentID desc";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    connection.Open();
                    return (command.ExecuteScalar() != null);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false; }
        }


    }
}
