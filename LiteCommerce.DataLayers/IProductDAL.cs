using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LiteCommerce.DataLayers
{
    public interface IProductDAL
    {
        int Add(Product data, HttpPostedFileBase file);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(Product data, HttpPostedFileBase file);
        bool Delete(int[] productIDs);
        Product Get(int productID);
        int Count(string searchValue, int categoryID, int supplierID);
        List<Product> List(int page, int pageSize, string searchValue, int categoryID, int supplierID);
    }
}
