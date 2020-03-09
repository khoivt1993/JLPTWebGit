using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JLPTWeb.Models;

namespace JLPTWeb.Controllers
{
    public class M_GrammarController : Controller
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        // GET: M_Grammar
        public ActionResult Index(string levelId)
        {
            var m_Grammar = db.M_Grammar.Include(m => m.M_Level).Where(m => m.M_Level.LevelID == levelId);
            return View(m_Grammar.ToList());
        }

        // GET: M_Grammar/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            M_Grammar m_Grammar = db.M_Grammar.Find(id);
            if (m_Grammar == null)
            {
                return HttpNotFound();
            }
            return View(m_Grammar);
        }

        // GET: M_Grammar/Create
        public ActionResult Create()
        {
            ViewBag.LevelID = new SelectList(db.M_Level, "LevelID", "Description");
            return View();
        }

        // POST: M_Grammar/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GrammarID,LevelID,BookID,GrammarName,Struct,MeanVN,MeanEN,FullVN,FullEN,FullJP,Relationship,Group,SearchKey,Yobi_2,Yobi_3,Yobi_4,InsDate,UpdDate")] M_Grammar m_Grammar)
        {
            if (ModelState.IsValid)
            {
                db.M_Grammar.Add(m_Grammar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LevelID = new SelectList(db.M_Level, "LevelID", "Description", m_Grammar.LevelID);
            return View(m_Grammar);
        }

        // GET: M_Grammar/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            M_Grammar m_Grammar = db.M_Grammar.Find(id);
            if (m_Grammar == null)
            {
                return HttpNotFound();
            }
            ViewBag.LevelID = new SelectList(db.M_Level, "LevelID", "Description", m_Grammar.LevelID);
            return View(m_Grammar);
        }

        // POST: M_Grammar/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GrammarID,LevelID,BookID,GrammarName,Struct,MeanVN,MeanEN,FullVN,FullEN,FullJP,Relationship,Group,SearchKey,Yobi_2,Yobi_3,Yobi_4,InsDate,UpdDate")] M_Grammar m_Grammar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(m_Grammar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LevelID = new SelectList(db.M_Level, "LevelID", "Description", m_Grammar.LevelID);
            return View(m_Grammar);
        }

        // GET: M_Grammar/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            M_Grammar m_Grammar = db.M_Grammar.Find(id);
            if (m_Grammar == null)
            {
                return HttpNotFound();
            }
            return View(m_Grammar);
        }

        // POST: M_Grammar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            M_Grammar m_Grammar = db.M_Grammar.Find(id);
            db.M_Grammar.Remove(m_Grammar);
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
