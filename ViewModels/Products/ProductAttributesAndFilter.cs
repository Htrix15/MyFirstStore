using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.Models;


namespace MyFirstStore.ViewModels
{
    public class ProductAttributesAndFilter: EnumToFilter, IFiltering
    {
        public List<ProductsAttribute> ProductAttributes { get; set; }
        public FilterBase Filters { get; set; }
        public void Sort()
        {
            string sortType = Filters.SortType;
            if (ProductAttributes != null &&
                ProductAttributes.Count > 0 &&
                sortType != null &&
                Enum.IsDefined(typeof(IdNameSort), sortType))
            {
                IdNameSort? enumSotrType = (IdNameSort)Enum.Parse(typeof(IdNameSort), sortType);
                switch (enumSotrType)
                {
                    case (IdNameSort.IdAsc): { ProductAttributes = ProductAttributes.OrderBy(s => s.Id).ToList(); break; }
                    case (IdNameSort.IdDesc): { ProductAttributes = ProductAttributes.OrderByDescending(s => s.Id).ToList(); break; }
                    case (IdNameSort.НазваниеAsc): { ProductAttributes = ProductAttributes.OrderBy(s => s.Name).ToList(); break; }
                    case (IdNameSort.НазваниеDesc): { ProductAttributes = ProductAttributes.OrderByDescending(s => s.Name).ToList(); break; }
                    default: { ProductAttributes = ProductAttributes.OrderBy(s => s.Id).ToList(); break; }
                }
            }
        }
        public void SetSelectList() { }
        public void SelectFilter() { }
        public void SetMinMaxPrice() { }
        public void BetweenPrice() { }
        public void OnlySale() { }
        public void OnlyAvailability() { }
        public void SetCount()
        {
            Filters.CountPage = ProductAttributes.Count;
        }

    }
}
