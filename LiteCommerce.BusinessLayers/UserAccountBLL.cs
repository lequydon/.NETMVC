using LiteCommerce.DataLayers;
using LiteCommerce.DataLayers.SqlServer;
using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LiteCommerce.BusinessLayers
{
    public static class UserAccountBLL
    {
        private static string connectionstring;
        /// <summary>
        /// 
        /// </summary>
        public static IUserAccountDAL UserAccountDB { get; set; }
        public static void Initialize(string connectionstring)
        {
            UserAccountBLL.connectionstring = connectionstring;
        }
        public static UserAccount Authorize(string userName,string password,UserAccountTypes userType)
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
            return userAccountDB.Authorize(userName, password);
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
            return userAccountDB.Update(account);
        }
        public static bool Account_Get(Account account, UserAccountTypes userType)
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
            return userAccountDB.Get(account);
        }
        public static bool Employee_Update(Employee data, HttpPostedFileBase file, UserAccountTypes userType)
        {
            switch (userType)
            {
                case UserAccountTypes.Employee:
                    return HumanResourceBLL.Employee_Update(data, file);
                default:
                    throw new Exception("usertype is not valid");
            }
        }
        public static string GetCode(string email,UserAccountTypes userType)
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
            return userAccountDB.GetCode(email);
        }
        public static bool SetCode(string email,string code, UserAccountTypes userType)
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
            return userAccountDB.SetCode(email,code);
        }
        public static UserProfile Account_GetEmployee(Account account, UserAccountTypes userType)
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
            return userAccountDB.GetEmployee(account);
        }
    }
}
