using Atreemo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Atreemo.DAL
{
    public class ReviewsContext : DbContext
    {
        public ReviewsContext() : base("ConnectionStringName")
        {
        }


        public DbSet<fbuser> Users { get; set; }
        public DbSet<page> Pages { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        
    }
}