using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeList.Models;

namespace WeList.BizLogic
{

    public static class InboxLogic
    {
        private static ApplicationDbContext db = new ApplicationDbContext();


        public static List<Message> InboxList(string userId)
        {
            var allmessgae = (from m in db.Messages
                              where m.To == userId
                              && m.hidden==false
                              orderby m.TimeStamp descending
                              select m).ToList();
            return allmessgae;
        }

        public static void ResponseDelete(int responseId)
        {
            var response=db.Messages.Find(responseId);
            response.hidden = true;
            db.SaveChanges();
        }
    }
}