using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities
{
    public class OrderStatus
    {
        public int OrderStatusId { get; set; }
        public int StatusCode { get; set; }
        public string StatusName { get; set; }
    }
}
