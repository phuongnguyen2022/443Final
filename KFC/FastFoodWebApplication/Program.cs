using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FastFoodWebApplication.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Identity;
using FastFoodWebApplication.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using AspNetCore.Unobtrusive.Ajax;
using FastFoodWebApplication;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FastFoodWebApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FastFoodWebApplicationContext") ?? throw new InvalidOperationException("Connection string 'FastFoodWebApplicationContext' not found.")));

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<FastFoodWebApplicationContext>();
builder.Services.AddUnobtrusiveAjax();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "YourAppCookieName";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.LoginPath = "/Account/Login";
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;

});
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole<int>>>();
    await SeedData.SeedRole(roleManager);
}
app.UseStaticFiles();
app.UseUnobtrusiveAjax(); //It is suggested to place it after UseStaticFiles()
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
