using AcquaDiCane.Data;
using AcquaDiCane.Models.Identity; // Asegúrate de que esta sea la ruta correcta para AplicationUser
using AcquaDiCane.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Añadir este using para ILogger
using Microsoft.AspNetCore.Mvc; // Añadir este using para AutoValidateAntiforgeryTokenAttribute
using Microsoft.AspNetCore.Builder; // Asegúrate de tener este using para WebApplicationBuilder

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<Contexto>(options =>
    options.UseSqlServer(connectionString));

// Importante: Usar AplicationUser (con una sola 'p') si así está definida en tu modelo
builder.Services.AddDefaultIdentity<AplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<Contexto>();

// ****** INICIO DE LAS CORRECCIONES ******

// 1. Configuración de CORS (Cross-Origin Resource Sharing)
// Esto permite que tu API acepte solicitudes desde tu frontend en un puerto diferente.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendOrigin", // Nombre de la política CORS
        builder => builder.WithOrigins("http://localhost:5050") // 
                            .AllowAnyHeader() // Permite cualquier encabezado (Content-Type, Authorization, etc.)
                            .AllowAnyMethod() // Permite cualquier método HTTP (GET, POST, PUT, DELETE)
                            .AllowCredentials()); // Permite el envío de cookies o encabezados de autorización (ej. JWT)
});

// 2. Configuración de Anti-Forgery Tokens (para [ValidateAntiForgeryToken])
// Si planeas usar [ValidateAntiForgeryToken] en tus controladores de API, debes configurar el servicio.
// Esto permite que ASP.NET Core maneje la generación y validación de tokens CSRF.
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN"; // Nombre de encabezado personalizado si tu frontend lo usa, sino es opcional.
});

// 3. Configuración de MVC y controladores de API
// Para habilitar AutoValidateAntiforgeryTokenAttribute globalmente para métodos POST, PUT, DELETE, etc.
// Opcional: Si solo lo quieres en algunos métodos, puedes quitar esta línea y mantener el atributo [ValidateAntiForgeryToken]
// directamente en los métodos de tu controlador.
builder.Services.AddControllersWithViews(options =>
{
    // ===> COMENTA ESTA LÍNEA POR AHORA <===
    // Para APIs que reciben JSON y usan autenticación basada en tokens, esta validación automática
    // a menudo no es necesaria y puede causar 400 Bad Request si el token CSRF no se envía.
    // options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});


builder.Services.AddRazorPages();

// 4. Configuración de políticas de autorización
// Estas políticas ya estaban aquí, las mantengo en la ubicación correcta.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequirePeluqueroRole", policy => policy.RequireRole("Peluquero"));
    options.AddPolicy("RequireAdministradorRole", policy => policy.RequireRole("Administrador"));
});

// ****** FIN DE LAS CORRECCIONES EN SERVICIOS ******


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseMigrationsEndPoint(); // Puedes habilitarlo en desarrollo si lo necesitas
    app.UseDeveloperExceptionPage(); // Muestra errores detallados en desarrollo
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ===> IMPORTANTE: Coloca UseCors DESPUÉS de UseRouting y ANTES de UseAuthentication/UseAuthorization <===
app.UseCors("AllowFrontendOrigin"); // Usa el nombre de la política CORS definida arriba

app.UseAuthentication(); // Middleware de autenticación
app.UseAuthorization();  // Middleware de autorización

// ****** INICIO DE LAS CORRECCIONES EN EL PIPELINE ******

// 5. Mapeo de Endpoints: Deben ir DESPUÉS de UseRouting, UseAuthentication y UseAuthorization.
// Has duplicado app.MapControllerRoute. Solo debe haber una sección de mapeo de rutas.
// He consolidado tus mapeos de rutas aquí.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"); // Ruta predeterminada

// Ruta específica para el peluquero, si la necesitas y no está cubierta por la default.
// Asegúrate de que no haya conflicto con la ruta por defecto o considera si es realmente necesaria.
// Si tu PeluqueroController es solo para vistas y el frontend navega a URLs como /Peluquero/Calendario,
// esta ruta es útil.
app.MapControllerRoute(
    name: "groomer",
    pattern: "Peluquero/{action=Calendario}/{id?}",
    defaults: new { controller = "Peluquero" });

app.MapRazorPages();

// ****** FIN DE LAS CORRECCIONES EN EL PIPELINE ******


// Bloque para inicializar la base de datos y sembrar roles/usuarios
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Contexto>();
        // Importante: Usar AplicationUser (con una sola 'p') aquí también
        var userManager = services.GetRequiredService<UserManager<AplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        await DbInitializer.Initialize(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al sembrar la base de datos.");
    }
}

app.Run();