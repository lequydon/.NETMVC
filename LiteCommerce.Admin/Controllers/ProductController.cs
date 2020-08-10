using LiteCommerce.BusinessLayers;
using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiteCommerce.Admin.Controllers
{
    [Authorize(Roles = WebUserRoles.STAFF)]
    [Authorize]
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index(int page=1 ,string searchValue="",int CategoryID=0,int SupplierID=0)
        {
            var model = new Models.ProductPaginationResult()
            {
                SearchValue = searchValue,
                CategoryID=CategoryID,
                SupplierID=SupplierID,
                Page = page,
                PageSize = AppSettings.DefaultPagesize,
                RowCount = CatalogBLL.Products_Count(searchValue, CategoryID, SupplierID),
                Data = CatalogBLL.Products_List(page, AppSettings.DefaultPagesize, searchValue, CategoryID, SupplierID)
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
        public ActionResult Input(string id = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Title = "add new Product";
                Product newProducts = new Product();
                newProducts.ProductID = 0;
                return View(newProducts);
            }
            else
            {
                ViewBag.Title = "edit Product";
                Product editProduct = CatalogBLL.Products_Get(Convert.ToInt32(id));
                return View(editProduct);
            }
        }
        [HttpPost]
        public ActionResult Input(Product model, HttpPostedFileBase file)
        {
            if (string.IsNullOrEmpty(model.ProductName))
                ModelState.AddModelError("ProductName", "Product Name required");
            if (string.IsNullOrEmpty(model.QuantityPerUnit))
                model.QuantityPerUnit = "";
            if (string.IsNullOrEmpty(model.UnitPrice.ToString()))
                model.UnitPrice = 0;
            if (string.IsNullOrEmpty(model.CategoryID.ToString()))
                model.CategoryID = 0;
            if (string.IsNullOrEmpty(model.SupplierID.ToString()))
                model.SupplierID = 0;
            var type = "Add";
            if (model.ProductID != 0)
                type = "Edit";
            var fileName = "";
            var typeFile = "";
            if (file != null)
            {
                //kiểm tra loại của file
                fileName = Path.GetFileName(file.FileName);
                typeFile = fileName.Substring(fileName.IndexOf('.'));
                if (typeFile != ".png" && typeFile != ".jpg" && typeFile != ".jpeg"&& typeFile != ".PNG" && typeFile != ".JPG" && typeFile != ".JPEG")
                {
                    ModelState.AddModelError("pathFile", "File is not image");
                    return View(model);
                }
            }
            else
            {
                model.PhotoPath = "";
            }
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                if (model.ProductID == 0)
                {
                    //var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    //file.SaveAs(path);
                    int supplierId = CatalogBLL.Products_Add(model, file);
                    return RedirectToAction("Index");
                }
                else
                {
                    bool updateResult = CatalogBLL.Products_Update(model, file);
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
        public ActionResult Delete(string method = "", int[] productIDs = null)
        {
            if (productIDs != null)
            {
                CatalogBLL.Product_Delete(productIDs);
            }
            return RedirectToAction("Index");
        }
        public ActionResult Detail()
        {
            return View();
        }
    }
}