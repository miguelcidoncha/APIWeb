using Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Entities.Entities
{
    public class OrderDetal
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int UsuarioId { get; set; }
        public int ProductStock { get; set; }
        public int TotalPrice { get; set; }
        public virtual OrderItem? OrderItem { get; set; }
        public virtual ProductItem? ProductItem { get; set; }
        public virtual UserItem? UserItem { get; set; }

    }
}