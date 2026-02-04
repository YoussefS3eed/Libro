using Libro.BLL.Common;
using Libro.DAL.Common;
using Libro.PL.Mapper;
using Libro.PL.Settings;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;
namespace Libro.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ------------------- Logging -------------------
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // ------------------- Add Connection String and DbContext -------------------
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<LibroDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // ------------------- Identity -------------------
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<LibroDbContext>();

            // ------------------- Dependecy Injection -------------------
            builder.Services.AddDataAccessLayerInPL();
            builder.Services.AddBusinessLogicLayerInPL();
            builder.Services.AddAutoMapper(x => x.AddProfile<DomainProfile>());

            // ------------------- Add MVC -------------------
            builder.Services.AddControllersWithViews();
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));
            builder.Services.AddExpressiveAnnotations();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
                .WithStaticAssets();

            app.Run();
        }
    }
}
