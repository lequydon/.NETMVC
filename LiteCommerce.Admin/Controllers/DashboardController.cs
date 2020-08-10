using LiteCommerce.BusinessLayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiteCommerce.Admin.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(DateTime timeStart,DateTime timeEnd,string country)
        {
            timeEnd = timeEnd.AddMonths(1).AddDays(-1);
            return Json(DashboardBLL.DashBoard_Get(timeStart, timeEnd, country), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListStatisticalOrder(DateTime timeStart, DateTime timeEnd, string country)
        {
            timeEnd=timeEnd.AddMonths(1).AddDays(-1);
            return Json(DashboardBLL.DashBoard_GetStatisticalOrder(timeStart, timeEnd, country), JsonRequestBehavior.AllowGet);
        }
        public ActionResult TotalStatisticalOrder(DateTime timeStart, DateTime timeEnd, string country)
        {
            timeEnd = timeEnd.AddMonths(1).AddDays(-1);
            return Json(DashboardBLL.DashBoard_GetTotalStatisticalOrder(timeStart, timeEnd, country), JsonRequestBehavior.AllowGet);
        }
        public ActionResult CountryUnitPrice(DateTime timeStart, DateTime timeEnd, string country)
        {
            timeEnd = timeEnd.AddMonths(1).AddDays(-1);
            return Json(DashboardBLL.DashBoard_GetCountryUnitPrice(timeStart, timeEnd, country), JsonRequestBehavior.AllowGet);
        }
    }
}