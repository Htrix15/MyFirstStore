using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class BigProductCardAndBasket
    {
        public decimal BasketCost { get; set; }
        public BigProductCard BigProductCard { get; set; }
        public List<ProductInBasket> ProductsInBasket { get; set; }
        public void SetBasketsCost()
        {
            decimal cost = 0;
            ProductsInBasket.ForEach(product => cost += product.Price * product.Count);
            BasketCost = cost;
        }
    }
}
