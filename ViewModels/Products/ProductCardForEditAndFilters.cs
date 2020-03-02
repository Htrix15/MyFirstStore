using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class ProductCardForEditAndFilters: EnumToFilter, IFiltering
    {
        public FilterBaseAndSearch Filters { get; set; }
        public List<ProductCardForEdit> ProductCards { get; set; }
        public void Sort()
        {
            string sortType = Filters.SortType;
            if (ProductCards != null &&
                ProductCards.Count > 0 &&
                sortType != null &&
                Enum.IsDefined(typeof(IdNameTypeSort), sortType))
            {
                IdNameTypeSort enumSortType = (IdNameTypeSort)Enum.Parse(typeof(IdNameTypeSort), sortType);
                switch (enumSortType)
                {
                    case (IdNameTypeSort.IdAsc): { ProductCards = ProductCards.OrderBy(s => s.Id).ToList(); break; }
                    case (IdNameTypeSort.IdDesc): { ProductCards = ProductCards.OrderByDescending(s => s.Id).ToList(); break; }
                    case (IdNameTypeSort.НазваниеAsc): { ProductCards = ProductCards.OrderBy(s => s.Name).ToList(); break; }
                    case (IdNameTypeSort.НазваниеDesc): { ProductCards = ProductCards.OrderByDescending(s => s.Name).ToList(); break; }
                    case (IdNameTypeSort.ТипAsc): { ProductCards = ProductCards.OrderBy(s => s.ProductTypeName).ToList(); break; }
                    case (IdNameTypeSort.ТипDesc): { ProductCards = ProductCards.OrderByDescending(s => s.ProductTypeName).ToList(); break; }
                    default: { ProductCards = ProductCards.OrderBy(s => s.Id).ToList(); break; }
                }
            }
        }
        public void SetSelectList()
        {
            if (ProductCards != null && ProductCards.Count > 0)
            {
                Dictionary<string, string> prodyctTypes = (ProductCards
                    .Select(products => new { id = products.ProductTypeId, name = products.ProductTypeName }).Distinct()
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
                if (productTypeId != 0)
                {
                    ProductCards = ProductCards.Where(pt => pt.ProductTypeId == productTypeId || pt.ParentTypeId == productTypeId).ToList();
                }
            }
        }
        public void SetMinMaxPrice() { }
        public void BetweenPrice() { }
        public void OnlySale() { }
        public void OnlyAvailability() { }
        public void SetCount()
        {
            Filters.CountPage = ProductCards.Count;
        }
    }
}
