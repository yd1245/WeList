using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeList.Models;



namespace WeList.BizLogic
{
    public static class LocationLogic
    {
        private static ApplicationDbContext db = new ApplicationDbContext();




        public static List<Locale> SearchLocale(string keyword)
        {
            var temp = (from f in db.Locales
                        where (f.Name.ToLower().Contains(keyword.ToLower()) ||
                        f.Slug.ToLower().Contains(keyword.ToLower()))
                        && f.Hidden == false
                        select f).ToList();

            if (temp.Count == 0)
                temp = new List<Locale>();

            return temp;
        }



        public static List<Area> SearchArea(string keyword)
        {
            var temp = (from f in db.Areas
                        where (f.Name.ToLower().Contains(keyword.ToLower()) ||
                        f.Slug.ToLower().Contains(keyword.ToLower()))
                        && f.Hidden == false
                        select f).ToList();

            if (temp.Count == 0)
                temp = new List<Area>();

            return temp;
        }

        public static Area ChooseDefaultArea()
        {
            //choose first && not hidden
            var area = db.Areas.FirstOrDefault(temp => temp.Hidden == false);
            return area;
        }

        public static Area FindArea(string area_sl)
        {
            var area = (from a in db.Areas where a.Slug == area_sl && a.Hidden == false select a).FirstOrDefault();
            return area;
        }

        public static Locale FindLocale(string loc_sl)
        {
            var locale = (from a in db.Locales where a.Slug == loc_sl && a.Hidden == false select a).FirstOrDefault();
            return locale;
        }

        public static List<Area> Area_List()
        {
            var areas = (from a in db.Areas where a.Hidden == false orderby a.Name select a);
            return areas.ToList();
        }

        public static List<Locale> Locale_List()
        {
            var locales = (from l in db.Locales where l.Hidden == false orderby l.AreaId select l);
            return locales.ToList();
        }

        public static List<Locale> Locale_List(int areaId)
        {
            var locales = (from l in db.Locales where l.AreaId == areaId && l.Hidden == false orderby l.Name select l);
            return locales.ToList();
        }

        public static List<Locale> AdminLocaleList(int areaId)
        {
            var temp = (from b in db.Locales
                        where b.AreaId == areaId
                        && b.Hidden == false
                        select b).ToList();
            return temp;
        }
        //==============Locale==============
        public static Boolean AdminLocaleCreate(Locale loc, int areaId)
        {
            //check unique
            loc.AreaId = areaId;
            var temp = (from b in db.Locales
                        where b.Name.ToLower() == loc.Name.ToLower()
                        && b.AreaId == loc.AreaId
                        && b.Hidden == false
                        select b).FirstOrDefault();
            if (temp != null) return false;
            var temp1= (from b in db.Locales
                        where b.Slug.ToLower() == loc.Slug.ToLower()
                        && b.AreaId == loc.AreaId
                        && b.Hidden == false
                        select b).FirstOrDefault();
            if (temp1 != null) return false;

            loc.Hidden = false;
            db.Locales.Add(loc);
            db.SaveChanges();
            return true;

        }
        public static void AdminLocaleHide(int localeId)
        {
            var temp = db.Locales.Find(localeId);
            var posts = (from ttemp in db.Posts
                         where ttemp.LocaleId == localeId
                         select ttemp).ToList();
            foreach (var item in posts)
            {
                item.Hidden = true;
            }
            temp.Hidden = true;
            db.SaveChanges();
        }

        public static Boolean AdminLocaleModify(ModifyLocaleViewModel cursub)  //partial, including name,areaid,localeId
        {
            // check name unique under a specific area
            // modify name OR modify parent area OR both
            var curarea = (from f in db.Areas
                           where f.Name.ToLower() == cursub.SelectArea.ToLower()
                           select f).FirstOrDefault();
            var temp = (from b in db.Locales
                        where (b.Name.ToLower() == cursub.Name.ToLower()
                        || b.Slug.ToLower() == cursub.Slug.ToLower())
                        && b.AreaId == curarea.Id   // New york, queens | washington, queens are accepted
                        && b.Id != cursub.LocaleId
                        && b.Hidden == false
                        select b).FirstOrDefault();
            if (temp != null) return false; //name already used
            else
            {
                //db.Entry(loc).State = EntityState.Modified;
                var toedit = db.Locales.Find(cursub.LocaleId);
                toedit.Name = cursub.Name;
                toedit.Slug = cursub.Slug;
                toedit.AreaId = curarea.Id;
                db.SaveChanges();
                return true;
            }
        }
        //===================Area==================
        public static Boolean AdminAreaCreate(Area area)
        {
            //check unique
            var temp = (from b in db.Areas
                        where b.Name.ToLower() == area.Name.ToLower()
                        && b.Hidden == false
                        select b).FirstOrDefault();
            if (temp != null) return false;
            var temp1= (from b in db.Areas
                        where b.Slug.ToLower() == area.Slug.ToLower()
                        && b.Hidden == false
                        select b).FirstOrDefault();
            if (temp1 != null) return false;

            area.Hidden = false;
            db.Areas.Add(area);
            db.SaveChanges();
            return true;

        }
        public static void AdminAreaHide(int areaId)
        {
            //hide all relevant locales
            var locales = (from b in db.Locales where b.AreaId == areaId select b).ToList();
            foreach (var item in locales)
            {
                var posts = (from temp in db.Posts
                             where temp.LocaleId == item.Id
                             select temp).ToList();
                foreach (var elm in posts)
                {
                    elm.Hidden = true;
                }
                item.Hidden = true;
            }
            var area = db.Areas.Find(areaId);
            area.Hidden = true;
            db.SaveChanges();
        }


        public static List<Locale> CombineLocation()
        {
            var locales = Locale_List();
            foreach (var item in locales)
            {
                var areatemp = db.Areas.Find(item.AreaId);
                item.Name = areatemp.Name + "-" + item.Name;
            }
            return locales;
        }


    }




}