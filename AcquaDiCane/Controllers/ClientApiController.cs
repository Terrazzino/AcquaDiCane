// Controllers/ClientApiController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AcquaDiCane.Data; // Para tu Contexto
using AcquaDiCane.Models; // Para tus modelos Mascota, Turno, Servicio, Peluquero, Cliente, DetalleDelTurno, Pago
using AcquaDiCane.Models.DTOs; // Para tus modelos Mascota, Turno, Servicio, Peluquero, Cliente, DetalleDelTurno, Pago
using AcquaDiCane.Models.Identity; // Para tu AplicationUser
using AcquaDiCane.Models.InputModels;
// Si los InputModels están en AcquaDiCane.Models, ya estaría cubierta por la línea de arriba.
// Pero si están en una carpeta específica como "InputModels", debes especificarlo.

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IO;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

// ... el resto de tu código

[Authorize(Roles = "Cliente")]
[ApiController]
[Route("api/[controller]")] // La ruta será /api/ClientApi
public class ClientApiController : ControllerBase
{
    private readonly Contexto _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly UserManager<AplicationUser> _userManager; // Si vas a usarlo para ChangePassword

    // Constructor modificado para inyectar IWebHostEnvironment y UserManager (si se usa)
    public ClientApiController(Contexto context, IWebHostEnvironment webHostEnvironment, UserManager<AplicationUser> userManager) // <-- Incluye UserManager aquí si lo vas a usar
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _userManager = userManager; // Asigna la inyección
    }

    // Método auxiliar para obtener el perfil de cliente del usuario logueado
    private async Task<Cliente> GetClienteProfileAsync()
    {
        var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserIdString == null)
        {
            return null; // Usuario no autenticado o ID no encontrado en los claims
        }

        // MODIFICACION CLAVE: Intentar parsear el ID del usuario a INT
        if (!int.TryParse(currentUserIdString, out int currentUserIdInt))
        {
            // Esto ocurrirá si el ID de AplicationUser no es un int parseable (ej. si fuera GUID).
            // Si esto sucede, significa que tu AplicationUser.Id NO es int, o que el ClaimTypes.NameIdentifier
            // no está proporcionando el ID de usuario como un string parseable a int.
            // Es CRÍTICO revisar la configuración de Identity en tu proyecto si esto falla.
            Console.WriteLine($"Error crítico: No se pudo parsear el AplicationUser ID '{currentUserIdString}' a INT.");
            return null; // ID de usuario no válido o formato incorrecto
        }

        // Buscar el Cliente cuyo AplicationUserId coincida con el ID entero parseado
        var clienteProfile = await _context.Clientes
                                            .FirstOrDefaultAsync(c => c.AplicationUserId == currentUserIdInt); // <-- Uso de int
        return clienteProfile;
    }

    // --- Mascotas ---

    // GET: api/ClientApi/pets
    [HttpGet("pets")]
    public async Task<ActionResult<IEnumerable<MascotaApiModel>>> GetPetsForClient(int clientId)
    {
        // Obtén el usuario autenticado para asegurarte de que el clientId que se pide es el del usuario logueado
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized("Usuario no autenticado."); // Debería ser 401 si no hay token
        }

        // El clientId que viene del frontend es el ApplicationUserId (string)
        if (clientId != currentUser.Id)
        {
            return Forbid("No tiene permiso para ver las mascotas de otro cliente."); // 403 Forbidden
        }

        // Busca el cliente real en tu tabla Clientes usando el ApplicationUserId
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.AplicationUserId == currentUser.Id);
        if (cliente == null)
        {
            return NotFound("Perfil de cliente no encontrado para el usuario actual.");
        }

        // Ahora, busca las mascotas asociadas con el ID del Cliente (int) de tu entidad Cliente
        var mascotas = await _context.Mascotas
                                    .Where(m => m.ClienteAsignadoId == cliente.Id) // <-- Usar ClienteAsignadoId
                                    .Select(m => new MascotaApiModel // Necesitas definir MascotaApiModel
                                    {
                                        Id = m.Id,
                                        Name = m.Nombre, // Asegúrate de que los nombres de propiedades coincidan con el frontend (pet.name)
                                        Breed = m.Raza,
                                        Size = m.Tamaño,
                                        Sex = m.Sexo,
                                        Weight = m.Peso,
                                        BirthDate = m.FechaNacimiento,
                                        Castrated = m.Castrado,
                                        Allergies = m.Alergico, // Asegúrate que coincida con pet.allergies
                                        ProfilePicUrl = m.UrlFotoPerfil // Asegúrate que coincida con pet.profilePicUrl
                                    })
                                    .ToListAsync();

        return Ok(mascotas);
    }
    // GET: api/ClientApi/pets/5 (para editar mascota)
    [HttpGet("pets/{id}")]
    public async Task<IActionResult> GetPet(int id)
    {
        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual o usuario no autenticado.");
        }

        var mascota = await _context.Mascotas
                                     .FirstOrDefaultAsync(p => p.Id == id && p.ClienteAsignadoId == clienteProfile.Id);
        if (mascota == null)
        {
            return NotFound("Mascota no encontrada o no pertenece al usuario.");
        }

        return Ok(new
        {
            id = mascota.Id,
            name = mascota.Nombre,
            size = mascota.Tamaño,
            sex = mascota.Sexo,
            breed = mascota.Raza,
            sinRaza = mascota.SinRaza,
            weight = mascota.Peso,
            castrated = mascota.Castrado,
            birthDate = mascota.FechaNacimiento.ToString("yyyy-MM-dd"),
            allergies = mascota.Alergico,
            profilePicUrl = mascota.UrlFotoPerfil
        });
    }

    // POST: api/ClientApi/pets
    [HttpPost("pets")]
    [ValidateAntiForgeryToken] // Añade esto si tu frontend envía el token y lo quieres validar
    public async Task<IActionResult> AddPet([FromForm] MascotaCreateModel model) // <-- [FromForm] para datos multipart/form-data
    {
        // Obtener el usuario autenticado para vincular la mascota
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized("Usuario no autenticado.");
        }

        // Obtener el perfil de Cliente asociado al AplicationUser
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.AplicationUserId == currentUser.Id);
        if (cliente == null)
        {
            return BadRequest("No se encontró un perfil de cliente asociado al usuario autenticado.");
        }

        if (!ModelState.IsValid)
        {
            // Gracias a la configuración en Program.cs, esto devolverá el JSON de errores.
            return BadRequest(ModelState);
        }

        string profilePicUrl = null;
        if (model.ProfilePic != null && model.ProfilePic.Length > 0)
        {
            // Guarda la imagen en un directorio wwwroot/uploads/pets o similar
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "pets");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePic.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProfilePic.CopyToAsync(fileStream);
            }
            profilePicUrl = $"/uploads/pets/{uniqueFileName}"; // URL relativa para acceder desde el frontend
        }

        var nuevaMascota = new Mascota
        {
            Nombre = model.Name, // Mapea del DTO al modelo de base de datos
            Raza = model.Breed,
            Tamaño = model.Size,
            Sexo = model.Sex,
            Peso = model.Weight,
            FechaNacimiento = model.BirthDate,
            Castrado = model.Castrated,
            Alergico = model.Allergies, // Tu entidad Mascota tiene 'Alergico', no 'Allergies'
            ClienteAsignadoId = cliente.Id, // Asigna el ID del cliente (int)
            UrlFotoPerfil = profilePicUrl
        };

        _context.Mascotas.Add(nuevaMascota);
        await _context.SaveChangesAsync();

        // Puedes devolver la mascota recién creada o un DTO de confirmación
        var mascotaCreadaDto = new MascotaApiModel
        {
            Id = nuevaMascota.Id,
            Name = nuevaMascota.Nombre,
            Breed = nuevaMascota.Raza,
            Size = nuevaMascota.Tamaño,
            Sex = nuevaMascota.Sexo,
            Weight = nuevaMascota.Peso,
            BirthDate = nuevaMascota.FechaNacimiento,
            Castrated = nuevaMascota.Castrado,
            Allergies = nuevaMascota.Alergico,
            ProfilePicUrl = nuevaMascota.UrlFotoPerfil
        };

        return CreatedAtAction(nameof(GetPet), new { id = nuevaMascota.Id }, mascotaCreadaDto);
    }

    // PUT: api/ClientApi/pets/5
    [HttpPut("pets/{id}")]
    [ValidateAntiForgeryToken] // Añade esto si tu frontend envía el token y lo quieres validar
    public async Task<IActionResult> UpdatePet(int id, [FromForm] MascotaInputModel model)
    {
        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual o usuario no autenticado.");
        }

        var existingPet = await _context.Mascotas.FindAsync(id);
        if (existingPet == null)
        {
            return NotFound("Mascota no encontrada.");
        }

        if (existingPet.ClienteAsignadoId != clienteProfile.Id)
        {
            return Unauthorized("No tienes permiso para actualizar esta mascota.");
        }

        // El MascotaInputModel contiene los campos a actualizar
        // No es necesario revalidar ModelState aquí si ya lo haces al inicio de la acción
        // y asumes que el modelo de entrada es válido.
        // Si hay validaciones específicas del lado del servidor que dependan del estado
        // del objeto existente, se harían aquí.
        // Pero para el caso de FromForm, ModelState.IsValid ya debería haber sido evaluado al inicio.

        // Actualizar propiedades
        existingPet.Nombre = model.Name;
        existingPet.Raza = model.SinRaza ? "Mestizo" : model.Breed;
        existingPet.SinRaza = model.SinRaza;
        existingPet.Tamaño = model.Size;
        existingPet.Sexo = model.Sex;
        existingPet.Peso = model.Weight;
        existingPet.FechaNacimiento = model.BirthDate;
        existingPet.Castrado = model.Castrated;
        existingPet.Alergico = model.Allergies;

        // Manejar actualización de foto de perfil
        if (model.ProfilePic != null && model.ProfilePic.Length > 0)
        {
            // Eliminar foto anterior si no es la por defecto
            if (!string.IsNullOrEmpty(existingPet.UrlFotoPerfil) && !existingPet.UrlFotoPerfil.Contains("default-pet.png"))
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingPet.UrlFotoPerfil.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "pet_profiles");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ProfilePic.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProfilePic.CopyToAsync(fileStream);
            }
            existingPet.UrlFotoPerfil = $"/uploads/pet_profiles/{uniqueFileName}";
        }
        else if (string.IsNullOrEmpty(existingPet.UrlFotoPerfil))
        {
            existingPet.UrlFotoPerfil = "/img/default-pet.png";
        }

        _context.Entry(existingPet).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Mascotas.Any(e => e.Id == id))
            {
                return NotFound("Mascota no encontrada después de intentar actualizar.");
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/ClientApi/pets/5
    [HttpDelete("pets/{id}")]
    [ValidateAntiForgeryToken] // Añade esto si tu frontend envía el token y lo quieres validar
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

        // Eliminar foto asociada si no es la por defecto
        if (!string.IsNullOrEmpty(pet.UrlFotoPerfil) && !pet.UrlFotoPerfil.Contains("default-pet.png"))
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, pet.UrlFotoPerfil.TrimStart('/'));
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

    // GET: api/ClientApi/appointments
    [HttpGet("appointments")]
    public async Task<IActionResult> GetAppointments()
    {
        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual o usuario no autenticado.");
        }

        var appointments = await _context.Turnos
                                         .Include(a => a.MascotaAsignada)
                                         .Include(a => a.PeluqueroAsignado)
                                             .ThenInclude(p => p.AplicationUser)
                                         .Include(a => a.Detalles)
                                             .ThenInclude(dt => dt.ServicioAsignado)
                                         .Include(a => a.Pago)
                                         .Where(a => a.MascotaAsignada.ClienteAsignadoId == clienteProfile.Id)
                                         .OrderByDescending(a => a.FechaHoraDelTurno)
                                         .Select(a => new
                                         {
                                             id = a.Id,
                                             date = a.FechaHoraDelTurno.ToString("yyyy-MM-dd"),
                                             time = a.FechaHoraDelTurno.ToString(@"HH\:mm"),
                                             groomerName = a.PeluqueroAsignado.AplicationUser.Nombre,
                                             serviceName = string.Join(", ", a.Detalles.Select(dt => dt.ServicioAsignado.Nombre).ToList()),
                                             price = a.PrecioTotal,
                                             status = a.Pago.Estado,
                                             petName = a.MascotaAsignada.Nombre
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
                id = a.Id,
                date = a.FechaHoraDelTurno.ToString("yyyy-MM-dd"),
                time = a.FechaHoraDelTurno.ToString(@"HH\:mm"),
                groomerName = a.PeluqueroAsignado.AplicationUser.Nombre,
                serviceName = string.Join(", ", a.Detalles.Select(dt => dt.ServicioAsignado.Nombre).ToList()),
                price = a.PrecioTotal,
                status = a.Pago.Estado,
                petName = a.MascotaAsignada.Nombre
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
    [ValidateAntiForgeryToken] // Añade esto si tu frontend envía el token y lo quieres validar
    public async Task<IActionResult> ScheduleAppointment([FromBody] TurnoInputModel model)
    {
        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual o usuario no autenticado.");
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();
            return BadRequest(new { errors = errors, message = "Datos del turno inválidos." });
        }

        var mascotaAsignada = await _context.Mascotas.FirstOrDefaultAsync(p => p.Id == model.PetId && p.ClienteAsignadoId == clienteProfile.Id);
        if (mascotaAsignada == null)
        {
            ModelState.AddModelError("PetId", "La mascota seleccionada no existe o no te pertenece.");
        }

        var groomer = await _context.Peluqueros.FirstOrDefaultAsync(g => g.Id == model.GroomerId);
        if (groomer == null)
        {
            ModelState.AddModelError("GroomerId", "El peluquero seleccionado no es válido.");
        }

        var servicio = await _context.Servicios.FirstOrDefaultAsync(s => s.Id == model.ServiceId);
        if (servicio == null)
        {
            ModelState.AddModelError("ServiceId", "El servicio seleccionado no es válido.");
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();
            return BadRequest(new { errors = errors, message = "Validación fallida en el servidor." });
        }

        DateTime fechaHoraCompleta = model.Date.Date + model.Time;
        if (fechaHoraCompleta < DateTime.Now)
        {
            ModelState.AddModelError("FechaHora", "No se puede agendar un turno en el pasado.");
            var errors = ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();
            return BadRequest(new { errors = errors, message = "Fecha/hora de turno inválida." });
        }

        // --- Validación adicional: ¿El peluquero está disponible a esa hora? ---
        // Asumiendo que un turno tiene una duración fija o se puede obtener del servicio.
        // Asegúrate de que tu modelo Servicio tenga una propiedad 'DuracionMinutos'.
        TimeSpan duracionTurno = TimeSpan.FromMinutes(servicio.DuracionEnMinutos > 0 ? servicio.DuracionEnMinutos : 60);

        var turnoFinaliza = fechaHoraCompleta.Add(duracionTurno);

        var peluqueroOcupado = await _context.Turnos
            .Where(t => t.PeluqueroAsignadoId == model.GroomerId &&
                        t.FechaHoraDelTurno < turnoFinaliza && // Un turno existente empieza antes de que el nuevo termine
                        t.FechaHoraDelTurno.Add(TimeSpan.FromMinutes(t.Detalles.Sum(dt => dt.ServicioAsignado.DuracionEnMinutos > 0 ? dt.ServicioAsignado.DuracionEnMinutos : 60))) > fechaHoraCompleta && // Y el turno existente termina después de que el nuevo empieza
                        t.Pago.Estado != "Cancelado" // No considerar turnos cancelados
                        )
            .AnyAsync();

        if (peluqueroOcupado)
        {
            ModelState.AddModelError("Disponibilidad", "El peluquero no está disponible en la fecha y hora seleccionadas.");
            var errors = ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();
            return BadRequest(new { errors = errors, message = "Peluquero no disponible." });
        }
        // --- Fin de validación de disponibilidad ---


        // Crear el Pago
        var pago = new Pago
        {
            Estado = "Pendiente",
            Monto = servicio.Precio,
            FechaPago = null
        };
        _context.Pagos.Add(pago);

        // Crear el Turno
        var turno = new Turno
        {
            MascotaAsignadaId = model.PetId,
            PeluqueroAsignadoId = model.GroomerId,
            FechaHoraDelTurno = fechaHoraCompleta,
            PrecioTotal = servicio.Precio,
            Pago = pago,
            Detalles = new List<DetalleDelTurno>
            {
                new DetalleDelTurno
                {
                    ServicioAsignadoId = model.ServiceId,
                    PrecioServicio = servicio.Precio,
                }
            }
        };

        _context.Turnos.Add(turno);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAppointments), new { message = "Turno y pago asociados agendados con éxito." });
    }


    // PUT: api/ClientApi/appointments/5/cancel
    [HttpPut("appointments/{id}/cancel")]
    [ValidateAntiForgeryToken] // Añade esto si tu frontend envía el token y lo quieres validar
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
        var services = await _context.Servicios
                                     .Select(s => new { s.Id, s.Nombre, s.Precio, s.DuracionEnMinutos })
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
                                     .Select(g => new { g.Id, Name = g.AplicationUser.Nombre })
                                     .ToListAsync();
        return Ok(groomers);
    }

    // --- Perfil del Cliente ---
    // GET: api/ClientApi/clientprofile
    [HttpGet("clientprofile")]
    public async Task<IActionResult> GetClientProfile()
    {
        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return NotFound("Perfil de cliente no encontrado.");
        }

        var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == clienteProfile.AplicationUserId);
        if (appUser == null)
        {
            return StatusCode(500, "Usuario de aplicación asociado no encontrado.");
        }

        return Ok(new
        {
            id = clienteProfile.Id,
            name = clienteProfile.AplicationUser.Nombre,
            lastName = clienteProfile.AplicationUser.Apellido,
            email = appUser.Email,
            phoneNumber = appUser.PhoneNumber,
            birthDate = clienteProfile.AplicationUser.FechaNacimiento.HasValue ? clienteProfile.AplicationUser.FechaNacimiento.Value.ToString("yyyy-MM-dd") : null
        });
    }

    // PUT: api/ClientApi/clientprofile/
    [HttpPut("clientprofile")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateClientProfile([FromBody] ClientProfileInputModel model)
    {
        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual.");
        }

        var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == clienteProfile.AplicationUserId);
        if (appUser == null)
        {
            return StatusCode(500, "Usuario de aplicación asociado no encontrado.");
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { errors = errors, message = "Datos de perfil inválidos." });
        }

        clienteProfile.AplicationUser.Nombre = model.Name;
        clienteProfile.AplicationUser.Apellido = model.LastName;

        appUser.PhoneNumber = model.PhoneNumber;

        _context.Entry(clienteProfile).State = EntityState.Modified;
        _context.Entry(appUser).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Clientes.Any(e => e.Id == clienteProfile.Id))
            {
                return NotFound("Perfil de cliente no encontrado después de intentar actualizar.");
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/ClientApi/clientprofile/change-password
    [HttpPost("clientprofile/change-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordInputModel model)
    {
        var clienteProfile = await GetClienteProfileAsync();
        if (clienteProfile == null)
        {
            return Unauthorized("Perfil de cliente no encontrado para el usuario actual.");
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { errors = errors, message = "Datos de cambio de contraseña inválidos." });
        }

        // Obtener el ApplicationUser por su ID
        var appUser = await _userManager.FindByIdAsync(clienteProfile.AplicationUserId.ToString()); // Convertir a string para FindByIdAsync si el UserManager espera string, aunque el Id sea int
        if (appUser == null)
        {
            return StatusCode(500, "Usuario de aplicación asociado no encontrado.");
        }

        // Lógica real de cambio de contraseña con UserManager<AplicationUser>
        // Si AplicationUser.Id es int, _userManager.FindByIdAsync(string id) puede requerir el ToString()
        // o usar _userManager.GetUserAsync(User) para obtener el usuario directamente del contexto.
        var changePasswordResult = await _userManager.ChangePasswordAsync(appUser, model.OldPassword, model.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            var errors = changePasswordResult.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { errors = errors, message = "Error al cambiar la contraseña." });
        }

        return Ok(new { message = "Contraseña cambiada con éxito." });
    }
}