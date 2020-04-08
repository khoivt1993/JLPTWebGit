using JLPTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using JLPTWeb.DAO;
using JLPTWeb.Helper;
using JLPTWeb.ViewModels;

namespace JLPTWeb.Business
{
    public class Sen_Mean_Bus
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();
        SenMeanDao senMeanDao = new SenMeanDao();

        public List<A_Sen_Mean> getListSentence(string strSearch, int size)
        {
            List<A_Sen_Mean> lstSenMean = new List<A_Sen_Mean>();
            //var tblSentences = db.A_Sen_Mean.Include(t => t.A_Sentence).Include(t => t.A_Mean).Include(t => t.A_User).Include(t => t.A_User1);
            //tblSentences = tblSentences.Where(t => t.A_Sentence != null && t.A_Mean != null && t.ActiveFlag == 1);
            var tblSentences = db.A_Sen_Mean.Where(t => t.A_Sentence != null && t.A_Mean != null && t.ActiveFlag == 1);

            //Thuc thi search theo gia tri tren man hinh
            if (!String.IsNullOrEmpty(strSearch)) { 
                if (CommonHelper.checkUnicode(strSearch))
                {
                    lstSenMean = tblSentences.Where(s => s.A_Sentence.SenSearch.Contains(strSearch) && s.ActiveFlag == 1).Take(size).ToList();
                    lstSenMean = lstSenMean.OrderBy(s => s.A_Sentence.SentenceId).ToList();
                } else
                {
                    lstSenMean = tblSentences.Where(s => s.A_Mean.ContentSearch.Contains(strSearch) && s.ActiveFlag == 1).Take(size).ToList();
                    lstSenMean = lstSenMean.OrderBy(s => s.MeanId).ToList();
                }
            } else
            {
                lstSenMean = tblSentences.Take(size).ToList();
                lstSenMean = lstSenMean.OrderBy(s => s.SentenceId).ToList();
            }

            return lstSenMean;
        }

        public IQueryable<SenViewModel> getListSenViewModel(string strSearch, int size)
        {
            IQueryable<SenViewModel> lstSenMean;
            //Thuc thi search theo gia tri tren man hinh
            if (!String.IsNullOrEmpty(strSearch))
            {
                if (CommonHelper.checkUnicode(strSearch))
                {
                    lstSenMean = (from d in db.A_Sen_Mean
                                  where d.A_Sentence.SenSearch.Contains(strSearch)
                                  select new SenViewModel()
                                  {
                                      SentenceId = d.A_Sentence.SentenceId,
                                      Sentence = d.A_Sentence.Sentence,
                                      SenFormat = d.A_Sentence.SenFormat,
                                      MeanSenContent = d.A_Mean.MeanContent
                                  });
                    lstSenMean = lstSenMean.Take(size).OrderBy(s => s.SentenceId);
                }
                else
                {
                    lstSenMean = (from d in db.A_Sen_Mean
                                  where d.A_Mean.ContentSearch.Contains(strSearch)
                                  select new SenViewModel()
                                  {
                                      
                                      Sentence = d.A_Sentence.Sentence,
                                      SenFormat = d.A_Sentence.SenFormat,
                                      MeanId = d.A_Mean.MeanId,
                                      MeanSenContent = d.A_Mean.MeanContent
                                  });
                    lstSenMean = lstSenMean.Take(size).OrderBy(s => s.MeanId);
                }
            }
            else
            {
                lstSenMean = (from d in db.A_Sen_Mean
                              select new SenViewModel()
                              {
                                  SentenceId = d.A_Sentence.SentenceId,
                                  Sentence = d.A_Sentence.Sentence,
                                  SenFormat = d.A_Sentence.SenFormat,
                                  MeanSenContent = d.A_Mean.MeanContent
                              });
                lstSenMean = lstSenMean.Take(size).OrderBy(s => s.SentenceId);
            }

            return lstSenMean;
        }

        public List<A_Sen_Mean> getListTransBySenId(long? senMeanId, int intCountry)
        {
            List<A_Sen_Mean> lstSenMean = new List<A_Sen_Mean>();

            //lay thong tin SentenceId va MeanId tu man hinh
            var tblSenMeanSearch = db.A_Sen_Mean.Find(senMeanId);
            
            var tblSenMean = db.A_Sen_Mean.Include(t => t.A_Sentence).Include(t => t.A_Mean).Include(t => t.A_User).Include(t => t.A_User1);
            tblSenMean = tblSenMean.Where(t => t.A_Sentence != null && t.A_Mean != null);

            //Thuc thi search theo gia tri tren man hinh
            if (intCountry == 1)
            {
                lstSenMean = tblSenMean.Where(s => s.SentenceId == tblSenMeanSearch.SentenceId && s.ActiveFlag == 1).ToList();
                lstSenMean = lstSenMean.OrderBy(s => s.SentenceId).ToList();
            }
            else
            {
                lstSenMean = tblSenMean.Where(s => s.MeanId == tblSenMeanSearch.MeanId && s.ActiveFlag == 1).ToList();
                lstSenMean = lstSenMean.OrderBy(s => s.MeanId).ToList();
            }
            return lstSenMean;
        }

