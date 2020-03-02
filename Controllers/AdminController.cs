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

namespace MyFirstStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        DataProcessingConveyor _dataProcessingConveyor;
        ISession _session;
        public AdminController(IHttpContextAccessor httpContextAccessor, 
                               DataProcessingConveyor dataProcessingConveyor)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _dataProcessingConveyor = dataProcessingConveyor;
        }
        public async Task<IActionResult> UsersView(int? currentPosition, 
                                                   string sortType = null,
                                                   string select = null)
        {
            FilterBase filters = new FilterBase(countVisablePozitins: 3,
                                                currentPosition: currentPosition,
                                                selectFromSelectList: select,
                                                sortType: sortType);
            string loadCurrentPosition = _session.GetString("CurrentPositionUser");
            if (loadCurrentPosition != null)
            {
                filters.CurrentPosition = Convert.ToInt32(loadCurrentPosition);
                _session.Remove("CurrentPositionUser");
            }
            UsersBaseInfoAndFilters result = await _dataProcessingConveyor.GetUsersAsync(filters);

            return View(result);
        }  
        [HttpGet]
        public async Task<IActionResult> DroppingPassword(string id, int currentPosition)
        {
            _session.SetString("CurrentPositionUser", currentPosition.ToString());
            if (id != null)
            {
                UserForEdit deleteUser = await _dataProcessingConveyor.GetUserForEditAsync(id);
                if (deleteUser != null)
                {
                    return View(deleteUser);
                }
            }
            return RedirectToAction("UsersView", "Admin");
        }
        [HttpPost]
        public async Task<IActionResult> DroppingPassword(string id, bool yesDroppingPassword)
        {
            if (yesDroppingPassword && id!=null)
            {
                if (await _dataProcessingConveyor.DroppingPasswordAsync(id))
                {
                    return RedirectToAction("UsersView", "Admin");
                }
            }
            return RedirectToAction("UsersView", "Admin");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string id, int currentPosition)
        {
            _session.SetString("CurrentPositionUser", currentPosition.ToString());
            UserForEdit deleteUser = await _dataProcessingConveyor.GetUserForEditAsync(id);
            if (deleteUser != null)
            {
                return View(deleteUser);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id, bool yesDelete)
        {
            if (yesDelete)
            {
                if (await _dataProcessingConveyor.DeleteUserAsync(id))
                {
                    return RedirectToAction("UsersView", "Admin");
                }
            }
            return RedirectToAction("UsersView", "Admin");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id, int currentPosition)
        {
            _session.SetString("CurrentPositionUser", currentPosition.ToString());
            if (id != null)
            {
                UserForEdit UserForEdit = await _dataProcessingConveyor.GetUserForEditAsync(id);
                if (UserForEdit != null)
                {
                    return View(UserForEdit);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserForEdit UserForEdit)
        {
            if (UserForEdit != null && ModelState.IsValid)
            {
                IdentityResult result = await _dataProcessingConveyor.EditUserAsync(UserForEdit);
                if (result.Succeeded)
                {
                    return RedirectToAction("UsersView", "Admin");
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
                    return View(UserForEdit);
                }
            }
            return View(UserForEdit);
        }
        [HttpGet]
        public async Task<IActionResult> SetRoles(string id, int currentPosition)
        {
            _session.SetString("CurrentPositionUser", currentPosition.ToString());
            if (id != null)
            {
                UserForEdit UserForEdit = await _dataProcessingConveyor.GetUserForEditRolesAsync(id);
                if (UserForEdit != null)
                {
                    return View(UserForEdit);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> SetRoles(string id, List<string> roles)
        {
            if (id != null)
            {
                var result = await _dataProcessingConveyor.SetNewUserRoles(id, roles);
                if (result)
                {
                    return RedirectToAction("UsersView", "Admin");
                }
            }
            return NotFound();
        }
        //------------------------
        public async Task<IActionResult> UsersViewAjax(int? currentPosition,
                                                       string sortType = null,
                                                       string select = null)
        {
            FilterBase filters = new FilterBase(countVisablePozitins: 3,
                                                currentPosition: currentPosition,
                                                selectFromSelectList: select,
                                                sortType: sortType);
            UsersBaseInfoAndFilters result = await _dataProcessingConveyor.GetUsersAsync(filters);

            return new JsonResult(result.UserBaseInfos.Select(t => new {
                jsId = t.Id,
                jsName = t.Name,
                jsEmail = t.Email,
                jsCurrentPosition = currentPosition,
                jsAllRoles = t.AllRoles}
                ).ToList());
        }
    }
}
