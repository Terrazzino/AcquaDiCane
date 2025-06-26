using System.ComponentModel.DataAnnotations;
using System; // Agrega esto para DateTime

namespace AcquaDiCane.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "El formato del número de teléfono no es válido.")]
        public string PhoneNumber { get; set; }

        // AÑADE ESTA PROPIEDAD PARA EL DNI
        [Required(ErrorMessage = "El DNI es obligatorio.")] // Si es un campo que siempre debe llenarse
        [StringLength(10, MinimumLength = 7, ErrorMessage = "El DNI debe tener entre 7 y 10 dígitos.")] // Ajusta la longitud si es necesario
        public string DNI { get; set; } // <--- ¡Añadir esta línea!

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y un máximo de {1} caracteres de longitud.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        // Considera añadir una expresión regular aquí para mayor seguridad de la contraseña,
        // si tus políticas de Identity son más estrictas (ej. mayúsculas, números, símbolos).
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}