        public List<KeyValuePair<long, int>> getLstLikeAmount(List<A_Sen_Mean> lstSenMean)
        {
            List<KeyValuePair<long, int>> lstLikeAmount = new List<KeyValuePair<long, int>>();
            foreach (var item in lstSenMean)
            {
                var tblLike = from l in db.A_Like
                              where l.A_Sen_Mean.SenMeanId == item.SenMeanId && l.LikeFlag == 1 && l.ActiveFlag == 1
                              select l.LikeId;
                lstLikeAmount.Add(new KeyValuePair<long, int>(item.SenMeanId, tblLike.Count()));
            }
            return lstLikeAmount;
        }

        public List<KeyValuePair<long, int>> getLstDislikeAmount(List<A_Sen_Mean> lstSenMean)
        {
            List<KeyValuePair<long, int>> lstDislikeAmount = new List<KeyValuePair<long, int>>();
            foreach (var item in lstSenMean)
            {
                var tblDisLike = from l in db.A_Like
                                 where l.A_Sen_Mean.SenMeanId == item.SenMeanId && l.LikeFlag == 0 && l.ActiveFlag == 1
                                 select l.LikeId;
                lstDislikeAmount.Add(new KeyValuePair<long, int>(item.SenMeanId, tblDisLike.Count()));
            }
            return lstDislikeAmount;
        }

        public void addSentence(string strSentence)
        {
            if (CommonHelper.checkUnicode(strSentence))
            {
                if (db.A_Sentence.Where(s => s.Sentence == strSentence).ToList().Count == 0)
                {
                    SentenceDao senDao = new SentenceDao();
                    senDao.Insert(new A_Sentence() { Sentence = strSentence, SenSearch = strSentence });
                }
                long senId = db.A_Sentence.Where(s => s.Sentence == strSentence).FirstOrDefault().SentenceId;
                if (db.A_Sen_Mean.Where(s => s.SentenceId == senId).ToList().Count == 0)
                {
                    senMeanDao.Insert(new A_Sen_Mean() { SentenceId = senId });
                }
            }
            else
            {
                if (db.A_Mean.Where(s => s.MeanContent == strSentence).ToList().Count == 0)
                {
                    MeanDao senDao = new MeanDao();
                    String strContentSearch = CommonHelper.convertToUnSign3(strSentence);
                    strContentSearch += ",";
                    strContentSearch += strSentence;
                    senDao.Insert(new A_Mean() { MeanContent = strSentence, ContentSearch = strContentSearch });
                }
                long meanId = db.A_Mean.Where(s => s.MeanContent == strSentence).FirstOrDefault().MeanId;
                if (db.A_Sen_Mean.Where(s => s.MeanId == meanId).ToList().Count == 0)
                {
                    senMeanDao.Insert(new A_Sen_Mean() { MeanId = meanId });
                }
            }
        }

        public void setLike(long senMeanId)
        {
            LikeDao lkDao = new LikeDao();
            A_Like sTblLike = lkDao.getLstLike(senMeanId);
            if (sTblLike == null)
            {
                lkDao.Insert(new A_Like() { SenMeanId = senMeanId, LikeFlag = 1 });
            }
            else
            {
                lkDao.UpdateLike(sTblLike);
            }
        }

        public void setDisLike(long senMeanId)
        {
            LikeDao lkDao = new LikeDao();
            A_Like sTblLike = lkDao.getLstLike(senMeanId);
            if (sTblLike == null)
            {
                lkDao.Insert(new A_Like() { SenMeanId = senMeanId, LikeFlag = 0 });
            }
            else
            {
                lkDao.UpdateDisLike(sTblLike);
            }
        }

