using LiteCommerce.Admin;
using LiteCommerce.BusinessLayers;
using LiteCommerce.DataLayers;
using LiteCommerce.DomainModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LiteCommerce.Admin.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {/// <summary>
     /// trang quản lý thông tin user 
     /// </summary>
     /// <returns></returns>
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ChangePwd(Account account,String oldPassword,String reNewPassword)
        {
            if(Request.HttpMethod=="GET")
            {
                return View();
            }
            else
            {
                try
                {
                    WebUserData userData = User.GetUserData();
                    account.Email = userData.UserID;
                    Account oldAccount = new Account();
                    oldAccount.Email = account.Email;
                    oldAccount.Password = oldPassword;
                    if (UserAccountBLL.Account_Get(oldAccount, UserAccountTypes.Employee))
                    {
                        if (account.Password == reNewPassword)
                        {
                            UserAccountBLL.Account_Update(account,UserAccountTypes.Employee);
                            return RedirectToAction("Index", "DashBoard");
                        }
                        ModelState.AddModelError("reNewPass", "Password incorrect,please try again");
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Wrong password");
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message + ": " + ex.StackTrace);
                    return View();
                }
            }
        }
        public ActionResult Signout()
        {
            Session.Abandon();
            Session.Clear();
            System.Web.Security.FormsAuthentication.SignOut();
            //var nameCoockie = new HttpCookie("name");
            //nameCoockie.Expires = DateTime.Now.AddDays(-1);
            //Response.Cookies.Add(nameCoockie);
            //var photoPathCoockie = new HttpCookie("photoPath");
            //photoPathCoockie.Expires = DateTime.Now.AddDays(-1);
            //Response.Cookies.Add(photoPathCoockie);
            //Response.Cookies.Remove("name");
            //Response.Cookies.Remove("photoPath");
            return RedirectToAction("Login", "Account");
        }
        [AllowAnonymous]
        public ActionResult Login(Account account)
        {
            if (Request.HttpMethod == "GET")
            {
                return View();
            }
            else
            {
                //TODO:kiểm tra đăng nhập qua csdl
                //if(AccountBLL.Account_Get(account))
                //{
                //    Response.Cookies["name"].Value = AccountBLL.Account_GetEmployee(account).FirstName+" "+ AccountBLL.Account_GetEmployee(account).LastName;
                //    Response.Cookies["photoPath"].Value = AccountBLL.Account_GetEmployee(account).PhotoPath;
                //    System.Web.Security.FormsAuthentication.SetAuthCookie(account.Email, false);
                //    return RedirectToAction("Index", "Dashboard");
                //}
                //else
                //{
                //    ModelState.AddModelError("", "login fail");
                //    ViewBag.Email = account.Email;
                //    return View();
                //}
                var userAccount = UserAccountBLL.Authorize(account.Email, account.Password, UserAccountTypes.Employee);
                if(userAccount.UserID==null)
                {
                    ModelState.AddModelError("", "login fail");
                    ViewBag.email = account.Email;
                    return View();
                }
                if (userAccount.UserID != null)
                {
                    WebUserData cookieData = new WebUserData()
                    {
                        UserID=userAccount.UserID,
                        FullName=userAccount.FullName,
                        GroupName= userAccount.GroupName,
                        SessionID=Session.SessionID,
                        ClientIP=Request.UserHostAddress,
                        Photo=userAccount.Photo
                    };
                    FormsAuthentication.SetAuthCookie(cookieData.ToCookieString(), false);
                    return RedirectToAction("Index", "DashBoard");
                }
            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult Forget(string email="",string recaptcha=null)
        {
            if (Request.HttpMethod == "GET")
            {
                return View();
            }
            //validate captcha
            //var response = Request["g-Recaptch-Response"];
            string secretKey = "6LdQm6kZAAAAAF22PPW7ALrTFfs6BIc8BoIRffUe";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, recaptcha));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            if (status)
            {
                if (HumanResourceBLL.Employee_CheckMail(email, "Add"))
                {
                    ModelState.AddModelError("", "Email is not exist");
                    return View();
                }
                string code = Guid.NewGuid().ToString();
                UserAccountBLL.SetCode(email, code, UserAccountTypes.Employee);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { email = email, code = code }, protocol: Request.Url.Scheme);
                Email.sendMail(callbackUrl, email);
                ViewBag.Status = "Sended";
                return View();
            }
            else
            {
                ViewBag.emai = email;
                ModelState.AddModelError("capchaError", "CAPTCHA was incorrect. Try again");
                return View();
            }
        }
        [AllowAnonymous]
        public ActionResult ResetPassword(string email="",string code="",string newpassword = "",string retypePassword="")
        {
            ViewBag.Email = email;
            ViewBag.code = code;
            if(UserAccountBLL.GetCode(email,UserAccountTypes.Employee)!=code)
            {
                ViewBag.Eror = "Eror";
                return View();
            }
            if (newpassword != "")
            {
                if(newpassword != retypePassword)
                {
                    ModelState.AddModelError("", "Password incorrect");
                    return View();
                }
                else
                {
                    Account account = new Account();
                    account.Email = email;
                    account.Password= newpassword;
                    UserAccountBLL.Account_Update(account, UserAccountTypes.Employee);
                    UserAccountBLL.SetCode(email, Guid.NewGuid().ToString(), UserAccountTypes.Employee);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }
        public ActionResult Profile(Employee model, HttpPostedFileBase file)
        {
            //string temp = Request.Cookies["name"].Value;
            if (model.Email == null)
            {
                Account account = new Account();
                account.Email = User.GetUserData().UserID;
                UserProfile userProfile = UserAccountBLL.Account_GetEmployee(account,UserAccountTypes.Employee);
                return View(userProfile);
            }
            else
            {
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
                if (compareDatetime < 0)
                {
                    ModelState.AddModelError("HireDate", "HireDate is not format");
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
                    if (typeFile != ".png" && typeFile != ".jpg" && typeFile != ".jpeg" && typeFile != ".JPG" && typeFile != ".PNG" && typeFile != ".JPEG")
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
                        bool updateResult = UserAccountBLL.Employee_Update(model, file, UserAccountTypes.Employee);
                        Account account = new Account();
                        account.Email = model.Email;
                        UserProfile userProfile = UserAccountBLL.Account_GetEmployee(account,UserAccountTypes.Employee);
                        //set cookie
                        WebUserData cookieData = new WebUserData()
                        {
                            UserID = userProfile.Email,
                            FullName = userProfile.FirstName+" "+ userProfile.LastName,
                            GroupName = userProfile.GroupName,
                            SessionID = Session.SessionID,
                            ClientIP = Request.UserHostAddress,
                            Photo = userProfile.PhotoPath
                        };
                        FormsAuthentication.SetAuthCookie(cookieData.ToCookieString(), false);
                    //var nameCoockie = new HttpCookie("name");
                    //nameCoockie.Expires = DateTime.Now.AddDays(-1);
                    //Response.Cookies.Add(nameCoockie);
                    //Response.Cookies["name"].Value = AccountBLL.Account_GetEmployee(account).FirstName + " " + AccountBLL.Account_GetEmployee(account).LastName;
                    //var photoPathCoockie = new HttpCookie("photoPath");
                    //photoPathCoockie.Expires = DateTime.Now.AddDays(-1);
                    //Response.Cookies.Add(photoPathCoockie);
                    //Response.Cookies["photoPath"].Value = AccountBLL.Account_GetEmployee(account).PhotoPath;
                    return View(userProfile);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message + ": " + ex.StackTrace);
                    return View();
                }
            }
        }
    }
}