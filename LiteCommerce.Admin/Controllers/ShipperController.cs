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
    public class ShipperController : Controller
    {
        // GET: Shipper
        public ActionResult Index(string searchValue="")
        {
            var model = new Models.ShipperPanginationResult()
            {
                SearchValue=searchValue,
                Page = 1,
                PageSize = AppSettings.DefaultPagesize,
                RowCount = CatalogBLL.Shippers_Count(searchValue),
                Data = CatalogBLL.Shippers_List(searchValue)
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
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Title = "add new Shipper";
                ViewBag.Type = "Add";
                Shipper newShipper = new Shipper();
                newShipper.ShipperID = 0;
                return View(newShipper);
            }
            else
            {
                ViewBag.Title = "edit Shipper";
                ViewBag.Type = "Edit";
                Shipper editShipper = CatalogBLL.Shippers_Get(Convert.ToInt32(id));
                return View(editShipper);
            }
        }
        [HttpPost] //lkhi submit dữ liệu
        public ActionResult Input(Shipper model, string type = "")
        {
            if (string.IsNullOrEmpty(model.CompanyName))
                ModelState.AddModelError("CompanyName", "Company Name required");
            if (string.IsNullOrEmpty(model.Phone))
                model.Phone = "";
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                if (type == "Add")
                {
                    int supplierId = CatalogBLL.Shippers_Add(model);
                    return RedirectToAction("Index");
                }
                else
                {
                    bool updateResult = CatalogBLL.Shippers_Update(model);
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
        public ActionResult Delete(string method = "", int[] shipperIDs = null)
        {
            if (shipperIDs != null)
            {
                CatalogBLL.Shippers_Delete(shipperIDs);
            }
            return RedirectToAction("Index");
        }
    }
}