        public void addSentenceTrans(A_Sen_Mean tblSenMean, string ctrSentenceTrans, int strCountry)
        {
            SentenceDao senDao = new SentenceDao();
            MeanDao meanDao = new MeanDao();
            if (strCountry == 0)
            {
                if (db.A_Sentence.Where(s => s.Sentence == ctrSentenceTrans && s.ActiveFlag == 1).ToList().Count == 0)
                {
                    senDao.Insert(new A_Sentence() { Sentence = ctrSentenceTrans, SenSearch = ctrSentenceTrans });
                }

                //lay thong tin SentenceId va MeanId tu man hinh
                var tblSenMeanSearch = db.A_Sen_Mean.Find(tblSenMean.SenMeanId);

                //lay thong tin SentenceId vua insert
                long senId = db.A_Sentence.Where(s => s.Sentence == ctrSentenceTrans && s.ActiveFlag == 1).FirstOrDefault().SentenceId;
                if(db.A_Sen_Mean.Where(s => s.SentenceId == senId && s.MeanId == tblSenMeanSearch.MeanId).FirstOrDefault() == null)
                {
                    tblSenMean = senMeanDao.FindById(tblSenMean.SenMeanId);
                    if (db.A_Sen_Mean.Find(tblSenMean.SenMeanId).SentenceId == null)
                    {
                        tblSenMean.SentenceId = senId;
                        senMeanDao.Update(tblSenMean);
                    }
                    else
                    {
                        senMeanDao.Insert(new A_Sen_Mean() { MeanId = tblSenMean.MeanId, SentenceId = senId });
                    }
                }
            } else
            {
                if (db.A_Mean.Where(s => s.MeanContent == ctrSentenceTrans && s.ActiveFlag == 1).ToList().Count == 0)
                {
                    String strContentSearch = CommonHelper.convertToUnSign3(ctrSentenceTrans);
                    strContentSearch += ",";
                    strContentSearch += ctrSentenceTrans;
                    meanDao.Insert(new A_Mean() { ContentSearch = strContentSearch, MeanContent = ctrSentenceTrans });
                }

                //lay thong tin SentenceId va MeanId tu man hinh
                var tblSenMeanSearch = db.A_Sen_Mean.Find(tblSenMean.SenMeanId);

                //lay thong tin SentenceId vua insert
                long meanId = db.A_Mean.Where(s => s.MeanContent == ctrSentenceTrans && s.ActiveFlag == 1).FirstOrDefault().MeanId;
                if (db.A_Sen_Mean.Where(s => s.SentenceId == tblSenMeanSearch.SentenceId && s.MeanId == meanId).FirstOrDefault() == null)
                {
                    tblSenMean = senMeanDao.FindById(tblSenMean.SenMeanId);
                    if (db.A_Sen_Mean.Find(tblSenMean.SenMeanId).MeanId == null)
                    {
                        tblSenMean.MeanId = meanId;
                        senMeanDao.Update(tblSenMean);
                    }
                    else
                    {
                        senMeanDao.Insert(new A_Sen_Mean() { MeanId = meanId, SentenceId = tblSenMean.SentenceId });
                    }
                }
            }

        }

        public A_Sen_Mean nextSentence(A_Sen_Mean tblSenMean, int intCountry)
        {
            //lay thong tin SentenceId va MeanId tu man hinh
            var tblSenMeanSearch = db.A_Sen_Mean.Find(tblSenMean.SenMeanId);

            if(intCountry == 0)
            {
                //cap nhat lai ngay Mean
                MeanDao meanDao = new MeanDao();
                meanDao.UpdateTime(meanDao.FindById(tblSenMeanSearch.MeanId.Value));

                return senMeanDao.getListSortByMeanUpdateTime().FirstOrDefault();
            } else
            {
                //cap nhat lai ngay Sentence
                SentenceDao senDao = new SentenceDao();
                senDao.UpdateTime(senDao.FindById(tblSenMeanSearch.SentenceId.Value));

                return senMeanDao.getListSortBySenUpdateTime().FirstOrDefault();
            }
        }

        public A_Sen_Mean getSenByCountry(int intCountry)
        {
            if(intCountry == 0)
            {
                return senMeanDao.getListSortByMeanUpdateTime().FirstOrDefault();
            } else
            {
                return senMeanDao.getListSortBySenUpdateTime().FirstOrDefault();
            }
        }

        public List<A_Sen_Mean> getListSenMeanByUserId(int? userId, int intCountry)
        {
            List<A_Sen_Mean> lstSenMean = new List<A_Sen_Mean>();

            var tblSenMean = db.A_Sen_Mean.Include(t => t.A_Sentence).Include(t => t.A_Mean).Include(t => t.A_User).Include(t => t.A_User1);
            tblSenMean = tblSenMean.Where(t => t.A_Sentence != null && t.A_Mean != null);

            //Thuc thi search theo gia tri tren man hinh
            if (intCountry == 1)
            {
                lstSenMean = tblSenMean.Where(s => s.A_Sentence.UserId == userId && s.ActiveFlag == 1).ToList();
                lstSenMean = lstSenMean.OrderBy(s => s.SentenceId).ToList();
            }
            else
            {
                lstSenMean = tblSenMean.Where(s => s.A_Mean.UserId == userId && s.ActiveFlag == 1).ToList();
                lstSenMean = lstSenMean.OrderBy(s => s.MeanId).ToList();
            }
            return lstSenMean;
        }
    }
}