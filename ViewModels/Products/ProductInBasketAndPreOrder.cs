using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class ProductInBasketAndPreOrder: ProductInBasket
    {
        public int CountToStore { get; set; }
        public int DeliveryToStoreDay { get; set; }
        public bool AvailableOnRequest { get; set; }
        public int AvailableMaxCountOnRequest { get; set; } 
        public OrderPositionStatus Status { get; set; }
        public void SetStatus()
        {
            if (Count <= CountToStore)
            {
                Status = OrderPositionStatus.Доступно;
                HEXcolor = "138a1b";//green
            }
            else
            {
                if(AvailableOnRequest && Count <= (CountToStore + AvailableMaxCountOnRequest))
                {
                    Status = OrderPositionStatus.Под_заказ;
                    HEXcolor = "ede211";//yellow
                }
                else
                {
                    Status = OrderPositionStatus.Недоступно;
                    HEXcolor = "ed1111";//red
                }
            }

        }
        public ProductInBasket GetLiteInfo()
        {
            return new ProductInBasket() {Id = this.Id, 
                                          Name = this.Name,
                                          Count= this.Count, 
                                          HEXcolor= this.HEXcolor, 
                                          MainPicturePath= this.MainPicturePath, 
                                          Price= this.Price };
        }
    }
}
