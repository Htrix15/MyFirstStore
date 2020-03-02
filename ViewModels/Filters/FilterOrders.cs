using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyFirstStore.ViewModels.Filters;

namespace MyFirstStore.ViewModels
{
    public class FilterOrders:FilterBase, IGetFilterInfoToHtml
    {
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }

        public FilterOrders(string selectFromSelectList, 
                                        int? minPrice, 
                                        int? maxPrice,
                                        int? countVisablePozitins,
                                        int? currentPozition,
                                        string sortType
                                        ) :base(
                                            countVisablePozitins:countVisablePozitins,
                                            currentPosition:currentPozition,
                                            selectFromSelectList:selectFromSelectList,
                                            sortType:sortType)
        {
            MinPrice = minPrice;
            MaxPrice = maxPrice;
        }
        public override string GetFilterName()
        {
            string baseFilerName = base.GetFilterName();
            return $"{baseFilerName}{MinPrice.ToString()??"_"}{MaxPrice.ToString() ?? "_"}";
        }
        public override string GetFilterInfoForTagHelper(bool noSortType)
        {
            StringBuilder stringBuilder = new StringBuilder();

            string minPrice = MinPrice != null ?
                        $"<input type=\"hidden\" name=\"minPrice\" value={MinPrice} />" : "";
            string maxPrice = MaxPrice != null ?
                        $"<input type=\"hidden\" name=\"maxPrice\" value={MaxPrice} />" : "";
            stringBuilder.Append(base.GetFilterInfoForTagHelper(noSortType));
            stringBuilder.Append(minPrice);
            stringBuilder.Append(maxPrice);
            return stringBuilder.ToString();
        }
        public override string GetFilterInfoForFilterTagHelper()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetFilterInfoForFilterTagHelper());
            stringBuilder.Append($"<span class=\"filterText\"> Цена от </span><input class=\"inputStyle\" type =\"text\" name = \"minPrice\" value =\"{MinPrice}\" />");
            stringBuilder.Append($"<span class=\"filterText\"> до </span><input class=\"inputStyle\" " +
                $"type =\"text\" name = \"maxPrice\" value =\"{MaxPrice}\" />");
            return stringBuilder.ToString();
        }
    }
}
