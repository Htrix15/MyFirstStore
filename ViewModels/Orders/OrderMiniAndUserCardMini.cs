using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels.Storekeeper
{
    public class OrderMiniAndUserCardMini: OrderMini
    {
        public UserCardMini UserInfo { get; set; }
    }
}
