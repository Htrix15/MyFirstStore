using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyFirstStore.ViewModels
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[A-Za-z0-9]{3,25}", ErrorMessage = "Имя должно состоять только из букв латинского алфавита и цифр, от 3 до 25 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[A-Za-z0-9!@#$%^&*]{5,25}", ErrorMessage = "Пароль должен быть длиной от 5 до 25 символов, состоять из букв латинского алфавита, цифр, символоа @#$%^&* ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
