using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities
{
    public class ProductItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; }

        // Навигационное свойство для связи с заказами
        [JsonIgnore]
        public ICollection<OrderItem> Orders { get; set; }
        public int Quantity { get; set; } // Добавленное поле для количества продуктов
        public string Manufacturer { get; set; } // Новое свойство для производителя
    }

}
