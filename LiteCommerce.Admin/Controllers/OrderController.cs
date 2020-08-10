using LiteCommerce.BusinessLayers;
using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiteCommerce.Admin.Controllers
{
    [Authorize(Roles = WebUserRoles.SALE)]
    [Authorize]
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index(int page = 1, string searchValue = "", string customerID = "", int employeeID = 0,int shipperID=0)
        {
            var model = new Models.OrderPaginationResult()
            {
                SearchValue=searchValue,
                searchValue = searchValue,
                CustomerID = customerID,
                EmployeeID = employeeID,
                ShipperID= shipperID,
                Page = page,
                PageSize = AppSettings.DefaultPagesize,
                RowCount = SaleManagementBLL.Order_Count(searchValue,employeeID,customerID,shipperID),
                Data = SaleManagementBLL.Order_List(page, AppSettings.DefaultPagesize, searchValue, employeeID, customerID, shipperID)
            };
            //var listofSuppliers = CatalogBLL.Suppliers_List(page, 10, searchValue);
            //int rowcount = CatalogBLL.Supplier_count(searchValue);
            //ViewBag.Count = rowcount;
            return View(model);
        }
        public ActionResult Input(string id = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Title = "add new Order";
                Order newOrder = new Order();
                newOrder.OrderID = 0;
                return View(newOrder);
            }
            else
            {
                ViewBag.Title = "edit Order";
                Order editOrder =SaleManagementBLL.Order_Get(Convert.ToInt32(id));
                return View(editOrder);
            }
        }
        [HttpPost]
        public ActionResult Input(Order model)
        {
            if (string.IsNullOrEmpty(model.ShipAddress))
                model.ShipAddress = "";
            if (string.IsNullOrEmpty(model.ShipCity))
                model.ShipCity = "";
            if (string.IsNullOrEmpty(model.Freight.ToString()))
                model.Freight = 0;
            if (string.IsNullOrEmpty(model.EmployeeID.ToString()))
                model.EmployeeID = 0;
            if (string.IsNullOrEmpty(model.CustomerID))
                model.CustomerID = "";
            if (string.IsNullOrEmpty(model.ShipperID.ToString()))
                model.ShipperID = 0;
            var dateTime = new DateTime(1900, 01, 01);
            var compareDatetime = DateTime.Compare(model.OrderDate, dateTime);
            if (compareDatetime < 0)
            {
                ModelState.AddModelError("OrderDate", "OrderDate is not format");
                return View(model);
            }
            compareDatetime = DateTime.Compare(model.RequiredDate, dateTime);
            if (compareDatetime < 0)
            {
                ModelState.AddModelError("RequiredDate", "RequiredDate is not format");
                return View(model);
            }
            compareDatetime = DateTime.Compare(model.ShippedDate, dateTime);
            if (compareDatetime < 0)
            {
                ModelState.AddModelError("ShippedDate", "ShippedDate is not format");
                return View(model);
            }
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                if (model.OrderID == 0)
                {
                    //var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    //file.SaveAs(path);
                    int orderId = SaleManagementBLL.Order_Add(model);
                    return RedirectToAction("Index");
                }
                else
                {
                    bool updateResult = SaleManagementBLL.Order_Update(model);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message + ": " + ex.StackTrace);
                return View();
            }
        }
        public ActionResult Delete(string method = "", int[] orderIDs = null)
        {
            if (orderIDs != null)
            {
                SaleManagementBLL.Order_Delete(orderIDs);
            }
            return RedirectToAction("Index");
        }
    }
}