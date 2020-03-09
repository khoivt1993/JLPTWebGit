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
    public class A_TopicSentenceController : Controller
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        // GET: A_TopicSentence
        public ActionResult Study()
        {
            var a_Topic = db.A_Topic.Where(m => m.ActiveFlag == 1);
            return View(a_Topic.ToList());
        }

        // GET: A_TopicSentence/Details/5
        public ActionResult StudyDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var a_TopicSentence = db.A_TopicSentence.Where(m => m.TopicId == id);
            return View(a_TopicSentence.ToList());
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
