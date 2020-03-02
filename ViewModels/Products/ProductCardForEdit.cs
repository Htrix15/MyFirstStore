using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class ProductCardForEdit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ProductTypeId { get; set; }
        public int? ParentTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string ParentTypeName { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"^[0-9]*[,]?[0-9]+$", ErrorMessage = "Десятичные числа через запятую")]
        public decimal Price { get; set; }
        public bool Sale { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"^[0-9]*[,]?[0-9]+$", ErrorMessage = "Десятичные числа через запятую")]
        public decimal SalePrice { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[0-9]+$", ErrorMessage = "Только целые числа")]
        public int CountToStore { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[0-9]+$", ErrorMessage = "Только целые числа")]
        public int DeliveryToStoreDay { get; set; }
        public bool AvailableOnRequest { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[0-9]+$", ErrorMessage = "Только целые числа")]
        public int AvailableMaxCountOnRequest { get; set; }
    }
}
