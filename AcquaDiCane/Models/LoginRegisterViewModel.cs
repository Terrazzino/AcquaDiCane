// File: AcquaDiCane/Models/LoginRegisterViewModel.cs
namespace AcquaDiCane.Models // Asegúrate de que este sea el namespace correcto de tu proyecto
{
    public class LoginRegisterViewModel
    {
        public ViewModels.LoginViewModel Login { get; set; } // Asegúrate del namespace correcto para ViewModels
        public ViewModels.RegisterViewModel Register { get; set; } // Asegúrate del namespace correcto para ViewModels
    }
}