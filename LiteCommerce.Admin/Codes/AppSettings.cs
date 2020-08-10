using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LiteCommerce.Admin
{
    public static class AppSettings
    {
        public static int DefaultPagesize
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["DefaultPagesize"]);
            }
        }
    }
}