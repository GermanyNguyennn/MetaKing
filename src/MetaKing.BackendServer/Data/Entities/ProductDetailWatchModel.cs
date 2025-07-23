using MetaKing.BackendServer.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MetaKing.BackendServer.Data.Entities
{
    public class ProductDetailWatchModel : IDateTracking
    {
        [Key]
        public int Id { get; set; }
        public string? ScreenTechnology { get; set; } // Công nghệ màn hình
        public string? FaceDiameter { get; set; } // Đường kính mặt
        public string? SuitableWristSize { get; set; } // Kích thước cổ tay phù hợp
        public string? ListenCall { get; set; } // Nghe. gọi
        public string? HealthBenefits { get; set; } // Tiện ích sức khoẻ
        public string? Compatible { get; set; } // Tương thích
        public string? BatteryLife { get; set; } // Thời lượng pin
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [JsonIgnore]
        public ProductModel? Product { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public CategoryModel? Category { get; set; }
        public int BrandId { get; set; }
        [ForeignKey("BrandId")]
        [JsonIgnore]
        public BrandModel? Brand { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        [JsonIgnore]
        public CompanyModel? Company { get; set; }
    }
}
