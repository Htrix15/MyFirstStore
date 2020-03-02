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
using MyFirstStore.ViewModels.Storekeeper;

namespace MyFirstStore.Controllers
{
    [Authorize(Roles = "Admin, Storekeeper")]
    public class StorekeeperController : Controller
    {
        private readonly DataProcessingConveyor _dataProcessingConveyor;
        public StorekeeperController(DataProcessingConveyor dataProcessingConveyor)
        {
            _dataProcessingConveyor = dataProcessingConveyor;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Orders(string whereSearch,
                                                string desired, 
                                                string sortType, 
                                                int? currentPosition, 
                                                string select)
        {
            FilterBaseAndSearch filters = new FilterBaseAndSearch(desired: desired,
                                                      whereSearch: whereSearch,
                                                      countVisablePositions: 3,
                                                      currentPosition: currentPosition,
                                                      selectFromSelectList: select,
                                                      sortType: sortType);
            UserOrdersAndFilters result = await _dataProcessingConveyor.GetUserOrdersAndFiltersAsync(filters);
            return View(result);
          
        }
        public async Task<IActionResult> Products(string whereSearch,
                                                  string desired,
                                                  string sortType,
                                                  int? currentPosition,
                                                  string select)
        {
            FilterBaseAndSearch filters = new FilterBaseAndSearch(desired: desired,
                                                                  whereSearch: whereSearch,
                                                                  countVisablePositions: 3,
                                                                  currentPosition: currentPosition,
                                                                  selectFromSelectList: select,
                                                                  sortType: sortType);
            ProductCardForEditAndFilters result = await _dataProcessingConveyor.GetProductCardForEditAndFiltersAsync(filters);
            return View(result);
        }
        [HttpGet]
        public IActionResult Edit(ProductCardForEdit product)
        {
            if (product != null)
            {
                return View(product);
            }
            return NotFound();
        }
        [HttpPost]
        [ActionName("Edit")]
        public async Task<IActionResult> PEdit(ProductCardForEdit product)
        {
            if (product != null)
            {
                if (ModelState.IsValid)
                {
                    bool resultUpdate = await _dataProcessingConveyor.EditStProductCard(product);
                    if (resultUpdate)
                    {
                        return RedirectToAction("Products");
                    }
                    return NotFound();
                }
                else
                {
                    return View(product);
                }
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> AboutOrder (UpdateStatus order)
        {
            var result = await _dataProcessingConveyor.GetOrderPositionsMiniAsync(order.IdOrder);
            return View(new OrderAndOrderPositions() { Order = order, OrderPositionsMinis = result });

        }
        [HttpGet] 
        public IActionResult UpdateStatus(UpdateStatus order)
        {
            return View(order);
        }
        [HttpPost]
        [ActionName("UpdateStatus")]
        public async Task<IActionResult> PUpdateStatus(UpdateStatus order)
        {
            if(order.YesUpdate==true)
            {
                if(await _dataProcessingConveyor.EditStatusOrder(order))
                {
                    return RedirectToAction("Orders");
                }
                else
                {
                    return NotFound();
                }
            }
            return RedirectToAction("Orders");
        }

    }
}
