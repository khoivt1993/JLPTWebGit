﻿using JLPTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using JLPTWeb.Properties;
using JLPTWeb.DAO;
using JLPTWeb.Helper;
using System.Data.Entity.Validation;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using HiraKana;
using System.Data.Entity.Core;

namespace JLPTWeb.Business
{
    public class Translate_Bus
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();
        SenMeanDao senMeanDao = new SenMeanDao();

        public void getDataFromApi(string strSearch)
        {
            KanaTools kanaTool = new KanaTools();
            ////get data tu DB truong hop khong ton tai get tu mazii
            //var a_Voc_Mean = db.A_Voc_Mean.Include(a => a.A_Mean).Include(a => a.A_User).Include(a => a.A_User1).Include(a => a.A_Vocabulary);
            //a_Voc_Mean = a_Voc_Mean.Where(a => (a.A_Vocabulary.VocContent == strSearch || a.A_Vocabulary.VocHiragana == strSearch) &&
            //                                    a.ApproveFlag == 1 && a.ActiveFlag == 1);
            //if (a_Voc_Mean.ToList().Count > 0)
            //{
            //    return;
            //}
            try
            {
                if (db.A_Voc_Mean.Any(a => (a.A_Vocabulary.VocContent == strSearch || a.A_Vocabulary.VocHiragana == strSearch)))
                {
                    return;
                }
                //get data tu mazii
                String responseString = "";
                bool flag = false;
                var values = new NameValueCollection();
                using (var client = new WebClient())
                {
                    using (DbContextTransaction dbTran = db.Database.BeginTransaction())
                    {
                        try
                        {
                            try
                            {
                                //mazii
                                values["dict"] = "javi";
                                values["type"] = "word";
                                values["query"] = strSearch;
                                values["limit"] = "20";
                                values["page"] = "1";

                                var response = client.UploadValues("https://mazii.net/api/search", values);
                                responseString = Encoding.UTF8.GetString(response);
                            }
                            catch (WebException e)
                            {
                                System.Threading.Thread.Sleep(1000);
                                dbTran.Rollback();
                                getDataFromApi(strSearch);
                            }

                            responseString = System.Net.WebUtility.HtmlDecode(responseString);
                            try
                            {
                                var vocabulary = Vocabulary.FromJson(responseString);

                                //check search co data
                                if (vocabulary.Data != null && vocabulary.Data.Count > 0 && vocabulary.Found == true)
                                {
                                    foreach (Helper.Datum item in vocabulary.Data)
                                    {
                                        // khong get tu lien quan
                                        if (item.Lang[0].ToString().Equals("Vi") && (!"".Equals(item.Word) || !"".Equals(item.Phonetic)) && (strSearch.Equals(item.Phonetic) || strSearch.Equals(item.Word)))
                                        {
                                            //check ton tai trong DB chua
                                            if (!db.A_Vocabulary.Any(s => (!item.Phonetic.Equals("") && s.VocContent == item.Phonetic) || (!item.Word.Equals("") && s.VocContent == item.Word)))
                                            {
                                                A_Vocabulary a_Vocs = new A_Vocabulary();
                                                //mazii get Kanji
                                                try
                                                {
                                                    values = new NameValueCollection();
                                                    values["dict"] = "javi";
                                                    values["page"] = "1";
                                                    values["query"] = item.Word;
                                                    values["type"] = "kanji";

                                                    var response1 = client.UploadValues("https://mazii.net/api/search", values);
                                                    responseString = Encoding.UTF8.GetString(response1);
                                                    responseString = System.Net.WebUtility.HtmlDecode(responseString);
                                                    var kanji = Kanji.FromJson(responseString);

                                                    if (kanji != null && kanji.Results != null && kanji.Results.Count > 0)
                                                    {
                                                        foreach (Helper.ResultKanji itemKanji in kanji.Results)
                                                        {
                                                            try
                                                            {
                                                                //Add kanji vao bang Vocabulary
                                                                if (itemKanji.Mean.Contains(",") == true)
                                                                {
                                                                    a_Vocs.VocKanji = a_Vocs.VocKanji + " " + itemKanji.Mean.Split(',')[0].ToString();
                                                                }
                                                                else
                                                                {
                                                                    a_Vocs.VocKanji = a_Vocs.VocKanji + " " + itemKanji.Mean;
                                                                }

                                                                //Add kanji vao bang Kanji
                                                                A_Kanji a_Kanji = new A_Kanji();
                                                                if (!db.A_Kanji.Any(s => s.KanjiContent == itemKanji.Kanji && s.ActiveFlag == 1))
                                                                {
                                                                    a_Kanji.KanjiContent = itemKanji.Kanji;
                                                                    a_Kanji.KanjiKun = itemKanji.Kun;
                                                                    a_Kanji.KanjiOn = itemKanji.On;
                                                                    a_Kanji.KanjiStrokes = Int32.Parse(itemKanji.StrokeCount.ToString());
                                                                    a_Kanji.KanjiMean = itemKanji.Mean;
                                                                    a_Kanji.KanjiMeanDetail = itemKanji.Detail;
                                                                    if (HttpContext.Current.Session["UserId"] == null)
                                                                    {
                                                                        a_Kanji.UserId = Settings.Default.TrialUserId;
                                                                    }
                                                                    else
                                                                    {
                                                                        a_Kanji.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                                    }
                                                                    a_Kanji.ActiveFlag = 1;
                                                                    a_Kanji.CreateTime = DateTime.Now;
                                                                    a_Kanji.UpdateTime = DateTime.Now;
                                                                    db.A_Kanji.Add(a_Kanji);
                                                                    db.SaveChanges();

                                                                    if (itemKanji.Examples != null && itemKanji.Examples.Count > 0)
                                                                    {
                                                                        foreach (Helper.Example itemExam in itemKanji.Examples)
                                                                        {
                                                                            //Add kanji vao bang KanjiExam
                                                                            A_KanjiExam a_KanjiExam = new A_KanjiExam();
                                                                            if (!db.A_KanjiExam.Any(s => s.KanjiContent == itemExam.W && s.ActiveFlag == 1))
                                                                            {
                                                                                a_KanjiExam.KanjiId = a_Kanji.KanjiId;
                                                                                a_KanjiExam.KanjiContent = itemExam.W;
                                                                                a_KanjiExam.Higarana = itemExam.P;
                                                                                a_KanjiExam.KanjiMean = itemExam.H;
                                                                                a_KanjiExam.KanjiMeanDetail = itemExam.M;
                                                                                if (HttpContext.Current.Session["UserId"] == null)
                                                                                {
                                                                                    a_KanjiExam.UserId = Settings.Default.TrialUserId;
                                                                                }
                                                                                else
                                                                                {
                                                                                    a_KanjiExam.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                                                }
                                                                                a_KanjiExam.ActiveFlag = 1;
                                                                                a_KanjiExam.CreateTime = DateTime.Now;
                                                                                a_KanjiExam.UpdateTime = DateTime.Now;
                                                                                db.A_KanjiExam.Add(a_KanjiExam);
                                                                                db.SaveChanges();
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception e)
                                                            {
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                }
                                                //Khoi-VT xoa get comment 20200203 Start
                                                ////mazii get comment
                                                //var request = (HttpWebRequest)WebRequest.Create("https://api.mazii.net/api/get-mean");
                                                //var postData = "{\"wordId\":" + item.MobileId.ToString() + ",\"type\":\"word\",\"dict\":\"javi\"}";
                                                //var data = Encoding.ASCII.GetBytes(postData);

                                                //request.Method = "POST";
                                                //request.ContentType = "application/json;charset=UTF8";
                                                //request.ContentLength = data.Length;

                                                //using (var stream = request.GetRequestStream())
                                                //{
                                                //    stream.Write(data, 0, data.Length);
                                                //}

                                                //var responseComment = (HttpWebResponse)request.GetResponse();

                                                //var responseStringComment = new StreamReader(responseComment.GetResponseStream()).ReadToEnd();
                                                //string result = System.Text.RegularExpressions.Regex.Unescape(responseStringComment);
                                                //string comment = "";
                                                //if (result.IndexOf("dislike") != -1)
                                                //{
                                                //    result = result.Substring(result.IndexOf("dislike") - 2);
                                                //    result = result.Remove(result.Length - 2);

                                                //    string[] strlist = result.Split(new[] { "{" }, StringSplitOptions.None);
                                                //    foreach (String s in strlist)
                                                //    {
                                                //        if (s != null && s.Equals("") == false && s.Length > 10)
                                                //        {
                                                //            if (s.IndexOf("reportId") == -1 || s.IndexOf("username") == -1 || s.IndexOf("wordId") == -1)
                                                //            {
                                                //                comment = comment + s;
                                                //            }
                                                //            else
                                                //            {
                                                //                comment = comment + s.Substring(0, s.IndexOf("reportId") - 1) + s.Substring(s.IndexOf("username") - 1, s.IndexOf("wordId") - s.IndexOf("username") - 1) + Environment.NewLine;
                                                //            }
                                                //        }
                                                //    }
                                                //}

                                                //a_Vocs.Note = comment;
                                                a_Vocs.Note = "{\"wordId\":" + item.MobileId.ToString();
                                                //Khoi-VT xoa get comment 20200203 End
                                                a_Vocs.VocContent = item.Word;

                                                // kiem tra ka
                                                if (item.Word != null && !item.Word.Equals("") && kanaTool.IsKatakana(item.Word))
                                                {
                                                    a_Vocs.VocHiragana = kanaTool.ToHiragana(item.Word);
                                                    a_Vocs.VocRomaji = kanaTool.ToRomaji(item.Word);
                                                }
                                                else
                                                {
                                                    if (item.Phonetic != null && !item.Phonetic.Equals(""))
                                                    {
                                                        a_Vocs.VocHiragana = item.Phonetic;
                                                        a_Vocs.VocRomaji = kanaTool.ToRomaji(item.Phonetic);
                                                    }
                                                    else
                                                    {
                                                        a_Vocs.VocHiragana = item.Word;
                                                        a_Vocs.VocRomaji = kanaTool.ToRomaji(item.Word);
                                                    }
                                                }
                                                a_Vocs.ApproveUserId = Settings.Default.AdminUserId;
                                                a_Vocs.ApproveFlag = 1;
                                                if (HttpContext.Current.Session["UserId"] == null)
                                                {
                                                    a_Vocs.UserId = Settings.Default.TrialUserId;
                                                }
                                                else
                                                {
                                                    a_Vocs.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                }
                                                a_Vocs.ActiveFlag = 1;
                                                a_Vocs.CreateTime = DateTime.Now;
                                                a_Vocs.UpdateTime = DateTime.Now;
                                                db.A_Vocabulary.Add(a_Vocs);
                                                db.SaveChanges();

                                                foreach (Mean meanItem in item.Means)
                                                {
                                                    //cap nhat kind
                                                    if (meanItem.Kind != null && !meanItem.Kind.Equals(""))
                                                    {
                                                        a_Vocs.VocKind = meanItem.Kind;
                                                        db.Entry(a_Vocs).State = EntityState.Modified;
                                                        db.SaveChanges();
                                                    }

                                                    //cap nhat nghia
                                                    A_Mean a_Means = new A_Mean();
                                                    if (!db.A_Mean.Any(s => s.MeanContent == meanItem.MeanMean))
                                                    {
                                                        a_Means.MeanContent = meanItem.MeanMean;
                                                        String strContentSearch = CommonHelper.convertToUnSign3(meanItem.MeanMean);
                                                        strContentSearch += ",";
                                                        strContentSearch += meanItem.MeanMean;
                                                        a_Means.ContentSearch = strContentSearch;
                                                        if (HttpContext.Current.Session["UserId"] == null)
                                                        {
                                                            a_Means.UserId = Settings.Default.TrialUserId;
                                                        }
                                                        else
                                                        {
                                                            a_Means.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                        }
                                                        a_Means.ActiveFlag = 1;
                                                        a_Means.CreateTime = DateTime.Now;
                                                        a_Means.UpdateTime = DateTime.Now;
                                                        db.A_Mean.Add(a_Means);
                                                        db.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        a_Means = db.A_Mean.Where(s => s.MeanContent == meanItem.MeanMean).FirstOrDefault();
                                                    }

                                                    // map tu va nghia lai voi nhau
                                                    A_Voc_Mean vocMean = new A_Voc_Mean();
                                                    if (!db.A_Voc_Mean.Any(s => s.VocId == a_Vocs.VocId && s.MeanId == a_Means.MeanId && s.ActiveFlag == 1))
                                                    {
                                                        vocMean.VocId = a_Vocs.VocId;
                                                        vocMean.MeanId = a_Means.MeanId;
                                                        vocMean.ApproveUserId = Settings.Default.AdminUserId;
                                                        vocMean.ApproveFlag = 1;
                                                        if (HttpContext.Current.Session["UserId"] == null)
                                                        {
                                                            vocMean.UserId = Settings.Default.TrialUserId;
                                                        }
                                                        else
                                                        {
                                                            vocMean.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                        }
                                                        vocMean.ActiveFlag = 1;
                                                        vocMean.CreateTime = DateTime.Now;
                                                        vocMean.UpdateTime = DateTime.Now;
                                                        db.A_Voc_Mean.Add(vocMean);
                                                        db.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        vocMean = db.A_Voc_Mean.Where(s => s.VocId == a_Vocs.VocId && s.MeanId == a_Means.MeanId).FirstOrDefault();
                                                    }

                                                    //them vidu cua nghia
                                                    if (meanItem.Examples != null && meanItem.Examples.Count > 0)
                                                    {
                                                        foreach (VocExample meanExample in meanItem.Examples)
                                                        {
                                                            // check sentence ton tai trong DB sau do insert
                                                            A_Sentence a_Sen = new A_Sentence();
                                                            string jpSentence = "";
                                                            string vnSentence = "";
                                                            if (CommonHelper.checkUnicode(meanExample.Content))
                                                            {
                                                                jpSentence = meanExample.Content.Trim();
                                                                vnSentence = meanExample.Mean.Trim();
                                                            }
                                                            else
                                                            {
                                                                jpSentence = meanExample.Mean.Trim();
                                                                vnSentence = meanExample.Content.Trim();
                                                            }
                                                            if (!db.A_Sentence.Any(s => s.Sentence == jpSentence))
                                                            {
                                                                a_Sen.Sentence = jpSentence;
                                                                String strContentSearch = jpSentence;
                                                                if (meanExample.Transcription != null && !"".Equals(meanExample.Transcription))
                                                                {
                                                                    strContentSearch += ",";
                                                                    strContentSearch += meanExample.Transcription;
                                                                    strContentSearch += ",";
                                                                    strContentSearch += kanaTool.ToRomaji(meanExample.Transcription);
                                                                }
                                                                a_Sen.SenSearch = strContentSearch;
                                                                if (HttpContext.Current.Session["UserId"] == null)
                                                                {
                                                                    a_Sen.UserId = Settings.Default.TrialUserId;
                                                                }
                                                                else
                                                                {
                                                                    a_Sen.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                                }
                                                                a_Sen.ApproveUserId = Settings.Default.AdminUserId;
                                                                a_Sen.ApproveFlag = 1;
                                                                a_Sen.ActiveFlag = 1;
                                                                a_Sen.CreateTime = DateTime.Now;
                                                                a_Sen.UpdateTime = DateTime.Now;
                                                                db.A_Sentence.Add(a_Sen);
                                                                db.SaveChanges();
                                                            }
                                                            else
                                                            {
                                                                a_Sen = db.A_Sentence.Where(s => s.Sentence == jpSentence).FirstOrDefault();
                                                            }

                                                            //cap nhat nghia
                                                            a_Means = new A_Mean();
                                                            if (!db.A_Mean.Any(s => s.MeanContent == vnSentence))
                                                            {
                                                                a_Means.MeanContent = vnSentence;
                                                                String strContentSearch = CommonHelper.convertToUnSign3(vnSentence);
                                                                strContentSearch += ",";
                                                                strContentSearch += vnSentence;
                                                                a_Means.ContentSearch = strContentSearch;
                                                                if (HttpContext.Current.Session["UserId"] == null)
                                                                {
                                                                    a_Means.UserId = Settings.Default.TrialUserId;
                                                                }
                                                                else
                                                                {
                                                                    a_Means.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                                }
                                                                a_Means.ActiveFlag = 1;
                                                                a_Means.CreateTime = DateTime.Now;
                                                                a_Means.UpdateTime = DateTime.Now;
                                                                db.A_Mean.Add(a_Means);
                                                                db.SaveChanges();
                                                            }
                                                            else
                                                            {
                                                                a_Means = db.A_Mean.Where(s => s.MeanContent == vnSentence).FirstOrDefault();
                                                            }

                                                            // map cau va nghia lai voi nhau
                                                            A_Sen_Mean senMean = new A_Sen_Mean();
                                                            if (!db.A_Sen_Mean.Any(s => s.SentenceId == a_Sen.SentenceId && s.MeanId == a_Means.MeanId))
                                                            {
                                                                senMean.SentenceId = a_Sen.SentenceId;
                                                                senMean.MeanId = a_Means.MeanId;
                                                                senMean.ApproveUserId = Settings.Default.AdminUserId;
                                                                senMean.ApproveFlag = 1;
                                                                if (HttpContext.Current.Session["UserId"] == null)
                                                                {
                                                                    senMean.UserId = Settings.Default.TrialUserId;
                                                                }
                                                                else
                                                                {
                                                                    senMean.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                                }
                                                                senMean.ActiveFlag = 1;
                                                                senMean.CreateTime = DateTime.Now;
                                                                senMean.UpdateTime = DateTime.Now;
                                                                db.A_Sen_Mean.Add(senMean);
                                                                db.SaveChanges();
                                                            }
                                                            else
                                                            {
                                                                senMean = db.A_Sen_Mean.Where(s => s.SentenceId == a_Sen.SentenceId && s.MeanId == a_Means.MeanId).FirstOrDefault();
                                                            }

                                                            //map tu vung vs cau
                                                            if (!db.A_Voc_Sen.Any(s => s.VocMeanId == vocMean.VocMeanId && s.SenMeanId == senMean.SenMeanId))
                                                            {
                                                                A_Voc_Sen vocSen = new A_Voc_Sen();
                                                                vocSen.VocMeanId = vocMean.VocMeanId;
                                                                vocSen.SenMeanId = senMean.SenMeanId;
                                                                if (HttpContext.Current.Session["UserId"] == null)
                                                                {
                                                                    vocSen.UserId = Settings.Default.TrialUserId;
                                                                }
                                                                else
                                                                {
                                                                    vocSen.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                                }
                                                                vocSen.ActiveFlag = 1;
                                                                vocSen.CreateTime = DateTime.Now;
                                                                vocSen.UpdateTime = DateTime.Now;
                                                                db.A_Voc_Sen.Add(vocSen);
                                                                db.SaveChanges();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                flag = true;
                            }

                            if (flag == true)
                            {
                                var vocabulary = Vocabulary2.FromJson(responseString);

                                //check search co data
                                if (vocabulary != null && vocabulary.Data != null && vocabulary.Data.Count > 0)
                                {
                                    foreach (Helper.Datum2 item in vocabulary.Data)
                                    {
                                        // khong get tu lien quan
                                        if (item.Lang[0].ToString().Equals("Vi") && (!"".Equals(item.Word) || !"".Equals(item.Phonetic)) && (strSearch.Equals(item.Phonetic) || strSearch.Equals(item.Word)))
                                        {
                                            //check ton tai trong DB chua
                                            if (!db.A_Vocabulary.Any(s => (!item.Phonetic.Equals("") && s.VocContent == item.Phonetic) || (!item.Word.Equals("") && s.VocContent == item.Word)))
                                            {
                                                A_Vocabulary a_Vocs = new A_Vocabulary();
                                                //mazii get Kanji
                                                try
                                                {
                                                    values = new NameValueCollection();
                                                    values["dict"] = "javi";
                                                    values["page"] = "1";
                                                    values["query"] = item.Word;
                                                    values["type"] = "kanji";

                                                    var response1 = client.UploadValues("https://mazii.net/api/search", values);
                                                    responseString = Encoding.UTF8.GetString(response1);
                                                    responseString = System.Net.WebUtility.HtmlDecode(responseString);
                                                    var kanji = Kanji.FromJson(responseString);

                                                    if (kanji != null && kanji.Results != null && kanji.Results.Count > 0)
                                                    {
                                                        foreach (Helper.ResultKanji itemKanji in kanji.Results)
                                                        {
                                                            try
                                                            {
                                                                //Add kanji vao bang Vocabulary
                                                                if (itemKanji.Mean.Contains(",") == true)
                                                                {
                                                                    a_Vocs.VocKanji = a_Vocs.VocKanji + " " + itemKanji.Mean.Split(',')[0].ToString();
                                                                }
                                                                else
                                                                {
                                                                    a_Vocs.VocKanji = a_Vocs.VocKanji + " " + itemKanji.Mean;
                                                                }

                                                                //Add kanji vao bang Kanji
                                                                A_Kanji a_Kanji = new A_Kanji();
                                                                if (!db.A_Kanji.Any(s => s.KanjiContent == itemKanji.Kanji && s.ActiveFlag == 1))
                                                                {
                                                                    a_Kanji.KanjiContent = itemKanji.Kanji;
                                                                    a_Kanji.KanjiKun = itemKanji.Kun;
                                                                    a_Kanji.KanjiOn = itemKanji.On;
                                                                    a_Kanji.KanjiStrokes = Int32.Parse(itemKanji.StrokeCount.ToString());
                                                                    a_Kanji.KanjiMean = itemKanji.Mean;
                                                                    a_Kanji.KanjiMeanDetail = itemKanji.Detail;
                                                                    if (HttpContext.Current.Session["UserId"] == null)
                                                                    {
                                                                        a_Kanji.UserId = Settings.Default.TrialUserId;
                                                                    }
                                                                    else
                                                                    {
                                                                        a_Kanji.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                                    }
                                                                    a_Kanji.ActiveFlag = 1;
                                                                    a_Kanji.CreateTime = DateTime.Now;
                                                                    a_Kanji.UpdateTime = DateTime.Now;
                                                                    db.A_Kanji.Add(a_Kanji);
                                                                    db.SaveChanges();

                                                                    if (itemKanji.Examples != null && itemKanji.Examples.Count > 0)
                                                                    {
                                                                        foreach (Helper.Example itemExam in itemKanji.Examples)
                                                                        {
                                                                            //Add kanji vao bang KanjiExam
                                                                            A_KanjiExam a_KanjiExam = new A_KanjiExam();
                                                                            if (!db.A_KanjiExam.Any(s => s.KanjiContent == itemExam.W && s.ActiveFlag == 1))
                                                                            {
                                                                                a_KanjiExam.KanjiId = a_Kanji.KanjiId;
                                                                                a_KanjiExam.KanjiContent = itemExam.W;
                                                                                if (itemExam.P.Length > 100)
                                                                                {
                                                                                    a_KanjiExam.Higarana = itemExam.P.Substring(0, 100);
                                                                                }
                                                                                else
                                                                                {
                                                                                    a_KanjiExam.Higarana = itemExam.P;
                                                                                }
                                                                                a_KanjiExam.KanjiMean = itemExam.H;
                                                                                a_KanjiExam.KanjiMeanDetail = itemExam.M;
                                                                                if (HttpContext.Current.Session["UserId"] == null)
                                                                                {
                                                                                    a_KanjiExam.UserId = Settings.Default.TrialUserId;
                                                                                }
                                                                                else
                                                                                {
                                                                                    a_KanjiExam.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                                                }
                                                                                a_KanjiExam.ActiveFlag = 1;
                                                                                a_KanjiExam.CreateTime = DateTime.Now;
                                                                                a_KanjiExam.UpdateTime = DateTime.Now;
                                                                                db.A_KanjiExam.Add(a_KanjiExam);
                                                                                db.SaveChanges();
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception e)
                                                            {
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                }
                                                //Khoi-VT xoa get comment 20200203 Start
                                                ////mazii get comment
                                                //var request = (HttpWebRequest)WebRequest.Create("https://api.mazii.net/api/get-mean");
                                                //var postData = "{\"wordId\":" + item.MobileId.ToString() + ",\"type\":\"word\",\"dict\":\"javi\"}";
                                                //var data = Encoding.ASCII.GetBytes(postData);

                                                //request.Method = "POST";
                                                //request.ContentType = "application/json;charset=UTF8";
                                                //request.ContentLength = data.Length;

                                                //using (var stream = request.GetRequestStream())
                                                //{
                                                //    stream.Write(data, 0, data.Length);
                                                //}

                                                //var responseComment = (HttpWebResponse)request.GetResponse();

                                                //var responseStringComment = new StreamReader(responseComment.GetResponseStream()).ReadToEnd();
                                                //string result = System.Text.RegularExpressions.Regex.Unescape(responseStringComment);
                                                //string comment = "";
                                                //if (result.IndexOf("dislike") != -1)
                                                //{
                                                //    result = result.Substring(result.IndexOf("dislike") - 2);
                                                //    result = result.Remove(result.Length - 2);

                                                //    string[] strlist = result.Split(new[] { "{" }, StringSplitOptions.None);
                                                //    foreach (String s in strlist)
                                                //    {
                                                //        if (s != null && s.Equals("") == false && s.Length > 10)
                                                //        {
                                                //            if (s.IndexOf("reportId") == -1 || s.IndexOf("username") == -1 || s.IndexOf("wordId") == -1)
                                                //            {
                                                //                comment = comment + s;
                                                //            }
                                                //            else
                                                //            {
                                                //                comment = comment + s.Substring(0, s.IndexOf("reportId") - 1) + s.Substring(s.IndexOf("username") - 1, s.IndexOf("wordId") - s.IndexOf("username") - 1) + Environment.NewLine;
                                                //            }
                                                //        }
                                                //    }
                                                //}

                                                //a_Vocs.Note = comment;
                                                a_Vocs.Note = "{\"wordId\":" + item.MobileId.ToString();
                                                //Khoi-VT xoa get comment 20200203 End
                                                a_Vocs.VocContent = item.Word;

                                                // kiem tra ka
                                                if (item.Word != null && !item.Word.Equals("") && kanaTool.IsKatakana(item.Word))
                                                {
                                                    a_Vocs.VocHiragana = kanaTool.ToHiragana(item.Word);
                                                    a_Vocs.VocRomaji = kanaTool.ToRomaji(item.Word);
                                                }
                                                else
                                                {
                                                    if (item.Phonetic != null && !item.Phonetic.Equals(""))
                                                    {
                                                        a_Vocs.VocHiragana = item.Phonetic;
                                                        a_Vocs.VocRomaji = kanaTool.ToRomaji(item.Phonetic);
                                                    }
                                                    else
                                                    {
                                                        a_Vocs.VocHiragana = item.Word;
                                                        a_Vocs.VocRomaji = kanaTool.ToRomaji(item.Word);
                                                    }
                                                }
                                                a_Vocs.ApproveUserId = Settings.Default.AdminUserId;
                                                a_Vocs.ApproveFlag = 1;
                                                if (HttpContext.Current.Session["UserId"] == null)
                                                {
                                                    a_Vocs.UserId = Settings.Default.TrialUserId;
                                                }
                                                else
                                                {
                                                    a_Vocs.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                }
                                                a_Vocs.ActiveFlag = 1;
                                                a_Vocs.CreateTime = DateTime.Now;
                                                a_Vocs.UpdateTime = DateTime.Now;
                                                db.A_Vocabulary.Add(a_Vocs);
                                                db.SaveChanges();
                                                if (item.Means != null)
                                                {
                                                    foreach (Mean2 meanItem in item.Means)
                                                    {
                                                        //cap nhat kind
                                                        if (meanItem.Kind != null && !meanItem.Kind.Equals(""))
                                                        {
                                                            a_Vocs.VocKind = meanItem.Kind;
                                                            db.Entry(a_Vocs).State = EntityState.Modified;
                                                            db.SaveChanges();
                                                        }

                                                        //cap nhat nghia
                                                        A_Mean a_Means = new A_Mean();
                                                        if (!db.A_Mean.Any(s => s.MeanContent == meanItem.MeanMean))
                                                        {
                                                            a_Means.MeanContent = meanItem.MeanMean;
                                                            String strContentSearch = CommonHelper.convertToUnSign3(meanItem.MeanMean);
                                                            strContentSearch += ",";
                                                            strContentSearch += meanItem.MeanMean;
                                                            a_Means.ContentSearch = strContentSearch;
                                                            if (HttpContext.Current.Session["UserId"] == null)
                                                            {
                                                                a_Means.UserId = Settings.Default.TrialUserId;
                                                            }
                                                            else
                                                            {
                                                                a_Means.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                            }
                                                            a_Means.ActiveFlag = 1;
                                                            a_Means.CreateTime = DateTime.Now;
                                                            a_Means.UpdateTime = DateTime.Now;
                                                            db.A_Mean.Add(a_Means);
                                                            db.SaveChanges();
                                                        }
                                                        else
                                                        {
                                                            a_Means = db.A_Mean.Where(s => s.MeanContent == meanItem.MeanMean).FirstOrDefault();
                                                        }

                                                        // map tu va nghia lai voi nhau
                                                        A_Voc_Mean vocMean = new A_Voc_Mean();
                                                        if (!db.A_Voc_Mean.Any(s => s.VocId == a_Vocs.VocId && s.MeanId == a_Means.MeanId && s.ActiveFlag == 1))
                                                        {
                                                            vocMean.VocId = a_Vocs.VocId;
                                                            vocMean.MeanId = a_Means.MeanId;
                                                            vocMean.ApproveUserId = Settings.Default.AdminUserId;
                                                            vocMean.ApproveFlag = 1;
                                                            if (HttpContext.Current.Session["UserId"] == null)
                                                            {
                                                                vocMean.UserId = Settings.Default.TrialUserId;
                                                            }
                                                            else
                                                            {
                                                                vocMean.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                            }
                                                            vocMean.ActiveFlag = 1;
                                                            vocMean.CreateTime = DateTime.Now;
                                                            vocMean.UpdateTime = DateTime.Now;
                                                            db.A_Voc_Mean.Add(vocMean);
                                                            db.SaveChanges();
                                                        }
                                                        else
                                                        {
                                                            vocMean = db.A_Voc_Mean.Where(s => s.VocId == a_Vocs.VocId && s.MeanId == a_Means.MeanId).FirstOrDefault();
                                                        }

                                                        //them vidu cua nghia
                                                        if (meanItem.Examples != null && meanItem.Examples.Count > 0)
                                                        {
                                                            foreach (Example2 meanExample in meanItem.Examples)
                                                            {
                                                                // check sentence ton tai trong DB sau do insert
                                                                A_Sentence a_Sen = new A_Sentence();
                                                                string jpSentence = "";
                                                                string vnSentence = "";
                                                                if (CommonHelper.checkUnicode(meanExample.Content))
                                                                {
                                                                    jpSentence = meanExample.Content.Trim();
                                                                    vnSentence = meanExample.Mean.Trim();
                                                                }
                                                                else
                                                                {
                                                                    jpSentence = meanExample.Mean.Trim();
                                                                    vnSentence = meanExample.Content.Trim();
                                                                }
                                                                if (!db.A_Sentence.Any(s => s.Sentence == jpSentence))
                                                                {
                                                                    a_Sen.Sentence = jpSentence;
                                                                    String strContentSearch = jpSentence;
                                                                    if (meanExample.Transcription != null && !"".Equals(meanExample.Transcription))
                                                                    {
                                                                        strContentSearch += ",";
                                                                        strContentSearch += meanExample.Transcription;
                                                                        strContentSearch += ",";
                                                                        strContentSearch += kanaTool.ToRomaji(meanExample.Transcription);
                                                                    }
                                                                    a_Sen.SenSearch = strContentSearch;
                                                                    if (HttpContext.Current.Session["UserId"] == null)
                                                                    {
                                                                        a_Sen.UserId = Settings.Default.TrialUserId;
                                                                    }
                                                                    else
                                                                    {
                                                                        a_Sen.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                                    }
                                                                    a_Sen.ApproveUserId = Settings.Default.AdminUserId;
                                                                    a_Sen.ApproveFlag = 1;
                                                                    a_Sen.ActiveFlag = 1;
                                                                    a_Sen.CreateTime = DateTime.Now;
                                                                    a_Sen.UpdateTime = DateTime.Now;
                                                                    db.A_Sentence.Add(a_Sen);
                                                                    db.SaveChanges();
                                                                }
                                                                else
                                                                {
                                                                    a_Sen = db.A_Sentence.Where(s => s.Sentence == jpSentence).FirstOrDefault();
                                                                }

                                                                //cap nhat nghia
                                                                a_Means = new A_Mean();
                                                                if (!db.A_Mean.Any(s => s.MeanContent == vnSentence))
                                                                {
                                                                    a_Means.MeanContent = vnSentence;
                                                                    String strContentSearch = CommonHelper.convertToUnSign3(vnSentence);
                                                                    strContentSearch += ",";
                                                                    strContentSearch += vnSentence;
                                                                    a_Means.ContentSearch = strContentSearch;
                                                                    if (HttpContext.Current.Session["UserId"] == null)
                                                                    {
                                                                        a_Means.UserId = Settings.Default.TrialUserId;
                                                                    }
                                                                    else
                                                                    {
                                                                        a_Means.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                                    }
                                                                    a_Means.ActiveFlag = 1;
                                                                    a_Means.CreateTime = DateTime.Now;
                                                                    a_Means.UpdateTime = DateTime.Now;
                                                                    db.A_Mean.Add(a_Means);
                                                                    db.SaveChanges();
                                                                }
                                                                else
                                                                {
                                                                    a_Means = db.A_Mean.Where(s => s.MeanContent == vnSentence).FirstOrDefault();
                                                                }

                                                                // map cau va nghia lai voi nhau
                                                                A_Sen_Mean senMean = new A_Sen_Mean();
                                                                if (!db.A_Sen_Mean.Any(s => s.SentenceId == a_Sen.SentenceId && s.MeanId == a_Means.MeanId))
                                                                {
                                                                    senMean.SentenceId = a_Sen.SentenceId;
                                                                    senMean.MeanId = a_Means.MeanId;
                                                                    senMean.ApproveUserId = Settings.Default.AdminUserId;
                                                                    senMean.ApproveFlag = 1;
                                                                    if (HttpContext.Current.Session["UserId"] == null)
                                                                    {
                                                                        senMean.UserId = Settings.Default.TrialUserId;
                                                                    }
                                                                    else
                                                                    {
                                                                        senMean.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                                    }
                                                                    senMean.ActiveFlag = 1;
                                                                    senMean.CreateTime = DateTime.Now;
                                                                    senMean.UpdateTime = DateTime.Now;
                                                                    db.A_Sen_Mean.Add(senMean);
                                                                    db.SaveChanges();
                                                                }
                                                                else
                                                                {
                                                                    senMean = db.A_Sen_Mean.Where(s => s.SentenceId == a_Sen.SentenceId && s.MeanId == a_Means.MeanId).FirstOrDefault();
                                                                }

                                                                //map tu vung vs cau
                                                                if (!db.A_Voc_Sen.Any(s => s.VocMeanId == vocMean.VocMeanId && s.SenMeanId == senMean.SenMeanId))
                                                                {
                                                                    A_Voc_Sen vocSen = new A_Voc_Sen();
                                                                    vocSen.VocMeanId = vocMean.VocMeanId;
                                                                    vocSen.SenMeanId = senMean.SenMeanId;
                                                                    if (HttpContext.Current.Session["UserId"] == null)
                                                                    {
                                                                        vocSen.UserId = Settings.Default.TrialUserId;
                                                                    }
                                                                    else
                                                                    {
                                                                        vocSen.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                                                                    }
                                                                    vocSen.ActiveFlag = 1;
                                                                    vocSen.CreateTime = DateTime.Now;
                                                                    vocSen.UpdateTime = DateTime.Now;
                                                                    db.A_Voc_Sen.Add(vocSen);
                                                                    db.SaveChanges();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            //start delete sen
                            ////mazii get sentence
                            //values = new NameValueCollection();
                            //values["dict"] = "javi";
                            //values["query"] = strSearch;
                            //values["type"] = "example";

                            //var response3 = client.UploadValues("https://mazii.net/api/search", values);
                            //responseString = Encoding.UTF8.GetString(response3);
                            //responseString = System.Net.WebUtility.HtmlDecode(responseString);
                            //var sentence = Sentence.FromJson(responseString);

                            //if (sentence != null && sentence.Results != null && sentence.Results.Count > 0)
                            //{
                            //    foreach (Helper.Result item in sentence.Results)
                            //    {
                            //        if (item.Content != null && item.Mean != null && !item.Content.Equals("") && !item.Mean.Equals(""))
                            //        {
                            //            // check sentence ton tai trong DB sau do insert
                            //            A_Sentence a_Sen = new A_Sentence();
                            //            string jpSentence = "";
                            //            string vnSentence = "";
                            //            if (CommonHelper.checkUnicode(item.Content.Trim()))
                            //            {
                            //                jpSentence = item.Content.Trim();
                            //                vnSentence = item.Mean.Trim();
                            //            }
                            //            else
                            //            {
                            //                jpSentence = item.Mean.Trim();
                            //                vnSentence = item.Content.Trim();
                            //            }
                            //            if (!db.A_Sentence.Any(s => s.Sentence == jpSentence))
                            //            {
                            //                a_Sen.Sentence = jpSentence;
                            //                String strContentSearch = jpSentence;
                            //                if (item.Transcription != null && !"".Equals(item.Transcription))
                            //                {
                            //                    strContentSearch += ",";
                            //                    strContentSearch += item.Transcription;
                            //                    strContentSearch += ",";
                            //                    strContentSearch += kanaTool.ToRomaji(item.Transcription);
                            //                }
                            //                a_Sen.SenSearch = strContentSearch;
                            //                if (HttpContext.Current.Session["UserId"] == null)
                            //                {
                            //                    a_Sen.UserId = Settings.Default.TrialUserId;
                            //                }
                            //                else
                            //                {
                            //                    a_Sen.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                            //                }
                            //                a_Sen.ApproveUserId = Settings.Default.AdminUserId;
                            //                a_Sen.ApproveFlag = 1;
                            //                a_Sen.ActiveFlag = 1;
                            //                a_Sen.CreateTime = DateTime.Now;
                            //                a_Sen.UpdateTime = DateTime.Now;
                            //                db.A_Sentence.Add(a_Sen);
                            //                db.SaveChanges();
                            //            }
                            //            else
                            //            {
                            //                a_Sen = db.A_Sentence.Where(s => s.Sentence == jpSentence).FirstOrDefault();
                            //            }

                            //            //cap nhat nghia
                            //            A_Mean a_Means = new A_Mean();
                            //            if (!db.A_Mean.Any(s => s.MeanContent == vnSentence))
                            //            {
                            //                a_Means.MeanContent = vnSentence;
                            //                String strContentSearch = CommonHelper.convertToUnSign3(vnSentence);
                            //                strContentSearch += ",";
                            //                strContentSearch += vnSentence;
                            //                a_Means.ContentSearch = strContentSearch;
                            //                if (HttpContext.Current.Session["UserId"] == null)
                            //                {
                            //                    a_Means.UserId = Settings.Default.TrialUserId;
                            //                }
                            //                else
                            //                {
                            //                    a_Means.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                            //                }
                            //                a_Means.ActiveFlag = 1;
                            //                a_Means.CreateTime = DateTime.Now;
                            //                a_Means.UpdateTime = DateTime.Now;
                            //                db.A_Mean.Add(a_Means);
                            //                db.SaveChanges();
                            //            }
                            //            else
                            //            {
                            //                a_Means = db.A_Mean.Where(s => s.MeanContent == vnSentence).FirstOrDefault();
                            //            }

                            //            // map cau va nghia lai voi nhau
                            //            if (!db.A_Sen_Mean.Any(s => s.SentenceId == a_Sen.SentenceId && s.MeanId == a_Means.MeanId))
                            //            {
                            //                A_Sen_Mean senMean = new A_Sen_Mean();
                            //                senMean.SentenceId = a_Sen.SentenceId;
                            //                senMean.MeanId = a_Means.MeanId;
                            //                senMean.ApproveUserId = Settings.Default.AdminUserId;
                            //                senMean.ApproveFlag = 1;
                            //                if (HttpContext.Current.Session["UserId"] == null)
                            //                {
                            //                    senMean.UserId = Settings.Default.TrialUserId;
                            //                }
                            //                else
                            //                {
                            //                    senMean.UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
                            //                }
                            //                senMean.ActiveFlag = 1;
                            //                senMean.CreateTime = DateTime.Now;
                            //                senMean.UpdateTime = DateTime.Now;
                            //                db.A_Sen_Mean.Add(senMean);
                            //                db.SaveChanges();
                            //            }
                            //        }
                            //    }
                            //}
                            //End delete Sen
                            dbTran.Commit();
                        }
                        catch (DbEntityValidationException e)
                        {
                            dbTran.Rollback();
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            throw;
                        }
                    }
                }
            }
            catch (EntityCommandExecutionException e)
            {
                getDataFromApi(strSearch);
            }
        }

        public Boolean checkExist(String text)
        {
            try
            {
                using (var client = new WebClient())
                {
                    //j-dict
                    var values = new NameValueCollection();
                    values["m"] = "dictionary";
                    values["fn"] = "suggest";
                    values["keyword"] = text;
                    values["page"] = "1";

                    var response = client.UploadValues("https://kantan.vn/postrequest.ashx", values);
                    string responseString = Encoding.UTF8.GetString(response);

                    if (responseString.Substring(responseString.IndexOf("Data") + 6, 2).Equals("[]"))
                    {
                        return false;
                    }
                    else
                    {
                        string[] separatingStrings = { "\"Text\":\"" };
                        string[] words = responseString.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 1; i < words.Count(); i++)
                        {
                            if (words[i].Substring(0, 1).Equals("<"))
                            {
                                if (words[i].Substring(words[i].IndexOf(">") + 1, text.Length).Equals(text))
                                {
                                    return true;
                                }
                            }
                            if (words[i].Substring(0, text.Length).Equals(text))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                }
            }
            catch (WebException e)
            {
                System.Threading.Thread.Sleep(1000);
                checkExist(text);
            }
            return true;
        }
        public Boolean checkExistExact(String text)
        {
            try
            {
                using (var client = new WebClient())
                {
                    //j-dict
                    var values = new NameValueCollection();
                    values["m"] = "dictionary";
                    values["fn"] = "suggest";
                    values["keyword"] = text;
                    values["page"] = "1";

                    var response = client.UploadValues("https://kantan.vn/postrequest.ashx", values);
                    string responseString = Encoding.UTF8.GetString(response);

                    if (responseString.Substring(responseString.IndexOf("Data") + 6, 2).Equals("[]"))
                    {
                        return false;
                    }
                    else
                    {
                        string[] separatingStrings = { "\"Text\":\"" };
                        string[] words = responseString.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 1; i < words.Count(); i++)
                        {
                            if (words[i].Substring(0, 1).Equals("<"))
                            {
                                if (words[i].Substring(words[i].IndexOf(">") + 1, words[i].IndexOf("</") - words[i].IndexOf(">") - 1).Equals(text))
                                {
                                    return true;
                                }
                                else if ((words[i].Substring(words[i].IndexOf(">") + 1, words[i].IndexOf("</") - words[i].IndexOf(">") - 1) + "する").Equals(text))
                                {
                                    return true;
                                }
                            }
                            else if (words[i].Substring(0, words[i].IndexOf(" [")).Equals(text))
                            {
                                return true;
                            }
                            else if ((words[i].Substring(0, words[i].IndexOf(" [")) + "する").Equals(text))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                }
            }
            catch (WebException e)
            {
                System.Threading.Thread.Sleep(1000);
                checkExistExact(text);
            }
            return true;
        }
    }
}