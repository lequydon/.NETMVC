using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DomainModels
{
    public class DashBoard
    {
        public int OrderID { get; set; }
        public string CustomerID { get; set; }
        public DateTime ShippedDate { get; set; }
        public string ShipCountry { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
