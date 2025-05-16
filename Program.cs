using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchuelerCheckIN2025.Data;
using Microsoft.Extensions.DependencyInjection;
using SchuelerCheckIN2025.Models;

namespace SchuelerCheckIN2025
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // SQLite-Verbindung anstelle von MSSQL
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                                   ?? throw new InvalidOperationException("Connection string 'SchuelerCheckIN2025Context' not found.");

            // DbContext mit SQLite konfigurieren
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString)); // Verwende UseSqlite anstelle von UseSqlServer

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Identity Setup
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // MVC und Razor Pages
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Konfiguration der HTTP-Anforderungs-Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint(); // Ermöglicht das Migrations-Tool
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapPost("/api/anwesend", (ApplicationDbContext context, ScanData scan) =>
            {
                Schuelerdaten daten  = context.Schuelerdatenset.Where(s => s.schluessel.Equals(scan.Scan)).Single();
                daten.anwesend = true;
                context.SaveChanges();
            });

            // Standard-Routing
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }

}
