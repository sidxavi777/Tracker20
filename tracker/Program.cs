
using System.Configuration;
using Database;
using Database.Data.IRepository;
using Database.Data.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using tracker;
using tracker.Areas.Identity.Pages.Account;
using Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//adding database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//added when scaffolding identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
//builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

//Razor page
builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath= "/Identity/Account/Login";

});
//microsoft login
builder.Services.AddAuthentication().AddMicrosoftAccount(options =>
{
    options.ClientId = "2f0c11bc-d66e-43ce-beb8-93951cb3ebdb";
    options.ClientSecret = "4qW8Q~DGjv2vjyW0D8ofVduI.cZG3uyB9z~ZxaMh";

});


//auth

//builder.Services.AddAuthentication("Cookies").AddCookie("Cookies", options =>
//{
//    options.LoginPath="/Login";
//});

//smtp
builder.Services.Configure<SMTP>(builder.Configuration.GetSection("SMTPConfig"));


//DI
builder.Services.AddScoped<ITracker, Tracker>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddSingleton<SMTP>(builder.Configuration.GetSection("SMTPConfig").Get<SMTP>());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();