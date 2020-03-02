using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.Models
{
    public class ProductsType
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[A-Za-z0-9а-яёА-ЯЁ ]{3,25}", ErrorMessage = "Имя должно состоять только из букв и цифр, от 3 до 25 символов")]
        public string Name { get; set; }
        public int? ParentProductTypeId { get; set; }
        public ProductsType ParentProductType { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[A-Fa-f0-9]{6}", ErrorMessage = "6 символов - цифры и буквы ABCDEF")]
        public string HexColor { get; set; }
    }
}
