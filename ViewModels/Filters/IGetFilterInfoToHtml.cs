using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels.Filters
{
    public interface IGetFilterInfoToHtml
    {
        string GetFilterInfoForTagHelper(bool noSortType);
        string GetFilterInfoForFilterTagHelper();
        List<SelectListItem> GetSelectListItems();
        int GetCurrentPosition();
        int GetCountPage();
        string GetSortType();
        string GetSelect();
        string GetWhereSearch();
        string GetDesired();
    }
}
