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
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index(string searchValue="")
        {
            var model = new Models.CategoryPaginationResult()
            {
                Page = 1,
                SearchValue=searchValue,
                PageSize = AppSettings.DefaultPagesize,
                RowCount = CatalogBLL.Categories_Count(searchValue),
                Data = CatalogBLL.Categories_List(searchValue)
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
                ViewBag.Title = "add new Category";
                Category newCategory = new Category();
                newCategory.CategoryID = 0;
                return View(newCategory);
            }
            else
            {
                ViewBag.Title = "edit Category";
                Category editCategory = CatalogBLL.Categories_Get(Convert.ToInt32(id));
                return View(editCategory);
            }
        }
        [HttpPost] //lkhi submit dữ liệu
        public ActionResult Input(Category model)
        {
            if (string.IsNullOrEmpty(model.CategoryName))
                ModelState.AddModelError("CategoryName", "Category Name required");
            if (string.IsNullOrEmpty(model.Description))
                model.Description = "";
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                if (model.CategoryID==0)
                {
                    int supplierId = CatalogBLL.Categories_Add(model);
                    return RedirectToAction("Index");
                }
                else
                {
                    bool updateResult = CatalogBLL.Categories_Update(model);
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
        public ActionResult Delete(string method = "", int[] categoriesIDs = null)
        {
            if (categoriesIDs != null)
            {
                CatalogBLL.Categories_Delete(categoriesIDs);
            }
            return RedirectToAction("Index");
        }
    }
}