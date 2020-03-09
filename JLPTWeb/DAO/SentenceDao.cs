using JLPTWeb.Helper;
using JLPTWeb.Models;
using JLPTWeb.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace JLPTWeb.DAO
{
    public class SentenceDao
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        public void Insert(A_Sentence tblSen)
        {
            if (HttpContext.Current.Session["UserId"] == null)
            {
                tblSen.UserId = Settings.Default.TrialUserId;
            }
            else
            {
                tblSen.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
            }
            if (tblSen.ApproveUserId == Settings.Default.TrialUserId)
            {
                tblSen.ApproveUserId = Settings.Default.AdminUserId;
            }
            tblSen.ActiveFlag = 1;
            tblSen.CreateTime = DateTime.Now;
            tblSen.UpdateTime = DateTime.Now;
            db.A_Sentence.Add(tblSen);
            db.SaveChanges();
        }

        public void UpdateTime(A_Sentence tblSen)
        {
            tblSen.UpdateTime = DateTime.Now;
            db.Entry(tblSen).State = EntityState.Modified;
            db.SaveChanges();
        }

        public A_Sentence FindById(long id)
        {
            return db.A_Sentence.Find(id);
        }
    }
}