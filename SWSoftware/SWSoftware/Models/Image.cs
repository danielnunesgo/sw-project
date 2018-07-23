using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SWSoftware.Models
{
    public class ProductImage
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public int ImageMimeType { get; set; }
        public byte[] ImageData { get; set; }
        public byte[] ImageThumbnail { get; set; }
        public int ProductID { get; set; }

    }
}