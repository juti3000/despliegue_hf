using Microsoft.EntityFrameworkCore;
using Habitforger.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

var mysqlConnectionString = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder
{
    Server = Environment.GetEnvironmentVariable("db_host"),
    Port = uint.Parse(Environment.GetEnvironmentVariable("db_port") ?? "3306"),
    UserID = Environment.GetEnvironmentVariable("db_user"),
    Password = Environment.GetEnvironmentVariable("db_password"),
    Database = Environment.GetEnvironmentVariable("db_name"),
    SslMode = MySql.Data.MySqlClient.MySqlSslMode.Required,
}.ConnectionString;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        mysqlConnectionString,
        new MySqlServerVersion(new Version(8, 0, 36))
    ));

// Configuración de autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiración de la cookie
        options.SlidingExpiration = true; // Renovar la cookie mientras esté activa
    });

// Otros servicios
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuración del pipeline de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANTE: UseAuthentication debe ir antes de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();