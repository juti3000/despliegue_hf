using System.ComponentModel.DataAnnotations;

namespace Habitforger.Models
{
    public class Logro
    {
        [Key]
        public int IdLogro { get; set; }
        public string NombreLogro { get; set; }
        public string Descripcion { get; set; }
        public bool Completado { get; set; } = false;

        // Relación con Usuario
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
    }
}