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
    public class OrderDAL : IOrderDAL
    {
        private string connectionString;
        public OrderDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public int Add(Order data)
        {
            int orderId = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO Orders
                                          (
                                                CustomerID,
                                                EmployeeID,
                                                OrderDate,
                                                RequiredDate,
                                                ShippedDate,
                                                ShipperID,
                                                Freight,
                                                ShipAddress,
                                                ShipCity,
                                                ShipCountry
                                          )
                                          VALUES
                                          (
	                                            @CustomerID,
                                                @EmployeeID,
                                                @OrderDate,
                                                @RequiredDate,
                                                @ShippedDate,
                                                @ShipperID,
                                                @Freight,
                                                @ShipAddress,
                                                @ShipCity,
                                                @ShipCountry
                                          );
                                          SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@CustomerID", data.CustomerID);
                cmd.Parameters.AddWithValue("@EmployeeID", data.EmployeeID);
                cmd.Parameters.AddWithValue("@OrderDate", data.OrderDate);
                cmd.Parameters.AddWithValue("@RequiredDate", data.RequiredDate);
                cmd.Parameters.AddWithValue("@ShippedDate", data.ShippedDate);
                cmd.Parameters.AddWithValue("@Freight", data.Freight);
                cmd.Parameters.AddWithValue("@ShipAddress", data.ShipAddress);
                cmd.Parameters.AddWithValue("@ShipCity", data.ShipCity);
                cmd.Parameters.AddWithValue("@ShipCountry", data.ShipCountry);
                cmd.Parameters.AddWithValue("@ShipperID", data.ShipperID);
                orderId = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
            }

            return orderId;
        }

        public bool Delete(int[] orderIDs)
        {
            bool result = true;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //delete Orderdetail
                connection.Open();

                SqlCommand cmdod = new SqlCommand();
                cmdod.CommandText = @"DELETE FROM OrderDetails
                                            WHERE(OrderID = @OrderID)";
                cmdod.CommandType = CommandType.Text;
                cmdod.Connection = connection;
                cmdod.Parameters.Add("@OrderID", SqlDbType.Int);
                foreach (int orderID in orderIDs)
                {
                    cmdod.Parameters["@OrderID"].Value = orderID;
                    cmdod.ExecuteNonQuery();
                }

                connection.Close();
                //delete Order
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE FROM Orders
                                            WHERE(OrderID = @OrderID)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@OrderID", SqlDbType.Int);
                foreach (int orderID in orderIDs)
                {
                    cmd.Parameters["@OrderID"].Value = orderID;
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
            return result;
        }

        public Order Get(int orderID)
        {
            Order data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT * FROM Orders WHERE OrderID = @OrderID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@OrderID", orderID);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dbReader.Read())
                    {
                        data = new Order()
                        {
                            CustomerID = Convert.ToString(dbReader["CustomerID"]),
                            EmployeeID = Convert.ToInt32(dbReader["EmployeeID"]),
                            ShipperID = Convert.ToInt32(dbReader["ShipperID"]),
                            ShippedDate = Convert.ToDateTime(dbReader["ShippedDate"]),
                            ShipCountry = Convert.ToString(dbReader["ShipCountry"]),
                            ShipCity = Convert.ToString(dbReader["ShipCity"]),
                            Freight = Convert.ToInt64(dbReader["Freight"]),
                            OrderDate = Convert.ToDateTime(dbReader["OrderDate"]),
                            OrderID = Convert.ToInt32(dbReader["OrderID"]),
                            RequiredDate = Convert.ToDateTime(dbReader["RequiredDate"]),
                            ShipAddress=Convert.ToString(dbReader["ShipAddress"])

                        };
                    }
                }

                connection.Close();
            }
            return data;
        }
        public int Count(string searchValue, int EmployeeID, string customerID, int shipperID)
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
                    cmd.CommandText = @"select count(*) as Count from Orders where ((@searchValue=N'') or (ShipAddress like @searchValue )) and (CustomerID = @CustomerID or @CustomerID='' ) and (EmployeeID = @EmployeeID or @EmployeeID=0) and (ShipperID = @ShipperID or @ShipperID=0)";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@searchValue", searchValue);
                    cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);
                    cmd.Parameters.AddWithValue("@ShipperID", shipperID);
                    Count = Convert.ToInt32(cmd.ExecuteScalar());
                }
                connection.Close();
            }

            return Count;
        }
        public List<Order> List(int page, int pageSize, string searchValue,int employeeID,string customerID,int shipperID)
        {
            List<Order> data = new List<Order>();
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
		                                        ROW_NUMBER() over(order by OrderID) as RowNumber
                                        from	Orders where ((@searchValue=N'' or ShipAddress like @searchValue or ShipCity like @searchValue or ShipCountry like @searchValue) and (CustomerID = @CustomerID or @CustomerID='' ) and (EmployeeID = @EmployeeID or @EmployeeID=0) and (ShipperID = @ShipperID or @ShipperID=0))
                                        )as t
                                        where t.RowNumber between (@page-1)*@pageSize + 1 and @page*@pageSize";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@page", page);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    cmd.Parameters.AddWithValue("@searchValue", searchValue);
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    cmd.Parameters.AddWithValue("@ShipperID", shipperID);
                    using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dbReader.Read())
                        {
                            data.Add(new Order()
                            {
                                OrderID = Convert.ToInt32(dbReader["OrderID"]),
                                CustomerID = Convert.ToString(dbReader["CustomerID"]),
                                EmployeeID = Convert.ToInt32(dbReader["EmployeeID"]),
                                ShipperID = Convert.ToInt32(dbReader["ShipperID"]),
                                OrderDate = Convert.ToDateTime(dbReader["OrderDate"]),
                                RequiredDate = Convert.ToDateTime(dbReader["RequiredDate"]),
                                ShippedDate = Convert.ToDateTime(dbReader["ShippedDate"]),
                                Freight = Convert.ToInt64(dbReader["Freight"]),
                                ShipAddress=Convert.ToString(dbReader["ShipAddress"]),
                                ShipCity = Convert.ToString(dbReader["ShipCity"]),
                                ShipCountry = Convert.ToString(dbReader["ShipCountry"])
                                //TODO:làm nốt còn lại các trường
                            });
                        }
                    }
                }

            }

            return data;
        }

        public bool Update(Order data)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"UPDATE Orders SET
                                                CustomerID=@CustomerID,
                                                EmployeeID=@EmployeeID,
                                                OrderDate=@OrderDate,
                                                RequiredDate=@RequiredDate,
                                                ShippedDate=@ShippedDate,
                                                ShipperID=@ShipperID,
                                                Freight=@Freight,
                                                ShipAddress=@ShipAddress,
                                                ShipCity=@ShipCity,
                                                ShipCountry=@ShipCountry                                                           
                                                    WHERE OrderID=@OrderID
                                                    SELECT @@IDENTITY;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@OrderID", data.OrderID);
                cmd.Parameters.AddWithValue("@CustomerID", data.CustomerID);
                cmd.Parameters.AddWithValue("@EmployeeID", data.EmployeeID);
                cmd.Parameters.AddWithValue("@OrderDate", data.OrderDate);
                cmd.Parameters.AddWithValue("@RequiredDate", data.RequiredDate);
                cmd.Parameters.AddWithValue("@ShippedDate", data.ShippedDate);
                cmd.Parameters.AddWithValue("@Freight", data.Freight);
                cmd.Parameters.AddWithValue("@ShipAddress", data.ShipAddress);
                cmd.Parameters.AddWithValue("@ShipCity", data.ShipCity);
                cmd.Parameters.AddWithValue("@ShipCountry", data.ShipCountry);
                cmd.Parameters.AddWithValue("@ShipperID", data.ShipperID);
                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());

                connection.Close();
            }

            return rowsAffected > 0;
        }
    }
}
