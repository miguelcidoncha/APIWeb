using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Entities.Entities
{
    public class RolItem
    {
        public int RolId { get; set; }

        public string? RolName { get; set; }

        public virtual ICollection<UserItem>? User { get; set; }  //навигационное свойство
    }
}
