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

        public string name { get; set; }
        public string brand { get; set; }
        public string sneakerModel { get; set; }
        public string typeOfFootwear { get; set; }
        public string recipient { get; set; }
        public string size { get; set; } 
        public string color { get; set; }
        public string stock { get; set; }
        public string price { get; set; }
        public string discount { get; set; }

    }

}
