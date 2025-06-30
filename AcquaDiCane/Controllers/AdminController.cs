using Microsoft.AspNetCore.Authorization; // Para restringir el acceso a administradores
using Microsoft.AspNetCore.Mvc; // Para las clases Controller y ActionResult

namespace AcquaDiCane.Web.Controllers // Ajusta el namespace a tu proyecto
{
    // [Authorize(Roles = "Admin")] // Descomenta esta línea cuando tengas roles configurados
    // Si no tienes roles, puedes usar [Authorize] para requerir que el usuario esté logueado
    [Authorize(Roles = "Administrador")] // Requiere que el usuario esté autenticado para acceder a estas acciones
    public class AdminController : Controller
    {
        // Acción para la página principal del dashboard de administración
        public IActionResult Inicio()
        {
            // Acción para la página de gestión de servicios
            return View("~/Views/Admin/Dashboard.cshtml"); // Busca Views/Admin/Dashboard.cshtml
        }
        public IActionResult Services()
        {
            return View(); // Busca Views/Admin/Services.cshtml
        }

        // Acción para la página de gestión de peluqueros
        public IActionResult Groomers()
        {
            return View(); // Busca Views/Admin/Groomers.cshtml
        }

        // Si necesitas un login específico para admin (aunque ASP.NET Identity ya maneja esto)
        // [AllowAnonymous] // Permite el acceso sin autenticación
        // public IActionResult Login()
        // {
        //     return View(); // Buscaría Views/Admin/Login.cshtml
        // }
    }
}