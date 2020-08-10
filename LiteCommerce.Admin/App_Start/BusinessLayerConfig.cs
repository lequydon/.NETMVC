using LiteCommerce.BusinessLayers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LiteCommerce.Admin
{
    /// <summary>
    /// khỏi tạo các chức năng tác nghiệp ứng dụng
    /// </summary>
    public static class BusinessLayerConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Initialize()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LiteCommence"].ConnectionString;
            CatalogBLL.Initialize(connectionString);
            HumanResourceBLL.Initialize(connectionString);
            AccountBLL.Initialize(connectionString);
            UserAccountBLL.Initialize(connectionString);
            SaleManagementBLL.Initialize(connectionString);
            DashboardBLL.Initialize(connectionString);
            //WebSecurity.InitializeDatabaseConnection(connectionString, "TableName", "ColumnId", "ColumnName", autoCreateTables: false);
            //TODO:bổ sùng khởi tạo các BLL khác khi cần sử dụng
        }
    }
}