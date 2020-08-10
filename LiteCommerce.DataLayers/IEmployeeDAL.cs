using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LiteCommerce.DataLayers
{
    public interface IEmployeeDAL
    {
        int Add(Employee data, HttpPostedFileBase file);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(Employee data, HttpPostedFileBase file);
        bool Delete(int[] employeeIDs);
        Employee Get(int employeeID);
        List<Employee> List(int page, int pageSize, string searchValue,string country);
        int Count(string searchValue,string country);
        bool CheckEmail(string email,string type);
    }
}
