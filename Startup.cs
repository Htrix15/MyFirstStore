using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFirstStore.Models;
using MyFirstStore.Services;

namespace MyFirstStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<StoreContext>(options => options.UseSqlServer(connection).EnableSensitiveDataLogging());
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
            services.AddIdentity<User, IdentityRole>(options => {
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<ApplicationContext>();
            services.AddSingleton<BuilderBackColorCSS>();
            services.AddHttpContextAccessor();
            services.AddScoped<ManagerBasketCook>();
            services.AddSingleton<CacheService>();
            services.AddScoped<PreOrderService>();
            services.AddScoped<FileOperations>();
            services.AddScoped<DataFiltering>();
            services.AddScoped<DataProcessingConveyor>();
            services.AddScoped<FcdStoreContext>();
            services.AddScoped<FcdUserAndSignManager>();
            services.AddScoped<DBInitializer>();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = ".MyShop.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(2);
                options.Cookie.IsEssential = true;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMemoryCache();
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.LoginPath = "/Account/Login";
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {

                routes.MapRoute(
                     name: "default",
                          template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
