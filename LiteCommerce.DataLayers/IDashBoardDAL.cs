using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DataLayers
{
    public interface IDashBoardDAL
    {
        List<DashBoard> Get(DateTime timeStart,DateTime timeEnd,string country);
        List<OrderStatistical> GetStatisticalOrder(DateTime timeStart, DateTime timeEnd, string country);
        OrderTotalStatistical GetTotalStatisticalOrder(DateTime timeStart, DateTime timeEnd, string country);
        CountryUnitPrice GetCountryUnitPrice(DateTime timeStart, DateTime timeEnd, string country);
    }
}
