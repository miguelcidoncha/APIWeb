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
        public ushort ProductId { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string ProductModel { get; set; }
        public string TypeOfFootwear { get; set; }
        public string Recipient { get; set; }
        public string ProductSize { get; set; }
        public string ProductColor { get; set; }
        public int ProductStock { get; set; }
        public double ProductPrice { get; set; }
        public double Discount { get; set; }

        // Propiedad de navegación para la comunicación de órdenes
        [JsonIgnore]
        public ICollection<OrderItem> Orders { get; set; }
    }

}
