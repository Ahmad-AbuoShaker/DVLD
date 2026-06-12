
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
        public class clsDriversData
        {
            public static DataTable GetAllDrivers()
            {
                DataTable dt = new DataTable();
                string query = "select * from Drivers_View";

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

            public static int AddNewDriver(int PersonID, int CreatedByUserID, DateTime CreatedDate)
            {
                string query = @"INSERT INTO Drivers (PersonID ,CreatedByUserID ,CreatedDate) 
                             VALUES (@PersonID,@CreatedByUserID ,@CreatedDate); 
                             SELECT SCOPE_IDENTITY();";

                try
                {
                    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        command.Parameters.AddWithValue("@CreatedDate", CreatedDate);

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

            public static bool UpdateDriver(int DriverID, int PersonID, int CreatedByUserID)
            {
                string query = @"UPDATE Drivers 
                             SET PersonID = @PersonID, CreatedByUserID = @CreatedByUserID 
                             WHERE DriverID = @DriverID";

                try
                {
                    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DriverID", DriverID);
                        command.Parameters.AddWithValue("@PersonID", PersonID);
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

            public static bool DeleteDriver(int DriverID)
            {
                string query = "DELETE FROM Drivers WHERE DriverID = @DriverID";

                try
                {
                    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DriverID", DriverID);
                        connection.Open();
                        return (command.ExecuteNonQuery() > 0);
                    }
                }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false; }
            }

            public static bool GetDriverInfoByID(int DriverID, ref int PersonID, ref int CreatedByUserID, ref DateTime CreatedDate)
            {
                bool isFound = false;
                string query = "SELECT * FROM Drivers WHERE DriverID = @DriverID";

                try
                {
                    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DriverID", DriverID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                PersonID = (int)reader["PersonID"];
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                                CreatedDate = (DateTime)reader["CreatedDate"];
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

            public static bool GetDriverInfoByPersonID(int PersonID, ref int DriverID, ref int CreatedByUserID, ref DateTime CreatedDate)
            {
                bool isFound = false;
                string query = "SELECT * FROM Drivers WHERE PersonID = @PersonID";

                try
                {
                    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                DriverID = (int)reader["DriverID"];
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                                CreatedDate = (DateTime)reader["CreatedDate"];
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

            public static int GetDriverIDByPersonID(int PersonID)
            {
                string query = "SELECT DriverID FROM Drivers WHERE PersonID = @PersonID";

                try
                {
                    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
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

            public static bool IsDriverExist(int DriverID)
            {
                string query = "SELECT Found=1 FROM Drivers WHERE DriverID = @DriverID";
                try
                {
                    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DriverID", DriverID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            return reader.HasRows;
                        }
                    }
                }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false; }
            }

            public static bool IsDriverExistForPersonID(int PersonID)
            {
                return (GetDriverIDByPersonID(PersonID) > -1);
            }
        }
    
}
