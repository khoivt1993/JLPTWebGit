using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JLPTWeb.Models;
using JLPTWeb.Properties;

namespace JLPTWeb.Controllers
{
    public class A_TopicVocDetailController : Controller
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        // GET: A_TopicSentence
        public ActionResult Study(int levelId)
        {
            ViewBag.NullSenMeanId = Settings.Default.NullSenMeanId;
            var a_TopicVoc = db.A_TopicVoc.Where(m => m.LevelID == levelId);
            return View(a_TopicVoc.ToList());
        }

        // GET: A_TopicSentence/Details/5
        public ActionResult StudyDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var a_TopicVocDetail = db.A_TopicVocDetail.Where(m => m.TopicVocId == id);
            return View(a_TopicVocDetail.ToList());
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
