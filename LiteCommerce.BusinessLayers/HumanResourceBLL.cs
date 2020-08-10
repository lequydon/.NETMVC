using LiteCommerce.DataLayers;
using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LiteCommerce.BusinessLayers
{
    public class HumanResourceBLL
    {
        private static IEmployeeDAL EmployeeDB { get; set; }
        public static void Initialize(string connectionString)
        {
            EmployeeDB = new DataLayers.SqlServer.EmployeeDAL(connectionString);
        }
        public static List<Employee> Employee_List(int page, int pageSize, string searchValue,string country)
        {
            if (page <= 0)
                page = 1;
            if (pageSize <= 0)
                pageSize = 30;
            return EmployeeDB.List(page, pageSize, searchValue,country);
        }
        public static int Employee_Count(string searchValue,string country)
        {
            return EmployeeDB.Count(searchValue, country);
        }
        public static int Employee_Add(Employee data, HttpPostedFileBase file)
        {
            return EmployeeDB.Add(data, file);
        }
        public static bool Employee_Update(Employee data, HttpPostedFileBase file)
        {
            return EmployeeDB.Update(data, file);
        }
        public static Employee Employee_Get(int employeeID)
        {
            return EmployeeDB.Get(employeeID);
        }
        public static bool Employee_Delete(int[] employeeIDs)
        {
            return EmployeeDB.Delete(employeeIDs);
        }
        public static bool Employee_CheckMail(string email, string type)
        {
            return EmployeeDB.CheckEmail(email, type);
        }
    }
}
