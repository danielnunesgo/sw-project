using SWSoftware.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace SWSoftware.DAL
{
    public class SalesContext : DbContext
    {
        public SalesContext() : base("SalesContext")
        {

        }

        public IDbSet<Product> Products { get; set; }
        public IDbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}