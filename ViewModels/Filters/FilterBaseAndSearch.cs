using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFirstStore.ViewModels.Filters;

namespace MyFirstStore.ViewModels
{
    public class FilterBaseAndSearch: FilterBase, IGetFilterInfoToHtml
    {
        public string WhereSearch { get; set; }
        public string Desired { get; set; }
        public FilterBaseAndSearch( 
                                    string desired,
                                    string whereSearch,
                                    int? countVisablePositions,
                                    int? currentPosition,
                                    string selectFromSelectList,
                                    string sortType)
                                    :base(countVisablePozitins:countVisablePositions,
                                         currentPosition:currentPosition,
                                         selectFromSelectList:selectFromSelectList,
                                         sortType:sortType)
        {
            WhereSearch = whereSearch;
            Desired = desired;
        }
        public override string GetFilterName()
        {
            string baseFilerName = base.GetFilterName();
            return $"{baseFilerName}{WhereSearch ?? "_"}{Desired ?? "_"}";
        }
        public override string GetFilterInfoForTagHelper(bool noSortType)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetFilterInfoForTagHelper(noSortType));
            string desired = Desired != null ?
                                  $"<input type=\"hidden\" name=\"desired\" value={Desired} />" : "";
            string whereSearch = WhereSearch != null ?
                                    $"<input type=\"hidden\" name=\"whereSearch\" value= {WhereSearch} />" : "";
            stringBuilder.Append(desired);
            stringBuilder.Append(whereSearch);
            return stringBuilder.ToString();
        }
        public override string GetFilterInfoForFilterTagHelper()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetFilterInfoForFilterTagHelper());
            string desired = Desired != null ?
                                  $"<input type=\"hidden\" name=\"desired\" value={Desired} />" : "";
            string whereSearch = WhereSearch != null ?
                                    $"<input type=\"hidden\" name=\"whereSearch\" value= {WhereSearch} />" : "";
            stringBuilder.Append(desired);
            stringBuilder.Append(whereSearch);
            return stringBuilder.ToString();
        }
        public override string GetWhereSearch()
        {
            return WhereSearch;
        }
        public override string GetDesired()
        {
            return Desired;
        }
    }
}
