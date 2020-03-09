using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JLPTWeb.Models;
using PagedList.Mvc;
using PagedList;

namespace JLPTWeb.Controllers
{
    public class B_NewsController : Controller
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        // GET: B_News
        public ActionResult Index(int? id, int?page)
        {
            var lstNews = db.B_News.Where(s => s.ActiveFlag == 1).OrderByDescending(s => s.NewsDate).Take(30);
            if (id == null)
            {
                ViewBag.lstNews = lstNews.FirstOrDefault();
            } else
            {
                ViewBag.lstNews = lstNews.Where(s => s.NewsId == id).FirstOrDefault();
            }
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            return View(lstNews.ToPagedList(pageNumber, pageSize));
        }

        // GET: B_News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            B_News b_News = db.B_News.Find(id);
            if (b_News == null)
            {
                return HttpNotFound();
            }
            return View(b_News);
        }

        // GET: B_News/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: B_News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NewsId,Title,NewContent,ImagePath,MoviePath,NewIdNHK,NewsDate,NewsWebUrl,ActiveFlag,CreateTime,UpdateTime,DeleteTime,TitleWithRuby,NewContentWithRuby")] B_News b_News)
        {
            if (ModelState.IsValid)
            {
                db.B_News.Add(b_News);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(b_News);
        }

        // GET: B_News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            B_News b_News = db.B_News.Find(id);
            if (b_News == null)
            {
                return HttpNotFound();
            }
            return View(b_News);
        }

        // POST: B_News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NewsId,Title,NewContent,ImagePath,MoviePath,NewIdNHK,NewsDate,NewsWebUrl,ActiveFlag,CreateTime,UpdateTime,DeleteTime,TitleWithRuby,NewContentWithRuby")] B_News b_News)
        {
            if (ModelState.IsValid)
            {
                db.Entry(b_News).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(b_News);
        }

        // GET: B_News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            B_News b_News = db.B_News.Find(id);
            if (b_News == null)
            {
                return HttpNotFound();
            }
            return View(b_News);
        }

        // POST: B_News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            B_News b_News = db.B_News.Find(id);
            db.B_News.Remove(b_News);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
