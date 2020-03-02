using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyFirstStore.ViewModels
{
    public class ProductCard 
    {
        public int Id { get; set; }
        public int CurrentPosition { get; set; }
        public string Name { get; set; }
        public bool Sale { get; set; }
        public decimal Price { get; set; }
        public string HexColor { get; set; }
        public string MainPicturePath { get; set; }
        public string MainAttribute { get; set; }
        public int CountToStore { get; set; }
        public int? ProductTypeId { get; set; }
        public int? ParentTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string ParentTypeName { get; set; }
    }
}
