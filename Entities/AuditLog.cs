using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public int RecordId { get; set; }
        //атрибут 'Column' для сокращения содержания строки
        [Column(TypeName = "datetime2")]
        public DateTime Timestamp { get; set; }
        public int? UserId { get; set; }

        // Навигационное свойство для связи с таблицей Users
        public UserItem User { get; set; }
    }
}
