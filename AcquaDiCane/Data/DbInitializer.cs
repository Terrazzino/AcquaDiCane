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
            string adminUserName = "admin@gmail.com"; // Email de acceso
            string adminPassword = "Abc_1234"; // Contraseña fuerte (¡CÁMBIALA EN PRODUCCIÓN!)

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

        }
    }
}