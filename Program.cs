// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SportsStore
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SportsStore.Models;
    using SportsStore.Models.Repository;

#pragma warning disable S1118 // Utility classes should not have public constructors
    public class Program
#pragma warning restore S1118 // Utility classes should not have public constructors
    {
#pragma warning disable SA1600 // Elements should be documented
        public static void Main(string[] args)
#pragma warning restore SA1600 // Elements should be documented
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<StoreDbContext>(opts =>
                opts.UseSqlServer(builder.Configuration.GetConnectionString("SportsStoreConnection")));

            builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
            builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            builder.Services.AddScoped<Cart>(SessionCart.GetCart);
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(builder.Configuration["ConnectionStrings:IdentityConnection"]));
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();

            var app = builder.Build();

            if (app.Environment.IsProduction())
            {
                app.UseExceptionHandler("/Error");
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
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

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapDefaultControllerRoute();
            SeedData.EnsurePopulated(app);
            IdentitySeedData.EnsurePopulated(app);

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "privacy",
                pattern: "privacy",
                defaults: new { controller = "Home", action = "Privacy" });

            app.MapControllerRoute(
                name: "pagination",
                pattern: "Products/Page{productPage:int}",
                defaults: new { Controller = "Home", action = "Index", productPage = 1 });

            app.MapControllerRoute(
                name: "categoryPage",
                pattern: "{category}/Page{productPage:int}",
                defaults: new { Controller = "Home", action = "Index" });

            app.MapControllerRoute(
                name: "category",
                pattern: "Products/{category}",
                defaults: new { Controller = "Home", action = "Index", productPage = 1 });

            app.MapControllerRoute(
                name: "shoppingCart",
                pattern: "Cart",
                defaults: new { Controller = "Cart", action = "Index" });

            app.MapControllerRoute(
                name: "default",
                pattern: "/",
                defaults: new { Controller = "Home", action = "Index" });

            app.MapControllerRoute(
                "checkout",
                "Checkout",
                new { Controller = "Order", action = "Checkout" });

            app.MapControllerRoute(
                "remove",
                "Remove",
                new { Controller = "Cart", action = "Remove" });

            app.Run();
        }
    }
}
