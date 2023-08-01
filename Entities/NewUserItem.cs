using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities
{
    public class NewUserRequest
    {
        [Required]
        public string NombreUsuario { get; set; }

        [Required]
        public string Contraseña { get; set; }

        [Required]
        public string Email { get; set; }
        public UserItem ToUserItem()
        {
            var userItem = new UserItem();

            userItem.IdRol = 2;
            userItem.NombreUsuario = NombreUsuario;
            userItem.Contraseña = Contraseña;
            userItem.Email = Email;

            return userItem;
        }

        [JsonIgnore]
        public List<AuditLog> AuditLogs { get; set; }

    }
}