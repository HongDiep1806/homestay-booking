using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HomestayBooking.Models;
using HomestayBooking.Models.DAL;
using HomestayBooking.Repositories;
using HomestayBooking.Service;
using HomestayBooking.Mappings;

namespace HomestayBooking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 🗂 AutoMapper
            builder.Services.AddAutoMapper(typeof(RoomProfile));
            builder.Services.AddAutoMapper(typeof(UserProfile));


            // 🔌 Database context
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 🔐 Identity Configuration (AppUser + IdentityRole)
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // 🍪 Cookie lifespan for RememberMe (optional)
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
                options.SlidingExpiration = true;
            });

            // 💉 Dependency Injection
            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<IRoomService, RoomService>();

            // 🧱 MVC & Blazor (if used)
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            var app = builder.Build();

            // 🌐 Middleware Pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // 🔁 Route config
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
