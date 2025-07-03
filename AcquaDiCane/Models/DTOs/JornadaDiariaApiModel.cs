using System;
using System.ComponentModel.DataAnnotations; // Necesario para los atributos de validación

namespace AcquaDiCane.Models.DTOs
{
    public class JornadaDiariaApiModel
    {
        public string Dia { get; set; }

        // Cambiado a string y añadido validación
        [Required(ErrorMessage = "La hora de inicio es requerida.")]
        [RegularExpression(@"^([01]\d|2[0-3]):([0-5]\d)$", ErrorMessage = "Formato de hora de inicio inválido. Use HH:mm (ej. 09:00).")]
        public string HoraInicio { get; set; }

        // Cambiado a string y añadido validación
        [Required(ErrorMessage = "La hora final es requerida.")]
        [RegularExpression(@"^([01]\d|2[0-3]):([0-5]\d)$", ErrorMessage = "Formato de hora final inválido. Use HH:mm (ej. 17:00).")]
        public string HoraFinal { get; set; }
    }
}