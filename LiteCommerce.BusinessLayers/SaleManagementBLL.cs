using LiteCommerce.DataLayers;
using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.BusinessLayers
{
    public class SaleManagementBLL
    {
        private static IOrderDAL orderDB { get; set; }
        private static IOrderDetailDAL orderdetailDB { get; set; }
        public static void Initialize(string connectionString)
        {
            orderDB = new DataLayers.SqlServer.OrderDAL(connectionString);
            orderdetailDB = new DataLayers.SqlServer.OrderDetailDAL(connectionString);
        }
        public static List<OrderDetailProduct> OrderDetail_List(string searchValue, int orderID)
        {
            return orderdetailDB.List(searchValue,orderID);
        }
        public static int OrderDetail_Count(string searchValue, int orderID)
        {
            return orderdetailDB.Count(searchValue, orderID);
        }
        public static int OrderDetail_Add(OrderDetail data)
        {
            return orderdetailDB.Add(data);
        }
        public static bool OrderDetail_Update(OrderDetail data, int oldProductID)
        {
            return orderdetailDB.Update(data,oldProductID);
        }
        public static OrderDetail OrderDetail_Get(int orderID, int productID)
        {
            return orderdetailDB.Get(orderID, productID);
        }
        public static bool OrderDetail_Delete(int orderID, int[] productIDs)
        {
            return orderdetailDB.Delete(orderID, productIDs);
        }
        public static List<Order> Order_List(int page, int pageSize, string searchValue, int employeeID, string customerID, int shipperID)
        {
            if (page <= 0)
                page = 1;
            if (pageSize <= 0)
                pageSize = 30;
            return orderDB.List(page, pageSize, searchValue,employeeID,customerID,shipperID);
        }

        public static int Order_Count(string searchValue, int employeeID, string customerID, int shipperID)
        {
            return orderDB.Count(searchValue, employeeID, customerID, shipperID);
        }
        public static int Order_Add(Order data)
        {
            return orderDB.Add(data);
        }
        public static bool Order_Update(Order data)
        {
            return orderDB.Update(data);
        }
        public static Order Order_Get(int orderID)
        {
            return orderDB.Get(orderID);
        }
        public static bool Order_Delete(int[] supplierIDs)
        {
            return orderDB.Delete(supplierIDs);
        }
    }
}
