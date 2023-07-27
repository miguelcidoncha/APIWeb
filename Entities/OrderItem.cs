using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities
{
    public class OrderItem
    {
        public int IdOrder { get; set; }
        public string CustomerName { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]

        [JsonIgnore]
        public ProductItem Product { get; set; }

        public int Productstock { get; set; }
    }
}
