using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int ProductsTypeId { get; set; }
        public ProductsType ProductsType { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[A-Za-z0-9а-яёА-ЯЁ ]{3,25}", ErrorMessage = "Имя должно состоять только из букв и цифр, от 3 до 25 символов")]
        public string Name { get; set; }

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

        [RegularExpression(@".{3,25}", ErrorMessage = "Краткое описание должно состоять только из букв и цифр, от 3 до 30 символов")]
        public string MainAttribute { get; set; }

        [RegularExpression(@".{3,300}", ErrorMessage = "Описание должно состоять только из букв и цифр, от 3 до 300 символов")]
        public string Description { get; set; }
        public string MainPicturePath { get; set; }
        public bool AvailableOnRequest { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"[0-9]+$", ErrorMessage = "Только целые числа")]
        public int AvailableMaxCountOnRequest { get; set; }
        //-----------------------------------------------
        public int AvailableMaxCountOnRequestOLD { get; set; }
        public int CountToStoreOLD { get; set; }
        public void SetCountToStore(int countToOrder)
        {
            CountToStoreOLD = CountToStore;
            AvailableMaxCountOnRequestOLD = AvailableMaxCountOnRequest;
            CountToStore -= countToOrder;
            if(CountToStore<0)
            {
                AvailableMaxCountOnRequest += CountToStore;
                CountToStore = 0;
            }
        }
        public void RecoveryCountToStore()
        {
            if (CountToStore < CountToStoreOLD)
            {
                CountToStore = CountToStoreOLD;
            }
            if (AvailableMaxCountOnRequest < AvailableMaxCountOnRequestOLD)
            {
                AvailableMaxCountOnRequest = AvailableMaxCountOnRequestOLD;
            }
            CountToStoreOLD = AvailableMaxCountOnRequestOLD = 0;
        }
    }
}
