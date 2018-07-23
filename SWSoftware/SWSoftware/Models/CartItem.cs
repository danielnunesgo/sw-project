using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SWSoftware.Models
{
    public class CartItem
    {
        public int ID { get; set; }
        public int CartId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}