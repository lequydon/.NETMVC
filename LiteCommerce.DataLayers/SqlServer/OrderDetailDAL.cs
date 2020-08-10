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
    public class OrderDetailDAL : IOrderDetailDAL
    {
        private string connectionString;
        public OrderDetailDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public int Add(OrderDetail data)
        {
            int orderDetailID = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO OrderDetails
                                          (
                                                OrderID,
                                                ProductID,
                                                UnitPrice,
                                                Quantity,
                                                Discount
                                          )
                                          VALUES
                                          (
                                                @OrderID,
                                                @ProductID,
                                                @UnitPrice,
                                                @Quantity,
                                                @Discount
                                          );
                                          SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@OrderID", data.OrderID);
                cmd.Parameters.AddWithValue("@ProductID", data.ProductID);
                cmd.Parameters.AddWithValue("@UnitPrice", data.UnitPrice);
                cmd.Parameters.AddWithValue("@Quantity", data.Quantity);
                cmd.Parameters.AddWithValue("@Discount", data.Discount);

                try {
                    orderDetailID = Convert.ToInt32(cmd.ExecuteScalar());
                } catch (Exception e)
                {

                }

                connection.Close();
            }

            return orderDetailID;
        }
        public int Count(string searchValue,int orderID)
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
                    cmd.CommandText = @"select count(*) as Count from	OrderDetails join Products on OrderDetails.ProductID=Products.ProductID where OrderDetails.OrderID=@orderID and ((@searchValue=N'') or (Products.ProductID like @searchValue ))";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@searchValue", searchValue);
                    cmd.Parameters.AddWithValue("@orderID", orderID);
                    Count = Convert.ToInt32(cmd.ExecuteScalar());
                }
                connection.Close();
            }

            return Count;
        }

        public bool Delete(int orderID, int[] productIDs)
        {
            bool result = true;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM OrderDetails
                                            WHERE(OrderID = @OrderID)and(ProductID = @ProductID)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@OrderID", SqlDbType.Int);
                cmd.Parameters.Add("@ProductID", SqlDbType.Int);
                    foreach (int productID in productIDs)
                    {
                        cmd.Parameters["@OrderID"].Value = orderID;
                        cmd.Parameters["@ProductID"].Value = productID;
                        cmd.ExecuteNonQuery();
                    }
                connection.Close();
            }
            return result;
        }

        public OrderDetail Get(int orderID,int productID)
        {
            OrderDetail data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM OrderDetails WHERE OrderID = @orderID and ProductID=@productID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@orderID", orderID);
                cmd.Parameters.AddWithValue("@productID", productID);
                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new OrderDetail()
                        {
                            OrderID = Convert.ToInt32(dbReader["OrderID"]),
                            ProductID = Convert.ToInt32(dbReader["ProductID"]),
                            UnitPrice = Convert.ToInt64(dbReader["UnitPrice"]),
                            Quantity = Convert.ToInt32(dbReader["Quantity"]),
                            Discount = Convert.ToDecimal(dbReader["Discount"])
                        };
                    }
                }

                connection.Close();
            }
            return data;
        }
        public List<OrderDetailProduct> List(string searchValue, int orderID)
        {
            List<OrderDetailProduct> data = new List<OrderDetailProduct>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            //TODO:truy vấn dữ liệu từ cơ sở dữ liệu database
            /// mở 1 kết nối cơ sở dữ liệu, khai báo 1 đối tương command đối tượng này chứa 1 câu lệnh dùng thực thi yêu cấu truy vấn cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = @"select OrderDetails.ProductID,OrderDetails.OrderID,OrderDetails.Discount,OrderDetails.Quantity,OrderDetails.UnitPrice,Products.ProductName
                                        from OrderDetails join Products on OrderDetails.ProductID=Products.ProductID
                                        where ((@searchValue=N'') or (Products.ProductName like @searchValue))and OrderDetails.OrderID=@orderID
                                        ";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    if (searchValue == null)
                    {
                        cmd.Parameters.AddWithValue("@searchValue", "");
                        cmd.Parameters.AddWithValue("@orderID", orderID);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@searchValue", searchValue);
                        cmd.Parameters.AddWithValue("@orderID", orderID);
                    }
                    using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dbReader.Read())
                        {
                            data.Add(new OrderDetailProduct()
                            {
                                ProductID = Convert.ToInt32(dbReader["ProductID"]),
                                ProductName = Convert.ToString(dbReader["ProductName"]),
                                OrderID = Convert.ToInt32(dbReader["OrderID"]),
                                Discount = Convert.ToDecimal(dbReader["Discount"]),
                                Quantity = Convert.ToInt32(dbReader["Quantity"]),
                                UnitPrice = Convert.ToInt64(dbReader["UnitPrice"])
                                //TODO:làm nốt còn lại các trường
                            });
                        }
                    }
                }
            }
            return data;
        }

        public bool Update(OrderDetail data,int oldProductID)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE OrderDetails SET
                                                        ProductID=@ProductIDnew,
                                                        UnitPrice=@UnitPrice,
                                                        Quantity=@Quantity,
                                                        Discount=@Discount
                                                    WHERE OrderID=@orderID and ProductID=@productIDold";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@ProductIDnew", data.ProductID);
                cmd.Parameters.AddWithValue("@UnitPrice", data.UnitPrice);
                cmd.Parameters.AddWithValue("@Quantity", data.Quantity);
                cmd.Parameters.AddWithValue("@Discount", data.Discount);
                cmd.Parameters.AddWithValue("@orderID", data.OrderID);
                cmd.Parameters.AddWithValue("@productIDold", oldProductID);

                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());
                connection.Close();
            }
            return rowsAffected > 0;
        }

    }
}
