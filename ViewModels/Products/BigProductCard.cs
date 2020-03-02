using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.Models;

namespace MyFirstStore.ViewModels
{
    public class BigProductCard:ProductCard
    {
        public int DeliveryToStoreDay { get; set; }
        public string Description { get; set; }
        public string ParentProductTypeHEXColor { get; set; }
        public List<string> PicturesPaths { get; set; }
        public Dictionary<string,string> AttributesToProduct { get; set; }
    }
}
