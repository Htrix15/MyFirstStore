using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class ProductTypeMini
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParentProductTypeName { get; set; }
        public int? ParentProductTypeId { get; set; }
        public string HEXColor { get; set; }
    }
}
