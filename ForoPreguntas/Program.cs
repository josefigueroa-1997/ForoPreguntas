using ForoPreguntas.Models;
using Microsoft.EntityFrameworkCore;
using ForoPreguntas.Filter;
using ForoPreguntas.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<CargarCarreras>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<CargarCarreras>();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SidebarService>();
builder.Services.AddScoped<Imagen>();
builder.Services.AddScoped<PreguntaServices>();
builder.Services.AddScoped<RespuestaService>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1800);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

});
builder.Services.AddDbContext<FOROPREGUNTASContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQL")));


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
app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
