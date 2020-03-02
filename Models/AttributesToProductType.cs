using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.Models
{
    public class AttributesToProductType
    {
        public int ProductsTypeId { get; set; }
        public ProductsType ProductsType { get; set; }
        public int ProductsAttributeId { get; set; }
        public ProductsAttribute ProductsAttribute { get; set; }
    }
}
