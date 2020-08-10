using LiteCommerce.DataLayers;
using LiteCommerce.DataLayers.SqlServer;
using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.BusinessLayers
{
    public class AccountBLL
    {
        private static string connectionstring;
        private static IAccountDAL AccountDB { get; set; }
        public static void Initialize(string connectionString)
        {
            AccountDB = new DataLayers.SqlServer.AccountBLL(connectionString);
        }
        public static List<Account> Account_List(int page, int pageSize, string searchValue)
        {
            if (page <= 0)
                page = 1;
            if (pageSize <= 0)
                pageSize = 30;
            return AccountDB.List(page, pageSize, searchValue);
        }
        public static bool Account_Get(Account account)
        {
            return AccountDB.Get(account);
        }
        public static Employee Account_GetEmployee(Account account)
        {
            return AccountDB.GetEmployee(account);
        }
        public static bool Account_Update(Account account, UserAccountTypes userType)
        {
            IUserAccountDAL userAccountDB;
            switch (userType)
            {
                case UserAccountTypes.Employee:
                    userAccountDB = new EmployeeUserAccountDAL(connectionstring);
                    break;
                case UserAccountTypes.Customer:
                    userAccountDB = new CustomerUserAccountDAL(connectionstring);
                    break;
                default:
                    throw new Exception("usertype is not valid");
            }
            //return userAccountDB.Authorize(userName, password);
            return AccountDB.Update(account);
        }
    }
}
