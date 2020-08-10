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
    public class CategoryDAL : ICategoryDAL
    {
        private string connectionString;
        public CategoryDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public int Add(Category data)
        {
            int CategoryID = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO Categories
                                          (
	                                          CategoryName,
                                              Description
                                          )
                                          VALUES
                                          (
	                                          @CategoryName,
                                              @Description
                                          );
                                          SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@CategoryName", data.CategoryName);
                cmd.Parameters.AddWithValue("@Description", data.Description);

                CategoryID = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }

            return CategoryID;
        }

        public int Count(string searchValue)
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
                    cmd.CommandText = @"select count(*) as Count from Categories where (@searchValue=N'') or (CategoryName like @searchValue )";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    if (searchValue == null)
                    {
                        cmd.Parameters.AddWithValue("@searchValue", "");
                    }
                    else
                        cmd.Parameters.AddWithValue("@searchValue", searchValue);
                    Count = Convert.ToInt32(cmd.ExecuteScalar());
                }
                connection.Close();
            }
            return Count;
        }
        public bool Delete(int[] categoryIDs)
        {
            bool result = true;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM categories
                                            WHERE(CategoryID = @categoryID) and CategoryID NOT IN (select CategoryID from Products)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@categoryID", SqlDbType.Int);
                foreach (int categoryID in categoryIDs)
                {
                    cmd.Parameters["@categoryID"].Value = categoryID;
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
            return result;
        }

        public Category Get(int categoryID)
        {
            Category data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Categories WHERE CategoryID = @categoryID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@categoryID", categoryID);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new Category()
                        {
                            CategoryID= Convert.ToInt32(dbReader["CategoryID"]),
                            CategoryName = Convert.ToString(dbReader["CategoryName"]),
                            Description = Convert.ToString(dbReader["Description"]),
                        };
                    }
                }

                connection.Close();
            }
            return data;
        }

        public List<Category> List(string searchValue)
        {
            List<Category> data = new List<Category>();
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
		                                        ROW_NUMBER() over(order by CategoryID) as RowNumber
                                        from	Categories where (@searchValue=N'') or (CategoryName like @searchValue)
                                        )as t";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    if (searchValue == null)
                    {
                        cmd.Parameters.AddWithValue("@searchValue", "");
                    }
                    else
                        cmd.Parameters.AddWithValue("@searchValue", searchValue);
                    using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dbReader.Read())
                        {
                            data.Add(new Category()
                            {
                                CategoryID = Convert.ToInt32(dbReader["CategoryID"]),
                                CategoryName = Convert.ToString(dbReader["CategoryName"]),
                                Description = Convert.ToString(dbReader["Description"])
                                //TODO:làm nốt còn lại các trường
                            });
                        }
                    }
                }
            }
            return data;
        }

        public bool Update(Category data)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Categories SET
                                                          CategoryName=@CategoryName,
	                                                      Description=@Description                                        
                                                    WHERE CategoryID=@CategoryID SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@CategoryID", data.CategoryID);
                cmd.Parameters.AddWithValue("@CategoryName", data.CategoryName);
                cmd.Parameters.AddWithValue("@Description", data.Description);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());
                connection.Close();
            }
            return rowsAffected > 0;
        }
    }
}
