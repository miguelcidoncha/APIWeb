using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entities
{
    public class ImageItem
    {
        public int IdImage { get; set; }
        public byte[] ImageData { get; set; } // BLOB

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public ProductItem Product { get; set; }
    }
}

