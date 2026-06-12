
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
    public class clsTestTypeData
    {
        public static DataTable GetAllTestTypes()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT * FROM TestTypes ";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    {
                        connection.Open();
                        // Execute the query and obtain a SqlDataReader.
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }


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

        public static bool GetTestTypeInfoByID(int TestTypeID, ref string TestTypeTitle,
                                         ref string TestTypeDescription, ref decimal TestTypeFees)
        {
            bool isFound = false;

            string query = @"SELECT * FROM [TestTypes]
                             WHERE TestTypeID = @TestTypesID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@TestTypesID", TestTypeID);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            isFound = true;

                            TestTypeTitle = (string)reader["TestTypeTitle"];
                            TestTypeDescription = (string)reader["TestTypeDescription"];
                            TestTypeFees = (decimal)reader["TestTypeFees"];
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


        public static int AddNewTestType(string Title, string Description, decimal Fees)
        {
            string query = @"INSERT INTO TestTypes (TestTypeTitle, TestTypeDescription, TestTypeFees) 
                     VALUES (@Title, @Description, @Fees);
                     SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Description", Description);
                    command.Parameters.AddWithValue("@Title", Title);
                    command.Parameters.AddWithValue("@Fees", Fees);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    // العودة بالقيمة مباشرة
                    return (result != null && int.TryParse(result.ToString(), out int id)) ? id : -1;
                }
            }
            catch(Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return -1;
            }
        }
        public static bool UpdateTestType(int TestTypeID, string TestTypeDescription,
                                                     string TestTypeTitle, decimal TestTypeFees)
        {

            string query = @"UPDATE TestTypes
                             SET TestTypeTitle = @TestTypeTitle,
                             SET TestTypeDescription = @TestTypeDescription,
                                 TestTypeFees = @TestTypeFees
                                 WHERE TestTypeID = @TestTypeID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@TestTypeTitle", TestTypeTitle);
                    command.Parameters.AddWithValue("@TestTypeDescription", TestTypeFees);
                    command.Parameters.AddWithValue("@TestTypeFees", TestTypeFees);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

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

        public static bool DeleteTestTypeID(int TestTypeID)
        {
            string query = @"DELETE FROM TestTypes
                             WHERE TestTypeID = @TestTypeID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
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

    }
}
