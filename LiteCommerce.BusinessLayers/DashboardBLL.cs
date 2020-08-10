using LiteCommerce.DataLayers;
using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.BusinessLayers
{
    public class DashboardBLL
    {
        //private static string connectionstring;
        private static IDashBoardDAL DashBoardDB { get; set; }
        public static void Initialize(string connectionString)
        {
            DashBoardDB = new DataLayers.SqlServer.DashBoardDAL(connectionString);
        }
        public static List<DashBoard> DashBoard_Get(DateTime timeStart,DateTime timeEnd,string country)
        {
            return DashBoardDB.Get(timeStart,timeEnd,country);
        }
        public static List<OrderStatistical> DashBoard_GetStatisticalOrder(DateTime timeStart, DateTime timeEnd, string country)
        {
            return DashBoardDB.GetStatisticalOrder(timeStart, timeEnd, country);
        }
        public static OrderTotalStatistical DashBoard_GetTotalStatisticalOrder(DateTime timeStart, DateTime timeEnd, string country)
        {
            return DashBoardDB.GetTotalStatisticalOrder(timeStart, timeEnd, country);
        }
        public static CountryUnitPrice DashBoard_GetCountryUnitPrice(DateTime timeStart, DateTime timeEnd, string country)
        {
            return DashBoardDB.GetCountryUnitPrice(timeStart, timeEnd, country);
        }
    }
}
