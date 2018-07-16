using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atreemo.Models
{
    [Table("Users")]
    public class fbuser
    {
        [Key]
        public string id { get; set; }
        public List<page> pagelist { get; set; }


    }
}