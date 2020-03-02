using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyFirstStore.ViewModels
{
    public class UserForEdit
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[A-Za-z0-9]{3,25}", ErrorMessage = "Имя должно состоять только из букв латинского алфавита и цифр, от 3 до 25 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public List<IdentityRole> AllRoles { get; set; }
        public IList<string> Roles { get; set; }
        public UserForEdit()
        {
            Roles = new List<string>();
            AllRoles = new List<IdentityRole>();
        }
    }
}
