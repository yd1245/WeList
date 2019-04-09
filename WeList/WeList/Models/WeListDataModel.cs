using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WeList.Models
{

    public class WeUser
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string SystemUserId { get; set; }   //system allocated user id

        public string Name { get; set; }  //addtional 

	    public string Role { get; set; }   // Admin| User

    }
    public class Post
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        //expiration > timestamp validation
        public DateTime Expiration { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public int AreaId { get; set; }    //foreign key

        public int LocaleId { get; set; }  //foreign key

        public int CategoryId { get; set; } //foreign key

        public int SubcategoryId { get; set; } //foreign key

        public Boolean Hidden { get; set; } = false;  //initialize 

        [Required]
        public string Owner { get; set; }         //foreign key ->userid 
    }

    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public Boolean Hidden { get; set; } = false;
    }

    public class Subcategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public Boolean Hidden { get; set; } = false;

        [Required]
        public int CategoryId { get; set; }  //foreign key

    }

    public class Area
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public Boolean Hidden { get; set; } = false;

    }

    public class Locale
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public Boolean Hidden { get; set; } = false;

        [Required]
        public int AreaId { get; set; }  //foreign key
    }


    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public string From { get; set; }   //foreign key -> userid

        [Required]
        public string To { get; set; }  //foreign key -> userid

        [Required]
        public int PostId { get; set; }

        [Required]
        public bool hidden { get; set; } = false;

    }
}