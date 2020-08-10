using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiteCommerce.Admin.Models
{
    public class paginationResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public string searchValue { get; set; }
        public int SupplierID { get; set; }
        public int CategoryID { get; set; }
        public int ShipperID { get; set; }
        public int OrderID { get; set; }
        public string CustomerID { get; set; }
        public int EmployeeID { get; set; }
        public string SearchValue { get; set; }
        public string Country { get; set; }
        public int pageCount
        {
            get
            {
                int pageCount = 1;
                pageCount = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                    pageCount += 1;
                return pageCount;
            }
        }
    }
}