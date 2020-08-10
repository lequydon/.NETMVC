using LiteCommerce.DataLayers;
using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LiteCommerce.BusinessLayers
{
    public class CatalogBLL
    {
        /// <summary>
        /// 
        /// </summary>
        private static ISupplierDAL SupplierDB { get;set; }
        private static ICountryDAL CountryDB { get; set; }
        private static ICustomerDAL CustomerDB { get; set; }
        private static IShipperDAL ShipperDB { get; set; }
        private static IProductDAL ProductDB { get; set; }
        private static ICategoryDAL CategoryDB { get; set; }
        /// <summary>
        /// hàm này fai được gọi để khởi tạo các chức năng tác nghiệp
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            CountryDB = new DataLayers.SqlServer.CountryDAL(connectionString);
            SupplierDB = new DataLayers.SqlServer.SupplierDAL(connectionString);
            CustomerDB = new DataLayers.SqlServer.CustomerDAL(connectionString);
            ShipperDB = new DataLayers.SqlServer.ShipperDAL(connectionString);
            ProductDB = new DataLayers.SqlServer.ProductDAL(connectionString);
            CategoryDB = new DataLayers.SqlServer.CategoryDAL(connectionString);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page">page cần hiển thị</param>
        /// <param name="pageSize"kích thước trang</param>
        /// <param name="searchValue">tên cần tìm kiếm</param>
        /// <returns></returns>
        /// 
        public static List<Supplier> Suppliers_List(int page, int pageSize, string searchValue,string country)
        {
            if (page <= 0)
                page = 1;
            if (pageSize <= 0)
                pageSize = 30;
            return SupplierDB.List(page, pageSize, searchValue,country);
        }
        public static List<Country> Country_List()
        {
            return CountryDB.GetListContry();
        }
        public static int Supplier_Count(string searchValue,string country)
        {
            return SupplierDB.Count(searchValue,country);
        }
        public static int Supplier_Add(Supplier data)
        {
            return SupplierDB.Add(data);
        }
        public static bool Supplier_Update(Supplier data)
        {
            return SupplierDB.Update(data);
        }
        public static Supplier Supplier_Get(int supplierID)
        {
            return SupplierDB.Get(supplierID);
        }
        public static bool Supplier_Delete(int[] supplierIDs)
        {
            return SupplierDB.Delete(supplierIDs);
        }
        public static List<Customer> Customers_List(int page, int pageSize, string searchValue,string country)
        {
            if (page <= 0)
                page = 1;
            if (pageSize <= 0)
                pageSize = 30;
            return CustomerDB.List(page, pageSize, searchValue,country);
        }
        public static int Customers_Count(string searchValue,string country)
        {
            return CustomerDB.Count(searchValue,country);
        }
        public static int Customers_Add(Customer data)
        {
            return CustomerDB.Add(data);
        }
        public static bool Customers_Update(Customer data)
        {
            return CustomerDB.Update(data);
        }
        public static Customer Customers_Get(string customerID)
        {
            return CustomerDB.Get(customerID);
        }
        public static bool Customers_Delete(string[] customerIDs)
        {
            return CustomerDB.Delete(customerIDs);
        }
        public static List<Shipper> Shippers_List(string searchValue)
        {
            return ShipperDB.List(searchValue);
        }
        public static int Shippers_Count(string searchValue)
        {
            return ShipperDB.Count(searchValue);
        }
        public static int Shippers_Add(Shipper data)
        {
            return ShipperDB.Add(data);
        }
        public static bool Shippers_Update(Shipper data)
        {
            return ShipperDB.Update(data);
        }
        public static Shipper Shippers_Get(int shipperID)
        {
            return ShipperDB.Get(shipperID);
        }
        public static bool Shippers_Delete(int[] shipperIDs)
        {
            return ShipperDB.Delete(shipperIDs);
        }
        public static List<Product> Products_List(int page, int pageSize, string searchValue, int categoryID, int supplierID)
        {
            if (page <= 0)
                page = 1;
            if (pageSize <= 0)
                pageSize = 30;
            return ProductDB.List(page, pageSize, searchValue, categoryID, supplierID);
        }
        public static int Products_Count(string searchValue,int categoryID, int supplierID)
        {
            return ProductDB.Count(searchValue,categoryID, supplierID);
        }
        public static Product Products_Get(int productID)
        {
            return ProductDB.Get(productID);
        }
        public static bool Product_Delete(int[] productIDs)
        {
            return ProductDB.Delete(productIDs);
        }
        public static int Products_Add(Product data, HttpPostedFileBase file)
        {
            return ProductDB.Add(data,file);
        }
        public static bool Products_Update(Product data, HttpPostedFileBase file)
        {
            return ProductDB.Update(data, file);
        }
        public static List<Category> Categories_List(string searchValue)
        {
            return CategoryDB.List(searchValue);
        }
        public static int Categories_Count(string searchValue)
        {
            return CategoryDB.Count(searchValue);
        }
        public static int Categories_Add(Category data)
        {
            return CategoryDB.Add(data);
        }
        public static bool Categories_Update(Category data)
        {
            return CategoryDB.Update(data);
        }
        public static Category Categories_Get(int categoryID)
        {
            return CategoryDB.Get(categoryID);
        }
        public static bool Categories_Delete(int[] categoryIDs)
        {
            return CategoryDB.Delete(categoryIDs);
        }
        //public static List<Product> Product_List(int page, int pageSize, string searchValue)
        //{
        //    return ProductDB.List(page, pageSize, searchValue,pageSize,se);
        //}
    }
}
