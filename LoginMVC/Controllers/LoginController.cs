using LoginMVC.DAL;
using LoginMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
                UserId=model.UserId,  //userID设置为了Key,既主键，EF会自动增长，这里添加的话，也不会报错。如果有UserID为1的记录我在添加也不会报错。加进去是递增的，是EF自动增长决定的，和模型绑定无关。
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "UserName,Password")] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new LoginDbContext())
                {
                    var loginModel = db.LoginModel.Where(s => s.UserName == model.UserName && s.Password == model.Password).FirstOrDefault();
                    if (loginModel != null)
                    {
                        //设置Session
                        Session["UserID"] = loginModel.UserId.ToString();
                        Session["UserName"] = loginModel.UserName.ToString();
                        //登陆成功跳转到DashBoard页面
                        return RedirectToAction("UserDashBoard");
                    }
                }
            }

            return View(model);
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
    }
}