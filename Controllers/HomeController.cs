using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyFirstStore.Models;
using MyFirstStore.Services;
using MyFirstStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;

namespace MyFirstStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ManagerBasketCook _managerBasketCook;
        private readonly UserManager<User> _userManager;
        private readonly ISession _session;
        private readonly DataProcessingConveyor _dataProcessingConveyor;
        public HomeController(DataProcessingConveyor dataProcessingConveyor,
                              IHttpContextAccessor httpContextAccessor,
                              ManagerBasketCook managerBasketCook, 
                              UserManager<User> userManager)
        {
            _dataProcessingConveyor = dataProcessingConveyor;
            _userManager = userManager;
            _managerBasketCook = managerBasketCook;
            _session = httpContextAccessor.HttpContext.Session;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _dataProcessingConveyor.GetProductTypeMIniAndBasketAsync();
            if(result.ProductTypeMinis!=null)
            {
                return View(result);
            }
            return NotFound();
        }  
        public async Task<IActionResult> ProductsList(string sortType,
                                                      bool? onlySale, 
                                                      bool? onlyAvailability, 
                                                      int? minPrice, 
                                                      int? maxPrice, 
                                                      int? currentPosition,
                                                      string select)
        {
            if (select != null)
            {
                FilterProductCard filters = new FilterProductCard(onlyAvailability: onlyAvailability,
                                                                 onlySale: onlySale,
                                                                 minPrice: minPrice,
                                                                 maxPrice: maxPrice,
                                                                 selectFromSelectList: select,
                                                                 countVisablePositions: 3,
                                                                 currentPosition: currentPosition,
                                                                 sortType: sortType
                                                                 );
                string loadCurrentPosition = _session.GetString("CurrentPositionProduct");
                if (loadCurrentPosition != null)
                {
                    filters.CurrentPosition = Convert.ToInt32(loadCurrentPosition);
                    _session.Remove("CurrentPositionProduct");
                }
                var result = await _dataProcessingConveyor.GetProductCardAndBasketAndFiltersAsync(filters);
                return View(result);
            }
            return NotFound();
        }     
        public async Task<IActionResult> AboutToProduct(int? id, 
                                                        int? currentPosition)
        {   
            if (id != null )
            {
                BigProductCardAndBasket result = await _dataProcessingConveyor.GetBigProductCardAndBasketAsync((int)id);
                if (result != null)
                {
                    _session.SetString("CurrentPositionProduct", currentPosition.ToString());
                    return View(result);
                }
            }
            return NotFound();
        }
        public async Task<IActionResult> Search(string whereSearch, 
                                                string desired, 
                                                string sortType, 
                                                bool? onlySale, 
                                                bool? onlyAvailability, 
                                                int? minPrice, 
                                                int? maxPrice, 
                                                int? currentPosition,
                                                string select)
        {
            if (desired != null)
            {
                if (desired.Length >= 3)
                {
                    FilterProductCard filters = new FilterProductCard(onlyAvailability: onlyAvailability,
                                                                     onlySale: onlySale,
                                                                     minPrice: minPrice,
                                                                     maxPrice: maxPrice,
                                                                     selectFromSelectList: select,
                                                                     countVisablePositions: 3,
                                                                     currentPosition: currentPosition,
                                                                     sortType: sortType,
                                                                     desired: desired,
                                                                     whereSearch: whereSearch
                                                                     );

                    string loadCurrentPosition = _session.GetString("CurrentPositionProduct");
                    if (loadCurrentPosition != null)
                    {
                        filters.CurrentPosition = Convert.ToInt32(loadCurrentPosition);
                        _session.Remove("CurrentPositionProduct");
                    }
                    ProductCardAndBasketAndFilters result = await _dataProcessingConveyor.GetFilteringProductCardAndBasketAndFiltersAsync(filters);
                    if (result != null)
                    {
                        return View(result);
                    }
                }
                else
                {
                    return Redirect(Request.Headers["Referer"].ToString());
                }
            }
            return NotFound();
        } 
        public async Task<IActionResult> BuyOneClick(int id)
        {
            await _managerBasketCook.AddProductInBasketCookAsync(id);
            return RedirectToAction("BuyStep1", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> BuyStep1()
        {
            PreOrder result = await _dataProcessingConveyor.GetPreOrderAsync();
            if (result != null)
            {
                _session.SetString("preOrder", JsonConvert.SerializeObject(result));
                return View(result);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [ActionName("BuyStep1")]
        public async Task<IActionResult> BuyStep1P()
        {
            string userId = (User.Identity.Name != null) ? _userManager.FindByNameAsync(User.Identity.Name).Result.Id : "anon";
            string oldPreOrderState = _session.GetString("preOrder");
            int orderCode = await _dataProcessingConveyor.GetOrderCodeAsync(userId, oldPreOrderState); 
            _session.Remove("preOrder");
            if (orderCode > 0)
            {
                return RedirectToAction("BuyStep2", "Home", new { codeOrder = orderCode });
            }
            else
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }
        public IActionResult BuyStep2(string codeOrder)
        {
            return View(model:codeOrder);
        }
        public async Task<IActionResult> ProductsListAjax(string sortType,
                                                          bool? onlySale,
                                                          bool? onlyAvailability,
                                                          int? minPrice,
                                                          int? maxPrice,
                                                          int? currentPosition,
                                                          string select)
        {
            if (select != null)
            {
                FilterProductCard filters = new FilterProductCard(onlyAvailability: onlyAvailability,
                                                                  onlySale: onlySale,
                                                                  minPrice: minPrice,
                                                                  maxPrice: maxPrice,
                                                                  selectFromSelectList: select,
                                                                  countVisablePositions: 3,
                                                                  currentPosition: currentPosition,
                                                                  sortType: sortType
                                                                  );
                var result = await _dataProcessingConveyor.GetProductCardAndBasketAndFiltersAsync(filters);

                return new JsonResult(result.ProductCards.Select(t=> new {
                    jsId=t.Id,
                    jsName = t.Name,
                    jsProductTypeId = t.ProductTypeId,
                    jsParentTypeName = t.ParentTypeName,
                    jsParentProductTypeId = t.ParentTypeId,
                    jsMainPicturePath = t.MainPicturePath,
                    jsMainAttribute = t.MainAttribute,
                    jsPrice = t.Price,
                    jsSale = t.Sale,
                    jsHexColor = t.HexColor,
                    jsCurrentPosition = currentPosition}).ToList());
            }
            return NotFound();
        }
        public async Task<IActionResult> SearchAjax(string whereSearch,
                                                    string desired,
                                                    string sortType,
                                                    bool? onlySale,
                                                    bool? onlyAvailability,
                                                    int? minPrice,
                                                    int? maxPrice,
                                                    int? currentPosition,
                                                    string select)
        {
            if (desired != null)
            {
                Type typeWhereSearch = typeof(WhereSearch);
                FilterProductCard filters = new FilterProductCard(onlyAvailability: onlyAvailability,
                                                                 onlySale: onlySale,
                                                                 minPrice: minPrice,
                                                                 maxPrice: maxPrice,
                                                                 selectFromSelectList: select,
                                                                 countVisablePositions: 3,
                                                                 currentPosition: currentPosition,
                                                                 sortType: sortType,
                                                                 desired: desired,
                                                                 whereSearch: whereSearch
                                                                 );
                var result = await _dataProcessingConveyor.GetFilteringProductCardAndBasketAndFiltersAsync(filters);
                return new JsonResult(result.ProductCards.Select(t => new { 
                    jsId = t.Id,
                    jsName = t.Name,
                    jsProductTypeId = t.ProductTypeId,
                    jsParentProductTypeId = t.ParentTypeId,
                    jsMainPicturePath = t.MainPicturePath,
                    jsMainAttribute = t.MainAttribute,
                    jsPrice = t.Price,
                    jsSale = t.Sale,
                    jsHexColor = t.HexColor,
                    jsCurrentPosition = currentPosition}).ToList());
            }
            return NotFound();
        }
        public async Task<IActionResult> ProductInBasketAjax(int id, 
                                                             string price, 
                                                             string picturePath,
                                                             string name,
                                                             string color)
        {
            int countProductToBasket = await _managerBasketCook.AddProductInBasketCookAsync(id);
            return new JsonResult(new { jsCountProductToBasket = countProductToBasket, 
                                        jsId = id, 
                                        jsName = name,
                                        jsPrice = Convert.ToDecimal(price), 
                                        jsPicturePath = picturePath,
                                        jsColor = color});
        }
        public async Task<IActionResult> AddProductToBasketAjax(int id)
        {
            int countProductToBasket;
            countProductToBasket = await _managerBasketCook.AddProductInBasketCookAsync(id);
            return new JsonResult(countProductToBasket);
        }
        public async Task<IActionResult> RemoveProductForBasketAjax(int id, 
                                                                    bool all)
        {
            int countProductToBasket;
            countProductToBasket = await _managerBasketCook.RemoveProductInBasketCookAsync(id, all);
            return new JsonResult(countProductToBasket);
        }
        public async Task<IActionResult> DeleteBasketAjax()
        {
           return new JsonResult(await _managerBasketCook.DeleteBasketCook());
        }
        public async Task<IActionResult> AddProductToBasket(int id)
        {
            
            await _managerBasketCook.AddProductInBasketCookAsync(id);
            return Redirect("BuyStep1");
        }
        public async Task<IActionResult> RemoveProductForBasket(int id, 
                                                                bool all)
        {
            await _managerBasketCook.RemoveProductInBasketCookAsync(id, all);
            return Redirect("BuyStep1");
        }

    }
}
