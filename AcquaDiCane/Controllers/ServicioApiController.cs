// Controllers/Api/ServicioApiController.cs
// Este es un ejemplo básico. Necesitarás integrar tu lógica de base de datos aquí.
// Usaré una lista en memoria para este ejemplo, pero tú la reemplazarías con Entity Framework, Dapper, etc.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using AcquaDiCane.Models; // Asegúrate de que el namespace coincida con tu modelo

namespace AcquaDiCane.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Admin")] // Descomenta esto si usas autenticación y autorización por roles
    public class ServicioApiController : ControllerBase
    {
        // Simulación de una base de datos en memoria
        private static List<ServicioApiModel> _servicios = new List<ServicioApiModel>
        {
            new ServicioApiModel { Id = 1, Nombre = "Baño y Cepillado", Descripcion = "Baño completo y cepillado para eliminar nudos.", Precio = 15000m, DuracionEnMinutos = 60, EstaActivo = true },
            new ServicioApiModel { Id = 2, Nombre = "Corte de Pelo Estándar", Descripcion = "Corte de pelo según raza o a gusto del cliente.", Precio = 25000m, DuracionEnMinutos = 90, EstaActivo = true },
            new ServicioApiModel { Id = 3, Nombre = "Corte de Uñas", Descripcion = "Recorte profesional de uñas.", Precio = 5000m, DuracionEnMinutos = 20, EstaActivo = true }
        };
        private static int _nextId = _servicios.Max(s => s.Id) + 1; // Para generar IDs únicos

        // GET: api/ServicioApi
        [HttpGet]
        public ActionResult<IEnumerable<ServicioApiModel>> GetServicios()
        {
            return Ok(_servicios.OrderBy(s => s.Nombre).ToList());
        }

        // GET: api/ServicioApi/5
        [HttpGet("{id}")]
        public ActionResult<ServicioApiModel> GetServicio(int id)
        {
            var servicio = _servicios.FirstOrDefault(s => s.Id == id);
            if (servicio == null)
            {
                return NotFound("Servicio no encontrado.");
            }
            return Ok(servicio);
        }

        // POST: api/ServicioApi
        [HttpPost]
        public ActionResult<ServicioApiModel> PostServicio([FromBody] ServicioApiModel servicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            servicio.Id = _nextId++;
            // Aseguramos que el servicio sea activo por defecto al crear
            servicio.EstaActivo = true;
            _servicios.Add(servicio);
            return CreatedAtAction(nameof(GetServicio), new { id = servicio.Id }, servicio);
        }

        // PUT: api/ServicioApi/5
        [HttpPut("{id}")]
        public IActionResult PutServicio(int id, [FromBody] ServicioApiModel servicio)
        {
            if (id != servicio.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del servicio.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingServicio = _servicios.FirstOrDefault(s => s.Id == id);
            if (existingServicio == null)
            {
                return NotFound("Servicio no encontrado para actualizar.");
            }

            // Actualizar propiedades del servicio existente
            existingServicio.Nombre = servicio.Nombre;
            existingServicio.Descripcion = servicio.Descripcion;
            existingServicio.Precio = servicio.Precio;
            existingServicio.DuracionEnMinutos = servicio.DuracionEnMinutos;
            existingServicio.EstaActivo = servicio.EstaActivo; // Permite cambiar el estado activo

            return NoContent(); // 204 No Content para una actualización exitosa sin retorno de datos
        }

        // DELETE: api/ServicioApi/5
        [HttpDelete("{id}")]
        public IActionResult DeleteServicio(int id)
        {
            var servicio = _servicios.FirstOrDefault(s => s.Id == id);
            if (servicio == null)
            {
                return NotFound("Servicio no encontrado para eliminar.");
            }

            _servicios.Remove(servicio);
            return NoContent(); // 204 No Content para una eliminación exitosa
        }

    }

}