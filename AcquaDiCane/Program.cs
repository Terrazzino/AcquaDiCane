using AcquaDiCane.Data;
using AcquaDiCane.Models.Identity; // Aseg�rate de que esta sea la ruta correcta para AplicationUser
using AcquaDiCane.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // A�adir este using para ILogger

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<Contexto>(options =>
    options.UseSqlServer(connectionString));

// Importante: Usar AplicationUser (con una sola 'p') si as� est� definida en tu modelo
builder.Services.AddDefaultIdentity<AplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<Contexto>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseMigrationsEndPoint(); // Puedes habilitarlo en desarrollo si lo necesitas
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();



app.UseAuthentication(); // Middleware de autenticaci�n
app.UseAuthorization();  // Middleware de autorizaci�n

// Mapeo de Endpoints (Solo una vez)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"); // <-- Aseg�rate de que apunte a Account/Login
app.MapRazorPages();

// Bloque para inicializar la base de datos y sembrar roles/usuarios
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Contexto>();
        // Importante: Usar AplicationUser (con una sola 'p') aqu� tambi�n
        var userManager = services.GetRequiredService<UserManager<AplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        await DbInitializer.Initialize(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurri� un error al sembrar la base de datos.");
    }
}

app.Run();