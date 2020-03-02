using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.ViewModels;

namespace MyFirstStore.Services
{
    public class DataFiltering
    {
        public void Filtering(IFiltering data)
        {
            data.OnlySale();
            data.OnlyAvailability();
            data.SelectFilter();
            data.BetweenPrice();
            data.Sort();
        }
        public void SetFilters(IFiltering data)
        {
            data.SetCount();
            data.SetMinMaxPrice();
            data.SetSelectList();
        }
        public List<T> Paginator<T>(List<T> result, int countVisablePositins, int currentPosition)
        {
            if (result != null && result.Count != 0)
            {
                result = result.Skip(((int)currentPosition - 1) * countVisablePositins).Take(countVisablePositins).ToList();
            }
            return result;
        }
    }
}
