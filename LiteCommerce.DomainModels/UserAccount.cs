using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DomainModels
{
    public class UserAccount
    {
        /// <summary>
        /// ID của user
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// tên đầy đủ
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// đường dẫn
        /// </summary>
        public string Photo { get; set; }
        public string GroupName { get; set; }
    }
}
