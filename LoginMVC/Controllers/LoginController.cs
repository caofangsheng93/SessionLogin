using LoginMVC.DAL;
using LoginMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LoginMVC.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            LoginDbContext db = new LoginDbContext();
            List<LoginModel> lstLoginModel = db.LoginModel.ToList();
            return View(lstLoginModel);
        }

        public ActionResult TestAdd()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TestAdd([Bind(Include = "UserName,Password,IsActive,UserId")] LoginModel model)   //没有Bind进来的字段就添加不进去数据库，
        {
            LoginDbContext db = new LoginDbContext();

            LoginModel loginModel = new LoginModel()
            {
                UserId = model.UserId,  //userID设置为了Key,既主键，EF会自动增长，这里添加的话，也不会报错。如果有UserID为1的记录我在添加也不会报错。加进去是递增的，是EF自动增长决定的，和模型绑定无关。
                UserName = model.UserName,
                Password = model.Password,
                IsActive = model.IsActive

            };
            db.LoginModel.Add(loginModel);
            db.SaveChanges();
            return RedirectToAction("Login");
        }

        #region Login
        public ActionResult Login()
        {
            return View();
        }

        #region V1.0版本
        //V1.0版本
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login([Bind(Include = "UserName,Password")] LoginModel model)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        using (var db = new LoginDbContext())
        //        {
        //            var loginModel = db.LoginModel.Where(s => s.UserName == model.UserName && s.Password == model.Password).FirstOrDefault();  

        //            if (loginModel != null)
        //            {
        //                //设置Session
        //                Session["UserID"] = loginModel.UserId.ToString();
        //                Session["UserName"] = loginModel.UserName.ToString();
        //                //登陆成功跳转到DashBoard页面
        //                return RedirectToAction("UserDashBoard");
        //            }
        //        } 

        //    }

        //    return View(model);
        //} 
        #endregion


        /// <summary>
        /// v2.0登陆版本
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "UserName,Password")] LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                LoginModel loginUserModel = new LoginModel();
                string pwd = GetUserPassword(model.UserName);
                if (string.IsNullOrEmpty(pwd))
                {
                    ModelState.AddModelError("", "用户名或密码错误，登陆失败，请联系管理员");
                }
                else
                {
                    //判断密码是否等于查出来的密码
                    if (model.Password.Equals(pwd))
                    {
                        //设置Session
                        Session["UserID"] = model.UserId.ToString();
                        Session["UserName"] = model.UserName.ToString();
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        return RedirectToAction("UserDashBoard", "Login");
                    }
                    else
                    {
                        ModelState.AddModelError("", "密码不正确");
                    }
                }

            }

            return View(model);
        }


        /// <summary>
        /// 登出功能
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }


        #endregion

        public ActionResult UserDashBoard()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        #region 获取密码 V2.0
        /// <summary>
        /// 获取密码 V2.0
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public string GetUserPassword(string loginName)
        {
            using (LoginDbContext db = new LoginDbContext())
            {
                var user = db.LoginModel.Where(o => o.UserName.ToLower().Equals(loginName));
                if (user.Any())
                    return user.FirstOrDefault().Password;
                else
                    return string.Empty;
            }
        } 
        #endregion

//        public bool IsUserInRole(string loginName, string roleName) {  
//            using (LoginDbContext db = new LoginDbContext()) {  
//                LoginModel SU = db.LoginModel.Where(o => o.UserName.ToLower().Equals(loginName)).FirstOrDefault();  
//                if (SU != null) {
//                    var roles = from q in db.LoginModel  
//                                join r in db.LOOKUPRoles on q.LOOKUPRoleID equals r.LOOKUPRoleID  
//                                where r.RoleName.Equals(roleName) && q.SYSUserID.Equals(SU.SYSUserID)  
//                                select r.RoleName;  
  
//                    if (roles != null) {  
//                        return roles.Any();  
//                    }  
//                }  
  
//                return false;  
//            }  
//}  

    }
}