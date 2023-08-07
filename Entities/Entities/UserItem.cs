using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public class UserItem
    {
        public int UsuarioId { get; set; }

        [Required]
        public string? NombreUsuario { get; set; }

        [Required]
        public int? RolId { get; set; }

        [Required]
        public string? Contraseña { get; set; }

        [Required]
        public string? Email { get; set; }

        public float? Discont { get; set; }


        [JsonIgnore]
        public virtual ICollection<OrderItem>? Order { get; set; }      //один пользователь может иметь много заказов.

        [JsonIgnore]
        public virtual ICollection<OrderDetal>? OrderDetal { get; set; }

        [JsonIgnore]
        public virtual RolItem? UserRol { get; set; } // UserRol может быть связан только с одним User


    }
}
