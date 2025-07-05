// Models/MascotaInputModel.cs (o ViewModels/MascotaViewModel.cs)
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Para IFormFile

public class MascotaInputModel
{
    public int? Id { get; set; } // Nullable para POST, no nullable para PUT
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Name { get; set; }
    public string Breed { get; set; } // No Required, ya que puede ser "Mestizo"
    public bool SinRaza { get; set; }
    [Required(ErrorMessage = "El tamaño es obligatorio.")]
    public string Size { get; set; }
    [Required(ErrorMessage = "El sexo es obligatorio.")]
    public string Sex { get; set; }
    [Required(ErrorMessage = "El peso es obligatorio.")]
    [Range(0.1, 100.0, ErrorMessage = "El peso debe estar entre 0.1 y 100 kg.")]
    public double Weight { get; set; }
    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }
    public bool Castrated { get; set; }
    public bool Allergies { get; set; }
    public IFormFile ProfilePic { get; set; } // Para la subida del archivo
    public int ClienteAsignadoId { get; set; } // Sigue siendo int
}