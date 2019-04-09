using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WeList.Models;

namespace WeList.BizLogic
{

    public static class AccessControlLogic
    {

        private static ApplicationDbContext db = new ApplicationDbContext();


        public static Boolean CheckAreaLegal(int areaID)
        {
            var area = db.Areas.Find(areaID);
            return area != null && area.Hidden == false;
        }

        public static Boolean CheckLocaleLegal(int localeID)
        {
            var locale = db.Locales.Find(localeID);
            return locale != null && locale.Hidden == false;
        }

        public static Boolean CheckCategoryLegal(int cateID)
        {
            var temp = db.Categories.Find(cateID);
            return temp != null && temp.Hidden == false;
        }

        public static Boolean CheckSubcategoryLegal(int subcID)
        {
            var temp = db.Subcategories.Find(subcID);
            return temp != null && temp.Hidden == false;
        }


        public static Area VerifyAreaSlug(string area_sl)
        {
            var area = (from a in db.Areas where a.Slug == area_sl && a.Hidden==false select a).FirstOrDefault();
            return area;
        }

        public static Locale VerifyLocaleSlug(int areaId,string loc_sl)
        {
            var locale = (from l in db.Locales where l.Slug == loc_sl && l.AreaId==areaId && l.Hidden==false select l).FirstOrDefault();
            return locale;
        }

        public static Category VerifyCategorySlug(string cate_sl)
        {
            var cate = (from l in db.Categories where l.Slug == cate_sl && l.Hidden == false select l).FirstOrDefault();
            return cate;
        }

        public static Subcategory VerifySubcategorySlug(int cateId, string loc_sl)
        {
            var locale = (from l in db.Subcategories where l.Slug == loc_sl && l.CategoryId == cateId && l.Hidden == false select l).FirstOrDefault();
            return locale;
        }

        public static Boolean CheckSearchLegal(string area_sl,string loc_sl,string cate_sl,string subc_sl, ref Area area, ref Locale locale, ref Category category, ref Subcategory subcategory)
        {
            if (area_sl == null || cate_sl==null) return false;
            else
            {
                //area
                area = VerifyAreaSlug(area_sl);
                if (area == null) return false;
                //locale
                if (loc_sl != null)
                {
                    locale = VerifyLocaleSlug(area.Id, loc_sl);
                    if (locale == null) return false;
                }
                //cate
                category = VerifyCategorySlug(cate_sl);
                if (category == null) return false;
                //subcate
                if (subc_sl != null)
                {
                    subcategory = VerifySubcategorySlug(category.Id, subc_sl);
                    if (subcategory == null) return false;
                }

                return true;
            }

        }

        public static Boolean CheckSearchLegal(string area_sl, string loc_sl, ref Area area, ref Locale locale)
        {
            if (area_sl == null) return false;
            else
            {
                area = VerifyAreaSlug(area_sl);
                if (area == null) return false;
                if (loc_sl != null)
                {
                    locale = VerifyLocaleSlug(area.Id, loc_sl);
                    if (locale == null) return false;
                }

                return true;
            }

        }

        public static Boolean AuthorizeAdmin(string sysuserId)
        {
            var user = (from u in db.WeUsers where u.SystemUserId == sysuserId select u).FirstOrDefault();           
            return user.Role.Equals("Admin");
        }

        public static Boolean CheckResponseLegal(string userId, int responseId)
        {
            var response = db.Messages.Find(responseId);
            if (response.To != userId) return false;
            else return true;
        }


        public static Boolean CheckPostLegal(string userId, int postId)
        {
            var post = db.Posts.Find(postId);
            return post!=null && post.Owner == userId;
        }
    }



}