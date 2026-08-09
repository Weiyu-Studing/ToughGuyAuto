using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToughGuyAuto.DAL.Data;
using ToughGuyAuto.Data;
using Microsoft.EntityFrameworkCore;
using ToughGuyAuto.BLL.Interfaces;
using ToughGuyAuto.BLL.Services;
using ToughGuyAuto.DAL.Data;
using ToughGuyAuto.DAL.Interfaces;
using ToughGuyAuto.DAL.Repositories;

namespace ToughGuyAuto
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ToughGuyAutoDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));

            //Identity
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ToughGuyAutoDbContext>();

            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<IMaintenanceRecordRepository,
                MaintenanceRecordRepository>();
            builder.Services.AddScoped<IServiceTypeRepository,
                ServiceTypeRepository>();

            builder.Services.AddScoped<IVehicleService, VehicleService>();
            builder.Services.AddScoped<IMaintenanceRecordService,
                MaintenanceRecordService>();
            builder.Services.AddScoped<IServiceTypeService,
                ServiceTypeService>();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
