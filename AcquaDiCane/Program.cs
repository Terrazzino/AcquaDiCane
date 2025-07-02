using AcquaDiCane.Data;
using AcquaDiCane.Models.Identity; // Aseg�rate de que esta sea la ruta correcta para AplicationUser
using AcquaDiCane.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // A�adir este using para ILogger
using Microsoft.AspNetCore.Mvc; // A�adir este using para AutoValidateAntiforgeryTokenAttribute
using Microsoft.AspNetCore.Builder; // Aseg�rate de tener este using para WebApplicationBuilder

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<Contexto>(options =>
    options.UseSqlServer(connectionString));

// Importante: Usar AplicationUser (con una sola 'p') si as� est� definida en tu modelo
builder.Services.AddDefaultIdentity<AplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<Contexto>();

// ****** INICIO DE LAS CORRECCIONES ******

// 1. Configuraci�n de CORS (Cross-Origin Resource Sharing)
// Esto permite que tu API acepte solicitudes desde tu frontend en un puerto diferente.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendOrigin", // Nombre de la pol�tica CORS
        builder => builder.WithOrigins("http://localhost:5050") // 
                            .AllowAnyHeader() // Permite cualquier encabezado (Content-Type, Authorization, etc.)
                            .AllowAnyMethod() // Permite cualquier m�todo HTTP (GET, POST, PUT, DELETE)
                            .AllowCredentials()); // Permite el env�o de cookies o encabezados de autorizaci�n (ej. JWT)
});

// 2. Configuraci�n de Anti-Forgery Tokens (para [ValidateAntiForgeryToken])
// Si planeas usar [ValidateAntiForgeryToken] en tus controladores de API, debes configurar el servicio.
// Esto permite que ASP.NET Core maneje la generaci�n y validaci�n de tokens CSRF.
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN"; // Nombre de encabezado personalizado si tu frontend lo usa, sino es opcional.
});

// 3. Configuraci�n de MVC y controladores de API
// Para habilitar AutoValidateAntiforgeryTokenAttribute globalmente para m�todos POST, PUT, DELETE, etc.
// Opcional: Si solo lo quieres en algunos m�todos, puedes quitar esta l�nea y mantener el atributo [ValidateAntiForgeryToken]
// directamente en los m�todos de tu controlador.
builder.Services.AddControllersWithViews(options =>
{
    // ===> COMENTA ESTA L�NEA POR AHORA <===
    // Para APIs que reciben JSON y usan autenticaci�n basada en tokens, esta validaci�n autom�tica
    // a menudo no es necesaria y puede causar 400 Bad Request si el token CSRF no se env�a.
    // options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});


builder.Services.AddRazorPages();

// 4. Configuraci�n de pol�ticas de autorizaci�n
// Estas pol�ticas ya estaban aqu�, las mantengo en la ubicaci�n correcta.
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

// ===> IMPORTANTE: Coloca UseCors DESPU�S de UseRouting y ANTES de UseAuthentication/UseAuthorization <===
app.UseCors("AllowFrontendOrigin"); // Usa el nombre de la pol�tica CORS definida arriba

app.UseAuthentication(); // Middleware de autenticaci�n
app.UseAuthorization();  // Middleware de autorizaci�n

// ****** INICIO DE LAS CORRECCIONES EN EL PIPELINE ******

// 5. Mapeo de Endpoints: Deben ir DESPU�S de UseRouting, UseAuthentication y UseAuthorization.
// Has duplicado app.MapControllerRoute. Solo debe haber una secci�n de mapeo de rutas.
// He consolidado tus mapeos de rutas aqu�.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"); // Ruta predeterminada

// Ruta espec�fica para el peluquero, si la necesitas y no est� cubierta por la default.
// Aseg�rate de que no haya conflicto con la ruta por defecto o considera si es realmente necesaria.
// Si tu PeluqueroController es solo para vistas y el frontend navega a URLs como /Peluquero/Calendario,
// esta ruta es �til.
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