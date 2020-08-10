using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DataLayers
{
    public interface ICategoryDAL
    {
        int Add(Category data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(Category data);
        bool Delete(int[] categoryIDs);
        Category Get(int categoryID);
        int Count(string searchValue);
        List<Category> List(string searchValue);
    }
}
