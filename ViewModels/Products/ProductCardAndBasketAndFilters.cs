using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class ProductCardAndBasketAndFilters: EnumToFilter, IFiltering
    {
        public decimal BasketCost { get; set; }
        public FilterProductCard Filters { get; set; }
        public List<ProductCard> ProductCards { get; set; }
        public List<ProductInBasket> ProductsInBasket { get; set; }
        public ProductCardAndBasketAndFilters()
        {
            BasketCost = 0;
            Filters = null;
            ProductCards = new List<ProductCard>();
            ProductsInBasket = new List<ProductInBasket>();
        }
        public void SetBasketsCost()
        {
            decimal cost = 0;
            ProductsInBasket.ForEach(product => cost += product.Price * product.Count);
            BasketCost = cost;
        }
        public void Sort()
        {
            string sortType = Filters.SortType;
            if (ProductCards != null &&
                ProductCards.Count > 0 &&
                sortType != null &&
                Enum.IsDefined(typeof(ProductsListSort), sortType))
            {
                ProductsListSort? enumSortType = (ProductsListSort)Enum.Parse(typeof(ProductsListSort), sortType);
                switch (enumSortType)
                {
                    case (ProductsListSort.НазваниеAsc): { ProductCards = ProductCards.OrderBy(s => s.Name).ToList(); break; }
                    case (ProductsListSort.НазваниеDesc): { ProductCards = ProductCards.OrderByDescending(s => s.Name).ToList(); break; }
                    case (ProductsListSort.ЦенаAsc): { ProductCards = ProductCards.OrderBy(s => s.Price).ToList(); break; }
                    case (ProductsListSort.ЦенаDesc): { ProductCards = ProductCards.OrderByDescending(s => s.Price).ToList(); break; }
                    case (ProductsListSort.СкидкаAsc): { ProductCards = ProductCards.OrderBy(s => s.Sale).ToList(); break; }
                    case (ProductsListSort.СкидкаDesc): { ProductCards = ProductCards.OrderByDescending(s => s.Sale).ToList(); break; }
                    default: { ProductCards = ProductCards.OrderBy(s => s.Name).ToList(); break; }
                }
            }
        }
        public void SetSelectList()
        {
            if (ProductCards != null && ProductCards.Count > 0)
            {
                Dictionary<string, string> prodyctTypes = (ProductCards
                    .Select(products => new {id= products.ProductTypeId, name= products.ProductTypeName }).Distinct()
                    .Union((ProductCards
                           .Select(products => new { id = products.ParentTypeId, name = products.ParentTypeName })).Distinct()))
                    .Distinct().ToDictionary(products => products.id.ToString(), products => products.name);
                Filters.SetSelectList(prodyctTypes);
            }
        }
        public void SelectFilter()
        {
            if (Filters.SelectFromSelectList != null)
            {
                int productTypeId = Convert.ToInt32(Filters.SelectFromSelectList);
                if (productTypeId != 0)//0 - all type
                {
                    ProductCards = ProductCards.Where(productCards => productCards.ProductTypeId == productTypeId || productCards.ParentTypeId == productTypeId).ToList();
                }
            }
        }
        public void SetMinMaxPrice()
        {
            if (ProductCards!=null && ProductCards.Count > 0)
            {
                Filters.MinPrice = (int?)ProductCards.Min(productCards => productCards.Price);
                Filters.MaxPrice = (int?)ProductCards.Max(productCards => productCards.Price);
            }
        }
        public void BetweenPrice()
        {
            if (Filters.MinPrice != null && Filters.MaxPrice != null)
            {
                ProductCards = ProductCards.Where(productCards => productCards.Price >= Filters.MinPrice && 
                                                                  productCards.Price <= Filters.MaxPrice).ToList();
            }
        }
        public void OnlySale()
        {
            if (Filters.OnlySale != null)
            {
                ProductCards = ProductCards.Where(productCards => productCards.Sale == true).ToList();
            }
        }
        public void OnlyAvailability()
        {
            if (Filters.OnlyAvailability != null)
            {
                ProductCards = ProductCards.Where(productCards => productCards.CountToStore > 0).ToList();
            }
        }
        public void SetCount()
        {
            Filters.CountPage = ProductCards.Count;
        }
    }
}
