
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atreemo.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        [DisplayName("Name")]
        public string CustomerName { get; set; }

        [Required]
        [DisplayName("Address")]       
        public string CustomerAddress { get; set; }
    }
}