using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atreemo.Models
{
    public class Issues
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Comment { get; set; }
        public string[] Comments { get; set; }
        public string SLA { get; set; }
        public string Product { get; set; }
        [DisplayName("Due date")]
        public DateTime DueDate { get; set; }
        [DisplayName("Creation date")]
        public DateTime CreatedDate { get; set; }
        public bool Attachment { get; set; }
        public string AttachmentFile { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Client { get; set; }
        public string DcUserName { get; set; }


        public Issues() { }

        public Issues(int ID, string Title, string SLA, string Product, bool Attachment, DateTime DueDate, DateTime CreatedDate, string Priority, string Status, string Client, string DcUserName)
        {
            this.ID = ID;
            this.Title = Title;
            this.SLA = SLA;
            this.Product = Product;
            this.DueDate = DueDate;
            this.CreatedDate = CreatedDate;
            this.Attachment = Attachment;
            this.Priority = Priority;
            this.Status = Status;
            this.Client = Client;
            this.DcUserName = DcUserName;
        }
    }
}