using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class AccountBLL : IAccountDAL
    {
        private string connectionString;
        public AccountBLL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public int Add(Account data)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int[] accountIDs)
        {
            throw new NotImplementedException();
        }

        public Account Get(int accountID)
        {
            throw new NotImplementedException();
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
        public Employee GetEmployee(Account account)
        {
            //account.Password = EmployeeDAL.GetMD5(account.Password);
            Employee data = null;
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
                        data = new Employee()
                        {
                            EmployeeID= Convert.ToInt32(dbReader["EmployeeID"]),
                            Email = Convert.ToString(dbReader["Email"]),
                            Password = Convert.ToString(dbReader["Password"]),
                            Address= Convert.ToString(dbReader["Address"]),
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
        public List<Account> List(int page, int pageSize, string searchValue)
        {
            throw new NotImplementedException();
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
    }
}
