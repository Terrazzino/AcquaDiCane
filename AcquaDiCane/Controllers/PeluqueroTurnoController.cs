using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AcquaDiCane.Data;
using AcquaDiCane.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using AcquaDiCane.Models.Identity; // Para AplicationUser

namespace AcquaDiCane.Controllers
{
    [Authorize(Roles = "Peluquero")]
    [Route("api/[controller]")]
    [ApiController]
    public class PeluqueroTurnoController : ControllerBase
    {
        private readonly Contexto _context;
        private readonly UserManager<AplicationUser> _userManager;

        public PeluqueroTurnoController(Contexto context, UserManager<AplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Método auxiliar para obtener el ID del peluquero logueado como int
        private async Task<int?> GetLoggedInPeluqueroId()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return null; // Usuario no encontrado o no logueado
            }

            // Según la configuración de tu Contexto, AplicationUser tiene un PerfilPeluquero
            // y Peluquero tiene una FK a AplicationUser.
            // Para obtener el PeluqueroId, buscamos el Peluquero cuyo AplicationUserId coincida con el user.Id
            var peluquero = await _context.Peluqueros
                                        .FirstOrDefaultAsync(p => p.AplicationUserId == user.Id);

            if (peluquero == null)
            {
                return null; // El usuario de Identity no está asociado a un perfil de Peluquero
            }
            return peluquero.Id; // Devolvemos el ID de la entidad Peluquero
        }


        // GET: api/PeluqueroTurno/GetLoggedInGroomerName
        [HttpGet("GetLoggedInGroomerName")]
        public async Task<IActionResult> GetLoggedInGroomerName()
        {
            var peluqueroId = await GetLoggedInPeluqueroId();
            if (!peluqueroId.HasValue)
            {
                return Unauthorized();
            }

            var peluquero = await _context.Peluqueros
                                        .Include(p => p.AplicationUser)
                                        .FirstOrDefaultAsync(p => p.Id == peluqueroId.Value);

            if (peluquero == null)
            {
                return NotFound("Peluquero no encontrado.");
            }

            return Ok(new { name = peluquero.AplicationUser?.Nombre ?? "Peluquero" });
        }

        // GET: api/PeluqueroTurno/Availability?year=YYYY&month=MM
        [HttpGet("Availability")]
        public async Task<IActionResult> Availability(int year, int month)
        {
            var peluqueroId = await GetLoggedInPeluqueroId();
            if (!peluqueroId.HasValue)
            {
                return Unauthorized();
            }

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var turns = await _context.Turnos
                                    .Where(t => t.PeluqueroAsignadoId == peluqueroId.Value &&
                                                t.FechaHoraDelTurno.Year == year &&
                                                t.FechaHoraDelTurno.Month == month &&
                                                t.Estado != "Cancelado")                      // *** AJUSTADO: Comparación con string
                                    .Select(t => new { t.FechaHoraDelTurno.Day, t.Estado })
                                    .ToListAsync();

            var availability = new Dictionary<int, string>();

            for (int day = 1; day <= endDate.Day; day++)
            {
                var dayTurns = turns.Where(t => t.Day == day).ToList();
                if (dayTurns.Any(t => t.Estado == "Pendiente")) // *** AJUSTADO: Comparación con string
                {
                    availability[day] = "available";
                }
                else if (dayTurns.Any(t => t.Estado == "Completado")) // *** AJUSTADO: Comparación con string
                {
                    availability[day] = "completed_only";
                }
            }

            return Ok(availability);
        }

        // GET: api/PeluqueroTurno/AppointmentsByDate?date=YYYY-MM-DD
        [HttpGet("AppointmentsByDate")]
        public async Task<IActionResult> AppointmentsByDate(string date)
        {
            var peluqueroId = await GetLoggedInPeluqueroId();
            if (!peluqueroId.HasValue)
            {
                return Unauthorized();
            }

            if (!DateTime.TryParse(date, out DateTime selectedDate))
            {
                return BadRequest("Formato de fecha inválido.");
            }

            var appointments = await _context.Turnos
                                            .Where(t => t.PeluqueroAsignadoId == peluqueroId.Value &&
                                                        t.FechaHoraDelTurno.Date == selectedDate.Date)
                                            .Include(t => t.MascotaAsignada)
                                                .ThenInclude(m => m.ClienteAsignado)
                                                    .ThenInclude(c => c.AplicationUser)
                                            .Include(t => t.Detalles)
                                                .ThenInclude(d => d.ServicioAsignado)
                                            .OrderBy(t => t.FechaHoraDelTurno)
                                            .Select(t => new
                                            {
                                                id = t.Id,
                                                petName = t.MascotaAsignada.Nombre,
                                                petAvatarUrl = t.MascotaAsignada.UrlFotoPerfil,
                                                date = t.FechaHoraDelTurno.ToString("yyyy-MM-dd"),
                                                time = t.FechaHoraDelTurno.ToString("HH:mm"),
                                                serviceNames = t.Detalles.Select(d => d.ServicioAsignado.Nombre).ToList(),
                                                ownerName = t.MascotaAsignada.ClienteAsignado.AplicationUser.Nombre,
                                                ownerContact = t.MascotaAsignada.ClienteAsignado.AplicationUser.PhoneNumber,
                                                ownerEmail = t.MascotaAsignada.ClienteAsignado.AplicationUser.Email,
                                                status = t.Estado // Ya es string, no necesita .ToString()
                                            })
                                            .ToListAsync();

            return Ok(appointments);
        }

        // POST: api/PeluqueroTurno/CompleteAppointment
        [HttpPost("CompleteAppointment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteAppointment([FromBody] CompleteAppointmentRequest request)
        {
            var peluqueroId = await GetLoggedInPeluqueroId();
            if (!peluqueroId.HasValue)
            {
                return Unauthorized();
            }

            var turno = await _context.Turnos.FindAsync(request.AppointmentId);

            if (turno == null || turno.PeluqueroAsignadoId != peluqueroId.Value)
            {
                return NotFound("Turno no encontrado o no pertenece a este peluquero.");
            }

            if (turno.Estado == "Pendiente") // *** AJUSTADO: Comparación con string
            {
                turno.Estado = "Completado"; // *** AJUSTADO: Asignación de string
                turno.ObservacionesFinalizacion = request.Observations;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Turno completado exitosamente." });
            }
            else
            {
                return BadRequest("El turno ya ha sido completado o cancelado.");
            }
        }

        // POST: api/PeluqueroTurno/CancelAppointment
        [HttpPost("CancelAppointment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment([FromBody] CancelAppointmentRequest request)
        {
            var peluqueroId = await GetLoggedInPeluqueroId();
            if (!peluqueroId.HasValue)
            {
                return Unauthorized();
            }

            var turno = await _context.Turnos.FindAsync(request.AppointmentId);

            if (turno == null || turno.PeluqueroAsignadoId != peluqueroId.Value)
            {
                return NotFound("Turno no encontrado o no pertenece a este peluquero.");
            }

            if (turno.Estado == "Pendiente") // *** AJUSTADO: Comparación con string
            {
                turno.Estado = "Cancelado"; // *** AJUSTADO: Asignación de string
                turno.MotivoCancelacion = request.Reason;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Turno cancelado exitosamente." });
            }
            else
            {
                return BadRequest("El turno ya ha sido completado o cancelado.");
            }
        }

        public class CompleteAppointmentRequest
        {
            public int AppointmentId { get; set; }
            public string Observations { get; set; }
        }

        public class CancelAppointmentRequest
        {
            public int AppointmentId { get; set; }
            public string Reason { get; set; }
        }
    }
}