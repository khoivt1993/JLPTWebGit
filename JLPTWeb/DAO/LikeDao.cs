using JLPTWeb.Models;
using JLPTWeb.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace JLPTWeb.DAO
{
    public class LikeDao
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        public void Insert(A_Like tblLike)
        {
            if (HttpContext.Current.Session["UserId"] == null)
            {
                tblLike.UserId = Settings.Default.TrialUserId;
            }
            else
            {
                tblLike.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
            }
            tblLike.ActiveFlag = 1;
            tblLike.CreateTime = DateTime.Now;
            tblLike.UpdateTime = DateTime.Now;
            db.A_Like.Add(tblLike);
            db.SaveChanges();
        }

        public void UpdateLike(A_Like tblLike)
        {
            if (tblLike.LikeFlag != 1)
            {
                tblLike.LikeFlag = 1;
            }
            else
            {
                tblLike.LikeFlag = 2;
            }
            tblLike.UpdateTime = DateTime.Now;
            db.Entry(tblLike).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void UpdateDisLike(A_Like tblLike)
        {
            if (tblLike.LikeFlag != 0)
            {
                tblLike.LikeFlag = 0;
            }
            else
            {
                tblLike.LikeFlag = 2;
            }
            tblLike.UpdateTime = DateTime.Now;
            db.Entry(tblLike).State = EntityState.Modified;
            db.SaveChanges();
        }

        public A_Like getLstLike(long senMeanId)
        {
            A_Like sTblLike;
            if (HttpContext.Current.Session["UserId"] == null)
            {
                sTblLike = db.A_Like.Where(s => s.SenMeanId == senMeanId && s.ActiveFlag == 1).FirstOrDefault();
            }
            else
            {
                int userId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                sTblLike = db.A_Like.Where(s => s.SenMeanId == senMeanId && s.UserId == userId && s.ActiveFlag == 1).FirstOrDefault(); 
            }

            if (sTblLike == null)
            {
                return null;
            } else
            {
                return sTblLike;
            }
        }
    }
}