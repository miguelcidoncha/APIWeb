using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class AuditLog
    {
        public ushort IdLog { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public int RecordId { get; set; }
        //Atributo "Columna" para acortar el contenido de una fila
        [Column(TypeName = "datetime2")]
        public DateTime Timestamp { get; set; }
        public int? UserId { get; set; }

        // Propiedad de navegación para enlazar con la tabla Usuarios
        public UserItem User { get; set; }
    }
}
