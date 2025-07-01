using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // Para acceder a los claims del usuario

namespace AcquaDiCane.Controllers
{
    // Aplica autorización para que solo los peluqueros puedan acceder
    [Authorize(Roles = "Peluquero")]
    public class PeluqueroController : Controller
    {
        // Si necesitas el Contexto de la base de datos para algo en el futuro, inyéctalo aquí
        // private readonly ApplicationDbContext _context;
        // public PeluqueroController(ApplicationDbContext context)
        // {
        //     _context = context;
        // }

        public IActionResult Calendario()
        {
            // Puedes pasar el nombre del peluquero a la vista si lo necesitas
            // El nombre del peluquero podría venir de los claims de autenticación
            // string groomerName = User.FindFirstValue(ClaimTypes.Name); // O un claim personalizado
            // ViewData["GroomerName"] = groomerName ?? "Peluquero"; // Si no se encuentra el nombre

            return View(); // Esto renderizará Views/Peluquero/Calendario.cshtml
        }

        // Aquí podrías añadir otras acciones si el peluquero necesita una página de perfil, etc.

    }
}