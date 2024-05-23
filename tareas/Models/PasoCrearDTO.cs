using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;

namespace tareas.Models
{
    public class PasoCrearDTO
    {
        [Required]
        public string Descripcion { get; set; }
        public bool Realizado { get; set; }
    }
}
