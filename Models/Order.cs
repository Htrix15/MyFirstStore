using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime DataReservation { get; set; }
        public decimal TotalFixPrice { get; set; }
        public Order(string UserId)
        {
            this.UserId = UserId;
            this.OrderStatusId = 1;

        }
    }
}
