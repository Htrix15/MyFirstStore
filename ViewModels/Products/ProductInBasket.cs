using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.Models;

namespace MyFirstStore.ViewModels
{
    public class ProductInBasket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string MainPicturePath { get; set; }
        public string HEXcolor { get; set; }
        public int Count { get; set; }
    }
}
