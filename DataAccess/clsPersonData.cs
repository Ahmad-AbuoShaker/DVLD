
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DVLD_DataAccess
{
    public class clsPersonData
    {

        public static bool GetPersonInfoByID(int PersonID, ref string FirstName, ref string SecondName,
          ref string ThirdName, ref string LastName, ref string NationalNo, ref DateTime DateOfBirth,
           ref short Gendor, ref string Address, ref string Phone, ref string Email,
           ref int NationalityCountryID, ref string ImagePath)
        {
            bool isFound = false;

            string query = "SELECT * FROM People WHERE PersonID = @PersonID";

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

                            FirstName = (string)reader["FirstName"];
                            SecondName = (string)reader["SecondName"];
                            LastName = (string)reader["LastName"];
                            NationalNo = (string)reader["NationalNo"];
                            DateOfBirth = (DateTime)reader["DateOfBirth"];
                            Gendor = Convert.ToInt16(reader["Gendor"]);
                            Address = (string)reader["Address"];
                            Phone = (string)reader["Phone"];
                            NationalityCountryID = (int)reader["NationalityCountryID"];

                            ThirdName = reader["ThirdName"] as string ?? "";
                            Email = reader["Email"] as string ?? "";
                            ImagePath = reader["ImagePath"] as string ?? "";
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
            

        public static bool GetPersonInfoByNationalNo(string NationalNo, ref int PersonID, ref string FirstName, ref string SecondName,
        ref string ThirdName, ref string LastName,   ref DateTime DateOfBirth,
         ref short Gendor,ref string Address, ref string Phone, ref string Email,
         ref int NationalityCountryID, ref string ImagePath)
        {
            bool isFound = false;
            string query = "SELECT * FROM People WHERE NationalNo = @NationalNo";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;

                            PersonID = (int)reader["PersonID"];
                            FirstName = (string)reader["FirstName"];
                            SecondName = (string)reader["SecondName"];
                            LastName = (string)reader["LastName"];
                            DateOfBirth = (DateTime)reader["DateOfBirth"];
                            Gendor = Convert.ToInt16(reader["Gendor"]);
                            Address = (string)reader["Address"];
                            Phone = (string)reader["Phone"];
                            NationalityCountryID = (int)reader["NationalityCountryID"];

                            ThirdName = reader["ThirdName"] as string ?? "";
                            Email = reader["Email"] as string ?? "";
                            ImagePath = reader["ImagePath"] as string ?? "";
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

        public static int AddNewPerson(string FirstName, string SecondName, string ThirdName, string LastName,
            string NationalNo, DateTime DateOfBirth, short Gendor, string Address, string Phone, string Email,
            int NationalityCountryID, string ImagePath)
        {

            string query = @"INSERT INTO People (FirstName, SecondName, ThirdName,LastName,NationalNo,
                                                   DateOfBirth,Gendor,Address,Phone, Email, NationalityCountryID,ImagePath)
                             VALUES (@FirstName, @SecondName,@ThirdName, @LastName, @NationalNo,
                                     @DateOfBirth,@Gendor,@Address,@Phone, @Email,@NationalityCountryID,@ImagePath);
                             SELECT SCOPE_IDENTITY();";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName);
                    command.Parameters.AddWithValue("@ThirdName", (object)ThirdName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    command.Parameters.AddWithValue("@Gendor", Gendor);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Email", (object)Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
                    command.Parameters.AddWithValue("@ImagePath", (object)ImagePath ?? DBNull.Value);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    return (result != null && int.TryParse(result.ToString(), out int insertedID) ? insertedID : -1);


                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return -1;
            }
        }

        public static bool UpdatePerson(int PersonID, string FirstName, string SecondName, string ThirdName, string LastName,
            string NationalNo, DateTime DateOfBirth, short Gendor, string Address, string Phone, string Email,
            int NationalityCountryID, string ImagePath)
        {
            string query = @"Update  People  
                            set FirstName = @FirstName,
                                SecondName = @SecondName,
                                ThirdName = @ThirdName,
                                LastName = @LastName, 
                                NationalNo = @NationalNo,
                                DateOfBirth = @DateOfBirth,
                                Gendor=@Gendor,
                                Address = @Address,  
                                Phone = @Phone,
                                Email = @Email, 
                                NationalityCountryID = @NationalityCountryID,
                                ImagePath =@ImagePath
                                where PersonID = @PersonID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName);
                    command.Parameters.AddWithValue("@ThirdName", (object)ThirdName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    command.Parameters.AddWithValue("@Gendor", Gendor);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Email", (object)Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
                    command.Parameters.AddWithValue("@ImagePath", (object)ImagePath ?? DBNull.Value);

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
       
        public static DataTable GetAllPeople()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT People.PersonID, People.NationalNo,
              People.FirstName, People.SecondName, People.ThirdName, People.LastName,
			  People.DateOfBirth, People.Gendor,  
				  CASE
                  WHEN People.Gendor = 0 THEN 'Male'

                  ELSE 'Female'

                  END as GendorCaption ,
			  People.Address, People.Phone, People.Email, 
              People.NationalityCountryID, Countries.CountryName, People.ImagePath
              FROM            People INNER JOIN
                         Countries ON People.NationalityCountryID = Countries.CountryID
                ORDER BY People.FirstName";

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

        public static bool DeletePerson(int PersonID)
        {

            string query = @"Delete People 
                                where PersonID = @PersonID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
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

        public static bool IsPersonExist(int PersonID)
        {
            string query = "SELECT Found=1 FROM People WHERE PersonID = @PersonID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    connection.Open();

                    return (command.ExecuteScalar() != null);
                }
            }
            catch (Exception ex)
            {
                clsDataAccessSettings.LogEvent(ex.ToString(), EventLogEntryType.Error);
                return false;
            }
        }

        public static bool IsPersonExist(string NationalNo)
        {
            string query = "SELECT Found=1 FROM People WHERE NationalNo = @NationalNo";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);
                    connection.Open();

                    return (command.ExecuteScalar() != null);
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
