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
        public double Price { get; set; }
        public int? SaleID { get; set; }

        public virtual ICollection<Sale> ProductSale { get; set; }
    }
}