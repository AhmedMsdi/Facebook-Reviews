using Atreemo.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atreemo.Models
{
    public class Person
    {
        /// <summary>
        /// Person title
        /// </summary>
        [DisplayName("Title")]
        public string _Title { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        [DisplayName("First name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Landline number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Mobile number
        /// </summary>
        [DisplayName("Mobile")]
        public string MobilPhone { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        [DisplayName("Email")]//Do no remove: it related to Add contact validation for Toshiba(Required field)
        public string Email { get; set; }

        /// <summary>
        /// Date of birth
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Person gender
        /// </summary>
        [DisplayName("Gender")]
        public string Gender { get; set; }

        public Person(string FirstName,string Email)
        {
            this.FirstName = FirstName;
            this.Email = Email;
        }
        public Person() { }
    }
}