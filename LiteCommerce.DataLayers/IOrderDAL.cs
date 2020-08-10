using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DataLayers
{
    public interface IOrderDAL
    {
        int Add(Order data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(Order data);
        bool Delete(int[] orderIDs);
        Order Get(int orderID);
        int Count(string searchValue, int employeeID, string customerID, int shipperID);
        List<Order> List(int page, int pageSize, string searchValue, int employeeID, string customerID, int shipperID);
    }
}
