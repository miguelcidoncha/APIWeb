using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class UserItem
    {
        public int IdUsuario { get; set; }

        [Required]
        public string NombreUsuario { get; set; }

        [Required]
        public string Contraseña { get; set; }

        [Required]
        public int Rol { get; set; }
    }
}
