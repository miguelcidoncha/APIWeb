using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Entities.Entities
{
    public class OrderItem
    {
        public int OrderId { get; set; }
        public int OrderNumber { get; set; }
        public DateTime DateOrder { get; set; }
        public int UsuarioId { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderDetal>? OrderDetal { get; set; } //один заказ может иметь много делалей

        [JsonIgnore]
        public virtual UserItem? Users { get; set; }

    }
}
