using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MetaKing.BackendServer.Data.Interfaces;
using System.Text.Json.Serialization;

namespace MetaKing.BackendServer.Data.Entities
{
    public class ProductVariantModel : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [JsonIgnore]
        public ProductModel? Product { get; set; }

        public int VersionId { get; set; }
        [ForeignKey("VersionId")]
        [JsonIgnore]
        public VersionModel? Version { get; set; }

        public int ColorId { get; set; }
        [ForeignKey("ColorId")]
        [JsonIgnore]
        public ColorModel? Color { get; set; }

        public decimal ImportPrice { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<OrderDetailModel> OrderDetails { get; set; } = new List<OrderDetailModel>();

    }
}
