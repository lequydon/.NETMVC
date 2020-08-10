using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DataLayers
{
    public interface ICustomerDAL
    {
        int Count(string searchValue,string country);
        int Add(Customer data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(Customer data);
        bool Delete(string[] customerIDs);
        Customer Get(string customerID);
        List<Customer> List(int page, int pageSize, string searchValue,string country);
    }
}
