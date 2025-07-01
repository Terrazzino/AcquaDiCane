// Ejemplo: Models/InputModels/ChangePasswordInputModel.cs
using System.ComponentModel.DataAnnotations;

namespace AcquaDiCane.Models.InputModels // <--- ¡Este es el namespace que debes importar!
{
    public class ChangePasswordInputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y un máximo de {1} caracteres de longitud.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmNewPassword { get; set; }
    }
}