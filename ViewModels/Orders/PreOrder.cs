using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class PreOrder
    {
        public List<ProductInBasket> ProductInBasket { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal BasketCost { get; set; }
        public bool Approve { get; set; }
        public void SetBasketCost()
        {
            decimal cost = 0;
            ProductInBasket.ForEach(item => cost += item.Price * item.Count);
            BasketCost = cost;
        }
    }
}
