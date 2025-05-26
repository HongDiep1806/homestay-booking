using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HomestayBooking.Models.DAL;
using HomestayBooking.Repositories;
using HomestayBooking.Service;
using HomestayBooking.Mappings;
using HomestayBooking.Models;

namespace HomestayBooking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //  AutoMapper
            builder.Services.AddAutoMapper(typeof(RoomProfile));
            builder.Services.AddAutoMapper(typeof(UserProfile));


            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            });



            //  Identity Configuration (AppUser + IdentityRole)
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
                options.SlidingExpiration = true;
            });

            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<IRoomService, RoomService>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
            builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            //
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddSession(); // Bật session


            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession(); // Kích hoạt session middleware


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
