using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atreemo.Models
{
    [Table("Pages")]
    public class page
    {
     
        [Key]
        public string id { get; set; }
        public string access_token { get; set; }
        public string category { get; set; }
        //           public Category_List[] category_list { get; set; }
        public string name { get; set; }
        [DefaultValue("false")]
        public bool IsCheck { get; set; }

        public string[] perms { get; set; }

        public List<Rating> ratinglist { get; set; }




    }
}