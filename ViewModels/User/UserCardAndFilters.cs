using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.Models;

namespace MyFirstStore.ViewModels
{
    public class UserCardAndFilters : UserCardMini, IFiltering
    {
        public List<OrderMini> Orders { get; set; }
        public List<OrderPositionsMini> OrderPositions { get; set; }
        public FilterOrders Filters { get; set; }
        public void Sort()
        {
            string sortType = Filters.SortType;
            if (Orders != null &&
                Orders.Count > 0 &&
                sortType != null &&
                Enum.IsDefined(typeof(OrderSort), sortType))
            {
                OrderSort? enumSortType = (OrderSort)Enum.Parse(typeof(OrderSort), sortType);
                switch (enumSortType)
                {
                    case (OrderSort.IdAsc): { Orders = Orders.OrderBy(s => s.Id).ToList(); break; }
                    case (OrderSort.IdDesc): { Orders = Orders.OrderByDescending(s => s.Id).ToList(); break; }
                    case (OrderSort.Дата_резерваAsc): { Orders = Orders.OrderBy(s => s.DataReservation).ToList(); break; }
                    case (OrderSort.Дата_резерваDesc): { Orders = Orders.OrderByDescending(s => s.DataReservation).ToList(); break; }
                    case (OrderSort.СтатусAsc): { Orders = Orders.OrderBy(s => s.StatusId).ToList(); break; }
                    case (OrderSort.СтатусDesc): { Orders = Orders.OrderByDescending(s => s.StatusId).ToList(); break; }
                    case (OrderSort.ЦенаAsc): { Orders = Orders.OrderBy(s => s.TotalFixCost).ToList(); break; }
                    case (OrderSort.ЦенаDesc): { Orders = Orders.OrderByDescending(s => s.TotalFixCost).ToList(); break; }
                    default: { Orders = Orders.OrderByDescending(s => s.Id).ToList(); break; }
                }
            }
        }
        public void SetSelectList()
        {
            if (Orders != null && Orders.Count > 0)
            {
                Dictionary<string, string> statuses = Orders.Select(stses => new { stses.StatusId, stses.StatusName})
                    .Distinct().ToDictionary(stses => stses.StatusId.ToString(), stses => stses.StatusName);
                Filters.SetSelectList(statuses);
            }
        }
        public void SelectFilter()
        {
            if (Filters.SelectFromSelectList != null)
            {
                int ordrStatusId = Convert.ToInt32(Filters.SelectFromSelectList);
                Orders = Orders.Where(preOrders => preOrders.StatusId == ordrStatusId).ToList();
            }
        }
        public void SetMinMaxPrice()
        {
            if (Orders != null && Orders.Count > 0)
            {
                Filters.MinPrice = (int?)Orders.Min(productCards => productCards.TotalFixCost);
                Filters.MaxPrice = (int?)Orders.Max(productCards => productCards.TotalFixCost);
            }
        }
        public void BetweenPrice()
        {
            if (Filters.MinPrice != null && Filters.MaxPrice != null)
            {
                Orders.Where(orders => orders.TotalFixCost >= (int)Filters.MinPrice && 
                                       orders.TotalFixCost <= (int)Filters.MaxPrice).ToList();
            }
        }
        public void OnlySale() { }
        public void OnlyAvailability() { }
        public void SetCount()
        {
            Filters.CountPage = Orders?.Count??0;
        }
    }
}
