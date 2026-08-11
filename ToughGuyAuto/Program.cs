using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToughGuyAuto.BLL.Interfaces;
using ToughGuyAuto.BLL.Services;
using ToughGuyAuto.DAL.Data;
using ToughGuyAuto.DAL.Interfaces;
using ToughGuyAuto.DAL.Repositories;
using ToughGuyAuto.Models;

namespace ToughGuyAuto
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register the EF Core DbContext
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<ToughGuyAutoDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            /* Configure ASP.NET Core Identity.
               1. ApplicationUser is the user class used by this application.
               2. AddRoles allows users to be assigned the Admin or User role.
               3. AddEntityFrameworkStores tells Identity to store its data in the ToughGuyAuto database.*/
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ToughGuyAutoDbContext>();

            /* Register the repository interfaces and their implementations.
               When a class asks for IVehicleRepository, dependency injection creates and provides a VehicleRepository object.
               Scoped means one instance is used during one HTTP request. */
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<IMaintenanceRecordRepository,
                MaintenanceRecordRepository>();
            builder.Services.AddScoped<IServiceTypeRepository,
                ServiceTypeRepository>();

            // Register the BLL service interfaces and implementations.
            // Controllers depend on the service interfaces, not directly creating the service classes.
            builder.Services.AddScoped<IVehicleService, VehicleService>();
            builder.Services.AddScoped<IMaintenanceRecordService,
                MaintenanceRecordService>();
            builder.Services.AddScoped<IServiceTypeService,
                ServiceTypeService>();

            var app = builder.Build();

            // Create a temporary dependency injection scope.
            // DbInitializer applies migrations and creates the initial roles and test users when the application starts.
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                await DbInitializer.InitializeAsync(services);
            }

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

            // Identify the user who is already login from the authentication cookie.
            // Check whether the user has permission to access the endpoint.
            app.UseAuthentication();
            app.UseAuthorization();

            // Configure the default MVC route.
            // For example, /Vehicles/Details/1 calls the Details action in VehiclesController and passes 1 as the ID.
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            await app.RunAsync();
        }
    }
}
