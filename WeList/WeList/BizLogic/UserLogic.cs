using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeList.Models;

namespace WeList.BizLogic
{
    public static class UserLogic
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static void CreateUser(string userId)
        {
            var newuser = new WeUser();
            
            var alluser = (from temp in db.WeUsers
                           select temp);
            if (alluser.Count()==0)
            {
                newuser.Role = "Admin";
            }
            else
            {
                newuser.Role = "User";
            }
            newuser.SystemUserId = userId;

            db.WeUsers.Add(newuser);
            db.SaveChanges();           
        }




    }
}