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

        [JsonIgnore]
        public ICollection<OrderItem> Orders { get; set; }
        public int Quantity { get; set; } 
        public string Manufacturer { get; set; } 
    }

}
