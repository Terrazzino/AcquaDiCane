using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcquaDiCane.Models.Identity
{
    public class Peluquero
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AplicationUser")]
        public int AplicationUserId { get; set; }
        public AplicationUser AplicationUser { get; set; }

        //ICollection => Favorece al rendimiento de Entity Framework
        public ICollection<JornadaDiaria> JornadaSemanal { get; set; }
        public ICollection<Turno> Turnos { get; set; }

        public void AgregarDisponibilidad(JornadaDiaria jornada)
        {
            var jor = JornadaSemanal.FirstOrDefault(x => jornada.Dia == x.Dia);
            if (jor == null)
            {
                JornadaSemanal.Add(jornada);
            }
        }

        //HashSet => Almacena los objetos desordenados, pero favorece al rendimiento de Entity Framework y no permite almacenar objetos duplicados
        public Peluquero()
        {
            JornadaSemanal = new HashSet<JornadaDiaria>();
            Turnos = new HashSet<Turno>();
        }
    }
}
