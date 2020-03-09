using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JLPTWeb.Models;
using JLPTWeb.Business;
using JLPTWeb.Helper;

namespace JLPTWeb.Controllers
{
    public class A_Sen_MeanController : Controller
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();
        Sen_Mean_Bus senMeanBus = new Sen_Mean_Bus();

        // GET: A_Sen_Mean
        public ActionResult Index()
        {
            //List<A_Sen_Mean> lstSenMean = new List<A_Sen_Mean>();
            //lstSenMean = senMeanBus.getListSentence(String.Empty);
            //ViewBag.Country = 1; //JP

            ////Get so luong like va dislike
            //ViewBag.LstLikeAmount = senMeanBus.getLstLikeAmount(lstSenMean);
            //ViewBag.LstDislikeAmount = senMeanBus.getLstDislikeAmount(lstSenMean);
            //return View(lstSenMean);

            return View();
        }


        [HttpPost]
        [MultipleButton(Name = "Index", Argument = "Search")]
        public ActionResult Search(string ctrSearchSentence)
        {
            List<A_Sen_Mean> lstSenMean = new List<A_Sen_Mean>();
            lstSenMean = senMeanBus.getListSentence(ctrSearchSentence, 100);

            if(CommonHelper.checkUnicode(ctrSearchSentence))
            {
                ViewBag.Country = 1; //JP
            } else
            {
                ViewBag.Country = 0; //VN
            }

            //Get so luong like va dislike
            ViewBag.LstLikeAmount = senMeanBus.getLstLikeAmount(lstSenMean);
            ViewBag.LstDislikeAmount = senMeanBus.getLstDislikeAmount(lstSenMean);
            return View(lstSenMean);
        }

        [HttpPost]
        [MultipleButton(Name = "Index", Argument = "Add")]
        public ActionResult AddSentence(string ctrSearchSentence)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(ctrSearchSentence))
                {
                    //bao loi khong the insert
                    return View();
                }

                senMeanBus.addSentence(ctrSearchSentence);
            }
            return RedirectToAction("Index");
        }

        // GET: A_JPSentences/Like/5
        public ActionResult Like(string ctrSearchSentence, int senMeanId)
        {
            senMeanBus.setLike(senMeanId);
            return RedirectToAction("Index", new { ctrSearchSentence = ctrSearchSentence });
        }

        // GET: A_JPSentences/Like/5
        public ActionResult Dislike(string ctrSearchSentence, int senMeanId)
        {
            senMeanBus.setDisLike(senMeanId);
            return RedirectToAction("Index", new { ctrSearchSentence = ctrSearchSentence });
        }
        
        // GET: A_Translate/Create
        public ActionResult Create(long? SenMeanId)
        {
            if (SenMeanId != null)
            {
                var lstSenMean = senMeanBus.getListTransBySenId(SenMeanId, CommonHelper.getCountryId(Request.Cookies["Language"]));
                ViewBag.lstSenMean = lstSenMean;

                //Get so luong like va dislike
                ViewBag.LstLikeAmount = senMeanBus.getLstLikeAmount(lstSenMean);
                ViewBag.LstDislikeAmount = senMeanBus.getLstDislikeAmount(lstSenMean);
            }
            return View(senMeanBus.getSenByCountry(CommonHelper.getCountryId(Request.Cookies["Language"])));
        }

        [HttpPost]
        [MultipleButton(Name = "Create", Argument = "Add")]
        public ActionResult AddSentenceTrans([Bind(Include = "SenMeanId")] A_Sen_Mean tblSenMean, string ctrSentenceTrans)
        {
            int strCountry = CommonHelper.getCountryId(Request.Cookies["Language"]);
            if (ModelState.IsValid)
            {
                senMeanBus.addSentenceTrans(tblSenMean, ctrSentenceTrans, strCountry);
            }

            var lstSenMean = senMeanBus.getListTransBySenId(tblSenMean.SenMeanId, strCountry);
            ViewBag.lstSenMean = lstSenMean;

            //Get so luong like va dislike
            ViewBag.LstLikeAmount = senMeanBus.getLstLikeAmount(lstSenMean);
            ViewBag.LstDislikeAmount = senMeanBus.getLstDislikeAmount(lstSenMean);
            return View(senMeanBus.getSenByCountry(strCountry));
        }

        [HttpPost]
        [MultipleButton(Name = "Create", Argument = "Next")]
        public ActionResult NextSentence([Bind(Include = "SenMeanId")] A_Sen_Mean tblSenMean, string ListFlag)
        {
            int intCountry = CommonHelper.getCountryId(Request.Cookies["Language"]);
            if (ListFlag == "1")
            {
                ModelState.Clear();
                return View(senMeanBus.nextSentence(tblSenMean, intCountry));
            }
            else
            {
                var lstSenMean = senMeanBus.getListTransBySenId(tblSenMean.SenMeanId, intCountry);
                ViewBag.lstSenMean = lstSenMean;

                //Get so luong like va dislike
                ViewBag.LstLikeAmount = senMeanBus.getLstLikeAmount(lstSenMean);
                ViewBag.LstDislikeAmount = senMeanBus.getLstDislikeAmount(lstSenMean);
                return View(senMeanBus.getSenByCountry(intCountry));
            }
        }

        [HttpPost]
        [MultipleButton(Name = "Create", Argument = "ChangeLanguage")]
        public ActionResult ChangeLanguage()
        {
            if (Request.Cookies["Language"] == null || Request.Cookies["Language"].Value.ToString() == "VN")
            {
                Response.Cookies.Add(new System.Web.HttpCookie("Language", "JP"));
            }
            else
            {
                Response.Cookies.Add(new System.Web.HttpCookie("Language", "VN"));
            }
            return RedirectToAction("Create");
        }

        public ActionResult LikeCreatePage([Bind(Include = "SenMeanId")] A_Sen_Mean tblSenMean)
        {
            senMeanBus.setLike(tblSenMean.SenMeanId);
            return RedirectToAction("Create", new { SenMeanId = tblSenMean.SenMeanId });
        }

        // GET: A_JPSentences/Like/5
        public ActionResult DislikeCreatePage([Bind(Include = "SenMeanId")] A_Sen_Mean tblSenMean)
        {
            senMeanBus.setDisLike(tblSenMean.SenMeanId);
            return RedirectToAction("Create", new { SenMeanId = tblSenMean.SenMeanId });
        }

        public ActionResult ViewUser()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Index", "A_Sen_Mean");
            }
            int userId = int.Parse(Session["UserId"].ToString());
            var lstSenMeanFromSen = senMeanBus.getListTransBySenId(userId, 1);
            ViewBag.lstSenMeanFromSen = lstSenMeanFromSen;

            //Get so luong like va dislike
            ViewBag.LstLikeAmountFromSen = senMeanBus.getLstLikeAmount(lstSenMeanFromSen);
            ViewBag.LstDislikeAmountFromSen = senMeanBus.getLstDislikeAmount(lstSenMeanFromSen);

            var lstSenMeanFromMean = senMeanBus.getListTransBySenId(userId, 0);

            //Get so luong like va dislike
            ViewBag.LstLikeAmountFromMean = senMeanBus.getLstLikeAmount(lstSenMeanFromMean);
            ViewBag.LstDislikeAmountFromMean = senMeanBus.getLstDislikeAmount(lstSenMeanFromMean);

            return View(lstSenMeanFromMean);
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
