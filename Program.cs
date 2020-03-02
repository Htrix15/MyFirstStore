using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyFirstStore.Models;
using Microsoft.AspNetCore.Identity;
using MyFirstStore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MyFirstStore
{
    public class Program
    {
        public static bool dbCreared = true;
        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<DBInitializer>();
                    dbCreared = db.CreateDb();
                    if (dbCreared)
                    {
                        var userManager = services.GetRequiredService<UserManager<User>>();
                        var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                        await db.InitializeRolesAsync(rolesManager);
                        await db.InitializerAdminUser(userManager);
                        await db.InitializerBaseOrderStatus();
                        await db.InitializerDefaultAttributes();
                        await db.InitializerDefaultProductType();
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Initialization database error");
                    dbCreared = false;
                }
            }
            if (dbCreared)
            {
                host.Run();
            }
           
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
