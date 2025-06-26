// Controllers/ClientApiController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AcquaDiCane.Data; // Para tu Contexto
using AcquaDiCane.Models; // Para tus modelos Mascota, Turno, Servicio, Peluquero, Cliente, DetalleDelTurno, Pago
using AcquaDiCane.Models.Identity; // Para tu AplicationUser
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IO; // Para Path y Directory
using System.Collections.Generic; // Para List<string>

[Authorize(Roles = "Cliente")]
[ApiController]
[Route("api/[controller]")] // La ruta será /api/ClientApi
public class ClientApiController : ControllerBase
{
    private readonly Contexto _context;

    public ClientApiController(Contexto context)
    {
        _context = context;
    }

    // Método auxiliar para obtener el perfil de cliente del usuario logueado
    private async Task<Cliente> GetClienteProfileAsync()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null || !int.TryParse(currentUserId, out int parsedCurrentUserId))
        {
            return null; // Usuario no autenticado o ID inválido
        }

        var clienteProfile = await _context.Clientes
                                           .FirstOrDefaultAsync(c => c.AplicationUserId == parsedCurrentUserId);
        return clienteProfile;
    }

    // --- Mascotas ---

    // GET: api/ClientApi/pets?ownerId=X
    [HttpGet("pets")]
    public async Task<IActionResult> GetPets(int ownerId)
    {
        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual o usuario no autenticado.");
        }

        if (clienteProfile.Id != ownerId)
        {
            return Unauthorized("No tienes permiso para ver estas mascotas.");
        }

        var mascotas = await _context.Mascotas
                                .Where(p => p.ClienteAsignadoId == ownerId)
                                .Select(p => new
                                {
                                    p.Id,
                                    p.Nombre,
                                    p.Tamaño,
                                    p.Sexo,
                                    p.Raza,
                                    p.SinRaza,
                                    p.Peso,
                                    p.Castrado,
                                    FechaNacimiento = p.FechaNacimiento.ToString("yyyy-MM-dd"),
                                    p.Alergico,
                                    p.UrlFotoPerfil
                                })
                                .ToListAsync();
        return Ok(mascotas);
    }

    // POST: api/ClientApi/pets
    [HttpPost("pets")]
    public async Task<IActionResult> AddPet([FromForm] Mascota model, IFormFile ProfilePic)
    {
        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual o usuario no autenticado.");
        }

        if (clienteProfile.Id != model.ClienteAsignadoId)
        {
            return Unauthorized("No tienes permiso para agregar una mascota a este usuario.");
        }

        if (ModelState.IsValid)
        {
            if (ProfilePic != null && ProfilePic.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pet_profiles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfilePic.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePic.CopyToAsync(fileStream);
                }
                model.UrlFotoPerfil = $"/uploads/pet_profiles/{uniqueFileName}";
            }
            else if (string.IsNullOrEmpty(model.UrlFotoPerfil))
            {
                model.UrlFotoPerfil = "/img/default_pet.png";
            }

            _context.Mascotas.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPets), new { ownerId = model.ClienteAsignadoId }, new { message = "Mascota registrada con éxito." });
        }
        return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }

    // DELETE: api/ClientApi/pets/5
    [HttpDelete("pets/{id}")]
    public async Task<IActionResult> DeletePet(int id)
    {
        var pet = await _context.Mascotas.FindAsync(id);
        if (pet == null)
        {
            return NotFound(new { message = "Mascota no encontrada." });
        }

        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual o usuario no autenticado.");
        }

        if (clienteProfile.Id != pet.ClienteAsignadoId)
        {
            return Unauthorized("No tienes permiso para eliminar esta mascota.");
        }

        if (!string.IsNullOrEmpty(pet.UrlFotoPerfil) && !pet.UrlFotoPerfil.Contains("default_pet.png"))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pet.UrlFotoPerfil.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        _context.Mascotas.Remove(pet);
        await _context.SaveChangesAsync();
        return NoContent();
    }


    // --- Turnos ---

    // GET: api/ClientApi/appointments?clientId=X
    [HttpGet("appointments")]
    public async Task<IActionResult> GetAppointments(int clientId)
    {
        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual o usuario no autenticado.");
        }

        if (clienteProfile.Id != clientId)
        {
            return Unauthorized("No tienes permiso para ver estos turnos.");
        }

        var appointments = await _context.Turnos
                                        .Include(a => a.MascotaAsignada)
                                        .Include(a => a.PeluqueroAsignado)
                                            .ThenInclude(p => p.AplicationUser)
                                        .Include(a => a.Detalles)
                                            .ThenInclude(dt => dt.ServicioAsignado)
                                        .Include(a => a.Pago)
                                        .Where(a => a.MascotaAsignada.ClienteAsignadoId == clientId)
                                        .OrderByDescending(a => a.FechaHoraDelTurno)
                                        .Select(a => new
                                        {
                                            a.Id,
                                            Fecha = a.FechaHoraDelTurno.ToString("yyyy-MM-dd"),
                                            Hora = a.FechaHoraDelTurno.ToString(@"hh\:mm"),
                                            NombrePeluquero = a.PeluqueroAsignado.AplicationUser.Nombre,
                                            NombresServicios = a.Detalles.Select(dt => dt.ServicioAsignado.nombreServicio).ToList(),
                                            a.PrecioTotal, // Ahora es double
                                            EstadoPago = a.Pago.Estado,
                                            NombreMascota = a.MascotaAsignada.Nombre
                                        })
                                        .ToListAsync();
        return Ok(appointments);
    }

    // GET: api/ClientApi/appointments/next?petId=X
    [HttpGet("appointments/next")]
    public async Task<IActionResult> GetNextAppointmentForPet(int petId)
    {
        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual o usuario no autenticado.");
        }

        var mascota = await _context.Mascotas.FirstOrDefaultAsync(p => p.Id == petId && p.ClienteAsignadoId == clienteProfile.Id);
        if (mascota == null)
        {
            return NotFound("Mascota no encontrada o no pertenece al usuario.");
        }

        var nextAppointment = await _context.Turnos
            .Include(a => a.Detalles)
                .ThenInclude(dt => dt.ServicioAsignado)
            .Include(a => a.PeluqueroAsignado)
                .ThenInclude(p => p.AplicationUser)
            .Include(a => a.Pago)
            .Where(a => a.MascotaAsignadaId == petId &&
                        (a.Pago.Estado == "Pendiente" || a.Pago.Estado == "Confirmado") &&
                        a.FechaHoraDelTurno >= DateTime.Now)
            .OrderBy(a => a.FechaHoraDelTurno)
            .Select(a => new
            {
                a.Id,
                Fecha = a.FechaHoraDelTurno.ToString("yyyy-MM-dd"),
                Hora = a.FechaHoraDelTurno.ToString(@"hh\:mm"),
                NombrePeluquero = a.PeluqueroAsignado.AplicationUser.Nombre,
                NombresServicios = a.Detalles.Select(dt => dt.ServicioAsignado.nombreServicio).ToList(),
                a.PrecioTotal, // Ahora es double
                EstadoPago = a.Pago.Estado
            })
            .FirstOrDefaultAsync();

        if (nextAppointment == null)
        {
            return NotFound();
        }

        return Ok(nextAppointment);
    }


    // POST: api/ClientApi/appointments
    [HttpPost("appointments")]
    public async Task<IActionResult> ScheduleAppointment([FromBody] Turno model)
    {
        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual o usuario no autenticado.");
        }

        var mascotaAsignada = await _context.Mascotas.FirstOrDefaultAsync(p => p.Id == model.MascotaAsignadaId && p.ClienteAsignadoId == clienteProfile.Id);
        if (mascotaAsignada == null)
        {
            ModelState.AddModelError("MascotaAsignadaId", "La mascota seleccionada no existe o no te pertenece.");
        }

        var groomer = await _context.Peluqueros.FirstOrDefaultAsync(g => g.Id == model.PeluqueroAsignadoId);
        if (groomer == null)
        {
            ModelState.AddModelError("PeluqueroAsignadoId", "El peluquero seleccionado no es válido.");
        }

        double totalPrecioServicios = 0; // Cambiado de decimal a double
        if (model.Detalles == null || !model.Detalles.Any())
        {
            ModelState.AddModelError("Detalles", "Se debe seleccionar al menos un servicio para el turno.");
        }
        else
        {
            var validatedDetails = new List<DetalleDelTurno>();
            foreach (var detalle in model.Detalles)
            {
                // ELIMINADO: el filtro por 's.EstaActivo'
                var servicio = await _context.Servicios.FirstOrDefaultAsync(s => s.Id == detalle.ServicioAsignadoId);
                if (servicio == null)
                {
                    ModelState.AddModelError($"Detalles", $"El servicio con ID {detalle.ServicioAsignadoId} no es válido.");
                    break;
                }
                else
                {
                    detalle.PrecioServicio = servicio.Precio; // Costo y Precio ahora son double
                    totalPrecioServicios += detalle.PrecioServicio;
                    validatedDetails.Add(detalle);
                }
            }
            model.Detalles = validatedDetails;
        }

        model.PrecioTotal = totalPrecioServicios; // PrecioTotal ahora es double

        if (model.FechaHoraDelTurno < DateTime.Now)
        {
            ModelState.AddModelError("FechaHoraDelTurno", "No se puede agendar un turno en el pasado.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        var pago = new Pago
        {
            Estado = "Pendiente",
            Monto = model.PrecioTotal, // Monto ahora es double
            FechaPago = null
        };

        model.Pago = pago;

        _context.Turnos.Add(model);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAppointments), new { clientId = mascotaAsignada.ClienteAsignadoId }, new { message = "Turno y pago asociados agendados con éxito." });
    }


    // PUT: api/ClientApi/appointments/5/cancel
    [HttpPut("appointments/{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        var appointment = await _context.Turnos
                                        .Include(a => a.MascotaAsignada)
                                        .Include(a => a.Pago)
                                        .FirstOrDefaultAsync(a => a.Id == id);
        if (appointment == null)
        {
            return NotFound(new { message = "Turno no encontrado." });
        }

        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual o usuario no autenticado.");
        }

        if (clienteProfile.Id != appointment.MascotaAsignada.ClienteAsignadoId)
        {
            return Unauthorized("No tienes permiso para cancelar este turno.");
        }

        if (appointment.Pago.Estado == "Cancelado" || appointment.Pago.Estado == "Reembolsado")
        {
            return BadRequest(new { message = "No se puede cancelar un turno cuyo pago ya está cancelado o reembolsado." });
        }

        appointment.Pago.Estado = "Cancelado";

        _context.Entry(appointment.Pago).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }


    // --- Servicios y Peluqueros (APIs para el cliente) ---

    // GET: api/ClientApi/services
    [HttpGet("services")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServices()
    {
        // ELIMINADO: el filtro por 's.EstaActivo'
        var services = await _context.Servicios
                                        .Select(s => new { s.Id, s.nombreServicio, s.Precio }) // Costo ahora es double
                                        .ToListAsync();
        return Ok(services);
    }

    // GET: api/ClientApi/groomers
    [HttpGet("groomers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetGroomers()
    {
        var groomers = await _context.Peluqueros
                                        .Include(g => g.AplicationUser)
                                        .Select(g => new { g.Id, Nombre = g.AplicationUser.Nombre })
                                        .ToListAsync();
        return Ok(groomers);
    }
}