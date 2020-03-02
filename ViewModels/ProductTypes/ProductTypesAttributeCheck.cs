using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.Models;

namespace MyFirstStore.ViewModels
{
    public class ProductTypesAttributeCheck
    {
        public int? ProdyctTypeId { get; set; }
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public bool Check { get; set; }
        public bool ParentAttribute { get; set; }
    }
}
