using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Habitforger.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Contrasena { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        // Relaciones
        public ICollection<Habito> Habitos { get; set; } = new List<Habito>();
        public ICollection<Logro> Logros { get; set; } = new List<Logro>();
        public ICollection<Estadisticas> Estadisticas { get; set; } = new List<Estadisticas>();
    }
}