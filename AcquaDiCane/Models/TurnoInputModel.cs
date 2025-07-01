// Models/TurnoInputModel.cs
using System;
using System.ComponentModel.DataAnnotations;

public class TurnoInputModel
{
    [Required]
    public int PetId { get; set; } // Nombre de propiedad para JS
    [Required]
    public DateTime Date { get; set; } // Fecha del turno
    [Required]
    public TimeSpan Time { get; set; } // Hora del turno
    [Required]
    public int GroomerId { get; set; } // Nombre de propiedad para JS
    [Required]
    public int ServiceId { get; set; } // Nombre de propiedad para JS (un solo servicio por ahora)
    public double Price { get; set; } // Precio total calculado en el frontend
    public string Status { get; set; } // "Pendiente"
    public int ClientId { get; set; } // ID del cliente
}