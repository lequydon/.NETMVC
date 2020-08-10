using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LiteCommerce.DataLayers
{
    public interface IOrderDetailDAL
    {
        int Add(OrderDetail data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(OrderDetail data, int oldProductID);
        bool Delete(int orderID, int[] productIDs);
        OrderDetail Get(int orderID, int productID);
        List<OrderDetailProduct> List(string searchValue,int orderID);
        int Count(string searchValue, int orderID);
    }
}
