using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class UserBaseInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AllRoles { get; set; }
        public IList<string> Roles { get; set; }
        public int CurrentPosition { get; set; }
        public UserBaseInfo()
        {
            Roles = new List<string>();
        }
    }
}
