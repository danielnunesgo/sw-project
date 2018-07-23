using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SWSoftware.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public double Price { get; set; }
        public int? SaleID { get; set; }
        public string SaleDescription { get; set; }
        public int? Quantity { get; set; }

        public virtual ProductImage ProductImage { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}