using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcquaDiCane.Models.Identity
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AplicationUser")]
        public int AplicationUserId { get; set; }
        public AplicationUser AplicationUser { get; set; }

        //ICollection => Favorece al rendimiento de Entity Framework
        public ICollection<Mascota> Mascotas { get; set; }

        //HashSet => Almacena los objetos desordenados, pero favorece al rendimiento de Entity Framework y no permite almacenar objetos duplicados
        public Cliente()
        {
            Mascotas = new HashSet<Mascota>();
        }
    }
}
