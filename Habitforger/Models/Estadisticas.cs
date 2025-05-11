using System.ComponentModel.DataAnnotations;

namespace Habitforger.Models
{
    public class Estadisticas
    {
        [Key]
        public int IdEstadistica { get; set; }
        public int NumHabitTotal { get; set; } = 0;
        public int NumHabitCompletados { get; set; } = 0;
        public int NumHabitActivos { get; set; } = 0;
        public int RachaMaxTotNum { get; set; } = 0;
        public string RachaMaxTotTitulo { get; set; }
        public int RachaMaxActNum { get; set; } = 0;
        public string RachaMaxActTitulo { get; set; }
        public float PorcentajeMayorAvance { get; set; } = 0;
        public string HabitoMayorAvance { get; set; }

        // Relación con Usuario
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
    }
}