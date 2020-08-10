using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class ProductDAL : IProductDAL
    {
        private string connectionString;
        public ProductDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public int Add(Product data, HttpPostedFileBase file)
        {
            //lưu ảnh vào thư mục
            if (data.PhotoPath != "")
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(HostingEnvironment.MapPath("~/Images"), fileName);
                file.SaveAs(path);
                var position = path.IndexOf("Images");
                var img = path.Substring(position);
                path = "\\" + img;
                data.PhotoPath = path;
            }
            int ProductID = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO Products
                                          (
                                                ProductName,
                                                SupplierID,
                                                CategoryID,
                                                QuantityPerUnit,
                                                UnitPrice,
                                                Descriptions,
                                                PhotoPath

                                          )
                                          VALUES
                                          (
                                                @ProductName,
                                                @SupplierID,
                                                @CategoryID,
                                                @QuantityPerUnit,
                                                @UnitPrice,
                                                @Descriptions,
                                                @PhotoPath
                                          );
                                          SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@ProductName", data.ProductName);
                cmd.Parameters.AddWithValue("@SupplierID", data.SupplierID);
                cmd.Parameters.AddWithValue("@CategoryID", data.CategoryID);
                cmd.Parameters.AddWithValue("@QuantityPerUnit", data.QuantityPerUnit);
                cmd.Parameters.AddWithValue("@UnitPrice", data.UnitPrice);
                cmd.Parameters.AddWithValue("@Descriptions", data.Descriptions);
                cmd.Parameters.AddWithValue("@PhotoPath", data.PhotoPath);

                ProductID = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }
            return ProductID;
        }
        public int Count(string searchValue, int categoryID, int supplierID)
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
                    cmd.CommandText = @"select count(*) as Count from Products where ((@searchValue=N'') or (ProductName like @searchValue ))and (CategoryID = @CategoryID or @CategoryID=0 ) and (SupplierID = @SupplierID or @SupplierID=0)";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@searchValue", searchValue);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("@SupplierID", supplierID);
                    Count = Convert.ToInt32(cmd.ExecuteScalar());
                }
                connection.Close();
            }

            return Count;
        }
        public bool Delete(int[] productIDs)
        {
            //xóa file đã tồn tại
            foreach (int productid in productIDs)
            {
                var productExist = Get(productid);
                if (productExist.PhotoPath != "")
                {
                    try
                    {
                        var fullPath = HostingEnvironment.MapPath(productExist.PhotoPath);
                        File.Delete(fullPath);
                    }
                    catch (Exception e) { }
                }
            }
            bool result = true;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //delete orderdetail
                connection.Open();

                SqlCommand cmdor = new SqlCommand();
                cmdor.CommandText = @"DELETE FROM OrderDetails
                                            WHERE ProductID = @ProductID";
                cmdor.CommandType = CommandType.Text;
                cmdor.Connection = connection;
                cmdor.Parameters.Add("@ProductID", SqlDbType.Int);
                foreach (int productid in productIDs)
                {
                    cmdor.Parameters["@ProductID"].Value = productid;
                    cmdor.ExecuteNonQuery();
                }

                connection.Close();
                //delete ProductAttributes
                connection.Open();

                SqlCommand cmdpr = new SqlCommand();
                cmdpr.CommandText = @"DELETE FROM ProductAttributes
                                            WHERE ProductID = @ProductID";
                cmdpr.CommandType = CommandType.Text;
                cmdpr.Connection = connection;
                cmdpr.Parameters.Add("@ProductID", SqlDbType.Int);
                foreach (int productid in productIDs)
                {
                    cmdpr.Parameters["@ProductID"].Value = productid;
                    cmdpr.ExecuteNonQuery();
                }

                connection.Close();
                //delete products
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM Products
                                            WHERE(ProductID = @ProductID) and ProductID not in(select ProductID from OrderDetails) and ProductID not in(select ProductID from ProductAttributes)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@ProductID", SqlDbType.Int);
                foreach (int productid in productIDs)
                {
                    cmd.Parameters["@ProductID"].Value = productid;
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
            return result;
        }

        public Product Get(int productID)
        {
            Product data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Products WHERE ProductID = @ProductID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@productID", productID);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new Product()
                        {
                            ProductID = Convert.ToInt32(dbReader["ProductID"]),
                            ProductName = Convert.ToString(dbReader["ProductName"]),
                            CategoryID = Convert.ToInt32(dbReader["CategoryID"]),
                            SupplierID = Convert.ToInt32(dbReader["SupplierID"]),
                            Descriptions = Convert.ToString(dbReader["Descriptions"]),
                            QuantityPerUnit = Convert.ToString(dbReader["QuantityPerUnit"]),
                            UnitPrice = Convert.ToInt32(dbReader["UnitPrice"]),
                            PhotoPath = Convert.ToString(dbReader["PhotoPath"])
                        };
                    }
                }

                connection.Close();
            }
            return data;
        }

        public List<Product> List(int page, int pageSize, string searchValue, int categoryID, int supplierID )
        {
            List<Product> data = new List<Product>();
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
		                                        ROW_NUMBER() over(order by ProductID) as RowNumber
                                        from	Products where ((@searchValue=N'' or ProductName like @searchValue) and (CategoryID = @CategoryID or @CategoryID=0 ) and (SupplierID = @SupplierID or @SupplierID=0))
                                        )as t
                                        where t.RowNumber between (@page-1)*@pageSize + 1 and @page*@pageSize";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@page", page);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    cmd.Parameters.AddWithValue("@searchValue", searchValue);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("@SupplierID", supplierID);
                    using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dbReader.Read())
                        {
                            data.Add(new Product()
                            {
                                ProductID = Convert.ToInt32(dbReader["ProductID"]),
                                ProductName = Convert.ToString(dbReader["ProductName"]),
                                SupplierID = Convert.ToInt32(dbReader["SupplierID"]),
                                CategoryID = Convert.ToInt32(dbReader["CategoryID"]),
                                QuantityPerUnit = Convert.ToString(dbReader["QuantityPerUnit"]),
                                UnitPrice = Convert.ToInt64(dbReader["UnitPrice"]),
                                Descriptions = Convert.ToString(dbReader["Descriptions"]),
                                PhotoPath = Convert.ToString(dbReader["PhotoPath"])
                                //TODO:làm nốt còn lại các trường
                            });
                        }
                    }
                }

            }

            return data;
        }

        public bool Update(Product data, HttpPostedFileBase file)
        {
            var dataProductExist = Get(data.ProductID);
            if (file != null)
            {
                if (dataProductExist.PhotoPath == "")
                {
                    //add file mới nếu chưa có file ảnh
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Images"), fileName);
                    file.SaveAs(path);
                    var position = path.IndexOf("Images");
                    var img = path.Substring(position);
                    path = "\\" + img;
                    data.PhotoPath = path;
                }
                else
                {
                    // xóa file đã tồn tại
                    try
                    {
                        var fullPath = HostingEnvironment.MapPath(dataProductExist.PhotoPath);
                        File.Delete(fullPath);
                    } catch (Exception e) { }
                    //add file mới vào
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(HostingEnvironment.MapPath("~/Images"), fileName);
                    file.SaveAs(path);
                    var position = path.IndexOf("Images");
                    var img = path.Substring(position);
                    path = "\\" + img;
                    data.PhotoPath = path;
                }
            }
            else
                data.PhotoPath = dataProductExist.PhotoPath;
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Products SET
                                                            ProductName=@ProductName,
                                                            SupplierID=@SupplierID,
                                                            CategoryID=@CategoryID,
                                                            QuantityPerUnit=@QuantityPerUnit,
                                                            UnitPrice=@UnitPrice,
                                                            Descriptions=@Descriptions,
                                                            PhotoPath=@PhotoPath
	                                                                                                              
                                                    WHERE ProductID=@ProductID
                                                    select @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@ProductID", data.ProductID);
                cmd.Parameters.AddWithValue("@ProductName", data.ProductName);
                cmd.Parameters.AddWithValue("@SupplierID", data.SupplierID);
                cmd.Parameters.AddWithValue("@CategoryID", data.CategoryID);
                cmd.Parameters.AddWithValue("@QuantityPerUnit", data.QuantityPerUnit);
                cmd.Parameters.AddWithValue("@UnitPrice", data.UnitPrice);
                cmd.Parameters.AddWithValue("@Descriptions", data.Descriptions);
                cmd.Parameters.AddWithValue("@PhotoPath", data.PhotoPath);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());
                connection.Close();
            }
            return rowsAffected > 0;
        }
    }
}
