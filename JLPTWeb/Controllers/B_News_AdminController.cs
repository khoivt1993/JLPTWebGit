using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JLPTWeb.Models;
using System.Data.Entity.Validation;
using System.Text;

namespace JLPTWeb.Controllers
{
    public class B_News_AdminController : Controller
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        // GET: B_News_Admin
        public ActionResult Index()
        {
            return View(db.B_News.ToList());
        }

        // GET: B_News_Admin/Details/5
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

        // GET: B_News_Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: B_News_Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NewsId,Title,NewContent,ImagePath,MoviePath,NewIdNHK,NewsDate,NewsWebUrl,ActiveFlag,CreateTime,UpdateTime,DeleteTime,TitleWithRuby,NewContentWithRuby")] B_News b_News)
        {

            var newsDate = (from d in db.B_News select d.NewsDate).Max();
            if (ModelState.IsValid)
            {
                using (var client = new WebClient())
                {
                    var responseString = client.DownloadString("https://www3.nhk.or.jp/news/easy/news-list.json?_=1569328766640");
                    var news = Helper.News.FromJson(responseString);
                    if (news.Count > 0)
                    {
                        foreach (KeyValuePair<string, List<Helper.News>> item in news[0])
                        {
                            foreach (var el in item.Value)
                            {
                                if (el.NewsPrearrangedTime.DateTime > newsDate)
                                {

                                    var responseString1 = client.DownloadString("https://www3.nhk.or.jp/news/easy/" + el.NewsId + "/" + el.NewsId + ".html");
                                    int startIndex = responseString1.IndexOf("<h1 class=\"article-main__title\"");
                                    int endIndex = responseString1.LastIndexOf("class=\"article-main__date\"") - 4;
                                    int length = endIndex - startIndex + 1;
                                    String title = responseString1.Substring(startIndex, length).Trim();
                                    title = title.Replace("  ", "");

                                    startIndex = responseString1.IndexOf("<div class=\"article-main__body article-body\"");
                                    endIndex = responseString1.LastIndexOf("<div class=\"article-main__colors\">") - 1;
                                    length = endIndex - startIndex + 1;
                                    String content = responseString1.Substring(startIndex, length).Trim();
                                    content = content.Replace("  ", "");
                                    try
                                    {
                                        b_News = new B_News();
                                        b_News.Title = el.Title;
                                        b_News.TitleWithRuby = el.TitleWithRuby;
                                        b_News.NewContent = content;
                                        b_News.NewContentWithRuby = content;
                                        b_News.NewIdNHK = el.NewsId;
                                        b_News.NewsDate = el.NewsPrearrangedTime.DateTime;
                                        b_News.ImagePath = el.NewsWebImageUri;
                                        b_News.MoviePath = el.NewsWebMovieUri;
                                        b_News.NewsWebUrl = el.NewsWebUrl.ToString();
                                        b_News.ActiveFlag = 1;
                                        b_News.CreateTime = DateTime.Now;
                                        b_News.UpdateTime = DateTime.Now;
                                        db.B_News.Add(b_News);
                                        db.SaveChanges();
                                    }
                                    catch (DbEntityValidationException ex)
                                    {
                                        var sb = new StringBuilder();

                                        foreach (var failure in ex.EntityValidationErrors)
                                        {
                                            sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                                            foreach (var error in failure.ValidationErrors)
                                            {
                                                sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                                                sb.AppendLine();
                                            }
                                        }

                                        throw new DbEntityValidationException(
                                            "Entity Validation Failed - errors follow:\n" +
                                            sb.ToString(), ex
                                            );
                                    }

                                }
                            }

                        }
                    }
                }
                return RedirectToAction("Create");
            }

            return View(b_News);
        }

        // GET: B_News_Admin/Edit/5
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

        // POST: B_News_Admin/Edit/5
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

        // GET: B_News_Admin/Delete/5
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

        // POST: B_News_Admin/Delete/5
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
