using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.ViewModels.Storekeeper;

namespace MyFirstStore.ViewModels
{
    public class UserOrdersAndFilters: EnumToFilter, IFiltering
    {
        public FilterBaseAndSearch Filters { get; set; }
        public List<OrderMiniAndUserCardMini> OrderMiniAndUserCardMini { get; set; }
        public void Sort()
        {
            string sortType = Filters.SortType;
            if (OrderMiniAndUserCardMini != null &&
                OrderMiniAndUserCardMini.Count > 0 &&
                sortType != null &&
                Enum.IsDefined(typeof(UserOrders), sortType))
            {
                UserOrders enumSortType = (UserOrders)Enum.Parse(typeof(UserOrders), sortType);
                switch (enumSortType)
                {
                    case (UserOrders.EmailAsc): { OrderMiniAndUserCardMini = OrderMiniAndUserCardMini.OrderBy(s => s.UserInfo.Email).ToList(); break; }
                    case (UserOrders.EmailDesc): { OrderMiniAndUserCardMini = OrderMiniAndUserCardMini.OrderByDescending(s => s.UserInfo.Email).ToList(); break; }
                    case (UserOrders.IdAsc): { OrderMiniAndUserCardMini = OrderMiniAndUserCardMini.OrderBy(s => s.Id).ToList(); break; }
                    case (UserOrders.IdDesc): { OrderMiniAndUserCardMini = OrderMiniAndUserCardMini.OrderByDescending(s => s.Id).ToList(); break; }
                    case (UserOrders.Дата_резерваAsc): { OrderMiniAndUserCardMini = OrderMiniAndUserCardMini.OrderBy(s => s.DataReservation).ToList(); break; }
                    case (UserOrders.Дата_резерваDesc): { OrderMiniAndUserCardMini = OrderMiniAndUserCardMini.OrderByDescending(s => s.DataReservation).ToList(); break; }
                    case (UserOrders.ЗаказчикAsc): { OrderMiniAndUserCardMini = OrderMiniAndUserCardMini.OrderBy(s => s.UserInfo.Name).ToList(); break; }
                    case (UserOrders.ЗаказчикDesc): { OrderMiniAndUserCardMini = OrderMiniAndUserCardMini.OrderByDescending(s => s.UserInfo.Name).ToList(); break; }
                    case (UserOrders.СтатусAsc): { OrderMiniAndUserCardMini = OrderMiniAndUserCardMini.OrderBy(s => s.StatusName).ToList(); break; }
                    case (UserOrders.СтатусDesc): { OrderMiniAndUserCardMini = OrderMiniAndUserCardMini.OrderByDescending(s => s.StatusName).ToList(); break; }
                    default: { OrderMiniAndUserCardMini = OrderMiniAndUserCardMini.OrderByDescending(s => s.DataReservation).ToList(); break; }
                }
            }
        }
        public void SetSelectList()
        {
            if (OrderMiniAndUserCardMini != null && OrderMiniAndUserCardMini.Count > 0)
            {
                Dictionary<string, string> statuses = OrderMiniAndUserCardMini.Select(stses => new { stses.StatusId, stses.StatusName })
                    .Distinct().ToDictionary(stses => stses.StatusId.ToString(), stses => stses.StatusName);
                Filters.SetSelectList(statuses);
            }
        }
        public void SelectFilter()
        {
            if (Filters.SelectFromSelectList != null)
            {
                int ordrStatusId = Convert.ToInt32(Filters.SelectFromSelectList);
                OrderMiniAndUserCardMini = OrderMiniAndUserCardMini.Where(preOrders => preOrders.StatusId == ordrStatusId).ToList();
            }
        }
        public void SetMinMaxPrice() { }
        public void BetweenPrice() { }
        public void OnlySale() { }
        public void OnlyAvailability() { }
        public void SetCount()
        {
            Filters.CountPage = OrderMiniAndUserCardMini?.Count??0;
        }
    }
}
