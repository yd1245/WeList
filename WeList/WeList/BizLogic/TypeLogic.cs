using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeList.Models;

namespace WeList.BizLogic
{
    public static class TypeLogic
    {
        private static  ApplicationDbContext db = new ApplicationDbContext();

        //List all categories and subcategories in HOME screen
        public static List<Tuple<Category, List<Subcategory>>> CategoryList()
        {
            //not hidden
            //in alpha order
            List<Tuple<Category, List<Subcategory>>> list = new List<Tuple<Category, List<Subcategory>>>();
            var cates = (from b in db.Categories
                         where b.Hidden == false
                         orderby b.Name ascending
                         select b).ToList();
            foreach (Category item in cates)
            {
                var subcates = (from b in db.Subcategories
                                where b.CategoryId == item.Id
                                && b.Hidden == false
                                orderby b.Name ascending
                                select b);
                if (subcates == null) list.Add(new Tuple<Category, List<Subcategory>>(item, new List<Subcategory>()));
                else list.Add(new Tuple<Category, List<Subcategory>>(item, subcates.ToList()));
            }

            return list;
        }

        public static List<Category> Category_List()
        {
            var categories = (from l in db.Categories where l.Hidden == false orderby l.Name select l);
            return categories.ToList();
        }

        public static List<Subcategory> Subcategory_List()
        {
            var subcategories = (from l in db.Subcategories where l.Hidden == false orderby l.CategoryId select l);
            return subcategories.ToList();
        }

        public static List<Subcategory> Subcategory_List(int categoryId)
        {
            var subcategories = (from l in db.Subcategories where l.CategoryId == categoryId && l.Hidden == false orderby l.Name select l);
            return subcategories.ToList();
        }

        public static List<Subcategory> AdminSubcategoryList(int categoryId)
        {
            var subcates = (from b in db.Subcategories
                            where b.CategoryId == categoryId
                            && b.Hidden == false
                            select b).ToList();
            return (subcates);
        }
        public static Boolean AdminCategoryCreate(CreateCategoryViewModel cate)
        {

            var category = (from c in db.Categories
                            where ((c.Name).ToLower() == (cate.Name).ToLower()
                            || (c.Slug).ToLower() == (cate.Slug).ToLower())
                            && c.Hidden == false
                            select c).SingleOrDefault();
            if (category != null) return false;


            Category tocreate = new Category
            {
                Name = cate.Name,
                Slug = cate.Slug
            };
            db.Categories.Add(tocreate);
            db.SaveChanges();
            return true;

        }

        public static Boolean AdminCategoryModify(Category cate)
        {
            //check unique, ignoring case
            var category = db.Categories.Find(cate.Id);
            var exists = (from c in db.Categories
                          where ((c.Name).ToLower() == (cate.Name).ToLower() || c.Slug.ToLower() == cate.Slug.ToLower())
                          && c.Hidden == false && c.Id != cate.Id
                          select c).FirstOrDefault();
            if (exists != null) return false; // the name or slug alredy exists
            if (category != null)
            {
                category.Name = cate.Name;
                category.Slug = cate.Slug;

                db.SaveChanges();
                return true;
            }
            else return false;
        }


        public static void AdminCategoryHide(int categoryId)
        {
            var subcategory = (from b in db.Subcategories where b.CategoryId == categoryId select b);
            foreach (var item in subcategory)
            {
                item.Hidden = true;
            }
            var posts = from temp in db.Posts
                        where temp.CategoryId == categoryId
                        select temp;
            foreach (var item in posts)
            {
                item.Hidden = true;
            }
            Category category = db.Categories.Find(categoryId);
            category.Hidden = true;
            db.SaveChanges();
        }

        //=============SubCategory============
        public static bool AdminSubcategoryHide(int subcategoryId)
        {
            Subcategory subcategory = db.Subcategories.Find(subcategoryId);
            if (subcategory == null) return false;
            else
            {
                subcategory.Hidden = true;
                var posts = from temp in db.Posts
                            where temp.SubcategoryId == subcategoryId
                            select temp;
                foreach (var item in posts)
                {
                    item.Hidden = true;
                }
                db.SaveChanges();
                return true;
            }
        }

        public static Boolean AdminSubcategoryCreate(Subcategory subc, int categoryId)
        {
            //check unique
            subc.CategoryId = categoryId;
            var subcategory = (from c in db.Subcategories
                               where ((c.Name).ToLower() == (subc.Name).ToLower()
                               || (c.Slug).ToLower() == (subc.Slug).ToLower())
                               && c.CategoryId == subc.CategoryId
                               select c).SingleOrDefault();
            if (subcategory == null)
            {
                var tocreate = new Subcategory
                {
                    Name = subc.Name,
                    Slug = subc.Slug,
                    CategoryId = categoryId,
                };
                db.Subcategories.Add(tocreate);
                db.SaveChanges();
                return true;
            }
            else return false;
        }

        public static Boolean AdminSubcategoryModify(Subcategory subc, int categoryId)
        {
            var cursubcategory = (from temp in db.Subcategories
                                  where temp.CategoryId == categoryId
                                  select temp).FirstOrDefault();
            if (cursubcategory == null) return false;
            var ttemp = (from f in db.Subcategories
                         where (f.Name.ToLower() == subc.Name.ToLower() ||
                         f.Slug.ToLower() == subc.Slug.ToLower()) &&
                         f.CategoryId == categoryId
                         select f).FirstOrDefault();
            if (ttemp != null) return false;
            else
            {
                cursubcategory.Slug = subc.Slug;
                cursubcategory.CategoryId = subc.CategoryId;
                cursubcategory.Name = subc.Name;
                db.SaveChanges();
                return true;

            }
        }

        public static List<Subcategory> CombineType()
        {
            var subcates = Subcategory_List();
            foreach (var item in subcates)
            {
                var catetemp = db.Categories.Find(item.CategoryId);
                item.Name = catetemp.Name + "-" + item.Name;
            }
            return subcates;
        }

        public static List<Subcategory> SearchSubcategory(string keyword)
        {
            var temp = (from f in db.Subcategories
                        where (f.Name.ToLower().Contains(keyword.ToLower()) ||
                        f.Slug.ToLower().Contains(keyword.ToLower()))
                        && f.Hidden == false
                        select f).ToList();

            if (temp.Count == 0)
                temp = new List<Subcategory>();

            return temp;
        }

        public static List<Category> SearchCategory(string keyword)
        {
            var temp = (from f in db.Categories
                        where (f.Name.ToLower().Contains(keyword.ToLower()) ||
                        f.Slug.ToLower().Contains(keyword.ToLower()))
                        && f.Hidden == false
                        select f).ToList();

            if (temp.Count == 0)
                temp = new List<Category>();

            return temp;
        }

    }

}