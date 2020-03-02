using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class UsersBaseInfoAndFilters:EnumToFilter, IFiltering
    {
        public FilterBase Filters { get; set; }
        public List<UserBaseInfo> UserBaseInfos { get; set; }
        public void Sort()
        {
            string sortType = Filters.SortType;
            if (UserBaseInfos != null &&
                UserBaseInfos.Count > 0 &&
                sortType != null &&
                Enum.IsDefined(typeof(IdNameUserSort), sortType))
            {
                IdNameUserSort? enumSortType = (IdNameUserSort)Enum.Parse(typeof(IdNameUserSort), sortType);
                switch (enumSortType)
                {
                    case (IdNameUserSort.IdAsc): { UserBaseInfos = UserBaseInfos.OrderBy(s => s.Id).ToList(); break; }
                    case (IdNameUserSort.IdDesc): { UserBaseInfos = UserBaseInfos.OrderByDescending(s => s.Id).ToList(); break; }
                    case (IdNameUserSort.ИмяAsc): { UserBaseInfos = UserBaseInfos.OrderBy(s => s.Name).ToList(); break; }
                    case (IdNameUserSort.ИмяDesc): { UserBaseInfos = UserBaseInfos.OrderByDescending(s => s.Name).ToList(); break; }
                    default: { UserBaseInfos = UserBaseInfos.OrderBy(s => s.Id).ToList(); break; }
                }
            }

        }
        public void SetSelectList()
        {
            if (UserBaseInfos != null & UserBaseInfos.Count > 0)
            {
                Filters.SetSelectList(UserBaseInfos.SelectMany(allUsersRoles => allUsersRoles.Roles).Distinct().ToList());
            }
        }
        public void SelectFilter()
        {
            if (Filters.SelectFromSelectList != null)
            {
                UserBaseInfos = UserBaseInfos.Where(roles => roles.Roles.Contains(Filters.SelectFromSelectList)).ToList();
            }
        }
        public void SetMinMaxPrice() { }
        public void BetweenPrice() { }
        public void OnlySale() { }
        public void OnlyAvailability() { }
        public void SetCount()
        {
            Filters.CountPage = UserBaseInfos.Count;
        }
    }
}
