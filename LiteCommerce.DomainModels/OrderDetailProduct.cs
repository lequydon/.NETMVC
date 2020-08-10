using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DomainModels
{
    public class OrderDetailProduct
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public long UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public string ProductName { get; set; }
    }
}
