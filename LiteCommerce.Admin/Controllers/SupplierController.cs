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
    public class SupplierController : Controller
    {
        /// <summary>
        /// trang hiên rthij danh sách các supplier các liên kết liên quan
        /// </summary>
        /// <returns></returns>
        // GET: Supplier
        public ActionResult Index(int page=1,string searchValue="",string country="")
        {
            var model = new Models.SupplierPaginationResult()
            {
                SearchValue= searchValue,
                Country= country,
                Page = page,
                PageSize = AppSettings.DefaultPagesize,
                RowCount = CatalogBLL.Supplier_Count(searchValue,country),
                Data= CatalogBLL.Suppliers_List(page, AppSettings.DefaultPagesize, searchValue,country)
            };
            //var listofSuppliers = CatalogBLL.Suppliers_List(page, 10, searchValue);
            //int rowcount = CatalogBLL.Supplier_count(searchValue);
            //ViewBag.Count = rowcount;
            return View(model);
        }
        /// <summary>
        /// add or edit
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]//truy cập bằng url
        public ActionResult Input(string id= "")
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Title = "add new Supplier";
                ViewBag.Type = "Add";
                Supplier newSupplier = new Supplier();
                newSupplier.SupplierID = 0;
                return View(newSupplier);
            }
            else
            {
                ViewBag.Title = "edit supplier";
                ViewBag.Type = "Edit";
                Supplier editSupplier = CatalogBLL.Supplier_Get(Convert.ToInt32(id));
                if (editSupplier == null)
                {
                    return RedirectToAction("Index");
                }
                return View(editSupplier);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost] //lkhi submit dữ liệu
        public ActionResult Input(Supplier model,string type="")
        {
            if (string.IsNullOrEmpty(model.CompanyName))
                ModelState.AddModelError("CompanyName", "Company Name required");
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
            if (string.IsNullOrEmpty(model.HomePage))
                model.HomePage = "";
            if (string.IsNullOrEmpty(model.Phone))
                model.Phone = "";
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                if (type=="Add")
                {
                    int supplierId = CatalogBLL.Supplier_Add(model);
                    return RedirectToAction("Index");
                }
                else
                {
                    bool updateResult = CatalogBLL.Supplier_Update(model);
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("",ex.Message+": "+ex.StackTrace );
                return View();
            }
        }
        [HttpPost]
        public ActionResult Delete(string method="",int[] supplierIDs=null)
        {
            if(supplierIDs != null)
            {
                CatalogBLL.Supplier_Delete(supplierIDs);
            }
            return RedirectToAction("Index");
        }
    }
}