using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DataLayers
{
    public interface IAccountDAL
    {
        int Add(Account data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(Account data);
        bool Delete(int[] accountIDs);
        Account Get(int accountID);
        Employee GetEmployee(Account account);
        List<Account> List(int page, int pageSize, string searchValue);
        bool Get(Account account);
    }
}
