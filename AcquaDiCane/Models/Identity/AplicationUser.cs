using Microsoft.AspNetCore.Identity;

namespace AcquaDiCane.Models.Identity
{
    public class AplicationUser:IdentityUser<int>
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public Cliente PerfilCliente { get; set; }
        public Peluquero PerfilPeluquero { get; set; }
    }
}
