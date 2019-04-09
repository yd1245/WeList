using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeList.Models;
using postl = WeList.BizLogic.PostLogic;
using acl = WeList.BizLogic.AccessControlLogic;
using typel = WeList.BizLogic.TypeLogic;
using locl = WeList.BizLogic.LocationLogic;
using System.Net;
using Microsoft.AspNet.Identity;

namespace WeList.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Posts
        public ActionResult List(string area_sl, string loc_sl, string cate_sl, string subc_sl, string keyword,int Page=1)
        {

            var searchviewmodel = new SearchViewModel();
            Area area=null;
            Locale locale=null;
            Category category=null;
            Subcategory subcategory=null;
            List<Post> posts=null;

            if (cate_sl == null)
            {
                if (keyword == null) return HttpNotFound();
                else
                {
                    // locale, area legal
                    var check = acl.CheckSearchLegal(area_sl, loc_sl, ref area, ref locale);
                    if (check == false) return HttpNotFound();
                    else
                    {
                        posts = postl.Search(keyword, area.Id, locale == null ? (int?)null : locale.Id,ref category);
                        if (posts.Count==0)
                        {
                            //no match post
                            TempData["Error_Keyword"] = "no posts related to " + keyword;
                            return RedirectToAction("List", "Index", new { area_sl = area_sl, loc_sl = loc_sl });
                        }
                    }
                }
            }
            else
            {
                var check = acl.CheckSearchLegal(area_sl, loc_sl, cate_sl, subc_sl, ref area, ref locale, ref category, ref subcategory);
                if (check == false) return HttpNotFound();
                else
                {
                    if (keyword == null)
                    {
                        posts = postl.PostFilter(area.Id, locale == null ? (int?)null : locale.Id, category.Id, subcategory == null ? (int?)null : subcategory.Id);
                        if (posts.Count == 0)
                            ModelState.AddModelError("Filter", "No posts");                          
                   }
                    else
                    {
                        posts = postl.Search(keyword, area.Id, locale == null ? (int?)null : locale.Id, category.Id, subcategory == null ? (int?)null : subcategory.Id);
                        if (posts.Count==0) ModelState.AddModelError("Filter", "No posts related to " + keyword);                                                  
                    }
                }

            }

            var tmpposts=postl.PostFilterPartial(posts, (Page - 1)*5,5);
            var partposts = new List<ListPostViewModel>();
            foreach(var p in tmpposts)
            {
                var tmppost = new ListPostViewModel()
                {
                    Title = p.Title,
                    Timestamp = p.Timestamp,
                    PostId = p.Id,
                    Email = db.Users.Find(p.Owner).Email
                };
                partposts.Add(tmppost);
            }

            searchviewmodel.CurrentPage = Page;
            searchviewmodel.PageCount = (posts.Count+4) / 5;   //every page show 5 posts
            searchviewmodel.Area = area;
            searchviewmodel.Locale = locale;
            searchviewmodel.Category = category;
            searchviewmodel.Subcategory = subcategory;
            searchviewmodel.Posts = partposts;
            searchviewmodel.Areas = locl.Area_List();
            searchviewmodel.Locales = locl.Locale_List(area.Id);
            searchviewmodel.Categories = typel.Category_List();
            searchviewmodel.Subcategories = typel.Subcategory_List(category.Id);
            searchviewmodel.Keyword = keyword;
            return View(searchviewmodel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(SearchViewModel s)
        {           
            return RedirectToAction("List", "Posts", new { area_sl = s.Area.Slug, loc_sl =(s.Locale==null?string.Empty:s.Locale.Slug), cate_sl = s.Category.Slug, subc_sl =(s.Subcategory==null?string.Empty:s.Subcategory.Slug),keyword=s.Keyword });
        }

    
        //GET
        public ActionResult Details(int? id, string returnUrl=null)
        {          
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Post temp = db.Posts.Find(id);
            ShowPostViewModel cur = new ShowPostViewModel
            {
                PostId = (int)id,
                Title = temp.Title,
                AreaName = db.Areas.Find(temp.AreaId).Name,
                LocaleName = db.Locales.Find(temp.LocaleId).Name,
                CategoryName = db.Categories.Find(temp.CategoryId).Name,
                SubcategoryName = db.Subcategories.Find(temp.SubcategoryId).Name,
                Body = temp.Body,
                Email = db.Users.Find(temp.Owner).Email,
                Timestamp = temp.Timestamp,
                ReturnUrl= returnUrl==null? Request.UrlReferrer.ToString():returnUrl
            };
            return View(cur);
        }

        //GET
        [Authorize]
        public ActionResult PostResponse(int? id, string returnUrl=null)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var post = db.Posts.Find((int)id);
            var from = User.Identity.GetUserId();
            var to = post.Owner;
            var createresponse = new CreateResponseViewModel
            {
                PostId = (int)id,
                PostTitle = post.Title,
                From = from,
                FromEmail = db.Users.Find(from).Email,
                To = to,
                ToEmail = db.Users.Find(to).Email,
                ReturnUrl=returnUrl
            };

            return View(createresponse);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostResponse(CreateResponseViewModel c)
        {
            if (ModelState.IsValid)
            {
                var temp = new Message
                {
                    Body = c.Body,
                    PostId = c.PostId,
                    From = c.From,
                    To = c.To,
                    TimeStamp = DateTime.Now
                };
                db.Messages.Add(temp);
                db.SaveChanges();

                TempData["ResponseSuccess"] = "Successfully send response";

                return RedirectToAction("Details", "Posts", new { id = c.PostId,returnUrl=c.ReturnUrl });
            }
            else
                return View(c);
        }

        
    }
}