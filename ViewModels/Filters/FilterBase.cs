using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFirstStore.ViewModels.Filters;

namespace MyFirstStore.ViewModels
{
    public class FilterBase : IGetFilterInfoToHtml
    {
        private int? countPage;
        public int? CountPage
        {
            get
            {
                return countPage;
            }
            set
            {
                if (CountVisablePositions == null || CountVisablePositions == 0)
                {
                    CountVisablePositions = 1;
                }
                countPage = (int)Math.Ceiling((double)value / (double)CountVisablePositions);
            }
        }
        public int? CountVisablePositions { get; set; }
        private int? currentPosition;
        public int? CurrentPosition 
        {
            get 
            {
                return currentPosition ?? 1;
            }
            set 
            {
                currentPosition = value;
            } 
        }
        public string SelectFromSelectList { get; set; }
        public string SortType { get; set; }
        public List<SelectListItem> SelectList { get; set; }
        public FilterBase(int? countVisablePozitins, int? currentPosition, string selectFromSelectList = null, string sortType = null)
        {
            CountVisablePositions = countVisablePozitins;
            CurrentPosition = currentPosition;
            SelectFromSelectList = selectFromSelectList;
            SortType = sortType;
        }
        public void SetSelectList(Dictionary<string, string> dictionaryValues)
        {
            SelectList = new List<SelectListItem>
                {
                    new SelectListItem() { Value = "0", Text = "Все" }
                };
            foreach (var item in dictionaryValues)
            {
                SelectList.Add(new SelectListItem() { Value = item.Key, Text = item.Value });
            }
        }
        public void SetSelectList(List<string> roles)
        {
            if (roles != null)
            {
                SelectList = new List<SelectListItem>
                {
                    new SelectListItem() { Value = "0", Text = "Все" }
                };
                foreach (var item in roles)
                {
                    SelectList.Add(new SelectListItem() { Value = item, Text = item });
                }
            }
        }
        public virtual string GetFilterName()
        {
            return $"{SortType ?? "_"}{SelectFromSelectList ?? "_"}";
        }
        public virtual string GetFilterInfoForTagHelper(bool noSortType)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!noSortType)
            {
                string sortType = SortType != null ?
                                        $"<input type=\"hidden\" name=\"sortType\" value={SortType} />" : "";
                stringBuilder.Append(sortType);
            }
            string select = SelectFromSelectList != null ?
                                    $"<input type=\"hidden\" name=\"select\" value={SelectFromSelectList} />" : "";
            stringBuilder.Append(select);
            return stringBuilder.ToString();
        }
        public int GetCurrentPosition()
        {
            return CurrentPosition ?? 1;
        }
        public int GetCountPage()
        {
            return (int)CountPage;
        }
        public string GetSortType()
        {
            return SortType;
        }
        public List<SelectListItem> GetSelectListItems()
        {
            return SelectList;
        }
        public string GetSelect()
        {
            return SelectFromSelectList;
        }
        public virtual string GetFilterInfoForFilterTagHelper()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"<input type=\"hidden\" name=\"currentPosition\" value=\"1\" />");
            return stringBuilder.ToString();
        }
        public virtual string GetWhereSearch()
        {
            return null;
        }
        public virtual string GetDesired()
        {
            return null;
        }
    }
}
