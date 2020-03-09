using JLPTWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace JLPTWeb.DAO
{
    public class UserDao
    {
        private JLPTDatabaseEntities db = new JLPTDatabaseEntities();

        public void Insert(A_User tblUser)
        {
            tblUser.ActiveFlag = 1;
            tblUser.CreateTime = DateTime.Now;
            tblUser.UpdateTime = DateTime.Now;
            db.A_User.Add(tblUser);
            db.SaveChanges();
        }

        public void Update(A_User tblUser)
        {
            A_User userEdit = new A_User();
            userEdit = db.A_User.Find(tblUser.UserId);
            userEdit.NickName = tblUser.NickName;
            userEdit.TelNumber = tblUser.TelNumber;
            userEdit.FullName = tblUser.FullName;
            userEdit.LevelId = tblUser.LevelId;
            userEdit.Sex = tblUser.Sex;
            userEdit.AboutMe = tblUser.AboutMe;
            userEdit.UpdateTime = DateTime.Now;
            db.Entry(userEdit).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}