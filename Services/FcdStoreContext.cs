using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.Models;
using MyFirstStore.Services;
using MyFirstStore.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using MyFirstStore.ViewModels.Storekeeper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace MyFirstStore.Services
{
    public class FcdStoreContext
    {
        private readonly StoreContext _storeContext;
        private readonly FileOperations _fileOperations;
        private readonly ILogger<FcdStoreContext> _logger;
        public FcdStoreContext(StoreContext storeContext,
                               FileOperations fileOperations,
                               ILogger<FcdStoreContext> logger)
        {
            _logger = logger;
            _storeContext = storeContext;
            _fileOperations = fileOperations;
        }
        public async Task<List<ProductTypeMini>> GetProductTypeMinisAsync()
        {
            return  await _storeContext.ProductsTypes
                                       .Select(pt => new ProductTypeMini()
                                       { 
                                           Id = pt.Id, 
                                           Name = pt.Name, 
                                           HEXColor = pt.HexColor, 
                                           ParentProductTypeId = pt.ParentProductTypeId, 
                                           ParentProductTypeName = pt.ParentProductType.Name 
                                       }).Skip(2).AsNoTracking().ToListAsync();
        }
        private async Task<List<ProductsType>> GetProductTypeAsync()
        {
            return await _storeContext.ProductsTypes.AsNoTracking().ToListAsync();
        }
        public async Task<List<ProductInBasket>> GetProductInBasketsAsync(List<int> prodyctIds)
        {
            return await _storeContext.Products.Where(products => prodyctIds.Contains(products.Id))
                                                                 .Select(products => new ProductInBasket()
                                                                 {
                                                                     Id = products.Id,
                                                                     Name = products.Name,
                                                                     Price = products.Sale ? products.SalePrice : products.Price,
                                                                     MainPicturePath = products.MainPicturePath,
                                                                     HEXcolor = products.Sale? "f69730" : products.ProductsType.HexColor
                                                                 }).AsNoTracking().ToListAsync();
        }
        public async Task<List<ProductCard>> GetProductCardsAsync(string select, int currentPosition)
        {
            List<ProductCard> result = new List<ProductCard>();
            if (select != null)
            {
                int prodyctTypeId =Convert.ToInt32(select);
                result = await _storeContext.Products.Where(products => products.ProductsTypeId == prodyctTypeId || products.ProductsType.ParentProductTypeId == prodyctTypeId)
                    .Select(products => new ProductCard{
                                                             Id = products.Id,
                                                             Name = products.Name,
                                                             Sale = products.Sale,
                                                             Price = products.Sale ? products.SalePrice : products.Price,
                                                             HexColor = products.Sale ? "f69730" : products.ProductsType.HexColor,
                                                             MainPicturePath = products.MainPicturePath,
                                                             MainAttribute = products.MainAttribute,
                                                             CountToStore = products.CountToStore,
                                                             ProductTypeId = products.ProductsTypeId,
                                                             ParentTypeId = products.ProductsType.ParentProductTypeId,
                                                             ParentTypeName = products.ProductsType.ParentProductType.Name,
                                                             ProductTypeName = products.ProductsType.Name,
                                                             CurrentPosition = currentPosition}).AsNoTracking().ToListAsync();
            }
            else
            {
                result = await _storeContext.Products.Select(products => new ProductCard{
                    Id = products.Id,
                    Name = products.Name,
                    Sale = products.Sale,
                    Price = products.Sale ? products.SalePrice : products.Price,
                    HexColor = products.ProductsType.HexColor,
                    MainPicturePath = products.MainPicturePath,
                    MainAttribute = products.MainAttribute,
                    CountToStore = products.CountToStore,
                    ProductTypeId = products.ProductsTypeId,
                    ParentTypeId = products.ProductsType.ParentProductTypeId,
                    ParentTypeName = products.ProductsType.ParentProductType.Name,
                    ProductTypeName = products.ProductsType.Name,
                    CurrentPosition = currentPosition
                }).AsNoTracking().ToListAsync();
            }
            return result;
        }
        public async Task<List<ProductCardForEdit>> GetStProductCardAsync()
        {
            return await _storeContext.Products.Select(products =>
                            new ProductCardForEdit()
                            {
                                Id = products.Id,
                                ProductTypeId = products.ProductsTypeId,
                                ParentTypeId = products.ProductsType.ParentProductTypeId,
                                Name = products.Name,
                                ProductTypeName = products.ProductsType.Name,
                                ParentTypeName = products.ProductsType.ParentProductType.Name,
                                Sale = products.Sale,
                                Price = products.Price,
                                CountToStore = products.CountToStore,
                                SalePrice = products.SalePrice,
                                DeliveryToStoreDay = products.DeliveryToStoreDay,
                                AvailableMaxCountOnRequest = products.AvailableMaxCountOnRequest,
                                AvailableOnRequest = products.AvailableOnRequest
                            }).ToListAsync();
        }
        public async Task<BigProductCard> GetBigProductCard(int id)
        {
            BigProductCard result = new BigProductCard();
            var allInfoAboutProduct = await (from products in _storeContext.Products
                                 join attributes in _storeContext.AttributesToProducts on products.Id equals attributes.ProductId
                                 join pictutes in _storeContext.ProductsPicturesPaths on products.Id equals pictutes.ProductId
                                 where products.Id == id
                                 select (new {products.Id,
                                     products.Name,
                                     products.Sale,
                                     Price = products.Sale ? products.SalePrice : products.Price,
                                     products.MainPicturePath,
                                     products.MainAttribute,
                                     products.CountToStore,
                                     products.DeliveryToStoreDay,
                                     products.Description,
                                     ProductTypeName = products.ProductsType.Name,
                                     ParentTypeName = products.ProductsType.ParentProductType.Name,
                                     ProductTypeId = products.ProductsTypeId,
                                     products.ProductsType.ParentProductTypeId,
                                     products.ProductsType.HexColor,
                                     ParentTypeHEXColor = products.ProductsType.ParentProductType.HexColor,
                                     attributesDesc = attributes.ProductsAttribute.Name,
                                     attributesValue = attributes.Value,
                                     ProductsPicturesPaths = pictutes.PicturePath})).AsNoTracking().ToListAsync();
            if(allInfoAboutProduct.Count>0)
            {
                result = allInfoAboutProduct.Select(products => new BigProductCard() {
                    Id = products.Id,
                    Name = products.Name,
                    Sale = products.Sale,
                    Price = products.Price,
                    MainPicturePath = products.MainPicturePath,
                    MainAttribute = products.MainAttribute,
                    CountToStore = products.CountToStore,
                    DeliveryToStoreDay = products.DeliveryToStoreDay,
                    Description = products.Description,
                    ProductTypeName = products.ProductTypeName,
                    ParentTypeName = products.ParentTypeName,
                    ProductTypeId = products.ProductTypeId,
                    ParentTypeId = products.ParentProductTypeId,
                    HexColor = products.HexColor,
                    ParentProductTypeHEXColor = products.ParentTypeHEXColor
                }).FirstOrDefault();
                List<string> productPicturesPaths = allInfoAboutProduct.Where(table => table.ProductsPicturesPaths != "" && table.ProductsPicturesPaths != null).
                                                                        Select(table => table.ProductsPicturesPaths).
                                                                        Distinct().ToList();
                Dictionary<string, string> productAttribute = allInfoAboutProduct.Where(table => table.attributesValue != "" && table.attributesValue != null).
                                                                        Select(table => new { table.attributesDesc, table.attributesValue }).
                                                                        Distinct().
                                                                        ToDictionary(table => table.attributesDesc, table => table.attributesValue);
                result.AttributesToProduct = productAttribute;
                result.PicturesPaths = productPicturesPaths;
            }
            return result;
        }
        public async Task<List<ProductCard>> GetProductCardsSearchNameAsync(string desired, int currentPosition)
        {
            return await _storeContext.Products.Where(products => EF.Functions.Like(products.Name.ToLower(), $"%{desired}%"))
                                            .Select(products => new ProductCard
                                            {
                                                Id = products.Id,
                                                Name = products.Name,
                                                Sale = products.Sale,
                                                Price = products.Sale ? products.SalePrice : products.Price,
                                                HexColor = products.Sale ? "f69730" : products.ProductsType.HexColor,
                                                MainPicturePath = products.MainPicturePath,
                                                MainAttribute = products.MainAttribute,
                                                CountToStore = products.CountToStore,
                                                ProductTypeId = products.ProductsTypeId,
                                                ProductTypeName = products.ProductsType.Name,
                                                ParentTypeId = products.ProductsType.ParentProductTypeId,
                                                ParentTypeName = products.ProductsType.ParentProductType.Name,
                                                CurrentPosition = currentPosition
                                            }).AsNoTracking().ToListAsync();
        }
        public async Task<List<ProductCard>> GetProductCardsSearchAttributesAsync(string desired, int currentPosition)
        {
            return await (
                from products in _storeContext.Products
                join attributesToProducts in _storeContext.AttributesToProducts on products.Id equals attributesToProducts.ProductId
                where (EF.Functions.Like(attributesToProducts.Value.ToLower(), $"%{desired}%"))
                select (new ProductCard
                {
                    Id = products.Id,
                    Name = products.Name,
                    Sale = products.Sale,
                    Price = products.Sale ? products.SalePrice : products.Price,
                    HexColor = products.Sale ? "f69730" : products.ProductsType.HexColor,
                    MainPicturePath = products.MainPicturePath,
                    MainAttribute = products.MainAttribute,
                    CountToStore = products.CountToStore,
                    ProductTypeId = products.ProductsTypeId,
                    ProductTypeName = products.ProductsType.Name,
                    ParentTypeId = products.ProductsType.ParentProductTypeId,
                    ParentTypeName = products.ProductsType.ParentProductType.Name,
                    CurrentPosition = currentPosition
                })).Distinct().AsNoTracking().ToListAsync();
        }
        public async Task<List<ProductCard>> GetProductCardsSearchDescriptionsAsync(string desired, int currentPosition)
        {
            return await _storeContext.Products.Where(
                                        products => EF.Functions.Like(products.MainAttribute.ToLower(), $"%{desired}%") ||
                                        EF.Functions.Like(products.Description.ToLower(), $"%{desired}%")
                                             )
                                            .Select(products => new ProductCard
                                            {
                                                Id = products.Id,
                                                Name = products.Name,
                                                Sale = products.Sale,
                                                Price = products.Sale ? products.SalePrice : products.Price,
                                                HexColor = products.Sale ? "f69730" : products.ProductsType.HexColor,
                                                MainPicturePath = products.MainPicturePath,
                                                MainAttribute = products.MainAttribute,
                                                CountToStore = products.CountToStore,
                                                ProductTypeId = products.ProductsTypeId,
                                                ProductTypeName = products.ProductsType.Name,
                                                ParentTypeId = products.ProductsType.ParentProductTypeId,
                                                ParentTypeName = products.ProductsType.ParentProductType.Name,
                                                CurrentPosition = currentPosition
                                            }).AsNoTracking().ToListAsync();
        }
        public async Task<List<ProductCard>> GetProductCardsSearchAllAsync(string desired, int currentPosition)
        {
            return  await (from products in _storeContext.Products
                            join attributesToProducts in _storeContext.AttributesToProducts on products.Id equals attributesToProducts.ProductId
                            where (EF.Functions.Like(attributesToProducts.Value.ToLower(), $"%{desired}%")) ||
                                  (EF.Functions.Like(products.MainAttribute.ToLower(), $"%{desired}%")) ||
                                  (EF.Functions.Like(products.Description.ToLower(), $"%{desired}%")) ||
                                  (EF.Functions.Like(products.Name.ToLower(), $"%{desired}%"))
                            select new ProductCard()
                            {
                                Id = products.Id,
                                Name = products.Name,
                                Sale = products.Sale,
                                Price = products.Sale ? products.SalePrice : products.Price,
                                HexColor = products.Sale ? "f69730" : products.ProductsType.HexColor,
                                MainPicturePath = products.MainPicturePath,
                                MainAttribute = products.MainAttribute,
                                CountToStore = products.CountToStore,
                                ProductTypeId = products.ProductsTypeId,
                                ProductTypeName = products.ProductsType.Name,
                                ParentTypeId = products.ProductsType.ParentProductTypeId,
                                ParentTypeName = products.ProductsType.ParentProductType.Name,
                                CurrentPosition = currentPosition
                            }).Distinct().AsNoTracking().ToListAsync();
        }
        public async Task<List<ProductInBasketAndPreOrder>> GetProductInBasketsPreOrderAsync(List<int> prodyctIds)
        {
            var result = await _storeContext.Products.Where(products => prodyctIds.Contains(products.Id))
                .Select(products => new ProductInBasketAndPreOrder() {
                    Id = products.Id,
                    Name = products.Name,
                    Price = products.Sale ? products.SalePrice : products.Price,
                    MainPicturePath = products.MainPicturePath,
                    CountToStore = products.CountToStore,
                    DeliveryToStoreDay = products.DeliveryToStoreDay,
                    AvailableOnRequest = products.AvailableOnRequest,
                    AvailableMaxCountOnRequest = products.AvailableMaxCountOnRequest}).AsNoTracking().ToListAsync();
            return result;
        }
        public async Task<int> SetOrderAsync(List<ProductInBasket> productsInBasket, DateTime orderDate, string userId)
        {
            int orderCode = -1;
            int succesAddOrUpdatePositions;
            Order newOrder = new Order(userId) { DataReservation = orderDate };
            List<OrderPosition> orderPosition = new List<OrderPosition>();
            try
            {
                bool succesCountToStores = await SetCountToStoreAsync(productsInBasket);
                if (succesCountToStores)
                {
                    _storeContext.Orders.Add(newOrder);
                    succesAddOrUpdatePositions = await _storeContext.SaveChangesAsync();
                    if (succesAddOrUpdatePositions != 0)
                    {
                        orderCode = newOrder.Id;
                        decimal totalPrice = 0m;
                        foreach (var item in productsInBasket)
                        {
                            orderPosition.Add(new OrderPosition()
                            {
                                Order = newOrder,
                                ProductId = item.Id,
                                Count = item.Count,
                                FixPrice = item.Price
                            });
                            totalPrice += item.Price* item.Count;
                        }
                        _storeContext.OrderPositions.AddRange(orderPosition);
                        succesAddOrUpdatePositions = await _storeContext.SaveChangesAsync();
                        if (succesAddOrUpdatePositions == orderPosition.Count)
                        {
                            newOrder.TotalFixPrice = totalPrice;
                            _storeContext.Orders.Update(newOrder);
                            succesAddOrUpdatePositions = await _storeContext.SaveChangesAsync();
                            if (succesAddOrUpdatePositions == 0)
                            {
                                throw new IsNecessaryRecoveryOrder();
                            }
                        }
                        else
                        {
                            throw new IsNecessaryRecoveryOrder();
                        }
                    }
                    else
                    {
                        throw new IsNecessaryRecoveryCount();
                    }
                }
                else
                {
                    throw new IsNecessaryRecoveryCount();
                }
            }
            catch (IsNecessaryRecoveryCount)
            {
                _logger.LogError("SetOrder error");
                await RecoveryCountToStoreAsync(productsInBasket);
                orderCode = -2;
            }
            catch (IsNecessaryRecoveryOrder)
            {
                _logger.LogError("SetOrder error");
                orderCode = -3;
                _storeContext.OrderPositions.RemoveRange(orderPosition);
                succesAddOrUpdatePositions = await _storeContext.SaveChangesAsync();
                if (succesAddOrUpdatePositions == 0)
                {
                    _logger.LogError("Fail remove order positions");
                    orderCode = -4;
                }
                _storeContext.Orders.Remove(newOrder);
                succesAddOrUpdatePositions = await _storeContext.SaveChangesAsync();
                if (succesAddOrUpdatePositions == 0)
                {
                    _logger.LogError("Fail remove order");
                    orderCode = -5;
                }
                if(!await RecoveryCountToStoreAsync(productsInBasket))
                {
                    _logger.LogError("Fail recovery store");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetOrder error");
                _storeContext.OrderPositions.RemoveRange(orderPosition);
                await _storeContext.SaveChangesAsync();
                _storeContext.Orders.Remove(newOrder);
                await _storeContext.SaveChangesAsync();
                await RecoveryCountToStoreAsync(productsInBasket);
            }
            return orderCode;
        }
        private async Task<bool> SetCountToStoreAsync(List<ProductInBasket> productsInBasket)
        {
            int succesСhanges = -1;
            Dictionary<int, int> productsInBasketIdAndCount = productsInBasket.Select(product => new { product.Id, product.Count })
                .ToDictionary(product => product.Id, product => product.Count);
            var products = await _storeContext.Products.Where(dbproducts => productsInBasketIdAndCount.Keys.Contains(dbproducts.Id)).ToListAsync();
            for (int i = 0; i < products.Count; i++)
            {
                products[i].SetCountToStore(productsInBasketIdAndCount[products[i].Id]);
            }
            try
            {
                _storeContext.Products.UpdateRange(products);
                succesСhanges = await _storeContext.SaveChangesAsync();
                if (succesСhanges == productsInBasketIdAndCount.Count)
                {
                    return true;
                }
                else
                {
                    if (!await RecoveryCountToStoreAsync(productsInBasket))
                    {
                        _logger.LogError("Fail recovery store");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail set count to store ");
                if(!await RecoveryCountToStoreAsync(productsInBasket))
                {
                    _logger.LogError(ex, "Fail recovery store");
                }
                return false;
            }
        }
        private async Task<bool> RecoveryCountToStoreAsync(List<ProductInBasket> productsInBasket)
        {
            int succesСhanges = -1;
            var products = await _storeContext.Products.Where(dbproducts =>
                          productsInBasket.Select(productInBasket => productInBasket.Id).Contains(dbproducts.Id)).ToListAsync();
            for (int i = 0; i < products.Count; i++)
            {
                products[i].RecoveryCountToStore();
            }
            try
            {
                _storeContext.Products.UpdateRange(products);
                succesСhanges = await _storeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail recovery store");
                return false;
            }
            if (succesСhanges == products.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        class IsNecessaryRecoveryCount : Exception
        {
            public IsNecessaryRecoveryCount() : base() { }
        }
        class IsNecessaryRecoveryOrder : Exception
        {
            public IsNecessaryRecoveryOrder() : base() { }
        }
        public async Task<UserCardAndFilters> GetUserOrdersAndOrderPositionsAsync(string userId, int currentPosition)
        {
            UserCardAndFilters result = new UserCardAndFilters();
            var userOrdersAndOrderPositions = await (from orders in _storeContext.Orders
                                                    join ordersPositions in _storeContext.OrderPositions on orders.Id equals ordersPositions.OrderId
                                                    where orders.UserId == userId
                                                    select new {orders.Id,
                                                        orders.OrderStatusId,
                                                        orders.OrderStatus.Status,
                                                        orders.TotalFixPrice,
                                                        orders.DataReservation,
                                                        ordersPositions.OrderId,
                                                        ordersPositions.ProductId,
                                                        productTypeName = ordersPositions.Product.ProductsType.Name,
                                                        ordersPositions.Product.MainPicturePath,
                                                        ordersPositions.Product.Name,
                                                        ordersPositions.Count,
                                                        ordersPositions.FixPrice}).AsNoTracking().ToListAsync();
            if(userOrdersAndOrderPositions.Count>0)
            {
                var userOrders = userOrdersAndOrderPositions.Select(orders => new OrderMini{Id = orders.Id,
                    StatusName = orders.Status,
                    StatusId = orders.OrderStatusId,
                    TotalFixCost = orders.TotalFixPrice,
                    DataReservation = orders.DataReservation,
                    CurrentPosition = currentPosition
                }).ToList();
                userOrders = userOrders.GroupBy(order => order.Id).Select(order => order.First()).ToList();
                result.Orders = userOrders;
                if (userOrders.Count()>0)
                {
                    var ordersPositions = userOrdersAndOrderPositions.Where(preOrdersPositions => userOrders.Select(orders => orders.Id).Contains(preOrdersPositions.OrderId))
                                                                     .Select(preOrdersPositions => new OrderPositionsMini{ OrderId = preOrdersPositions.OrderId,
                                                                         Count = preOrdersPositions.Count,
                                                                         FixPrice = preOrdersPositions.FixPrice,
                                                                         MainPicturePath = preOrdersPositions.MainPicturePath,
                                                                         ProductId = preOrdersPositions.ProductId,
                                                                         ProductName = preOrdersPositions.Name,
                                                                         ProductTypeName = preOrdersPositions.productTypeName}).ToList();
                    result.OrderPositions = ordersPositions;
                }
            }
            return result;
        }
        public async Task<bool> DeAnonOrderAsync(string orderId, string userId)
        {
            if (orderId != null && userId != null)
            {
                try
                {
                    Order order = _storeContext.Orders.Where(u => u.Id == Convert.ToInt32(orderId)).FirstOrDefault();
                    if (order == null)
                    {
                        _logger.LogError("Fail deanon order");
                        return false;
                    }
                    order.UserId = userId;
                    _storeContext.Orders.Update(order);
                    var updateOrderCount = await _storeContext.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fail deanon order");
                    return false;
                }
            }
            return true;
        }
        public async Task<List<OrderMiniAndUserCardMini>> GetUsersOrdersToStorekeeperAsync()
        {
                return await _storeContext.Orders.Select(orders => new OrderMiniAndUserCardMini()
                {
                    Id = orders.Id,
                    UserInfo = new UserCardMini() { Id = orders.UserId },
                    DataReservation = orders.DataReservation,
                    TotalFixCost = orders.TotalFixPrice,
                    StatusName = orders.OrderStatus.Status,
                    StatusId = orders.OrderStatusId
                }).ToListAsync();
        }
        public async Task<List<OrderMiniAndUserCardMini>> GetUsersOrdersToStorekeeperAsync(string[] userIds)
        {
            return await _storeContext.Orders.Where(orders=> userIds.Contains(orders.UserId)).Select(orders => new OrderMiniAndUserCardMini()
            {
                Id = orders.Id,
                UserInfo = new UserCardMini() { Id = orders.UserId },
                DataReservation = orders.DataReservation,
                TotalFixCost = orders.TotalFixPrice,
                StatusName = orders.OrderStatus.Status,
                StatusId = orders.OrderStatusId
            }).ToListAsync();
        }
        public async Task<List<OrderMiniAndUserCardMini>> GetUsersOrdersToStorekeeperAsync(string orderId)
        {
            return await _storeContext.Orders.Where(orders => EF.Functions.Like(orders.Id.ToString(), $"%{orderId}%"))
                                             .Select(orders => new OrderMiniAndUserCardMini()
                                             { 
                                                 Id = orders.Id,
                                                 UserInfo = new UserCardMini() { Id = orders.UserId },
                                                 DataReservation = orders.DataReservation,
                                                 TotalFixCost = orders.TotalFixPrice,
                                                 StatusName = orders.OrderStatus.Status,
                                                 StatusId = orders.OrderStatusId
                                             }).ToListAsync();
        }
        public async Task<bool> EditStProductCard(ProductCardForEdit product)
        {
            Product oldProduct = await _storeContext.Products.Where(p => p.Id == product.Id).FirstOrDefaultAsync();
            if (oldProduct != null)
            {
                try
                {
                    oldProduct.Price = product.Price;
                    oldProduct.AvailableMaxCountOnRequest = product.AvailableMaxCountOnRequest;
                    oldProduct.AvailableOnRequest = product.AvailableOnRequest;
                    oldProduct.CountToStore = product.CountToStore;
                    oldProduct.DeliveryToStoreDay = product.DeliveryToStoreDay;
                    oldProduct.Sale = product.Sale;
                    oldProduct.SalePrice = product.SalePrice;
                    _storeContext.Products.Update(oldProduct);
                    int countUpdate = await _storeContext.SaveChangesAsync();
                    if (countUpdate == 0)
                    {
                        _logger.LogError("Fail edit product card");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fail edit product card");
                    return false;
                }
            }
            return false;
        }
        public async Task<List<OrderPositionsMini>> GetOrderPositionsMiniAsync(int orderId)
        {
            return await (from orderPosition in _storeContext.OrderPositions
                          join productCard in _storeContext.Products on orderPosition.ProductId equals productCard.Id
                          where orderPosition.OrderId == orderId
                          select new OrderPositionsMini()
                          {
                            ProductId = productCard.Id,
                            ProductName = productCard.Name,
                            Count = orderPosition.Count,
                            FixPrice = orderPosition.FixPrice,
                            ProductTypeName = productCard.ProductsType.Name,
                            MainPicturePath = productCard.MainPicturePath
                          }).ToListAsync();

        }
        public async Task<bool> EditStatusOrder(UpdateStatus order)
        {
            Order oldOrder = await _storeContext.Orders.Where(o => o.Id == order.IdOrder).FirstOrDefaultAsync();
            if (oldOrder != null)
            {
                try 
                { 
                    oldOrder.OrderStatusId = order.IdNewStatus;
                    _storeContext.Orders.Update(oldOrder);
                    int countUpdate = await _storeContext.SaveChangesAsync();
                    if (countUpdate > 0)
                    {
                        if (order.IdNewStatus == 4)
                        {
                            var orderPosiions = await _storeContext.OrderPositions.Where(op => op.OrderId == order.IdOrder).ToListAsync();
                            var productIdInOrder = orderPosiions.Select(op => op.ProductId).ToList();
                            var products = await _storeContext.Products.Where(pr => productIdInOrder.Contains(pr.Id)).ToListAsync();
                            foreach (var item in products)
                            {
                                item.AvailableMaxCountOnRequest += orderPosiions.Where(op => op.ProductId == item.Id).Select(op => op.Count).FirstOrDefault();
                            }
                            _storeContext.Products.UpdateRange(products);
                            await _storeContext.SaveChangesAsync();
                        }
                        return true;
                    }
                    else
                    {
                        _logger.LogError("Fail edit status order");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fail edit status order");
                    return false;
                }
            }
            return false;
        }
        public async Task<List<ProductsAttribute>> GetAllProductsAttributesAsync()
        {
            return await _storeContext.ProductsAttributes.Skip(1).ToListAsync();//1 - default category, not edit
        }
        public async Task<bool> AddProductsAttributeAsync(ProductsAttribute productsAttribute)
        {
            try
            {
                _storeContext.ProductsAttributes.Add(productsAttribute);
                int addCount = await _storeContext.SaveChangesAsync();
                if (addCount > 0)
                {
                    return true;
                }
                _logger.LogError("Fail add products attributes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail add products attributes");
                return false;
            }
            return false;
        }
        public async Task<ProductsAttribute> GetProductsAttributeAsync(int id)
        {
            return await _storeContext.ProductsAttributes.Where(u => u.Id == id).FirstOrDefaultAsync();
        }
        public async Task<bool> EditProductsAttributeAsync(ProductsAttribute productsAttribute)
        {
            try
            {
                _storeContext.ProductsAttributes.Update(productsAttribute);
                int removeCount = await _storeContext.SaveChangesAsync();
                if (removeCount > 0)
                {
                    return true;
                }
                _logger.LogError("Fail edit product attributes");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Object has already been modified.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail edit products attributes");
                return false;
            }
            return false;
        }
        public async Task<bool> DeleteProductsAttributeAsync(ProductsAttribute productsAttribute)
        {
            try
            {
                _storeContext.ProductsAttributes.Remove(productsAttribute);
                int removeCount = await _storeContext.SaveChangesAsync();
                if (removeCount > 0)
                {
                    return true;
                }
                _logger.LogError("Fail delete products attributes");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Object has already been modified.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail delete products attributes");
                return false;
            }
            return false;
        }
        public async Task<List<ProductTypeMini>> GetParentProductTypes()
        {
            return await _storeContext.ProductsTypes.Where(preProductTypes => preProductTypes.ParentProductTypeId == 1)
                         .Select(preProductTypes=> new ProductTypeMini() {Id= preProductTypes.Id, Name = preProductTypes.Name }).ToListAsync();
        }
        public async Task<bool> AddProductsTypeAsync(ProductsType productsType)
        {
            try
            {
                _storeContext.ProductsTypes.Add(productsType);
                _storeContext.AttributesToProductTypes.Add(new AttributesToProductType() { ProductsTypeId = productsType.Id, ProductsAttributeId = 1 });
                int addCount = await _storeContext.SaveChangesAsync();
                if (addCount > 0)
                {
                    return true;
                }
                _logger.LogError("Fail add products type");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail add products type");
                return false;
            }
            return false;
        }
        public async Task<ProductTypeMini> GetProductsTypeAsync(int id)
        {
            return await _storeContext.ProductsTypes.Where(pt => pt.Id == id)
                                      .Select(pt=> new ProductTypeMini() {
                                          Id=pt.Id,
                                          Name=pt.Name,
                                          HEXColor=pt.HexColor,
                                          ParentProductTypeId=pt.ParentProductTypeId
                                      }).FirstOrDefaultAsync();
        }
        public async Task<bool> EditParentProductTypeToNoParent(ProductsType productsType)
        {
            try
            {
                ProductsType newParent = await _storeContext.ProductsTypes.Where(table => table.Id == 2).FirstAsync();
                var childs = await _storeContext.ProductsTypes.Where(table => table.ParentProductType.Id == Convert.ToInt32(productsType.Id)).ToListAsync();
                int countChilds = childs.Count();
                foreach (var item in childs)
                {
                    item.ParentProductType = newParent;
                }
                _storeContext.ProductsTypes.UpdateRange(childs);
                if (await _storeContext.SaveChangesAsync() < countChilds)
                {
                    _logger.LogError("Fail edit parent product type to not parent");
                    return false;
                }

                return await EditProductType(productsType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail edit parent product type to not parent");
                return false;
            }
        }
        public async Task<bool> EditProductTypeToParentProductType(ProductsType productsType)
        {
            try
            {
                ProductsType newParent = await _storeContext.ProductsTypes.Where(table => table.Id == 2).FirstAsync();
                var childs = await _storeContext.Products.Where(table => table.ProductsTypeId == Convert.ToInt32(productsType.Id)).ToListAsync();
                int countChilds = childs.Count();
                foreach (var item in childs)
                {
                    item.ProductsType = newParent;
                }
                _storeContext.Products.UpdateRange(childs);
                if (await _storeContext.SaveChangesAsync() < countChilds)
                {
                    _logger.LogError("Fail edit product type to parent type");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail edit product type to parent type");
                return false;
            }
            return await EditProductType(productsType);
        }
        public async Task<bool> EditProductType(ProductsType productsType)
        {
            try
            {
                _storeContext.ProductsTypes.Update(productsType);
                if (await _storeContext.SaveChangesAsync() > 0)
                {
                    return true;
                }
                _logger.LogError("Fail edit product type");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail edit product type");
                return false;
            }
        }
        public async Task<bool> DeleteProductTypeAsync(ProductsType productsType)
        {
            try
            {
                int updateCount = 0;
                var productTypes = await GetProductTypeAsync();
                var childProductType = productTypes.Where(pt => pt.ParentProductTypeId == productsType.Id).ToList();
                ProductsType newParent = productTypes.Where(table => table.Id == 2).First();
                foreach (var item in childProductType)
                {
                    item.ParentProductType = newParent;
                }
                _storeContext.ProductsTypes.UpdateRange(childProductType);
                updateCount = await _storeContext.SaveChangesAsync();
                if (updateCount < childProductType.Count)
                {
                    _logger.LogError("Fail delete product type");
                    return false;
                }
                var childProduct = _storeContext.Products.Where(products => products.ProductsTypeId == productsType.Id).ToList();
                foreach (var item in childProduct)
                    item.ProductsType = newParent;
                _storeContext.Products.UpdateRange(childProduct);
                updateCount = await _storeContext.SaveChangesAsync();
                if (updateCount < childProduct.Count)
                {
                    _logger.LogError("Fail delete product type");
                    return false;
                }
                _storeContext.ProductsTypes.Remove(productsType);
                updateCount = await _storeContext.SaveChangesAsync();
                if (updateCount == 0)
                {
                    _logger.LogError("Fail delete product type");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail delete product type");
                return false;
            }
        }
        public async Task<List<ProductsAttribute>> GetProductsAttributesAsync()
        {
            return await _storeContext.ProductsAttributes.Skip(1).ToListAsync();//1 - default attribute
        }
        public async Task<List<ProductTypesAttributeCheck>> GetAttributesToProductsTypesChecksAsync(List<ProductsAttribute> allProductsAttrubutes, int idProductType, int? idParentProductType)
        {
            List<ProductTypesAttributeCheck> attributesToProductTypesAndParentPT = await _storeContext.AttributesToProductTypes
                                                                                .Where(attribute => attribute.ProductsAttributeId != 1 && (attribute.ProductsTypeId == idProductType ||
                                                                                attribute.ProductsTypeId == idParentProductType)).
                                                                                Select(attribute => new ProductTypesAttributeCheck()
                                                                                {
                                                                                    AttributeId = attribute.ProductsAttributeId,
                                                                                    AttributeName = attribute.ProductsAttribute.Name,
                                                                                    ProdyctTypeId = attribute.ProductsTypeId
                                                                                }).ToListAsync();

            List<ProductTypesAttributeCheck> attributesToProductTypes = attributesToProductTypesAndParentPT
                                            .Where(attribute => attribute.ProdyctTypeId == idProductType)
                                            .Select(attribute => new ProductTypesAttributeCheck()
                                            {
                                                AttributeId = attribute.AttributeId,
                                                AttributeName = attribute.AttributeName,
                                                ProdyctTypeId = attribute.ProdyctTypeId,
                                                ParentAttribute = false,
                                                Check = true
                                            }).ToList();
            List<ProductTypesAttributeCheck> attributesToParentPT = attributesToProductTypesAndParentPT
                                            .Where(attribute => attribute.ProdyctTypeId == idParentProductType)
                                            .Select(attribute => new ProductTypesAttributeCheck()
                                            {
                                                AttributeId = attribute.AttributeId,
                                                AttributeName = attribute.AttributeName,
                                                ProdyctTypeId = attribute.ProdyctTypeId,
                                                ParentAttribute = true,
                                                Check = true
                                            }).ToList();
            var idNoSelectionAttributes = allProductsAttrubutes.Select(a => a.Id)
                                                                .Except(attributesToProductTypes.Select(a => a.AttributeId).Union(attributesToParentPT.Select(a => a.AttributeId))).ToList();
            List<ProductTypesAttributeCheck> NoSelectionAttributes = allProductsAttrubutes.Where(a => idNoSelectionAttributes.Contains(a.Id))
                                                                         .Select(a => new ProductTypesAttributeCheck()
                                                                         {
                                                                             AttributeId = a.Id,
                                                                             AttributeName = a.Name,
                                                                             ProdyctTypeId = null,
                                                                             ParentAttribute = false,
                                                                             Check = false
                                                                         }).ToList();
            return attributesToProductTypes.Concat(attributesToParentPT).Concat(NoSelectionAttributes).ToList();
        }
        public async Task<bool> SetAttributesToProductsTypesChecksAsync(int idProductType, 
                                                                        int? idParentProductType, 
                                                                        List<int> attributesToProductsTypesIdCheck)
        {
            List<AttributesToProductType> doubleAttribytesChildType = new List<AttributesToProductType>();
            List<AttributesToProductType> attribytesTypeAndPT = new List<AttributesToProductType>();
            try
            {
                if (idParentProductType == 1)//если я родитель то
                {
                    List<int> childProductType = new List<int>();
                    //child product type
                    childProductType = await _storeContext.ProductsTypes.Where(productType => productType.ParentProductTypeId == (int)idProductType).Select(productType => productType.Id).ToListAsync();
                    //attributes to this type and child type
                    attribytesTypeAndPT = await _storeContext.AttributesToProductTypes.AsNoTracking().Where(a => (childProductType.Contains(a.ProductsTypeId) || a.ProductsTypeId == idProductType) && a.ProductsAttributeId != 1).ToListAsync();
                    //delete doubles attribute to child
                    doubleAttribytesChildType = attribytesTypeAndPT.Where(a => childProductType.Contains(a.ProductsTypeId)
                                                                                                        && attributesToProductsTypesIdCheck.Contains(a.ProductsAttributeId)).ToList();
                    _storeContext.AttributesToProductTypes.RemoveRange(doubleAttribytesChildType);
                    if (await _storeContext.SaveChangesAsync() < doubleAttribytesChildType.Count)
                    {
                        _logger.LogError("Fail set attributes to productsTypes");
                        return false;
                    }
                }
                List<int> oldAttributesToProductsTypesId = new List<int>();
                if (attribytesTypeAndPT.Count == 0)
                {
                    oldAttributesToProductsTypesId = await _storeContext.AttributesToProductTypes.AsNoTracking().Where(a => a.ProductsTypeId == idProductType && a.ProductsAttributeId != 1).Select(a => a.ProductsAttributeId).ToListAsync();
                }
                else
                {
                    oldAttributesToProductsTypesId = attribytesTypeAndPT.Where(a => a.ProductsTypeId == idProductType).Select(a => a.ProductsAttributeId).ToList();
                }
                var newAttributesToProductsTypesId = attributesToProductsTypesIdCheck.Except(oldAttributesToProductsTypesId).ToList();
                var delAttributesToProductsTypesId = oldAttributesToProductsTypesId.Except(attributesToProductsTypesIdCheck).ToList();
                List<AttributesToProductType> newAttributesToProductsTypes = new List<AttributesToProductType>();
                List<AttributesToProductType> delAttributesToProductsTypes = new List<AttributesToProductType>();
                List<AttributesToProduct> delAttributesToProducts = new List<AttributesToProduct>();
                var ProductsAndArttributesWithDelete = await
                        (from product in _storeContext.Products
                         join attributes in _storeContext.AttributesToProducts on product.Id equals attributes.ProductId
                         where (product.ProductsTypeId == idProductType || product.ProductsType.ParentProductTypeId == idProductType)
                         && delAttributesToProductsTypesId.Contains(attributes.ProductsAttributeId)
                         select new { product.Id, attributes.ProductsAttributeId }).ToListAsync();
                foreach (var item in newAttributesToProductsTypesId)
                {
                    newAttributesToProductsTypes.Add(new AttributesToProductType() { ProductsTypeId = (int)idProductType, ProductsAttributeId = item });
                }
                foreach (var item in delAttributesToProductsTypesId)
                {
                    delAttributesToProductsTypes.Add(new AttributesToProductType() { ProductsTypeId = (int)idProductType, ProductsAttributeId = item });
                }
                foreach (var item in ProductsAndArttributesWithDelete)
                {
                    delAttributesToProducts.Add(new AttributesToProduct() { ProductId = item.Id, ProductsAttributeId = item.ProductsAttributeId });
                }
                _storeContext.AttributesToProductTypes.AddRange(newAttributesToProductsTypes);
                _storeContext.AttributesToProductTypes.RemoveRange(delAttributesToProductsTypes);
                _storeContext.AttributesToProducts.RemoveRange(delAttributesToProducts);
                if (await _storeContext.SaveChangesAsync() == 0)
                {
                    _logger.LogError("Fail set attributes to productsTypes");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail set attributes to productsTypes");
                return false;
            }
        }
        public async Task<List<ProductTypeMini>> GetNoParentPTtoSelectListAsync()
        {
           return await _storeContext.ProductsTypes.Where(preProductTypes => preProductTypes.ParentProductTypeId != 2 && preProductTypes.ParentProductTypeId != 1)
                .Select(pt => new ProductTypeMini() { Id = pt.Id, Name = pt.Name, ParentProductTypeName = pt.ParentProductType.Name }).ToListAsync();

        }
        public async Task<bool> AddProductsAsync(Product product, IFormFile uploadedFile)
        {
            try
            {
                if (!product.Sale)
                {
                    product.SalePrice = product.Price;
                }
                _storeContext.Products.Add(product);
                int countUpdate = await _storeContext.SaveChangesAsync();
                if (uploadedFile != null)
                {
                    var typeAndParentTypeNames = await _storeContext.Products.Where(products => products.Id == product.Id)
                                                                       .Select(products => new { typeName = products.ProductsType.Name, parentTypeName = products.ProductsType.ParentProductType.Name }).FirstAsync();
                    product.MainPicturePath = await _fileOperations.SavePictureAsync(typeAndParentTypeNames.parentTypeName, typeAndParentTypeNames.typeName, product.Name, uploadedFile);
                }
                countUpdate = await _storeContext.SaveChangesAsync();
                _storeContext.AttributesToProducts.Add(new AttributesToProduct() { Product = product, ProductsAttributeId = 1, Value = "" });
                _storeContext.ProductsPicturesPaths.Add(new ProductsPicturesPath() { Product = product, NPicture = 0, PicturePath = "" });
                countUpdate = await _storeContext.SaveChangesAsync();
                if (countUpdate == 0)
                {
                    _logger.LogError("Fail add product");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail add product");
                return false;
            }
        }
        public async Task<Product> GetProductAsync(int id)
        {
            return await _storeContext.Products.Where(u => u.Id == id).FirstAsync();
        }
        public async Task<bool> EditProductsAsync(Product product, IFormFile uploadedFile)
        {
            try
            {
                if (uploadedFile != null)
                {
                    var typeAndParentTypeNames = await _storeContext.Products.Where(products => products.Id == product.Id)
                                                                       .Select(products => new
                                                                       {
                                                                           typeName = products.ProductsType.Name,
                                                                           parentTypeName = products.ProductsType.ParentProductType.Name
                                                                       }).FirstAsync();
                    product.MainPicturePath = await _fileOperations.SavePictureAsync(typeAndParentTypeNames.parentTypeName, typeAndParentTypeNames.typeName, product.Name, uploadedFile);
                }
                if (!product.Sale)
                {
                    product.SalePrice = product.Price;
                }
                _storeContext.Products.Update(product);
                if (await _storeContext.SaveChangesAsync() == 0)
                {
                    _logger.LogError("Fail edit product");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail edit product");
                return false;
            }
        }
        public async Task<List<AttributesToProductType>> GetAttributesToProductTypesAsync()
        {
            return await _storeContext.AttributesToProductTypes.Where(table => table.ProductsAttributeId != 1).ToListAsync();
        }
        public async Task<List<AttributesToProduct>> GetAttributesToProductAsync()
        {
            return await _storeContext.AttributesToProducts.Where(table => table.ProductsAttributeId != 1).ToListAsync();
        }
        public async Task<List<AttributesToProduct>> GetAttributesToProductAsync(List<AttributesToProductType> attributesToProductTypes, 
                                                                                 List<AttributesToProduct> productsAttributes,
                                                                                int idProduct, int idProductType, int idParentProductType)
        {
            List<int> allAttributesToProductType = attributesToProductTypes
                .Where(table => (table.ProductsTypeId == idParentProductType || table.ProductsTypeId == idProductType))
                .Select(u => u.ProductsAttributeId).ToList();

            List<int> oldAttributesToProductType = productsAttributes
                .Where(u => u.ProductId == idProduct)
                .Select(u => u.ProductsAttributeId).ToList();

            var newAttributesToProductType = allAttributesToProductType.Except(oldAttributesToProductType);
            if (newAttributesToProductType.Count() != 0)
            {
                foreach (var item in newAttributesToProductType)
                    _storeContext.AttributesToProducts.Add(new AttributesToProduct() { ProductId = idProduct, ProductsAttributeId = item, Value = "" });
                await _storeContext.SaveChangesAsync();
            }
            return await _storeContext.AttributesToProducts.Where(u => u.ProductId == idProduct && allAttributesToProductType.Contains(u.ProductsAttributeId))
                                                             .Include(t => t.ProductsAttribute).ToListAsync();
        }
        public async Task<bool> SetAttributesToProductAsync(int productId, List<AttributesToProduct> value)
        {
            try
            {
                value.ForEach(m => { m.ProductId = productId; });
                _storeContext.AttributesToProducts.UpdateRange(value);
                if (await _storeContext.SaveChangesAsync() > value.Count)
                {
                    _logger.LogError("Fail set attributes to product");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail set attributes to product");
                return false;
            }
        }
        public async Task<ProductAndOrders> ProductAndOrdersAsync(int id)
        {
            ProductAndOrders result = new ProductAndOrders();
            var preResult = await (from products in _storeContext.Products
                                   join orderPositions in _storeContext.OrderPositions on products.Id equals orderPositions.ProductId
                                   join orderss in _storeContext.Orders on orderPositions.OrderId equals orderss.Id
                                   where products.Id == id && (orderss.OrderStatusId != 3 && orderss.OrderStatusId != 4)
                                   select new { products.Id, products.Name, tName = products.ProductsType.Name, ptName = products.ProductsType.ParentProductType.Name, orderId = orderss.Id }
                         ).ToListAsync();
            if (preResult.Count != 0)
            {
                var orders = preResult.Select(or => or.orderId).Distinct().ToList();
                result.AllowedDelete = false;
                result.Orders = new List<int>();
                result.Orders = orders;
                result.Id = id;
                result.Name = preResult.Select(pr => pr.Name).FirstOrDefault();
                result.ProductTypeName = (preResult.Select(pr => new { typename = $"{pr.ptName} - {pr.tName}" }).FirstOrDefault()).typename;
            }
            else
            {
                result = await _storeContext.Products.Where(pr => pr.Id == id)
                    .Select(pr => new ProductAndOrders()
                    {
                        Id = pr.Id,
                        AllowedDelete = true,
                        Name = pr.Name,
                        ProductTypeName = $"{pr.ProductsType.ParentProductType.Name} - {pr.ProductsType.Name}"
                    }).FirstOrDefaultAsync();
            }
            return result;
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _storeContext.Products.Where(products => products.Id == id).FirstOrDefaultAsync();
                _storeContext.Products.Remove(product);
                if (await _storeContext.SaveChangesAsync() > 0)
                {
                    return true;
                }
                _logger.LogError("Fail delete product");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail delete product");
                return false;
            }
        }
        public async Task<List<ProductsPicturesPath>> GetProductsPicturesPathsAsync(int id)
        {
            var result = await _storeContext.ProductsPicturesPaths.Where(u => u.ProductId == id).AsNoTracking().ToListAsync();
            for (int i = result.Count; i < 3; i++)
            {
                result.Add(new ProductsPicturesPath() { ProductId = (int)id, NPicture = i });
                _storeContext.ProductsPicturesPaths.Add(new ProductsPicturesPath() { ProductId = (int)id, NPicture = i });
                await _storeContext.SaveChangesAsync();
            }
            return result;
        }
        public async Task<bool> AddProductsPicturesPathsAsync(int id, IFormFile uploadedFiles1, IFormFile uploadedFiles2, IFormFile uploadedFiles3)
        {
            try
            {
                List<IFormFile> uploadedFiles = new List<IFormFile>
            {
                uploadedFiles1,
                uploadedFiles2,
                uploadedFiles3
            };
                int updateCount = 0;
                List<ProductsPicturesPath> updatePictures = new List<ProductsPicturesPath>();
                var typeAndParentTypeNames = await _storeContext.Products.Where(products => products.Id == id)
                                                       .Select(products => new { name = products.Name, typeName = products.ProductsType.Name, parentTypeName = products.ProductsType.ParentProductType.Name }).FirstAsync();
                for (int i = 0; i < uploadedFiles.Count; i++)
                {
                    if (uploadedFiles[i] != null)
                    {
                        updateCount++;
                        updatePictures.Add(new ProductsPicturesPath()
                        {
                            ProductId = id,
                            NPicture = i,
                            PicturePath = await _fileOperations.SavePictureAsync(typeAndParentTypeNames.parentTypeName, typeAndParentTypeNames.typeName, typeAndParentTypeNames.name, uploadedFiles[i])
                        });
                    }
                }
                _storeContext.ProductsPicturesPaths.UpdateRange(updatePictures);
                if (await _storeContext.SaveChangesAsync() < updateCount)
                {
                    _logger.LogError("Fail add pictures");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail add pictures");
                return false;
            }
        }
    }
}
