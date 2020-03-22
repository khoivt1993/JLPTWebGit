using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JLPTWeb.Models;
using System.Collections.Specialized;
using System.Text;
using JLPTWeb.Helper;
using System.Data.Entity.Validation;
using HiraKana;
using JLPTWeb.Business;
using JLPTWeb.Properties;
using System.IO;

namespace JLPTWeb.Controllers
{
    public class A_Voc_MeanController : Controller
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        [HttpPost]
        [MultipleButton(Name = "Index", Argument = "Search")]
        public ActionResult Search(string ctrSearchVoc)
        {
            if ("".Equals(ctrSearchVoc))
            {
                return View();
            }
            // khi serch bang romaji se chuyen sang hiragana
            KanaTools kanaTool = new KanaTools();
            if (kanaTool.IsRomaji(ctrSearchVoc))
            {
                ctrSearchVoc = kanaTool.ToHiragana(ctrSearchVoc);
            }
            // get sentence 
            List<A_Sen_Mean> lstSenMean = new List<A_Sen_Mean>();
            Sen_Mean_Bus senMeanBus = new Sen_Mean_Bus();
            lstSenMean = senMeanBus.getListSentence(ctrSearchVoc, 10);
            ViewBag.LstSenMean = lstSenMean.ToList();

            // get sentence of vocabulary
            List<A_Voc_Sen> lstVocSen = new List<A_Voc_Sen>();
            lstVocSen = db.A_Voc_Sen.Where(s => s.A_Sen_Mean.A_Sentence.SenSearch.Contains(ctrSearchVoc)).ToList();
            ViewBag.lstVocSen = lstVocSen;
            ViewBag.NullSenMeanId = Settings.Default.NullSenMeanId;

            //get data tu DB truong hop khong ton tai get tu mazii
            //var a_Voc_Mean = db.A_Voc_Mean.Include(a => a.A_Mean).Include(a => a.A_User).Include(a => a.A_User1).Include(a => a.A_Vocabulary);
            //var a_Voc_Mean = db.A_Voc_Mean.Where(a => a.A_Vocabulary.VocContent.Equals(ctrSearchVoc) || a.A_Vocabulary.VocHiragana.Equals(ctrSearchVoc));
            List<A_Voc_Mean> lstVocMean = new List<A_Voc_Mean>();
            lstVocMean = db.A_Voc_Mean.Where(a => a.A_Vocabulary.VocContent.Equals(ctrSearchVoc) || a.A_Vocabulary.VocHiragana.Equals(ctrSearchVoc)).ToList();
            if (lstVocMean.Count > 0)
            {
                return View(lstVocMean);
            }
            // U_Start 20200322 Khoi-VT sua add vao table history
            //Translate_Bus tranBus = new Translate_Bus();
            //tranBus.getDataFromApi(ctrSearchVoc);

            //lstSenMean = new List<A_Sen_Mean>();
            //senMeanBus = new Sen_Mean_Bus();
            //lstSenMean = senMeanBus.getListSentence(ctrSearchVoc, 10);
            //ViewBag.LstSenMean = lstSenMean.ToList();


            //// get sentence of vocabulary
            //lstVocSen = new List<A_Voc_Sen>();
            //lstVocSen = db.A_Voc_Sen.Where(s => s.A_Sen_Mean.A_Sentence.SenSearch.Contains(ctrSearchVoc)).ToList();
            //ViewBag.lstVocSen = lstVocSen;

            ////a_Voc_Mean = db.A_Voc_Mean.Include(a => a.A_Mean).Include(a => a.A_User).Include(a => a.A_User1).Include(a => a.A_Vocabulary);
            //a_Voc_Mean = a_Voc_Mean.Where(a => (a.A_Vocabulary.VocContent == ctrSearchVoc || a.A_Vocabulary.VocHiragana == ctrSearchVoc) &&
            //                                    a.ApproveFlag == 1 && a.ActiveFlag == 1);
            //return View(a_Voc_Mean.ToList());
            if (!db.A_VocHistory.Any(s => s.VocHistoryContent == ctrSearchVoc))
            {
                A_VocHistory vocHis = new A_VocHistory();
                vocHis.VocHistoryContent = ctrSearchVoc;
                vocHis.ActiveFlag = 1;
                vocHis.CreateTime = DateTime.Now;
                vocHis.UpdateTime = DateTime.Now;
                db.A_VocHistory.Add(vocHis);
                db.SaveChanges();
            }
            return View();
            // U_End 20200322 Khoi-VT sua add vao table history
        }
        // GET: A_Voc_Mean
        public ActionResult Index()
        {

            var a_Voc_Mean = db.A_Voc_Mean.Include(a => a.A_Mean).Include(a => a.A_User).Include(a => a.A_User1).Include(a => a.A_Vocabulary);
            return View();
        }

