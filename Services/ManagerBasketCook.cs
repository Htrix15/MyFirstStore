using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyFirstStore.Models;
using MyFirstStore.Services;
using MyFirstStore.ViewModels;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace MyFirstStore.Services
{
    public class ManagerBasketCook 
    {
        private readonly UserManager<User> _userManager;
        private readonly CookieOptions cookieOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ManagerBasketCook> _logger;
        public ManagerBasketCook(UserManager<User> userManager, 
                                IHttpContextAccessor httpContextAccessor,
                                ILogger<ManagerBasketCook> logger)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddMinutes(30),
                IsEssential = true
            };
        }
        public async Task<string> GetCookNameAsync()
        {
            bool test = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
                return "Basket" + user.Id.ToString();
            }
            else
            {
                return "BasketAnon";
            }
        }
        public async Task<int> AddProductInBasketCookAsync(int idProduct)
        {
            try
            {
                string products;
                int countProduct = 1;
                string cookName = await GetCookNameAsync();
                CountProductAndProducts rezult;
                bool existCook = _httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(cookName);
                if (existCook)
                {
                    rezult = UpdateProductInfo(_httpContextAccessor.HttpContext.Request.Cookies[cookName], 1, idProduct);
                    products = rezult.getProducts();
                    countProduct = rezult.getCount();
                }
                else
                {
                    products = JsonConvert.SerializeObject(new Dictionary<int, int> { { idProduct, 1 } });
                }
                _httpContextAccessor.HttpContext.Response.Cookies.Append(cookName, products, cookieOptions);
                return countProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail add product to cook");
                return 0;
            }
        }
        private class CountProductAndProducts
        {
            int count;
            string products;
            public CountProductAndProducts(int count, string products)
            {
                this.count = count;
                this.products = products;
            }
            public int getCount()
            {
                return count;
            }
            public string getProducts()
            {
                return products;
            }
        }
        private CountProductAndProducts UpdateProductInfo(string products, int increment, int idProduct, bool deleteAllCount = false)
        {
            try
            {
                Dictionary<int, int> oldProducts = JsonConvert.DeserializeObject<Dictionary<int, int>>(products);
                int newCount = 0;
                bool existProduct = oldProducts.ContainsKey(idProduct);
                if (existProduct)
                {

                    if (deleteAllCount)
                    {
                        oldProducts.Remove(idProduct);
                    }
                    else
                    {
                        newCount = oldProducts[idProduct] += increment;
                        if (newCount > 0)
                        {
                            oldProducts[idProduct] = newCount;
                        }
                        else
                        {
                            oldProducts.Remove(idProduct);
                        }
                    }
                }
                else
                {
                    oldProducts.Add(idProduct, 1);
                }
                products = JsonConvert.SerializeObject(oldProducts);
                return new CountProductAndProducts(newCount, products);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Fail update cook");
                return null;
            }

        }
        public async Task<int> RemoveProductInBasketCookAsync(int idProduct, bool deleteAllCount = false)
        {
            try
            {
                string cookName = await GetCookNameAsync();
                string products = "";
                int countProduct = 0;
                CountProductAndProducts rezult;
                bool existCook = _httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(cookName);
                if (existCook)
                {
                    rezult = UpdateProductInfo(_httpContextAccessor.HttpContext.Request.Cookies[cookName], -1, idProduct, deleteAllCount);
                    products = rezult.getProducts();
                    countProduct = rezult.getCount();
                }
                if (products.Length > 2)
                {
                    _httpContextAccessor.HttpContext.Response.Cookies.Append(cookName, products, cookieOptions);
                }
                else
                {
                    await DeleteBasketCook();
                }
                return countProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail remove product in cook");
                return 0;
            }
        }
        public async Task<Dictionary<int,int>> GetDictionaryProductsForBasketCookAsync()
        {
            string cookName = await GetCookNameAsync();
            bool existCook = _httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(cookName);
            if (existCook)
            {
              return JsonConvert.DeserializeObject<Dictionary<int, int>>(_httpContextAccessor.HttpContext.Request.Cookies[cookName]);
            }
            return null;
        }
        public async Task<bool> DeleteBasketCook()
        {
            try
            {
                string cookName = await GetCookNameAsync();
                if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(cookName))
                {
                    _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookName);
                }
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Fail delete cook");
                return false;
            }
        }
    }
}
