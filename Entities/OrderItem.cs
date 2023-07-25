using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public int ProductId { get; set; } // Внешний ключ для связи с продуктом

        // Навигационное свойство для связи с продуктом
        //[ForeignKey("ProductId")]

        [JsonIgnore]
        public ProductItem Product { get; set; }

        public int Quantity { get; set; }
        // Другие свойства заказа, если есть
    }
}
