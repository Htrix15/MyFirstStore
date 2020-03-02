using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyFirstStore.Models
{
    public class ProductsAttribute
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[A-Za-z0-9а-яёА-ЯЁ ]{3,25}", ErrorMessage = "Имя должно состоять только из букв и цифр, от 3 до 25 символов")]
        public string Name { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
