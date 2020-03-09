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
using System.Threading;
using System.Diagnostics;

namespace JLPTWeb.Controllers
{
    public class A_Voc_Mean_AdminController : Controller
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        // GET: A_Voc_Mean_Admin
        public ActionResult Index()
        {
            var a_Voc_Mean = db.A_Voc_Mean.Include(a => a.A_Mean).Include(a => a.A_User).Include(a => a.A_User1).Include(a => a.A_Vocabulary);
            return View(a_Voc_Mean.ToList());
        }


        // GET: A_Voc_Mean_Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: A_Voc_Mean_Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string ctrText, string ctrStart, string ctrEnd)
        {
            int startpos = 0;
            int endpos = 0;
            if (!ctrStart.Equals(""))
            {
                startpos = Int32.Parse(ctrStart);
            }
            if (!ctrEnd.Equals(""))
            {
                endpos = Int32.Parse(ctrEnd);
            }
            
            //int currpos = 0;
            string text = "";
            bool flg = false;
            Translate_Bus transBus = new Translate_Bus();
            List<A_Alphabet> lstAlphabet = new List<A_Alphabet>();
            lstAlphabet = db.A_Alphabet.ToList();
            if (endpos == 0)
            {
                endpos = lstAlphabet.Count;
            }
            for (int i = startpos; i < endpos; i++)
            {
                text = lstAlphabet[i].Hiragana;
                if (flg == false)
                {
                    if (!(ctrText.Length >= text.Length && text.Equals(ctrText.Substring(0, text.Length))))
                    {
                        continue;
                    }
                }
                if (text.Equals(ctrText))
                {
                    flg = true;
                }
                for (int i1 = 0; i1 < lstAlphabet.Count; i1++)
                {
                    text = lstAlphabet[i].Hiragana + lstAlphabet[i1].Hiragana;
                    if(flg == false) { 
                        if (!(ctrText.Length >= text.Length && text.Equals(ctrText.Substring(0,text.Length))))
                        {
                            continue;
                        }
                    }
                    if (text.Equals(ctrText))
                    {
                        flg = true;
                    }
                    if (flg == true)
                    {
                        if (transBus.checkExist(text) == false)
                        {
                            //currpos = currpos + (lstAlphabet.Count * lstAlphabet.Count * lstAlphabet.Count);
                            continue;
                            //currpos++;
                            //transBus.getDataFromApi(text);
                        }
                        transBus.getDataFromApi(text);
                    }
                    for (int i2 = 0; i2 < lstAlphabet.Count; i2++)
                    {
                        //text = lstAlphabet[i].Hiragana;
                        text = lstAlphabet[i].Hiragana + lstAlphabet[i1].Hiragana + lstAlphabet[i2].Hiragana;
                        if (flg == false)
                        {
                            if (!(ctrText.Length >= text.Length && text.Equals(ctrText.Substring(0, text.Length))))
                            {
                                continue;
                            }
                        }
                        if (text.Equals(ctrText))
                        {
                            flg = true;
                        }
                        if (flg == true)
                        {
                            if (transBus.checkExist(text) == false)
                            {
                                //currpos = currpos + (lstAlphabet.Count * lstAlphabet.Count);
                                continue;
                                //currpos++;
                                //transBus.getDataFromApi(text);
                            }
                            transBus.getDataFromApi(text);
                        }
                        for (int i3 = 0; i3 < lstAlphabet.Count; i3++)
                        {
                            //text = lstAlphabet[i].Hiragana;
                            text = lstAlphabet[i].Hiragana + lstAlphabet[i1].Hiragana + lstAlphabet[i2].Hiragana + lstAlphabet[i3].Hiragana;
                            if (flg == false)
                            {
                                if (!(ctrText.Length >= text.Length && text.Equals(ctrText.Substring(0, text.Length))))
                                {
                                    continue;
                                }
                            }
                            if (text.Equals(ctrText))
                            {
                                flg = true;
                            }
                            if (flg == true)
                            {
                                if (transBus.checkExist(text) == false)
                                {
                                    continue;
                                }
                                transBus.getDataFromApi(text);
                            }
                        }
                    }
                }
            }
            return View();
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
