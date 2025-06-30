using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using AcquaDiCane.Models.Identity; // Asegúrate de que este sea el namespace correcto para tu AplicationUser

namespace AcquaDiCane.Web.Controllers // O el namespace donde quieres ubicar tus API Controllers
{
    [Route("api/[controller]")] // Esto mapeará a /api/AccountApi
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly UserManager<AplicationUser> _userManager;

        public AccountApiController(UserManager<AplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/AccountApi/current-user-id
        [HttpGet("current-user-id")] // Esto mapeará a /api/AccountApi/current-user-id
        public async Task<ActionResult<string>> GetCurrentUserId()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Usuario no autenticado.");
            }

            var userId = _userManager.GetUserId(User); // Obtiene el ID del usuario de Identity

            if (userId == null)
            {
                // Esto podría ocurrir si el usuario está autenticado pero su ID no se puede obtener por alguna razón.
                return NotFound("ID de usuario no encontrado.");
            }

            return Ok(userId);
        }

        // Puedes agregar más endpoints aquí, como:
        // GET: api/AccountApi/current-user-role
        [HttpGet("current-user-role")]
        public async Task<ActionResult<string>> GetCurrentUserRole()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Usuario no autenticado.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            // Asume que un usuario solo tiene un rol principal para simplificar,
            // o devuelve una lista si esperas varios roles.
            var primaryRole = roles.FirstOrDefault();

            if (string.IsNullOrEmpty(primaryRole))
            {
                return NotFound("Rol de usuario no encontrado.");
            }

            return Ok(primaryRole);
        }
    }
}