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
using JLPTWeb.ViewModels;

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
        public ActionResult StudyDetailModal()
        {
            ViewBag.LstTopicVoc = db.A_TopicVoc.FirstOrDefault();
            return View(db.A_TopicVoc.FirstOrDefault());
        }

        // GET: A_TopicSentence/Details/5
        public ActionResult StudyDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IQueryable<VocSenViewModel> lsVocSen = (from m in db.A_TopicVocDetail
                        where m.TopicVocId == id
                        select new VocSenViewModel() {
                            VocContent = m.A_Voc_Sen.A_Voc_Mean.A_Vocabulary.VocContent,
                            VocKanji = m.A_Voc_Sen.A_Voc_Mean.A_Vocabulary.VocKanji,
                            VocHiragana = m.A_Voc_Sen.A_Voc_Mean.A_Vocabulary.VocHiragana,
                            MeanContent = m.A_Voc_Sen.A_Voc_Mean.A_Mean.MeanContent,
                            Sentence = m.A_Voc_Sen.A_Sen_Mean.A_Sentence.Sentence,
                            SenFormat = m.A_Voc_Sen.A_Sen_Mean.A_Sentence.SenFormat,
                            MeanSenContent = m.A_Voc_Sen.A_Sen_Mean.A_Mean.MeanContent
                        });
                //db.A_TopicVoc.Where(m => m.TopicVocId == id).FirstOrDefault();
            return View(lsVocSen);
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
