using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class ImageItem
    {
        public ushort IdImage { get; set; }
        public byte[] ImageData { get; set; } // BLOB

        [ForeignKey("ProductId")]
        public ushort ProductId { get; set; }
        public ProductItem Product { get; set; }
    }
}

