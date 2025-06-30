using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AcquaDiCane.Controllers
{
    [Authorize(Roles = "Cliente")] // Solo los usuarios con rol "Cliente" pueden acceder
    public class ClienteController : Controller
    {
        public IActionResult Inicio() // Esta será la acción a la que rediriges
        {
            // Aquí puedes pasar un modelo si tu vista de dashboard lo necesita.
            // Por ejemplo, si quieres mostrar el nombre del cliente en el dashboard:
            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // var user = await _userManager.FindByIdAsync(userId); // Necesitarías inyectar UserManager
            // return View("~/Views/Client/Dashboard.cshtml", user.Nombre); // Pasa el nombre como modelo

            // Asumiendo que tu dashboard del cliente es una vista Razor:
            return View("~/Views/Client/Dashboard.cshtml"); //
        }
    }
}