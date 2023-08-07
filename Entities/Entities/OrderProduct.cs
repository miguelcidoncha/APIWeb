using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Entities
{
    public class OrderProduct

    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public UserItem? UserItem { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public OrderItem? Order { get; set; }

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public ProductItem? Product { get; set; }
    }
}
