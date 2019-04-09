using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WeList.Models
{
    public class HomeViewModel
    {
     
        public Locale locale { get; set; }

        [Required]
        public Area area { get; set; }

        [Required]
        public IOrderedQueryable<Area> areas { get; set; }
        [Required]
        public IOrderedQueryable<Locale> locales { get; set; }

        [Required]
        public List<Tuple<Category,List<Subcategory>>> categories { get; set; }

        [Single]
        public string keyword { get; set; }
        [Required]
        public int catecount { get; set; }

        //string area_sl { get; set; }
        //string loc_sl { get; set; }

        public HomeViewModel()
        {
            locale = null;
            keyword = null;
        }

    }
    public class SingleAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is string && !(value as string).Contains(" ");

        }
    }



}