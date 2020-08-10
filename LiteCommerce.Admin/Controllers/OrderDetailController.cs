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
    public class OrderDetailController : Controller
    {
        // GET: OrderDetail
        public ActionResult Index(string searchValue="",int orderID=0)
        {
            if (TempData["orderId"]!= null)
            {
                var model = new Models.OrderDetailPaginationResult()
                {
                    SearchValue = searchValue,
                    OrderID = orderID,
                    Page = 1,
                    PageSize = AppSettings.DefaultPagesize,
                    RowCount = SaleManagementBLL.OrderDetail_Count(searchValue, (int)TempData["orderId"]),
                    Data = SaleManagementBLL.OrderDetail_List(searchValue, (int)TempData["orderId"])
                };
                return View(model);
            }
            if (orderID == 0)
                return RedirectToAction("Index", "Order");
            var modelOrderDetail = new Models.OrderDetailPaginationResult()
            {
                SearchValue=searchValue,
                OrderID = orderID,
                Page = 1,
                PageSize = AppSettings.DefaultPagesize,
                RowCount = SaleManagementBLL.OrderDetail_Count(searchValue, orderID),
                Data = SaleManagementBLL.OrderDetail_List(searchValue, orderID)
            };
            return View(modelOrderDetail);
        }
        public ActionResult Input(int orderID = 0,int productID=0)
        {
            if (orderID == 0)
                return RedirectToAction("Index", "Order");
            if (productID==0)
            {
                ViewBag.Title = "add new Orderdetail";
                OrderDetail newOrderTail = new OrderDetail();
                newOrderTail.ProductID = 0;
                newOrderTail.OrderID = orderID;
                return View(newOrderTail);
            }
            else
            {
                ViewBag.Title = "edit Orderdetail";
                OrderDetail editOrderDetail = SaleManagementBLL.OrderDetail_Get(orderID,productID);
                return View(editOrderDetail);
            }
        }
        [HttpPost] //lkhi submit dữ liệu
        public ActionResult Input(OrderDetail model,string discountString,int oldProductID)
        {
            if (string.IsNullOrEmpty(model.ProductID.ToString()))
                ModelState.AddModelError("Product", "product required");
            if (string.IsNullOrEmpty(model.UnitPrice.ToString()))
                ModelState.AddModelError("UnitPrice", "UnitPrice required");
            if (string.IsNullOrEmpty(model.Quantity.ToString()))
                ModelState.AddModelError("Quantity", "Quantity required");
            if (string.IsNullOrEmpty(discountString.ToString()))
                ModelState.AddModelError("Discount", "Discount required");
            if (!ModelState.IsValid)
                return View(model);
            model.Discount = Convert.ToDecimal(discountString);
            try
            {
                if (oldProductID == 0)
                {
                    int Id = SaleManagementBLL.OrderDetail_Add(model);
                    TempData["orderId"] = model.OrderID;
                    return RedirectToAction("Index");
                }
                else
                {
                    bool updateResult = SaleManagementBLL.OrderDetail_Update(model,oldProductID);
                    TempData["orderId"] = model.OrderID;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Product already exist");
                OrderDetail newOrderTail = new OrderDetail();
                if (oldProductID == 0)
                {
                    newOrderTail.OrderID = model.OrderID;
                }else
                    newOrderTail= SaleManagementBLL.OrderDetail_Get(model.OrderID,oldProductID);
                return View(newOrderTail);
                //TempData["orderId"] = model.OrderID;
                //return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult Delete(string method = "", int orderID = 0,int[] productIDs=null)
        {
            try
            {
                if (productIDs != null)
                {
                    SaleManagementBLL.OrderDetail_Delete(orderID, productIDs);
                }
                TempData["orderId"] = orderID;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["orderId"] = orderID;
                return RedirectToAction("Index");
            }
        }
    }
}