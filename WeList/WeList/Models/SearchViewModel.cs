using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WeList.Models
{
    public class SearchViewModel
    {

        [Required]
        public List<ListPostViewModel> Posts { get; set; }

        [Required]
        public List<Locale> Locales { get; set; }


        [Required]
        public List<Area> Areas { get; set; }

        [Required]
        public List<Category> Categories { get; set; }

        [Required]
        public List<Subcategory> Subcategories { get; set; }

        [Required]
        public Area Area { get; set; }

        public Locale Locale { get; set; }

        [Required]
        public Category Category { get; set; }

        public Subcategory Subcategory { get; set; }


        public string Keyword { get; set; }

        public Boolean Filter { get; set; }

        public int PageCount { get; set; }

        public int CurrentPage { get; set; }

    }
}