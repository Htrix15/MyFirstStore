using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class ProductTypeMiniAndFilters: EnumToFilter, IFiltering
    {
        public List<ProductTypeMini> ProductTypeMinis { get; set; }
        public FilterBase Filters { get; set; }
        public void Sort()
        {
            string sortType = Filters.SortType;
            if (ProductTypeMinis != null &&
                ProductTypeMinis.Count > 0 &&
                sortType != null &&
                Enum.IsDefined(typeof(IdNameTypeSort), sortType))
            {
                IdNameTypeSort? enumSortType = (IdNameTypeSort)Enum.Parse(typeof(IdNameTypeSort), sortType);
                switch (enumSortType)
                {
                    case (IdNameTypeSort.IdAsc): { ProductTypeMinis = ProductTypeMinis.OrderBy(s => s.Id).ToList(); break; }
                    case (IdNameTypeSort.IdDesc): { ProductTypeMinis = ProductTypeMinis.OrderByDescending(s => s.Id).ToList(); break; }
                    case (IdNameTypeSort.НазваниеAsc): { ProductTypeMinis = ProductTypeMinis.OrderBy(s => s.Name).ToList(); break; }
                    case (IdNameTypeSort.НазваниеDesc): { ProductTypeMinis = ProductTypeMinis.OrderByDescending(s => s.Name).ToList(); break; }
                    case (IdNameTypeSort.ТипAsc): { ProductTypeMinis = ProductTypeMinis.OrderBy(s => s.ParentProductTypeName).ToList(); break; }
                    case (IdNameTypeSort.ТипDesc): { ProductTypeMinis = ProductTypeMinis.OrderByDescending(s => s.ParentProductTypeName).ToList(); break; }
                    default: { ProductTypeMinis = ProductTypeMinis.OrderBy(s => s.Id).ToList(); break; }
                }
            }
        }
        public void SetSelectList()
        {
            if (ProductTypeMinis!=null && ProductTypeMinis.Count > 0)
            {
                Dictionary<string, string> prodyctTypes = ProductTypeMinis
                    .Select(pt => new { id = pt.ParentProductTypeId, name = pt.ParentProductTypeName })
                    .Distinct().ToDictionary(products => products.id.ToString(), products => products.name);
                Filters.SetSelectList(prodyctTypes);
            }
        }
        public void SelectFilter()
        {
            if (Filters.SelectFromSelectList != null)
            {
                int productTypeId = Convert.ToInt32(Filters.SelectFromSelectList);
                ProductTypeMinis = ProductTypeMinis.Where(pt => pt.ParentProductTypeId == productTypeId).ToList();
            }
        }
        public void SetMinMaxPrice() { }
        public void BetweenPrice() { }
        public void OnlySale() { }
        public void OnlyAvailability() { }
        public void SetCount()
        {
            Filters.CountPage = ProductTypeMinis.Count;
        }
    }
}
