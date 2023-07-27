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
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string ProductModel { get; set; }
        public string TypeOfFootwear { get; set; }
        public string Recipient { get; set; }
        public string Size { get; set; } 
        public string Color { get; set; }
        public int Productstock { get; set; }
        public int Price { get; set; }
        public string Discount { get; set; }

        // Навигационное свойство для связи с заказами
        [JsonIgnore]
        public ICollection<OrderItem> Orders { get; set; }
    }

}
