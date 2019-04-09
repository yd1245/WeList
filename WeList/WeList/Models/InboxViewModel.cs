using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WeList.Models
{
    public class CreateResponseViewModel
    {

        public string PostTitle { get; set; }

        public string FromEmail { get; set; }  //email

        [Required]
        public string From { get; set; }    //userId

        public string ToEmail { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        [MaxLength(300, ErrorMessage = "Message must be 300 characters or less")]
        [MinLength(3, ErrorMessage = "Message must be at least 3 characters")]
        public string Body { get; set; }

        public string ReturnUrl { get; set; }
    }

   
    public class ShowResponseViewModel
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public string PostTitle { get; set; }

        public string FromEmail { get; set; }  //email

        public string ToEmail { get; set; }

        public DateTime ResponseTime { get; set; }

        public string Body { get; set; }
    }

    public class BriefResponseViewModel
    {
        public int Id { get; set; }

        public string PostTitle { get; set; }
  
        public string Sender { get; set; }

        public DateTime Timestamp { get; set; }
    }



}