using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeList.Models;

namespace WeList.BizLogic
{
    public static class PostLogic
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static Boolean CheckNotExpire(int postId)
        {
            var post = db.Posts.Find(postId);
            return post.Expiration > post.Timestamp;
        }

        public static Boolean CheckKeywordExist(string keyword,int areaId,int? localeId)
        {
            keyword.ToLower();
            var temp = (from b in db.Posts
                        where (b.Title.ToLower().Contains(keyword) || b.Body.ToLower().Contains(keyword))
                        && b.AreaId == areaId
                        && b.Hidden == false
                        select b);            

            if (localeId != null)
            {
                temp = from b in temp
                       where b.LocaleId == localeId
                       select b;
            }

            if (temp.Count() == 0) return false;
            else return true;

        }

        public static List<Post> Search(string keyword, int areaId, int? localeId, ref Category cate)
        {
            keyword.ToLower();
            var temp = (from b in db.Posts
                        where (b.Title.ToLower().Contains(keyword) || b.Body.ToLower().Contains(keyword))
                        && b.AreaId == areaId
                        && b.Hidden == false
                        select b);
            if (localeId != null)
            {
                temp = from b in temp
                       where b.LocaleId == localeId
                       select b;
            }

            if (temp.Count() == 0) return new List<Post>();

            //find the largest category
            var categoryId = (from b in temp
                            group b by b.CategoryId into g
                            orderby g.Count()
                            select g.FirstOrDefault().CategoryId).FirstOrDefault();

            cate = db.Categories.Find(categoryId);

            //filter category
            var posts = (from b in temp
                         where b.CategoryId == categoryId
                         orderby b.Timestamp descending
                         select b);

            return posts.ToList();
        }

        public static List<Post> Search(string keyword, int areaId, int? localeId, int categoryId,int? subcategoryId)
        {
            keyword.ToLower();
            var temp = (from b in db.Posts
                        where (b.Title.ToLower().Contains(keyword) || b.Body.ToLower().Contains(keyword))
                        && b.AreaId == areaId
                        && b.Hidden == false
                        && b.CategoryId == categoryId
                        orderby b.Timestamp descending
                        select b);
            if (localeId != null)
            {
                temp = (from b in temp
                        where b.LocaleId == localeId
                        orderby b.Timestamp descending
                        select b);
            }
            if (subcategoryId != null)
            {
                temp = (from b in temp
                        where b.SubcategoryId == subcategoryId
                        orderby b.Timestamp descending
                        select b);
            }
            if (temp.Count() == 0) return new List<Post>();
            return temp.ToList();
        }


        public static List<Post> PostFilter(int areaId, int? localeId, int categoryId, int? subcategoryId)
        {

            var posts = from b in db.Posts
                        where b.AreaId == areaId
                        && b.CategoryId == categoryId
                        && b.Hidden == false
                        select b;
            if (localeId != null && subcategoryId != null)
            {
                posts = from c in posts
                        where c.LocaleId == localeId && c.SubcategoryId == subcategoryId
                        select c;
            }
            else if (localeId != null)
            {
                posts = from c in posts
                        where c.LocaleId == localeId
                        select c;
            }

            else if (subcategoryId != null)
            {
                posts = from c in posts
                        where c.SubcategoryId == subcategoryId
                        select c;
            }

            posts = from c in posts orderby c.Timestamp descending select c;
            return posts.ToList();
        }

        public static List<Post> PostFilterPartial(List<Post> l,int start,int range)
        {
            List<Post> posts = new List<Post>();
            for(int i = start; i < start+range && i<l.Count; i++)
            {
                posts.Add(l[i]);
            }
            return posts;
        }

        public static List<Post> AdminPostListAll(string userId)
        {
            if (userId == null)
            {
                var allposts = (from temp in db.Posts
                                where temp.Hidden==false
                                select temp).ToList(); // display all posts includes deleted posts
                return allposts;
            }
            else
            {
                return UserPostList(userId);
            }
        }
        

        public static List<Post> UserPostList(string userId)
        {
            var posts = (from temp in db.Posts
                         where temp.Owner == userId & temp.Hidden == false
                         orderby temp.Timestamp descending
                         select temp);
            var postlist = posts.ToList();
            return postlist;  
        }

        public static void UserCreatePost(CreatePostViewModel c, string userid)
        {

            var newpost = new Post();
            newpost.Title = c.Title;
            newpost.Body = c.Body;
            newpost.LocaleId = c.LocationName;
            newpost.SubcategoryId = c.TypeName;

            var areaId = db.Locales.Find(c.LocationName).AreaId;
            var cateId = db.Subcategories.Find(c.TypeName).CategoryId;
            newpost.AreaId = areaId;
            newpost.CategoryId = cateId;

            newpost.Timestamp = DateTime.Now;
            newpost.Expiration = DateTime.Now.AddDays(10d);
            newpost.Owner = userid;
            newpost.Hidden = false;
            db.Posts.Add(newpost);
            db.SaveChanges();

        }


        public static bool UserPostModify(ModifyPostViewModel post)
        {
            var oldpost = (from temp in db.Posts
                           where temp.Id == post.PostId
                           select temp).SingleOrDefault();
            if (oldpost == null) return false;
            else
            {
                oldpost.Title = post.Title;
                oldpost.Body = post.Body;
                oldpost.AreaId = db.Locales.Find(post.LocationName).AreaId;
                oldpost.LocaleId = post.LocationName;
                oldpost.CategoryId = db.Subcategories.Find(post.TypeName).CategoryId;
                oldpost.SubcategoryId = post.TypeName;
                oldpost.Timestamp = DateTime.Now;
                db.SaveChanges();
                return true;
            }
        }


        public static bool UserPostDelete(int postId)
        {
            var temppost = (from temp in db.Posts
                            where temp.Id == postId
                            select temp).FirstOrDefault();

            if (temppost == null) return false;
            temppost.Hidden = true;
            db.SaveChanges();
            return true;
        }

        public static Post PostFilter(int postId)
        {
            var post = (from a in db.Posts where a.Id == postId && a.Hidden == false select a).FirstOrDefault();
            return post;
        }

        public static List<Post> SearchPost(string keyword)
        {
            var temp = (from f in db.Posts
                         where f.Title.ToLower().Contains(keyword.ToLower())
                         && f.Hidden == false
                         select f).ToList();
            if (temp.Count == 0) return new List<Post>();

            return temp;
        }

    }



}