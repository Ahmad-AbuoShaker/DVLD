
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static DVLD_DataAccess.clsCountryData;
using static System.Net.Mime.MediaTypeNames;

namespace DVLD_DataAccess
{
    public class clsTestAppointmentsData
    {
        public static DataTable GetAllTestAppointments()
        {
            DataTable dt = new DataTable();
            string query = @"select * from TestAppointments_View
                             order by AppointmentDate Desc";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
            }

            return dt;
        }

        public static bool GetTestAppointmentInfoByID(int TestAppointmentID,
            ref int TestTypeID, ref int LocalDrivingLicenseApplicationID,
            ref DateTime AppointmentDate, ref float PaidFees, ref int CreatedByUserID, ref bool IsLocked, ref int RetakeTestApplicationID)
        {
            bool isFound = false;
            string query = "SELECT * FROM TestAppointments WHERE TestAppointmentID = @TestAppointmentID";

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

                            TestTypeID = (int)reader["TestTypeID"];
                            LocalDrivingLicenseApplicationID = (int)reader["LocalDrivingLicenseApplicationID"];
                            AppointmentDate = (DateTime)reader["AppointmentDate"];
                            CreatedByUserID = (int)reader["CreatedByUserID"];
                            PaidFees = Convert.ToSingle(reader["PaidFees"]);
                            IsLocked = (bool)reader["IsLocked"];
                            RetakeTestApplicationID = (reader["RetakeTestApplicationID"] == DBNull.Value) ? -1 : (int)reader["RetakeTestApplicationID"];
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

        public static bool GetLastTestAppointment(int LocalDrivingLicenseApplicationID, int TestTypeID,
            ref int TestAppointmentID, ref DateTime AppointmentDate,
            ref float PaidFees, ref int CreatedByUserID, ref bool IsLocked, ref int RetakeTestApplicationID)
        {
            bool isFound = false;
            string query = @"SELECT top 1 * FROM TestAppointments
                             WHERE (TestTypeID = @TestTypeID) 
                             AND (LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID) 
                             order by TestAppointmentID Desc";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            TestAppointmentID = (int)reader["TestAppointmentID"];
                            AppointmentDate = (DateTime)reader["AppointmentDate"];
                            PaidFees = Convert.ToSingle(reader["PaidFees"]);
                            CreatedByUserID = (int)reader["CreatedByUserID"];
                            IsLocked = (bool)reader["IsLocked"];
                            RetakeTestApplicationID = (reader["RetakeTestApplicationID"] == DBNull.Value) ? -1 : (int)reader["RetakeTestApplicationID"];
                        }
                    }
                }
            }
            catch { isFound = false; }

            return isFound;
        }

        public static DataTable GetApplicationTestAppointmentsPerTestType(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            DataTable dt = new DataTable();
            string query = @" SELECT TestAppointments.TestAppointmentID, TestAppointments.AppointmentDate,
                               TestAppointments.PaidFees, TestAppointments.IsLocked,
                               CASE WHEN Tests.TestResult = 1 THEN 'Passed'
                                    WHEN Tests.TestResult = 0 THEN 'Failed'
                                    WHEN Tests.TestResult IS NULL THEN 'Not taken yet' END AS TestResult
                               FROM TestAppointments LEFT JOIN Tests ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                               WHERE (TestTypeID = @TestTypeID) 
                               AND (LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID)
                               order by TestAppointmentID desc;";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
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

        public static int AddNewTestAppointment(int TestTypeID, int LocalDrivingLicenseApplicationID,
                                            DateTime AppointmentDate, float PaidFees, int CreatedByUserID, int RetakeTestApplicationID)
        {
            string query = @"Insert Into TestAppointments (TestTypeID,LocalDrivingLicenseApplicationID,AppointmentDate,PaidFees,CreatedByUserID,IsLocked,RetakeTestApplicationID)
                             Values (@TestTypeID,@LocalDrivingLicenseApplicationID,@AppointmentDate,@PaidFees,@CreatedByUserID,0,@RetakeTestApplicationID);
                             SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                    command.Parameters.AddWithValue("@RetakeTestApplicationID", (RetakeTestApplicationID == -1) ? DBNull.Value : (object)RetakeTestApplicationID);

                    connection.Open();
                    object result = command.ExecuteScalar();


                    return (result != null && int.TryParse(result.ToString(), out int insertedID) ? insertedID : -1);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
            }

            return -1;
        }

        public static bool UpdateTestAppointment(int TestAppointmentID, int TestTypeID, int LocalDrivingLicenseApplicationID,
                                             DateTime AppointmentDate, float PaidFees, int CreatedByUserID, bool IsLocked, int RetakeTestApplicationID)
        {
            string query = @"Update TestAppointments set TestTypeID = @TestTypeID,
                                LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID,
                                AppointmentDate = @AppointmentDate, PaidFees = @PaidFees,
                                CreatedByUserID = @CreatedByUserID, IsLocked=@IsLocked,
                                RetakeTestApplicationID=@RetakeTestApplicationID
                                where TestAppointmentID = @TestAppointmentID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                    command.Parameters.AddWithValue("@IsLocked", IsLocked);
                    command.Parameters.AddWithValue("@RetakeTestApplicationID", (RetakeTestApplicationID == -1) ? DBNull.Value : (object)RetakeTestApplicationID);

                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false; }
        }

        public static int GetTestID(int TestAppointmentID)
        {
            string query = @"select TestID from Tests where TestAppointmentID=@TestAppointmentID;";

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

    }
}
