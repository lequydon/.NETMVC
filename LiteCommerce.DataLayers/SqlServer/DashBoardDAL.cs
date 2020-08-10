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
    public class DashBoardDAL : IDashBoardDAL
    {
        private string connectionString;
        public DashBoardDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public List<DashBoard> Get(DateTime timeStart,DateTime timeEnd,string country)
        {
            List<DashBoard> data = new List<DashBoard>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = @"select Orders.OrderID,Orders.CustomerID,Orders.ShippedDate,Orders.ShipCountry,OrderDetails.UnitPrice from Orders join OrderDetails on Orders.OrderID=OrderDetails.OrderID
                                        where (Orders.ShippedDate is not NULL) and (Orders.ShippedDate between @timeStart and @timeEnd) and (Orders.ShipCountry=@country or @country='')
                                        order by Orders.ShippedDate";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@timeStart", timeStart);
                    cmd.Parameters.AddWithValue("@timeEnd", timeEnd);
                    cmd.Parameters.AddWithValue("@country", country);
                    using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dbReader.Read())
                        {
                            data.Add(new DashBoard()
                            {
                                OrderID = Convert.ToInt32(dbReader["OrderID"]),
                                CustomerID = Convert.ToString(dbReader["CustomerID"]),
                                ShippedDate = Convert.ToDateTime(dbReader["ShippedDate"]),
                                ShipCountry = Convert.ToString(dbReader["ShipCountry"]),
                                UnitPrice = Convert.ToDecimal(dbReader["UnitPrice"])
                                //TODO:làm nốt còn lại các trường
                            });
                        }
                    }
                }
            }
            return data;
        }
        public List<OrderStatistical> GetStatisticalOrder(DateTime timeStart, DateTime timeEnd, string country)
        {
            List<OrderStatistical> data = new List<OrderStatistical>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = @"select count(Orders.CustomerID)as CountCustomer,Orders.ShippedDate,sum(OrderDetails.UnitPrice)as SumUnitPrice,count(OrderDetails.OrderID) as CountOrder
										from Orders join OrderDetails on Orders.OrderID=OrderDetails.OrderID
                                        where (Orders.ShippedDate is not NULL) and (Orders.ShippedDate between @timeStart and @timeEnd )and(Orders.ShipCountry=@country or @country='' )
										group by Orders.ShippedDate order by Orders.ShippedDate";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@timeStart", timeStart);
                    cmd.Parameters.AddWithValue("@timeEnd", timeEnd);
                    cmd.Parameters.AddWithValue("@country", country);
                    using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dbReader.Read())
                        {
                            if (dbReader.IsDBNull(dbReader.GetOrdinal("CountCustomer")))
                            {
                                data = null;
                            }
                            else
                            data.Add(new OrderStatistical()
                            {
                                CountCustomer = Convert.ToInt32(dbReader["CountCustomer"]),
                                SumUnitPrice = Convert.ToDecimal(dbReader["SumUnitPrice"]),
                                ShippedDate = Convert.ToDateTime(dbReader["ShippedDate"]),
                                CountOrder = Convert.ToInt32(dbReader["CountOrder"])
                            });
                        }
                    }
                }
            }
            //if (data.Count == 0)
            //{
            //    data.Add(new OrderStatistical()
            //    {
            //        CountCustomer = 0,
            //        SumUnitPrice = 0,
            //        CountOrder = 0,
            //        ShippedDate = new DateTime(0, 0, 0000),
            //    });
            //}
            return data;
        }
        public OrderTotalStatistical GetTotalStatisticalOrder(DateTime timeStart, DateTime timeEnd, string country)
        {
            OrderTotalStatistical data = new OrderTotalStatistical();
            data.CountCustomer = 0;
            data.CountOrder = 0;
            data.UnitPrice = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = @"select (
                                        select top 1 * from(select ROW_NUMBER() over(order by Orders.CustomerID) as CountCustomer
										from Orders join OrderDetails on Orders.OrderID=OrderDetails.OrderID
                                        where (Orders.ShippedDate is not NULL) and (Orders.ShippedDate between @timeStart and @timeEnd )and(Orders.ShipCountry=@country or @country='' )
										group by Orders.CustomerID)as nb order by CountCustomer DESC
                                        ) as CountCustomer,sum(OrderDetails.UnitPrice)as UnitPrice,count(OrderDetails.OrderID) as CountOrder
										from Orders join OrderDetails on Orders.OrderID=OrderDetails.OrderID
                                        where (Orders.ShippedDate is not NULL) and (Orders.ShippedDate between @timeStart and @timeEnd )and(Orders.ShipCountry=@country or @country='' )";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@timeStart", timeStart);
                    cmd.Parameters.AddWithValue("@timeEnd", timeEnd);
                    cmd.Parameters.AddWithValue("@country", country);
                    using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dbReader.Read())
                        {
                            if (!dbReader.IsDBNull(dbReader.GetOrdinal("CountCustomer")))
                                data = (new OrderTotalStatistical()
                                {
                                    CountCustomer = Convert.ToInt32(dbReader["CountCustomer"]),
                                    UnitPrice = Convert.ToDecimal(dbReader["UnitPrice"]),
                                    CountOrder = Convert.ToInt32(dbReader["CountOrder"])
                                });
                        }
                    }
                }
            }
            return data;
        }
        public CountryUnitPrice GetCountryUnitPrice(DateTime timeStart, DateTime timeEnd, string country)
        {
            CountryUnitPrice data = new CountryUnitPrice();
            data.CountryUnitprice = 0;
            data.AllCountryUnitPrice = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = @"select (select sum(OrderDetails.UnitPrice)  from Orders join OrderDetails on Orders.OrderID=OrderDetails.OrderID
                                        where (Orders.ShippedDate is not NULL) and (Orders.ShippedDate between @timeStart and @timeEnd )and(Orders.ShipCountry=@country or @country='' ) ) as CountryUnitPrice,sum(OrderDetails.UnitPrice) as AllCountryUnitPrice
										from Orders join OrderDetails on Orders.OrderID=OrderDetails.OrderID
                                        where (Orders.ShippedDate is not NULL) and (Orders.ShippedDate between @timeStart and @timeEnd)";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@timeStart", timeStart);
                    cmd.Parameters.AddWithValue("@timeEnd", timeEnd);
                    cmd.Parameters.AddWithValue("@country", country);
                    using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dbReader.Read())
                        {
                            if (!dbReader.IsDBNull(dbReader.GetOrdinal("CountryUnitPrice")))
                                data.CountryUnitprice = Convert.ToDecimal(dbReader["CountryUnitPrice"]);
                            if (!dbReader.IsDBNull(dbReader.GetOrdinal("AllCountryUnitPrice")))
                                data.AllCountryUnitPrice = Convert.ToDecimal(dbReader["AllCountryUnitPrice"]);
                            //data = (new CountryUnitPrice()
                            //{
                            //    CountryUnitprice = Convert.ToDecimal(dbReader["CountryUnitPrice"]),
                            //    AllCountryUnitPrice = Convert.ToDecimal(dbReader["AllCountryUnitPrice"])
                            //});
                        }
                    }
                }
            }
            return data;
        }

    }
}
