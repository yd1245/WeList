using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using WeList.Models;
using WeList.BizLogic;
using acl = WeList.BizLogic.AccessControlLogic;
using locl = WeList.BizLogic.LocationLogic;
using catl = WeList.BizLogic.TypeLogic;
using amposts = WeList.BizLogic.PostLogic;
using typel = WeList.BizLogic.TypeLogic;using Microsoft.AspNet.Identity;

namespace WeList.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AdminArea()
        {
            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            var areas = from f in db.Areas
                        where f.Hidden == false
                        select f;
            return View(areas.ToList());
        }
        public ActionResult AdminCategory()
        {
            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            var categorys = from f in db.Categories
                            where f.Hidden == false
                            select f;
            return View(categorys.ToList());
        }
        public ActionResult AdminUsers()
        {
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();
            var users = (from f in db.Users
                         select f).ToList();
            List<AdminIUsersViewModel> toshow = new List<AdminIUsersViewModel>();
            foreach (var item in users)
            {
                AdminIUsersViewModel temp = new AdminIUsersViewModel
                {
                    GUID = item.Id,
                    Email = item.UserName,
                };
                toshow.Add(temp);
            }
            foreach (var item in toshow)
            {
                var temp = (from f in db.WeUsers
                            where f.SystemUserId == item.GUID
                            select f).FirstOrDefault();
                item.role = temp.Role;
                item.weusedid = temp.Id;
            }
            return View(toshow);
        }

        public ActionResult EditUserRole(int? userID)
        {
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();
            if (userID == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var user = db.WeUsers.Find(userID);
            return View(user);

        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("EditUserRole")]
        public ActionResult EditUserRole(int userID)
        {
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();
            var user = db.WeUsers.Find(userID);
            user.Role = "Admin";
            db.SaveChanges();
            return RedirectToAction("AdminUsers");
        }




        //===================Area=================
        public ActionResult AreaDetail(int? areaID)
        {
            if (areaID == null) return HttpNotFound();

            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            //check legal
            if(acl.CheckAreaLegal((int)areaID)==false) return HttpNotFound();

            var area = db.Areas.Find((int)areaID);

            ViewData["area"] = area.Name;

            var temp = locl.AdminLocaleList((int)areaID);

            return View(temp);
        }
        public ActionResult AreaEdit(int? areaID)
        {
            if (areaID == null) return HttpNotFound();

            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            //check legal
            if (acl.CheckAreaLegal((int)areaID) == false) return HttpNotFound();

            Area curarea = db.Areas.Find((int)areaID);

            var Viewarea = new ShowAreaViewModel
            {
                AreaId = curarea.Id,
                Name = curarea.Name,
                Slug = curarea.Slug
            };

            return View(Viewarea);
        }

        [HttpPost, ActionName("AreaEdit")]
        [ValidateAntiForgeryToken]    
        public ActionResult AreaEdit(ShowAreaViewModel cur)
        {

            var temp = (from f in db.Areas
                        where (f.Name.ToLower() == cur.Name.ToLower() ||
                        f.Slug.ToLower() == cur.Name.ToLower())
                        && f.Id!=cur.AreaId
                        select f).FirstOrDefault();
            if (temp != null)
            {
                TempData["error"] = "The name or slug already exists.";
                return View(cur);
            }
            if (ModelState.IsValid)
            {
                Area old = (from f in db.Areas
                            where f.Id == cur.AreaId
                            select f).FirstOrDefault();
                old.Name = cur.Name;
                old.Slug = cur.Slug;
                db.SaveChanges();
                return RedirectToAction("AdminArea");
            }
            else
            {
                return View(cur);
            }
        }

        public ActionResult AreaDelete(int? areaID)
        {
            if (areaID == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            //check legal
            if (acl.CheckAreaLegal((int)areaID) == false) return HttpNotFound();

            var cur = db.Areas.Find(areaID);
            return View(cur);
        }

        [HttpPost, ActionName("AreaDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult AreaDelete(int areaID)
        {

            locl.AdminAreaHide(areaID);
            return RedirectToAction("AdminArea");
        }


        public ActionResult AreaCreate()
        {
            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            CreateLocationViewModel temp = new CreateLocationViewModel();
            return View(temp);
        }

        [HttpPost, ActionName("AreaCreate")]
        [ValidateAntiForgeryToken]
        public ActionResult AreaCreate(CreateLocationViewModel cur)
        {
            if (ModelState.IsValid)
            {

                Area create = new Area
                {
                    Name = cur.Name,
                    Slug = cur.Slug,
                    Hidden = false
                };
                if (locl.AdminAreaCreate(create))
                {
                    return RedirectToAction("AdminArea");
                }

                TempData["CreateError"] = "Name or Slug already exists. Invalid Creation.";
                return View(cur);
            }
            return View(cur);
        }
        //=================Locale=======================
        public ActionResult LocaleCreate()
        {
            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            CreateLocaleViewModel temp = new CreateLocaleViewModel();
            var areas = (from f in db.Areas
                         where f.Hidden == false
                         select f).ToList();
            temp.Areas = areas;

            return View(temp);
        }

        [HttpPost, ActionName("LocaleCreate")]
        [ValidateAntiForgeryToken]
        public ActionResult LocaleCreate(CreateLocaleViewModel cur)
        {

            cur.Areas = locl.Area_List();

            if (ModelState.IsValid)
            {
                //Area selected = ;
                Locale tocreate = new Locale
                {
                    Name = cur.Name,
                    AreaId = cur.SelectArea,
                    Slug = cur.Slug,
                    Hidden = false
                };
                if (locl.AdminLocaleCreate(tocreate, cur.SelectArea))
                {
                    return RedirectToAction("AreaDetail", new { areaID = cur.SelectArea });
                }

                TempData["CreateError"] = "The selected area already have the locale name or slug. Creation failed";
                return View(cur);
            }
            return View(cur);
        }

        //GET
        public ActionResult LocaleDelete(int? localeID)
        {
            if (localeID == null) return HttpNotFound();

            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            //check legal
            if (acl.CheckLocaleLegal((int)localeID) == false) return HttpNotFound();

            var cur = db.Locales.Find((int)localeID);
            return View(cur);

        }

        [HttpPost, ActionName("LocaleDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult LocaleDelete(int localeID)
        {
            int areaid = (from f in db.Locales
                          where f.Id == localeID
                          select f).FirstOrDefault().AreaId;

            LocationLogic.AdminLocaleHide(localeID);
            return RedirectToAction("AreaDetail", new { areaID = areaid });
        }

        public ActionResult LocaleEdit(int? localeID)
        {
            if (localeID == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            //check legal
            if (acl.CheckLocaleLegal((int)localeID) == false) return HttpNotFound();

            var areas = (from f in db.Areas
                         where f.Hidden == false
                         select f).ToList();
            ViewData["areas"] = areas;
        
            var curlocale = db.Locales.Find(localeID);
            var curarea = db.Areas.Find(curlocale.AreaId);
            ModifyLocaleViewModel temp = new ModifyLocaleViewModel
            {
                Name = curlocale.Name,
                SelectArea = curarea.Name,
                Slug = curlocale.Slug,
                LocaleId = curlocale.Id
            };
            return View(temp);
        }

        [HttpPost, ActionName("LocaleEdit")]
        [ValidateAntiForgeryToken]
        public ActionResult LocaleEdit(ModifyLocaleViewModel cursub)
        {

            if (ModelState.IsValid)
            {
                if (locl.AdminLocaleModify(cursub))
                {
                    var curarea = (from f in db.Areas
                                   where f.Name == cursub.SelectArea
                                   select f).FirstOrDefault();

                    return RedirectToAction("AreaDetail", new { areaID = curarea.Id });
                }
            }

            TempData["error"] = "Invalid Modification.The name or slug already exists.";

            return View(cursub);

        }
        //=================Category================

        public ActionResult CategoryDetail(int? cateID)
        {
            if (cateID == null) return HttpNotFound();

            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            //check legal
            if (acl.CheckCategoryLegal((int)cateID) == false) return HttpNotFound();

            var cate = (from f in db.Categories
                        where f.Id == (int)cateID
                        select f).FirstOrDefault();
            ViewData["cate"] = cate.Name;
            var temp = catl.AdminSubcategoryList((int)cateID);
            return View(temp);
        }

        [HttpGet]
        public ActionResult CategoryDelete(int? cateID)
        {
            if (cateID == null) return HttpNotFound();

            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            //check legal
            if (acl.CheckCategoryLegal((int)cateID) == false) return HttpNotFound();

            var Cate = (from f in db.Categories
                        where f.Id == cateID && f.Hidden == false
                        select f).FirstOrDefault();
            return View(Cate);
        }

        [HttpPost, ActionName("CategoryDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryDelete(int cateID)
        {
            TypeLogic.AdminCategoryHide(cateID);
            return RedirectToAction("AdminCategory");
        }


        public ActionResult CategoryCreate()
        {
            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            CreateCategoryViewModel temp = new CreateCategoryViewModel();
            return View(temp);
        }

        [HttpPost, ActionName("CategoryCreate")]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryCreate(CreateCategoryViewModel cur)
        {
            if (ModelState.IsValid)
            {
                if (catl.AdminCategoryCreate(cur))
                {
                    return RedirectToAction("AdminCategory");
                }

                TempData["CreateError"] = "Invalid Creation. The category already exists";
                return View(cur);
            }
            return View(cur);
        }

        public ActionResult CategoryEdit(int? cateID)
        {
            if (cateID == null) return HttpNotFound();

            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            //check legal
            if (acl.CheckCategoryLegal((int)cateID) == false) return HttpNotFound();

            var temp = db.Categories.Find((int)cateID);
            var toedit = new ModifyCategoryViewModel
            {
                CategoryId = (int)cateID,
                Name = temp.Name,
                Slug = temp.Slug
            };
            return View(toedit);
        }

        [HttpPost, ActionName("CategoryEdit")]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryEdit(ModifyCategoryViewModel cur)
        {

            var toedit = new Category
            {
                Id = cur.CategoryId,
                Name = cur.Name,
                Slug = cur.Slug,
            };
            if (catl.AdminCategoryModify(toedit))
            {
                return RedirectToAction("AdminCategory");
            }
            TempData["error"] = "Invalid Modification. The name or slug already exists.";
            return View(cur);
        }

        //============Subcategory==============
        public ActionResult SubcategoryDelete(int? subcateID)
        {
            if (subcateID == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            //check legal
            if (acl.CheckSubcategoryLegal((int)subcateID) == false) return HttpNotFound();

            var cursub = (from f in db.Subcategories
                          where f.Id == (int)subcateID
                          select f).FirstOrDefault();
           
            return View(cursub);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubcategoryDelete(int subcateID)
        {
            var cursub = db.Subcategories.Find(subcateID);
            var cate = db.Categories.Find(cursub.CategoryId);
            if (TypeLogic.AdminSubcategoryHide(subcateID))
            {
                return RedirectToAction("CategoryDetail", new { cateID = cate.Id });
            }
            ViewData["error"] = "Invalid Deletation.";
            return View(cursub);
        }

        public ActionResult SubcategoryCreate()
        {
            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            var temp = new CreateSubcategoryViewModel();
            temp.Categoires = typel.Category_List();
            return View(temp);
        }

        [HttpPost, ActionName("SubcategoryCreate")]
        [ValidateAntiForgeryToken]
        public ActionResult SubcategoryCreate(CreateSubcategoryViewModel cur)
        {
            cur.Categoires = typel.Category_List();
            if (ModelState.IsValid)
            {
                var tocreate = new Subcategory
                {
                    Name = cur.Name,
                    CategoryId = cur.SelectCategory,
                    Slug = cur.Slug,
                    Hidden = false
                };

                if (typel.AdminSubcategoryCreate(tocreate, cur.SelectCategory))
                {
                    return RedirectToAction("CategoryDetail", new { cateID = cur.SelectCategory });
                }
                TempData["CreateError"] = "Invalid Create of Subcategory. The category name or slug already exists";
                return View(cur);
            }
            return View(cur);
        }

        public ActionResult SubcategoryEdit(int? subcateID)
        {

            if (subcateID == null) return HttpNotFound();

            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();


            //check legal
            if (acl.CheckSubcategoryLegal((int)subcateID) == false) return HttpNotFound();

            var subcate = db.Subcategories.Find((int)subcateID);
            var parentcate = db.Categories.Find(subcate.CategoryId);
            var subtoedit = new ModifySubcategoryViewModel
            {
                SubcategoryId = subcate.Id,
                SelectCategory = parentcate.Name,
                Name = subcate.Name,
                Slug = subcate.Slug

            };
            ViewData["cates"] = (from f in db.Categories
                                 where f.Hidden == false
                                 select f.Name).ToList();
            return View(subtoedit);

        }

        [HttpPost, ActionName("SubcategoryEdit")]
        [ValidateAntiForgeryToken]
        public ActionResult SubcategoryEdit(ModifySubcategoryViewModel cur)
        {

            if (ModelState.IsValid)
            {
                var parentcate = (from f in db.Categories
                                  where f.Name == cur.SelectCategory
                                  select f).FirstOrDefault();
                var toedit = new Subcategory
                {
                    Id = cur.SubcategoryId,
                    Name = cur.Name,
                    Slug = cur.Slug,
                    CategoryId = parentcate.Id,
                    Hidden = false,
                };
                if (TypeLogic.AdminSubcategoryModify(toedit, parentcate.Id))
                {
                    return RedirectToAction("CategoryDetail", new { cateID = parentcate.Id });
                }
            }
            ViewData["cates"] = (from f in db.Categories
                                 where f.Hidden == false
                                 select f.Name).ToList();
            TempData["error"] = "Invalid Edition of the Subcategory. The name or slug already exists for the same category.";
            return View(cur);
        }

        //==============Posts=============

        public ActionResult AdminPosts()
        {

            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            var userID = (string)null;
            var posts = amposts.AdminPostListAll(userID);
            var mo = new List<ShowPostViewModel>();
            foreach (var post in posts)
            {
                var temp = new ShowPostViewModel()
                {
                    PostId = post.Id,
                    Title = post.Title,
                    Email = db.Users.Find(post.Owner).Email,
                    AreaName = db.Areas.Find(post.AreaId).Name,
                    LocaleName = db.Locales.Find(post.LocaleId).Name,
                    CategoryName = db.Categories.Find(post.CategoryId).Name,
                    SubcategoryName = db.Subcategories.Find(post.SubcategoryId).Name,
                };
                mo.Add(temp);
            }
            
            return View(mo);

        }

        //============search==============
        public ActionResult AdminSearch(string keyword)
        {
            //authorize admin
            if (acl.AuthorizeAdmin(User.Identity.GetUserId()) == false) return HttpNotFound();

            ViewData["keyword"] = keyword;
            AdminViewModel result = new AdminViewModel();

            //subcategory
            var temp = typel.SearchSubcategory(keyword);

            result.Subcategories = new List<Subcategory>(temp);

            //category
            var temp1 = typel.SearchCategory(keyword);

            result.Categories = new List<Category>(temp1);

            //areas
            var temp2 = locl.SearchArea(keyword);

            result.Areas = new List<Area>(temp2);

            //locales
            var temp3 = locl.SearchLocale(keyword);

            result.Locales = new List<Locale>(temp3);

            var temp4 = PostLogic.SearchPost(keyword);
            
            result.Posts = new List<Post>(temp4);

            return View(result);
        }





    }







}