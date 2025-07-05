using System;
using System.ComponentModel.DataAnnotations;

public class TurnoInputModel
{
    [Required]
    public int PetId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public TimeSpan Time { get; set; }

    [Required]
    public int GroomerId { get; set; }

    [Required]
    public int ServiceId { get; set; }

    // Eliminados:
    // public double Price { get; set; }
    // public string Status { get; set; }
    // public int ClientId { get; set; }
}
