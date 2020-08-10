using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DomainModels
{
    public class OrderStatistical
    {
        public int CountCustomer { get; set; }
        public DateTime ShippedDate { get; set; }
        public decimal SumUnitPrice { get; set; }
        public int CountOrder { get; set; }
    }
}
