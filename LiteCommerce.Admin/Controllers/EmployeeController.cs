using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LiteCommerce.BusinessLayers;
using LiteCommerce.DomainModels;

namespace LiteCommerce.Admin.Controllers
{
    [Authorize(Roles = WebUserRoles.ADMINISTRATOR)]
    [Authorize]
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index(int page = 1,string searchValue="",string country="")
        {
            var model = new Models.EmployeePaginationResult()
            {
                SearchValue=searchValue,
                Country=country,
                Page = page,
                PageSize = AppSettings.DefaultPagesize,
                RowCount = HumanResourceBLL.Employee_Count(searchValue, country),
                Data = HumanResourceBLL.Employee_List(page, AppSettings.DefaultPagesize, searchValue,country)
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
                ViewBag.Title = "Add new Employee";
                Employee newEmployss = new Employee();
                newEmployss.EmployeeID = 0;
                return View(newEmployss);
            }
            else
            {
                ViewBag.Type = "Edit";
                ViewBag.Title = "Edit Employee";
                Employee editEmployee = HumanResourceBLL.Employee_Get(Convert.ToInt32(id));
                return View(editEmployee);
            }
        }
        [HttpPost] //lkhi submit dữ liệu
        public ActionResult Input(Employee model, HttpPostedFileBase file,string[] role=null)
        {
            string groupName = "";
            if (role != null)
            {
                foreach (var eachRole in role)
                {
                    if (groupName == "")
                        groupName = groupName + eachRole;
                    else
                        groupName = groupName + "," + eachRole;
                }
            }model.GroupName = groupName;
            if (string.IsNullOrEmpty(model.LastName))
                ModelState.AddModelError("LastName", "Last Name required");
            if (string.IsNullOrEmpty(model.FirstName))
                ModelState.AddModelError("FirstName", "First Name required");
            if (string.IsNullOrEmpty(model.City))
                model.City = "";
            if (string.IsNullOrEmpty(model.Address))
                model.Address = "";
            if (string.IsNullOrEmpty(model.Title))
                model.Title = "";
            if (string.IsNullOrEmpty(model.Country))
                model.Country = "";
            if (string.IsNullOrEmpty(model.Email))
                model.Email = "";
            if (string.IsNullOrEmpty(model.HomePhone))
                model.HomePhone = "";
            if (string.IsNullOrEmpty(model.Notes))
                model.Notes = "";
            var dateTime = new DateTime(1900, 01, 01);
            var compareDatetime = DateTime.Compare(model.BirthDate, dateTime);
            if (compareDatetime < 0)
            {
                ModelState.AddModelError("BirthDate", "BirthDate is not format");
                return View(model);
            }
            compareDatetime = DateTime.Compare(model.HireDate, dateTime);
            if(compareDatetime<0)
            {
                ModelState.AddModelError("HireDate", "HireDate is not format");
                return View(model);
            }
            var type = "Add";
            if (model.EmployeeID != 0)
                type = "Edit";
            if(type=="Add")
            if (!HumanResourceBLL.Employee_CheckMail(model.Email, type))
            {
                ModelState.AddModelError("Email", "Email already exist");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.Password))
                ModelState.AddModelError("Password", "Password required");
            var fileName = "";
            var typeFile = "";
            if (file != null)
            {
               //kiểm tra loại của file
                fileName = Path.GetFileName(file.FileName);
                typeFile = fileName.Substring(fileName.IndexOf('.'));
                if (typeFile != ".png" && typeFile != ".jpg" && typeFile != ".jpeg" && typeFile != ".PNG" && typeFile != ".JPG" && typeFile != ".JPEG")
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
                if (model.EmployeeID == 0)
                {
                    //var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    //file.SaveAs(path);
                    int supplierId = HumanResourceBLL.Employee_Add(model, file);
                    return RedirectToAction("Index");
                }
                else
                {
                    bool updateResult = HumanResourceBLL.Employee_Update(model, file);
                    //set cookie
                    //var nameCoockie = new HttpCookie("name");
                    //nameCoockie.Expires = DateTime.Now.AddDays(-1);
                    //Response.Cookies.Add(nameCoockie);
                    //Response.Cookies["name"].Value = AccountBLL.Account_GetEmployee(account).FirstName + " " + AccountBLL.Account_GetEmployee(account).LastName;
                    //var photoPathCoockie = new HttpCookie("photoPath");
                    //photoPathCoockie.Expires = DateTime.Now.AddDays(-1);
                    //Response.Cookies.Add(photoPathCoockie);
                    //Response.Cookies["photoPath"].Value = AccountBLL.Account_GetEmployee(account).PhotoPath;
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
        public ActionResult Delete(string method = "", int[] employeeIDs = null)
        {
            if (employeeIDs != null)
            {
                HumanResourceBLL.Employee_Delete(employeeIDs);
            }
            return RedirectToAction("Index");
        }
    }
}