using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MyFirstStore.Services
{
    public class DBInitializer
    {
        private readonly StoreContext _storeContext;
        private readonly ApplicationContext _applicationContext;
        private readonly ILogger<DBInitializer> _logger;
        public DBInitializer(StoreContext storeContext,
                            ApplicationContext applicationContext,
                            ILogger<DBInitializer> logger)
        {
            _storeContext = storeContext;
            _applicationContext = applicationContext;
            _logger = logger;
        }
        public bool CreateDb()
        {
            try
            {
                _applicationContext.Database.EnsureCreated();
                try
                {
                    RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)_applicationContext.Database.GetService<IDatabaseCreator>();
                    databaseCreator.CreateTables();
                }
                catch { }
                try
                {
                    RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)_storeContext.Database.GetService<IDatabaseCreator>();
                    databaseCreator.CreateTables();
                }
                catch { }
                return true;
            }
            catch(Exception ex)
            {

                _logger.LogError(ex, "Create DB fail!");
                return false;
            }
        }
        public async Task InitializeRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            try
            {
                if (await roleManager.FindByNameAsync("Admin") == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (await roleManager.FindByNameAsync("User") == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }
                if (await roleManager.FindByNameAsync("ContentManager") == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("ContentManager"));
                }
                if (await roleManager.FindByNameAsync("Storekeeper") == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("Storekeeper"));
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Fail initialize roles");
            }
        }
        public async Task InitializerAdminUser(UserManager<User> userManager)
        {
            try
            {
                if (await userManager.FindByNameAsync("Admin") == null)
                {
                    string email = "test@test.com",
                           password = "Qwerty_123";
                    User admin = new User()
                    {
                        UserName = "Admin",
                        Email = email
                    };
                    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                    IConfiguration AppConfiguration = builder.Build();
                    IConfigurationSection adminOptions = AppConfiguration.GetSection("Admin");

                    if (adminOptions != null)
                    {
                        admin.Email = adminOptions.GetSection("email").Value;
                        password = adminOptions.GetSection("password").Value;
                    }
                    IdentityResult result = await userManager.CreateAsync(admin, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail initialize admin");
            }
        }
        public async Task InitializerBaseOrderStatus()
        {
            try
            {
                var oldOrderStatuses = _storeContext.OrderStatuses.ToList();
                List<OrderStatus> newOrderStatuses = new List<OrderStatus>();
                if (oldOrderStatuses.Where(os => os.Id == 1).FirstOrDefault() == null)
                {
                    newOrderStatuses.Add(new OrderStatus() { Id = 1, Status = "Бронь" });
                }
                if (oldOrderStatuses.Where(os => os.Id == 2).FirstOrDefault() == null)
                {
                    newOrderStatuses.Add(new OrderStatus() { Id = 2, Status = "Поставлен" });
                }
                if (oldOrderStatuses.Where(os => os.Id == 3).FirstOrDefault() == null)
                {
                    newOrderStatuses.Add(new OrderStatus() { Id = 3, Status = "Выдан" });
                }
                if (oldOrderStatuses.Where(os => os.Id == 4).FirstOrDefault() == null)
                {
                    newOrderStatuses.Add(new OrderStatus() { Id = 4, Status = "Отменен" });
                }
                if (newOrderStatuses.Count() > 0)
                {
                    _storeContext.AddRange(newOrderStatuses);
                    _storeContext.Database.OpenConnection();
                    _storeContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.OrderStatuses ON");
                    await _storeContext.SaveChangesAsync();
                    _storeContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.OrderStatuses OFF");
                    _storeContext.Database.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail initialize order statuses");
            }
        }
        public async Task InitializerDefaultAttributes()
        {
            try
            {
                var productAttribute = _storeContext.ProductsAttributes.Where(pa => pa.Id == 1).FirstOrDefault();
                if (productAttribute == null)
                {
                    productAttribute = new ProductsAttribute() { Id = 1, Name = "DEFAULT" };
                    _storeContext.AddRange(productAttribute);
                    _storeContext.Database.OpenConnection();
                    _storeContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.ProductsAttributes ON");
                    await _storeContext.SaveChangesAsync();
                    _storeContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.ProductsAttributes OFF");
                    _storeContext.Database.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail initialize attributes");
            }
        }
        public async Task InitializerDefaultProductType()
        {
            try
            {
                var oldProductTypes = await _storeContext.ProductsTypes.Where(pt => pt.Id == 1 || pt.Id == 2).ToListAsync();
                List<ProductsType> newProductTypes = new List<ProductsType>();
                if (oldProductTypes.Where(pt => pt.Id == 1).FirstOrDefault() == null)
                {
                    newProductTypes.Add(new ProductsType() { Id = 1, Name = "Это родительский тип", HexColor = "fcba03", ParentProductTypeId = 1 });
                }
                if (oldProductTypes.Where(pt => pt.Id == 2).FirstOrDefault() == null)
                {
                    newProductTypes.Add(new ProductsType() { Id = 2, Name = "Без родителя", HexColor = "fcba03", ParentProductTypeId = 1 });
                }
                if (newProductTypes.Count > 0)
                {
                    _storeContext.AddRange(newProductTypes);
                    _storeContext.Database.OpenConnection();
                    _storeContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.ProductsTypes ON");
                    await _storeContext.SaveChangesAsync();
                    newProductTypes[0].ParentProductTypeId = 2;
                    _storeContext.Update(newProductTypes[0]);
                    await _storeContext.SaveChangesAsync();
                    _storeContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.ProductsTypes OFF");
                    _storeContext.Database.CloseConnection();
                }
                    oldProductTypes[0].ParentProductTypeId = 2;
                    _storeContext.Update(oldProductTypes[0]);
                    await _storeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail initialize product types");
            }
        }
    }
}
