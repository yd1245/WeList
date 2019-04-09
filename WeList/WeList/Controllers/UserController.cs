using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeList.Models;
using WeList.BizLogic;
using acl = WeList.BizLogic.AccessControlLogic;
using locl = WeList.BizLogic.LocationLogic;
using postl = WeList.BizLogic.PostLogic;
using inl = WeList.BizLogic.InboxLogic;
using typel = WeList.BizLogic.TypeLogic;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Data.Entity;

namespace WeList.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET
        public ActionResult Index()
        {

            ViewBag.Admin = acl.AuthorizeAdmin(User.Identity.GetUserId());
            return View();
        }
    
        // GET
        public ActionResult PostList()

        {

            var userId = User.Identity.GetUserId();

            var currentuser = db.Users.SingleOrDefault(u => u.Id == userId);
            if (currentuser == null) return HttpNotFound();

            var posts = postl.UserPostList(userId);
            if (posts.Count == 0) posts = new List<Post>();

            return View(posts);

        }

        // GET: Post/Create
        public ActionResult PostCreate()
        {

            var createpostview = new CreatePostViewModel();

            var locations = locl.CombineLocation();
            ViewData["location"] = new SelectList(locations, "Id", "Name");

            var types = typel.CombineType();
            ViewData["type"] = new SelectList(types, "Id", "Name");

            return View(createpostview);

        }

        // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostCreate(CreatePostViewModel post)
        {

            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                postl.UserCreatePost(post, userId);
                return RedirectToAction("PostList");
            }
            else
            {
                var locations = locl.CombineLocation();
                ViewData["location"] = new SelectList(locations, "Id", "Name");

                var types = typel.CombineType();
                ViewData["type"] = new SelectList(types, "Id", "Name");

                return View(post);
            }
           
        }

        // GET: Post/Details/5
        public ActionResult PostDetails(int? id)
        {
            if (id == null) return HttpNotFound();

            var userId = User.Identity.GetUserId();
            Post post = db.Posts.Find((int)id);
            if (post.Hidden == true) return HttpNotFound();

            if (acl.AuthorizeAdmin(userId) != true)
            {
                if (!acl.CheckPostLegal(userId, (int)id) )
                {
                    return HttpNotFound();
                }
            }

            if (postl.CheckNotExpire((int)id) == false)
            {
                ViewBag.Expire = true;
            }

            var showpostmodel = new ShowPostViewModel();
            showpostmodel.Email = db.Users.Find(post.Owner).Email;
            showpostmodel.PostId = (int)id;
            showpostmodel.Timestamp = post.Timestamp;
            showpostmodel.Expiration = post.Expiration;
            showpostmodel.Title = post.Title;
            showpostmodel.Body = post.Body;
            Area area = db.Areas.Find(post.AreaId);
            Locale locale = db.Locales.Find(post.LocaleId);
            Category cate = db.Categories.Find(post.CategoryId);
            Subcategory sub = db.Subcategories.Find(post.SubcategoryId);
            showpostmodel.AreaName = area.Name;
            showpostmodel.LocaleName = locale.Name;
            showpostmodel.CategoryName = cate.Name;
            showpostmodel.SubcategoryName = sub.Name;

            return View(showpostmodel);
        }

        // GET: Post/Edit/5
        public ActionResult PostEdit(int? id)
        {
            if (id == null) return HttpNotFound();

            var userId = User.Identity.GetUserId();
            Post post = db.Posts.Find(id);
            if (post.Hidden == true) return HttpNotFound();
            if (acl.AuthorizeAdmin(userId) != true)
            {
                if (!acl.CheckPostLegal(userId, (int)id))
                    return HttpNotFound();
            }

            if(!postl.CheckNotExpire((int)id))
            {
                TempData["Expire"] = "The post has expired, cannot edit!";
                return RedirectToAction("PostDetails",new { id = (int)id });
            }
          
            var user = db.Users.Find(userId);

            var locations = locl.CombineLocation();
            ViewData["location"] = new SelectList(locations, "Id", "Name");

            var types = typel.CombineType();
            ViewData["type"] = new SelectList(types, "Id", "Name");


            var modifypostmodel = new ModifyPostViewModel
            {
                PostId = (int)id,
                Email = user.Email,
                Expiration = post.Expiration,
                Title = post.Title,
                Body = post.Body,
                LocationName = post.LocaleId,
                TypeName = post.SubcategoryId
            };

            return View(modifypostmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostEdit([Bind(Exclude ="Expiration,Email")] ModifyPostViewModel post)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();

                if (postl.UserPostModify(post) == false) return HttpNotFound();
                else
                {
                    TempData["Edit_Success"] = "Successfully modify post";
                    return RedirectToAction("PostDetails", new { id = post.PostId });
                }                                

            }
            else
            {
                var locations = locl.CombineLocation();
                ViewData["location"] = new SelectList(locations, "Id", "Name");

                var types = typel.CombineType();
                ViewData["type"] = new SelectList(types, "Id", "Name");

                var temppost = db.Posts.Find(post.PostId);
                post.Expiration = temppost.Expiration;
                post.Email = db.Users.Find(User.Identity.GetUserId()).Email;
                return View(post);
            }
          
        }

        // GET: Post/Delete/5
        public ActionResult PostDelete(int? id)
        {
            if (id == null) return HttpNotFound();

            var userId = User.Identity.GetUserId();
            Post post = db.Posts.Find((int)id);
            if (post.Hidden == true) return HttpNotFound();
            if (acl.AuthorizeAdmin(userId) != true)
            {
                if (!acl.CheckPostLegal(userId, (int)id)) return HttpNotFound();
            }

            if (!postl.CheckNotExpire((int)id))
            {
                TempData["Expire"] = "The post has expired, cannot Delete!";
                return RedirectToAction("PostDetails", new { id = (int)id });
            }


            var showpostmodel = new ShowPostViewModel();
            showpostmodel.Email = db.Users.Find(post.Owner).Email;
            showpostmodel.PostId = (int)id;
            showpostmodel.Timestamp = post.Timestamp;
            showpostmodel.Expiration = post.Expiration;
            showpostmodel.Title = post.Title;
            showpostmodel.Body = post.Body;
            Area area = db.Areas.Find(post.AreaId);
            Locale locale = db.Locales.Find(post.LocaleId);
            Category cate = db.Categories.Find(post.CategoryId);
            Subcategory sub = db.Subcategories.Find(post.SubcategoryId);
            showpostmodel.AreaName = area.Name;
            showpostmodel.LocaleName = locale.Name;
            showpostmodel.CategoryName = cate.Name;
            showpostmodel.SubcategoryName = sub.Name;

            return View(showpostmodel);

        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("PostDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            if (postl.UserPostDelete(id) == false)
            {
                return HttpNotFound();
            }
            TempData["Delete_Success"] = "Successfully deleted post";
            return RedirectToAction("PostList");
        }

        //GET
        public ActionResult Inbox()
        {
            var listresponses = new List<BriefResponseViewModel>();

            var responses = inl.InboxList(User.Identity.GetUserId());
            foreach(var res in responses)
            {
                var temp = new BriefResponseViewModel
                {
                    Id = res.Id,
                    PostTitle = db.Posts.Find(res.PostId).Title,
                    Sender = db.Users.Find(res.From).Email,
                    Timestamp = res.TimeStamp
                };
                listresponses.Add(temp);
            }

            return View(listresponses);
        }

        public ActionResult DeleteResponse(int? id)
        {
            if(id==null) return HttpNotFound();
            if (acl.CheckResponseLegal(User.Identity.GetUserId(), (int)id) == false)
            {
                return HttpNotFound();
            }
            else
            {
                inl.ResponseDelete((int)id);
            }
            return RedirectToAction("Inbox");

        }

        public ActionResult ResponseDetails(int? id)
        {
            if(id==null) return HttpNotFound();
            if (acl.CheckResponseLegal(User.Identity.GetUserId(), (int)id) == false)
            {
                return HttpNotFound();
            }
            else
            {
                var response = db.Messages.Find((int)id);
                var showresponse = new ShowResponseViewModel
                {
                    Id = (int)id,
                    PostId=response.PostId,
                    PostTitle = db.Posts.Find(response.PostId).Title,
                    FromEmail = db.Users.Find(response.From).Email,
                    ToEmail = db.Users.Find(User.Identity.GetUserId()).Email,
                    ResponseTime = response.TimeStamp,
                    Body = response.Body
                };
                return View(showresponse);
            }
        }


    }
}