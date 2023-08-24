using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LuxLens.Api.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo Correo de usuario es obligatorio.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo de usuario")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es obligatorio.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El campo Tipo de usuario es obligatorio.")]
        public string UserType { get; set; }
    }
}
