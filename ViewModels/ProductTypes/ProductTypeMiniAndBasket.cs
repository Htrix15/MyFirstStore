using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class ProductTypeMiniAndBasket: EnumToFilter
    {
        public decimal BasketCost { get; set; }
        public List<ProductInBasket> ProductsInBasket { get; set; }
        public List<ProductTypeMini> ProductTypeMinis { get; set; }
        public ProductTypeMiniAndBasket()
        {
            BasketCost = 0;
            ProductsInBasket = null;
            ProductTypeMinis = null;
            SearchEnumArray = null;
            SortEnumArray = null;
        }
        public void SetBasketsCost()
        {
            decimal cost = 0;
            ProductsInBasket.ForEach(product => cost += product.Price * product.Count);
            BasketCost = cost;
        }
    }
}
