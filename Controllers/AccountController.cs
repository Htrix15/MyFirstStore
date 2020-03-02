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

namespace MyFirstStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ISession _session;
        private readonly DataProcessingConveyor _dataProcessingConveyor;
        public AccountController(IHttpContextAccessor httpContextAccessor, 
                                DataProcessingConveyor dataProcessingConveyor)
        {
            _dataProcessingConveyor = dataProcessingConveyor;
            _session = httpContextAccessor.HttpContext.Session;
        }
        [HttpGet]
        public async Task<IActionResult> Card(string sortType, 
                                              int? minPrice,
                                              int? maxPrice,
                                              int? currentPosition,
                                              string select)
        {
            if (User.Identity.IsAuthenticated)
            {

                FilterOrders filterUserOrders = new FilterOrders(selectFromSelectList: select,
                                                                 minPrice: minPrice,
                                                                 maxPrice: maxPrice,
                                                                 countVisablePozitins: 3,
                                                                 currentPozition: currentPosition,
                                                                 sortType: sortType);
                
                string loadCurrentPosition = _session.GetString("CurrentPositionProduct");
                if (loadCurrentPosition != null)
                {
                    filterUserOrders.CurrentPosition = Convert.ToInt32(loadCurrentPosition);
                    _session.Remove("CurrentPositionProduct");
                }
                UserCardAndFilters userCard = await _dataProcessingConveyor.GetUserCardAndFiltersAsync(User.Identity.Name, filterUserOrders);
                return View(userCard);
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult Register(string order)
        {
            if (order != null)
            {
                return View(new UserRegister { OrderId = order });
            }
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegister userRegisterInfo)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _dataProcessingConveyor.RegisterUserAsync(userRegisterInfo);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    string errorText, key;
                    foreach (var error in result.Errors)
                    {
                        errorText = error.Description;
                        key = String.Empty;
                        if (error.Code== "DuplicateUserName")
                        {
                            errorText = "Имя уже занято, выберете другое";
                            key = "Name";
                        }
                        ModelState.AddModelError(key, errorText);
                    }
                }
            }
            return View(userRegisterInfo);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new UserLogin { ReturnUrl = returnUrl });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLogin userLoginInfo)
        {
            if (ModelState.IsValid)
            {
                var result = await _dataProcessingConveyor.LoginUserAsync(userLoginInfo);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(userLoginInfo.ReturnUrl) && Url.IsLocalUrl(userLoginInfo.ReturnUrl))
                    {
                        return Redirect(userLoginInfo.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Неправильный логин и (или) пароль");
                }
            }
            return View(userLoginInfo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _dataProcessingConveyor.Logout();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccessDenied(string ReturnUrl)
        {
            return View(model: ReturnUrl);
        }
        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            if (User.Identity.IsAuthenticated)
            {

                UserForEdit deleteUser = await _dataProcessingConveyor.GetUserForEditAsync(User.Identity.Name);
                if (deleteUser != null)
                {
                    return View(deleteUser);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(bool yesDelete)
        {
            if (yesDelete)
            {
                if (await _dataProcessingConveyor.DeleteUserAsync(User.Identity.Name))
                {

                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Card", "Account");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserChangePassword changePassword)
        {
            if (changePassword != null && ModelState.IsValid)
            {
                changePassword.UserId = User.Identity.Name;
                IdentityResult result = await _dataProcessingConveyor.ChangePasswordAsync(changePassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    string errorText, key;
                    foreach (var error in result.Errors)
                    {
                        errorText = error.Description;
                        key = String.Empty;
                        if (error.Description == "Incorrect password.")
                        {
                            errorText = "Неправильный старый пароль";
                            key = "OldPassword";
                        }
                        ModelState.AddModelError(key, errorText);
                    }
                    return View(changePassword);
                }
            }
            return View(changePassword);
        }
        [HttpGet]
        public async Task<IActionResult> Edit() 
        {
            if (User.Identity.IsAuthenticated)
            {
                UserForEdit editUserViewModel = await _dataProcessingConveyor.GetUserForEditAsync(User.Identity.Name);
                if (editUserViewModel != null)
                {
                    return View(editUserViewModel);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserForEdit userForEdit)
        {
            if (userForEdit != null && ModelState.IsValid)
            {
                IdentityResult result = await _dataProcessingConveyor.EditUserAsync(userForEdit);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    string errorText, key;
                    foreach (var error in result.Errors)
                    {
                        errorText = error.Description;
                        key = String.Empty;
                        if (error.Code == "DuplicateUserName")
                        {
                            errorText = "Имя уже занято, выберете другое";
                            key = "Name";
                        }
                        ModelState.AddModelError(key, errorText);
                    }
                    return View(userForEdit);
                }
            }
            return View(userForEdit);
        }
        //==============================
        [HttpGet]
        public async Task<IActionResult> CardAjax(string sortType,
                              int? minPrice,
                              int? maxPrice,
                              int? currentPosition,
                              string select)
        {
            if (User.Identity.IsAuthenticated)
            {
                FilterOrders filterUserOrders = new FilterOrders(selectFromSelectList: select,
                                                                minPrice: minPrice,
                                                                maxPrice: maxPrice,
                                                                countVisablePozitins: 3,
                                                                currentPozition: currentPosition,
                                                                sortType: sortType);
                UserCardAndFilters userCard = await _dataProcessingConveyor.GetUserCardAndFiltersAsync(User.Identity.Name, filterUserOrders);
                return new JsonResult(new
                {
                    orders = userCard.Orders.Select(t => new
                    {
                        jsId = t.Id,
                        jsDataReservation = t.DataReservation.ToShortDateString(),
                        jsOrderStatusId = t.StatusId,
                        jsStatus = t.StatusName,
                        jsTotalFixPrice = t.TotalFixCost,
                        jsCurrentPosition= currentPosition
                    }).ToList()
                ,
                    orderPositions = userCard.OrderPositions.Select(t => new
                    {
                        jsCount = t.Count,
                        jsFixPrice = t.FixPrice,
                        jsMainPicturePath = t.MainPicturePath,
                        jsOrderId = t.OrderId,
                        jsProductId = t.ProductId,
                        jsProductName = t.ProductName,
                        jsProductTypeName = t.ProductTypeName
                    }).ToList()
                });
            }
            return NotFound();
        }

    }
}
