using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WeList.Models
{
    //get|post
    public class CreateLocationViewModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "Area name must be at least 3 characters")]
        public string Name { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Area slug must be 10 characters or less")]
        public string Slug { get; set; }
    }

    //get|post
    public class ModifyLocationViewModel
    {
        [Required]
        [ReadOnly(true)]
        public int AreaId { get; }

        [Required]
        [MinLength(3, ErrorMessage = "Area name must be at least 3 characters")]
        public string Name { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Area slug must be 10 characters or less")]
        public string Slug { get; set; }
    }

    //get
    //[ReadOnly(true)]
    public class ShowAreaViewModel
    {
        public int AreaId { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Subcategory name must be at least 3 characters")]
        public string Name { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "Subcategory slug must be 10 characters or less")]
        public string Slug { get; set; }

    }


    //get|post
    public class CreateLocaleViewModel
    {
        public List<Area> Areas { get; set; }

        [Required]
        public int SelectArea { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Subcategory name must be at least 3 characters")]
        public string Name { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Subcategory slug must be 10 characters or less")]
        public string Slug { get; set; }
    }

    //get
    [ReadOnly(true)]
    public class ShowLocaleViewModel
    {
        public int LocaleId { get; set; }

        public string AreaName { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }
    }

    //get|post
    public class ModifyLocaleViewModel
    {
        [Required]
        public int LocaleId { get; set; }

        [Required]
        public string SelectArea { get; set; }


        [Required]
        [MinLength(3, ErrorMessage = "Subcategory name must be at least 3 characters")]
        public string Name { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Subcategory slug must be 10 characters or less")]
        public string Slug { get; set; }
    }
}