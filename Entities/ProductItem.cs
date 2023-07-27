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
        public string BrandName { get; set; }
        public string ProductModel { get; set; }
        public string typeOfFootwear { get; set; }
        public string recipient { get; set; }
        public string size { get; set; } 
        public string color { get; set; }
        public int Productstock { get; set; }
        public int price { get; set; }
        public string discount { get; set; }

        // Навигационное свойство для связи с заказами
        [JsonIgnore]
        public ICollection<OrderItem> Orders { get; set; }
    }

}
