
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
    public class clsTestsData
    {
     
        public static DataTable GetAllTests()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT * From Tests ";

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
        public static bool GetTestByID(int TestID, ref int TestAppointmentID,
                              ref bool TestResult, ref string Notes, ref int CreatedByUserID)
        {
            bool isFound = false;
            string query = @"SELECT * FROM Tests WHERE TestID = @TestID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestID", TestID);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;

                            TestAppointmentID = (int)reader["TestAppointmentID"];
                            TestResult = (bool)reader["TestResult"];
                            Notes = (reader["Notes"] == DBNull.Value) ? "" : (string)reader["Notes"];
                            CreatedByUserID = (int)reader["CreatedByUserID"];
                        }
                    }
                }
            }
            catch { isFound = false; }

            return isFound;
        }

        public static bool GetTestByTestAppointmentID(int TestAppointmentID, ref int TestID,
                               ref bool TestResult, ref string Notes, ref int CreatedByUserID)
        {
            bool isFound = false;
            string query = @"SELECT * FROM Tests WHERE TestAppointmentID = @TestAppointmentID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            TestID = (int)reader["TestID"];
                            TestResult = (bool)reader["TestResult"];
                            Notes = (reader["Notes"] == DBNull.Value) ? "" : (string)reader["Notes"];
                            CreatedByUserID = (int)reader["CreatedByUserID"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); 
                isFound = false;
            }

            return isFound;
        }

        public static int GetTestIDByTestAppointmentID(int TestAppointmentID)
        {
            string query = @"SELECT TestID FROM Tests WHERE TestAppointmentID = @TestAppointmentID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
                    connection.Open();
                    object result = command.ExecuteScalar();

                    return (result != null && int.TryParse(result.ToString(), out int TestID) ? TestID : -1);
                   
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return -1;
            }
        }

        public static int AddNewTest(int TestAppointmentID, bool TestResult, string Notes, int CreatedByUserID)
        {
            string query = @"INSERT INTO Tests (TestAppointmentID, TestResult, Notes, CreatedByUserID) 
                             VALUES (@TestAppointmentID, @TestResult, @Notes, @CreatedByUserID);
                             UPDATE TestAppointments SET IsLocked=1 WHERE TestAppointmentID = @TestAppointmentID;
                             SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
                    command.Parameters.AddWithValue("@TestResult", TestResult);
                    command.Parameters.AddWithValue("@Notes", (object)Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    return (result != null && int.TryParse(result.ToString(), out int NewTestID) ? NewTestID : -1);


                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return -1;
            }
        }

        public static bool UpdateTest(int TestID, int TestAppointmentID, bool TestResult, string Notes, int CreatedByUserID)
        {
            string query = @"UPDATE Tests SET TestAppointmentID = @TestAppointmentID, TestResult = @TestResult, 
                             Notes = @Notes, CreatedByUserID = @CreatedByUserID WHERE TestID = @TestID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestID", TestID);
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
                    command.Parameters.AddWithValue("@TestResult", TestResult);
                    command.Parameters.AddWithValue("@Notes", (object)Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false; 
            }
        }

        public static bool DeleteTest(int TestID)
        {
            string query = @"DELETE FROM Tests WHERE TestID = @TestID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestID", TestID);
                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false;
            }

        }

        public static byte GetPassedTestCount(int LocalDrivingLicenseApplicationID)
        {
            string query = @"SELECT COUNT(TestAppointments.TestTypeID) FROM Tests INNER JOIN
                             TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
                             WHERE (TestAppointments.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID) AND (Tests.TestResult = 1)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    connection.Open();
                    object result = command.ExecuteScalar();

                    return (result != null && byte.TryParse(result.ToString(), out byte PassedTestCount) ? PassedTestCount : (byte)0);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return 0;
            }
        }

        public static bool GetLastTestByPersonAndTestTypeAndLicenseClass(int PersonID, int LicenseClassID, int TestTypeID,
            ref int TestID, ref int TestAppointmentID, ref bool TestResult, ref string Notes, ref int CreatedByUserID)
        {
            bool isFound = false;
            string query = @"SELECT top 1 Tests.TestID, Tests.TestAppointmentID, Tests.TestResult, Tests.Notes, Tests.CreatedByUserID
                             FROM LocalDrivingLicenseApplications 
                             INNER JOIN TestAppointments ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID 
                             INNER JOIN Tests ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID 
                             INNER JOIN Applications ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID
                             WHERE (Applications.ApplicantPersonID = @PersonID) 
                               AND (LocalDrivingLicenseApplications.LicenseClassID = @LicenseClassID)
                               AND (TestAppointments.TestTypeID = @TestTypeID)
                             ORDER BY Tests.TestAppointmentID DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            TestID = (int)reader["TestID"];
                            TestAppointmentID = (int)reader["TestAppointmentID"];
                            TestResult = (bool)reader["TestResult"];
                            Notes = (reader["Notes"] == DBNull.Value) ? "" : (string)reader["Notes"];
                            CreatedByUserID = (int)reader["CreatedByUserID"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                isFound = false; 
            }

            return isFound;
        }
    }
}
    


