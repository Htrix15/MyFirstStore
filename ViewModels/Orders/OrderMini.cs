using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class OrderMini
    {
        public int Id { get; set; }
        public int CurrentPosition { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set;}
        public DateTime DataReservation { get; set; }
        public decimal TotalFixCost { get; set; }
    }
}
