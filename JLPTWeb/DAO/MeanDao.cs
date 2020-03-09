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
    public class MeanDao    
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        public void Insert(A_Mean tblMean)
        {
            if (HttpContext.Current.Session["UserId"] == null)
            {
                tblMean.UserId = Settings.Default.TrialUserId;
            }
            else
            {
                tblMean.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
            }
            tblMean.ActiveFlag = 1;
            tblMean.CreateTime = DateTime.Now;
            tblMean.UpdateTime = DateTime.Now;
            db.A_Mean.Add(tblMean);
            db.SaveChanges();
        }

        public void UpdateTime(A_Mean tblMean)
        {
            tblMean.UpdateTime = DateTime.Now;
            db.Entry(tblMean).State = EntityState.Modified;
            db.SaveChanges();
        }

        public A_Mean FindById(long id)
        {
            return db.A_Mean.Find(id);
        }
    }
}