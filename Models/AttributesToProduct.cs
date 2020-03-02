using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.Models
{
    public class AttributesToProduct
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ProductsAttributeId { get; set; }
        public ProductsAttribute ProductsAttribute { get; set; }
        [StringLength(20, ErrorMessage = "До 20 символов")]
        public string Value { get; set; }
    }
}
