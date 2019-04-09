
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WeList.Models
{
    public class CreatePostViewModel
    {

        [Required]
        [MaxLength(50, ErrorMessage = "title must be 50 or less")]
        [MinLength(4, ErrorMessage = "title must be at least 4 characters")]
        public string Title { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "description must be 500 or less")]
        [MinLength(50, ErrorMessage = "description must be at least 50 characters")]
        public string Body { get; set; }

        [Required]
        public int LocationName { get; set; }

        [Required]
        public int TypeName { get; set; }

    }

    public class ModifyPostViewModel
    {
        [Required]
        public int PostId { get; set; }

        public DateTime Expiration { get; set; }

        public string Email { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "title must be 50 or less")]
        [MinLength(4, ErrorMessage = "title must be at least 4 characters")]
        public string Title { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "description must be 500 or less")]
        [MinLength(50, ErrorMessage = "description must be at least 50 characters")]
        public string Body { get; set; }

        [Required]
        public int LocationName { get; set; }

        [Required]
        public int TypeName { get; set; }

    }

    [ReadOnly(true)]
    public class ShowPostViewModel
    {
        public int PostId { get; set; }

        public string Email { get; set; }  

        public DateTime Timestamp { get; set; }

        public DateTime Expiration { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string AreaName { get; set; }

        public string LocaleName { get; set; }

        public string CategoryName { get; set; }

        public string SubcategoryName { get; set; }

        public string ReturnUrl { get; set; }
    }

    [ReadOnly(true)]
    public class ListPostViewModel
    {
        public int PostId { get; set; }

        public string Title { get; set; }

        public string Email { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd / MM}")]
        public DateTime Timestamp { get; set; }

    }

}