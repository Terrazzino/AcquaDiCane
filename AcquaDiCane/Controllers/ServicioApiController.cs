// Controllers/Api/ServicioApiController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Necesario para .ToListAsync(), .FirstOrDefaultAsync(), etc.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcquaDiCane.Data; // Para tu Contexto
using AcquaDiCane.Models; // Para tus modelos Servicio y ServicioApiModel
using Microsoft.AspNetCore.Authorization; // Para el atributo [Authorize]

namespace AcquaDiCane.Web.Controllers // Ajusta el namespace si es diferente
{
    [Authorize(Roles = "Administrador")] // Solo los administradores pueden gestionar servicios
    [ApiController]
    [Route("api/[controller]")] // La ruta será /api/ServicioApi
    public class ServicioApiController : ControllerBase
    {
        private readonly Contexto _context; // Inyectamos el Contexto de la base de datos

        public ServicioApiController(Contexto context)
        {
            _context = context;
        }

        // ----------------------------------------
        // READ (GET) - Obtener todos los servicios
        // GET: api/ServicioApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicioApiModel>>> GetServicios()
        {
            // Proyectamos directamente de la entidad de DB 'Servicio' a tu DTO 'ServicioApiModel'
            var servicios = await _context.Servicios
                                        .OrderBy(s => s.Nombre)
                                        .Select(s => new ServicioApiModel
                                        {
                                            Id = s.Id,
                                            Nombre = s.Nombre, // Mapea nombreServicio a Nombre del DTO
                                            Descripcion = s.Descripcion,
                                            Precio = s.Precio,
                                            DuracionEnMinutos = s.DuracionEnMinutos, // Mapea Duracion a DuracionEnMinutos
                                        })
                                        .ToListAsync();
            return Ok(servicios);
        }

        // READ (GET) - Obtener un servicio por ID
        // GET: api/ServicioApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServicioApiModel>> GetServicio(int id)
        {
            var servicio = await _context.Servicios
                                        .Where(s => s.Id == id)
                                        .Select(s => new ServicioApiModel
                                        {
                                            Id = s.Id,
                                            Nombre = s.Nombre, // Mapea nombreServicio a Nombre del DTO
                                            Descripcion = s.Descripcion,
                                            Precio = s.Precio,
                                            DuracionEnMinutos = s.DuracionEnMinutos, // Mapea Duracion a DuracionEnMinutos
                                        })
                                        .FirstOrDefaultAsync();

            if (servicio == null)
            {
                return NotFound("Servicio no encontrado.");
            }
            return Ok(servicio);
        }

        // ----------------------------------------
        // CREATE (POST) - Crear un nuevo servicio
        // POST: api/ServicioApi
        [HttpPost]
        public async Task<ActionResult<ServicioApiModel>> PostServicio([FromBody] ServicioApiModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Mapeamos el DTO de entrada 'ServicioApiModel' a tu entidad de DB 'Servicio'
            var servicio = new Servicio
            {
                Nombre = model.Nombre, // Mapea Nombre del DTO a nombreServicio
                Descripcion = model.Descripcion,
                Precio = model.Precio,
                DuracionEnMinutos = model.DuracionEnMinutos, // Mapea DuracionEnMinutos a Duracion
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync(); // Guarda el nuevo servicio en la base de datos

            // Mapeamos la entidad guardada (que ahora tiene un Id de la DB) de vuelta al DTO para la respuesta
            var createdApiModel = new ServicioApiModel
            {
                Id = servicio.Id,
                Nombre = servicio.Nombre,
                Descripcion = servicio.Descripcion,
                Precio = servicio.Precio,
                DuracionEnMinutos = servicio.DuracionEnMinutos,
            };

            return CreatedAtAction(nameof(GetServicio), new { id = createdApiModel.Id }, createdApiModel);
        }

        // ----------------------------------------
        // UPDATE (PUT) - Actualizar un servicio existente
        // PUT: api/ServicioApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServicio(int id, [FromBody] ServicioApiModel model)
        {
            if (id != model.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del servicio en el cuerpo de la solicitud.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscamos la entidad de DB existente
            var servicio = await _context.Servicios.FirstOrDefaultAsync(s => s.Id == id);

            if (servicio == null)
            {
                return NotFound("Servicio no encontrado para actualizar.");
            }

            // Actualizamos las propiedades de la entidad de DB con los datos del DTO
            servicio.Nombre = model.Nombre; // Actualiza nombreServicio
            servicio.Descripcion = model.Descripcion;
            servicio.Precio = model.Precio;
            servicio.DuracionEnMinutos = model.DuracionEnMinutos; // Actualiza Duracion

            try
            {
                // No es estrictamente necesario marcar el estado como Modified si se accede a través del contexto
                // y se han modificado propiedades de un objeto ya trackeado. SaveChangesAsync lo detectaría.
                // Sin embargo, para mayor claridad o si el objeto viniera detached:
                _context.Entry(servicio).State = EntityState.Modified;
                await _context.SaveChangesAsync(); // Persiste los cambios en la base de datos
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Volver a lanzar si es otro tipo de error de concurrencia
                }
            }

            return NoContent(); // 204 No Content para una actualización exitosa
        }

        // ----------------------------------------
        // DELETE (DELETE) - Eliminar un servicio
        // DELETE: api/ServicioApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return NotFound("Servicio no encontrado para eliminar.");
            }

            // Buscar si hay algún DetalleDelTurno con este servicio en turnos futuros o no cancelados
            var estaEnUso = await _context.DetallesDeTurnos
                .Include(d => d.TurnoAsignado)
                .AnyAsync(d =>
                    d.ServicioAsignadoId == id &&
                    d.TurnoAsignado.Estado != "Cancelado" // Podés ajustar esta lógica
                );

            if (estaEnUso)
            {
                return Conflict("No se puede eliminar el servicio porque está asignado a al menos un turno activo o pendiente.");
            }

            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServicioExists(int id)
        {
            return _context.Servicios.Any(e => e.Id == id);
        }
    }
}