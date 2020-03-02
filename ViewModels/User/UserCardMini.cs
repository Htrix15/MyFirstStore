using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.Models;

namespace MyFirstStore.ViewModels
{
    public class UserCardMini: EnumToFilter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public void SetUserCardMini(User user)
        {
            Id = user.Id;
            Name = user.UserName;
            Email = user.Email;
        }
    }
}
