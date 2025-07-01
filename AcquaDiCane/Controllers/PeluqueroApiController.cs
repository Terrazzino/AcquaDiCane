using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AcquaDiCane.Data; // Para tu Contexto
using AcquaDiCane.Models; // Para tu modelo Peluquero
using AcquaDiCane.Models.Identity; // Para tu AplicationUser
using Microsoft.AspNetCore.Identity; // Para UserManager
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // Para obtener el ID del usuario
using System.Collections.Generic; // Para ICollection
using System; // Para TimeSpan y DateTime

namespace AcquaDiCane.Web.Controllers // Ajusta el namespace si es diferente
{
    // Solo los administradores pueden gestionar peluqueros
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/[controller]")] // La ruta será /api/PeluqueroApi
    public class PeluqueroApiController : ControllerBase
    {
        private readonly Contexto _context;
        private readonly UserManager<AplicationUser> _userManager;

        public PeluqueroApiController(Contexto context, UserManager<AplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Helper para obtener el usuario actual de Identity (si lo necesitas, aunque el [Authorize] ya lo valida)
        private async Task<AplicationUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(User);


        // ----------------------------------------
        // READ (GET) - Obtener todos los peluqueros
        // GET: api/PeluqueroApi
        [HttpGet]
        public async Task<IActionResult> GetPeluqueros()
        {
            var peluqueros = await _context.Peluqueros
                .Include(p => p.AplicationUser)
                .Include(p => p.JornadaSemanal) // Incluir las jornadas diarias
                .Select(p => new
                {
                    p.Id,
                    AplicationUserId = p.AplicationUser.Id, // Incluir el AplicationUserId
                    Nombre = p.AplicationUser.Nombre,
                    Apellido = p.AplicationUser.Apellido,
                    DNI = p.AplicationUser.DNI,
                    Email = p.AplicationUser.Email,
                    PhoneNumber = p.AplicationUser.PhoneNumber,
                    FechaNacimiento = p.AplicationUser.FechaNacimiento.HasValue ?
                                      p.AplicationUser.FechaNacimiento.Value.ToString("yyyy-MM-dd") :
                                      null, // O string.Empty, según cómo quieras representar una fecha nula en tu frontend
                    JornadasSemanales = p.JornadaSemanal.Select(j => new JornadaDiariaApiModel
                    {
                        Dia = j.Dia,
                        HoraInicio = j.HoraInicio.ToString(@"HH\:mm"), // Formato HH:mm
                        HoraFinal = j.HoraFinal.ToString(@"HH\:mm")    // Formato HH:mm
                    }).ToList()
                })
                .ToListAsync();
            return Ok(peluqueros);
        }

        // READ (GET) - Obtener un peluquero por ID
        // GET: api/PeluqueroApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPeluquero(int id)
        {
            var peluquero = await _context.Peluqueros
                .Include(p => p.AplicationUser)
                .Include(p => p.JornadaSemanal) // Incluir las jornadas diarias
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    AplicationUserId = p.AplicationUser.Id, // Incluir el AplicationUserId
                    Nombre = p.AplicationUser.Nombre,
                    Apellido = p.AplicationUser.Apellido,
                    DNI = p.AplicationUser.DNI,
                    Email = p.AplicationUser.Email,
                    PhoneNumber = p.AplicationUser.PhoneNumber,
                    FechaNacimiento = p.AplicationUser.FechaNacimiento.HasValue ?
                                      p.AplicationUser.FechaNacimiento.Value.ToString("yyyy-MM-dd") :
                                      null, // O string.Empty, según cómo quieras representar una fecha nula en tu frontend
                    JornadasSemanales = p.JornadaSemanal.Select(j => new JornadaDiariaApiModel
                    {
                        Dia = j.Dia,
                        HoraInicio = j.HoraInicio.ToString(@"HH\:mm"), // Formato HH:mm
                        HoraFinal = j.HoraFinal.ToString(@"HH\:mm")    // Formato HH:mm
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (peluquero == null)
            {
                return NotFound("Peluquero no encontrado.");
            }
            return Ok(peluquero);
        }

        // ----------------------------------------
        // CREATE (POST) - Crear un nuevo peluquero
        // POST: api/PeluqueroApi
        [HttpPost]
        public async Task<IActionResult> CreatePeluquero([FromBody] PeluqueroCreateModel model)
        {
            // Validar que el email no esté ya en uso para un AplicationUser
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Ya existe un usuario registrado con este correo electrónico.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Crear el AplicationUser de Identity
            var user = new AplicationUser
            {
                UserName = model.Email, // Usamos el email como UserName
                Email = model.Email,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                DNI = model.DNI,
                PhoneNumber = model.PhoneNumber,
                FechaNacimiento = model.FechaNacimiento
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors.Select(e => e.Description));
            }

            // 2. Asignar el rol de "Peluquero"
            var roleResult = await _userManager.AddToRoleAsync(user, "Peluquero");
            if (!roleResult.Succeeded)
            {
                // Si falla la asignación de rol, intentamos borrar el usuario creado para limpiar
                await _userManager.DeleteAsync(user);
                return BadRequest(roleResult.Errors.Select(e => e.Description));
            }

            // 3. Crear el Perfil de Peluquero en tu tabla 'Peluqueros'
            var peluquero = new Peluquero
            {
                AplicationUserId = user.Id, // Vincula con el Id del AplicationUser
                JornadaSemanal = new List<JornadaDiaria>() // Inicializar la colección
            };

            // 4. Asignar las jornadas semanales
            foreach (var jornadaModel in model.JornadasSemanales)
            {
                if (TimeSpan.TryParse(jornadaModel.HoraInicio, out TimeSpan horaInicio) &&
                    TimeSpan.TryParse(jornadaModel.HoraFinal, out TimeSpan horaFinal))
                {
                    if (horaInicio < horaFinal) // Basic validation: start time must be before end time
                    {
                        peluquero.JornadaSemanal.Add(new JornadaDiaria
                        {
                            Dia = jornadaModel.Dia,
                            HoraInicio = horaInicio,
                            HoraFinal = horaFinal
                            // PeluqueroId will be set automatically by EF Core when saving the peluquero
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("JornadasSemanales", $"La hora de inicio debe ser anterior a la hora final para el día {jornadaModel.Dia}.");
                    }
                }
                else
                {
                    ModelState.AddModelError("JornadasSemanales", $"Formato de hora inválido para el día {jornadaModel.Dia}. Use HH:mm.");
                }
            }

            if (!ModelState.IsValid)
            {
                await _userManager.DeleteAsync(user); // Borrar el usuario creado si hay errores de jornada
                return BadRequest(ModelState);
            }

            _context.Peluqueros.Add(peluquero);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPeluquero), new { id = peluquero.Id }, new { message = "Peluquero creado con éxito.", peluqueroId = peluquero.Id, userId = user.Id });
        }

        // ----------------------------------------
        // UPDATE (PUT) - Actualizar un peluquero existente
        // PUT: api/PeluqueroApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePeluquero(int id, [FromBody] PeluqueroUpdateModel model)
        {
            if (id != model.Id)
            {
                return BadRequest("El ID del peluquero en la URL no coincide con el ID en el cuerpo de la solicitud.");
            }

            var peluquero = await _context.Peluqueros
                                .Include(p => p.AplicationUser)
                                .Include(p => p.JornadaSemanal) // Crucial for updating nested collection
                                .FirstOrDefaultAsync(p => p.Id == id);

            if (peluquero == null)
            {
                return NotFound("Peluquero no encontrado.");
            }

            // Validar si el email ha cambiado y si el nuevo email ya está en uso por otro usuario
            if (model.Email != peluquero.AplicationUser.Email)
            {
                var existingUserWithNewEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingUserWithNewEmail != null && existingUserWithNewEmail.Id != peluquero.AplicationUser.Id)
                {
                    ModelState.AddModelError("Email", "Este correo electrónico ya está en uso por otro usuario.");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Actualizar datos del AplicationUser
            var user = peluquero.AplicationUser;
            user.Nombre = model.Nombre;
            user.Apellido = model.Apellido;
            user.DNI = model.DNI;
            user.Email = model.Email;
            user.UserName = model.Email; // Mantener UserName e Email sincronizados
            user.NormalizedEmail = _userManager.NormalizeEmail(model.Email);
            user.NormalizedUserName = _userManager.NormalizeName(model.Email);
            user.PhoneNumber = model.PhoneNumber;
            user.FechaNacimiento = model.FechaNacimiento;
            user.EmailConfirmed = model.EstaActivo; // Actualiza el estado activo del usuario de Identity

            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                return BadRequest(userUpdateResult.Errors.Select(e => e.Description));
            }

            // **Actualizar las Jornadas Semanales**
            // Eliminar las jornadas existentes para este peluquero
            _context.JornadasSemanales.RemoveRange(peluquero.JornadaSemanal);
            peluquero.JornadaSemanal.Clear(); // Limpiar la colección en memoria

            // Añadir las nuevas jornadas
            foreach (var jornadaModel in model.JornadasSemanales)
            {
                if (TimeSpan.TryParse(jornadaModel.HoraInicio, out TimeSpan horaInicio) &&
                    TimeSpan.TryParse(jornadaModel.HoraFinal, out TimeSpan horaFinal))
                {
                    if (horaInicio < horaFinal)
                    {
                        peluquero.JornadaSemanal.Add(new JornadaDiaria
                        {
                            Dia = jornadaModel.Dia,
                            HoraInicio = horaInicio,
                            HoraFinal = horaFinal
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("JornadasSemanales", $"La hora de inicio debe ser anterior a la hora final para el día {jornadaModel.Dia}.");
                    }
                }
                else
                {
                    ModelState.AddModelError("JornadasSemanales", $"Formato de hora inválido para el día {jornadaModel.Dia}. Use HH:mm.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Si hubo errores de parseo de tiempo o validación, regresamos Bad Request
                return BadRequest(ModelState);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PeluqueroExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 No Content para una actualización exitosa sin contenido de retorno
        }

        // ----------------------------------------
        // DELETE (DELETE) - Eliminar un peluquero
        // DELETE: api/PeluqueroApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePeluquero(int id)
        {
            var peluquero = await _context.Peluqueros.Include(p => p.AplicationUser).FirstOrDefaultAsync(p => p.Id == id);
            if (peluquero == null)
            {
                return NotFound("Peluquero no encontrado.");
            }

            // Eliminar primero las jornadas diarias asociadas al peluquero
            _context.JornadasSemanales.RemoveRange(peluquero.JornadaSemanal);
            await _context.SaveChangesAsync(); // Guardar cambios para asegurar que las FKs de jornadas se liberan

            // Eliminar el perfil de Peluquero de tu tabla
            _context.Peluqueros.Remove(peluquero);
            await _context.SaveChangesAsync(); // Guardar cambios para asegurar que la FK de Peluquero se libera

            // Luego eliminar el AplicationUser de Identity
            var user = peluquero.AplicationUser;
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors.Select(e => e.Description));
                }
            }

            return NoContent(); // 204 No Content para una eliminación exitosa
        }

        private bool PeluqueroExists(int id)
        {
            return _context.Peluqueros.Any(e => e.Id == id);
        }
    }

    // ----------------------------------------
    // Modelos para la creación y actualización de Peluqueros (ViewModels/DTOs)
    // IDEALMENTE: Mueve estos modelos a una carpeta 'DTOs' o 'ViewModels'
    public class JornadaDiariaApiModel
    {
        public string Dia { get; set; } // Ej: "Lunes", "Martes"
        public string HoraInicio { get; set; } // Ej: "09:00"
        public string HoraFinal { get; set; } // Ej: "18:00"
    }

    public class PeluqueroCreateModel
    {
        // Propiedades para AplicationUser
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Password { get; set; } // Contraseña para el nuevo usuario

        // Nuevo: Colección para las jornadas diarias
        public ICollection<JornadaDiariaApiModel> JornadasSemanales { get; set; } = new List<JornadaDiariaApiModel>();
    }

    public class PeluqueroUpdateModel
    {
        public int Id { get; set; } // ID del perfil de Peluquero

        // Propiedades para AplicationUser
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime FechaNacimiento { get; set; }

        // Propiedades específicas de Peluquero
        public bool EstaActivo { get; set; } // Para activar/desactivar el peluquero

        // Nuevo: Colección para las jornadas diarias
        public ICollection<JornadaDiariaApiModel> JornadasSemanales { get; set; } = new List<JornadaDiariaApiModel>();
    }
}