using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class EmployeeDAL : IEmployeeDAL
    {
        private string connectionString;
        public EmployeeDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// mã hóa md5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5(string str)
        {

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder sbHash = new StringBuilder();

            foreach (byte b in bHash)
            {

                sbHash.Append(String.Format("{0:x2}", b));

            }

            return sbHash.ToString();

        }
        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }
        public int Add(Employee data, HttpPostedFileBase file)
        {
            //mã hóa md5
            data.Password = GetMD5(data.Password);
            //lưu ảnh vào thư mục
            if (data.PhotoPath != "")
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(HostingEnvironment.MapPath("~/Images"),fileName);
                file.SaveAs(path);
                var position = path.IndexOf("Images");
                var img = path.Substring(position);
                path = "\\" + img;
                data.PhotoPath = path;
            }
            int EmployeeID = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO Employees
                                          (
                                                LastName,
                                                FirstName,
                                                Title,
                                                BirthDate,
                                                HireDate,
                                                Email,
                                                Address,
                                                City,
                                                Country,
                                                HomePhone,
                                                Notes,
                                                PhotoPath,
                                                Password,
                                                GroupName

                                          )
                                          VALUES
                                          (
                                                @LastName,
                                                @FirstName,
                                                @Title,
                                                @BirthDate,
                                                @HireDate,
                                                @Email,
                                                @Address,
                                                @City,
                                                @Country,
                                                @HomePhone,
                                                @Notes,
                                                @PhotoPath,
                                                @Password,
                                                @GroupName
                                          );
                                          SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@LastName", data.LastName);
                cmd.Parameters.AddWithValue("@FirstName", data.FirstName);
                cmd.Parameters.AddWithValue("@Title", data.Title);
                cmd.Parameters.AddWithValue("@BirthDate", data.BirthDate);
                cmd.Parameters.AddWithValue("@HireDate", data.HireDate);
                cmd.Parameters.AddWithValue("@Email", data.Email);
                cmd.Parameters.AddWithValue("@Address", data.Address);
                cmd.Parameters.AddWithValue("@City", data.City);
                cmd.Parameters.AddWithValue("@Country", data.Country);
                cmd.Parameters.AddWithValue("@HomePhone", data.HomePhone);
                cmd.Parameters.AddWithValue("@Notes", data.Notes);
                cmd.Parameters.AddWithValue("@PhotoPath", data.PhotoPath);
                cmd.Parameters.AddWithValue("@Password", data.Password);
                cmd.Parameters.AddWithValue("@GroupName", data.GroupName);

                EmployeeID = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }
            return EmployeeID;
        }
        public bool CheckEmail(string email, string type)
        {
            List<Employee>  data = new List<Employee>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Employees WHERE Email = @Email";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@Email", email);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dbReader.Read())
                    {
                        data.Add(
                            new Employee()
                            {
                                EmployeeID = Convert.ToInt32(dbReader["EmployeeID"]),
                                LastName = Convert.ToString(dbReader["LastName"]),
                                FirstName = Convert.ToString(dbReader["FirstName"]),
                                Title = Convert.ToString(dbReader["Title"]),
                                BirthDate = Convert.ToDateTime(dbReader["BirthDate"]),
                                HireDate = Convert.ToDateTime(dbReader["HireDate"]),
                                Email = Convert.ToString(dbReader["Email"]),
                                Address = Convert.ToString(dbReader["Address"]),
                                City = Convert.ToString(dbReader["City"]),
                                Country = Convert.ToString(dbReader["Country"]),
                                HomePhone = Convert.ToString(dbReader["HomePhone"]),
                                Notes = Convert.ToString(dbReader["Notes"]),
                                PhotoPath = Convert.ToString(dbReader["PhotoPath"]),
                                Password = Convert.ToString(dbReader["Password"])
                            });
                    }
                }
                connection.Close();
            }
            if (type == "Edit")
            {
                if (data.Count > 1)
                    return false;
            }
            else
                if (data.Count > 0)
                return false;
            return true;
        }

        public int Count(string searchValue,string country)
        {
            int Count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            /// mở 1 kết nối cơ sở dữ liệu, khai báo 1 đối tương command đối tượng này chứa 1 câu lệnh dùng thực thi yêu cấu truy vấn cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = @"select count(*) as Count from Employees where ((@searchValue=N'') or (LastName like @searchValue )or(FirstName like @searchValue))and((@country=N'') or (Country = @country ))";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    if (searchValue == null)
                    {
                        cmd.Parameters.AddWithValue("@searchValue", "");
                        cmd.Parameters.AddWithValue("@country", country);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@searchValue", searchValue);
                        cmd.Parameters.AddWithValue("@country", country);
                    }
                    Count = Convert.ToInt32(cmd.ExecuteScalar());
                }
                connection.Close();
            }
            return Count;
        }

        public bool Delete(int[] employeeIDs)
        {
            //xóa file đã tồn tại
            foreach (int employeeid in employeeIDs)
            {
                var employeeExist = Get(employeeid);
                if (employeeExist.PhotoPath != "")
                {
                    try{
                        var fullPath = HostingEnvironment.MapPath(employeeExist.PhotoPath);
                        File.Delete(fullPath);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            bool result = true;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM Employees
                                            WHERE(EmployeeID = @EmployeeID) and EmployeeID NOT IN(select EmployeeID from Orders)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@EmployeeID", SqlDbType.Int);
                foreach (int employeeid in employeeIDs)
                {
                    cmd.Parameters["@EmployeeID"].Value = employeeid;
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
            return result;
        }

        public Employee Get(int employeeID)
        {
            Employee data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Employees WHERE EmployeeID = @EmployeeID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new Employee()
                        {
                            EmployeeID = Convert.ToInt32(dbReader["EmployeeID"]),
                            LastName = Convert.ToString(dbReader["LastName"]),
                            FirstName = Convert.ToString(dbReader["FirstName"]),
                            Title = Convert.ToString(dbReader["Title"]),
                            BirthDate = Convert.ToDateTime(dbReader["BirthDate"]),
                            HireDate = Convert.ToDateTime(dbReader["HireDate"]),
                            Email = Convert.ToString(dbReader["Email"]),
                            Address = Convert.ToString(dbReader["Address"]),
                            City = Convert.ToString(dbReader["City"]),
                            Country = Convert.ToString(dbReader["Country"]),
                            HomePhone = Convert.ToString(dbReader["HomePhone"]),
                            Notes = Convert.ToString(dbReader["Notes"]),
                            PhotoPath = Convert.ToString(dbReader["PhotoPath"]),
                            Password = Convert.ToString(dbReader["Password"]),
                            GroupName=Convert.ToString(dbReader["GroupName"])
                        };
                    }
                }

                connection.Close();
            }
            return data;
        }

        public List<Employee> List(int page, int pageSize, string searchValue,string country)
        {
            List<Employee> data = new List<Employee>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            //TODO:truy vấn dữ liệu từ cơ sở dữ liệu database
            /// mở 1 kết nối cơ sở dữ liệu, khai báo 1 đối tương command đối tượng này chứa 1 câu lệnh dùng thực thi yêu cấu truy vấn cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = @"select  *
                                        from(
		                                        select *,
		                                        ROW_NUMBER() over(order by EmployeeID) as RowNumber
                                        from	Employees where ((@searchValue=N'') or (LastName like @searchValue)or(FirstName like @searchValue))and((@country=N'') or (Country = @country))
                                        )as t
where t.RowNumber between (@page-1)*@pageSize + 1 and @page*@pageSize";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    if (searchValue == null)
                    {
                        cmd.Parameters.AddWithValue("@searchValue", "");
                        cmd.Parameters.AddWithValue("@country", country);
                        cmd.Parameters.AddWithValue("@page", page);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@searchValue", searchValue);
                        cmd.Parameters.AddWithValue("@country", country);
                        cmd.Parameters.AddWithValue("@page", page);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    }
                    using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dbReader.Read())
                        {
                            data.Add(new Employee()
                            {
                                EmployeeID = Convert.ToInt32(dbReader["EmployeeID"]),
                                LastName = Convert.ToString(dbReader["LastName"]),
                                FirstName = Convert.ToString(dbReader["FirstName"]),
                                Title = Convert.ToString(dbReader["Title"]),
                                BirthDate = Convert.ToDateTime(dbReader["BirthDate"]),
                                HireDate = Convert.ToDateTime(dbReader["HireDate"]),
                                Email = Convert.ToString(dbReader["Email"]),
                                Address = Convert.ToString(dbReader["Address"]),
                                City = Convert.ToString(dbReader["City"]),
                                Country = Convert.ToString(dbReader["Country"]),
                                HomePhone = Convert.ToString(dbReader["HomePhone"]),
                                Notes = Convert.ToString(dbReader["Notes"]),
                                PhotoPath = Convert.ToString(dbReader["PhotoPath"]),
                                Password = Convert.ToString(dbReader["Password"])
                            });
                        }
                    }
                }
            }
            return data;
        }

        public bool Update(Employee data, HttpPostedFileBase file)
        {
            var dataEmployessExist = Get(data.EmployeeID);
            if (file != null)
            {
                if (dataEmployessExist.PhotoPath == "")
                {
                    //add file mới nếu chưa có file ảnh
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Images"),fileName);
                    file.SaveAs(path);
                    var position = path.IndexOf("Images");
                    var img = path.Substring(position);
                    path = "\\" + img;
                    data.PhotoPath = path;
                }
                else
                {
                    // xóa file đã tồn tại
                    try {
                        var fullPath = HostingEnvironment.MapPath(dataEmployessExist.PhotoPath);
                        File.Delete(fullPath);
                    }
                    catch(Exception e) { }
                        
                    //add file mới vào
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Images"),fileName);
                    file.SaveAs(path);
                    var position = path.IndexOf("Images");
                    var img = path.Substring(position);
                    path = "\\" + img;
                    data.PhotoPath = path;
                }
            }
            else
                data.PhotoPath = dataEmployessExist.PhotoPath;
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Employees SET
                                                            LastName=@LastName,
                                                            FirstName=@FirstName,
                                                            Title=@Title,
                                                            BirthDate=@BirthDate,
                                                            HireDate=@HireDate,
                                                            Email=@Email,
                                                            Address=@Address,
                                                            City=@City,
                                                            Country=@Country,
                                                            HomePhone=@HomePhone,
                                                            Notes=@Notes,
                                                            PhotoPath=@PhotoPath,
                                                            Password=@Password,
                                                            GroupName=@GroupName
	                                                                                                              
                                                    WHERE EmployeeID=@EmployeeID
                                                    select @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@EmployeeID", data.EmployeeID);
                cmd.Parameters.AddWithValue("@FirstName", data.FirstName);
                cmd.Parameters.AddWithValue("@LastName", data.LastName);
                cmd.Parameters.AddWithValue("@Title", data.Title);
                cmd.Parameters.AddWithValue("@BirthDate", data.BirthDate);
                cmd.Parameters.AddWithValue("@HireDate", data.HireDate);
                cmd.Parameters.AddWithValue("@Email", data.Email);
                cmd.Parameters.AddWithValue("@Address", data.Address);
                cmd.Parameters.AddWithValue("@City", data.City);
                cmd.Parameters.AddWithValue("@Country", data.Country);
                cmd.Parameters.AddWithValue("@HomePhone", data.HomePhone);
                cmd.Parameters.AddWithValue("@Notes", data.Notes);
                cmd.Parameters.AddWithValue("@PhotoPath", data.PhotoPath);
                cmd.Parameters.AddWithValue("@Password", data.Password);
                cmd.Parameters.AddWithValue("@GroupName", data.GroupName);
                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());
                connection.Close();
            }
            return rowsAffected > 0;
        }
    }
}
