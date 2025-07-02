using System;
using System.Collections.Generic;
namespace AcquaDiCane.Models.DTOs
{
    public class JornadaDiariaApiModel
    {
                public string Dia { get; set; }
        public string HoraInicio { get; set; } // string para el frontend (HH:mm)
        public string HoraFinal { get; set; }  // string para el frontend (HH:mm)
    }
}
