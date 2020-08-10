using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class UserAccountDAL : IUserAccountDAL
    {
        private string connectionString;
        /// <summary>
        /// authorize nhân viên
        /// </summary>
        /// <param name="userName">địa chỉ email của nhân viên</param>
        /// <param name="password">mật khẩu (đã md5)</param>
        /// <returns></returns>
        public UserAccount Authorize(string userName, string password)
        {
            //TODO:kiểm tra thông tin đăng nhập từ DB và trả về đúng thông tin Employee
            return new UserAccount()
            {
                UserID = userName,
                FullName = "Lê Quý Đôn",
                Photo = "don.png"
            };
        }

        public bool Get(Account account)
        {
            throw new NotImplementedException();
        }

        public string GetCode(string email)
        {
            throw new NotImplementedException();
        }

        public UserProfile GetEmployee(Account account)
        {
            throw new NotImplementedException();
        }

        public bool SendMailChangePassword(string callbackUrl, string email)
        {
            throw new NotImplementedException();
        }

        public bool SetCode(string email, string code)
        {
            throw new NotImplementedException();
        }

        public bool Update(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
