using System.ComponentModel.DataAnnotations;

namespace Habitforger.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [MaxLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        [Display(Name = "Nombre de usuario")] // <-- Esta línea es clave
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [MaxLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasena { get; set; }
    }
}
