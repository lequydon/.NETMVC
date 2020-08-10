using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DataLayers
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISupplierDAL
    {
        /// <summary>
        /// bổ sung 1supplier
        /// </summary>
        /// <param name="data"></param>
        /// <returns>id của supplier được bổ sung (nhở hơn hoặc bằng 0 nếu lỗi)</returns>
        int Add(Supplier data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(Supplier data);
        bool Delete(int[] supplierIDs);
        Supplier Get(int supplierID);
        List<Supplier> List(int page,int pageSize , string searchValue,string country);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        int Count(string searchValue,string country);
    }
}
