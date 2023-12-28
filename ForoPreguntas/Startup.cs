using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ForoPreguntas.Models;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Otros servicios...

        services.AddControllersWithViews();

        // Agregar configuración de la sesión
        services.AddDistributedMemoryCache(); // Este servicio es necesario para almacenar las sesiones en la memoria
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // Puedes ajustar el tiempo de expiración según tus necesidades
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddDbContext<FOROPREGUNTASContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("cadenaSQL")));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Resto del código...

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        // Configuración de la sesión
        app.UseSession();

        // Resto del código...
    }
}
