using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Entities.Entities
{
    public class OrderItem
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int ProductStock { get; set; }
        public DateTime DateOrder { get; set; }
        public int UsuarioId { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderProduct>? OrderProduct { get; set; }

        [JsonIgnore]
        public virtual UserItem? Users { get; set; }

    }
}
