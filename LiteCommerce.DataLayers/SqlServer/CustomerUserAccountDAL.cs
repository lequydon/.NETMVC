using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCommerce.DataLayers.SqlServer
{
    public class CustomerUserAccountDAL:IUserAccountDAL
    {
        private string connectionString;
        public CustomerUserAccountDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public UserAccount Authorize(string userName, string password)
        {
            throw new NotImplementedException();
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
