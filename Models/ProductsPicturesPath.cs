using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.Models
{
    public class ProductsPicturesPath
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int NPicture { get; set; }
        public string PicturePath { get; set; }
    }
}
