using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeList.Models;
using WeList.BizLogic;
using acl = WeList.BizLogic.AccessControlLogic;
using locl = WeList.BizLogic.LocationLogic;
using typel = WeList.BizLogic.TypeLogic;
using postl = WeList.BizLogic.PostLogic;
using Microsoft.AspNet.Identity;

namespace WeList.Controllers
{
    public class IndexController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: List
        // /Index/List/
        // /Index/List/ny
        // /Index/List/ny/brk
        public ActionResult List(string area_sl, string loc_sl)
        {


            if (TempData["Error_Keyword"] != null)
            {
                ModelState.AddModelError("keyword", TempData["Error_Keyword"] as String);
            }

            var homeviewmodel = new HomeViewModel();

            //Location
            Area area = null;
            Locale locale = null;

            if (area_sl == null)
            {
                area = locl.ChooseDefaultArea();   //choose default area               
            }
            else
            {
                area = acl.VerifyAreaSlug(area_sl);
                if (area == null) { return HttpNotFound(); }  //no this area
            }

            if (loc_sl != null)
            {
                locale = acl.VerifyLocaleSlug(area.Id, loc_sl);
                if (locale == null) { return HttpNotFound(); }   //no this locale
                homeviewmodel.locale = locale;
            }

            //get list of locales and Area or Locale
            var locales = (from l in db.Locales where l.AreaId == area.Id && l.Hidden == false orderby l.Name select l);
            var areas = (from a in db.Areas where a.Hidden == false orderby a.Name select a);
            homeviewmodel.areas = areas;
            homeviewmodel.locales = locales;
            homeviewmodel.area = area;


            //get list of categories and subcategories
            var catelist = typel.CategoryList();
            homeviewmodel.categories = catelist;
            homeviewmodel.catecount = catelist.Count;

            return View(homeviewmodel);
        }


        //POST: search       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(HomeViewModel homeviewmodel)
        {
            var keyword = homeviewmodel.keyword;
            var area = homeviewmodel.area;
            var locale = homeviewmodel.locale;
            var localeId = locale == null ? (int?)null : locale.Id;
            var locale_sl = locale == null ? null : locale.Slug;

            if (keyword == null)
            {
                TempData["Error_Keyword"] = "must enter keyword to search";
                return RedirectToAction("List", "Index", new { area_sl = area.Slug, loc_sl = locale_sl });
            }
            else
            {
                //check keyword               
                if (postl.CheckKeywordExist(keyword, area.Id, localeId) == false)
                {
                    TempData["Error_Keyword"] = "no posts related to " + keyword + " at this location";
                    //ModelState.AddModelError("keyword", "no posts related to " + keyword);
                    return RedirectToAction("List", "Index", new { area_sl = area.Slug, loc_sl = locale_sl });
                }
                else return RedirectToAction("List", "Posts", new
                {
                    area_sl = area.Slug,
                    loc_sl = locale_sl,
                    keyword = keyword
                });
            }
        }
    }
}