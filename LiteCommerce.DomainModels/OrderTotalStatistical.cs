using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DomainModels
{
    public class OrderTotalStatistical
    {
        public int CountCustomer { get; set; }
        public decimal UnitPrice { get; set; }
        public int CountOrder { get; set; }
    }
}