        // GET: A_Voc_Mean/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            A_Voc_Mean a_Voc_Mean = db.A_Voc_Mean.Find(id);
            if (a_Voc_Mean == null)
            {
                return HttpNotFound();
            }
            return View(a_Voc_Mean);
        }

        // GET: A_Voc_Mean/Create
        public ActionResult Create()
        {
            ViewBag.MeanId = new SelectList(db.A_Mean, "MeanId", "MeanContent");
            ViewBag.UserId = new SelectList(db.A_User, "UserId", "Email");
            ViewBag.ApproveUserId = new SelectList(db.A_User, "UserId", "Email");
            ViewBag.VocId = new SelectList(db.A_Vocabulary, "VocId", "VocContent");
            return View();
        }

        // POST: A_Voc_Mean/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VocMeanId,VocId,MeanId,Sort,ApproveUserId,ApproveFlag,UserId,ActiveFlag,CreateTime,UpdateTime,DeleteTime")] A_Voc_Mean a_Voc_Mean)
        {
            if (ModelState.IsValid)
            {
                db.A_Voc_Mean.Add(a_Voc_Mean);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MeanId = new SelectList(db.A_Mean, "MeanId", "MeanContent", a_Voc_Mean.MeanId);
            ViewBag.UserId = new SelectList(db.A_User, "UserId", "Email", a_Voc_Mean.UserId);
            ViewBag.ApproveUserId = new SelectList(db.A_User, "UserId", "Email", a_Voc_Mean.ApproveUserId);
            ViewBag.VocId = new SelectList(db.A_Vocabulary, "VocId", "VocContent", a_Voc_Mean.VocId);
            return View(a_Voc_Mean);
        }

        // GET: A_Voc_Mean/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            A_Voc_Mean a_Voc_Mean = db.A_Voc_Mean.Find(id);
            if (a_Voc_Mean == null)
            {
                return HttpNotFound();
            }
            ViewBag.MeanId = new SelectList(db.A_Mean, "MeanId", "MeanContent", a_Voc_Mean.MeanId);
            ViewBag.UserId = new SelectList(db.A_User, "UserId", "Email", a_Voc_Mean.UserId);
            ViewBag.ApproveUserId = new SelectList(db.A_User, "UserId", "Email", a_Voc_Mean.ApproveUserId);
            ViewBag.VocId = new SelectList(db.A_Vocabulary, "VocId", "VocContent", a_Voc_Mean.VocId);
            return View(a_Voc_Mean);
        }

        // POST: A_Voc_Mean/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VocMeanId,VocId,MeanId,Sort,ApproveUserId,ApproveFlag,UserId,ActiveFlag,CreateTime,UpdateTime,DeleteTime")] A_Voc_Mean a_Voc_Mean)
        {
            if (ModelState.IsValid)
            {
                db.Entry(a_Voc_Mean).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MeanId = new SelectList(db.A_Mean, "MeanId", "MeanContent", a_Voc_Mean.MeanId);
            ViewBag.UserId = new SelectList(db.A_User, "UserId", "Email", a_Voc_Mean.UserId);
            ViewBag.ApproveUserId = new SelectList(db.A_User, "UserId", "Email", a_Voc_Mean.ApproveUserId);
            ViewBag.VocId = new SelectList(db.A_Vocabulary, "VocId", "VocContent", a_Voc_Mean.VocId);
            return View(a_Voc_Mean);
        }

        // GET: A_Voc_Mean/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            A_Voc_Mean a_Voc_Mean = db.A_Voc_Mean.Find(id);
            if (a_Voc_Mean == null)
            {
                return HttpNotFound();
            }
            return View(a_Voc_Mean);
        }

        // POST: A_Voc_Mean/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            A_Voc_Mean a_Voc_Mean = db.A_Voc_Mean.Find(id);
            db.A_Voc_Mean.Remove(a_Voc_Mean);
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
