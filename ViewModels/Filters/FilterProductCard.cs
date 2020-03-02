using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFirstStore.ViewModels.Filters;

namespace MyFirstStore.ViewModels
{
    public class FilterProductCard: FilterBaseAndSearch, IGetFilterInfoToHtml
    {
        public bool? OnlyAvailability { get; set; }
        public bool? OnlySale { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public FilterProductCard(bool? onlyAvailability,
                                 bool? onlySale,
                                 int? minPrice,
                                 int? maxPrice,
                                 string selectFromSelectList,
                                 int? countVisablePositions, 
                                 int? currentPosition, 
                                 string desired =null,
                                 string sortType = null,
                                 string whereSearch=null
                                 ) :base(
                                     desired:desired,
                                     whereSearch: whereSearch,
                                     countVisablePositions:countVisablePositions,
                                     currentPosition:currentPosition,
                                     selectFromSelectList:selectFromSelectList,
                                     sortType:sortType
                                     )
        {
            OnlyAvailability = onlyAvailability;
            OnlySale = onlySale;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
        }
        public override string GetFilterName()
        {
            string baseFilerName = base.GetFilterName();
            return $"{baseFilerName}{OnlyAvailability.ToString() ?? "_"}{OnlySale.ToString() ?? "_"}{MinPrice.ToString() ?? "_"}{MaxPrice.ToString() ?? "_"}";
        }
        public override string GetFilterInfoForTagHelper(bool noSortType)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string productTypeIdy = SelectFromSelectList != null ?
                        $"<input type=\"hidden\" name=\"select\" value={SelectFromSelectList} />" : "";
            string onlyAvailability = OnlyAvailability != null ?
                        $"<input type=\"hidden\" name=\"onlyAvailability\" value={OnlyAvailability} />" : "";
            string onlySale = OnlySale != null ?
                                    $"<input type=\"hidden\" name=\"onlySale\" value={OnlySale} />" : "";
            string minPrice = MinPrice != null ?
                       $"<input type=\"hidden\" name=\"minPrice\" value={MinPrice} />" : "";
            string maxPrice = MaxPrice != null ?
                        $"<input type=\"hidden\" name=\"maxPrice\" value={MaxPrice} />" : "";
            stringBuilder.Append(base.GetFilterInfoForTagHelper(noSortType));
            stringBuilder.Append(productTypeIdy);
            stringBuilder.Append(minPrice);
            stringBuilder.Append(maxPrice);
            stringBuilder.Append(onlyAvailability);
            stringBuilder.Append(onlySale);
            return stringBuilder.ToString();
        }
        public override string GetFilterInfoForFilterTagHelper()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetFilterInfoForFilterTagHelper());
            stringBuilder.Append($"<span class=\"filterText\"> Цена от </span><input type =\"text\" class=\"inputStyle\" name = \"minPrice\" value =\"{MinPrice}\" />");
            stringBuilder.Append($"<span class=\"filterText\"> до </span><input type =\"text\" class=\"inputStyle\" name = \"maxPrice\" value =\"{MaxPrice}\" />");
            string checkedOnly = OnlySale??false ? "checked" : "";
            stringBuilder.Append($"<span class=\"filterText\"> Только скидки:</span><input type = \"checkbox\" name = \"onlySale\"  {checkedOnly} value = \"true\"/>");
            checkedOnly = OnlyAvailability ?? false ? "checked" : "";
            stringBuilder.Append($"<span class=\"filterText\">Только в наличии:</span><input type = \"checkbox\" name = \"onlyAvailability\" {checkedOnly} value = \"true\"/>");
            return stringBuilder.ToString();
        }


    }
}
