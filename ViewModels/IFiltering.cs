using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public interface IFiltering
    {
        void Sort();
        void SetSelectList();
        void SetCount();
        void SelectFilter();
        void SetMinMaxPrice();
        void BetweenPrice();
        void OnlySale();
        void OnlyAvailability();
    }
}
