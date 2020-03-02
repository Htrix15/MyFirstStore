using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.ViewModels.Storekeeper;

namespace MyFirstStore.ViewModels
{
    public class OrderAndOrderPositions
    {
        public UpdateStatus Order { get; set; } 
        public List<OrderPositionsMini> OrderPositionsMinis { get; set; }
    }
}
