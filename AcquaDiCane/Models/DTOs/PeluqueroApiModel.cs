using System;
using System.Collections.Generic;

namespace AcquaDiCane.Models.DTOs
{
    public class PeluqueroApiModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DNI { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public bool EstaActivo { get; set; }

        public List<JornadaDiariaApiModel> JornadasSemanales { get; set; }
    }
}