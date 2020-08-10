using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class EmployeeUserAccountDAL:IUserAccountDAL
    {
        private string connectionString;
        public EmployeeUserAccountDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public UserAccount Authorize(string userName, string password)
        {
            UserAccount data = new UserAccount();
            password = EmployeeDAL.GetMD5(password);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Employees WHERE Email=@Email and Password=@Password";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@Email", userName);
                cmd.Parameters.AddWithValue("@Password", password);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new UserAccount()
                        {
                            UserID=userName,
                            FullName= Convert.ToString(dbReader["FirstName"])+" "+ Convert.ToString(dbReader["LastName"]),
                            Photo=Convert.ToString(dbReader["PhotoPath"]),
                            GroupName=Convert.ToString(dbReader["GroupName"])
                        };
                    }
                }

                connection.Close();
            }
            return data;
            //return new UserAccount()
            //{
            //    UserID = userName,
            //    FullName = "Lê Quý Đôn",
            //    Photo = "don.png"
            //};
        }
        public UserProfile GetEmployee(Account account)
        {
            //account.Password = EmployeeDAL.GetMD5(account.Password);
            UserProfile data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Employees WHERE Email=@Email";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@Email", account.Email);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new UserProfile()
                        {
                            UserID = Convert.ToInt32(dbReader["EmployeeID"]),
                            Email = Convert.ToString(dbReader["Email"]),
                            Password = Convert.ToString(dbReader["Password"]),
                            Address = Convert.ToString(dbReader["Address"]),
                            BirthDate = Convert.ToDateTime(dbReader["BirthDate"]),
                            City = Convert.ToString(dbReader["City"]),
                            Country = Convert.ToString(dbReader["Country"]),
                            FirstName = Convert.ToString(dbReader["FirstName"]),
                            HireDate = Convert.ToDateTime(dbReader["HireDate"]),
                            HomePhone = Convert.ToString(dbReader["HomePhone"]),
                            LastName = Convert.ToString(dbReader["LastName"]),
                            Notes = Convert.ToString(dbReader["Notes"]),
                            PhotoPath = Convert.ToString(dbReader["PhotoPath"]),
                            Title = Convert.ToString(dbReader["Title"]),
                            GroupName = Convert.ToString(dbReader["GroupName"])
                        };
                    }
                }

                connection.Close();
            }
            if (data != null)
                return data;
            else
                return null;
        }
        public bool Update(Account account)
        {
            account.Password = EmployeeDAL.GetMD5(account.Password);
            //Account data = null;
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Employees SET
	                                                      Password=@Password                                        
                                                    WHERE Email=@Email SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@Password", account.Password);
                cmd.Parameters.AddWithValue("@Email", account.Email);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());
                connection.Close();
            }
            return rowsAffected > 0;
        }
        public bool Get(Account account)
        {
            account.Password = EmployeeDAL.GetMD5(account.Password);
            Account data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Employees WHERE Email=@Email and Password=@Password";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@Email", account.Email);
                cmd.Parameters.AddWithValue("@Password", account.Password);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new Account()
                        {
                            Email = Convert.ToString(dbReader["Email"]),
                            Password = Convert.ToString(dbReader["Password"]),
                        };
                    }
                }

                connection.Close();
            }
            if (data != null)
                return true;
            else
                return false;
        }

        public string GetCode(string email)
        {
            string data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT Code FROM Employees WHERE Email=@Email";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@Email", email);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = Convert.ToString(dbReader["Code"]);
                    }
                }

                connection.Close();
            }
            if (data != null)
                return data;
            else
                return null;
        }
        public bool SetCode(string email,string code)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Employees SET
	                                                      Code=@Code                                        
                                                    WHERE Email=@Email SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Code", code);
                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());
                connection.Close();
            }
            return rowsAffected > 0;
        }
    }
}
