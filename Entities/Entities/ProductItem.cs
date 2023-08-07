using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Entities.Entities
{
    public class ProductItem
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? BrandName { get; set; }
        public int ProductStock { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderDetal>? OrderDetal { get; set; }    //один продукт может быть во многих заказах

    }

}
