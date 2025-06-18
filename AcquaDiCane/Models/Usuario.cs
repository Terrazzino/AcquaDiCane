using System.Net.Mail;
using AcquaDiCane.Models.ClasesEnum;

namespace AcquaDiCane.Models
{
    public abstract class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Celular { get; set; }
        public string CorreoElectronico { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Contraseña { get; set; }
        public RolUsuario Rol {  get; set; }
    }
}
