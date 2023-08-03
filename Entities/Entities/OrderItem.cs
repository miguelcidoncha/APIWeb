using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Entities
{
    public class OrderItem
    {
        public int IdOrder { get; set; }
        public string CustomerName { get; set; }
        public int ProductId { get; set; }
        public int ProductStock { get; set; }
        public double TotalPrice { get; set; }
        public DateTime DateOrder { get; set; }
        public int OrderStatusId { get; set; }

        [ForeignKey("OrderStatusId")]

        [JsonIgnore]
        public OrderStatus OrderStatus { get; set; }

        [JsonIgnore]
        public ICollection<OrderProduct> OrderProduct { get; set; }

        public OrderItem()
        {
            // Инициализация коллекции OrderProduct
            OrderProduct = new List<OrderProduct>();
        }
    }
}
