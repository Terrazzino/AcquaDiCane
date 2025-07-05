using System.ComponentModel.DataAnnotations;

public class PetUpdateDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Name { get; set; } // Coincide con name="Name" en el HTML

    public string Breed { get; set; } // Coincide con name="Breed"
    [Required(ErrorMessage = "El tamaño es obligatorio.")]
    public string Size { get; set; } // Coincide con name="Size"
    [Required(ErrorMessage = "El sexo es obligatorio.")]
    public string Sex { get; set; } // Coincide con name="Sex"
    [Required(ErrorMessage = "El peso es obligatorio.")]
    [Range(0.1, 500.0, ErrorMessage = "El peso debe ser un número positivo.")]
    public double Weight { get; set; } // Coincide con name="Weight"
    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; } // Coincide con name="BirthDate"

    public bool Castrated { get; set; } // Coincide con name="Castrated"
    public bool Allergies { get; set; } // Coincide con name="Allergies"

    // Campo para la imagen, si la vas a recibir como IFormFile
    public IFormFile? ProfilePic { get; set; }
}

