// AcquaDiCane.Models.DTOs/MascotaApiModel.cs
using System;

namespace AcquaDiCane.Models.DTOs
{
    public class MascotaApiModel
    {
        public int Id { get; set; }
        public string Name { get; set; } // Coincide con pet.name en JS
        public string Breed { get; set; } // Coincide con pet.breed en JS
        public string Size { get; set; } // Coincide con pet.size en JS
        public string Sex { get; set; } // Coincide con pet.sex en JS
        public double Weight { get; set; } // Coincide con pet.weight en JS
        public DateTime BirthDate { get; set; } // Coincide con pet.birthDate en JS
        public bool Castrated { get; set; } // Coincide con pet.castrated en JS
        public bool Allergies { get; set; } // Coincide con pet.allergies en JS (antes era Alergico)
        public string ProfilePicUrl { get; set; } // Coincide con pet.profilePicUrl en JS
        // No necesitas ClienteAsignadoId aquí si no lo vas a enviar al frontend
    }
}