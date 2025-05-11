// Models/Habito.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Habitforger.Models
{
    public class Habito
    {
        [Key]
        public int IdHabito { get; set; }

        [Required]
        public string TituloHabito { get; set; }
        public int RachaActual { get; set; } = 0;
        public int MaximaRacha { get; set; } = 0;
        public int DiasPosibles { get; set; } = 0;
        public int DiasCompletados { get; set; } = 0;
        public int DiasFallados { get; set; } = 0;

        [Required]
        [Range(1, int.MaxValue)]
        public int Objetivo { get; set; }

        [Required]
        public int Frecuencia { get; set; }
        public int RecuentoObjetivo { get; set; } = 0;
        public float Progreso { get; set; } = 0;
        public bool ObjetivoCumplido { get; set; } = false;

        // Campos de control diario
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool HoyRespondido { get; set; } = false;
        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;
        public DateTime UltimaRespuestaFecha { get; set; } = DateTime.MinValue;
        public bool HacerPrivado { get; set; } = false;

        // Relación con Usuario
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
    }
}