using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyFirstStore.ViewModels
{
    public class UserChangePassword
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[A-Za-z0-9!@#$%^&*]{5,25}", ErrorMessage = "Пароль должен быть длиной от 5 до 25 символов, состоять из букв латинского алфавита, цифр, символоа @#$%^&* ")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[A-Za-z0-9!@#$%^&*]{5,25}", ErrorMessage = "Пароль должен быть длиной от 5 до 25 символов, состоять из букв латинского алфавита, цифр, символоа @#$%^&* ")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
}
