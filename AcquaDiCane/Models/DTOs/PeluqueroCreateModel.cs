using System.ComponentModel.DataAnnotations;

namespace AcquaDiCane.Models.DTOs
{
    public class PeluqueroCreateModel
    {
        // Propiedades para AplicationUser
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los {1} caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder los {1} caracteres.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(20, ErrorMessage = "El DNI no puede exceder los {1} caracteres.")]
        public string DNI { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        [StringLength(255, ErrorMessage = "El email no puede exceder los {1} caracteres.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "El formato del número de teléfono no es válido.")]
        [StringLength(20, ErrorMessage = "El número de teléfono no puede exceder los {1} caracteres.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos {2} y como máximo {1} caracteres.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } // Contraseña para el nuevo usuario

        // Colección para las jornadas diarias
        [Required(ErrorMessage = "Debe configurar al menos una jornada semanal.")]
        [MinLength(1, ErrorMessage = "Debe configurar al menos una jornada semanal.")]
        public ICollection<JornadaDiariaApiModel> JornadasSemanales { get; set; } = new List<JornadaDiariaApiModel>();
    }
}
