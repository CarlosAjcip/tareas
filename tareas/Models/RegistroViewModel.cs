using System.ComponentModel.DataAnnotations;

namespace tareas.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo valido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
