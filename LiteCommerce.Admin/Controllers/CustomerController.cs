using LiteCommerce.BusinessLayers;
using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiteCommerce.Admin.Controllers
{
    [Authorize(Roles = WebUserRoles.STAFF)]
    [Authorize]
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Index(int page = 1, string searchValue = "",string country="")
        {
            var model = new Models.CustomerPaginationResult()
            {
                SearchValue=searchValue,
                Country=country,
                Page = page,
                PageSize = AppSettings.DefaultPagesize,
                RowCount = CatalogBLL.Customers_Count(searchValue, country),
                Data = CatalogBLL.Customers_List(page, AppSettings.DefaultPagesize, searchValue, country)
            };
            return View(model);
        }
        /// <summary>
        /// add or edit
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult Input(string id = "")
        {
            //ViewBag.ListCountry = CatalogBLL.Country_List();
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Title = "add new Customer";
                ViewBag.Type = "Add";
                Customer newCustomer = new Customer();
                newCustomer.CustomerID = "";
                return View(newCustomer);
            }
            else
            {
                ViewBag.Title = "edit Customer";
                ViewBag.Type = "Edit";
                Customer editCustomer = CatalogBLL.Customers_Get(id);
                return View(editCustomer);
            }
        }
        [HttpPost] //lkhi submit dữ liệu
        public ActionResult Input(Customer model,string type="")
        {
            ViewBag.ListCountry = CatalogBLL.Country_List();
            if (string.IsNullOrEmpty(model.CompanyName))
                model.Address = "";
            if (string.IsNullOrEmpty(model.Address))
                model.Address = "";
            if (string.IsNullOrEmpty(model.City))
                model.City = "";
            if (string.IsNullOrEmpty(model.Address))
                model.Address = "";
            if (string.IsNullOrEmpty(model.ContactName))
                model.ContactName = "";
            if (string.IsNullOrEmpty(model.Country))
                model.Country = "";
            if (string.IsNullOrEmpty(model.Fax))
                model.Fax = "";
            if (string.IsNullOrEmpty(model.Phone))
                model.Phone = "";
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                if (type=="Add")
                {
                    int supplierId = CatalogBLL.Customers_Add(model);
                    return RedirectToAction("Index");
                }
                else
                {
                    bool updateResult = CatalogBLL.Customers_Update(model);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message + ": " + ex.StackTrace);
                return View();
            }
        }
        [HttpPost]
        public ActionResult Delete(string method = "", string[] customerIDs = null)
        {
            if (customerIDs != null)
            {
                CatalogBLL.Customers_Delete(customerIDs);
            }
            return RedirectToAction("Index");
        }
    }
}