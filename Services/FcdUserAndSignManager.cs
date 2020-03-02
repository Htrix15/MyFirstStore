using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyFirstStore.Models;
using MyFirstStore.Services;
using MyFirstStore.ViewModels;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using MyFirstStore.ViewModels.Storekeeper;
using Microsoft.Extensions.Logging;

namespace MyFirstStore.Services
{
    public class FcdUserAndSignManager
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly HttpContext _httpContext;
        private readonly ILogger<FcdUserAndSignManager> _logger;
        public FcdUserAndSignManager(UserManager<User> userManager,
                                     SignInManager<User> signInManager,
                                     RoleManager<IdentityRole> roleManager,
                                     IHttpContextAccessor httpContextAccessor,
                                     ILogger<FcdUserAndSignManager> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _httpContext = httpContextAccessor.HttpContext;
            _logger = logger;
        }
        public async Task<User> GetUserAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }
        public async Task<List<UserBaseInfo>> GetUserBaseInfosAsync()
        {
            List<UserBaseInfo> result = new List<UserBaseInfo>();
            var users = await _userManager.Users.ToListAsync();
            foreach (var item in users)
            {
                string allRoles = "";
                var roles = await _userManager.GetRolesAsync(item);
                roles.ToList().ForEach(role => allRoles += role + " ");
                result.Add(new UserBaseInfo()
                    {
                        Email = item.Email,
                        Id = item.Id,
                        Name = item.UserName,
                        Roles = roles,
                        AllRoles = allRoles
                    });
            }
            return result;
        }
        public async Task<IdentityResult> AddUserAsync(UserRegister userRegisterInfo)
        {
            User user = new User { Email = userRegisterInfo.Email, UserName = userRegisterInfo.Name };
            IdentityResult result = await _userManager.CreateAsync(user, userRegisterInfo.Password);
            try
            {
                if (result.Succeeded)
                {
                    List<String> newRolew = new List<string>() { "User" };
                    await _userManager.AddToRolesAsync(user, newRolew);
                    await _signInManager.SignInAsync(user, false);
                }
                else
                {
                    _logger.LogError("Fail add user");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Fail add user");
                return result;
            }
        }
        public async Task<SignInResult> GetPasswordSignInrAsync(UserLogin userLoginInfo)
        { 
            return await _signInManager.PasswordSignInAsync(userLoginInfo.Name, userLoginInfo.Password, userLoginInfo.RememberMe, false);
        }
        public async Task<bool> Logout()
        {
            await _signInManager.SignOutAsync();
            return true;
        }
        public async Task<UserForEdit> GetUserForEditAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = await _userManager.FindByIdAsync(userName);
            }
            UserForEdit userForEdit = new UserForEdit();
            if (user != null)
            {
                userForEdit.Id = user.Id;
                userForEdit.Name = user.UserName;
                userForEdit.Email = user.Email;
            }
            return userForEdit;
        }
        public async Task<bool> DeleteUserAsync(string userName)
        {
            try
            {
                User user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    IdentityResult result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        //await _signInManager.SignOutAsync();
                        return true;
                    }
                    _logger.LogError("Fail delete user");
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail delete user");
                return false;
            }
        }
        public async Task<UserChangePassword> GetUserForChangePasswordAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            UserChangePassword changePassword = new UserChangePassword();
            if (user != null)
            {
                changePassword.UserId = user.Id;
            }
            return changePassword;
        }
        public async Task<IdentityResult> SetNewPasswordAsync(UserChangePassword changePassword)
        {
            User user = await _userManager.FindByNameAsync(changePassword.UserId);
            IdentityResult result = new IdentityResult();
            if (user != null)
            {
                result = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                }
            }
            return result;
        }
        public async Task<IdentityResult> EditUserAsync(UserForEdit UserForEdit)
        {
            User user = await _userManager.FindByIdAsync(UserForEdit.Id);
            if(user==null)
            {
               user = await _userManager.FindByNameAsync(UserForEdit.Id);
            }
            IdentityResult result = new IdentityResult();
            try
            {
                if (user != null)
                {
                    user.Email = UserForEdit.Email;
                    user.UserName = UserForEdit.Name;
                    result = await _userManager.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail edit user");
            }
            return result;
        }
        public async Task<bool> SetNewDefaultPasswordAsync(string userId)
        {
            User user = await _userManager.FindByNameAsync(userId);
            try
            {
                if (user != null)
                {
                    var _passwordHasher = _httpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;
                    user.PasswordHash = _passwordHasher.HashPassword(user, "Qwerty123$");
                    var successUpdate = await _userManager.UpdateAsync(user);
                    if (successUpdate.Succeeded)
                    {
                        return true;
                    }
                }
                _logger.LogError("Fail set new password");
                return false;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Fail set new password");
                return false;
            }
        }
        public async Task<UserForEdit> GetUserForEditRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            UserForEdit userForEdit = new UserForEdit();
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var allRoles = await _roleManager.Roles.ToListAsync();
                userForEdit.Id = user.Id;
                userForEdit.Name = user.UserName;
                userForEdit.Email = user.Email;
                userForEdit.Roles = roles;
                userForEdit.AllRoles = allRoles;
            }
            return userForEdit;
        }
        public async Task<bool> SetNewUserRoles(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            try
            {
                if (user != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var newRoles = roles.Except(userRoles);
                    var delRoles = userRoles.Except(roles);
                    var successAdd = await _userManager.AddToRolesAsync(user, newRoles);
                    var successRemove = await _userManager.RemoveFromRolesAsync(user, delRoles);
                    if (successAdd.Succeeded && successRemove.Succeeded)
                    {
                        return true;
                    }
                    _logger.LogError("Fail set new user roles");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Fail set new user roles");
            }
            return false;
        }
    }
}
