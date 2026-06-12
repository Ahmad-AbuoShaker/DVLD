
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
    public class clsApplicationTypeData
    {
        public static DataTable GetAllApplicationType()
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM ApplicationTypes";

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

        public static bool GetApplicationTypeInfoByID(int ApplicationTypeID, ref string ApplicationTypeTitle, ref float ApplicationFees)
        {
            bool isFound = false;
            string query = "SELECT * FROM ApplicationTypes WHERE ApplicationTypeID = @ApplicationTypeID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            ApplicationTypeTitle = (string)reader["ApplicationTypeTitle"];
                            ApplicationFees = Convert.ToSingle(reader["ApplicationFees"]);
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

        public static bool GetApplicationTypeInfoByTitle(string ApplicationTypeTitle, ref int ApplicationTypeID, ref float ApplicationFees)
        {
            bool isFound = false;
            string query = "SELECT * FROM ApplicationTypes WHERE ApplicationTypeTitle = @ApplicationTypeTitle";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            ApplicationTypeID = (int)reader["ApplicationTypeID"];
                            ApplicationFees = Convert.ToSingle(reader["ApplicationFees"]);
                        }
                    }
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); isFound = false; }
            return isFound;
        }

        public static int AddNewApplicationType(string ApplicationTypeTitle, float ApplicationFees)
        {
            string query = @"INSERT INTO ApplicationTypes (ApplicationTypeTitle, ApplicationFees)
                             VALUES (@ApplicationTypeTitle, @ApplicationFees);
                             SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // تصحيح: توحيد اسم البارامتر مع الكويري
                    command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
                    command.Parameters.AddWithValue("@ApplicationFees", ApplicationFees);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    return (result != null && int.TryParse(result.ToString(), out int id) ? id : -1);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return -1; }
        }

        public static bool UpdateApplicationTypeID(int ApplicationTypeID, string ApplicationTypeTitle, float ApplicationFees)
        {
            string query = @"UPDATE ApplicationTypes
                             SET ApplicationTypeTitle = @ApplicationTypeTitle,
                                 ApplicationFees = @ApplicationFees
                             WHERE ApplicationTypeID = @ApplicationTypeID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
                    command.Parameters.AddWithValue("@ApplicationFees", ApplicationFees);
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return false; }
        }

        public static bool DeleteApplicationTypeID(int ApplicationTypeID)
        {
            string query = "DELETE FROM ApplicationTypes WHERE ApplicationTypeID = @ApplicationTypeID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    connection.Open();
                    return (command.ExecuteNonQuery() > 0);
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return false; }
        }

        public static bool IsApplicationTypeExist(string ApplicationTypeTitle)
        {
            string query = "SELECT Found=1 FROM ApplicationTypes WHERE ApplicationTypeTitle = @ApplicationTypeTitle";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
            catch (Exception ex) { clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error); return false; }
        }
    }
}
