using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class OrderPositionsMini
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string MainPicturePath { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductName { get; set; }
        public int Count { get; set; }
        public decimal FixPrice { get; set; }
    }
}
