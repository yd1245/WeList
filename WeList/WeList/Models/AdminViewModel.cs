using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeList.Models
{
    public class AdminViewModel
    {
        public List<Area> Areas { get; set; }

        public List<Locale> Locales { get; set; }
        public List<Category> Categories { get; set; }
        public List<Subcategory> Subcategories { get; set; }
        public List<Post> Posts { get; set; }
    }

    public class AdminIUsersViewModel
    {
        public int weusedid { get; set; }
        public string Email { get; set; }
        public string GUID { get; set; }
        public string role { get; set; }
    }
}