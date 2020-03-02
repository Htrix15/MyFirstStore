using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class ProductAndOrders
    {
        public int Id { get; set; }
        public string ProductTypeName { get; set; }
        public string Name { get; set; }
        public bool AllowedDelete { get; set; }
        public List<int> Orders { get; set; }
    }
}
