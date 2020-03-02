using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.ViewModels;

namespace MyFirstStore.Services
{
    public class PreOrderService
    {
        public List<ProductInBasketAndPreOrder> CreatePreOrder(List<ProductInBasketAndPreOrder> preOrder)
        {
            foreach(var preOrderItem in preOrder)
            {
                preOrderItem.SetStatus();
            }
            return preOrder;
        }
        public bool ApprovePreOrder(List<ProductInBasketAndPreOrder> preOrder)
        {
            bool approve = true;
            if (preOrder==null || preOrder.Count==0)
            {
                return false;
            }
            foreach (var preOrderItem in preOrder)
            {
                if(preOrderItem.Status == OrderPositionStatus.Недоступно)
                {
                    approve = false;
                    break;
                }
            }
            return approve;
        }
        public DateTime GetOrderData(List<ProductInBasketAndPreOrder> preOrder)
        {
            DateTime orderDate = DateTime.Today;
            int deliveryToStoreDay = 0;
            var UnderOrderPosition = preOrder.Where(preOrderPositions => preOrderPositions.Status == OrderPositionStatus.Под_заказ).ToList();
            if (UnderOrderPosition.Count()>0)
            {
                deliveryToStoreDay = UnderOrderPosition.Max(preOrderPositions => preOrderPositions.DeliveryToStoreDay);
            }
            orderDate = orderDate.AddDays(deliveryToStoreDay);
            return orderDate;
        }
        public List<ProductInBasket> AlleviationProductInBasket(List<ProductInBasketAndPreOrder> preOrder)
        {
            List<ProductInBasket> rezult = new List<ProductInBasket>();
            foreach (var item in preOrder)
                rezult.Add(item.GetLiteInfo());
            return rezult;
        }
    }
}
