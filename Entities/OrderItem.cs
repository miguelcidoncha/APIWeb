using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities
{
    public class OrderItem
    {
        public ushort IdOrder { get; set; }
        public string CustomerName { get; set; }
        public ushort ProductId { get; set; }

        [ForeignKey("ProductId")]

        [JsonIgnore]
        public ProductItem Product { get; set; }

        public int ProductStock { get; set; }
        public double TotalPrice { get; set; }
        public DateTime DateOrder { get; set; }
        public int OrderStatusId { get; set; }

        [ForeignKey("OrderStatusId")]

        [JsonIgnore]
        public OrderStatus OrderStatus { get; set; }
    }
}
