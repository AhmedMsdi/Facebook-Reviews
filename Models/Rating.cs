using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atreemo.Models
{
    [Table("Ratings")]
    public class Rating
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
            public int Id { get; set; }
        
           
            public DateTime created_time { get; set; }
            public Reviewer reviewer { get; set; }
            public int rating { get; set; }
            public string review_text { get; set; }

       public class Reviewer {
         
            public string id { get; set; }
            public string name { get; set; }
           
        }


    }
}