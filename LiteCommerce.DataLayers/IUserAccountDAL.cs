using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DataLayers
{
    public interface IUserAccountDAL
    {
        /// <summary>
        /// kiểm tra thông tin đăng nhập của user có hợp lệ ko
        /// -nếu ko hợp lệ hàm trả về thông tin user
        /// ngược lại trả về null
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAccount Authorize(string userName, string password);
        bool Update(Account account);
        bool Get(Account account);
        UserProfile GetEmployee(Account account);
        string GetCode(string email);
        bool SetCode(string email, string code);
    }
}
