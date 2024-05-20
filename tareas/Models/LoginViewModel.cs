using System.ComponentModel.DataAnnotations;

namespace tareas.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Error.Requerido")]
        [EmailAddress(ErrorMessage = "Error.Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Error.Requerido")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Recuerdame")]
        public bool Recuerdame { get; set; }
    }
}
