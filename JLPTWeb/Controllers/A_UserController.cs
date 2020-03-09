using JLPTWeb.DAO;
using JLPTWeb.Models;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JLPTWeb.Controllers
{
    public class A_UserController : Controller
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        // GET: User
        public ActionResult UserProfile()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("Index", "A_Sen_Mean");
            }
            
            string email = Session["Email"].ToString();
            var userDetails = db.A_User.Where(m => m.Email == email).FirstOrDefault();
            ViewBag.LevelId = new SelectList(db.A_Level, "LevelId", "Description", userDetails.LevelId);
            return View(userDetails);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile(A_User tblUser, string sex)
        {
            if (ModelState.IsValid)
            {
                UserDao userDao = new UserDao();
                tblUser.Sex = sex;
                userDao.Update(tblUser);
                return RedirectToAction("UserProfile");
            }
            ViewBag.LevelId = new SelectList(db.A_Level, "LevelId", "Description", tblUser.LevelId);
            return View(tblUser);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(A_User tblUser)
        {
            var userDetails = db.A_User.Where(m => m.Email == tblUser.Email && m.Password == tblUser.Password).FirstOrDefault();
            if (userDetails == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }
            else
            {
                Session["UserId"] = userDetails.UserId;
                Session["Email"] = userDetails.Email;
                return RedirectToAction("UserProfile", "A_User");
            }
        }

        // GET: User
        public ActionResult Create()
        {
            return View();
        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(A_User tblUser)
        {
            if (ModelState.IsValid)
            {
                UserDao userDao = new UserDao();
                A_User user = new A_User();
                user = db.A_User.Where(m => m.Email == tblUser.Email).SingleOrDefault();
                if (user != null)
                {
                    ViewBag.NotValidUser = "Email đã được đăng kí";
                } else
                {
                    userDao.Insert(tblUser);
                    user = db.A_User.Where(m => m.Email == tblUser.Email).SingleOrDefault();
                    Session["UserId"] = user.UserId;
                    Session["Email"] = user.Email;
                    return RedirectToAction("UserProfile");
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "A_Sen_Mean");
        }
    }
}