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
    public class SenMeanDao    
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        public void Insert(A_Sen_Mean tblSenMean)
        {
            if (HttpContext.Current.Session["UserId"] == null)
            {
                tblSenMean.UserId = Settings.Default.TrialUserId;
            }
            else
            {
                tblSenMean.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
            }
            tblSenMean.ActiveFlag = 1;
            tblSenMean.CreateTime = DateTime.Now;
            tblSenMean.UpdateTime = DateTime.Now;
            db.A_Sen_Mean.Add(tblSenMean);
            db.SaveChanges();
        }

        public void Update(A_Sen_Mean tblSenMean)
        {
            tblSenMean.UpdateTime = DateTime.Now;
            db.Entry(tblSenMean).State = EntityState.Modified;
            db.SaveChanges();
        }

        public A_Sen_Mean FindById(long id)
        {
            return db.A_Sen_Mean.Find(id);
        }

        public List<A_Sen_Mean> getListSortBySenUpdateTime()
        {
            var lstTrans = db.A_Sen_Mean.Where(s => s.ActiveFlag == 1 && s.ApproveFlag != 1 && s.A_Sentence != null).ToList();
            if(lstTrans != null && lstTrans.Where(t => t.A_Sentence != null).ToList().Count > 0) { 
                lstTrans = lstTrans.OrderBy(s => s.A_Sentence.UpdateTime).ToList();
            }
            return lstTrans;
        }
        public List<A_Sen_Mean> getListSortByMeanUpdateTime()
        {
            var lstTrans = db.A_Sen_Mean.Where(s => s.ActiveFlag == 1 && s.ApproveFlag != 1 && s.A_Mean != null).ToList();
            if (lstTrans != null && lstTrans.Where(t => t.A_Mean != null).ToList().Count > 0)
            {
                lstTrans = lstTrans.OrderBy(s => s.A_Mean.UpdateTime).ToList();
            }
            return lstTrans;
        }
    }
}