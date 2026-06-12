
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace DVLD_DataAccess
{
    public class clsUserData
    {
        public static DataTable GetAllUser()
        {

            DataTable dt = new DataTable();


            string query = @"SELECT Users.PersonID, Users.UserID, People.FirstName + ' ' + People.SecondName + ' ' + ISNULL( People.ThirdName,'') +' ' + People.LastName,
         Users.UserName, Users.IsActive
          FROM Users INNER JOIN People 
            ON Users.PersonID = People.PersonID";

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
                clsDataAccessSettings.LogEvent(ex.ToString(),EventLogEntryType.Error);
            }

            return dt;
        }


        public static bool GetUserInfoByID(int UserID, ref int PersonID,
               ref string UserName, ref string Password, ref bool IsActive)
        {
            bool isFound = false;

            string query = @"SELECT PersonID, UserName, Password, IsActive
                             FROM Users
                             WHERE UserID = @UserID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;

                            PersonID = (int)reader["PersonID"];
                            UserName = (string)reader["UserName"];
                            Password = (string)reader["Password"];
                            IsActive = (bool)reader["IsActive"];
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

        public static bool GetUserInfoByPersonID(int PersonID, ref int UserID,
            ref string UserName, ref string Password, ref bool IsActive)
        {
            bool isFound = false;
            string query = @"SELECT UserID, UserName, Password, IsActive
                             FROM Users
                             WHERE PersonID = @PersonID";
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

                            UserID = (int)reader["UserID"];
                            UserName = (string)reader["UserName"];
                            Password = (string)reader["Password"];
                            IsActive = (bool)reader["IsActive"];
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

        public static bool GetUserInfoByUserNameAndPassword(string UserName, string Password,
            ref int UserID, ref int PersonID, ref bool IsActive)
        {
            bool isFound = false;
            string query = @"SELECT UserID, PersonID, IsActive
                             FROM Users
                             WHERE UserName = @UserName AND Password = @Password";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            isFound = true;

                            UserID = (int)reader["UserID"];
                            PersonID = (int)reader["PersonID"];
                            IsActive = (bool)reader["IsActive"];
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

        public static int AddNewUser(int PersonID, string UserName, string Password, bool IsActive)
        {
            int UserID = -1;

            string query = @"INSERT INTO Users (PersonID, UserName, Password, IsActive)
                             VALUES (@PersonID, @UserName, @Password, @IsActive);
                                 SELECT SCOPE_IDENTITY();";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);
                    command.Parameters.AddWithValue("@IsActive", IsActive);

                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        UserID = insertedID;
                    }
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
            }
            return UserID;

        }

        public static bool UpdateUser(int UserID, int PersonID, string UserName, string Password, bool IsActive)
        {
            string query = @"UPDATE Users
                             SET PersonID = @PersonID,
                                 UserName = @UserName,
                                 Password = @Password,
                                 IsActive = @IsActive
                             WHERE UserID = @UserID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);
                    command.Parameters.AddWithValue("@IsActive", IsActive);
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

        public static bool DeleteUser(int UserID)
        {
            string query = @"DELETE FROM Users
                             WHERE UserID = @UserID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@UserID", UserID);
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

        public static bool IsUserExist(int UserID)
        {

            string query = "SELECT Found=1 FROM Users WHERE UserID = @UserID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    connection.Open();

                    object result = command.ExecuteScalar();
                    return (result != null);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false;
            }

        }

        public static bool IsUserExistForPersonID(int PersonID)
        {
            string query = "SELECT Found=1 FROM Users WHERE PersonID = @PersonID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    connection.Open();

                    object result = command.ExecuteScalar();
                    return (result != null);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false;
            }
        }

        public static bool IsUserExist(string UserName)
        {
            string query = "SELECT Found=1 FROM Users WHERE UserName = @UserName";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);
                    connection.Open();

                    object result = command.ExecuteScalar();
                    return (result != null);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false;
            }

        }

        public static bool DoesPersonHaveUser(int PersonID)
        {
            return IsUserExistForPersonID(PersonID);
        }

        public static bool ChangePassword(int UserID, string NewPassword)
        {
            string query = @"UPDATE Users
                             SET Password = @NewPassword
                             WHERE UserID = @UserID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@NewPassword", NewPassword);

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