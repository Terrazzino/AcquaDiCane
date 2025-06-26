using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Necesario para .Migrate()
using AcquaDiCane.Models.Identity; // Para tu ApplicationUser, Cliente, Peluquero
using AcquaDiCane.Models; // Para otras entidades de negocio si las necesitaras en el seed (ej. MetodoDePago)
using System;
using System.Linq; // Necesario para .Select()
using System.Threading.Tasks;

namespace AcquaDiCane.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(
            Contexto context,
            UserManager<AplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager) // Importante: RoleManager con IdentityRole<int>
        {
            // Opcional pero recomendado: Asegurar que las migraciones pendientes se apliquen al inicio.
            // Esto es útil en entornos de desarrollo o en la primera ejecución en producción.
            // Para entornos de producción más controlados, podrías ejecutar Update-Database manualmente.
            context.Database.Migrate();

            // 1. Definir los nombres de los roles que necesitamos
            string[] roleNames = { "Administrador", "Peluquero", "Cliente" };

            // 2. Crear los roles si no existen
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // Si el rol no existe en la base de datos, lo creamos.
                    // El IdentityRole<int> constructor toma el nombre del rol.
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                    Console.WriteLine($"Rol '{roleName}' creado."); // Mensaje de consola para depuración
                }
                else
                {
                    Console.WriteLine($"Rol '{roleName}' ya existe."); // Mensaje de consola para depuración
                }
            }

            // 3. Crear un usuario administrador inicial si no existe
            // Este será tu punto de entrada al sistema con privilegios completos.
            string adminUserName = "admin@aquadicane.com"; // Email de acceso
            string adminPassword = "PasswordSegura123!"; // Contraseña fuerte (¡CÁMBIALA EN PRODUCCIÓN!)

            var adminUser = await userManager.FindByNameAsync(adminUserName);

            if (adminUser == null)
            {
                // Si el usuario administrador no existe, lo creamos.
                adminUser = new AplicationUser
                {
                    UserName = adminUserName,
                    Email = adminUserName,
                    EmailConfirmed = true, // Marcamos el email como confirmado para que pueda iniciar sesión inmediatamente
                    // Puedes añadir más propiedades específicas de ApplicationUser aquí si lo deseas
                    Nombre = "Admin",
                    Apellido = "Sistema",
                    FechaNacimiento = DateTime.Parse("1990-01-01"), // Ejemplo
                    DNI = "00000000A" // Ejemplo
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    // Asignar el rol de "Administrador" al usuario recién creado
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                    Console.WriteLine($"Usuario administrador '{adminUserName}' creado y asignado al rol 'Administrador'.");
                }
                else
                {
                    // Manejo de errores si la creación del usuario administrador falla
                    Console.WriteLine($"Error al crear usuario administrador '{adminUserName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine($"Usuario administrador '{adminUserName}' ya existe.");
                // Opcional: Asegurarse de que el usuario administrador existente tenga el rol de Administrador
                if (!await userManager.IsInRoleAsync(adminUser, "Administrador"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                    Console.WriteLine($"Usuario '{adminUserName}' asignado al rol 'Administrador' (ya existía).");
                }
            }

            // Opcional: Puedes sembrar otros datos maestros si los tienes
            // Ejemplo: Métodos de pago
            /*
            if (!context.MetodosDePago.Any())
            {
                context.MetodosDePago.AddRange(
                    new MetodoDePago { NombreDelMetodo = "Efectivo" },
                    new MetodoDePago { NombreDelMetodo = "Tarjeta de Crédito" },
                    new MetodoDePago { NombreDelMetodo = "Mercado Pago" }
                );
                await context.SaveChangesAsync();
                Console.WriteLine("Métodos de pago sembrados.");
            }
            */

            // Opcional: Aquí podrías crear un Peluquero o Cliente de ejemplo si lo necesitas para pruebas iniciales,
            // y vincularlo a su perfil y ApplicationUser.
            // Para un peluquero de ejemplo:
            /*
            string peluqueroUserName = "peluquero1@aquadicane.com";
            var peluqueroUser = await userManager.FindByNameAsync(peluqueroUserName);

            if (peluqueroUser == null)
            {
                peluqueroUser = new ApplicationUser
                {
                    UserName = peluqueroUserName,
                    Email = peluqueroUserName,
                    EmailConfirmed = true,
                    Nombre = "Juan",
                    Apellido = "Perez",
                    FechaNacimiento = DateTime.Parse("1985-05-10"),
                    DNI = "12345678X"
                };

                var createPeluqueroResult = await userManager.CreateAsync(peluqueroUser, "PasswordPeluquero1!");
                if (createPeluqueroResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(peluqueroUser, "Peluquero");

                    // Crear el perfil de Peluquero y asociarlo al ApplicationUser
                    var peluqueroProfile = new Peluquero
                    {
                        AplicationUser = peluqueroUser, // EF Core vinculará esto automáticamente si el objeto ya está en el contexto
                        // Si no está, tendrías que asignar peluqueroUser.Id a AplicationUserId
                        // O, si quieres evitar cargar el usuario de nuevo:
                        // AplicationUserId = peluqueroUser.Id
                    };
                    context.Peluqueros.Add(peluqueroProfile);
                    await context.SaveChangesAsync(); // Guardar el perfil de peluquero
                    Console.WriteLine($"Usuario peluquero '{peluqueroUserName}' creado y asignado al rol 'Peluquero' con perfil.");
                }
                else
                {
                    Console.WriteLine($"Error al crear usuario peluquero '{peluqueroUserName}': {string.Join(", ", createPeluqueroResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                 Console.WriteLine($"Usuario peluquero '{peluqueroUserName}' ya existe.");
                 if (!await userManager.IsInRoleAsync(peluqueroUser, "Peluquero"))
                 {
                     await userManager.AddToRoleAsync(peluqueroUser, "Peluquero");
                     Console.WriteLine($"Usuario '{peluqueroUserName}' asignado al rol 'Peluquero' (ya existía).");
                 }
            }
            */

        }
    }
}