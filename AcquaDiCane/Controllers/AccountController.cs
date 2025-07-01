// Controllers/AccountController.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims; // No es estrictamente necesario aquí si no usas ClaimTypes directamente
using AcquaDiCane.ViewModels; // Tu namespace para LoginViewModel y RegisterViewModel
using AcquaDiCane.Models.Identity; // Para AplicationUser
using AcquaDiCane.Data; // Para tu Contexto (DbContext)
using AcquaDiCane.Models; // Para tu modelo Cliente
using Microsoft.AspNetCore.Authorization; // Para [AllowAnonymous]

namespace AcquaDiCane.Controllers
{
    [AllowAnonymous] // Permitir acceso a acciones de login/registro sin autenticar
    public class AccountController : Controller
    {
        private readonly UserManager<AplicationUser> _userManager;
        private readonly SignInManager<AplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly Contexto _context; // ¡IMPORTANTE! Necesitas tu DbContext aquí

        public AccountController(
            UserManager<AplicationUser> userManager,
            SignInManager<AplicationUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            Contexto context) // Inyecta el contexto aquí
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context; // Asigna el contexto
        }

        // GET: /Account/Login o /Account/Index (si esta es tu acción por defecto)
        // Esta acción solo sirve para redirigir a la vista principal si es necesario.
        // Si tu Home/Index ya carga los formularios, esta acción podría no ser necesaria
        // o podría ser solo para manejar un returnUrl en la URL.
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            // Si la vista Views/Home/Index.cshtml ya muestra los formularios de Login y Register
            // sin necesidad de un modelo en el controlador Home, puedes simplemente devolverla.
            // Si Home/Index necesita un modelo, ese modelo debe ser gestionado por Home/Index.
            return View("~/Views/Home/Index.cshtml");
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Pasa el returnUrl a la vista
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        // Redirección basada en el rol
                        if (await _userManager.IsInRoleAsync(user, "Administrador"))
                        {
                            return RedirectToLocal(returnUrl ?? "/Admin/Inicio"); // Ruta de tu dashboard de admin
                        }
                        else if (await _userManager.IsInRoleAsync(user, "Peluquero"))
                        {
                            return RedirectToLocal(returnUrl ?? "/Peluquero"); // Ruta de tu dashboard de peluquero
                        }
                        else if (await _userManager.IsInRoleAsync(user, "Cliente"))
                        {
                            return RedirectToLocal(returnUrl ?? "/Cliente/Inicio"); // Ruta de tu dashboard de cliente
                        }
                    }
                    // Si el usuario no tiene un rol específico, redirigir a una página predeterminada
                    return RedirectToLocal(returnUrl ?? "/Home/Index");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "La cuenta está bloqueada.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "El inicio de sesión no está permitido para este usuario.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión inválido. Email o contraseña incorrectos.");
                }
            }

            // Si el modelo no es válido o el login falla, vuelve a la vista de inicio
            // Es CRUCIAL que la vista Views/Home/Index.cshtml sepa cómo manejar los errores de ModelState.
            return View("~/Views/Home/Index.cshtml", model); // Pasamos el modelo de Login para que se mantengan los datos
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Pasa el returnUrl a la vista
            if (ModelState.IsValid)
            {
                // 1. Crear el AplicationUser (usuario de Identity)
                var user = new AplicationUser
                {
                    UserName = model.Email, // Usamos el email como UserName
                    Email = model.Email,
                    Nombre = model.Name,
                    Apellido = model.LastName,
                    PhoneNumber = model.PhoneNumber, // Asignar solo si tu AplicationUser tiene esta propiedad
                    DNI = model.DNI,
                    FechaNacimiento = model.BirthDate
                    // No asignar FechaNacimiento aquí a menos que AplicationUser también la tenga
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // 2. Asegurar que el rol "Cliente" exista y asignarlo
                    if (!await _roleManager.RoleExistsAsync("Cliente"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<int>("Cliente"));
                    }
                    await _userManager.AddToRoleAsync(user, "Cliente");

                    // 3. Crear el registro en la tabla Clientes
                    var cliente = new Cliente
                    {
                        AplicationUserId = user.Id, // Vincula al AplicationUser recién creado
                    };
                    _context.Clientes.Add(cliente); // Agrega el cliente al DbContext
                    await _context.SaveChangesAsync(); // Guarda los cambios en la BD

                    // 4. Iniciar sesión al usuario automáticamente después de registrarse
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // 5. Redirigir al dashboard de cliente
                    return RedirectToLocal(returnUrl ?? "/Cliente/Inicio");
                }
                // Si el registro de usuario falla, agregar los errores de Identity a ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Si el modelo no es válido o el registro falla, vuelve a la vista de inicio.
            // Es CRUCIAL que la vista Views/Home/Index.cshtml sepa cómo manejar los errores de ModelState
            // para el formulario de registro.
            return View("~/Views/Home/Index.cshtml", model); // Pasamos el modelo de Register para que se mantengan los datos
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login)); // Redirige a la acción GET Login de AccountController
        }

        // Helper para redirección segura (evita ataques de open redirect)
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home"); // Redirección por defecto si returnUrl no es local
            }
        }
    }
}