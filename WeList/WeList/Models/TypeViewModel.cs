using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WeList.Models
{
    // get|post
    public class CreateCategoryViewModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "Category name must be at least 3 characters")]
        public string Name { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Category slug must be 10 characters or less")]
        public string Slug { get; set; }

    }

    //get
    [ReadOnly(true)]
    public class ShowCategoryViewModel
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

    }

    //get|post
    public class ModifyCategoryViewModel
    {
        [Required]
        [ReadOnly(true)]
        public int CategoryId;

        [Required]
        [MinLength(3, ErrorMessage = "category name must be at least 3 characters")]
        public string Name { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "category slug must be 10 characters or less")]
        public string Slug { get; set; }

    }

    //get|post
    public class CreateSubcategoryViewModel
    {
        public List<Category> Categoires { get; set; }

        [Required]
        public int SelectCategory { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Subcategory name must be at least 3 characters")]
        public string Name { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Subcategory slug must be 10 characters or less")]
        public string Slug { get; set; }
    }

    //get
    [ReadOnly(true)]
    public class ShowSubcategoryViewModel
    {
        public int SubcategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }
    }

    //get|post
    public class ModifySubcategoryViewModel
    {
        [Required]
        [ReadOnly(true)]
        public int SubcategoryId { get; set; }


        [Required]
        public string SelectCategory { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Subcategory name must be at least 3 characters")]
        public string Name { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Subcategory slug must be 10 characters or less")]
        public string Slug { get; set; }
    }

}