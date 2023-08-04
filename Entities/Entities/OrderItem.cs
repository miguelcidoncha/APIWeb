using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Entities
{
    public class OrderItem
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public int ProductId { get; set; }
        public int ProductStock { get; set; }
        public DateTime DateOrder { get; set; }

    }
}
