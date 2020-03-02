using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.Models;
using MyFirstStore.ViewModels;
using MyFirstStore.ViewModels.Storekeeper;

namespace MyFirstStore.Services
{
    public class DataProcessingConveyor
    {
        private readonly FcdStoreContext _fcdStoreContext;
        private readonly CacheService _cacheService;
        private readonly DataFiltering _dataFiltering;
        private readonly PreOrderService _preOrderService;
        private readonly ManagerBasketCook _managerBasketCook;
        private readonly BuilderBackColorCSS _builderBackColorCSS;
        private readonly Array searchEnumArray,
                               searchEnumArrayOrders,
                               searchEnumArrayProducts,
                               sortEnumArrayProductAttributes,
                               sortEnumArray,
                               sortEnumArrayUserOrder,
                               sortEnumArrayUsers,
                               sortEnumArrayOrders,
                               sortEnumArrayProducts;
        private readonly Type typeWhereSearchProduct,
                              typeWhereSearchOrders,
                              typeWhereSearchProductsSt;
        private readonly FcdUserAndSignManager _fcdUserAndSignManager;
        public DataProcessingConveyor(FcdStoreContext fcdStoreContext,
                                      CacheService cacheService,
                                      DataFiltering dataFiltering,
                                      ManagerBasketCook managerBasketCook,
                                      PreOrderService preOrderService,
                                      FcdUserAndSignManager fcdUserAndSignManager,
                                      BuilderBackColorCSS builderBackColorCSS)
        {
            _fcdStoreContext = fcdStoreContext;
            _cacheService = cacheService;
            _preOrderService = preOrderService;
            _dataFiltering = dataFiltering;
            _managerBasketCook = managerBasketCook;
            _fcdUserAndSignManager = fcdUserAndSignManager;
            _builderBackColorCSS = builderBackColorCSS;
            searchEnumArray = Enum.GetValues(typeof(WhereSearch));
            searchEnumArrayOrders = Enum.GetValues(typeof(WhereSearchOrders));
            searchEnumArrayProducts = Enum.GetValues(typeof(StProductSearchs));
            sortEnumArrayProductAttributes = Enum.GetValues(typeof(IdNameSort));
            sortEnumArray = Enum.GetValues(typeof(ProductsListSort));
            sortEnumArrayUserOrder = Enum.GetValues(typeof(OrderSort));
            sortEnumArrayUsers = Enum.GetValues(typeof(IdNameUserSort));
            sortEnumArrayOrders = Enum.GetValues(typeof(UserOrders));
            sortEnumArrayProducts = Enum.GetValues(typeof(IdNameTypeSort));
            typeWhereSearchProduct = typeof(WhereSearch);
            typeWhereSearchOrders = typeof(WhereSearchOrders);
            typeWhereSearchProductsSt = typeof(StProductSearchs);
        }
        private async Task<List<ProductInBasket>> GetProductsInBasketAsync()
        {
            List<ProductInBasket> result = new List<ProductInBasket>();
            Dictionary<int, int> productsInCook = await _managerBasketCook.GetDictionaryProductsForBasketCookAsync() ?? null;
            if (productsInCook != null)
            {
                var prodyctIds = productsInCook.Keys.ToList();
                for (int i = 0; i < prodyctIds.Count; i++)
                {
                    var cacheresult = _cacheService.Get<ProductInBasket>($"ProductInBasket{prodyctIds[i]}");
                    if (cacheresult != null)
                    {
                        result.Add(cacheresult);
                        prodyctIds.RemoveAt(i);
                    }
                }
                if (prodyctIds.Count > 0)
                {
                    List<ProductInBasket> dbresult = await _fcdStoreContext.GetProductInBasketsAsync(prodyctIds);
                    for (int i = 0; i < dbresult.Count; i++)
                    {
                        result.Add(dbresult[i]);
                        _cacheService.Set<ProductInBasket>($"ProductInBasket{dbresult[i].Id}", dbresult[i]);
                    }
                }
                for (int i = 0; i < result.Count; i++)
                {
                    result[i].Count = productsInCook[result[i].Id];
                }
            }
            return result;
        }
        public async Task<ProductTypeMiniAndBasket> GetProductTypeMIniAndBasketAsync()
        {
            ProductTypeMiniAndBasket result = new ProductTypeMiniAndBasket();
            result.SearchEnumArray = searchEnumArray;
            string keyName = $"ProductTypeMini";
            List<ProductTypeMini> productTypeMinis = _cacheService.Get<List<ProductTypeMini>>(keyName);
            if (productTypeMinis == null)
            {
                productTypeMinis = await _fcdStoreContext.GetProductTypeMinisAsync();
                _cacheService.Set<List<ProductTypeMini>>(keyName, productTypeMinis);
            }
            result.ProductTypeMinis = productTypeMinis;
            result.ProductsInBasket = await GetProductsInBasketAsync();
            result.SetBasketsCost();
            return result;
        }
        public async Task<ProductCardAndBasketAndFilters> GetProductCardAndBasketAndFiltersAsync(FilterProductCard filters)
        {
            ProductCardAndBasketAndFilters result = new ProductCardAndBasketAndFilters
            {
                SearchEnumArray = searchEnumArray,
                SortEnumArray = sortEnumArray,
                Filters = filters
            };
            string keyName = $"ProductCards:{filters.GetFilterName()}";
            List<ProductCard> productCards = _cacheService.Get<List<ProductCard>>(keyName);
            if (productCards == null)
            {
                productCards = await _fcdStoreContext.GetProductCardsAsync(filters.SelectFromSelectList, (int)filters.CurrentPosition);
                result.ProductCards = productCards;
                _dataFiltering.Filtering(result);
                productCards = result.ProductCards;
                _cacheService.Set<List<ProductCard>>(keyName, result.ProductCards);
            }
            result.ProductCards = productCards;
            _dataFiltering.SetFilters(result);
            result.ProductCards = _dataFiltering.Paginator<ProductCard>(result.ProductCards, (int)filters.CountVisablePositions, (int)filters.CurrentPosition);
            result.ProductsInBasket = await GetProductsInBasketAsync();
            result.SetBasketsCost();
            return result;
        }
        public async Task<BigProductCardAndBasket> GetBigProductCardAndBasketAsync(int id)
        {
            BigProductCardAndBasket result = new BigProductCardAndBasket();
            string keyName = $"BigProductCards:{id}";
            BigProductCard bigProductCard = _cacheService.Get<BigProductCard>(keyName);
            if (bigProductCard == null)
            {
                bigProductCard = await _fcdStoreContext.GetBigProductCard(id);
                _cacheService.Set<BigProductCard>(keyName, bigProductCard);
            }
            result.BigProductCard = bigProductCard;
            result.ProductsInBasket = await GetProductsInBasketAsync();
            result.SetBasketsCost();
            return result;
        }
        public async Task<ProductCardAndBasketAndFilters> GetFilteringProductCardAndBasketAndFiltersAsync(FilterProductCard filters)
        {
            ProductCardAndBasketAndFilters result = new ProductCardAndBasketAndFilters
            {
                SearchEnumArray = searchEnumArray,
                SortEnumArray = sortEnumArray,
                Filters = filters
            };
            int currentPosition = (int)filters.CurrentPosition;
            string keyName = $"ProductCardsForSearch:{filters.GetFilterName()}",
                   desired = filters.Desired.ToLower();
            List<ProductCard> productCards = _cacheService.Get<List<ProductCard>>(keyName);
            if (productCards == null)
            {
                switch (Enum.Parse(typeWhereSearchProduct, filters.WhereSearch))
                {
                    case (WhereSearch.По_названии):
                        {
                            productCards = await _fcdStoreContext.GetProductCardsSearchNameAsync(desired, currentPosition);
                            break;
                        }
                    case (WhereSearch.В_атрибутах):
                        {
                            productCards = await _fcdStoreContext.GetProductCardsSearchAttributesAsync(desired, currentPosition);
                            break;
                        }
                    case (WhereSearch.В_описании):
                        {
                            productCards = await _fcdStoreContext.GetProductCardsSearchDescriptionsAsync(desired, currentPosition);
                            break;
                        }
                    case (WhereSearch.Везде):
                        {
                            productCards = await _fcdStoreContext.GetProductCardsSearchAllAsync(desired, currentPosition);
                            break;
                        }
                    default:
                        {
                            productCards = await _fcdStoreContext.GetProductCardsSearchNameAsync(desired, currentPosition);
                            break;
                        }
                }
                result.ProductCards = productCards;
                _dataFiltering.Filtering(result);
                productCards = result.ProductCards;
                _cacheService.Set<List<ProductCard>>(keyName, productCards);
            }
            result.ProductCards = productCards;
            _dataFiltering.SetFilters(result);
            result.ProductCards = _dataFiltering.Paginator<ProductCard>(result.ProductCards, (int)filters.CountVisablePositions, currentPosition);
            result.ProductsInBasket = await GetProductsInBasketAsync();
            result.SetBasketsCost();
            return result;
        }
        public async Task<PreOrder> GetPreOrderAsync()
        {
            PreOrder result = new PreOrder();
            Dictionary<int, int> productsInCook = await _managerBasketCook.GetDictionaryProductsForBasketCookAsync() ?? null;
            if (productsInCook != null && productsInCook.Count > 0)
            {
                List<ProductInBasketAndPreOrder> productInBasketAndPreOrder = await _fcdStoreContext.GetProductInBasketsPreOrderAsync(productsInCook.Keys.ToList());
                if (productInBasketAndPreOrder != null && productInBasketAndPreOrder.Count > 0)
                {
                    for (int i = 0; i < productInBasketAndPreOrder.Count; i++)
                    {
                        productInBasketAndPreOrder[i].Count = productsInCook[productInBasketAndPreOrder[i].Id];
                    }
                    productInBasketAndPreOrder = _preOrderService.CreatePreOrder(productInBasketAndPreOrder);
                    result.OrderDate = _preOrderService.GetOrderData(productInBasketAndPreOrder);
                    result.Approve = _preOrderService.ApprovePreOrder(productInBasketAndPreOrder);
                    result.ProductInBasket = _preOrderService.AlleviationProductInBasket(productInBasketAndPreOrder);
                    result.SetBasketCost();
                }
            }
            return result;
        }
        public async Task<int> GetOrderCodeAsync(string userId, string oldPreOrderState)
        {
            int orderCode = -1;
            var preOrder = await GetPreOrderAsync();
            string newPreOrderState = JsonConvert.SerializeObject(preOrder);
            if (newPreOrderState == oldPreOrderState)
            {
                orderCode = await _fcdStoreContext.SetOrderAsync(preOrder.ProductInBasket, preOrder.OrderDate, userId);
                if (orderCode > 0)
                {
                    await _managerBasketCook.DeleteBasketCook();
                }
            }
            return orderCode;
        }
        public async Task<UserCardAndFilters> GetUserCardAndFiltersAsync(string userName, FilterOrders filters)
        {
            UserCardAndFilters result = new UserCardAndFilters
            {
                Filters = filters,
                SortEnumArray = sortEnumArrayUserOrder
            };
            string keyName = $"UserCardAndFilters{userName}{filters.GetFilterName()}";
            var cacheResult = _cacheService.Get<UserCardAndFilters>(keyName);
            if (cacheResult == null)
            {
                cacheResult = new UserCardAndFilters
                {
                    Filters = filters,
                    SortEnumArray = sortEnumArrayUserOrder
                };
                User user = await _fcdUserAndSignManager.GetUserAsync(userName);
                cacheResult.SetUserCardMini(user);
                if (user != null)
                {
                    UserCardAndFilters userOrderAndOrderPositions = await _fcdStoreContext.GetUserOrdersAndOrderPositionsAsync(user.Id, (int)filters.CurrentPosition);
                    cacheResult.Orders = userOrderAndOrderPositions.Orders;
                    cacheResult.OrderPositions = userOrderAndOrderPositions.OrderPositions;
                }
                _dataFiltering.Filtering(cacheResult);
                _cacheService.Set<UserCardAndFilters>(keyName, cacheResult);
            }
            result.Id = cacheResult.Id;
            result.Name = cacheResult.Name;
            result.Email = cacheResult.Email;
            result.Orders = cacheResult.Orders;
            result.OrderPositions = cacheResult.OrderPositions;
            _dataFiltering.SetFilters(result);
            result.Orders = _dataFiltering.Paginator<OrderMini>(result.Orders, (int)filters.CountVisablePositions, (int)filters.CurrentPosition);
            return result;
        }
        public async Task<IdentityResult> RegisterUserAsync(UserRegister userRegisterInfo)
        {
            IdentityResult result = await _fcdUserAndSignManager.AddUserAsync(userRegisterInfo);
            if (result.Succeeded)
            {
                User user = await _fcdUserAndSignManager.GetUserAsync(userRegisterInfo.Name);
                if (user != null)
                {
                    await _fcdStoreContext.DeAnonOrderAsync(userRegisterInfo.OrderId, user.Id);
                }
                _cacheService.Remove("UserList");
            }
            return result;
        }
        public async Task<SignInResult> LoginUserAsync(UserLogin userLoginInfo)
        {
            return await _fcdUserAndSignManager.GetPasswordSignInrAsync(userLoginInfo);
        }
        public async Task<bool> Logout()
        {
            await _fcdUserAndSignManager.Logout();
            return true;
        }
        public async Task<UserForEdit> GetUserForEditAsync(string userId)
        {
            return await _fcdUserAndSignManager.GetUserForEditAsync(userId);
        }
        public async Task<bool> DeleteUserAsync(string userId)
        {
            bool result = await _fcdUserAndSignManager.DeleteUserAsync(userId);
            if (result)
            {
                _cacheService.Remove("UserList");
            }
            return result;
        }
        public async Task<UserChangePassword> GetUserForChangePasswordAsync(string userId)
        {
            return await _fcdUserAndSignManager.GetUserForChangePasswordAsync(userId);
        }
        public async Task<IdentityResult> ChangePasswordAsync(UserChangePassword changePassword)
        {
            return await _fcdUserAndSignManager.SetNewPasswordAsync(changePassword);
        }
        public async Task<IdentityResult> EditUserAsync(UserForEdit userForEdit)
        {
            IdentityResult result = await _fcdUserAndSignManager.EditUserAsync(userForEdit);
            if (result.Succeeded)
            {
                _cacheService.Remove("UserList");
            }
            return result;
        }
        public async Task<bool> DroppingPasswordAsync(string userId)
        {
            return await _fcdUserAndSignManager.SetNewDefaultPasswordAsync(userId);
        }
        public async Task<UsersBaseInfoAndFilters> GetUsersAsync(FilterBase filters)
        {
            UsersBaseInfoAndFilters result = new UsersBaseInfoAndFilters
            {
                Filters = filters,
                SortEnumArray = sortEnumArrayUsers
            };
            string keyName = $"UserList{filters.GetFilterName()}";
            var userBaseInfos = _cacheService.Get<List<UserBaseInfo>>(keyName);
            if (userBaseInfos == null)
            {
                userBaseInfos = await _fcdUserAndSignManager.GetUserBaseInfosAsync();
                result.UserBaseInfos = userBaseInfos;
                _dataFiltering.Filtering(result);
                userBaseInfos = result.UserBaseInfos;
                _cacheService.Set<List<UserBaseInfo>>(keyName, userBaseInfos);
            }
            result.UserBaseInfos = userBaseInfos;
            _dataFiltering.SetFilters(result);
            result.UserBaseInfos = _dataFiltering.Paginator<UserBaseInfo>(result.UserBaseInfos, (int)filters.CountVisablePositions, (int)filters.CurrentPosition);
            return result;
        }
        public async Task<UserForEdit> GetUserForEditRolesAsync(string userId)
        {
            return await _fcdUserAndSignManager.GetUserForEditRolesAsync(userId);
        }
        public async Task<bool> SetNewUserRoles(string userId, List<string> roles)
        {
            bool result = await _fcdUserAndSignManager.SetNewUserRoles(userId, roles);
            if (result)
            {
                _cacheService.Remove("UserList");
            }
            return result;
        }
        private List<UserBaseInfo> FindOrdersForUser(List<UserBaseInfo> userBaseInfos, string desired)
        {
            List<UserBaseInfo> result = new List<UserBaseInfo>();
            if (desired.ToUpper().Contains("ANON"))
            {
                result.Add(new UserBaseInfo { Id = "anon", Email = "anon", Name = "anon" });
                return result;
            }
            foreach (var user in userBaseInfos)
            {
                if (user.Name.ToUpper().Contains(desired.ToUpper()))
                {
                    result.Add(user);
                }
            }
            return result;
        }
        private List<UserBaseInfo> FindOrdersForEmail(List<UserBaseInfo> userBaseInfos, string desired)
        {
            List<UserBaseInfo> result = new List<UserBaseInfo>();
            foreach (var user in userBaseInfos)
            {
                if (user.Email.ToUpper().Contains(desired.ToUpper()))
                {
                    result.Add(user);
                }
            }
            return result;
        }
        private List<ProductCardForEdit> FindProductsForId(List<ProductCardForEdit> products, string desired)
        {
            List<ProductCardForEdit> result = new List<ProductCardForEdit>();

            foreach (var product in products)
            {
                if (product.Id.ToString().Contains(desired.ToUpper()))
                {
                    result.Add(product);
                }
            }
            return result;
        }
        private List<ProductCardForEdit> FindProductsForName(List<ProductCardForEdit> products, string desired)
        {
            List<ProductCardForEdit> result = new List<ProductCardForEdit>();
            foreach (var product in products)
            {
                if (product.Name.ToUpper().Contains(desired.ToUpper()))
                {
                    result.Add(product);
                }
            }
            return result;
        }
        public async Task<UserOrdersAndFilters> GetUserOrdersAndFiltersAsync(FilterBaseAndSearch filters)
        {
            UserOrdersAndFilters result = new UserOrdersAndFilters()
            {
                Filters = filters,
                SearchEnumArray = searchEnumArrayOrders,
                SortEnumArray = sortEnumArrayOrders
            };
            string keyName = $"UserListAndOrdersStorekeeper{filters.GetFilterName()}",
                    keyNameUserListAll = $"UserListAll",
                    desired = filters.Desired;
            List<OrderMiniAndUserCardMini> orderMiniAndUserCardMini = _cacheService.Get<List<OrderMiniAndUserCardMini>>(keyName);
            if (orderMiniAndUserCardMini == null)
            {
                var userBaseInfos = _cacheService.Get<List<UserBaseInfo>>(keyNameUserListAll);
                if (userBaseInfos == null || userBaseInfos.Count == 0)
                {
                    userBaseInfos = await _fcdUserAndSignManager.GetUserBaseInfosAsync();
                    _cacheService.Set<List<UserBaseInfo>>(keyNameUserListAll, userBaseInfos);
                }
                if (desired != null && filters.WhereSearch != null)
                {
                    switch (Enum.Parse(typeWhereSearchOrders, filters.WhereSearch))
                    {
                        case (WhereSearchOrders.По_заказчику):
                            {
                                userBaseInfos = FindOrdersForUser(userBaseInfos, desired);
                                break;
                            }
                        case (WhereSearchOrders.По_почте):
                            {
                                userBaseInfos = FindOrdersForEmail(userBaseInfos, desired);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    _cacheService.Set<List<UserBaseInfo>>(keyNameUserListAll, userBaseInfos);
                }
                if (userBaseInfos?.Count != 0)
                {
                    string[] usersId = userBaseInfos.Select(users => users.Id).ToArray();
                    if (desired != null && filters.WhereSearch != null)
                    {
                        switch (Enum.Parse(typeWhereSearchOrders, filters.WhereSearch))
                        {
                            case (WhereSearchOrders.По_номеру):
                                {
                                    orderMiniAndUserCardMini = await _fcdStoreContext.GetUsersOrdersToStorekeeperAsync(desired);
                                    break;
                                }
                            default:
                                {
                                    orderMiniAndUserCardMini = await _fcdStoreContext.GetUsersOrdersToStorekeeperAsync(usersId);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        orderMiniAndUserCardMini = await _fcdStoreContext.GetUsersOrdersToStorekeeperAsync();
                    }
                    if (orderMiniAndUserCardMini?.Count > 0)
                    {
                        foreach (var order in orderMiniAndUserCardMini)
                        {
                            foreach (var user in userBaseInfos)
                            {
                                if (order.UserInfo.Id == user.Id)
                                {
                                    order.UserInfo.Email = user.Email;
                                    order.UserInfo.Name = user.Name;
                                    break;
                                }
                            }
                            if (order.UserInfo.Email == null)
                            {
                                order.UserInfo.Email = "Anon";
                                order.UserInfo.Name = "Anon";
                            }
                        }
                    }
                }
                result.OrderMiniAndUserCardMini = orderMiniAndUserCardMini;
                _dataFiltering.Filtering(result);
                orderMiniAndUserCardMini = result.OrderMiniAndUserCardMini;
                _cacheService.Set<List<OrderMiniAndUserCardMini>>(keyName, orderMiniAndUserCardMini);
            }
            result.OrderMiniAndUserCardMini = orderMiniAndUserCardMini;
            _dataFiltering.SetFilters(result);
            if (orderMiniAndUserCardMini?.Count > 0)
            {
                result.OrderMiniAndUserCardMini = _dataFiltering.Paginator<OrderMiniAndUserCardMini>(result.OrderMiniAndUserCardMini, (int)filters.CountVisablePositions, (int)filters.CurrentPosition);
            }
            return result;
        }
        public async Task<ProductCardForEditAndFilters> GetProductCardForEditAndFiltersAsync(FilterBaseAndSearch filters)
        {
            ProductCardForEditAndFilters result = new ProductCardForEditAndFilters()
            {
                Filters = filters,
                SearchEnumArray = searchEnumArrayProducts,
                SortEnumArray = sortEnumArrayProducts
            };
            string keyName = $"ProductListFilter{filters.GetFilterName()}",
                   keyNameAllProduct = $"ProductListALll",
                   desired = filters.Desired;
            var products = _cacheService.Get<List<ProductCardForEdit>>(keyName);
            if (products == null || products.Count == 0)
            {
                products = _cacheService.Get<List<ProductCardForEdit>>(keyNameAllProduct);
                if (products == null || products.Count == 0)
                {
                    products = await _fcdStoreContext.GetStProductCardAsync();
                    _cacheService.Set<List<ProductCardForEdit>>(keyNameAllProduct, products);
                }
                if (desired != null && filters.WhereSearch != null)
                {
                    switch (Enum.Parse(typeWhereSearchProductsSt, filters.WhereSearch))
                    {
                        case (StProductSearchs.По_Id):
                            {
                                products = FindProductsForId(products, desired);
                                break;
                            }
                        case (StProductSearchs.По_Названию):
                            {
                                products = FindProductsForName(products, desired);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                result.ProductCards = products;
                _dataFiltering.Filtering(result);
                products = result.ProductCards;
                _cacheService.Set<List<ProductCardForEdit>>(keyName, products);
            }
            result.ProductCards = products;
            _dataFiltering.SetFilters(result);
            if (result.ProductCards.Count > 0)
            {
                result.ProductCards = _dataFiltering.Paginator<ProductCardForEdit>(result.ProductCards, (int)filters.CountVisablePositions, (int)filters.CurrentPosition);
            }
            return result;
        }
        public async Task<bool> EditStProductCard(ProductCardForEdit product)
        {
            bool result = await _fcdStoreContext.EditStProductCard(product);
            if (result)
            {
                _cacheService.Remove("Product");
            }
            return result;
        }
        public async Task<List<OrderPositionsMini>> GetOrderPositionsMiniAsync(int orderId)
        {
            string keyName = $"OrderPositionsMini{orderId}";
            List<OrderPositionsMini> result = _cacheService.Get<List<OrderPositionsMini>>(keyName);
            if (result == null || result.Count == 0)
            {
                result = await _fcdStoreContext.GetOrderPositionsMiniAsync(orderId);
                _cacheService.Set<List<OrderPositionsMini>>(keyName, result);
            }
            return result;
        }
        public async Task<bool> EditStatusOrder(UpdateStatus order)
        {
            bool result = await _fcdStoreContext.EditStatusOrder(order);
            if (result == true)
            {
                _cacheService.Remove("Order");
                _cacheService.Remove("UserCardAndFilters");
                if (order.IdNewStatus == 4)
                {
                    _cacheService.Remove("Product");
                }
            }
            return result;
        }
        public async Task<ProductAttributesAndFilter> GetProductAttributesAndFilterAsync(FilterBase filters)
        {
            ProductAttributesAndFilter result = new ProductAttributesAndFilter()
            {
                Filters = filters,
                SortEnumArray = sortEnumArrayProductAttributes
            };
            string keyName = $"Attributes{filters.GetFilterName()}",
                   keyNameAllAttributes = "AllAttributes";
            List<ProductsAttribute> productAttributes = _cacheService.Get<List<ProductsAttribute>>(keyName);
            if (productAttributes == null)
            {
                productAttributes = _cacheService.Get<List<ProductsAttribute>>(keyNameAllAttributes);
                if (productAttributes == null)
                {
                    productAttributes = await _fcdStoreContext.GetAllProductsAttributesAsync();
                    _cacheService.Set<List<ProductsAttribute>>(keyNameAllAttributes, productAttributes);
                }
                if (productAttributes != null && productAttributes.Count > 0)
                {
                    result.ProductAttributes = productAttributes;
                    _dataFiltering.Filtering(result);
                    productAttributes = result.ProductAttributes;
                    _cacheService.Set<List<ProductsAttribute>>(keyName, productAttributes);
                }
            }
            result.ProductAttributes = productAttributes;
            _dataFiltering.SetFilters(result);
            result.ProductAttributes = _dataFiltering.Paginator<ProductsAttribute>(result.ProductAttributes, (int)filters.CountVisablePositions, (int)filters.CurrentPosition);
            return result;
        }
        public async Task<bool> AddProductsAttributeAsync(ProductsAttribute productsAttribute)
        {
            bool result = await _fcdStoreContext.AddProductsAttributeAsync(productsAttribute);
            if (result)
            {
                _cacheService.Remove("Attributes");
            }
            return result;
        }
        public async Task<ProductsAttribute> GetProductsAttributeAsync(int id)
        {
            ProductsAttribute result = new ProductsAttribute();
            List<ProductsAttribute> productAttributes = _cacheService.Get<List<ProductsAttribute>>("AllAttributes");
            if (productAttributes != null)
            {
                result = productAttributes.Where(attributes => attributes.Id == id).FirstOrDefault();
            }
            if (result == null)
            {
                result = await _fcdStoreContext.GetProductsAttributeAsync(id);
            }
            return result;
        }
        public async Task<bool> EditProductsAttributeAsync(ProductsAttribute productsAttribute)
        {
            bool result = await _fcdStoreContext.EditProductsAttributeAsync(productsAttribute);
            if (result)
            {
                _cacheService.Remove("Attributes");
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteProductsAttributeAsync(ProductsAttribute productsAttribute)
        {
            bool result = await _fcdStoreContext.DeleteProductsAttributeAsync(productsAttribute);
            if (result)
            {
                _cacheService.Remove("Attributes");
                return true;
            }
            return false;
        }
        public async Task<ProductTypeMiniAndFilters> GetProductTypeMinisAsync(FilterBase filters)
        {
            ProductTypeMiniAndFilters result = new ProductTypeMiniAndFilters()
            {
                Filters = filters,
                SortEnumArray = sortEnumArrayProducts
            };
            string keyName = $"ProductTypeMinis{filters.GetFilterName()}",
                   keyNameAllProductTypeMini = "ProductTypeMini";
            List<ProductTypeMini> productTypeMinis = _cacheService.Get<List<ProductTypeMini>>(keyName);
            if (productTypeMinis == null)
            {
                productTypeMinis = _cacheService.Get<List<ProductTypeMini>>(keyNameAllProductTypeMini);
                if (productTypeMinis == null || productTypeMinis.Count == 0)
                {
                    productTypeMinis = await _fcdStoreContext.GetProductTypeMinisAsync();
                    _cacheService.Set<List<ProductTypeMini>>(keyNameAllProductTypeMini, productTypeMinis);
                }
                result.ProductTypeMinis = productTypeMinis;
                _dataFiltering.Filtering(result);
                productTypeMinis = result.ProductTypeMinis;
                _cacheService.Set<List<ProductTypeMini>>(keyName, productTypeMinis);
            }
            result.ProductTypeMinis = productTypeMinis;
            _dataFiltering.SetFilters(result);
            result.ProductTypeMinis = _dataFiltering.Paginator<ProductTypeMini>(result.ProductTypeMinis, (int)filters.CountVisablePositions, (int)filters.CurrentPosition);
            return result;
        }
        private async Task<List<ProductTypeMini>> GetProductTypeMinisAsync()
        {
            string keyName = "ProductTypeMini",
                   keyNameParentTypes = "ParentsProductTypeMini";
            List<ProductTypeMini> productTypeMinis = _cacheService.Get<List<ProductTypeMini>>(keyNameParentTypes);
            if (productTypeMinis == null)
            {
                productTypeMinis = _cacheService.Get<List<ProductTypeMini>>(keyName);
                if (productTypeMinis != null && productTypeMinis.Count > 0)
                {
                    productTypeMinis = productTypeMinis.Where(preProductTypes => preProductTypes.ParentProductTypeId == 1).ToList();
                }
                else
                {
                    productTypeMinis = await _fcdStoreContext.GetParentProductTypes();
                    _cacheService.Set<List<ProductTypeMini>>(keyNameParentTypes, productTypeMinis);
                }
            }
            return productTypeMinis;
        }
        public async Task<ProductsTypesAllAndThis> GetPropductParentTypesAsync()
        {
            ProductsTypesAllAndThis result = new ProductsTypesAllAndThis();
            List<ProductTypeMini> productTypeMinis = await GetProductTypeMinisAsync();
            result.SetProductTypes(productTypeMinis);
            return result;
        }
        public async Task<ProductsTypesAllAndThis> GetProductTypeThisAndParentsAsync(int id)
        {
            ProductsTypesAllAndThis result = new ProductsTypesAllAndThis();
            List<ProductTypeMini> productTypeMinis = await GetProductTypeMinisAsync();
            result.SetProductTypes(productTypeMinis, id);
            string keyName = "ProductTypeMini";
            var productsTypes = _cacheService.Get<List<ProductTypeMini>>(keyName);
            if (productsTypes != null)
            {
                result.ProductType = productsTypes.Where(preProductTypes => preProductTypes.Id == id).First();
            }
            else
            {
                result.ProductType = await _fcdStoreContext.GetProductsTypeAsync(id);
            }
            return result;
        }
        public async Task<bool> AddProductsTypeAsync(ProductsType productsType)
        {
            bool result = await _fcdStoreContext.AddProductsTypeAsync(productsType);
            if (result)
            {
                _builderBackColorCSS.AddNewCSS(productsType.HexColor);
                _cacheService.Remove("ProductType");
                return true;
            }
            return false;
        }
        public async Task<bool> EditProdyctTypeAsync(ProductsType productsType, string oldHEXColor, int olpParentProductTypeId)
        {
            bool result;
            if (olpParentProductTypeId == 1 && productsType.ParentProductTypeId != 1)
            {
                result = await _fcdStoreContext.EditParentProductTypeToNoParent(productsType);
            }
            else
            {
                if (olpParentProductTypeId != 1 && (productsType.ParentProductTypeId == 1 || productsType.ParentProductTypeId == 2))
                {
                    result = await _fcdStoreContext.EditProductTypeToParentProductType(productsType);
                }
                else
                {
                    result = await _fcdStoreContext.EditProductType(productsType);
                }
            }
            if (result)
            {
                _builderBackColorCSS.RefreshCSS(oldHEXColor, productsType.HexColor);
                _cacheService.Remove("ProductType");
                _cacheService.Remove("Product");
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteProductTypeAsync(ProductsType productsType)
        {
            bool result = await _fcdStoreContext.DeleteProductTypeAsync(productsType);
            if (result)
            {
                _cacheService.Remove("ProductType");
                _cacheService.Remove("Product");
                return true;
            }
            return false;
        }
        public async Task<List<ProductTypesAttributeCheck>> GetAttributesToProductsTypesChecksAsync(int idProductType, int? idParentProductType)
        {
            string keyName = $"allProductsAttributes";
            var allProductsAttrubutes = _cacheService.Get<List<ProductsAttribute>>(keyName);
            if (allProductsAttrubutes == null)
            {
                allProductsAttrubutes = await _fcdStoreContext.GetProductsAttributesAsync();
                _cacheService.Set<List<ProductsAttribute>>(keyName, allProductsAttrubutes);
            }
            return await _fcdStoreContext.GetAttributesToProductsTypesChecksAsync(allProductsAttrubutes, idProductType, idParentProductType);
        }
        public async Task<bool> SetAttributesToProductsTypesChecksAsync(int idProductType,
                                                                int? idParentProductType,
                                                                List<int> attributesToProductsTypesIdCheck)
        {
            bool result = await _fcdStoreContext.SetAttributesToProductsTypesChecksAsync(idProductType, idParentProductType, attributesToProductsTypesIdCheck);
            if (result)
            {
                _cacheService.Remove("Attributes");
                return true;
            }
            return false;
        }
        public async Task<List<SelectListItem>> GetNoParentPTtoSelectListAsync()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            string keyName = $"ProductTypeMini";
            List<ProductTypeMini> productTypeMinis = _cacheService.Get<List<ProductTypeMini>>(keyName);
            List<ProductTypeMini> allNoParentsProductsTypes = new List<ProductTypeMini>();

            if (productTypeMinis != null)
            {
                allNoParentsProductsTypes = productTypeMinis.Where(preProductTypes => preProductTypes.ParentProductTypeId != 2 && preProductTypes.ParentProductTypeId!=1)
                    .Select(pt => new ProductTypeMini() { Id = pt.Id, Name = pt.Name, ParentProductTypeName = pt.ParentProductTypeName }).ToList();
            }
            else
            {
                allNoParentsProductsTypes = await _fcdStoreContext.GetNoParentPTtoSelectListAsync();
            }
            foreach (var item in allNoParentsProductsTypes)
            {
                result.Add(new SelectListItem() { Value = item.Id.ToString(), Text = $"{item.ParentProductTypeName} - {item.Name}" });
            }
            return result;
        }
        public async Task<bool> AddProductsAsync(Product product, IFormFile uploadedFile)
        {
            bool result = await _fcdStoreContext.AddProductsAsync(product, uploadedFile);
            if(result)
            {
                _cacheService.Remove("Product");
                return true;
            }
            return false;
        }
        public async Task<ProductAndProductTypes> GetProductAndProductTypesAsync(int id)
        {
            string keyName = $"Product{id}";
            ProductAndProductTypes result = new ProductAndProductTypes
            {
                Product = _cacheService.Get<Product>(keyName),
                ProductTypes = await GetNoParentPTtoSelectListAsync()
            };
            if(result.Product==null)
            {
                result.Product = await _fcdStoreContext.GetProductAsync(id);
                _cacheService.Set<Product>(keyName, result.Product);
            }
            return result;
        }
        public async Task<bool> EditProductsAsync(Product product, IFormFile uploadedFile)
        {
            bool result = await _fcdStoreContext.EditProductsAsync(product, uploadedFile);
            if(result)
            {
                _cacheService.Remove("Product");
                return true;
            }
            return false;
        }
        public async Task<List<AttributesToProduct>> GetAttributesToProductAsync(int idProduct, int idProductType, int idParentProductType)
        {
            string keyName = $"allAttributesToProductType";
            var allAttributesToProductTypes = _cacheService.Get<List<AttributesToProductType>>(keyName);
            if (allAttributesToProductTypes == null)
            {
                allAttributesToProductTypes = await _fcdStoreContext.GetAttributesToProductTypesAsync();
                _cacheService.Set<List<AttributesToProductType>>(keyName, allAttributesToProductTypes);
            }
            keyName = $"allAttributesToProduct";
            var allAttributesToProduct = _cacheService.Get<List<AttributesToProduct>>(keyName);
            if (allAttributesToProduct == null)
            {
                allAttributesToProduct = await _fcdStoreContext.GetAttributesToProductAsync();
                _cacheService.Set<List<AttributesToProduct>>(keyName, allAttributesToProduct);
            }
            return await _fcdStoreContext.GetAttributesToProductAsync(attributesToProductTypes: allAttributesToProductTypes,
                                                                      productsAttributes: allAttributesToProduct,
                                                                      idProduct: idProduct,
                                                                      idProductType: idProductType,
                                                                      idParentProductType: idParentProductType);
        }
        public async Task<bool> SetAttributesToProductAsync(int productId, List<AttributesToProduct> value)
        {
            bool result = await _fcdStoreContext.SetAttributesToProductAsync(productId, value);
            if(result)
            {
                _cacheService.Remove("Attribut");
                _cacheService.Remove("Product");
                return true;
            }
            return false;
        }
        public async Task<ProductAndOrders> ProductAndOrdersAsync(int id)
        {
            return await _fcdStoreContext.ProductAndOrdersAsync(id);
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            bool result = await _fcdStoreContext.DeleteProductAsync(id);
            if(result)
            {
                _cacheService.Remove("Attribut");
                _cacheService.Remove("Product");
                return true;
            }
            return false;
        }
        public async Task<List<ProductsPicturesPath>> GetProductsPicturesPathsAsync(int id)
        {
            return await _fcdStoreContext.GetProductsPicturesPathsAsync(id);
        }
        public async Task<bool> AddProductsPicturesPathsAsync(int id, IFormFile uploadedFiles1, IFormFile uploadedFiles2, IFormFile uploadedFiles3)
        {
            return await _fcdStoreContext.AddProductsPicturesPathsAsync(id, uploadedFiles1, uploadedFiles2, uploadedFiles3);
        }
    }
}
