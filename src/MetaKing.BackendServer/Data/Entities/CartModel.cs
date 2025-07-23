using MetaKing.BackendServer.Data.Entities;
using MetaKing.BackendServer.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MetaKing.BackendServer.Data
{
    public class CartModel : IDateTracking
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }

        public int ProductVariantId { get; set; }
        [ForeignKey("ProductVariantId")]
        [JsonIgnore]
        public ProductVariantModel? ProductVariant { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string VersionName { get; set; }
        public string ColorName { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public UserModel? User { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
