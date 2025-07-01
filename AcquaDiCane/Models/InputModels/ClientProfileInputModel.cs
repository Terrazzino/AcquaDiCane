// Ejemplo: Models/InputModels/ClientProfileInputModel.cs
namespace AcquaDiCane.Models.InputModels // <--- ¡Este es el namespace que debes importar!
{
    public class ClientProfileInputModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        // No incluyas Email aquí si no lo vas a permitir modificar o ya lo manejas con UserManager
        // public DateTime? BirthDate { get; set; } // Si permites editar la fecha de nacimiento
    }
}