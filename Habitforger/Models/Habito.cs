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
        [Display(Name = "Título del hábito")]
        public string TituloHabito { get; set; }

        [Display(Name = "Racha Actual")]
        public int RachaActual { get; set; } = 0;

        [Display(Name = "Maxima Racha")]
        public int MaximaRacha { get; set; } = 0;

        [Display(Name = "Días posibles")]
        public int DiasPosibles { get; set; } = 0;

        [Display(Name = "Días completados")]
        public int DiasCompletados { get; set; } = 0;

        [Display(Name = "Días fallados")]
        public int DiasFallados { get; set; } = 0;

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Objetivo (días)")]
        public int Objetivo { get; set; }

        [Required]
        public int Frecuencia { get; set; }

        [Display(Name = "Recuento Objetivo")]
        public int RecuentoObjetivo { get; set; } = 0;
        public float Progreso { get; set; } = 0;

        [Display(Name = "Objeto cumplido")]
        public bool ObjetivoCumplido { get; set; } = false;

        // Campos de control diario
        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "¿Hoy has respondido?")]
        public bool HoyRespondido { get; set; } = false;

        [Display(Name = "Última actualización")]
        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;

        [Display(Name = "Última respuesta")]
        public DateTime UltimaRespuestaFecha { get; set; } = DateTime.MinValue;

        [Display(Name = "¿Hacer privado?")]
        public bool HacerPrivado { get; set; } = false;

        // Relación con Usuario
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
    }
}